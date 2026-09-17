using System.Linq;

namespace Sonic4Episode1.Tests;

// PC's event ids mean different things to ours from 295 up, so a PC-era .EV is translated as it is loaded
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

    // a remapped id landing on a null maker is silently skipped by the dispatch, so the card would simply never appear
    [Fact]
    public void EveryPortedIdHasAMaker()
    {
        for (int id = AppMain.GMD_EVENT_ID_PORTED_START; id <= AppMain.GMD_EVENT_ID_NOSET_TRUMP_ROAD_ROUTE; ++id)
            Assert.NotNull(AppMain.g_gm_event_tbl[id]);
    }

    [Fact]
    public void EveryLostLabyrinthIdHasAMaker()
    {
        for (int id = AppMain.GMD_EVENT_ID_GMK_TORCH; id < AppMain.GMD_EVENT_ID_MAX_PC; ++id)
            Assert.NotNull(AppMain.g_gm_event_tbl[id]);
    }

    // PC shares one maker across the fuse line and the dynamite and another across the wall, and
    // the far-margin twins go to the same pair. Ours keep six distinct ids so the remap stays 1:1,
    // which only works if they land on the right two makers
    [Fact]
    public void TheFuseFamilySharesTwoMakers()
    {
        var fuse = AppMain.g_gm_event_tbl[AppMain.GMD_EVENT_ID_GMK_FUSE_LINE];
        var wall = AppMain.g_gm_event_tbl[AppMain.GMD_EVENT_ID_GMK_BOMB_WALL];

        Assert.NotEqual(fuse, wall);
        Assert.Equal(fuse, AppMain.g_gm_event_tbl[AppMain.GMD_EVENT_ID_GMK_DYNAMITE]);
        Assert.Equal(fuse, AppMain.g_gm_event_tbl[AppMain.GMD_EVENT_ID_GMK_FUSE_LINE_FAR]);
        Assert.Equal(fuse, AppMain.g_gm_event_tbl[AppMain.GMD_EVENT_ID_GMK_DYNAMITE_FAR]);
        Assert.Equal(wall, AppMain.g_gm_event_tbl[AppMain.GMD_EVENT_ID_GMK_BOMB_WALL_FAR]);
    }

    // PC dispatches all eight suits through gmGmkTrumpStageInit and picks the model off the id, so
    // eight distinct makers here would mean the suit was being chosen twice
    [Fact]
    public void TheEightSuitsShareOneMaker()
    {
        var maker = AppMain.g_gm_event_tbl[AppMain.GMD_EVENT_ID_GMK_TRUMP_STAGE_S];

        for (int id = AppMain.GMD_EVENT_ID_GMK_TRUMP_STAGE_S;
             id <= AppMain.GMD_EVENT_ID_GMK_TRUMP_STAGE_C9;
             ++id)
            Assert.Equal(maker.Method, AppMain.g_gm_event_tbl[id].Method);
    }

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

    [Theory]
    [InlineData(296, AppMain.GMD_EVENT_ID_GMK_TORCH, 64)]
    [InlineData(297, AppMain.GMD_EVENT_ID_GMK_CANDLE, 64)]
    [InlineData(298, AppMain.GMD_EVENT_ID_GMK_RAIL_END, 64)]
    [InlineData(299, AppMain.GMD_EVENT_ID_GMK_RAIL_END_AREA, 64)]
    [InlineData(305, AppMain.GMD_EVENT_ID_GMK_FUSE_LINE, 128)]
    [InlineData(306, AppMain.GMD_EVENT_ID_GMK_DYNAMITE, 128)]
    [InlineData(307, AppMain.GMD_EVENT_ID_GMK_BOMB_WALL, 128)]
    [InlineData(312, AppMain.GMD_EVENT_ID_GMK_LL2_REGION, 384)]
    [InlineData(313, AppMain.GMD_EVENT_ID_GMK_LL2_DARK_TRIGGER, 384)]
    [InlineData(318, AppMain.GMD_EVENT_ID_GMK_FUSE_LINE_FAR, 447)]
    [InlineData(319, AppMain.GMD_EVENT_ID_GMK_DYNAMITE_FAR, 447)]
    [InlineData(320, AppMain.GMD_EVENT_ID_GMK_BOMB_WALL_FAR, 448)]
    public void TheLostLabyrinthIdsAreTranslated(int pc_id, int ported_id, int margin)
    {
        var record = Remapped(pc_id);

        Assert.Equal(ported_id, record.id);
        Assert.NotEqual(byte.MaxValue, record.pos_x);
        Assert.Equal(margin, AppMain.g_gm_event_size_tbl[ported_id]);
    }

    // 0-294 mean the same thing in both trees, so the whole shared span has to come through
    // untouched
    [Fact]
    public void TheSharedIdSpaceIsNotTouched()
    {
        for (int id = 0; id < 295; ++id)
        {
            var record = Remapped(id);
            Assert.Equal(id, record.id);
            Assert.NotEqual(byte.MaxValue, record.pos_x);
        }
    }

    // 300 and 301 share PC's dark trigger maker but are unplaced on its Z3A2, so they have no row.
    // both index a live maker of ours, which is why they have to be dropped rather than let through
    [Theory]
    [InlineData(300)]
    [InlineData(301)]
    public void AnUnportedPcIdIsDroppedRatherThanDispatched(int pc_id)
    {
        Assert.NotNull(AppMain.g_gm_event_tbl[pc_id]);
        Assert.Equal(byte.MaxValue, Remapped(pc_id).pos_x);
    }

    // the twelve have to stay 1:1 with distinct ids of ours, since PC shares makers across
    // 305/306/318/319 and 307/320 and we branch on our id instead
    [Fact]
    public void TheRemapIsOneToOne()
    {
        var ported = AppMain.gm_event_pc_id_remap.Values.ToArray();

        Assert.Equal(24, ported.Length);
        Assert.Equal(ported.Length, ported.Distinct().Count());
        Assert.All(ported, id => Assert.InRange(id, AppMain.GMD_EVENT_ID_PORTED_START, AppMain.GMD_EVENT_ID_MAX_PC - 1));
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
