using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ODExplorer.UI.Avalonia.Tests;

// Builds valid RIFF/WAVE byte buffers for the WavReader / AudioPlayer tests.
public static class WavBuilder
{
    public static byte[] Riff(params (string Id, byte[] Payload)[] chunks)
    {
        int riffSize = 4;
        foreach (var (_, p) in chunks)
            riffSize += 8 + p.Length + (p.Length & 1);

        using var ms = new MemoryStream();
        WriteAscii(ms, "RIFF");
        ms.Write(U32((uint)riffSize), 0, 4);
        WriteAscii(ms, "WAVE");
        foreach (var (id, p) in chunks)
        {
            WriteAscii(ms, id);
            ms.Write(U32((uint)p.Length), 0, 4);
            ms.Write(p);
            if ((p.Length & 1) == 1)
                ms.WriteByte(0); // RIFF chunks are word-aligned
        }
        return ms.ToArray();
    }

    public static (string Id, byte[] Payload) Fmt(ushort channels, int bits, uint rate, int format = 1, int extraBytes = 0)
    {
        int byteRate = (int)(rate * channels * bits / 8);
        int blockAlign = channels * bits / 8;
        var p = new List<byte>();
        p.AddRange(U16((ushort)format));
        p.AddRange(U16(channels));
        p.AddRange(U32(rate));
        p.AddRange(U32((uint)byteRate));
        p.AddRange(U16((ushort)blockAlign));
        p.AddRange(U16((ushort)bits));
        p.AddRange(new byte[extraBytes]);
        return ("fmt ", p.ToArray());
    }

    public static (string Id, byte[] Payload) Data(byte[] pcm) => ("data", pcm);

    public static (string Id, byte[] Payload) Chunk(string id, byte[] payload) => (id, payload);

    public static byte[] Sine(double freq, double seconds, uint rate, ushort channels = 1, int bits = 16, double amplitude = 0.5)
    {
        int sampleCount = (int)(rate * seconds);
        int bytesPerSample = bits / 8;
        var pcm = new byte[sampleCount * channels * bytesPerSample];
        for (int i = 0; i < sampleCount; i++)
        {
            double v = amplitude * Math.Sin(2 * Math.PI * freq * i / rate);
            for (int c = 0; c < channels; c++)
            {
                int idx = (i * channels + c) * bytesPerSample;
                WriteSample(pcm, idx, bits, v);
            }
        }
        return pcm;
    }

    // Writes one signed sample at the given bit depth (8-bit becomes unsigned
    // PCM with a 128 offset, matching the RIFF PCM convention).
    private static void WriteSample(byte[] dst, int idx, int bits, double v)
    {
        switch (bits)
        {
            case 8:
                dst[idx] = (byte)((v * 127) + 128);
                break;
            case 24:
                int s24 = (int)(v * (1 << 23));
                dst[idx] = (byte)s24;
                dst[idx + 1] = (byte)(s24 >> 8);
                dst[idx + 2] = (byte)(s24 >> 16);
                break;
            default: // 16 and 32 share the LE signed layout
                int s = (int)(v * (1 << (bits - 1)));
                for (int b = 0; b < bits / 8; b++)
                    dst[idx + b] = (byte)(s >> (8 * b));
                break;
        }
    }

    private static void WriteAscii(Stream s, string text)
    {
        var b = Encoding.ASCII.GetBytes(text);
        s.Write(b, 0, b.Length);
    }

    private static byte[] U16(ushort v) => new[] { (byte)v, (byte)(v >> 8) };
    private static byte[] U32(uint v) => new[] { (byte)v, (byte)(v >> 8), (byte)(v >> 16), (byte)(v >> 24) };
}
