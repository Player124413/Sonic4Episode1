using System;
using System.Buffers.Binary;
using System.Globalization;
using System.IO;

namespace Sonic4Episode1.Content;

public enum TextureFormat
{
    Unknown,
    Png,
    Bc1,
    Bc2,
    Bc3,
    Bc7,
    Rgba32,
    Bgra32,
    L8,
    Pvr,
    Astc4x4,
    Astc6x6,
}

internal readonly struct ImageInfo
{
    public TextureFormat Format { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }
    public int MipLevels { get; init; }
    public bool PitchAndLinear { get; init; }
    public string Detail { get; init; }

    public long Texels => (long)Width * Height;

    public static ImageInfo None => new ImageInfo { Format = TextureFormat.Unknown };
}

// identifies a texture member from its header alone, deliberately free of any DirectXTexNet
// reference so `check` doesn't pull in a Windows-only native DLL to probe every archive member.
// the DDS side mirrors FNA's Texture.ParseDDS field for field, so a DXGI code FNA rejects stays
// Unknown rather than being mapped to something close
internal static class TextureFormats
{
    private const int DDS_MAGIC = 0x20534444;
    private const int DDS_HEADER_SIZE = 124;
    private const int DDS_PIXELFORMAT_SIZE = 32;

    private const int DDSD_PITCH = 0x8;
    private const int DDSD_LINEARSIZE = 0x80000;

    private const int DDPF_FOURCC = 0x4;
    private const int DDPF_RGB = 0x40;
    private const int DDPF_LUMINANCE = 0x20000;

    private const int OFF_FLAGS = 0x08;
    private const int OFF_HEIGHT = 0x0C;
    private const int OFF_WIDTH = 0x10;
    private const int OFF_MIPS = 0x1C;
    private const int OFF_PF_SIZE = 0x4C;
    private const int OFF_PF_FLAGS = 0x50;
    private const int OFF_FOURCC = 0x54;
    private const int OFF_BITCOUNT = 0x58;
    private const int OFF_RMASK = 0x5C;
    private const int OFF_GMASK = 0x60;
    private const int OFF_BMASK = 0x64;
    private const int OFF_AMASK = 0x68;
    private const int OFF_DX10 = 0x80;

    private const int FOURCC_DXT1 = 0x31545844;
    private const int FOURCC_DXT3 = 0x33545844;
    private const int FOURCC_DXT5 = 0x35545844;
    private const int FOURCC_DX10 = 0x30315844;

    private const int DXGI_BC1_UNORM = 71;
    private const int DXGI_BC2_UNORM = 74;
    private const int DXGI_BC3_UNORM = 77;
    private const int DXGI_BC7_UNORM = 98;
    private const int DXGI_BC7_UNORM_SRGB = 99;

    private static ReadOnlySpan<byte> PngSignature =>
        new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

    public static ImageInfo Probe(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length >= 8 && bytes.Slice(0, 8).SequenceEqual(PngSignature))
            return ProbePng(bytes);

        if (bytes.Length >= 4 && ReadInt(bytes, 0) == DDS_MAGIC)
            return ProbeDds(bytes);

        if (IsPvr(bytes))
            return ProbePvr(bytes);

        if (bytes.Length >= 4 && ReadInt(bytes, 0) == ASTC_MAGIC)
            return ProbeAstc(bytes);

        return ImageInfo.None;
    }

    private static ImageInfo ProbePng(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length < 26)
            return Unknown("truncated PNG");

        if (bytes[12] != 'I' || bytes[13] != 'H' || bytes[14] != 'D' || bytes[15] != 'R')
            return Unknown("PNG without a leading IHDR");

        return new ImageInfo
        {
            Format = TextureFormat.Png,
            Width = BinaryPrimitives.ReadInt32BigEndian(bytes.Slice(16, 4)),
            Height = BinaryPrimitives.ReadInt32BigEndian(bytes.Slice(20, 4)),
            MipLevels = 1,
        };
    }

    private static ImageInfo ProbeDds(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length < 128)
            return Unknown("truncated DDS");
        if (ReadInt(bytes, 4) != DDS_HEADER_SIZE)
            return Unknown($"DDS header size {ReadInt(bytes, 4)}");
        if (ReadInt(bytes, OFF_PF_SIZE) != DDS_PIXELFORMAT_SIZE)
            return Unknown($"DDS pixel format size {ReadInt(bytes, OFF_PF_SIZE)}");

        var flags = ReadInt(bytes, OFF_FLAGS);
        var format = ReadFormat(bytes, out var detail);

        return new ImageInfo
        {
            Format = format,
            Width = ReadInt(bytes, OFF_WIDTH),
            Height = ReadInt(bytes, OFF_HEIGHT),
            MipLevels = Math.Max(1, ReadInt(bytes, OFF_MIPS)),
            PitchAndLinear = (flags & DDSD_PITCH) != 0 && (flags & DDSD_LINEARSIZE) != 0,
            Detail = detail,
        };
    }

    private static TextureFormat ReadFormat(ReadOnlySpan<byte> bytes, out string detail)
    {
        detail = null;
        var pixelFlags = ReadInt(bytes, OFF_PF_FLAGS);

        if ((pixelFlags & DDPF_FOURCC) != 0)
        {
            var fourCc = ReadInt(bytes, OFF_FOURCC);
            switch (fourCc)
            {
                case FOURCC_DXT1: return TextureFormat.Bc1;
                case FOURCC_DXT3: return TextureFormat.Bc2;
                case FOURCC_DXT5: return TextureFormat.Bc3;
                case FOURCC_DX10: return ReadDx10(bytes, out detail);
            }

            detail = $"FourCC '{FourCcText(fourCc)}'";
            return TextureFormat.Unknown;
        }

        var bitCount = ReadInt(bytes, OFF_BITCOUNT);

        if ((pixelFlags & DDPF_RGB) != 0)
        {
            if (bitCount != 32)
            {
                detail = $"uncompressed RGB at {bitCount}bpp";
                return TextureFormat.Unknown;
            }

            var r = ReadUInt(bytes, OFF_RMASK);
            var g = ReadUInt(bytes, OFF_GMASK);
            var b = ReadUInt(bytes, OFF_BMASK);
            var a = ReadUInt(bytes, OFF_AMASK);

            if (r == 0x00FF0000 && g == 0x0000FF00 && b == 0x000000FF && a == 0xFF000000)
                return TextureFormat.Bgra32;
            if (r == 0x000000FF && g == 0x0000FF00 && b == 0x00FF0000 && a == 0xFF000000)
                return TextureFormat.Rgba32;

            detail = $"32bpp masks {r:X8}/{g:X8}/{b:X8}/{a:X8}";
            return TextureFormat.Unknown;
        }

        if ((pixelFlags & DDPF_LUMINANCE) != 0 && bitCount == 8)
            return TextureFormat.L8;

        detail = $"pixel format flags {pixelFlags:X8} at {bitCount}bpp";
        return TextureFormat.Unknown;
    }

    private static TextureFormat ReadDx10(ReadOnlySpan<byte> bytes, out string detail)
    {
        detail = null;
        if (bytes.Length < OFF_DX10 + 20)
        {
            detail = "truncated DX10 header";
            return TextureFormat.Unknown;
        }

        var dxgi = ReadInt(bytes, OFF_DX10);
        switch (dxgi)
        {
            case DXGI_BC1_UNORM: return TextureFormat.Bc1;
            case DXGI_BC2_UNORM: return TextureFormat.Bc2;
            case DXGI_BC3_UNORM: return TextureFormat.Bc3;
            case DXGI_BC7_UNORM:
            case DXGI_BC7_UNORM_SRGB: return TextureFormat.Bc7;
        }

        detail = $"DXGI format {dxgi.ToString(CultureInfo.InvariantCulture)}";
        return TextureFormat.Unknown;
    }

    // v2 header is height first, then width, backwards from DDS and PNG
    private const int OFF_PVR_HEIGHT = 0x04;
    private const int OFF_PVR_WIDTH = 0x08;

    private static ImageInfo ProbePvr(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length < 0x34 || bytes[3] == 3)
            return new ImageInfo { Format = TextureFormat.Pvr, MipLevels = 1 };

        return new ImageInfo
        {
            Format = TextureFormat.Pvr,
            Height = ReadInt(bytes, OFF_PVR_HEIGHT),
            Width = ReadInt(bytes, OFF_PVR_WIDTH),
            MipLevels = 1,
        };
    }

    // 16 byte header: magic, then one byte each of block x/y/z, then three 24-bit LE dimensions
    private const int ASTC_MAGIC = 0x5CA1AB13;
    private const int ASTC_HEADER_SIZE = 16;

    private static ImageInfo ProbeAstc(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length < ASTC_HEADER_SIZE)
            return Unknown("truncated ASTC");

        var format = (bytes[4], bytes[5], bytes[6]) switch
        {
            (4, 4, 1) => TextureFormat.Astc4x4,
            (6, 6, 1) => TextureFormat.Astc6x6,
            _ => TextureFormat.Unknown,
        };

        if (format == TextureFormat.Unknown)
            return Unknown($"ASTC block {bytes[4]}x{bytes[5]}x{bytes[6]}");

        return new ImageInfo
        {
            Format = format,
            Width = Read24(bytes, 7),
            Height = Read24(bytes, 10),
            MipLevels = 1,
        };
    }

    private static int Read24(ReadOnlySpan<byte> bytes, int offset)
    {
        return bytes[offset] | (bytes[offset + 1] << 8) | (bytes[offset + 2] << 16);
    }

    // beside ProbeAstc so the two halves of the header can't drift
    public static byte[] WrapAstc(TextureFormat format, int width, int height,
        ReadOnlySpan<byte> blocks)
    {
        var block = AstcBlock(format);
        var output = new byte[ASTC_HEADER_SIZE + blocks.Length];

        BinaryPrimitives.WriteInt32LittleEndian(output, ASTC_MAGIC);
        output[4] = (byte)block.X;
        output[5] = (byte)block.Y;
        output[6] = 1;
        Write24(output, 7, width);
        Write24(output, 10, height);
        Write24(output, 13, 1);
        blocks.CopyTo(output.AsSpan(ASTC_HEADER_SIZE));

        return output;
    }

    public static ReadOnlySpan<byte> AstcBlocks(ReadOnlySpan<byte> bytes)
    {
        return bytes.Slice(ASTC_HEADER_SIZE);
    }

    private static void Write24(Span<byte> bytes, int offset, int value)
    {
        bytes[offset] = (byte)value;
        bytes[offset + 1] = (byte)(value >> 8);
        bytes[offset + 2] = (byte)(value >> 16);
    }

    // PVRv3 leads with its magic; PVRv2 puts "PVR!" at 0x2C of a 52 byte header.
    private static bool IsPvr(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length >= 4 && bytes[0] == 'P' && bytes[1] == 'V' && bytes[2] == 'R' && bytes[3] == 3)
            return true;

        return bytes.Length >= 0x30
            && bytes[0x2C] == 'P' && bytes[0x2D] == 'V' && bytes[0x2E] == 'R' && bytes[0x2F] == '!';
    }

    private static ImageInfo Unknown(string detail)
    {
        return new ImageInfo { Format = TextureFormat.Unknown, Detail = detail };
    }

    private static int ReadInt(ReadOnlySpan<byte> bytes, int offset)
    {
        return BinaryPrimitives.ReadInt32LittleEndian(bytes.Slice(offset, 4));
    }

    private static uint ReadUInt(ReadOnlySpan<byte> bytes, int offset)
    {
        return BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(offset, 4));
    }

    private static string FourCcText(int fourCc)
    {
        Span<char> text = stackalloc char[4];
        for (int i = 0; i < 4; i++)
        {
            var c = (char)((fourCc >> (i * 8)) & 0xFF);
            text[i] = c >= 0x20 && c < 0x7F ? c : '?';
        }

        return new string(text);
    }

    public static bool IsBlockCompressed(TextureFormat format)
    {
        switch (format)
        {
            case TextureFormat.Bc1:
            case TextureFormat.Bc2:
            case TextureFormat.Bc3:
            case TextureFormat.Bc7:
            case TextureFormat.Astc4x4:
            case TextureFormat.Astc6x6:
                return true;
            default:
                return false;
        }
    }

    public static bool IsAstc(TextureFormat format)
    {
        return format == TextureFormat.Astc4x4 || format == TextureFormat.Astc6x6;
    }

    // ASTC is a fixed 128 bits a block whatever the footprint; only the block size changes
    public static (int X, int Y) AstcBlock(TextureFormat format)
    {
        return format == TextureFormat.Astc6x6 ? (6, 6) : (4, 4);
    }

    public static string Token(TextureFormat format)
    {
        return format.ToString().ToLowerInvariant();
    }

    public static bool TryParse(string token, out TextureFormat format)
    {
        switch (token?.Trim().ToLowerInvariant())
        {
            case "png": format = TextureFormat.Png; return true;
            case "bc1":
            case "dxt1": format = TextureFormat.Bc1; return true;
            case "bc2":
            case "dxt3": format = TextureFormat.Bc2; return true;
            case "bc3":
            case "dxt5": format = TextureFormat.Bc3; return true;
            case "bc7": format = TextureFormat.Bc7; return true;
            case "rgba32": format = TextureFormat.Rgba32; return true;
            case "bgra32": format = TextureFormat.Bgra32; return true;
            case "l8": format = TextureFormat.L8; return true;
            case "pvr": format = TextureFormat.Pvr; return true;
            case "astc4x4": format = TextureFormat.Astc4x4; return true;
            case "astc6x6": format = TextureFormat.Astc6x6; return true;
            default: format = TextureFormat.Unknown; return false;
        }
    }

    public static string Extension(TextureFormat format)
    {
        switch (format)
        {
            case TextureFormat.Png: return ".PNG";
            case TextureFormat.Pvr: return ".PVR";
            case TextureFormat.Astc4x4:
            case TextureFormat.Astc6x6: return ".AST";
            case TextureFormat.Unknown: return null;
            default: return ".DDS";
        }
    }

    // amTextureDecode dispatches on extension, so the name has to follow the payload on transcode
    public static string NameFor(string member, TextureFormat format)
    {
        var extension = Extension(format);
        if (extension == null)
            return member;

        var directory = Path.GetDirectoryName(member);
        var stem = Path.GetFileNameWithoutExtension(member);

        return string.IsNullOrEmpty(directory) ? stem + extension : Path.Combine(directory, stem + extension);
    }

    // level 0 only: Texture2D.FromStream doesn't walk a mip chain even though DDSFromStreamEXT does
    public static long GpuBytes(ImageInfo info)
    {
        switch (info.Format)
        {
            case TextureFormat.Bc1:
                return Blocks(info) * 8;
            case TextureFormat.Bc2:
            case TextureFormat.Bc3:
            case TextureFormat.Bc7:
                return Blocks(info) * 16;
            case TextureFormat.Astc4x4:
            case TextureFormat.Astc6x6:
            {
                var block = AstcBlock(info.Format);
                return (long)((info.Width + block.X - 1) / block.X)
                       * ((info.Height + block.Y - 1) / block.Y) * 16;
            }
            case TextureFormat.L8:
                return info.Texels;
            case TextureFormat.Unknown:
                return 0;
            default:
                return info.Texels * 4;
        }
    }

    private static long Blocks(ImageInfo info)
    {
        return (long)((info.Width + 3) / 4) * ((info.Height + 3) / 4);
    }
}
