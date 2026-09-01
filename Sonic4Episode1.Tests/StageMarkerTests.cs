using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Sonic4Episode1.Viewer;

namespace Sonic4Episode1.Tests;

// .ev, .rg and .dc, whose readers were mppAssertNotImpl until the leak's map_file_format.h turned
// up. One format, three payloads, on a 256x256 grid rather than the map's 64x64 one.
public class StageMarkerTests
{
    private const int Cell = 256;

    private static byte[] BuildCells(int w, int h, bool big_endian, params (int Cell, byte[][] Records)[] filled)
    {
        var lookup = filled.ToDictionary(f => f.Cell, f => f.Records);
        var body = new System.Collections.Generic.List<byte>();
        var offsets = new uint[w * h];
        int table = 4 + w * h * 4;

        for (int i = 0; i < w * h; i++)
        {
            if (!lookup.TryGetValue(i, out var records))
                continue;

            offsets[i] = (uint)(table + body.Count);
            body.AddRange(Bytes((ushort)records.Length, big_endian));
            foreach (var record in records)
                body.AddRange(record);
        }

        var buffer = new System.Collections.Generic.List<byte>();
        buffer.AddRange(Bytes((ushort)w, big_endian));
        buffer.AddRange(Bytes((ushort)h, big_endian));
        foreach (var offset in offsets)
            buffer.AddRange(Bytes(offset, big_endian));
        buffer.AddRange(body);

        return buffer.ToArray();
    }

    private static byte[] Bytes(ushort value, bool big) =>
        big ? new[] { (byte)(value >> 8), (byte)value } : new[] { (byte)value, (byte)(value >> 8) };

    private static byte[] Bytes(uint value, bool big) => big
        ? new[] { (byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value }
        : new[] { (byte)value, (byte)(value >> 8), (byte)(value >> 16), (byte)(value >> 24) };

    private static byte[] Event(byte x, byte y, ushort id, ushort flag, sbyte rx, sbyte ry, byte rw, byte rh, ushort user, bool big)
    {
        var record = new System.Collections.Generic.List<byte> { x, y };
        record.AddRange(Bytes(id, big));
        record.AddRange(Bytes(flag, big));
        record.AddRange(new[] { (byte)rx, (byte)ry, rw, rh });
        record.AddRange(Bytes(user, big));
        return record.ToArray();
    }

    private static AppMain.AmbChunk Chunk(byte[] data, bool container_is_big)
    {
        var amb = new AppMain.AMS_AMB_HEADER { big_endian = container_is_big };
        return new AppMain.AmbChunk(data, 0, data.Length, amb);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ReadsAnEventEitherWayRound(bool big)
    {
        var data = BuildCells(4, 2, big, (3, new[]
        {
            Event(10, 20, 0x1234, 5, -2, -3, 40, 50, 0xBEEF, big),
            Event(200, 201, 7, 0, 0, 0, 0, 0, 0, big),
        }));

        // the container is deliberately the wrong way round: the length identity decides
        var ev = AppMain.readEVFile(Chunk(data, !big));

        Assert.Equal(4, ev.map_w);
        Assert.Equal(2, ev.map_h);
        Assert.Equal(8, ev.cells.Length);
        Assert.All(ev.cells.Where((_, i) => i != 3), cell => Assert.Empty(cell));

        var first = ev.cells[3][0];
        Assert.Equal(10, first.x);
        Assert.Equal(20, first.y);
        Assert.Equal(0x1234, first.id);
        Assert.Equal(5, first.flag);
        Assert.Equal(-2, first.rect_x);
        Assert.Equal(-3, first.rect_y);
        Assert.Equal(40, first.rect_w);
        Assert.Equal(50, first.rect_h);
        Assert.Equal(0xBEEF, first.user);

        Assert.Equal(7, ev.cells[3][1].id);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ReadsDecorationsAndRingsEitherWayRound(bool big)
    {
        var dcRecord = new System.Collections.Generic.List<byte> { 1, 2 };
        dcRecord.AddRange(Bytes((ushort)99, big));

        var dc = AppMain.readDCFile(Chunk(BuildCells(2, 2, big, (0, new[] { dcRecord.ToArray() })), !big));
        Assert.Equal(99, dc.cells[0][0].id);
        Assert.Equal(1, dc.cells[0][0].x);
        Assert.Equal(2, dc.cells[0][0].y);

        var rg = AppMain.readRGFile(Chunk(BuildCells(2, 2, big, (3, new[] { new byte[] { 7, 8 } })), !big));
        Assert.Equal(7, rg.cells[3][0].x);
        Assert.Equal(8, rg.cells[3][0].y);
    }

    // measured across mobile, xbla and pc: 70 x 8 cells of 256, which is exactly the 280 x 32
    // blocks of 64 the map grid has
    [Fact]
    public void TheStagedZone1Act1EventGridCoversTheMap()
    {
        var map = Load();

        Assert.Equal(70, map.Ev.map_w);
        Assert.Equal(8, map.Ev.map_h);
        Assert.Equal(map.Ev.map_w * StageMarkers.CellSize, map.MainMapSize.X);
        Assert.Equal(map.Ev.map_h * StageMarkers.CellSize, map.MainMapSize.Y);

        Assert.Equal(map.Ev.map_w, map.Dc.map_w);
        Assert.Equal(map.Ev.map_w, map.Rg.map_w);
    }

    [Fact]
    public void TheStagedZone1Act1CountsMatchWhatTheFilesHold()
    {
        var map = Load();
        var markers = StageMarkers.Build(map);

        Assert.Equal(192, markers.Count(m => m.Kind == StageMarkerKind.Event));
        Assert.Equal(400, markers.Count(m => m.Kind == StageMarkerKind.Decoration));
        Assert.Equal(296, markers.Count(m => m.Kind == StageMarkerKind.Ring));
    }

    // x and y are within the cell, so every marker must land inside the map
    [Fact]
    public void EveryMarkerLandsOnTheMap()
    {
        var map = Load();

        foreach (var marker in StageMarkers.Build(map))
        {
            Assert.InRange(marker.Position.X, 0f, map.MainMapSize.X);
            Assert.InRange(marker.Position.Y, -map.MainMapSize.Y, 0f);
        }
    }

    [Fact]
    public void EveryStagedStageReadsItsMarkers()
    {
        var root = StagedRoot();

        for (int i = 0; i < StageTable.Count; i++)
        {
            if (!StageTable.HasBlockMap(i))
                continue;

            var map = new StageMap(root, i);
            map.Load();

            Assert.Equal(map.Ev.map_w, map.Dc.map_w);
            Assert.Equal(map.Ev.map_h, map.Dc.map_h);
            Assert.Equal(map.Ev.map_w, map.Rg.map_w);
            Assert.Equal(map.Ev.map_h, map.Rg.map_h);

            // the event grid is the map grid rounded up to whole 256 unit cells
            int cells = (map.AttrA.map_w * AppMain.GMD_MAP_BLOCK_SIZE + 255) / 256;
            Assert.Equal(cells, map.Ev.map_w);
        }
    }

    private static StageMap Load()
    {
        var map = new StageMap(StagedRoot(), 0);
        map.Load();
        return map;
    }

    private static string StagedRoot()
    {
        var here = Path.GetDirectoryName(typeof(StageMarkerTests).Assembly.Location);
        var root = Path.GetFullPath(Path.Combine(here, "..", "..", "..", "..", "Content", "bin", "DesktopGL", "Content"));

        if (!Directory.Exists(root))
            throw Xunit.Sdk.SkipException.ForSkip("Content/bin/DesktopGL is not present");

        return root;
    }
}
