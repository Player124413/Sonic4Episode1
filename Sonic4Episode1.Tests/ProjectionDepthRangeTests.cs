using Xunit;

namespace Sonic4Episode1.Tests;

// nnMake*Matrix are D3DX, so they hand back z in 0..1 and the GL wrapper must not remap it.
// Both builders shipped with znear and zfar transposed in one term each, which put z in -1..0;
// PROJECTION_CORRECTION then halved it into 0..0.5, costing half the depth buffer. Retail's
// depth for stage geometry sits around 0.02, ours sat at 0.01088 for the same vertex.
public class ProjectionDepthRangeTests
{
    private const float ZNear = 1f;
    private const float ZFar = 60000f;

    private static float OrthoDepth(float view_z)
    {
        var mtx = new NNS_MATRIX();
        AppMain.nnMakeOrthoMatrix(mtx, -1f, 1f, -1f, 1f, ZNear, ZFar);
        return mtx.M22 * view_z + mtx.M23;
    }

    private static float PerspectiveDepth(float view_z)
    {
        var mtx = new NNS_MATRIX();
        AppMain.nnMakePerspectiveMatrix(mtx, AppMain.NNM_DEGtoA32(45f), 1.5f, ZNear, ZFar);
        float z = mtx.M22 * view_z + mtx.M23;
        float w = mtx.M32 * view_z;
        return z / w;
    }

    [Fact]
    public void OrthoPutsTheNearPlaneAtZeroAndTheFarPlaneAtOne()
    {
        Assert.Equal(0f, OrthoDepth(-ZNear), 5);
        Assert.Equal(1f, OrthoDepth(-ZFar), 5);
    }

    [Fact]
    public void PerspectivePutsTheNearPlaneAtZeroAndTheFarPlaneAtOne()
    {
        Assert.Equal(0f, PerspectiveDepth(-ZNear), 5);
        Assert.Equal(1f, PerspectiveDepth(-ZFar), 5);
    }

    // the vertex the depth-range bug was caught on, captured out of the vertex shader in RenderDoc
    [Fact]
    public void StageGeometryLandsWhereRetailPutsIt()
    {
        Assert.Equal(0.02175f, OrthoDepth(-1306f), 5);
    }

    [Fact]
    public void OrthoDepthIsMonotonicSoLayersStillSeparate()
    {
        float near_layer = OrthoDepth(-1178f);
        float far_layer = OrthoDepth(-1946f);
        Assert.True(near_layer < far_layer);
    }
}
