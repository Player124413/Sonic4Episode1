using Xunit;

namespace Sonic4Episode1.Tests;

public class CameraViewExtentTests
{
    private const float DispHeight = 720f;

    private static (float w, float h) Extent(float bbWidth, float bbHeight)
    {
        AppMain.g_obj.disp_height = DispHeight;
        AppMain.ObjCameraGetOrthoExtent(AppMain.GMD_CAMERA_SCALE, bbWidth / bbHeight, out float dx, out float dy);
        return (dx * 2f, dy * 2f);
    }

    // gmMain.h: a 16:9 window is scale*1280 by scale*720, matching GMD_OBJ_CLIP_LCD_X/Y (383, 215)
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
