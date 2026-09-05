using System.IO;

namespace Sonic4Episode1.Tests;

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
        Assert.False(AppMain.gmDecoIsPcDataComplete(Stage1_1));
    }

    [Fact]
    public void ConsoleGradeDoesNotImplyThePcTier()
    {
        Load(Zone1Deco + " console");
        Assert.True(AppMain.gmDecoIsConsoleDataComplete(Stage1_1));
        Assert.False(AppMain.gmDecoIsPcDataComplete(Stage1_1));

        Load(Zone1Deco + " console", Zone1Deco + " pc");
        Assert.True(AppMain.gmDecoIsConsoleDataComplete(Stage1_1));
        Assert.True(AppMain.gmDecoIsPcDataComplete(Stage1_1));
    }

    [Fact]
    public void ZoneFourIsNotDraggedToThePcTierByZoneOne()
    {
        Load(Zone1Deco + " console", Zone1Deco + " pc");

        Assert.False(AppMain.gmDecoIsPcDataComplete(Stage4_1));
    }

    // zone 4 loads its own DECO_ZONE4, so declaring zone 1's does not reach it
    [Fact]
    public void ZoneFourIsUnaffectedByZoneOnesDeclaration()
    {
        Load(Zone1Deco + " console");

        Assert.False(AppMain.gmDecoIsConsoleDataComplete(Stage4_1));
    }

    // special stages load no decorations, so "complete" must be false, not vacuously true
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
