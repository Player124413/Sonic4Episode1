using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sonic4Episode1.Tests;

// Zone 1's gimmicks ship as their own amFsAllVariant group. The archives are member for member
// identical between the builds, which is why they need no format fork, but the gimmick *modules*
// fork on _IPHONE for lighting and for how a thing is drawn, so there is a runtime gate after all.
// See docs/console-gimmicks.md.
//
// amFsLoadVariants replaces a process-wide static, so this runs in the shared collection.
[Collection(FsVariantCollection.Name)]
public class GimmickVariantTests
{
    private const int Stage1_1 = 0;
    private const int Stage1_2 = 1;
    private const int Stage1_3 = 2;
    private const int Stage1_Boss = 3;

    private static void Load(params string[] lines)
    {
        AppMain.amFsLoadVariants(new StringReader(string.Join("\n", lines)));
    }

    // every G_ZONE1/GMK archive the stage's own load table names, which is the only place those
    // names live
    private static string[] GroupFor(int stage_id)
    {
        var info = AppMain.gm_gamedat_tbl_gimmick_info_tbl[stage_id];
        var paths = new List<string>();

        for (int index = 0; index < info.num; ++index)
        {
            var path = info.data_tbl[index].path;
            if (path.StartsWith("G_ZONE1/GMK/"))
                paths.Add(path);
        }

        return paths.ToArray();
    }

    private static string[] ZoneOneGimmicks()
    {
        return new[] { Stage1_1, Stage1_2, Stage1_3, Stage1_Boss }
            .SelectMany(GroupFor)
            .Distinct()
            .ToArray();
    }

    // the manifest content-sources.toml produces. seventeen archives, and END_SONIC is not one of
    // them: it is a post-fork mobile addition the XBLA beta does not have at all
    private static readonly string[] Declared =
    {
        "G_ZONE1/GMK/GMK_LAND_1_MDL.AMB",
        "G_ZONE1/GMK/GMK_LAND_1_TEX.AMB",
        "G_ZONE1/GMK/GMK_T_ROPE_MDL.AMB",
        "G_ZONE1/GMK/GMK_T_ROPE_TEX.AMB",
        "G_ZONE1/GMK/GMK_T_ROPE_MTN.AMB",
        "G_ZONE1/GMK/GMK_B_LAND1_MDL.AMB",
        "G_ZONE1/GMK/GMK_B_LAND1_TEX.AMB",
        "G_ZONE1/GMK/GMK_B_LAND1_MTN.AMB",
        "G_ZONE1/GMK/GMK_B_OBJ_1_MDL.AMB",
        "G_ZONE1/GMK/GMK_B_OBJ_1_TEX.AMB",
        "G_ZONE1/GMK/GMK_B_WALL1_MDL.AMB",
        "G_ZONE1/GMK/GMK_B_WALL1_TEX.AMB",
        "G_ZONE1/GMK/GMK_BRIDGE_MDL.AMB",
        "G_ZONE1/GMK/GMK_BRIDGE_TEX.AMB",
        "G_ZONE1/GMK/GMK_PULLEY_MDL.AMB",
        "G_ZONE1/GMK/GMK_PULLEY_TEX.AMB",
        "G_ZONE1/GMK/GMK_PULLEY_MTN.AMB",
    };

    [Fact]
    public void TheDeclaredGroupIsEveryGimmickZoneOneActuallyLoads()
    {
        Assert.Equal(
            ZoneOneGimmicks().OrderBy(path => path, System.StringComparer.Ordinal),
            Declared.OrderBy(path => path, System.StringComparer.Ordinal));
    }

    [Fact]
    public void EveryZoneOneGimmickArchiveResolvesConsole()
    {
        Load(Declared.Select(path => path + " console").ToArray());

        foreach (var path in ZoneOneGimmicks())
            Assert.Equal(AppMain.AMD_FS_VARIANT_CONSOLE, AppMain.amFsResolvedVariant(path));
    }

    // the failure a half-built group produces: one archive left off the manifest keeps the base
    // asset while its siblings go console, which is a stage with one mobile gimmick in it
    [Fact]
    public void AnUndeclaredArchiveStaysOnTheBase()
    {
        Load(Declared.Skip(1).Select(path => path + " console").ToArray());

        Assert.Null(AppMain.amFsResolvedVariant(Declared[0]));
        Assert.Equal(AppMain.AMD_FS_VARIANT_CONSOLE, AppMain.amFsResolvedVariant(Declared[1]));
    }

    // gimmicks resolve on their own, so declaring them does not drag the map group across
    [Fact]
    public void GimmicksAndTheMapResolveIndependently()
    {
        Load(Declared.Select(path => path + " console").ToArray());

        Assert.Equal(AppMain.AMD_FS_VARIANT_CONSOLE,
            AppMain.amFsResolvedVariant("G_ZONE1/GMK/GMK_BRIDGE_MDL.AMB"));
        Assert.False(AppMain.gmMapIsConsoleDataComplete(Stage1_1));
    }

    // END_SONIC is in the mobile load table for the ending stage and nowhere in the beta, so it
    // must never join the group
    [Fact]
    public void TheEndingSonicModelIsNotDeclared()
    {
        Assert.DoesNotContain(Declared, path => path.Contains("END_SONIC"));
    }

    // the gimmick modules fork on _IPHONE even though the archives do not, so there is a runtime
    // gate after all. See docs/console-gimmicks.md.
    [Fact]
    public void TheGateFollowsTheDeclaredGroup()
    {
        Load(Declared.Select(path => path + " console").ToArray());

        Assert.True(AppMain.gmGmkIsConsoleDataComplete(Stage1_1));
        Assert.True(AppMain.gmGmkIsConsoleDataComplete(Stage1_3));
    }

    [Fact]
    public void AHalfDeclaredGroupLeavesTheGateOff()
    {
        Load(Declared.Skip(1).Select(path => path + " console").ToArray());

        Assert.False(AppMain.gmGmkIsConsoleDataComplete(Stage1_1));
    }

    [Fact]
    public void NoManifestLeavesTheGateOff()
    {
        Load();

        Assert.False(AppMain.gmGmkIsConsoleDataComplete(Stage1_1));
    }

    // the G_COM gimmicks load for every stage in the game, so they are deliberately not in any
    // console group and must not be able to hold the gate off
    [Fact]
    public void TheCommonGimmicksAreNotPartOfTheGate()
    {
        Load(Declared.Select(path => path + " console").ToArray());

        var common = AppMain.gm_gamedat_tbl_gimmick_common_info_tbl[0];
        var anyCommon = false;
        for (int index = 0; index < common.num; ++index)
        {
            if (common.data_tbl[index].path.StartsWith("G_COM/"))
                anyCommon = true;
        }

        Assert.True(anyCommon, "the common table should hold G_COM gimmicks");
        Assert.True(AppMain.gmGmkIsConsoleDataComplete(Stage1_1));
    }
}
