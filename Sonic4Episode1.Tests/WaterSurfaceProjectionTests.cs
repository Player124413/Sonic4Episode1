using Xunit;

namespace Sonic4Episode1.Tests;

public class WaterSurfaceProjectionTests
{
    private const float DispHeight = 720f;

    private static float HalfViewHeight(float aspect)
    {
        AppMain.g_obj.disp_height = DispHeight;
        AppMain.ObjCameraGetOrthoExtent(AppMain.GMD_CAMERA_SCALE, aspect, out float _, out float dy);
        return dy;
    }

    private static float SurfaceRow(float water_dy, float aspect, float bb_height)
    {
        AppMain.g_obj.disp_height = DispHeight;
        AppMain.ObjCameraGetOrthoExtent(AppMain.GMD_CAMERA_SCALE, aspect, out float dx, out float dy);

        NNS_MATRIX proj = new NNS_MATRIX();
        AppMain.nnMakeOrthoMatrix(proj, -dx, dx, -dy, dy, 1f, 60000f);

        float ndc_y = proj.M11 * water_dy + proj.M13;
        return bb_height * 0.5f - ndc_y * 0.5f * bb_height;
    }

    [Fact]
    public void WaterLevelOnTheCameraCentreDrawsAcrossTheMiddle()
    {
        Assert.Equal(540f, SurfaceRow(0f, 16f / 9f, 1080f), 3);
    }

    // the ortho box is symmetric, so half the visible height is exactly the top edge at any aspect
    [Theory]
    [InlineData(16f / 9f)]
    [InlineData(2560f / 1080f)]
    [InlineData(3440f / 1440f)]
    [InlineData(4f / 3f)]
    public void WaterLevelAtTheTopOfTheViewDrawsOnTheTopEdge(float aspect)
    {
        float half = HalfViewHeight(aspect);

        Assert.Equal(0f, SurfaceRow(half, aspect, 1080f), 3);
    }

    [Fact]
    public void WaterLevelAtTheBottomOfTheViewDrawsOnTheBottomEdge()
    {
        float half = HalfViewHeight(16f / 9f);

        Assert.Equal(1080f, SurfaceRow(-half, 16f / 9f, 1080f), 3);
    }
}
