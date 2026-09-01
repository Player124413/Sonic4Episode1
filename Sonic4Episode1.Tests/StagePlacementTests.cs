using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Sonic4Episode1.Viewer;

namespace Sonic4Episode1.Tests;

// gmMap.cpp's block transform, and the cell walk that feeds it. The transform is the console arm's;
// gmMapGetUsePrimMatrix is the TVX arm's, and the last test here pins that the two differ by exactly
// a uniform 3.2 and nothing else.
public class StagePlacementTests
{
    private const float Block = 64f;
    private const float Scale = 3.2f;
    private const float Local = 20f;

    private static AppMain.MP_BLOCK Mp(int id, int rot = 0, int flipH = 0, int flipV = 0)
    {
        return new AppMain.MP_BLOCK((ushort)((id & 0xFFF) | (rot << 12) | (flipH << 14) | (flipV << 15)));
    }

    private static Matrix World(AppMain.MP_BLOCK mp, NNS_VECTOR node0 = null, float dx = 0f, float dy = 0f, float z = 0f)
    {
        return StagePlacement.BlockMatrix(mp, node0 ?? new NNS_VECTOR(0f, 0f, 0f), dx, dy, z);
    }

    // cell (0,0) is the top left of the grid and the map runs down in -y
    [Theory]
    [InlineData(0, 0, 32f, -32f)]
    [InlineData(1, 0, 96f, -32f)]
    [InlineData(0, 1, 32f, -96f)]
    [InlineData(279, 31, 17888f, -2016f)]
    public void ACellSitsAtItsCentre(int x, int y, float wx, float wy)
    {
        var centre = Vector3.Transform(new Vector3(10f, 10f, 0f), World(Mp(1), dx: x, dy: y, z: 128f));

        Assert.Equal(wx, centre.X, 3);
        Assert.Equal(wy, centre.Y, 3);
        Assert.Equal(128f, centre.Z, 3);
    }

    // a block is authored 20 units wide and the grid is 64, hence GMD_OBJ_DRAW_SCALE 3.2
    [Fact]
    public void ABlockFillsItsCell()
    {
        var world = World(Mp(1));

        var min = Vector3.Transform(Vector3.Zero, world);
        var max = Vector3.Transform(new Vector3(Local, Local, 0f), world);

        Assert.Equal(0f, min.X, 3);
        Assert.Equal(-Block, min.Y, 3);
        Assert.Equal(Block, max.X, 3);
        Assert.Equal(0f, max.Y, 3);
    }

    // node 0 is not always at the origin, and the recentre is what takes it out
    [Fact]
    public void NodeZeroIsTakenOutBeforeTheScale()
    {
        var node0 = new NNS_VECTOR(5f, -3f, 7f);
        var centre = Vector3.Transform(new Vector3(15f, 7f, 7f), World(Mp(1), node0));

        Assert.Equal(32f, centre.X, 3);
        Assert.Equal(-32f, centre.Y, 3);
        Assert.Equal(0f, centre.Z, 3);
    }

    // rot counts quarter turns: nnRotateZMatrix takes 0x4000 of NN's 0x10000 circle per step
    [Theory]
    [InlineData(0, 32f, 0f)]
    [InlineData(1, 0f, 32f)]
    [InlineData(2, -32f, 0f)]
    [InlineData(3, 0f, -32f)]
    public void RotCountsQuarterTurns(int rot, float ex, float ey)
    {
        var world = World(Mp(1, rot));

        var centre = Vector3.Transform(new Vector3(10f, 10f, 0f), world);
        var right = Vector3.Transform(new Vector3(Local, 10f, 0f), world);

        Assert.Equal(ex, right.X - centre.X, 3);
        Assert.Equal(ey, right.Y - centre.Y, 3);
    }

    // gmMapCreateUsePrimMatrix uses the same index, flip_h | (flip_v << 1), and the same signs
    [Theory]
    [InlineData(0, 0, 1f, 1f)]
    [InlineData(1, 0, -1f, 1f)]
    [InlineData(0, 1, 1f, -1f)]
    [InlineData(1, 1, -1f, -1f)]
    public void FlipMatchesGmMapCreateUsePrimMatrix(int flipH, int flipV, float sx, float sy)
    {
        var world = World(Mp(1, 0, flipH, flipV));

        var centre = Vector3.Transform(new Vector3(10f, 10f, 0f), world);
        var corner = Vector3.Transform(new Vector3(Local, Local, 0f), world);

        Assert.Equal(sx * 32f, corner.X - centre.X, 3);
        Assert.Equal(sy * 32f, corner.Y - centre.Y, 3);
    }

    // scale(3.2) * translate(-10,-10,0) == translate(-32,-32,0) * scale(3.2), so the console chain
    // and gmMapGetUsePrimMatrix are the same transform on vertices that differ by a uniform 3.2
    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 1, 0)]
    [InlineData(2, 0, 1)]
    [InlineData(3, 1, 1)]
    public void TheConsoleTransformIsTheTvxOneScaledBy32(int rot, int flipH, int flipV)
    {
        var mp = Mp(1, rot, flipH, flipV);

        var console = World(mp, dx: 3f, dy: 2f);
        var tvx = Matrix.CreateTranslation(-32f, -32f, 0f)
            * Matrix.CreateScale(flipH != 0 ? -1f : 1f, flipV != 0 ? -1f : 1f, 1f)
            * Matrix.CreateRotationZ(rot * MathHelper.PiOver2)
            * Matrix.CreateTranslation((3f + 0.5f) * Block, (-2f - 0.5f) * Block, 0f);

        foreach (var local in new[] { Vector3.Zero, new Vector3(Local, 0f, 0f), new Vector3(Local, Local, 0f) })
        {
            // TVX vertices are already in 0..64 where a block model's are in 0..20
            var a = Vector3.Transform(local, console);
            var b = Vector3.Transform(local * Scale, tvx);

            Assert.Equal(b.X, a.X, 3);
            Assert.Equal(b.Y, a.Y, 3);
        }
    }

    [Fact]
    public void EveryCellIsPlacedAtMostOnce()
    {
        var map = new StageMap(StagedRoot(), 0);
        map.Load();

        foreach (var layer in map.Layers.Where(l => l.Present))
        {
            var placed = StagePlacement.Place(layer, 0);
            var cells = placed.Select(b => (b.CellX, b.CellY)).ToList();

            Assert.Equal(cells.Count, cells.Distinct().Count());
            Assert.All(placed, b => Assert.InRange(b.CellX, 0, layer.Width - 1));
            Assert.All(placed, b => Assert.InRange(b.CellY, 0, layer.Height - 1));
            Assert.All(placed, b => Assert.NotEqual(0, b.Mp.id));
        }
    }

    [Fact]
    public void EveryNonEmptyCellIsPlaced()
    {
        var map = new StageMap(StagedRoot(), 0);
        map.Load();

        var layer = map.Layers[0];
        var placed = StagePlacement.Place(layer, 0)
            .Select(b => b.CellY * layer.Width + b.CellX)
            .ToHashSet();

        for (int cell = 0; cell < layer.Mp.blocks.Length; cell++)
        {
            if (layer.Mp.blocks[cell].id != 0)
                Assert.Contains(cell, placed);
        }
    }

    // An .md offset on an empty cell borrows the block it points at and places it on the cell it
    // points at, not on the cell carrying the offset. gm_map_block_check is what stops that block
    // being placed a second time when the walk reaches it directly: 3x3, block at (1,1), offset on
    // (0,0) pointing there, one placement out.
    [Fact]
    public void AnOverhangAndItsSourcePlaceOneBlockBetweenThem()
    {
        var layer = new StageLayer
        {
            Name = "test",
            Mp = Grid(3, 3, cell => cell == 4 ? Mp(7) : Mp(0)),
            Md = Offsets(3, 3, cell => cell == 0 ? (1, 1) : (0, 0)),
        };

        var placed = Assert.Single(StagePlacement.Place(layer, 0));

        Assert.Equal(7, placed.Mp.id);
        Assert.Equal(1, placed.CellX);
        Assert.Equal(1, placed.CellY);
    }

    [Fact]
    public void AnOverhangPointingAtNothingPlacesNothing()
    {
        var layer = new StageLayer
        {
            Name = "test",
            Mp = Grid(3, 3, _ => Mp(0)),
            Md = Offsets(3, 3, cell => cell == 0 ? (1, 1) : (0, 0)),
        };

        Assert.Empty(StagePlacement.Place(layer, 0));
    }

    private static AppMain.MP_HEADER Grid(int w, int h, Func<int, AppMain.MP_BLOCK> cell)
    {
        var header = new AppMain.MP_HEADER
        {
            map_w = (ushort)w,
            map_h = (ushort)h,
            blocks = new AppMain.MP_BLOCK[w * h],
        };

        for (int i = 0; i < header.blocks.Length; i++)
            header.blocks[i] = cell(i);

        return header;
    }

    private static AppMain.MD_HEADER Offsets(int w, int h, Func<int, (int X, int Y)> cell)
    {
        var header = new AppMain.MD_HEADER
        {
            map_w = (ushort)w,
            map_h = (ushort)h,
            blocks = new AppMain.MD_BLOCK[w * h],
        };

        for (int i = 0; i < header.blocks.Length; i++)
        {
            var (x, y) = cell(i);
            header.blocks[i] = new AppMain.MD_BLOCK((sbyte)((x & 0x0F) | ((y & 0x0F) << 4)));
        }

        return header;
    }

    private static string StagedRoot()
    {
        var here = Path.GetDirectoryName(typeof(StagePlacementTests).Assembly.Location);
        var root = Path.GetFullPath(Path.Combine(here, "..", "..", "..", "..", "Content", "bin", "DesktopGL", "Content"));

        if (!Directory.Exists(root))
            throw Xunit.Sdk.SkipException.ForSkip("Content/bin/DesktopGL is not present");

        return root;
    }
}
