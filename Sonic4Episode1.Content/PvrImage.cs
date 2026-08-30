using System;
using System.IO;
using CSharp_PVR;
using Pvrtc;

namespace Sonic4Episode1.Content;

// decodes a PVR v2 texture to straight RGBA8; PVRTextureLoader can't be reused since it's gated to
// DEBUG plus FNA and needs a GraphicsDevice to build a Texture2D
internal static class PvrImage
{
    // the byte[] constructor never assigns textureData; only the Stream one reads the payload
    private static PVRContainerV2 Open(byte[] data)
    {
        return new PVRContainerV2(new MemoryStream(data));
    }

    public static bool TryReadInfo(byte[] data, out int width, out int height, out string format)
    {
        width = height = 0;
        format = null;

        try
        {
            var header = Open(data).GetHeader();
            width = (int)header.GetWidth();
            height = (int)header.GetHeight();
            format = header.GetPixelFormat().ToString();
            return width > 0 && height > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static byte[] DecodeRgba(byte[] data, out int width, out int height)
    {
        var container = Open(data);
        var header = container.GetHeader();
        var pixels = container.GetTextureData();

        width = (int)header.GetWidth();
        height = (int)header.GetHeight();

        var texels = width * height;
        var rgba = new byte[texels * 4];

        switch (header.GetPixelFormat())
        {
            // decoder hands back BGRA
            case PixelFormatV2.PVRTC2:
            case PixelFormatV2.PVRTC2_2:
                Bgra(new PvrtcDecoder().DecompressPVRTC(pixels, width, height, true), rgba, texels);
                break;

            case PixelFormatV2.PVRTC4:
            case PixelFormatV2.PVRTC4_2:
                Bgra(new PvrtcDecoder().DecompressPVRTC(pixels, width, height, false), rgba, texels);
                break;

            // already RGBA despite the name
            case PixelFormatV2.ARGB8888:
            case PixelFormatV2.ARGB8888_2:
                Rgba(pixels, rgba, texels);
                break;

            case PixelFormatV2.BGRA8888:
                Bgra(pixels, rgba, texels);
                break;

            case PixelFormatV2.RGB888:
            case PixelFormatV2.RGB888_2:
                Rgb888(pixels, rgba, texels);
                break;

            case PixelFormatV2.RGB565:
            case PixelFormatV2.RGB565_2:
                Sixteen(pixels, rgba, texels, Rgb565);
                break;

            case PixelFormatV2.RGB555:
            case PixelFormatV2.RGB555_2:
                Sixteen(pixels, rgba, texels, Rgb555);
                break;

            case PixelFormatV2.ARGB1555:
            case PixelFormatV2.ARGB1555_2:
                Sixteen(pixels, rgba, texels, Argb1555);
                break;

            case PixelFormatV2.ARGB4444:
            case PixelFormatV2.ARGB4444_2:
                Sixteen(pixels, rgba, texels, Argb4444);
                break;

            case PixelFormatV2.AI88:
            case PixelFormatV2.AI88_2:
                Ai88(pixels, rgba, texels);
                break;

            case PixelFormatV2.I8:
            case PixelFormatV2.I8_2:
                I8(pixels, rgba, texels);
                break;

            default:
                throw new NotSupportedException($"PVR pixel format {header.GetPixelFormat()}");
        }

        // no vertical flip: the game's UVs are OpenGL's, so the shipped tree is stored upside down
        // already, matching how a PVR reads out
        return rgba;
    }

    private static void Rgba(byte[] source, byte[] rgba, int texels)
    {
        Need(source, texels * 4);
        Buffer.BlockCopy(source, 0, rgba, 0, texels * 4);
    }

    private static void Bgra(byte[] source, byte[] rgba, int texels)
    {
        Need(source, texels * 4);
        for (int i = 0; i < texels; i++)
        {
            rgba[(i * 4) + 0] = source[(i * 4) + 2];
            rgba[(i * 4) + 1] = source[(i * 4) + 1];
            rgba[(i * 4) + 2] = source[(i * 4) + 0];
            rgba[(i * 4) + 3] = source[(i * 4) + 3];
        }
    }

    private static void Rgb888(byte[] source, byte[] rgba, int texels)
    {
        Need(source, texels * 3);
        for (int i = 0; i < texels; i++)
        {
            rgba[(i * 4) + 0] = source[(i * 3) + 0];
            rgba[(i * 4) + 1] = source[(i * 3) + 1];
            rgba[(i * 4) + 2] = source[(i * 3) + 2];
            rgba[(i * 4) + 3] = 255;
        }
    }

    private delegate void Unpack(ushort pixel, out byte r, out byte g, out byte b, out byte a);

    private static void Sixteen(byte[] source, byte[] rgba, int texels, Unpack unpack)
    {
        Need(source, texels * 2);
        for (int i = 0; i < texels; i++)
        {
            var pixel = (ushort)(source[i * 2] | (source[(i * 2) + 1] << 8));
            unpack(pixel, out var r, out var g, out var b, out var a);
            rgba[(i * 4) + 0] = r;
            rgba[(i * 4) + 1] = g;
            rgba[(i * 4) + 2] = b;
            rgba[(i * 4) + 3] = a;
        }
    }

    // 5 and 6 bit channels are replicated into the low bits rather than shifted, so full-scale
    // stays full-scale: 0x1F becomes 0xFF and not 0xF8.
    private static void Rgb565(ushort pixel, out byte r, out byte g, out byte b, out byte a)
    {
        r = Expand5((pixel >> 11) & 0x1F);
        g = (byte)((((pixel >> 5) & 0x3F) * 255) / 63);
        b = Expand5(pixel & 0x1F);
        a = 255;
    }

    private static void Rgb555(ushort pixel, out byte r, out byte g, out byte b, out byte a)
    {
        r = Expand5((pixel >> 10) & 0x1F);
        g = Expand5((pixel >> 5) & 0x1F);
        b = Expand5(pixel & 0x1F);
        a = 255;
    }

    // named ARGB1555 but laid out RGBA: red from the top five bits, alpha from bit 0
    private static void Argb1555(ushort pixel, out byte r, out byte g, out byte b, out byte a)
    {
        r = Expand5((pixel >> 11) & 0x1F);
        g = Expand5((pixel >> 6) & 0x1F);
        b = Expand5((pixel >> 1) & 0x1F);
        a = (byte)((pixel & 0x0001) != 0 ? 255 : 0);
    }

    // named ARGB4444, laid out RGBA4444: a transparent white background reads 0xFFF0 and Sonic's
    // blue reads 0x00FF, both wrong under a genuine ARGB4444
    private static void Argb4444(ushort pixel, out byte r, out byte g, out byte b, out byte a)
    {
        r = Expand4((pixel >> 12) & 0xF);
        g = Expand4((pixel >> 8) & 0xF);
        b = Expand4((pixel >> 4) & 0xF);
        a = Expand4(pixel & 0xF);
    }

    // Alpha and intensity, so the intensity drives all three colour channels.
    private static void Ai88(byte[] source, byte[] rgba, int texels)
    {
        Need(source, texels * 2);
        for (int i = 0; i < texels; i++)
        {
            var intensity = source[i * 2];
            rgba[(i * 4) + 0] = intensity;
            rgba[(i * 4) + 1] = intensity;
            rgba[(i * 4) + 2] = intensity;
            rgba[(i * 4) + 3] = source[(i * 2) + 1];
        }
    }

    private static void I8(byte[] source, byte[] rgba, int texels)
    {
        Need(source, texels);
        for (int i = 0; i < texels; i++)
        {
            rgba[(i * 4) + 0] = source[i];
            rgba[(i * 4) + 1] = source[i];
            rgba[(i * 4) + 2] = source[i];
            rgba[(i * 4) + 3] = 255;
        }
    }

    private static byte Expand5(int value) => (byte)((value * 255) / 31);

    private static byte Expand4(int value) => (byte)(value * 17);

    private static void Need(byte[] source, int bytes)
    {
        if (source.Length < bytes)
            throw new InvalidDataException($"PVR payload is {source.Length} bytes, needs {bytes}");
    }
}
