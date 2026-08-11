using System;
using System.IO;
using System.Threading;
using NUnit.Framework;
using ODExplorer.UI.Avalonia.Services;

namespace ODExplorer.UI.Avalonia.Tests;

// Live playback through PulseAudio. Skipped (assumed-away) on machines without
// a reachable audio server, so CI stays green while real desktops get coverage.
public class AudioPlayerTests
{
    [Test]
    public void Plays_generated_wav_through_pulseaudio()
    {
        Assume.That(AudioPlayer.CanConnect(), Is.True, "no PulseAudio server reachable");

        byte[] wav = WavBuilder.Riff(
            WavBuilder.Fmt(2, 16, 44100),
            WavBuilder.Data(WavBuilder.Sine(440, 1.5, 44100, channels: 2)));

        string path = Path.Combine(Path.GetTempPath(), $"odexplorer_audio_test_{Guid.NewGuid():N}.wav");
        File.WriteAllBytes(path, wav);

        try
        {
            var player = new AudioPlayer();
            player.Play(path);

            Thread.Sleep(400);
            Assert.That(player.IsPlaying, Is.True, "the clip should still be playing 0.4 s in");

            player.Stop();
            var deadline = DateTime.UtcNow.AddSeconds(3);
            while (player.IsPlaying && DateTime.UtcNow < deadline)
                Thread.Sleep(50);

            Assert.That(player.IsPlaying, Is.False, "Stop() cancels the playback");
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public void Play_with_garbage_file_silently_noops()
    {
        string path = Path.Combine(Path.GetTempPath(), $"odexplorer_audio_bad_{Guid.NewGuid():N}.wav");
        File.WriteAllBytes(path, new byte[] { 0, 1, 2, 3, 4 });

        try
        {
            var player = new AudioPlayer();
            Assert.DoesNotThrow(() => player.Play(path));
            Thread.Sleep(200);
            Assert.That(player.IsPlaying, Is.False);
        }
        finally
        {
            File.Delete(path);
        }
    }
}
