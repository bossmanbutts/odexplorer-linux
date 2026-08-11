using System;
using System.IO;
using NUnit.Framework;
using ODExplorer.UI.Avalonia.Services;

namespace ODExplorer.UI.Avalonia.Tests;

public class WavReaderTests
{
    [Test]
    public void Reads_16bit_stereo_pcm()
    {
        byte[] pcm = WavBuilder.Sine(440, 0.1, 44100, channels: 2);
        byte[] wav = WavBuilder.Riff(WavBuilder.Fmt(2, 16, 44100), WavBuilder.Data(pcm));

        var result = WavReader.Read(wav);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Format, Is.EqualTo(1));
        Assert.That(result.IsPcm, Is.True);
        Assert.That(result.Channels, Is.EqualTo(2));
        Assert.That(result.BitsPerSample, Is.EqualTo(16));
        Assert.That(result.SampleRate, Is.EqualTo(44100));
        Assert.That(result.Data, Is.EqualTo(pcm));
    }

    [TestCase(8)]
    [TestCase(24)]
    [TestCase(32)]
    public void Reads_pcm_at_any_supported_bit_depth(int bits)
    {
        byte[] pcm = WavBuilder.Sine(440, 0.05, 22050, channels: 1, bits: bits);
        byte[] wav = WavBuilder.Riff(WavBuilder.Fmt(1, bits, 22050), WavBuilder.Data(pcm));

        var result = WavReader.Read(wav);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.BitsPerSample, Is.EqualTo(bits));
    }

    [Test]
    public void Reads_ieee_float32()
    {
        byte[] pcm = WavBuilder.Sine(440, 0.05, 16000, bits: 32);
        byte[] wav = WavBuilder.Riff(WavBuilder.Fmt(1, 32, 16000, format: 3), WavBuilder.Data(pcm));

        var result = WavReader.Read(wav);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Format, Is.EqualTo(3));
        Assert.That(result.IsFloat, Is.True);
        Assert.That(result.BitsPerSample, Is.EqualTo(32));
    }

    [Test]
    public void Tolerates_extra_fmt_chunk_bytes()
    {
        // WAVE_FORMAT_EXTENSIBLE carries 18+2 bytes in fmt; the reader must not
        // require exactly 16.
        byte[] pcm = WavBuilder.Sine(440, 0.05, 44100);
        byte[] wav = WavBuilder.Riff(WavBuilder.Fmt(1, 16, 44100, extraBytes: 2), WavBuilder.Data(pcm));

        var result = WavReader.Read(wav);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.SampleRate, Is.EqualTo(44100));
    }

    [Test]
    public void Skips_intermediate_chunks_between_fmt_and_data()
    {
        byte[] pcm = WavBuilder.Sine(440, 0.05, 44100);
        byte[] wav = WavBuilder.Riff(
            WavBuilder.Fmt(1, 16, 44100),
            WavBuilder.Chunk("fact", new byte[] { 1, 0, 0, 0 }),
            WavBuilder.Chunk("JUNK", new byte[] { 0, 0, 0, 0 }),
            WavBuilder.Data(pcm));

        var result = WavReader.Read(wav);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Data, Is.EqualTo(pcm));
    }

    [Test]
    public void Handles_odd_sized_data_chunk_with_padding()
    {
        byte[] pcm = new byte[] { 1, 2, 3, 4, 5 }; // 5 bytes -> pad byte follows
        byte[] wav = WavBuilder.Riff(WavBuilder.Fmt(1, 8, 8000), WavBuilder.Data(pcm));

        var result = WavReader.Read(wav);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Data, Is.EqualTo(pcm));
    }

    [Test]
    public void Returns_null_for_garbage()
    {
        Assert.That(WavReader.Read(Array.Empty<byte>()), Is.Null);
        Assert.That(WavReader.Read(new byte[] { 1, 2, 3 }), Is.Null);
        Assert.That(WavReader.Read(new byte[] { (byte)'R', (byte)'I', (byte)'F', (byte)'F' }), Is.Null);
        Assert.That(WavReader.Read(System.Text.Encoding.ASCII.GetBytes("this is not a wave file at all!")), Is.Null);
    }

    [Test]
    public void Returns_null_for_truncated_header()
    {
        byte[] wav = WavBuilder.Riff(WavBuilder.Fmt(1, 16, 44100), WavBuilder.Data(new byte[8]));
        byte[] truncated = wav[..16];

        Assert.That(WavReader.Read(truncated), Is.Null);
    }

    [Test]
    public void Returns_null_when_data_chunk_overflows_file()
    {
        // Hand-build a data chunk claiming more bytes than are present.
        var buffer = WavBuilder.Riff(WavBuilder.Fmt(1, 16, 44100));
        using var ms = new MemoryStream();
        ms.Write(buffer, 0, buffer.Length);
        byte[] header = { (byte)'d', (byte)'a', (byte)'t', (byte)'a', 0xFF, 0xFF, 0xFF, 0x7F };
        ms.Write(header, 0, header.Length);

        Assert.That(WavReader.Read(ms.ToArray()), Is.Null);
    }

    [Test]
    public void Returns_null_for_unsupported_formats()
    {
        // format 2 = ADPCM
        byte[] pcm = new byte[16];
        byte[] wav = WavBuilder.Riff(WavBuilder.Fmt(1, 16, 44100, format: 2), WavBuilder.Data(pcm));

        Assert.That(WavReader.Read(wav), Is.Null);
    }

    [Test]
    public void Returns_null_for_more_than_two_channels()
    {
        byte[] pcm = new byte[32];
        byte[] wav = WavBuilder.Riff(WavBuilder.Fmt(6, 16, 44100), WavBuilder.Data(pcm));

        Assert.That(WavReader.Read(wav), Is.Null);
    }

    [Test]
    public void Returns_null_for_zero_sample_rate()
    {
        byte[] pcm = new byte[16];
        byte[] wav = WavBuilder.Riff(WavBuilder.Fmt(1, 16, 0), WavBuilder.Data(pcm));

        Assert.That(WavReader.Read(wav), Is.Null);
    }

    [Test]
    public void Returns_null_for_empty_data_chunk()
    {
        byte[] wav = WavBuilder.Riff(WavBuilder.Fmt(1, 16, 44100), WavBuilder.Data(Array.Empty<byte>()));

        Assert.That(WavReader.Read(wav), Is.Null);
    }
}
