namespace Sonic4Episode1.Tests;

// AMS_PARAM_DRAW_PRIMITIVE is a memset stack local in the C++ and a pooled object here, so every
// field a draw path leaves unset has to come back zeroed, and a copy has to copy every field.
public class DrawPrimitiveParamTests
{
    private static AppMain.AMS_PARAM_DRAW_PRIMITIVE Filled()
    {
        return new AppMain.AMS_PARAM_DRAW_PRIMITIVE
        {
            mtx = new NNS_MATRIX(),
            type = 1,
            count = 12,
            texId = 7,
            ablend = 1,
            sortZ = 3.5f,
            bldSrc = 770,
            bldDst = 771,
            bldMode = 32774,
            aTest = 1,
            zMask = 1,
            zTest = 1,
            noSort = 1,
            uwrap = 1,
            vwrap = 0,
            format3D = 4,
        };
    }

    [Fact]
    public void Assign_copies_uwrap_and_vwrap_independently()
    {
        var source = Filled();
        var copy = new AppMain.AMS_PARAM_DRAW_PRIMITIVE();

        copy.Assign(source);

        Assert.Equal(1, copy.uwrap);
        Assert.Equal(0, copy.vwrap);
    }

    [Fact]
    public void Assign_copies_every_field()
    {
        var source = Filled();
        var copy = new AppMain.AMS_PARAM_DRAW_PRIMITIVE();

        copy.Assign(source);

        Assert.Same(source.mtx, copy.mtx);
        Assert.Equal(source.type, copy.type);
        Assert.Equal(source.count, copy.count);
        Assert.Equal(source.texId, copy.texId);
        Assert.Equal(source.ablend, copy.ablend);
        Assert.Equal(source.sortZ, copy.sortZ);
        Assert.Equal(source.bldSrc, copy.bldSrc);
        Assert.Equal(source.bldDst, copy.bldDst);
        Assert.Equal(source.bldMode, copy.bldMode);
        Assert.Equal(source.aTest, copy.aTest);
        Assert.Equal(source.zMask, copy.zMask);
        Assert.Equal(source.zTest, copy.zTest);
        Assert.Equal(source.noSort, copy.noSort);
        Assert.Equal(source.format3D, copy.format3D);
    }

    [Fact]
    public void Clear_zeroes_the_fields_a_sprite_draw_leaves_alone()
    {
        var param = Filled();

        param.Clear();

        Assert.Null(param.mtx);
        Assert.Equal(0, param.noSort);
        Assert.Equal(0, param.uwrap);
        Assert.Equal(0, param.vwrap);
        Assert.Equal(0, param.bldSrc);
        Assert.Equal(0, param.bldDst);
        Assert.Equal(0, param.bldMode);
    }
}
