using System;
using System.Collections.Generic;
using System.IO;
using DirectXTexNet;

namespace Sonic4Episode1.Content;

// turns a shipped PNG into a DDS the engine can upload without decoding. Source is the PNG rather
// than the retail PVR it came from; PNG is lossless so re-encoding either gives the same result,
// see docs/content-catalog.md. Renaming .PNG to .DDS is safe since amTextureLoad probes extensions
internal static class TextureTranscode
{
    public static bool IsTexture(string name)
    {
        var extension = Path.GetExtension(name);
        return extension.Equals(".PNG", StringComparison.OrdinalIgnoreCase)
            || extension.Equals(".DDS", StringComparison.OrdinalIgnoreCase)
            || extension.Equals(".PVR", StringComparison.OrdinalIgnoreCase);
    }

    // BC1's one-bit alpha covers opaque and hard-edged cutouts at half BC3's size; only genuinely
    // partial transparency needs BC3. Zero for now, reproducing the original rule exactly.
    public const int AlphaSnapTolerance = 0;

    public static unsafe AlphaShape AnalyseAlpha(ScratchImage rgba)
    {
        if (rgba.IsAlphaAllOpaque())
            return AlphaShape.Opaque;

        var info = rgba.GetImage(0);
        var pixels = (byte*)info.Pixels;
        var width = (int)info.Width;
        var pitch = (int)info.RowPitch;
        var opaque = true;

        for (int y = 0; y < info.Height; y++)
        {
            var row = pixels + (y * pitch);
            for (int x = 0; x < width; x++)
            {
                var alpha = row[(x * 4) + 3];
                if (alpha != 255)
                    opaque = false;

                if (Math.Min(alpha, 255 - alpha) > AlphaSnapTolerance)
                    return AlphaShape.Partial;
            }
        }

        return opaque ? AlphaShape.Opaque : AlphaShape.PunchThrough;
    }

    // mean channel value; a gamma mistake moves this a long way, BC compression barely at all
    public static unsafe (string Format, double R, double G, double B, double A) Describe(byte[] image)
    {
        var probed = TextureFormats.Probe(image);
        ScratchImage loaded = null;
        ScratchImage rgba = null;
        try
        {
            rgba = DecodeToRgba(image, probed, out loaded);
            var format = loaded.GetMetadata().Format;
            var info = rgba.GetImage(0);
            var pixels = (byte*)info.Pixels;
            double r = 0, g = 0, b = 0, a = 0;
            long n = 0, all = 0;
            for (int y = 0; y < info.Height; y++)
            {
                var row = pixels + (y * (int)info.RowPitch);
                for (int x = 0; x < info.Width; x++)
                {
                    var alpha = row[(x * 4) + 3];
                    a += alpha;
                    all++;

                    // BC1 forces a transparent texel's colour to black, so only drawn texels count
                    if (alpha == 0)
                        continue;

                    r += row[x * 4]; g += row[(x * 4) + 1]; b += row[(x * 4) + 2];
                    n++;
                }
            }

            n = Math.Max(n, 1);
            return (format.ToString(), r / n, g / n, b / n, a / Math.Max(all, 1));
        }
        finally
        {
            if (!ReferenceEquals(rgba, loaded))
                rgba?.Dispose();

            loaded?.Dispose();
        }
    }

    // the one decode, shared by alpha analysis and every encode target
    private static unsafe ScratchImage DecodeToRgba(byte[] data, ImageInfo info,
        out ScratchImage loaded)
    {
        if (info.Format == TextureFormat.Pvr)
        {
            loaded = FromRgba(PvrImage.DecodeRgba(data, out var width, out var height), width, height);
            return loaded;
        }

        var handle = System.Runtime.InteropServices.GCHandle.Alloc(
            data, System.Runtime.InteropServices.GCHandleType.Pinned);
        try
        {
            // IGNORE_SRGB matters: every PNG carries an sRGB chunk, and honouring it would apply a
            // real sRGB-to-linear transform the game's own stb_image loader never does
            loaded = info.Format == TextureFormat.Png
                ? TexHelper.Instance.LoadFromWICMemory(handle.AddrOfPinnedObject(), data.Length, WIC_FLAGS.IGNORE_SRGB)
                : TexHelper.Instance.LoadFromDDSMemory(handle.AddrOfPinnedObject(), data.Length, DDS_FLAGS.NONE);
        }
        finally
        {
            handle.Free();
        }

        var format = loaded.GetMetadata().Format;
        if (TexHelper.Instance.IsCompressed(format))
            return loaded.Decompress(DXGI_FORMAT.R8G8B8A8_UNORM);

        // Convert throws when asked for the format it already has
        if (format == DXGI_FORMAT.R8G8B8A8_UNORM)
            return loaded;

        return loaded.Convert(DXGI_FORMAT.R8G8B8A8_UNORM, TEX_FILTER_FLAGS.DEFAULT, 0.5f);
    }

    private static unsafe ScratchImage FromRgba(byte[] rgba, int width, int height)
    {
        var image = TexHelper.Instance.Initialize2D(
            DXGI_FORMAT.R8G8B8A8_UNORM, width, height, 1, 1, CP_FLAGS.NONE);

        var target = image.GetImage(0);
        var pixels = (byte*)target.Pixels;
        var pitch = (int)target.RowPitch;

        for (int y = 0; y < height; y++)
            System.Runtime.InteropServices.Marshal.Copy(
                rgba, y * width * 4, (IntPtr)(pixels + (y * pitch)), width * 4);

        return image;
    }

    public static AlphaShape AlphaOf(byte[] data, ImageInfo info)
    {
        ScratchImage loaded = null;
        ScratchImage rgba = null;
        try
        {
            rgba = DecodeToRgba(data, info, out loaded);
            return AnalyseAlpha(rgba);
        }
        finally
        {
            if (!ReferenceEquals(rgba, loaded))
                rgba?.Dispose();

            loaded?.Dispose();
        }
    }


    public readonly struct ImageCompare
    {
        public double RgbPsnr { get; init; }
        public double MeanAbsAlphaDelta { get; init; }
        public int MaxAbsAlphaDelta { get; init; }
        public long AlphaFlipped { get; init; }
        public long Texels { get; init; }
    }

    // unlike Describe, RGB error here is weighted by min(alphaA, alphaB) so an invisible texel
    // contributes nothing; AlphaFlipped only counts a texel crossing the halfway mark
    public static unsafe ImageCompare Compare(byte[] left, byte[] right)
    {
        var leftInfo = TextureFormats.Probe(left);
        var rightInfo = TextureFormats.Probe(right);

        ScratchImage leftLoaded = null, leftRgba = null, rightLoaded = null, rightRgba = null;
        try
        {
            leftRgba = DecodeToRgba(left, leftInfo, out leftLoaded);
            rightRgba = DecodeToRgba(right, rightInfo, out rightLoaded);

            var a = leftRgba.GetImage(0);
            var b = rightRgba.GetImage(0);
            if (a.Width != b.Width || a.Height != b.Height)
                throw new InvalidDataException($"{a.Width}x{a.Height} against {b.Width}x{b.Height}");

            var aPixels = (byte*)a.Pixels;
            var bPixels = (byte*)b.Pixels;
            double weightedError = 0, weight = 0, alphaDelta = 0;
            int maxAlphaDelta = 0;
            long flipped = 0, texels = 0;

            for (int y = 0; y < a.Height; y++)
            {
                var rowA = aPixels + (y * (int)a.RowPitch);
                var rowB = bPixels + (y * (int)b.RowPitch);
                for (int x = 0; x < a.Width; x++)
                {
                    var pa = rowA + (x * 4);
                    var pb = rowB + (x * 4);
                    texels++;

                    var delta = Math.Abs(pa[3] - pb[3]);
                    alphaDelta += delta;
                    maxAlphaDelta = Math.Max(maxAlphaDelta, delta);
                    if ((pa[3] >= 128) != (pb[3] >= 128))
                        flipped++;

                    var visible = Math.Min(pa[3], pb[3]) / 255.0;
                    if (visible <= 0)
                        continue;

                    for (int channel = 0; channel < 3; channel++)
                    {
                        var difference = pa[channel] - pb[channel];
                        weightedError += visible * difference * difference;
                    }

                    weight += visible * 3;
                }
            }

            var mse = weight > 0 ? weightedError / weight : 0;
            return new ImageCompare
            {
                RgbPsnr = mse > 0 ? 10.0 * Math.Log10(255.0 * 255.0 / mse) : double.PositiveInfinity,
                MeanAbsAlphaDelta = texels > 0 ? alphaDelta / texels : 0,
                MaxAbsAlphaDelta = maxAlphaDelta,
                AlphaFlipped = flipped,
                Texels = texels,
            };
        }
        finally
        {
            if (!ReferenceEquals(leftRgba, leftLoaded)) leftRgba?.Dispose();
            if (!ReferenceEquals(rightRgba, rightLoaded)) rightRgba?.Dispose();
            leftLoaded?.Dispose();
            rightLoaded?.Dispose();
        }
    }

    // a decode held open so the alpha analysis that picks the target and the encode that follows
    // can share it. PVRTC decode dwarfs everything else here, and planning used to pay it twice
    public sealed class Decoded : IDisposable
    {
        private readonly ScratchImage loaded;
        private readonly ScratchImage rgba;
        private AlphaShape? alpha;

        internal Decoded(ScratchImage loaded, ScratchImage rgba)
        {
            this.loaded = loaded;
            this.rgba = rgba;
        }

        public AlphaShape Alpha()
        {
            return alpha ??= AnalyseAlpha(rgba);
        }

        public byte[] Save(TextureFormat target, string name)
        {
            try
            {
                if (target == TextureFormat.Png)
                {
                    using var png = rgba.SaveToWICMemory(
                        0, WIC_FLAGS.IGNORE_SRGB, TexHelper.Instance.GetWICCodec(WICCodecs.PNG));
                    return Drain(png);
                }

                // alphaRef is the cutoff BC1 uses to decide a texel is transparent; it only matters
                // on the cutout path, where the alpha is already 0 or 255
                using var encoded = rgba.Compress(DxgiOf(target), TEX_COMPRESS_FLAGS.PARALLEL, 0.5f);
                using var dds = encoded.SaveToDDSMemory(DDS_FLAGS.NONE);
                return Drain(dds);
            }
            catch (Exception ex)
            {
                throw new InvalidDataException($"{name}: {ex.Message}", ex);
            }
        }

        public void Dispose()
        {
            if (!ReferenceEquals(rgba, loaded))
                rgba?.Dispose();

            loaded?.Dispose();
        }
    }

    public static Decoded Decode(byte[] data, ImageInfo info, bool flip = false)
    {
        ScratchImage loaded = null;
        ScratchImage rgba = null;
        try
        {
            rgba = DecodeToRgba(data, info, out loaded);

            if (flip)
            {
                var flipped = rgba.FlipRotate(TEX_FR_FLAGS.FLIP_VERTICAL);
                if (!ReferenceEquals(rgba, loaded))
                    rgba.Dispose();

                rgba = flipped;
            }

            return new Decoded(loaded, rgba);
        }
        catch
        {
            if (!ReferenceEquals(rgba, loaded))
                rgba?.Dispose();

            loaded?.Dispose();
            throw;
        }
    }

    public static byte[] Encode(byte[] data, ImageInfo info, TextureFormat target, string name,
        bool flip = false)
    {
        using var decoded = Decode(data, info, flip);
        return decoded.Save(target, name);
    }

    private static DXGI_FORMAT DxgiOf(TextureFormat target)
    {
        switch (target)
        {
            case TextureFormat.Bc1: return DXGI_FORMAT.BC1_UNORM;
            case TextureFormat.Bc2: return DXGI_FORMAT.BC2_UNORM;
            case TextureFormat.Bc3: return DXGI_FORMAT.BC3_UNORM;
            default:
                throw new NotSupportedException($"cannot encode to {TextureFormats.Token(target)}");
        }
    }

    private static byte[] Drain(System.IO.UnmanagedMemoryStream stream)
    {
        var output = new byte[stream.Length];
        var read = 0;
        while (read < output.Length)
        {
            var got = stream.Read(output, read, output.Length - read);
            if (got <= 0)
                break;

            read += got;
        }

        return output;
    }

}
