using System;
using System.Collections.Generic;
using Sonic4Episode1.Content;

namespace Sonic4Episode1.Tests;

// covers the pair no single ordered list can satisfy: a DXT1 under desktop-sd has to stay put, and
// a PNG under desktop-hd has to be encoded, even though both profiles have png in play
public class TexturePlanTests
{
    private static readonly IReadOnlyList<TextureFormat> Nothing = Array.Empty<TextureFormat>();
    private static readonly IReadOnlyList<TextureFormat> Bc = new[] { TextureFormat.Bc1, TextureFormat.Bc3 };
    private static readonly IReadOnlyList<TextureFormat> ToPng = new[] { TextureFormat.Png };

    private static IReadOnlySet<TextureFormat> Accept(params TextureFormat[] formats)
    {
        return new HashSet<TextureFormat>(formats);
    }

    private static ImageInfo Source(TextureFormat format, int width = 64, int height = 32)
    {
        return new ImageInfo { Format = format, Width = width, Height = height, MipLevels = 1 };
    }

    private static TexturePlan Plan(ImageInfo source, IReadOnlyList<TextureFormat> encode,
        IReadOnlySet<TextureFormat> accept, AlphaShape alpha, out string problem)
    {
        return TexturePlanner.Choose(source, () => alpha, encode, accept, out problem);
    }

    [Fact]
    public void IdentityProfileNeverChangesAnything()
    {
        var accept = Accept(TextureFormat.Png, TextureFormat.Bc1);

        foreach (var format in new[] { TextureFormat.Png, TextureFormat.Bc1 })
        {
            var plan = Plan(Source(format), Nothing, accept, AlphaShape.Opaque, out var problem);

            Assert.Null(problem);
            Assert.Equal(format, plan.Target);
        }
    }

    [Theory]
    [InlineData(AlphaShape.Opaque, TextureFormat.Bc1)]
    [InlineData(AlphaShape.PunchThrough, TextureFormat.Bc1)]
    [InlineData(AlphaShape.Partial, TextureFormat.Bc3)]
    public void AlphaShapeChoosesBetweenBc1AndBc3(AlphaShape alpha, TextureFormat expected)
    {
        var plan = Plan(Source(TextureFormat.Png), Bc, Accept(TextureFormat.Bc1, TextureFormat.Bc3,
            TextureFormat.Png), alpha, out var problem);

        Assert.Null(problem);
        Assert.Equal(expected, plan.Target);
    }

    [Theory]
    [InlineData(TextureFormat.Bc1)]
    [InlineData(TextureFormat.Bc3)]
    public void AlreadyCompressedPassesThrough(TextureFormat format)
    {
        var plan = Plan(Source(format), Bc, Accept(TextureFormat.Bc1, TextureFormat.Bc3),
            AlphaShape.Partial, out var problem);

        Assert.Null(problem);
        Assert.Equal(format, plan.Target);
    }

    [Fact]
    public void Bc2MatchesNoEncodeTargetAndFallsToAccept()
    {
        var plan = Plan(Source(TextureFormat.Bc2), Bc,
            Accept(TextureFormat.Bc1, TextureFormat.Bc2, TextureFormat.Bc3), AlphaShape.Partial,
            out var problem);

        Assert.Null(problem);
        Assert.Equal(TextureFormat.Bc2, plan.Target);
    }

    [Fact]
    public void Bc2WithoutAcceptIsAProblem()
    {
        Plan(Source(TextureFormat.Bc2), Bc, Accept(TextureFormat.Bc1, TextureFormat.Bc3),
            AlphaShape.Partial, out var problem);

        Assert.NotNull(problem);
        Assert.Contains("bc2", problem);
    }

    [Theory]
    [InlineData(TextureFormat.Bgra32, AlphaShape.Opaque, TextureFormat.Bc1)]
    [InlineData(TextureFormat.Bgra32, AlphaShape.Partial, TextureFormat.Bc3)]
    [InlineData(TextureFormat.L8, AlphaShape.Opaque, TextureFormat.Bc1)]
    public void UncompressedDdsIsEncoded(TextureFormat source, AlphaShape alpha, TextureFormat expected)
    {
        var plan = Plan(Source(source), Bc, Accept(TextureFormat.Bc1, TextureFormat.Bc3),
            alpha, out var problem);

        Assert.Null(problem);
        Assert.Equal(expected, plan.Target);
    }

    // BC works on 4x4 blocks; anything else would need padding that shifts the UVs
    [Fact]
    public void NonBlockAlignedFallsBackToAccept()
    {
        var plan = Plan(Source(TextureFormat.Png, width: 62, height: 30), Bc,
            Accept(TextureFormat.Bc1, TextureFormat.Bc3, TextureFormat.Png), AlphaShape.Opaque,
            out var problem);

        Assert.Null(problem);
        Assert.Equal(TextureFormat.Png, plan.Target);
    }

    [Fact]
    public void NonBlockAlignedWithoutPngAcceptedIsAProblem()
    {
        Plan(Source(TextureFormat.Png, width: 62, height: 30), Bc,
            Accept(TextureFormat.Bc1, TextureFormat.Bc3), AlphaShape.Opaque, out var problem);

        Assert.NotNull(problem);
    }

    [Fact]
    public void EncodingToPngDecodesAnythingBlockCompressed()
    {
        var plan = Plan(Source(TextureFormat.Bc1), ToPng, Accept(TextureFormat.Png),
            AlphaShape.Opaque, out var problem);

        Assert.Null(problem);
        Assert.Equal(TextureFormat.Png, plan.Target);
    }

    [Fact]
    public void PngUnderAPngProfileIsUntouched()
    {
        var plan = Plan(Source(TextureFormat.Png), ToPng, Accept(TextureFormat.Png),
            AlphaShape.Partial, out var problem);

        Assert.Null(problem);
        Assert.Equal(TextureFormat.Png, plan.Target);
    }

    [Fact]
    public void TheOrderingCounterexample()
    {
        var sdAccept = Accept(TextureFormat.Png, TextureFormat.Bc1);
        var hdAccept = Accept(TextureFormat.Png, TextureFormat.Bc1, TextureFormat.Bc3);

        var sdDxt = Plan(Source(TextureFormat.Bc1), Nothing, sdAccept, AlphaShape.Opaque, out _);
        var sdPng = Plan(Source(TextureFormat.Png), Nothing, sdAccept, AlphaShape.Opaque, out _);
        var hdPng = Plan(Source(TextureFormat.Png), Bc, hdAccept, AlphaShape.Opaque, out _);

        Assert.Equal(TextureFormat.Bc1, sdDxt.Target);
        Assert.Equal(TextureFormat.Png, sdPng.Target);
        Assert.Equal(TextureFormat.Bc1, hdPng.Target);
    }

    [Fact]
    public void UnknownSourceIsAProblemRatherThanAGuess()
    {
        var source = new ImageInfo { Format = TextureFormat.Unknown, Detail = "FourCC 'ATI2'" };
        var plan = TexturePlanner.Choose(source, () => AlphaShape.Opaque, Bc,
            Accept(TextureFormat.Bc1, TextureFormat.Bc3), out var problem);

        Assert.NotNull(problem);
        Assert.Equal(TextureFormat.Unknown, plan.Target);
    }

    // Decoding 2524 passthrough DDS to ask about their alpha would cost more than the whole build.
    [Fact]
    public void AlphaIsNotInspectedForAnAlreadyCompressedSource()
    {
        var asked = false;
        TexturePlanner.Choose(Source(TextureFormat.Bc3), () => { asked = true; return AlphaShape.Opaque; },
            Bc, Accept(TextureFormat.Bc1, TextureFormat.Bc3), out _);

        Assert.False(asked);
    }

    // the ASTC set pairs footprint to the BC rule it stands in for: 6x6 where BC1 would apply,
    // 4x4 where BC3 would
    [Theory]
    [InlineData(AlphaShape.Opaque, TextureFormat.Astc6x6)]
    [InlineData(AlphaShape.PunchThrough, TextureFormat.Astc6x6)]
    [InlineData(AlphaShape.Partial, TextureFormat.Astc4x4)]
    public void AlphaShapeChoosesTheAstcFootprint(AlphaShape alpha, TextureFormat expected)
    {
        var encode = new[] { TextureFormat.Astc6x6, TextureFormat.Astc4x4 };
        var plan = Plan(Source(TextureFormat.Png), encode,
            Accept(TextureFormat.Astc4x4, TextureFormat.Astc6x6, TextureFormat.Png), alpha,
            out var problem);

        Assert.Null(problem);
        Assert.Equal(expected, plan.Target);
    }

    // ASTC pads a partial block itself, so unlike BC there is no multiple-of-four gate
    [Fact]
    public void AstcTakesDimensionsBlockCompressionWouldRefuse()
    {
        var encode = new[] { TextureFormat.Astc6x6 };
        var plan = Plan(Source(TextureFormat.Png, 30, 18), encode,
            Accept(TextureFormat.Astc6x6, TextureFormat.Png), AlphaShape.Opaque, out var problem);

        Assert.Null(problem);
        Assert.Equal(TextureFormat.Astc6x6, plan.Target);
    }

    // the one place a second lossy hop is right: the console variants only exist as DDS, and an
    // ASTC device cannot upload those at all, so BC has to come across
    [Fact]
    public void AstcTakesTheSecondLossyHopFromBc()
    {
        var encode = new[] { TextureFormat.Astc6x6, TextureFormat.Astc4x4 };
        var plan = Plan(Source(TextureFormat.Bc3), encode,
            Accept(TextureFormat.Astc4x4, TextureFormat.Astc6x6, TextureFormat.Bc3),
            AlphaShape.Partial, out var problem);

        Assert.Null(problem);
        Assert.Equal(TextureFormat.Astc4x4, plan.Target);
    }

    [Fact]
    public void AstcIsNeverReEncodedIntoTheOtherFootprint()
    {
        var encode = new[] { TextureFormat.Astc6x6, TextureFormat.Astc4x4 };
        var plan = Plan(Source(TextureFormat.Astc4x4), encode,
            Accept(TextureFormat.Astc4x4, TextureFormat.Astc6x6), AlphaShape.Opaque,
            out var problem);

        Assert.Null(problem);
        Assert.Equal(TextureFormat.Astc4x4, plan.Target);
    }

    [Fact]
    public void AlphaIsNotInspectedWhenNothingCouldBeEncoded()
    {
        var asked = false;
        TexturePlanner.Choose(Source(TextureFormat.Png), () => { asked = true; return AlphaShape.Opaque; },
            Nothing, Accept(TextureFormat.Png), out _);

        Assert.False(asked);
    }
}
