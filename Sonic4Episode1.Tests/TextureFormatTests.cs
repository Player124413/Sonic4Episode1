using System;
using System.Buffers.Binary;
using Sonic4Episode1.Content;

namespace Sonic4Episode1.Tests;

// headers are built by hand so a format the probe claims to know is pinned to the exact bytes
// FNA's Texture.ParseDDS looks at
public class TextureFormatTests
{
    private const int DDPF_FOURCC = 0x4;
    private const int DDPF_RGB = 0x40;
    private const int DDPF_LUMINANCE = 0x20000;

    private static byte[] Dds(int pixelFlags, string fourCc, int bitCount,
        uint r = 0, uint g = 0, uint b = 0, uint a = 0, int width = 64, int height = 32,
        int mips = 1, int headerFlags = 0x1007, int dxgi = 0, int payload = 4096)
    {
        var bytes = new byte[128 + payload];
        var span = bytes.AsSpan();

        BinaryPrimitives.WriteInt32LittleEndian(span.Slice(0, 4), 0x20534444);
        BinaryPrimitives.WriteInt32LittleEndian(span.Slice(4, 4), 124);
        BinaryPrimitives.WriteInt32LittleEndian(span.Slice(0x08, 4), headerFlags);
        BinaryPrimitives.WriteInt32LittleEndian(span.Slice(0x0C, 4), height);
        BinaryPrimitives.WriteInt32LittleEndian(span.Slice(0x10, 4), width);
        BinaryPrimitives.WriteInt32LittleEndian(span.Slice(0x1C, 4), mips);
        BinaryPrimitives.WriteInt32LittleEndian(span.Slice(0x4C, 4), 32);
        BinaryPrimitives.WriteInt32LittleEndian(span.Slice(0x50, 4), pixelFlags);

        if (fourCc != null)
        {
            for (int i = 0; i < 4; i++)
                bytes[0x54 + i] = (byte)fourCc[i];
        }

        BinaryPrimitives.WriteInt32LittleEndian(span.Slice(0x58, 4), bitCount);
        BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(0x5C, 4), r);
        BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(0x60, 4), g);
        BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(0x64, 4), b);
        BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(0x68, 4), a);

        if (fourCc == "DX10")
            BinaryPrimitives.WriteInt32LittleEndian(span.Slice(0x80, 4), dxgi);

        return bytes;
    }

    private static byte[] Png(int width, int height)
    {
        var bytes = new byte[64];
        var signature = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        signature.CopyTo(bytes, 0);

        BinaryPrimitives.WriteInt32BigEndian(bytes.AsSpan(8, 4), 13);
        bytes[12] = (byte)'I';
        bytes[13] = (byte)'H';
        bytes[14] = (byte)'D';
        bytes[15] = (byte)'R';
        BinaryPrimitives.WriteInt32BigEndian(bytes.AsSpan(16, 4), width);
        BinaryPrimitives.WriteInt32BigEndian(bytes.AsSpan(20, 4), height);

        return bytes;
    }

    [Theory]
    [InlineData("DXT1", TextureFormat.Bc1)]
    [InlineData("DXT3", TextureFormat.Bc2)]
    [InlineData("DXT5", TextureFormat.Bc3)]
    public void FourCcMapsToBlockFormat(string fourCc, TextureFormat expected)
    {
        Assert.Equal(expected, TextureFormats.Probe(Dds(DDPF_FOURCC, fourCc, 0)).Format);
    }

    [Theory]
    [InlineData(71, TextureFormat.Bc1)]
    [InlineData(74, TextureFormat.Bc2)]
    [InlineData(77, TextureFormat.Bc3)]
    [InlineData(98, TextureFormat.Bc7)]
    [InlineData(99, TextureFormat.Bc7)]
    public void Dx10HeaderMapsKnownDxgiFormats(int dxgi, TextureFormat expected)
    {
        Assert.Equal(expected, TextureFormats.Probe(Dds(DDPF_FOURCC, "DX10", 0, dxgi: dxgi)).Format);
    }

    // FNA's ParseDDS only handles five DXGI codes on the DX10 path, everything else must throw there
    [Theory]
    [InlineData(28)]
    [InlineData(87)]
    [InlineData(85)]
    [InlineData(115)]
    public void Dx10HeaderRejectsWhatFnaRejects(int dxgi)
    {
        var info = TextureFormats.Probe(Dds(DDPF_FOURCC, "DX10", 0, dxgi: dxgi));

        Assert.Equal(TextureFormat.Unknown, info.Format);
        Assert.Contains(dxgi.ToString(), info.Detail);
    }

    [Fact]
    public void ThirtyTwoBitMasksSeparateBgraFromRgba()
    {
        var bgra = Dds(DDPF_RGB, null, 32, 0x00FF0000, 0x0000FF00, 0x000000FF, 0xFF000000);
        var rgba = Dds(DDPF_RGB, null, 32, 0x000000FF, 0x0000FF00, 0x00FF0000, 0xFF000000);

        Assert.Equal(TextureFormat.Bgra32, TextureFormats.Probe(bgra).Format);
        Assert.Equal(TextureFormat.Rgba32, TextureFormats.Probe(rgba).Format);
    }

    // FNA's DDPF_RGB path requires exactly 32bpp (Texture.cs:485)
    [Fact]
    public void SixteenBitRgbIsUnknown()
    {
        var info = TextureFormats.Probe(Dds(DDPF_RGB, null, 16, 0xF800, 0x07E0, 0x001F));

        Assert.Equal(TextureFormat.Unknown, info.Format);
        Assert.Contains("16bpp", info.Detail);
    }

    [Fact]
    public void EightBitLuminanceIsL8()
    {
        Assert.Equal(TextureFormat.L8, TextureFormats.Probe(Dds(DDPF_LUMINANCE, null, 8, 0xFF)).Format);
    }

    [Fact]
    public void UnrecognisedFourCcReportsItself()
    {
        var info = TextureFormats.Probe(Dds(DDPF_FOURCC, "ATI2", 0));

        Assert.Equal(TextureFormat.Unknown, info.Format);
        Assert.Contains("ATI2", info.Detail);
    }

    [Fact]
    public void DimensionsComeOffTheHeader()
    {
        var dds = TextureFormats.Probe(Dds(DDPF_FOURCC, "DXT5", 0, width: 256, height: 128));
        Assert.Equal(256, dds.Width);
        Assert.Equal(128, dds.Height);

        var png = TextureFormats.Probe(Png(512, 64));
        Assert.Equal(TextureFormat.Png, png.Format);
        Assert.Equal(512, png.Width);
        Assert.Equal(64, png.Height);
    }

    // FNA rejects a header setting both; only mppNormaliseDdsFlags at runtime saves it
    [Fact]
    public void PitchAndLinearTogetherIsFlagged()
    {
        const int both = 0x1007 | 0x8 | 0x80000;

        Assert.True(TextureFormats.Probe(Dds(DDPF_FOURCC, "DXT5", 0, headerFlags: both)).PitchAndLinear);
        Assert.False(TextureFormats.Probe(Dds(DDPF_FOURCC, "DXT5", 0)).PitchAndLinear);
    }

    [Fact]
    public void GarbageIsUnknownRatherThanAGuess()
    {
        Assert.Equal(TextureFormat.Unknown, TextureFormats.Probe(new byte[] { 1, 2, 3, 4 }).Format);
        Assert.Equal(TextureFormat.Unknown, TextureFormats.Probe(Array.Empty<byte>()).Format);
    }

    [Theory]
    [InlineData(TextureFormat.Png, ".PNG")]
    [InlineData(TextureFormat.Bc1, ".DDS")]
    [InlineData(TextureFormat.Bc3, ".DDS")]
    [InlineData(TextureFormat.Bgra32, ".DDS")]
    [InlineData(TextureFormat.Pvr, ".PVR")]
    public void NameForUsesTheFormatsExtension(TextureFormat format, string extension)
    {
        Assert.Equal("SONIC_DEF" + extension, TextureFormats.NameFor("SONIC_DEF.PNG", format));
        Assert.Equal(extension, TextureFormats.Extension(format));
    }

    // the Xbox 360 ring archive calls its member ".\RING.ENO", the prefix has to survive a rename
    [Fact]
    public void NameForKeepsADirectoryPrefix()
    {
        Assert.Equal(@".\CH_SONIC_A.DDS", TextureFormats.NameFor(@".\CH_SONIC_A.PVR", TextureFormat.Bc3));
    }

    [Fact]
    public void GpuBytesFollowTheBlockSize()
    {
        var bc1 = TextureFormats.Probe(Dds(DDPF_FOURCC, "DXT1", 0, width: 256, height: 256));
        var bc3 = TextureFormats.Probe(Dds(DDPF_FOURCC, "DXT5", 0, width: 256, height: 256));
        var png = TextureFormats.Probe(Png(256, 256));

        Assert.Equal(256 * 256 / 2, TextureFormats.GpuBytes(bc1));
        Assert.Equal(256 * 256, TextureFormats.GpuBytes(bc3));
        Assert.Equal(256 * 256 * 4, TextureFormats.GpuBytes(png));
    }

    private static byte[] Astc(int blockX, int blockY, int width, int height, int payload = 256)
    {
        var bytes = new byte[16 + payload];
        BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(0, 4), 0x5CA1AB13);
        bytes[4] = (byte)blockX;
        bytes[5] = (byte)blockY;
        bytes[6] = 1;

        Write24(bytes, 7, width);
        Write24(bytes, 10, height);
        Write24(bytes, 13, 1);

        return bytes;
    }

    private static void Write24(byte[] bytes, int offset, int value)
    {
        bytes[offset] = (byte)value;
        bytes[offset + 1] = (byte)(value >> 8);
        bytes[offset + 2] = (byte)(value >> 16);
    }

    [Theory]
    [InlineData(4, 4, TextureFormat.Astc4x4)]
    [InlineData(6, 6, TextureFormat.Astc6x6)]
    public void AstcFootprintComesFromTheBlockDimensions(int blockX, int blockY, TextureFormat expected)
    {
        var info = TextureFormats.Probe(Astc(blockX, blockY, 256, 128));

        Assert.Equal(expected, info.Format);
        Assert.Equal(256, info.Width);
        Assert.Equal(128, info.Height);
    }

    // only the two footprints the pipeline emits are known; anything else must not be guessed at
    [Fact]
    public void UnplannedAstcFootprintIsUnknownRatherThanApproximated()
    {
        var info = TextureFormats.Probe(Astc(8, 8, 256, 128));

        Assert.Equal(TextureFormat.Unknown, info.Format);
        Assert.Contains("8x8", info.Detail);
    }

    // 128 bits a block whatever the footprint, so 6x6 is the smaller upload
    [Fact]
    public void AstcGpuBytesCountsBlocksNotTexels()
    {
        Assert.Equal(64 * 32 * 16, TextureFormats.GpuBytes(TextureFormats.Probe(Astc(4, 4, 256, 128))));
        Assert.Equal(43 * 22 * 16, TextureFormats.GpuBytes(TextureFormats.Probe(Astc(6, 6, 256, 128))));
    }

    [Theory]
    [InlineData("dxt1", TextureFormat.Bc1)]
    [InlineData("bc1", TextureFormat.Bc1)]
    [InlineData("DXT5", TextureFormat.Bc3)]
    [InlineData("png", TextureFormat.Png)]
    [InlineData("astc4x4", TextureFormat.Astc4x4)]
    [InlineData("ASTC6x6", TextureFormat.Astc6x6)]
    public void TokensParseWithTheirDxtAliases(string token, TextureFormat expected)
    {
        Assert.True(TextureFormats.TryParse(token, out var format));
        Assert.Equal(expected, format);
    }

    [Fact]
    public void UnknownTokenIsRejectedRatherThanDefaulted()
    {
        Assert.False(TextureFormats.TryParse("etc2", out _));
        Assert.False(TextureFormats.TryParse("", out _));
    }
}
