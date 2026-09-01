using Xunit;

namespace Sonic4Episode1.Tests;

// The scene used to be drawn rolled 90 degrees into a portrait framebuffer, which is an iPhone-only
// arm of the C++ that the WP7 build inherited. Unrolling it means the ortho box is objCamera.cpp
// verbatim again and GMD_CAMERA_SCALE goes back to its book value. These pin the result.
// See docs/camera-view-extent.md.
public class CameraViewExtentTests
{
    private const float DispHeight = 720f;

    // full world extent visible at a given backbuffer, x then y
    private static (float w, float h) Extent(float bbWidth, float bbHeight)
    {
        AppMain.g_obj.disp_height = DispHeight;
        AppMain.ObjCameraGetOrthoExtent(AppMain.GMD_CAMERA_SCALE, bbWidth / bbHeight, out float dx, out float dy);
        return (dx * 2f, dy * 2f);
    }

    // gmMain.h:83-84 at a 16:9 window is scale * 1280 by scale * 720. Independently cross-checked
    // by the console's own GMD_OBJ_CLIP_LCD_X/Y at gmMain.h:88-89, which are 383 and 215.
    [Fact]
    public void SixteenNineFramingMatchesTheConsole()
    {
        var (w, h) = Extent(1280f, 720f);

        Assert.Equal(383.649f, w, 3);
        Assert.Equal(215.803f, h, 3);
    }

    [Theory]
    [InlineData(1280f, 720f)]
    [InlineData(2560f, 1080f)]
    [InlineData(3440f, 1440f)]
    [InlineData(5120f, 1440f)]
    public void PixelsAreSquareAtEveryAspect(float bbWidth, float bbHeight)
    {
        var (w, h) = Extent(bbWidth, bbHeight);

        Assert.Equal(bbWidth / w, bbHeight / h, 3);
    }

    // Hor+: a wider window sees more level, it does not see less of it vertically. This is the
    // property the rotated form had backwards, because it derived the vertical from the aspect.
    [Fact]
    public void AWiderWindowShowsMoreHorizontallyAndTheSameVertically()
    {
        var wide = Extent(1280f, 720f);
        var wider = Extent(2560f, 1080f);

        Assert.Equal(wide.h, wider.h, 3);
        Assert.True(wider.w > wide.w);
    }

    [Fact]
    public void TheHorizontalExtentTracksTheAspect()
    {
        var (w, h) = Extent(2560f, 1080f);

        Assert.Equal(h * (2560f / 1080f), w, 3);
    }
}
