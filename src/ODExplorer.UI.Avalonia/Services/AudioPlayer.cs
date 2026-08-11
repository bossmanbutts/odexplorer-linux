using System;
using System.Runtime.InteropServices;
using ODExplorer.Audio;

namespace ODExplorer.UI.Avalonia.Services
{
    // Minimal RIFF/WAVE reader: extracts PCM (8/16/24/32-bit) or IEEE-float
    // (32-bit) audio for playback. Returns null for anything structural or
    // unsupported so the player can silently skip the sound.
    public static class WavReader
    {
        public sealed record WavData(int Format, ushort Channels, int BitsPerSample, uint SampleRate, byte[] Data)
        {
            public bool IsPcm => Format == 1;
            public bool IsFloat => Format == 3;
        }

        public static WavData? Read(byte[] bytes)
        {
            if (bytes is null || bytes.Length < 12)
                return null;
            if (bytes[0] != (byte)'R' || bytes[1] != (byte)'I' || bytes[2] != (byte)'F' || bytes[3] != (byte)'F')
                return null;
            if (bytes[8] != (byte)'W' || bytes[9] != (byte)'A' || bytes[10] != (byte)'V' || bytes[11] != (byte)'E')
                return null;

            int format = 0, bits = 0;
            ushort channels = 0;
            uint rate = 0;
            byte[]? data = null;

            int offset = 12;
            while (offset + 8 <= bytes.Length)
            {
                uint chunkSize = (uint)(bytes[offset + 4] | (bytes[offset + 5] << 8) | (bytes[offset + 6] << 16) | (bytes[offset + 7] << 24));
                long payloadStart = offset + 8;
                if (chunkSize > (uint)(bytes.Length - payloadStart) || chunkSize > int.MaxValue)
                    return null;

                string id = new string(new[] { (char)bytes[offset], (char)bytes[offset + 1], (char)bytes[offset + 2], (char)bytes[offset + 3] });

                switch (id)
                {
                    case "fmt ":
                        if (chunkSize < 16)
                            return null;
                        format = bytes[payloadStart] | (bytes[payloadStart + 1] << 8);
                        channels = (ushort)(bytes[payloadStart + 2] | (bytes[payloadStart + 3] << 8));
                        rate = (uint)(bytes[payloadStart + 4] | (bytes[payloadStart + 5] << 8) | (bytes[payloadStart + 6] << 16) | (bytes[payloadStart + 7] << 24));
                        bits = bytes[payloadStart + 14] | (bytes[payloadStart + 15] << 8);
                        break;
                    case "data":
                        data = bytes[(int)payloadStart..(int)(payloadStart + chunkSize)];
                        break;
                }

                offset = (int)(payloadStart + chunkSize + (chunkSize & 1)); // RIFF chunks are word-aligned
            }

            if (format is not (1 or 3))
                return null;
            if (channels is not (1 or 2))
                return null;
            if (rate == 0)
                return null;
            if (bits is not (8 or 16 or 24 or 32))
                return null;
            if (data is null || data.Length == 0)
                return null;

            return new WavData(format, channels, bits, rate, data);
        }
    }

    // IAudioPlayer over PulseAudio's simple API (libpulse-simple) instead of
    // shelling out to paplay/aplay. PulseAudio/PipeWire is the standard Linux
    // desktop audio stack; if the library or server is unavailable playback is
    // silently skipped (same graceful behaviour as the old shell fallback).
    public class AudioPlayer : IAudioPlayer
    {
        private const int PaStreamPlayback = 1;
        private const int PaSampleU8 = 0;
        private const int PaSampleS16Le = 3;
        private const int PaSampleFloat32Le = 5;
        private const int PaSampleS32Le = 7;
        private const int PaSampleS24Le = 9;

        private readonly object _gate = new();
        private System.Threading.CancellationTokenSource? _cts;
        private System.Threading.Tasks.Task? _current;

        public bool IsPlaying
        {
            get
            {
                lock (_gate)
                    return _current is { IsCompleted: false };
            }
        }

        public void Play(string filePath)
        {
            lock (_gate)
            {
                _cts?.Cancel();
                var cts = new System.Threading.CancellationTokenSource();
                _cts = cts;
                _current = System.Threading.Tasks.Task.Run(() => PlayInternal(filePath, cts.Token));
            }
        }

        public void Stop()
        {
            lock (_gate)
                _cts?.Cancel();
        }

        // Exposed so tests can probe whether a PulseAudio server is reachable.
        public static bool CanConnect()
        {
            try
            {
                var spec = new PaSampleSpec { Format = PaSampleS16Le, Rate = 8000, Channels = 1 };
                int error = 0;
                IntPtr stream = pa_simple_new(null, "ODExplorer", PaStreamPlayback, null, "connectivity probe", ref spec, IntPtr.Zero, IntPtr.Zero, out error);
                if (stream == IntPtr.Zero)
                    return false;
                pa_simple_free(stream);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void PlayInternal(string filePath, System.Threading.CancellationToken token)
        {
            try
            {
                var wav = WavReader.Read(System.IO.File.ReadAllBytes(filePath));
                if (wav is null)
                    return;

                var spec = new PaSampleSpec { Format = ToSampleFormat(wav), Rate = wav.SampleRate, Channels = (byte)wav.Channels };

                int error = 0;
                IntPtr stream = pa_simple_new(null, "ODExplorer", PaStreamPlayback, null, "fleet carrier timer", ref spec, IntPtr.Zero, IntPtr.Zero, out error);
                if (stream == IntPtr.Zero)
                    return;

                try
                {
                    ReadOnlySpan<byte> remaining = wav.Data;
                    const int chunkBytes = 64 * 1024;
                    while (remaining.Length > 0)
                    {
                        if (token.IsCancellationRequested)
                        {
                            pa_simple_flush(stream, out error);
                            return;
                        }
                        var chunk = remaining[..Math.Min(chunkBytes, remaining.Length)];
                        pa_simple_write(stream, chunk.ToArray(), (nuint)chunk.Length, out error);
                        remaining = remaining[chunk.Length..];
                    }
                    pa_simple_drain(stream, out error);
                }
                finally
                {
                    pa_simple_free(stream);
                }
            }
            catch
            {
                // library/server unavailable or unparseable file: skip silently
            }
        }

        private static int ToSampleFormat(WavReader.WavData wav)
        {
            if (wav.IsFloat && wav.BitsPerSample == 32)
                return PaSampleFloat32Le;
            return wav.BitsPerSample switch
            {
                8 => PaSampleU8,
                24 => PaSampleS24Le,
                32 => PaSampleS32Le,
                _ => PaSampleS16Le
            };
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PaSampleSpec
        {
            public int Format;
            public uint Rate;
            public byte Channels;
        }

        [DllImport("libpulse-simple.so.0", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr pa_simple_new(string? server, string name, int dir, string? device,
            string streamName, ref PaSampleSpec sampleSpec, IntPtr attr, IntPtr map, out int error);

        [DllImport("libpulse-simple.so.0", CallingConvention = CallingConvention.Cdecl)]
        private static extern int pa_simple_write(IntPtr s, byte[] data, nuint bytes, out int error);

        [DllImport("libpulse-simple.so.0", CallingConvention = CallingConvention.Cdecl)]
        private static extern int pa_simple_drain(IntPtr s, out int error);

        [DllImport("libpulse-simple.so.0", CallingConvention = CallingConvention.Cdecl)]
        private static extern int pa_simple_flush(IntPtr s, out int error);

        [DllImport("libpulse-simple.so.0", CallingConvention = CallingConvention.Cdecl)]
        private static extern void pa_simple_free(IntPtr s);
    }
}
