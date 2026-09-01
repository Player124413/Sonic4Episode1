using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Sonic4Episode1.Viewer;

namespace Sonic4Episode1.Tests;

// GmMapGetAddMapCameraPos. A parallax layer covers the same screen travel over a shorter distance,
// so it scrolls slower, and it clamps flat once the main camera reaches either end of the map.
public class StageParallaxTests
{
    private static readonly Vector2 Lcd = StageParallax.GameCamera;
    private static readonly Vector2 Main = new Vector2(280 * 64, 32 * 64);

    // GMD_OBJ_LCD_X is GMD_CAMERA_SCALE * 720 and _Y folds in 720/1280, both truncated to short
    [Fact]
    public void TheGameCameraIsTheOneGmMainDerives()
    {
        Assert.Equal(393f, Lcd.X);
        Assert.Equal(221f, Lcd.Y);
    }

    [Fact]
    public void ALayerTheSizeOfTheMapDoesNotScroll()
    {
        var scale = StageParallax.ScrollScale(Main, Main);

        Assert.Equal(1f, scale.X, 5);
        Assert.Equal(1f, scale.Y, 5);
    }

    // a layer exactly one screen wide never moves at all, which is the far backdrop case
    [Fact]
    public void AScreenSizedLayerIsStill()
    {
        var scale = StageParallax.ScrollScale(Lcd, Main);

        Assert.Equal(0f, scale.X, 5);
        Assert.Equal(0f, scale.Y, 5);
    }

    [Fact]
    public void AHalfWidthLayerScrollsAboutHalfAsFar()
    {
        var layer = new Vector2(Main.X * 0.5f, Main.Y);
        var scale = StageParallax.ScrollScale(layer, Main);

        Assert.InRange(scale.X, 0.48f, 0.5f);
    }

    // both ends clamp, and the clamp is to the layer's own edge rather than the main map's
    [Fact]
    public void TheCameraClampsAtBothEnds()
    {
        var layer = new Vector2(Main.X * 0.5f, Main.Y * 0.5f);
        var scale = StageParallax.ScrollScale(layer, Main);

        var atLeft = StageParallax.CameraFor(new Vector2(0f, 0f), layer, Main, scale);
        Assert.Equal(Lcd.X * 0.5f, atLeft.X, 3);
        Assert.Equal(-Lcd.Y * 0.5f, atLeft.Y, 3);

        var atRight = StageParallax.CameraFor(new Vector2(Main.X, -Main.Y), layer, Main, scale);
        Assert.Equal(layer.X - Lcd.X * 0.5f, atRight.X, 3);
        Assert.Equal(-(layer.Y - Lcd.Y * 0.5f), atRight.Y, 3);
    }

    // the interesting property: at both edges of the scrollable span the layer is at its own edge,
    // so a half-size layer really does show its whole width over the main map's whole width
    [Fact]
    public void TheLayerSpansItselfExactlyAsTheMainMapSpansItself()
    {
        var layer = new Vector2(Main.X * 0.5f, Main.Y);
        var scale = StageParallax.ScrollScale(layer, Main);

        var start = StageParallax.CameraFor(new Vector2(Lcd.X * 0.5f, 0f), layer, Main, scale);
        var end = StageParallax.CameraFor(new Vector2(Main.X - Lcd.X * 0.5f, 0f), layer, Main, scale);

        Assert.Equal(Lcd.X * 0.5f, start.X, 3);
        Assert.Equal(layer.X - Lcd.X * 0.5f, end.X, 3);
    }

    [Fact]
    public void ALayerTheSizeOfTheMapTracksTheMainCamera()
    {
        var scale = StageParallax.ScrollScale(Main, Main);
        var main = new Vector2(4000f, -800f);

        var eye = StageParallax.CameraFor(main, Main, Main, scale);

        Assert.Equal(main.X, eye.X, 3);
        Assert.Equal(main.Y, eye.Y, 3);
    }

    // gm_map_addmap_camera_tbl is {2, 0, 3, 4, 5}: the M layer draws through the main camera, so it
    // is the one add layer that does not parallax
    [Fact]
    public void TheMiddleLayerDrawsThroughTheMainCamera()
    {
        Assert.Equal(new[] { 2, 0, 3, 4, 5 }, AppMain.gm_map_addmap_camera_tbl);

        var map = new StageMap(StagedRoot(), 0);
        map.Load();

        Assert.Equal(new[] { "A", "B", "N", "M", "MIPHONE" }, map.Layers.Select(l => l.Name));
        Assert.Equal(new[] { false, false, true, false, true }, map.Layers.Select(l => l.Parallax));
    }

    [Fact]
    public void StagedZone1Act1ScrollScalesAreOrdered()
    {
        var map = new StageMap(StagedRoot(), 0);
        map.Load();

        Assert.Equal(280 * 64f, map.MainMapSize.X);
        Assert.Equal(32 * 64f, map.MainMapSize.Y);

        // N is wider than the map so it scrolls faster than it; the far layers are all slower
        Assert.True(map.Layers[2].ScrollScale.X > 1f, "N should outrun the map");
        Assert.Equal(1f, map.Layers[3].ScrollScale.X, 3);
        Assert.True(map.Layers[4].ScrollScale.X < 1f, "MIPHONE should trail the map");
    }

    // Layer index order is not depth order and never was: N is the near layer at +256, in front of
    // both A (+128) and B (-128), but it is index 2. Anything that draws back to front has to sort
    // on PosZ, not walk the indices.
    [Fact]
    public void LayerIndexOrderIsNotDepthOrder()
    {
        var map = new StageMap(StagedRoot(), 0);
        map.Load();

        var byIndex = map.Layers.Select(l => l.PosZ).ToArray();
        var byDepth = byIndex.OrderBy(z => z).ToArray();

        Assert.NotEqual(byDepth, byIndex);
        Assert.Equal(new[] { 128f, -128f, 256f, -256f, -384f }, byIndex);
        Assert.Equal(new[] { -384f, -256f, -128f, 128f, 256f }, byDepth);
    }

    private static string StagedRoot()
    {
        var here = Path.GetDirectoryName(typeof(StageParallaxTests).Assembly.Location);
        var root = Path.GetFullPath(Path.Combine(here, "..", "..", "..", "..", "Content", "bin", "DesktopGL", "Content"));

        if (!Directory.Exists(root))
            throw Xunit.Sdk.SkipException.ForSkip("Content/bin/DesktopGL is not present");

        return root;
    }
}
