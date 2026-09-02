using Xunit;

namespace Sonic4Episode1.Tests;

// Hit rects are written in a 480x320 space that was authored against a 1080-wide 2D box (3:2 at
// 720 tall, the iPhone). The art under them is CORW_CENTER, so it moves with the window. These pin
// that a finger on the art produces the authored rect coordinate at any aspect.
// See docs/camera-view-extent.md.
public class TouchRectAlignmentTests
{
    private const float BaseW = 960f;
    private const float AuthoredWideW = AppMain.AOD_ACT_AUTHORED_WIDE_W;

    private static float WideW(float bbWidth, float bbHeight) => 720f * (bbWidth / bbHeight);

    // where a base-space x is drawn on screen, as a fraction of viewport width
    private static float ArtScreenFraction(float baseX, float wideW)
        => (baseX + (wideW - BaseW) * 0.5f) / wideW;

    // the touch coordinate screen2real produces for that screen fraction
    private static float TouchFor(float screenFraction, float wideW)
    {
        float wideX = screenFraction * wideW;
        float baseX = wideX - (wideW - BaseW) * 0.5f;
        return (baseX + (AuthoredWideW - BaseW) * 0.5f)
                * AppMain.AOD_ACT_SCREEN_WIDTH / AuthoredWideW;
    }

    // G_FIX.AMA act 48 "jump", trs.x = 920 in base space. Authored rect {400,228,472,300}. The
    // rects are hand-authored so the art is not exactly centred in them; what has to hold is that
    // a finger on the art is inside the rect, at every aspect.
    [Theory]
    [InlineData(1280f, 720f)]   // 16:9
    [InlineData(1920f, 1080f)]  // 16:9
    [InlineData(1080f, 720f)]   // 3:2, the authoring frame
    [InlineData(2560f, 1080f)]  // 21:9
    [InlineData(3440f, 1440f)]  // 21:9 ultrawide
    [InlineData(5120f, 1440f)]  // 32:9
    public void TheJumpButtonArtLandsInsideItsAuthoredRect(float bbWidth, float bbHeight)
    {
        float wideW = WideW(bbWidth, bbHeight);

        float touch = TouchFor(ArtScreenFraction(920f, wideW), wideW);

        Assert.InRange(touch, 400f, 472f);
    }

    // act 63 "da_icon_super01", trs.x = 743. Authored rect {320,228,392,300}.
    [Theory]
    [InlineData(1280f, 720f)]
    [InlineData(1080f, 720f)]
    [InlineData(2560f, 1080f)]
    [InlineData(3440f, 1440f)]
    public void TheSuperSonicArtLandsInsideItsAuthoredRect(float bbWidth, float bbHeight)
    {
        float wideW = WideW(bbWidth, bbHeight);

        float touch = TouchFor(ArtScreenFraction(743f, wideW), wideW);

        Assert.InRange(touch, 320f, 392f);
    }

    // the authoring frame is where the rects and the art were tuned against each other, so the art
    // centre should sit near the rect centre there. this is what identifies 1080 as the frame.
    [Theory]
    [InlineData(920f, 436f)]    // jump
    [InlineData(743f, 356f)]    // super sonic
    public void AtTheAuthoringFrameTheArtSitsAtTheRectCentre(float baseX, float rectCentre)
    {
        float wideW = WideW(1080f, 720f);

        float touch = TouchFor(ArtScreenFraction(baseX, wideW), wideW);

        Assert.InRange(touch, rectCentre - 1f, rectCentre + 1f);
    }

    // the whole point: the same piece of art gives the same touch coordinate whatever the window
    [Fact]
    public void TheMappingIsAspectIndependent()
    {
        float atSixteenNine = TouchFor(ArtScreenFraction(920f, WideW(1280f, 720f)), WideW(1280f, 720f));
        float atUltrawide = TouchFor(ArtScreenFraction(920f, WideW(2560f, 1080f)), WideW(2560f, 1080f));
        float atAuthored = TouchFor(ArtScreenFraction(920f, WideW(1080f, 720f)), WideW(1080f, 720f));

        Assert.Equal(atSixteenNine, atUltrawide, 3);
        Assert.Equal(atSixteenNine, atAuthored, 3);
    }

    // AoActCorReverse is the inverse, so a round trip through both must be identity
    [Theory]
    [InlineData(0f)]
    [InlineData(240f)]
    [InlineData(436f)]
    [InlineData(480f)]
    public void CorReverseInvertsTheAuthoringFrame(float touchX)
    {
        float x = touchX;
        float y = 160f;
        AppMain.AoActCorReverse(ref x, ref y);

        // back out of base space into the authoring frame
        float round = (x + (AuthoredWideW - BaseW) * 0.5f)
                * AppMain.AOD_ACT_SCREEN_WIDTH / AuthoredWideW;

        Assert.Equal(touchX, round, 3);
        Assert.Equal(360f, y, 3);
    }
}
