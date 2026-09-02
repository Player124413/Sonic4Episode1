using System.Linq;

namespace Sonic4Episode1.Tests;

// PC's event ids mean different things to ours from 295 up, so a PC-era .EV is translated as it is
// loaded. These pin the parts that fail silently rather than loudly: a missing remap row spawns
// whatever our table holds at that index (PC's 308 is our Boss 1 chain), and a ported id that
// drifted back down into PC's own range would be reachable straight from map data. See
// docs/z2a2-port-plan.md "C scoped".
public class EventPcIdTests
{
    private static AppMain.GMS_EVE_DATA_EV_HEADER PlacedAt(params int[] ids)
    {
        var records = ids
            .Select(id => new AppMain.GMS_EVE_RECORD_EVENT { pos_x = 1, pos_y = 2, id = (ushort)id })
            .ToArray();

        return new AppMain.GMS_EVE_DATA_EV_HEADER
        {
            width = 1,
            height = 1,
            ev_list = new[]
            {
                new AppMain.GMS_EVE_DATA_EV_LIST
                {
                    eve_num = (ushort)records.Length,
                    eve_rec = records,
                },
            },
        };
    }

    private static AppMain.GMS_EVE_RECORD_EVENT Remapped(int id)
    {
        var data = PlacedAt(id);
        AppMain.gmEventRemapPcEventIds(data);
        return data.ev_list[0].eve_rec[0];
    }

    [Fact]
    public void BothIdIndexedTablesAreWidenedTogether()
    {
        Assert.Equal(AppMain.GMD_EVENT_ID_MAX_PC, AppMain.g_gm_event_tbl.Length);
        Assert.Equal(AppMain.GMD_EVENT_ID_MAX_PC, AppMain.g_gm_event_size_tbl.Length);
    }

    // a resize that shifted anything would break every stage, not just this one
    [Fact]
    public void TheMobileEntriesSurviveTheResize()
    {
        Assert.Equal(128, AppMain.g_gm_event_size_tbl[0]);
        Assert.Equal(255, AppMain.g_gm_event_size_tbl[1]);
        Assert.Equal(256, AppMain.g_gm_event_size_tbl[306]);
        Assert.NotNull(AppMain.g_gm_event_tbl[313]);
        Assert.Null(AppMain.g_gm_event_tbl[295]);
    }

    // trumproad is written; tp_turn and trumpstage are not. A non-null slot for a family that has
    // no module yet would dispatch into nothing
    [Fact]
    public void OnlyTheImplementedFamiliesHaveMakers()
    {
        Assert.NotNull(AppMain.g_gm_event_tbl[AppMain.GMD_EVENT_ID_GMK_TRUMP_ROAD_CARD]);
        Assert.NotNull(AppMain.g_gm_event_tbl[AppMain.GMD_EVENT_ID_NOSET_TRUMP_ROAD_ROUTE]);

        for (int id = AppMain.GMD_EVENT_ID_GMK_TP_TURN_BOARD;
             id <= AppMain.GMD_EVENT_ID_NOSET_TP_TURN_PICT;
             ++id)
            Assert.Null(AppMain.g_gm_event_tbl[id]);
    }

    // read out of PC's gm_event_size_tbl at 0x5B3C38
    [Theory]
    [InlineData(AppMain.GMD_EVENT_ID_GMK_TRUMP_ROAD_CARD, 64)]
    [InlineData(AppMain.GMD_EVENT_ID_GMK_TP_TURN_BOARD, 128)]
    [InlineData(AppMain.GMD_EVENT_ID_GMK_TP_TURN_ROW, 128)]
    [InlineData(AppMain.GMD_EVENT_ID_GMK_TP_TURN_COLUMN, 128)]
    [InlineData(AppMain.GMD_EVENT_ID_GMK_TRUMP_STAGE_S, 96)]
    [InlineData(AppMain.GMD_EVENT_ID_GMK_TRUMP_STAGE_C, 96)]
    [InlineData(AppMain.GMD_EVENT_ID_GMK_TRUMP_STAGE_S5, 96)]
    [InlineData(AppMain.GMD_EVENT_ID_GMK_TRUMP_STAGE_C9, 96)]
    public void ThePortedMarginsMatchThePcTable(int id, int margin)
    {
        Assert.Equal(margin, AppMain.g_gm_event_size_tbl[id]);
    }

    [Theory]
    [InlineData(295, AppMain.GMD_EVENT_ID_GMK_TRUMP_ROAD_CARD)]
    [InlineData(302, AppMain.GMD_EVENT_ID_GMK_TP_TURN_BOARD)]
    [InlineData(303, AppMain.GMD_EVENT_ID_GMK_TP_TURN_ROW)]
    [InlineData(304, AppMain.GMD_EVENT_ID_GMK_TP_TURN_COLUMN)]
    [InlineData(308, AppMain.GMD_EVENT_ID_GMK_TRUMP_STAGE_S)]
    [InlineData(309, AppMain.GMD_EVENT_ID_GMK_TRUMP_STAGE_H)]
    [InlineData(310, AppMain.GMD_EVENT_ID_GMK_TRUMP_STAGE_D)]
    [InlineData(311, AppMain.GMD_EVENT_ID_GMK_TRUMP_STAGE_C)]
    [InlineData(314, AppMain.GMD_EVENT_ID_GMK_TRUMP_STAGE_S5)]
    [InlineData(315, AppMain.GMD_EVENT_ID_GMK_TRUMP_STAGE_H7)]
    [InlineData(316, AppMain.GMD_EVENT_ID_GMK_TRUMP_STAGE_D10)]
    [InlineData(317, AppMain.GMD_EVENT_ID_GMK_TRUMP_STAGE_C9)]
    public void TheCardIdsAreTranslated(int pc_id, int ported_id)
    {
        var record = Remapped(pc_id);

        Assert.Equal(ported_id, record.id);
        Assert.NotEqual(byte.MaxValue, record.pos_x);
    }

    // 0-294 mean the same thing in both trees, so the whole shared span has to come through
    // untouched. 295-299 are null in our table and skipped by the dispatch, so they are left alone
    // rather than dropped
    [Fact]
    public void TheSharedIdSpaceIsNotTouched()
    {
        for (int id = 0; id < AppMain.GMD_EVENT_ID_NOSET_START; ++id)
        {
            if (id == 295)
                continue;

            var record = Remapped(id);
            Assert.Equal(id, record.id);
            Assert.NotEqual(byte.MaxValue, record.pos_x);
        }
    }

    // the Lost Labyrinth act 2 family PC interleaves with the cards, plus the unplaced match grid.
    // none is ported, and all of them index a live maker of ours
    [Theory]
    [InlineData(300)]
    [InlineData(305)]
    [InlineData(307)]
    [InlineData(312)]
    [InlineData(313)]
    [InlineData(318)]
    [InlineData(320)]
    public void AnUnportedPcIdIsDroppedRatherThanDispatched(int pc_id)
    {
        Assert.NotNull(AppMain.g_gm_event_tbl[pc_id]);
        Assert.Equal(byte.MaxValue, Remapped(pc_id).pos_x);
    }

    // the invariant that keeps the next PC stage we port off this one's makers: nothing a PC map
    // can hold is a valid index into the ported block
    [Fact]
    public void NoPcIdCanReachThePortedBlock()
    {
        Assert.True(AppMain.GMD_EVENT_ID_PORTED_START > AppMain.GMD_EVENT_ID_PC_TABLE_MAX);

        for (int pc_id = 0; pc_id <= AppMain.GMD_EVENT_ID_PC_TABLE_MAX; ++pc_id)
        {
            var record = Remapped(pc_id);
            if (record.id >= AppMain.GMD_EVENT_ID_PORTED_START)
                Assert.NotEqual(pc_id, record.id);
        }
    }

    // running it twice is not a hypothetical: GmEventDataBuild runs again on a restart
    [Fact]
    public void TranslatingAlreadyTranslatedDataChangesNothing()
    {
        var data = PlacedAt(295, 302, 308, 317, 130);
        AppMain.gmEventRemapPcEventIds(data);
        var once = data.ev_list[0].eve_rec.Select(r => r.id).ToArray();

        AppMain.gmEventRemapPcEventIds(data);

        Assert.Equal(once, data.ev_list[0].eve_rec.Select(r => r.id).ToArray());
    }
}
