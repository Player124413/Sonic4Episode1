using Xunit;

namespace Sonic4Episode1.Tests;

// The game renders Hor+ up to an aspect cap and pillarboxes beyond it, so that a very wide display
// does not reveal unbounded amounts of unauthored level. See docs/camera-view-extent.md.
public class ViewportCapTests
{
    [Theory]
    [InlineData(1024, 768)]     // 4:3
    [InlineData(1280, 720)]     // 16:9
    [InlineData(1920, 1080)]    // 16:9
    [InlineData(2560, 1080)]    // 21:9, 2.370
    [InlineData(3440, 1440)]    // 21:9 ultrawide, 2.389
    public void CommonDisplaysAreNativeWithNoBars(int bbWidth, int bbHeight)
    {
        var vp = AppMain.mppCalcViewport(bbWidth, bbHeight);

        Assert.Equal(0, vp.X);
        Assert.Equal(bbWidth, vp.Width);
        Assert.Equal(bbHeight, vp.Height);
    }

    // 32:9 is 3.556, well past the cap
    [Fact]
    public void SuperUltrawideIsPillarboxedAndCentred()
    {
        var vp = AppMain.mppCalcViewport(5120, 1440);

        Assert.True(vp.Width < 5120);
        Assert.Equal(1440, vp.Height);
        Assert.Equal((5120 - vp.Width) / 2, vp.X);
        Assert.Equal(0, vp.Y);
    }

    [Fact]
    public void APillarboxedViewportSitsAtTheCapAspect()
    {
        var vp = AppMain.mppCalcViewport(5120, 1440);

        Assert.Equal(AppMain.mppAspectCap, vp.Width / (float)vp.Height, 2);
    }

    // the bars are equal, so the drawn area is centred rather than drifting to one side
    [Fact]
    public void TheBarsAreBalanced()
    {
        var vp = AppMain.mppCalcViewport(5120, 1440);

        int left = vp.X;
        int right = 5120 - (vp.X + vp.Width);
        Assert.InRange(left - right, -1, 1);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 720)]
    [InlineData(1280, 0)]
    public void ADegenerateBackBufferDoesNotThrow(int bbWidth, int bbHeight)
    {
        var vp = AppMain.mppCalcViewport(bbWidth, bbHeight);

        Assert.Equal(bbWidth, vp.Width);
        Assert.Equal(bbHeight, vp.Height);
    }
}
