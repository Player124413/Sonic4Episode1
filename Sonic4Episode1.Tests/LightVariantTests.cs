using System.IO;

namespace Sonic4Episode1.Tests;

[Collection(FsVariantCollection.Name)]
public class LightVariantTests
{
    private const int Stage1_1 = 0;
    private const int Zone4_1 = 12;
    private const int Zone4_3 = 14;
    private const int Final_1 = 16;

    private static void Load(params string[] lines)
    {
        AppMain.amFsLoadVariants(new StringReader(string.Join("\n", lines)));
    }

    private static string[] MapGroup(int stage_id)
    {
        var info = AppMain.gm_gamedat_tbl_map_info_tbl[stage_id];
        var paths = new System.Collections.Generic.List<string>();

        for (int index = 0; index < info.num; ++index)
        {
            if (AppMain.gmMapOwnsLoad(info.data_tbl[index]))
                paths.Add(info.data_tbl[index].path + " console");
        }

        return paths.ToArray();
    }

    // runs gmMainInitLight for one stage and hands back the two lights it pushed, restoring the
    // globals it had to move
    private static (NNS_VECTOR def, NNS_VECTOR ply, float ply_intensity) InitLight(int stage_id)
    {
        var saved = AppMain.g_gs_main_sys_info.stage_id;
        var saved_variant = AppMain.gm_main_light_variant;

        try
        {
            AppMain.g_gs_main_sys_info.stage_id = (ushort)stage_id;
            AppMain.gmMainSelectLightVariant(stage_id);
            AppMain.gmMainInitLight();

            return (new NNS_VECTOR().Assign(AppMain.g_gm_main_system.def_light_vec),
                    new NNS_VECTOR().Assign(AppMain.g_gm_main_system.ply_light_vec),
                    AppMain.g_obj.light[AppMain.NNE_LIGHT_6].parallel.Intensity);
        }
        finally
        {
            AppMain.g_gs_main_sys_info.stage_id = saved;
            AppMain.gm_main_light_variant = saved_variant;
        }
    }

    private static void AssertDirection(float x, float y, float z, NNS_VECTOR actual)
    {
        var expected = new NNS_VECTOR(x, y, z);
        AppMain.nnNormalizeVector(expected, expected);

        Assert.Equal(expected.x, actual.x, 5);
        Assert.Equal(expected.y, actual.y, 5);
        Assert.Equal(expected.z, actual.z, 5);
    }

    [Fact]
    public void MobileDataKeepsTheHeadOnLight()
    {
        Load();

        var light = InitLight(Stage1_1);

        AssertDirection(0f, 0f, -1f, light.def);
        AssertDirection(-0.5f, -0.4f, -0.25f, light.ply);
        Assert.Equal(1.5f, light.ply_intensity, 5);
    }

    [Fact]
    public void ConsoleDataTakesTheDiagonalAndThePlayerFollowsIt()
    {
        Load(MapGroup(Stage1_1));

        var light = InitLight(Stage1_1);

        AssertDirection(-1f, -1f, -1f, light.def);
        AssertDirection(-1f, -1f, -1f, light.ply);
        Assert.Equal(1f, light.ply_intensity, 5);
    }

    [Fact]
    public void Zone4KeepsItsOwnDirectionOnBothArms()
    {
        Load();
        AssertDirection(-0.95f, 0.25f, -1f, InitLight(Zone4_1).def);

        Load(MapGroup(Zone4_1));
        AssertDirection(-0.95f, 0.25f, -1f, InitLight(Zone4_1).def);
    }

    [Fact]
    public void Zone4PlayerOverrideIsMobileOnly()
    {
        Load();
        AssertDirection(0.05f, 0.15f, -0.05f, InitLight(Zone4_1).ply);

        Load(MapGroup(Zone4_1));
        AssertDirection(-0.95f, 0.25f, -1f, InitLight(Zone4_1).ply);
    }

    // GMD_LIGHT_PLY_DARK_INTENSITY is 1.0 on both, so 4-3 must not pick up the 1.5
    [Fact]
    public void Zone4_3IntensityIsSharedByBothArms()
    {
        Load();
        Assert.Equal(1f, InitLight(Zone4_3).ply_intensity, 5);

        Load(MapGroup(Zone4_3));
        Assert.Equal(1f, InitLight(Zone4_3).ply_intensity, 5);
    }

    // retail folds this branch away because both arms produce the same vector; keeping it must not
    // make the stage follow the variant
    [Fact]
    public void Final1IsTheDiagonalOnBothArms()
    {
        Load();
        AssertDirection(-1f, -1f, -1f, InitLight(Final_1).def);

        Load(MapGroup(Final_1));
        AssertDirection(-1f, -1f, -1f, InitLight(Final_1).def);
    }

    [Fact]
    public void SelectionFollowsTheMapGroup()
    {
        Load(MapGroup(Stage1_1));
        AppMain.gmMainSelectLightVariant(Stage1_1);
        Assert.Same(AppMain.gm_main_light_variant_console, AppMain.gm_main_light_variant);

        Load();
        AppMain.gmMainSelectLightVariant(Stage1_1);
        Assert.Same(AppMain.gm_main_light_variant_mobile, AppMain.gm_main_light_variant);
    }
}
