using System;
using System.IO;
using NVorbis;

namespace Sonic4Episode1.Tests;

public class BgmStreamTests
{
    private const string LoopingTrack = "S4EP1_40_SuperSonic_Mixed4816";
    private const string OneShotTrack = "S4EP1_24_1up_Mixed4816";

    [Fact]
    public void IntroDecodesToItsDeclaredLength()
    {
        using var stream = Open(OneShotTrack, false);
        var declared = stream.IntroSamples * stream.Channels;
        var buffer = new float[4096];

        long total = 0;
        int read;
        while ((read = stream.Read(buffer, buffer.Length)) > 0)
            total += read;

        Assert.Equal(declared, total);
    }

    [Fact]
    public void AOneShotStreamStopsAtTheEndOfTheIntro()
    {
        using var stream = Open(OneShotTrack, false);
        Drain(stream);

        Assert.False(stream.InLoop);
        Assert.Equal(0, stream.Read(new float[4096], 4096));
    }

    [Fact]
    public void TheIntroHandsOverToTheStartOfTheLoop()
    {
        var expected = FirstSamplesOf(LoopingTrack, "Loop", 1024);

        using var stream = Open(LoopingTrack, true);
        var introFloats = stream.IntroSamples * stream.Channels;
        SkipExactly(stream, introFloats);

        Assert.False(stream.InLoop);

        var actual = new float[expected.Length];
        Assert.Equal(actual.Length, stream.Read(actual, actual.Length));
        Assert.True(stream.InLoop);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TheLoopWrapsBackToItsOwnFirstSample()
    {
        var expected = FirstSamplesOf(LoopingTrack, "Loop", 1024);

        using var stream = Open(LoopingTrack, true);
        var upToWrap = (stream.IntroSamples + stream.LoopSamples) * stream.Channels;
        SkipExactly(stream, upToWrap);

        var actual = new float[expected.Length];
        Assert.Equal(actual.Length, stream.Read(actual, actual.Length));
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ALoopingStreamKeepsProducingSamples()
    {
        using var stream = Open(LoopingTrack, true);
        var buffer = new float[4096];
        var wanted = (stream.IntroSamples + stream.LoopSamples * 2) * stream.Channels;

        long total = 0;
        while (total < wanted)
        {
            var read = stream.Read(buffer, buffer.Length);
            Assert.True(read > 0);
            total += read;
        }
    }

    [Fact]
    public void RewindReturnsToTheStartOfTheIntro()
    {
        var expected = FirstSamplesOf(LoopingTrack, "Intro", 1024);

        using var stream = Open(LoopingTrack, true);
        SkipExactly(stream, (stream.IntroSamples + 1024) * stream.Channels);
        Assert.True(stream.InLoop);

        stream.Rewind();
        Assert.False(stream.InLoop);

        var actual = new float[expected.Length];
        Assert.Equal(actual.Length, stream.Read(actual, actual.Length));
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReadsAlwaysLandOnAWholeFrame()
    {
        using var stream = Open(LoopingTrack, true);
        var buffer = new float[4097];

        for (int i = 0; i < 16; ++i)
            Assert.Equal(0, stream.Read(buffer, buffer.Length) % stream.Channels);
    }

    private static AppMain.CriAuBgmStream Open(string track, bool loop)
    {
        var root = SoundRoot();
        return new AppMain.CriAuBgmStream(
            File.OpenRead(Path.Combine(root, track, "Intro.ogg")),
            loop ? File.OpenRead(Path.Combine(root, track, "Loop.ogg")) : null);
    }

    private static float[] FirstSamplesOf(string track, string part, int count)
    {
        using var file = File.OpenRead(Path.Combine(SoundRoot(), track, part + ".ogg"));
        using var reader = new VorbisReader(file, true);

        var buffer = new float[count];
        int total = 0;
        while (total < count)
        {
            var read = reader.ReadSamples(buffer, total, count - total);
            if (read == 0)
                break;

            total += read;
        }

        Assert.Equal(count, total);
        return buffer;
    }

    private static void Drain(AppMain.CriAuBgmStream stream)
    {
        var buffer = new float[4096];
        while (stream.Read(buffer, buffer.Length) > 0)
        {
        }
    }

    private static void SkipExactly(AppMain.CriAuBgmStream stream, long floats)
    {
        var buffer = new float[4096];
        while (floats > 0)
        {
            var want = (int)Math.Min(buffer.Length, floats);
            var read = stream.Read(buffer, want);
            Assert.True(read > 0);
            floats -= read;
        }
    }

    private static string SoundRoot()
    {
        var here = Path.GetDirectoryName(typeof(BgmStreamTests).Assembly.Location);
        var root = Path.GetFullPath(Path.Combine(here, "..", "..", "..", "..", "Content.Build", "desktop-hd-dxt", "SOUND"));

        if (!Directory.Exists(root))
            throw Xunit.Sdk.SkipException.ForSkip("Content.Build/desktop-hd-dxt is not present");

        return root;
    }
}
