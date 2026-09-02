using System.Linq;

namespace Sonic4Episode1.Tests;

// The two map motion sidecars are positional: gmGameDatLoadProcPostMap files each row into
// g_gm_gamedat_map by the row's index in the table, so ZONE2_M_M has to land on
// GMD_GAMEDAT_MAP_MTN and ZONE2_M_V on GMD_GAMEDAT_MAP_MMTN. Swap them and the node motions are
// handed to the material loader; move them and the block models or the attributes are. None of
// that throws. Row order is transcribed from the leak's gmGameDatTbl.inc, where the pair sits
// inside `#if !defined GMD_GAMEDAT_NO_CREATE_MAP_MOTION` between ATTR and MAPFAR.
public class MapMotionTests
{
    private const string MTN = "G_ZONE2/MAP/ZONE2_M_M.AMB";
    private const string MMTN = "G_ZONE2/MAP/ZONE2_M_V.AMB";

    // zone 2's four acts, the only stages with motion sidecars
    public static TheoryData<int> Zone2Stages => new() { 4, 5, 6, 7 };

    private static AppMain.GMS_GAMEDAT_LOAD_DATA[] ConsoleTable(int stage_id)
    {
        switch (stage_id)
        {
            case 4: return AppMain.gm_gamedat_tbl_map02_01_console;
            case 5: return AppMain.gm_gamedat_tbl_map02_02_console;
            case 6: return AppMain.gm_gamedat_tbl_map02_03_console;
            default: return AppMain.gm_gamedat_tbl_map02_BOSS_console;
        }
    }

    [Theory]
    [MemberData(nameof(Zone2Stages))]
    public void TheSidecarsLandOnTheirOwnSlots(int stage_id)
    {
        var table = ConsoleTable(stage_id);

        Assert.Equal(MTN, table[AppMain.GMD_GAMEDAT_MAP_MTN].path);
        Assert.Equal(MMTN, table[AppMain.GMD_GAMEDAT_MAP_MMTN].path);
    }

    // both go through the map post proc, which is what files them into g_gm_gamedat_map at all
    [Theory]
    [MemberData(nameof(Zone2Stages))]
    public void TheSidecarsLoadThroughTheMapProc(int stage_id)
    {
        var table = ConsoleTable(stage_id);

        foreach (int slot in new[] { AppMain.GMD_GAMEDAT_MAP_MTN, AppMain.GMD_GAMEDAT_MAP_MMTN })
        {
            Assert.Equal(AppMain.gmGameDatLoadProcPostMap, table[slot].proc_post);
            Assert.Null(table[slot].proc_pre);
            Assert.Equal(-1, table[slot].user_data);
        }
    }

    // the first four rows are what GmMapBuildData reads by slot, and everything after the splice
    // has to keep its order too even though MAPFAR and DECO no longer sit where they did
    [Theory]
    [MemberData(nameof(Zone2Stages))]
    public void SplicingChangesNothingButThePosition(int stage_id)
    {
        var plain = AppMain.gm_gamedat_tbl_map_info_tbl[stage_id];
        var table = ConsoleTable(stage_id);

        Assert.Equal(plain.num + 2, table.Length);

        var spliced = table.Take(AppMain.GMD_GAMEDAT_MAP_MTN)
            .Concat(table.Skip(AppMain.GMD_GAMEDAT_MAP_MMTN + 1))
            .Select(row => row.path)
            .ToArray();

        var original = Enumerable.Range(0, plain.num)
            .Select(i => plain.data_tbl[i].path)
            .ToArray();

        Assert.Equal(original, spliced);
    }

    // The tier is decided by requiring every map row to resolve to it, so a sidecar row in the
    // plain table would make that decision depend on archives that only exist above base. This is
    // why the splice lives in gmGameDatMapInfoFor and not in the table literal
    [Fact]
    public void ThePlainTablesStayFreeOfSidecars()
    {
        for (int stage_id = 0; stage_id < AppMain.gm_gamedat_tbl_map_info_tbl.Length; ++stage_id)
        {
            var info = AppMain.gm_gamedat_tbl_map_info_tbl[stage_id];

            for (int i = 0; i < info.num; ++i)
                Assert.DoesNotContain("_M_M.AMB", info.data_tbl[i].path);
        }
    }

    // zone 1, 3 and 4 ship sidecars on PC too, but ours are not in the content group, so a row for
    // them would be a load of a file that is not there
    [Fact]
    public void OnlyZoneTwoGetsSidecars()
    {
        foreach (int stage_id in new[] { 0, 1, 2, 3, 8, 9, 10, 11 })
            Assert.Same(AppMain.gm_gamedat_tbl_map_info_tbl[stage_id].data_tbl,
                AppMain.gmGameDatMapInfoFor(stage_id).data_tbl);
    }
}
