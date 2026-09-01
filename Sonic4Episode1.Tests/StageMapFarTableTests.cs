using System.Linq;
using Microsoft.Xna.Framework;
using Sonic4Episode1.Viewer;

namespace Sonic4Episode1.Tests;

// The object tables decide what a background actually draws, so these pin them against the member
// counts measured out of the archives and against the oracle's scroll and camera maths.
public class StageMapFarTableTests
{
    private static int Highest(int zoneType, StageMapFarArm arm, StageMapFarSource source)
    {
        var used = StageMapFarTable.Objects(zoneType, arm)
            .Where(o => o.Source == source)
            .Select(o => o.Model)
            .ToList();

        return used.Count == 0 ? -1 : used.Max();
    }

    // measured: mobile ships 5 models and 1 render model, console 8 and 1
    [Fact]
    public void ZoneOneUsesEveryModelItsArchiveShips()
    {
        Assert.Equal(4, Highest(0, StageMapFarArm.Mobile, StageMapFarSource.Models));
        Assert.Equal(0, Highest(0, StageMapFarArm.Mobile, StageMapFarSource.RenderModels));

        Assert.Equal(7, Highest(0, StageMapFarArm.Console, StageMapFarSource.Models));
        Assert.Equal(0, Highest(0, StageMapFarArm.Console, StageMapFarSource.RenderModels));
    }

    [Fact]
    public void EveryZoneUsesEveryModelItsArchiveShips()
    {
        Assert.Equal(2, Highest(1, StageMapFarArm.Mobile, StageMapFarSource.Models));
        Assert.Equal(4, Highest(1, StageMapFarArm.Console, StageMapFarSource.Models));

        Assert.Equal(0, Highest(2, StageMapFarArm.Mobile, StageMapFarSource.Models));
        Assert.Equal(0, Highest(2, StageMapFarArm.Console, StageMapFarSource.Models));

        Assert.Equal(0, Highest(4, StageMapFarArm.Mobile, StageMapFarSource.Models));
        Assert.Equal(3, Highest(4, StageMapFarArm.Console, StageMapFarSource.Models));

        Assert.Empty(StageMapFarTable.Objects(3, StageMapFarArm.Console));
    }

    // gmMapFar.cpp:4721, `i >= ZONE_1_SEA ? MAPFAR : PRE_MAPFAR`, with SEA at 4 on console and 0 on
    // mobile. The four _B models are the reflection and have to land in the earlier bucket.
    [Fact]
    public void ZoneOneConsoleDrawsTheReflectionBeforeTheSea()
    {
        var console = StageMapFarTable.Objects(0, StageMapFarArm.Console);

        for (int i = 0; i < 4; i++)
            Assert.Equal(AppMain.OBD_DRAW_CMD_STATE_PRE_MAPFAR, console[i].Bucket);

        Assert.Equal(StageMapFarDraw.Sea, console[4].Draw);

        for (int i = 4; i < console.Length; i++)
            Assert.Equal(AppMain.OBD_DRAW_CMD_STATE_MAPFAR, console[i].Bucket);

        // the iPhone enum puts SEA at 0, so `i >= 0` is constant and PRE_MAPFAR is unreachable
        Assert.All(
            StageMapFarTable.Objects(0, StageMapFarArm.Mobile),
            o => Assert.Equal(AppMain.OBD_DRAW_CMD_STATE_MAPFAR, o.Bucket));
    }

    // only zone 2 has node motion, and only on the wheel and the searchlight
    [Fact]
    public void OnlyZoneTwoCarriesNodeMotion()
    {
        foreach (int zoneType in new[] { 0, 2, 4, 5 })
        {
            foreach (var arm in new[] { StageMapFarArm.Mobile, StageMapFarArm.Console })
                Assert.All(StageMapFarTable.Objects(zoneType, arm), o => Assert.Equal(-1, o.Motion));
        }

        var zone2 = StageMapFarTable.Objects(1, StageMapFarArm.Console);
        Assert.Equal(new[] { -1, -1, 0, -1, 1 }, zone2.Select(o => o.Motion).ToArray());
    }

    // ObjDrawAction3DNN's unconditional 90 about Y, ObjDraw.cs:371-378. The block path sets
    // NODIRFLIP and escapes it; map far mostly does not, and getting this wrong turns Lost
    // Labyrinth's backdrop side-on so you see the inside of its walls.
    [Fact]
    public void OnlyTheSkySeaAndFinalBackdropEscapeTheDirFlip()
    {
        Assert.False(StageMapFarTable.DirFlip(StageMapFarDraw.Sea, 0));
        Assert.False(StageMapFarTable.DirFlip(StageMapFarDraw.Rotate, 0));
        Assert.False(StageMapFarTable.DirFlip(StageMapFarDraw.Bg, 4));

        Assert.True(StageMapFarTable.DirFlip(StageMapFarDraw.RockA, 0));
        Assert.True(StageMapFarTable.DirFlip(StageMapFarDraw.RockB, 0));
        Assert.True(StageMapFarTable.DirFlip(StageMapFarDraw.RockC, 0));
        Assert.True(StageMapFarTable.DirFlip(StageMapFarDraw.Wheel, 1));
        Assert.True(StageMapFarTable.DirFlip(StageMapFarDraw.SearchLight, 1));
        Assert.True(StageMapFarTable.DirFlip(StageMapFarDraw.Bg, 1));
        Assert.True(StageMapFarTable.DirFlip(StageMapFarDraw.Bg, 2));
        Assert.True(StageMapFarTable.DirFlip(StageMapFarDraw.FlatBg, 0));
    }

    // the divergence that shows: mobile fills Splash Hill's horizon brown, console sky blue
    [Fact]
    public void ZoneOneClearsBrownOnMobileAndBlueOnConsole()
    {
        Assert.Equal(new Color(0x5a, 0x30, 0x10), StageMapFarTable.ClearColour(0, StageMapFarArm.Mobile));
        Assert.Equal(new Color(0x44, 0x66, 0xff), StageMapFarTable.ClearColour(0, StageMapFarArm.Console));
    }

    [Fact]
    public void ConsoleScrollsFurtherThanMobileInZonesTwoAndThree()
    {
        Assert.Equal(90, StageMapFarTable.ScrollX(1, StageMapFarArm.Mobile, false).Width);
        Assert.Equal(300, StageMapFarTable.ScrollX(1, StageMapFarArm.Console, false).Width);

        Assert.Equal(51, StageMapFarTable.ScrollY(2, StageMapFarArm.Mobile, false).Width);
        Assert.Equal(100, StageMapFarTable.ScrollY(2, StageMapFarArm.Console, false).Width);

        // zone 1 is not split, and its boss is a different table rather than a different arm
        Assert.Equal(600, StageMapFarTable.ScrollX(0, StageMapFarArm.Console, false).Width);
        Assert.Equal(10, StageMapFarTable.ScrollX(0, StageMapFarArm.Console, true).Width);
    }

    // gmMapFarCameraGetPos: linear in the range, then flat once the whole span is behind you
    [Fact]
    public void ScrollIsLinearThenClamps()
    {
        var scroll = new StageMapFarScroll(0, 600, 1);
        float speed = StageMapFarCamera.Speed(scroll, 17920f);

        Assert.Equal(0f, StageMapFarCamera.Scrolled(0f, scroll, speed), 3);
        Assert.Equal(300f, StageMapFarCamera.Scrolled(8960f, scroll, speed), 3);
        Assert.Equal(600f, StageMapFarCamera.Scrolled(17920f, scroll, speed), 3);
        Assert.Equal(600f, StageMapFarCamera.Scrolled(40000f, scroll, speed), 3);
    }

    [Fact]
    public void AZeroWidthScrollDoesNotDivideByZero()
    {
        var scroll = new StageMapFarScroll(0, 0, 1);
        Assert.Equal(0f, StageMapFarCamera.Scrolled(1234f, scroll, 0f), 3);
    }

    // the zone wrappers end on (x, y, z) <- (160, scrolled y, -scrolled x), which is why the
    // placement functions read pos.z for the horizontal position
    [Fact]
    public void TheBackgroundLivesWithDepthOnX()
    {
        var mapSize = new Vector2(17920f, 2048f);
        var position = StageMapFarCamera.Position(
            new Vector2(8960f, -1024f), mapSize,
            new StageMapFarScroll(0, 600, 1), new StageMapFarScroll(0, 20, 1));

        Assert.Equal(StageMapFarCamera.Depth, position.X, 3);
        Assert.Equal(10f, position.Y, 3);
        Assert.Equal(-300f, position.Z, 3);

        // the camera looks back down -X from 160 toward 0
        var target = StageMapFarCamera.Target(position, 0);
        Assert.Equal(0f, target.X, 3);
        Assert.Equal(position.Z, target.Z, 3);
    }
}
