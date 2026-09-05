using System;
using System.Collections.Generic;

namespace Sonic4Episode1.Content;

public enum AlphaShape
{
    Opaque,
    PunchThrough,
    Partial,
}

internal readonly struct TexturePlan
{
    public TextureFormat Target { get; init; }
    public string Reason { get; init; }
}

// the first entry of `encode` whose predicate holds wins, so an empty `encode` can't change
// anything. BC never re-encodes lossy to lossy; ASTC is the one exception, because the console
// variants only exist as DDS and an ASTC device cannot upload those at all
internal static class TexturePlanner
{
    public static TexturePlan Choose(
        ImageInfo source,
        Func<AlphaShape> alpha,
        IReadOnlyList<TextureFormat> encode,
        IReadOnlySet<TextureFormat> accept,
        out string problem)
    {
        problem = null;

        if (source.Format == TextureFormat.Unknown)
        {
            problem = $"unrecognised texture ({source.Detail ?? "no header"})";
            return Keep(source, "unrecognised");
        }

        for (int i = 0; i < encode.Count; i++)
        {
            var candidate = encode[i];
            if (!Applicable(candidate, source, alpha))
                continue;

            return new TexturePlan
            {
                Target = candidate,
                Reason = candidate == source.Format ? "already " + TextureFormats.Token(candidate)
                    : TextureFormats.Token(candidate),
            };
        }

        if (!accept.Contains(source.Format))
        {
            problem = $"{TextureFormats.Token(source.Format)} is neither encodable nor accepted";
            return Keep(source, "not accepted");
        }

        return Keep(source, "no candidate applies");
    }

    private static TexturePlan Keep(ImageInfo source, string reason)
    {
        return new TexturePlan { Target = source.Format, Reason = reason };
    }

    private static bool Applicable(TextureFormat target, ImageInfo source, Func<AlphaShape> alpha)
    {
        if (target == source.Format)
            return true;

        switch (target)
        {
            // PNG is lossless and holds any decoded image exactly, so it always applies
            case TextureFormat.Png:
                return true;

            case TextureFormat.Bc1:
                return Blocks(source) && !TextureFormats.IsBlockCompressed(source.Format)
                    && alpha() != AlphaShape.Partial;

            case TextureFormat.Bc3:
                return Blocks(source) && !TextureFormats.IsBlockCompressed(source.Format);

            // ASTC pads a partial block itself, so unlike BC there is no dimension gate. The alpha
            // test is not about alpha: every ASTC footprint carries full 8-bit alpha. It is a proxy
            // for how much detail a texture has, and 6x6's 3.56bpp cannot hold the busy ones.
            // BC sources take a second lossy hop; only ASTC is refused, so a footprint never churns
            case TextureFormat.Astc6x6:
                return !TextureFormats.IsAstc(source.Format)
                    && alpha() != AlphaShape.Partial;

            case TextureFormat.Astc4x4:
                return !TextureFormats.IsAstc(source.Format);

            // BC2/BC7 are never encode targets: nothing produces DXT3 deliberately, and BC7 is
            // parseable but never capability-checked, so it'd risk a black texture without BPTC
            default:
                return false;
        }
    }

    private static bool Blocks(ImageInfo source)
    {
        return (source.Width % 4) == 0 && (source.Height % 4) == 0;
    }
}
