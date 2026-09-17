using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Sonic4Episode1.Tests;

[Collection(FsVariantCollection.Name)]
public class MapVariantTests
{
    private const int Stage1_1 = 0;
    private const int Stage1_Boss = 3;
    private const int Stage4_1 = 12;

    private static readonly int[] Zone3Stages = { 8, 9, 10, 11 };

    private const string Zone1MapFar = "G_ZONE1/MAPFAR/ZONE1_MAPFAR.AMB";

    private static void Load(params string[] lines)
    {
        AppMain.amFsLoadVariants(new StringReader(string.Join("\n", lines)));
    }

    private static string[] GroupFor(int stage_id)
    {
        var info = AppMain.gm_gamedat_tbl_map_info_tbl[stage_id];
        var paths = new List<string>();

        for (int index = 0; index < info.num; ++index)
        {
            var load_data = info.data_tbl[index];
            if (AppMain.gmMapOwnsLoad(load_data))
                paths.Add(load_data.path);
        }

        return paths.ToArray();
    }

    private static string[] Declare(string variant, params int[] stage_ids)
    {
        return stage_ids
            .SelectMany(GroupFor)
            .Distinct()
            .Select(path => path + " " + variant)
            .ToArray();
    }

    private static string[] DeclareConsole(params int[] stage_ids)
    {
        return Declare(AppMain.AMD_FS_VARIANT_CONSOLE, stage_ids);
    }

    // the four map archives plus the background. deco is a separate subsystem with its own variant
    // story and must not be dragged into this gate
    [Fact]
    public void TheGroupIsTheMapArchivesAndTheBackground()
    {
        var group = GroupFor(Stage1_1);

        Assert.Equal(
            new[]
            {
                "G_ZONE1/MAP/ZONE11_MAP.AMB",
                "G_ZONE1/MAP/ZONE1_M.AMB",
                "G_ZONE1/MAP/ZONE1_T.AMB",
                "G_ZONE1/MAP/ZONE1_ATTR.AMB",
                Zone1MapFar,
            },
            group);

        Assert.DoesNotContain(group, path => path.Contains("/DECO/"));
    }

    [Fact]
    public void EveryArchiveInTheGroupHasToBeDeclared()
    {
        Load(DeclareConsole(Stage1_1));
        Assert.True(AppMain.gmMapIsConsoleDataComplete(Stage1_1));

        foreach (var dropped in GroupFor(Stage1_1))
        {
            Load(DeclareConsole(Stage1_1).Where(line => !line.StartsWith(dropped + " ")).ToArray());

            Assert.False(AppMain.gmMapIsConsoleDataComplete(Stage1_1));
        }
    }

    [Fact]
    public void NoManifestMeansMobile()
    {
        Load();

        Assert.False(AppMain.gmMapIsConsoleDataComplete(Stage1_1));
        Assert.False(AppMain.gmMapIsConsoleDataComplete(Stage1_Boss));
    }

    // zone 4 has no background of its own and loads zone 1's, so declaring the zone 1 group must not
    // drag zone 4 onto the console arm with mobile blocks
    [Fact]
    public void ZoneFourStaysMobileOnZoneOnesBackgroundAlone()
    {
        Assert.Contains(Zone1MapFar, GroupFor(Stage4_1));

        Load(DeclareConsole(Stage1_1, Stage1_Boss));

        Assert.True(AppMain.gmMapIsConsoleDataComplete(Stage1_1));
        Assert.False(AppMain.gmMapIsConsoleDataComplete(Stage4_1));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void VariantDescriptorIsFullyPopulated(bool console)
    {
        var variant = AppMain.MapVariantForTest(console);

        Assert.NotNull(variant);
        Assert.NotNull(variant.fs_variant);
        Assert.True(variant.registlist_num > 0);
    }

    // amDraw.h:91-95 is #if _IPHONE (256) #else (128), and the registration gate takes 64 off it
    [Fact]
    public void TheTwoArmsDifferWhereTheCppSaysAndNowhereElse()
    {
        var mobile = AppMain.MapVariantForTest(false);
        var console = AppMain.MapVariantForTest(true);

        Assert.False(mobile.blocks_as_model);
        Assert.True(console.blocks_as_model);

        Assert.Equal(256, mobile.registlist_num);
        Assert.Equal(128, console.registlist_num);

        Assert.Equal(AppMain.AMD_FS_VARIANT_BASE, mobile.fs_variant);
        Assert.Equal(AppMain.AMD_FS_VARIANT_CONSOLE, console.fs_variant);
    }

    // gmMap.cpp:1281-1284, literal 0x00D13100 at 0x4CF768 in the retail binary
    [Fact]
    public void TheBlockDispFlagMatchesTheOracle()
    {
        uint disp_flag = AppMain.OBD_DISP_NOPOS | AppMain.OBD_DISP_NOSCALE
            | AppMain.OBD_DISP_NODRAWSCALE | AppMain.OBD_DISP_NODIR
            | AppMain.OBD_DISP_NODIRFLIP | AppMain.OBD_DISP_USERMTX | AppMain.OBD_DISP_NOUPDATE;

        Assert.Equal(0x00D13100u, disp_flag);
        Assert.NotEqual(0x00D12100u, disp_flag);
    }

    // one buffer serves both arms, so it is sized off the widest window either uses: the console
    // gmMapMain pushes gm_map_draw_size to 8 for zone 2-2. measured 28 in the retail binary
    [Fact]
    public void TheCheckGridCoversTheWidestWindow()
    {
        Assert.Equal(28, AppMain.GMD_MAP_DRAW_CHECK_WIDTH);
        Assert.Equal(28, AppMain.GMD_MAP_DRAW_CHECK_HEIGHT);
    }

    // the resumable registration bails with fewer than 64 free slots, off a list that is half the
    // size on console
    [Fact]
    public void TheRegistrationGateIsSixtyFourFreeSlotsOnEitherArm()
    {
        Assert.Equal(192, AppMain.MapVariantForTest(false).registlist_num - 64);
        Assert.Equal(64, AppMain.MapVariantForTest(true).registlist_num - 64);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void MapFarVariantIsFullyPopulated(bool console)
    {
        var variant = AppMain.MapFarVariantForTest(console);

        Assert.NotNull(variant.zone_1_obj_data);
        Assert.NotNull(variant.zone_1_obj_data_mat_motion);
        Assert.NotNull(variant.zone_1_obj_func_main);
        Assert.NotNull(variant.zone_1_obj_func_out);
        Assert.NotNull(variant.clear_color);
        Assert.NotNull(variant.fog_enabled);

        int count = variant.zone_1_obj_data.Length;
        Assert.Equal(count, variant.zone_1_obj_data_mat_motion.Length);
        Assert.Equal(count, variant.zone_1_obj_func_main.Length);
        Assert.Equal(count, variant.zone_1_obj_func_out.Length);

        Assert.Equal(6, variant.clear_color.Length);
        Assert.Equal(6, variant.fog_enabled.Length);
    }

    // gmMapFar.cpp:144-165. console drops SEA to index 4 and puts the mirrored group in front of it,
    // and GMD_MAP_FAR_2DBG is 0 there so ZONE_1_MAX is 9 against mobile's 6. the retail binary has
    // a tenth entry for a stage our load table does not have; the leak does not
    [Fact]
    public void TheConsoleZoneOneTableCarriesTheMirroredGroup()
    {
        var mobile = AppMain.MapFarVariantForTest(false);
        var console = AppMain.MapFarVariantForTest(true);

        Assert.Equal(6, mobile.zone_1_obj_data.Length);
        Assert.Equal(9, console.zone_1_obj_data.Length);

        Assert.Equal(new uint[] { 0, 1, 2, 3, 0, 4, 5, 6, 7 }, console.zone_1_obj_data);

        // the sea comes out of the render list, and everything before it is the mirrored group
        Assert.Equal(4, console.zone_1_render_index);
        Assert.Equal(4, console.zone_1_pre_mapfar_below);
        Assert.Equal(4, console.zone_1_sea_index);
        Assert.Equal(5, console.zone_1_sky_index);

        // mobile puts SEA at 0, which makes the PRE_MAPFAR arm unreachable
        Assert.Equal(0, mobile.zone_1_render_index);
        Assert.Equal(0, mobile.zone_1_pre_mapfar_below);
        Assert.Equal(-1, mobile.zone_1_skip_index);
        Assert.Equal(-1, console.zone_1_skip_index);

        // mobile has no mirrored group at all; console draws it into PRE_MAPFAR for the sea to
        // reflect, and gmMapFarDrawSeaUserFunc is what keeps it off the visible frame
        Assert.False(mobile.zone_1_draws_mirror_group);
        Assert.True(console.zone_1_draws_mirror_group);
    }

    // gmMapFar.cpp:999-1014 forks zone 1 and zone 4 only; the rest are shared
    [Fact]
    public void TheClearColoursForkWhereTheCppSays()
    {
        var mobile = AppMain.MapFarVariantForTest(false).clear_color;
        var console = AppMain.MapFarVariantForTest(true).clear_color;

        Assert.Equal(0x44, console[AppMain.GSD_MAIN_ZONE_TYPE_1].r);
        Assert.Equal(0x66, console[AppMain.GSD_MAIN_ZONE_TYPE_1].g);
        Assert.Equal(0xFF, console[AppMain.GSD_MAIN_ZONE_TYPE_1].b);

        Assert.Equal(90, mobile[AppMain.GSD_MAIN_ZONE_TYPE_1].r);

        Assert.Equal(0x44, console[AppMain.GSD_MAIN_ZONE_TYPE_4].r);
        Assert.Equal(0xFF, console[AppMain.GSD_MAIN_ZONE_TYPE_4].g);

        for (int zone = 0; zone < 6; zone++)
        {
            if (zone == AppMain.GSD_MAIN_ZONE_TYPE_1 || zone == AppMain.GSD_MAIN_ZONE_TYPE_4
                || zone == AppMain.GSD_MAIN_ZONE_TYPE_SS)
                continue;

            Assert.Equal(mobile[zone].r, console[zone].r);
            Assert.Equal(mobile[zone].g, console[zone].g);
            Assert.Equal(mobile[zone].b, console[zone].b);
        }
    }

    [Fact]
    public void ConsoleLightsTheFogEverywhereMobileDoesNot()
    {
        var mobile = AppMain.MapFarVariantForTest(false);
        var console = AppMain.MapFarVariantForTest(true);

        Assert.Equal(new[] { true, false, false, false, false, true }, mobile.fog_enabled);
        Assert.Equal(new[] { true, true, true, false, true, true }, console.fog_enabled);

        Assert.True(mobile.fog_off_at_zone_1_boss);
        Assert.False(console.fog_off_at_zone_1_boss);

        Assert.True(mobile.applies_ss_startframe_hack);
        Assert.False(console.applies_ss_startframe_hack);
    }

    // the four per-object forks that were still preprocessor blocks. mobile replaces the sky, sea
    // and rocks at the zone 1 boss with a flat backdrop; console draws all three
    [Fact]
    public void TheZoneOneBossForksMatchTheCpp()
    {
        var mobile = AppMain.MapFarVariantForTest(false);
        var console = AppMain.MapFarVariantForTest(true);

        Assert.True(mobile.zone_1_sky_hidden_at_boss);
        Assert.False(console.zone_1_sky_hidden_at_boss);

        Assert.False(mobile.zone_1_draws_extra_boss_rocks);
        Assert.True(console.zone_1_draws_extra_boss_rocks);

        Assert.NotNull(mobile.zone_1_sea_is_hidden);
        Assert.NotNull(console.zone_1_sea_is_hidden);
        Assert.NotSame(mobile.zone_1_sea_is_hidden, console.zone_1_sea_is_hidden);

        Assert.NotSame(mobile.zone_1_rocks_are_hidden, console.zone_1_rocks_are_hidden);
        Assert.NotSame(mobile.zone_2_night_view_is_hidden, console.zone_2_night_view_is_hidden);
    }

    // measured against the console archives: zone 2's MDL is SKY, GROUND, WHEEL, GLARE, SLIGHT, its
    // MTN is WHEEL, SLIGHT and its MAT is GROUND, WHEEL, GLARE. zone 3 gains a material motion it
    // has no members for on mobile, and the final zone goes from one backdrop to four objects
    [Fact]
    public void TheOtherZoneTablesGrowWhereTheContentDoes()
    {
        var mobile = AppMain.MapFarVariantForTest(false);
        var console = AppMain.MapFarVariantForTest(true);

        Assert.Equal(3, mobile.zone_2_obj_data.Length);
        Assert.Equal(5, console.zone_2_obj_data.Length);
        Assert.Equal(new uint[] { 0, 1, 2, 3, 4 }, console.zone_2_obj_data);
        Assert.Equal(
            new[] { uint.MaxValue, uint.MaxValue, 0u, uint.MaxValue, 1u },
            console.zone_2_obj_data_motion);
        Assert.Equal(
            new[] { uint.MaxValue, 0u, 1u, 2u, uint.MaxValue },
            console.zone_2_obj_data_mat_motion);

        Assert.False(mobile.zone_2_scales_sky);
        Assert.True(console.zone_2_scales_sky);

        // INVALID against Z3_FAR_ZNV
        Assert.Equal(uint.MaxValue, mobile.zone_3_obj_data_mat_motion[0]);
        Assert.Equal(0u, console.zone_3_obj_data_mat_motion[0]);

        Assert.Single(mobile.zone_final_obj_data);
        Assert.Equal(4, console.zone_final_obj_data.Length);
        Assert.False(mobile.zone_final_rotates_per_object);
        Assert.True(console.zone_final_rotates_per_object);
    }

    // gmMapFar.cpp:841-848. mobile stands Lost Labyrinth's backdrop up as one flat quad through
    // Draw2DBG, console draws the real model through DrawBg, and each of those carries its own
    // per-stage cull windows tuned against its own arm's scroll width. running the mobile pair on
    // console put the windows in a 678 wide space the camera only reaches 600 of
    [Fact]
    public void ZoneThreeDrawsThroughItsOwnFunctionOnEachArm()
    {
        var mobile = AppMain.MapFarVariantForTest(false);
        var console = AppMain.MapFarVariantForTest(true);

        Assert.Equal("gmMapFarSceneObjFuncDraw2DBG", mobile.zone_3_obj_func_out[0].Method.Name);
        Assert.Equal("gmMapFarSceneObjFuncDrawBg", console.zone_3_obj_func_out[0].Method.Name);
    }

    // every per-zone table has to agree in length with its siblings or the init loops walk off one
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void EveryZoneTableAgreesWithItsSiblings(bool console)
    {
        var variant = AppMain.MapFarVariantForTest(console);

        int zone2 = variant.zone_2_obj_data.Length;
        Assert.Equal(zone2, variant.zone_2_obj_data_motion.Length);
        Assert.Equal(zone2, variant.zone_2_obj_data_mat_motion.Length);
        Assert.Equal(zone2, variant.zone_2_obj_func_main.Length);
        Assert.Equal(zone2, variant.zone_2_obj_func_out.Length);

        int zoneF = variant.zone_final_obj_data.Length;
        Assert.Equal(zoneF, variant.zone_final_obj_func_main.Length);
        Assert.Equal(zoneF, variant.zone_final_obj_func_out.Length);

        Assert.NotEmpty(variant.zone_3_obj_data_mat_motion);
        Assert.Equal(
            variant.zone_3_obj_data_mat_motion.Length,
            variant.zone_3_obj_func_out.Length);

        Assert.NotEmpty(variant.zone_2_scroll_x);
        Assert.NotEmpty(variant.zone_2_scroll_y);
        Assert.NotEmpty(variant.zone_2_boss_scroll_y);
        Assert.NotEmpty(variant.zone_3_scroll_x);
        Assert.NotEmpty(variant.zone_3_scroll_y);
    }

    // gmMapFar.cpp:774-808 and :851-873. five scroll values fork and the rest do not, so a shared
    // table here would be as wrong as a forked one anywhere else
    [Fact]
    public void TheScrollWidthsForkWhereTheCppSays()
    {
        var mobile = AppMain.MapFarVariantForTest(false);
        var console = AppMain.MapFarVariantForTest(true);

        Assert.Equal(90, mobile.zone_2_scroll_x[0].width);
        Assert.Equal(300, console.zone_2_scroll_x[0].width);
        Assert.Equal(40, mobile.zone_2_scroll_y[0].width);
        Assert.Equal(50, console.zone_2_scroll_y[0].width);
        Assert.Equal(40, mobile.zone_2_boss_scroll_y[0].width);
        Assert.Equal(50, console.zone_2_boss_scroll_y[0].width);

        Assert.Equal(678, mobile.zone_3_scroll_x[0].width);
        Assert.Equal(600, console.zone_3_scroll_x[0].width);
        Assert.Equal(51, mobile.zone_3_scroll_y[0].width);
        Assert.Equal(100, console.zone_3_scroll_y[0].width);

        // zone 2's boss x is shared, and so is every zone 1, 4, final and SS value
        Assert.Equal(20, AppMain.g_map_far_zone_2_boss_scroll_x[0].width);
        Assert.Equal(600, AppMain.g_map_far_zone_1_scroll_x[0].width);
        Assert.Equal(20, AppMain.g_map_far_zone_1_scroll_y[0].width);
    }

    // gmMapFarCameraGetPos clamps to the scroll width, so an arm's cull windows are only reachable
    // if they were tuned against that arm's own scroll table.
    // 2-1 is the stage to measure on: every one of its windows bounds z, where 2-3 opens with a
    // bare y test that covers the whole span
    [Theory]
    [InlineData(true, 4)]
    [InlineData(false, 3)]
    public void EveryNightViewWindowIsReachableOnItsOwnArm(bool console, int windows)
    {
        var variant = AppMain.MapFarVariantForTest(console);
        float zSpan = variant.zone_2_scroll_x[0].width;
        float ySpan = variant.zone_2_scroll_y[0].width;

        const int Steps = 2000;
        var hiddenAt = new SortedSet<int>();
        for (int zi = 0; zi <= Steps; ++zi)
        {
            float z = -zSpan * zi / Steps;
            for (int yi = 0; yi <= 200; ++yi)
            {
                if (AppMain.NightViewIsHiddenForTest(variant, AppMain.GSD_MAIN_STAGE_ID_2_1, ySpan * yi / 200f, z))
                {
                    hiddenAt.Add(zi);
                    break;
                }
            }
        }

        // one run per window, so a window pushed off the end of the domain shows up as a missing run
        int runs = 0;
        int previous = int.MinValue;
        foreach (int zi in hiddenAt)
        {
            if (zi != previous + 1)
                ++runs;
            previous = zi;
        }

        Assert.Equal(windows, runs);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    [InlineData(8)]
    [InlineData(12)]
    [InlineData(16)]
    public void EachZoneGateResolvesOnItsOwnGroup(int stage_id)
    {
        Load(DeclareConsole(stage_id));
        Assert.True(AppMain.gmMapIsConsoleDataComplete(stage_id));

        Load();
        Assert.False(AppMain.gmMapIsConsoleDataComplete(stage_id));
    }

    // ZONE3_M/_T/_ATTR and the background are shared by all four zone 3 acts, so dropping any one
    // of them takes the whole zone off the pc tier rather than just the act that named it
    [Fact]
    public void ZoneThreeTakesThePcTierAsAWholeZone()
    {
        var whole = Declare(AppMain.AMD_FS_VARIANT_PC, Zone3Stages);

        Load(whole);
        foreach (int stage_id in Zone3Stages)
            Assert.True(AppMain.gmMapIsPcLevelDataComplete(stage_id));

        Load(whole.Where(line => !line.StartsWith("G_ZONE3/MAP/ZONE3_M.AMB ")).ToArray());
        foreach (int stage_id in Zone3Stages)
            Assert.False(AppMain.gmMapIsPcLevelDataComplete(stage_id));
    }

    // the pc tier is console-grade art on PC-era levels, so it answers both gates; console answers
    // only the one. gmMapSelectVariant asks the narrow question first
    [Fact]
    public void ThePcTierIsConsoleGradeAndConsoleIsNotPc()
    {
        Load(Declare(AppMain.AMD_FS_VARIANT_PC, Zone3Stages));

        Assert.True(AppMain.gmMapIsPcLevelDataComplete(9));
        Assert.True(AppMain.gmMapIsConsoleDataComplete(9));

        Load(DeclareConsole(Zone3Stages));

        Assert.False(AppMain.gmMapIsPcLevelDataComplete(9));
        Assert.True(AppMain.gmMapIsConsoleDataComplete(9));
    }

    // gmMap.cpp:3249-3259 pushes four, and NNM_MTX(m, row, col) is m[col][row] (nns.h:788), so
    // uniform k is the four consecutive floats m[k][0..3]
    [Fact]
    public void AllFourProjectionRowsReachTheUserUniforms()
    {
        var projection = new Matrix(
            11, 12, 13, 14,
            21, 22, 23, 24,
            31, 32, 33, 34,
            41, 42, 43, 44);

        var rows = NnStdShaderEffect.ProjectionUserUniforms(projection);

        Assert.Equal(4, rows.Length);
        Assert.Equal(new Vector4(11, 12, 13, 14), rows[0]);
        Assert.Equal(new Vector4(21, 22, 23, 24), rows[1]);
        Assert.Equal(new Vector4(31, 32, 33, 34), rows[2]);
        Assert.Equal(new Vector4(41, 42, 43, 44), rows[3]);

        Assert.DoesNotContain(rows, row => row == Vector4.Zero);
    }

    // gmGameDat.h:64-67 adds GMD_GAMEDAT_MAP_MTN and _MMTN on console only, so the array has to hold
    // six and GmMapRelease has to clear all six
    [Fact]
    public void TheGamedatMapSlotsCoverBothArms()
    {
        Assert.Equal(4, AppMain.GMD_GAMEDAT_MAP_MTN);
        Assert.Equal(5, AppMain.GMD_GAMEDAT_MAP_MMTN);
        Assert.Equal(6, AppMain.GMD_GAMEDAT_MAP_MAX);
    }
}
