using System.IO;

namespace Sonic4Episode1.Tests;

// Decorations ship as their own amFsAllVariant group, separate from the map's. Console draws an NN
// model per decoration; mobile draws baked TVX sprites. The property that matters is that the two
// gates move independently, so a missing sibling on one side cannot drag the other back.
// See docs/console-deco.md.
//
// amFsLoadVariants replaces a process-wide static, so this runs in the shared collection.
[Collection(FsVariantCollection.Name)]
public class DecoVariantTests
{
    private const int Stage1_1 = 0;
    private const int Stage1_Boss = 3;
    private const int Stage4_1 = 12;

    private const string Zone1Deco = "G_ZONE1/DECO/DECO_ZONE1.AMB";

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

    [Fact]
    public void TheDecoGroupIsTheDecoArchiveAlone()
    {
        Load(Zone1Deco + " console");

        Assert.True(AppMain.gmDecoIsConsoleDataComplete(Stage1_1));
        Assert.True(AppMain.gmDecoIsConsoleDataComplete(Stage1_Boss));
    }

    // the whole reason for a second gate: declaring one does not move the other
    [Fact]
    public void DecoAndMapResolveIndependently()
    {
        Load(MapGroup(Stage1_1));
        Assert.True(AppMain.gmMapIsConsoleDataComplete(Stage1_1));
        Assert.False(AppMain.gmDecoIsConsoleDataComplete(Stage1_1));

        Load(Zone1Deco + " console");
        Assert.False(AppMain.gmMapIsConsoleDataComplete(Stage1_1));
        Assert.True(AppMain.gmDecoIsConsoleDataComplete(Stage1_1));
    }

    [Fact]
    public void NoManifestMeansMobile()
    {
        Load();

        Assert.False(AppMain.gmDecoIsConsoleDataComplete(Stage1_1));
    }

    // zone 4 loads its own DECO_ZONE4, so declaring zone 1's does not reach it. This is the
    // decoration twin of the ZONE1_MAPFAR trap that made the map arm pin its loads.
    [Fact]
    public void ZoneFourIsUnaffectedByZoneOnesDeclaration()
    {
        Load(Zone1Deco + " console");

        Assert.False(AppMain.gmDecoIsConsoleDataComplete(Stage4_1));
    }

    // the special stages load no decorations at all, so "complete" has to be false rather than
    // vacuously true
    [Fact]
    public void AStageWithNoDecorationsIsNotConsoleComplete()
    {
        Load(Zone1Deco + " console");

        int withoutDeco = -1;
        for (int stage = 0; stage < AppMain.gm_gamedat_tbl_map_info_tbl.Length; stage++)
        {
            var info = AppMain.gm_gamedat_tbl_map_info_tbl[stage];
            bool any = false;

            for (int index = 0; index < info.num; ++index)
            {
                if (info.data_tbl[index].proc_post == AppMain.gmGameDatLoadProcPostDeco)
                    any = true;
            }

            if (!any)
            {
                withoutDeco = stage;
                break;
            }
        }

        Assert.True(withoutDeco >= 0, "no stage loads without decorations");
        Assert.False(AppMain.gmDecoIsConsoleDataComplete(withoutDeco));
    }
}
