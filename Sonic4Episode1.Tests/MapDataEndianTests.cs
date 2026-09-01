using System;
using System.Buffers.Binary;
using System.IO;
using System.Linq;

namespace Sonic4Episode1.Tests;

// .mp, .md, .at, .df and .di carry no magic and no version, so the only thing that says which way
// round they are is that their counts have to fill the member exactly. these pin that, and pin the
// container fallback for the degenerate buffers where both readings fit
public class MapDataEndianTests
{
    private const int Width = 280;
    private const int Height = 32;

    private static AppMain.AmbChunk Chunk(byte[] data, bool container_is_big)
    {
        var amb = new AppMain.AMS_AMB_HEADER { big_endian = container_is_big };
        return new AppMain.AmbChunk(data, 0, data.Length, amb);
    }

    private static byte[] BuildMp(int w, int h, bool big_endian, Func<int, ushort> cell)
    {
        var buffer = new byte[4 + w * h * 2];
        var span = buffer.AsSpan();

        Write16(span, 0, (ushort)w, big_endian);
        Write16(span, 2, (ushort)h, big_endian);
        for (int i = 0; i < w * h; i++)
            Write16(span, 4 + i * 2, cell(i), big_endian);

        return buffer;
    }

    private static byte[] BuildMd(int w, int h, bool big_endian, Func<int, byte> cell)
    {
        var buffer = new byte[4 + w * h];
        var span = buffer.AsSpan();

        Write16(span, 0, (ushort)w, big_endian);
        Write16(span, 2, (ushort)h, big_endian);
        for (int i = 0; i < w * h; i++)
            buffer[4 + i] = cell(i);

        return buffer;
    }

    private static byte[] BuildBlocks(int block_num, int block_size, bool big_endian)
    {
        var buffer = new byte[4 + block_num * block_size];
        var span = buffer.AsSpan();

        if (big_endian)
            BinaryPrimitives.WriteUInt32BigEndian(span, (uint)block_num);
        else
            BinaryPrimitives.WriteUInt32LittleEndian(span, (uint)block_num);

        for (int i = 4; i < buffer.Length; i++)
            buffer[i] = (byte)(i & 0xFF);

        return buffer;
    }

    private static void Write16(Span<byte> span, int offset, ushort value, bool big_endian)
    {
        if (big_endian)
            BinaryPrimitives.WriteUInt16BigEndian(span.Slice(offset, 2), value);
        else
            BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(offset, 2), value);
    }

    // an mp cell is id:12 rot:2 flip_h:1 flip_v:1, so vary all four rather than just the id
    private static ushort MpCell(int i)
    {
        return (ushort)(((i * 7) & 0x0FFF) | ((i & 3) << 12) | ((i & 1) << 14) | ((i & 2) << 14));
    }

    private static byte MdCell(int i)
    {
        return (byte)(i * 11);
    }

    [Fact]
    public void ReadsAnMpEitherWayRound()
    {
        var little = AppMain.readMPFile(Chunk(BuildMp(Width, Height, false, MpCell), false));
        var big = AppMain.readMPFile(Chunk(BuildMp(Width, Height, true, MpCell), true));

        Assert.Equal(Width, little.map_w);
        Assert.Equal(Height, little.map_h);
        Assert.Equal(little.map_w, big.map_w);
        Assert.Equal(little.map_h, big.map_h);

        for (int i = 0; i < Width * Height; i++)
        {
            Assert.Equal(little.blocks[i].id, big.blocks[i].id);
            Assert.Equal(little.blocks[i].rot, big.blocks[i].rot);
            Assert.Equal(little.blocks[i].flip_h, big.blocks[i].flip_h);
            Assert.Equal(little.blocks[i].flip_v, big.blocks[i].flip_v);
        }
    }

    [Fact]
    public void ReadsAnMdEitherWayRound()
    {
        var little = AppMain.readMDFile(Chunk(BuildMd(Width, Height, false, MdCell), false));
        var big = AppMain.readMDFile(Chunk(BuildMd(Width, Height, true, MdCell), true));

        Assert.Equal(Width, little.map_w);
        Assert.Equal(Height, little.map_h);

        for (int i = 0; i < Width * Height; i++)
        {
            Assert.Equal(little.blocks[i].ofst_x, big.blocks[i].ofst_x);
            Assert.Equal(little.blocks[i].ofst_y, big.blocks[i].ofst_y);
        }
    }

    // the container is wrong on purpose: the length identity is what decides, so a big-endian .mp
    // spliced into a little-endian archive still reads correctly
    [Fact]
    public void LengthDecidesRatherThanTheContainer()
    {
        var big = AppMain.readMPFile(Chunk(BuildMp(Width, Height, true, MpCell), container_is_big: false));
        var little = AppMain.readMPFile(Chunk(BuildMp(Width, Height, false, MpCell), container_is_big: true));

        Assert.Equal(Width, big.map_w);
        Assert.Equal(Height, big.map_h);
        Assert.Equal(Width, little.map_w);
        Assert.Equal(Height, little.map_h);
    }

    // 8x8 reads the same either way round, and then only the container knows
    [Fact]
    public void FallsBackToTheContainerWhenBothReadingsFit()
    {
        var square = BuildMp(8, 8, false, i => 0);

        Assert.Equal(8, AppMain.readMPFile(Chunk(square, false)).map_w);
        Assert.Equal(8, AppMain.readMPFile(Chunk(square, true)).map_w);
    }

    [Theory]
    [InlineData(588)]
    [InlineData(483)]
    [InlineData(285)]
    [InlineData(493)]
    public void ReadsCollisionCountsEitherWayRound(int block_num)
    {
        Assert.Equal((uint)block_num, AppMain.readATFile(Chunk(BuildBlocks(block_num, 8 * 8, false), false)).block_num);
        Assert.Equal((uint)block_num, AppMain.readATFile(Chunk(BuildBlocks(block_num, 8 * 8, true), true)).block_num);

        Assert.Equal((uint)block_num, AppMain.readDIFile(Chunk(BuildBlocks(block_num, 8 * 8, false), false)).block_num);
        Assert.Equal((uint)block_num, AppMain.readDIFile(Chunk(BuildBlocks(block_num, 8 * 8, true), true)).block_num);

        Assert.Equal((uint)block_num, AppMain.readDFFile(Chunk(BuildBlocks(block_num, 8 * 8 * 8, false), false)).block_num);
        Assert.Equal((uint)block_num, AppMain.readDFFile(Chunk(BuildBlocks(block_num, 8 * 8 * 8, true), true)).block_num);
    }

    [Fact]
    public void ReadsCollisionBodiesIdenticallyEitherWayRound()
    {
        var little = AppMain.readATFile(Chunk(BuildBlocks(4, 8 * 8, false), false));
        var big = AppMain.readATFile(Chunk(BuildBlocks(4, 8 * 8, true), true));

        for (int block = 0; block < 4; block++)
        {
            for (int row = 0; row < 8; row++)
                Assert.Equal(little.blocks[block].at[row], big.blocks[block].at[row]);
        }
    }

    // zone 1 act 1 is 280x32 in every build, and its collision is 588 blocks. the staged tree is
    // gitignored, so this is a skip rather than a fixture
    [Fact]
    public void ReadsTheStagedZone1Act1Map()
    {
        var root = ContentTree("DesktopGL");
        if (root == null)
            throw Xunit.Sdk.SkipException.ForSkip("Content/bin/DesktopGL is not present");

        var map = ReadArchive(Path.Combine(root, "G_ZONE1", "MAP", "ZONE11_MAP.AMB"));
        var attr = ReadArchive(Path.Combine(root, "G_ZONE1", "MAP", "ZONE1_ATTR.AMB"));

        foreach (var index in new[] { 0, 1, 4, 5 })
        {
            var mp = AppMain.readMPFile((AppMain.AmbChunk)AppMain.amBindGet(map, index));
            Assert.Equal(Width, mp.map_w);
            Assert.Equal(Height, mp.map_h);
        }

        foreach (var index in new[] { 2, 3 })
        {
            var md = AppMain.readMDFile((AppMain.AmbChunk)AppMain.amBindGet(map, index));
            Assert.Equal(Width, md.map_w);
            Assert.Equal(Height, md.map_h);
        }

        Assert.Equal(588u, AppMain.readATFile((AppMain.AmbChunk)AppMain.amBindGet(attr, 0)).block_num);
        Assert.Equal(588u, AppMain.readDFFile((AppMain.AmbChunk)AppMain.amBindGet(attr, 1)).block_num);
        Assert.Equal(588u, AppMain.readDIFile((AppMain.AmbChunk)AppMain.amBindGet(attr, 2)).block_num);
    }

    // gm_map_block_check was a literal short[24,24], which is exactly right for SEGA's fixed margin
    // and one short once GmMapSetDrawMarginMag widens the window. worst case is DF_SIZE + span + 7,
    // where span is the full window and 7 is the largest ofst a 4-bit signed MD_BLOCK can carry
    [Fact]
    public void BlockCheckBufferCoversTheWidestDrawWindow()
    {
        const int maxOfst = 7;

        var widest = 0;
        var tallest = 0;
        for (int size = 0; size < AppMain.GME_MAP_DRAW_SIZE_NUM; size++)
        {
            widest = Math.Max(widest, (int)AppMain.gm_map_set_draw_size[size, 0]);
            tallest = Math.Max(tallest, (int)AppMain.gm_map_set_draw_size[size, 1]);
        }

        var span = 2 * ((widest + (AppMain.GMD_MAP_DRAW_MARGIN + AppMain.GMD_MAP_DRAW_MARGIN_ADJUST_MAX) * 2) / 2);
        var tall = 2 * ((tallest + (AppMain.GMD_MAP_DRAW_MARGIN + AppMain.GMD_MAP_DRAW_MARGIN_ADJUST_MAX) * 2) / 2);

        Assert.True(AppMain.GMD_MAP_DRAW_DF_SIZE + span + maxOfst < AppMain.GMD_MAP_DRAW_CHECK_WIDTH,
            $"widest window {widest} indexes {AppMain.GMD_MAP_DRAW_DF_SIZE + span + maxOfst} into {AppMain.GMD_MAP_DRAW_CHECK_WIDTH}");
        Assert.True(AppMain.GMD_MAP_DRAW_DF_SIZE + tall + maxOfst < AppMain.GMD_MAP_DRAW_CHECK_HEIGHT,
            $"tallest window {tallest} indexes {AppMain.GMD_MAP_DRAW_DF_SIZE + tall + maxOfst} into {AppMain.GMD_MAP_DRAW_CHECK_HEIGHT}");
    }

    private static AppMain.AMS_AMB_HEADER ReadArchive(string path)
    {
        var bytes = File.ReadAllBytes(path);
        var header = new AppMain.AMS_AMB_HEADER { data = bytes };

        using var stream = new MemoryStream(bytes);
        AppMain.readAmbHeader(stream, ref header);

        return header;
    }

    private static string ContentTree(string name)
    {
        var here = Path.GetDirectoryName(typeof(MapDataEndianTests).Assembly.Location);
        var root = Path.GetFullPath(Path.Combine(here, "..", "..", "..", "..", "Content", "bin", name, "Content"));
        return Directory.Exists(root) ? root : null;
    }

    // one cell holding one record: map_w, map_h, a single offset, then the count and the record
    private static byte[] BuildCells(int record_size, bool big_endian, Action<Span<byte>> record)
    {
        var buffer = new byte[4 + 4 + 2 + record_size];
        var span = buffer.AsSpan();

        Write16(span, 0, 1, big_endian);
        Write16(span, 2, 1, big_endian);

        if (big_endian)
            BinaryPrimitives.WriteUInt32BigEndian(span.Slice(4), 8);
        else
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(4), 8);

        Write16(span, 8, 1, big_endian);
        record(span.Slice(10));

        return buffer;
    }

    // GmEventDataBuild reads .ev, .dc and .rg through their own types rather than through
    // readEVFile and friends, so they need the same sniff. reading a console .ev little-endian
    // makes map_w * map_h enormous and runs off the end of the member
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void TheEventDataReadsTheSameFromEitherByteOrder(bool big_endian)
    {
        var data = BuildCells(12, big_endian, record =>
        {
            record[0] = 3;
            record[1] = 4;
            Write16(record, 2, 0x1234, big_endian);
            Write16(record, 4, 0x5678, big_endian);
            Write16(record, 10, 0x9ABC, big_endian);
        });

        var header = new AppMain.GMS_EVE_DATA_EV_HEADER(Chunk(data, container_is_big: big_endian));

        Assert.Equal(1, header.width);
        Assert.Equal(1, header.height);
        Assert.Equal(1, header.ev_list[0].eve_num);
        Assert.Equal(3, header.ev_list[0].eve_rec[0].pos_x);
        Assert.Equal(0x1234, header.ev_list[0].eve_rec[0].id);
        Assert.Equal(0x5678, header.ev_list[0].eve_rec[0].flag);
        Assert.Equal(0x9ABC, header.ev_list[0].eve_rec[0].word_param);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void TheDecoDataReadsTheSameFromEitherByteOrder(bool big_endian)
    {
        var data = BuildCells(4, big_endian, record =>
        {
            record[0] = 5;
            record[1] = 6;
            Write16(record, 2, 0x0ABC, big_endian);
        });

        var header = new AppMain.GMS_EVE_DATA_DC_HEADER(Chunk(data, container_is_big: big_endian));

        Assert.Equal(1, header.width);
        Assert.Equal(1, header.dc_list[0].dec_num);
        Assert.Equal(5, header.dc_list[0].dec_data[0].pos_x);
        Assert.Equal(0x0ABC, header.dc_list[0].dec_data[0].id);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void TheRingDataReadsTheSameFromEitherByteOrder(bool big_endian)
    {
        var data = BuildCells(2, big_endian, record =>
        {
            record[0] = 7;
            record[1] = 8;
        });

        var header = new AppMain.GMS_EVE_DATA_RG_HEADER(Chunk(data, container_is_big: big_endian));

        Assert.Equal(1, header.width);
        Assert.Equal(1, header.ring[0].ring_num);
        Assert.Equal(7, header.ring[0].ring_data[0].pos_x);
        Assert.Equal(8, header.ring[0].ring_data[0].pos_y);
    }
}
