using System;
using System.Collections.Generic;
using System.Diagnostics;

internal partial class AppMain
{
    // PC's event id space agrees with our mobile-lineage one up to 294 and diverges from 295, so a
    // PC-era .EV has to be translated before the dispatch table sees it.
    //
    // The ported ids sit above PC's own table rather than at our old tail, so no id a PC map can
    // hold is ever a valid index into the ported block. That is what stops the next PC stage we
    // port landing its makers on the card stage's.
    internal const int GMD_EVENT_ID_PC_TABLE_MAX = 379;
    internal const int GMD_EVENT_ID_PORTED_START = 380;

    internal const int GMD_EVENT_ID_GMK_TRUMP_ROAD_CARD = 380;
    internal const int GMD_EVENT_ID_GMK_TP_TURN_BOARD = 381;
    internal const int GMD_EVENT_ID_GMK_TP_TURN_ROW = 382;
    internal const int GMD_EVENT_ID_GMK_TP_TURN_COLUMN = 383;
    internal const int GMD_EVENT_ID_GMK_TRUMP_STAGE_S = 384;
    internal const int GMD_EVENT_ID_GMK_TRUMP_STAGE_H = 385;
    internal const int GMD_EVENT_ID_GMK_TRUMP_STAGE_D = 386;
    internal const int GMD_EVENT_ID_GMK_TRUMP_STAGE_C = 387;
    internal const int GMD_EVENT_ID_GMK_TRUMP_STAGE_S5 = 388;
    internal const int GMD_EVENT_ID_GMK_TRUMP_STAGE_H7 = 389;
    internal const int GMD_EVENT_ID_GMK_TRUMP_STAGE_D10 = 390;
    internal const int GMD_EVENT_ID_GMK_TRUMP_STAGE_C9 = 391;
    internal const int GMD_EVENT_ID_NOSET_TP_TURN_PICT = 392;
    internal const int GMD_EVENT_ID_NOSET_TRUMP_ROAD_ROUTE = 393;

    internal const int GMD_EVENT_ID_GMK_TORCH = 394;
    internal const int GMD_EVENT_ID_GMK_CANDLE = 395;
    internal const int GMD_EVENT_ID_GMK_RAIL_END = 396;
    internal const int GMD_EVENT_ID_GMK_RAIL_END_AREA = 397;
    internal const int GMD_EVENT_ID_GMK_FUSE_LINE = 398;
    internal const int GMD_EVENT_ID_GMK_DYNAMITE = 399;
    internal const int GMD_EVENT_ID_GMK_BOMB_WALL = 400;
    internal const int GMD_EVENT_ID_GMK_LL2_REGION = 401;
    internal const int GMD_EVENT_ID_GMK_LL2_DARK_TRIGGER = 402;
    internal const int GMD_EVENT_ID_GMK_FUSE_LINE_FAR = 403;
    internal const int GMD_EVENT_ID_GMK_DYNAMITE_FAR = 404;
    internal const int GMD_EVENT_ID_GMK_BOMB_WALL_FAR = 405;

    internal const int GMD_EVENT_ID_MAX_PC = 406;

    // PC event id -> ours, zone 2 act 2's twelve then Lost Labyrinth act 2's. PC shares one maker
    // across 305/306/318/319 and another across 307/320; ours stay distinct so the remap is 1:1 and
    // the maker branches on our id. 300 and 301 are unplaced on PC's Z3A2 and get no row
    internal static readonly Dictionary<int, int> gm_event_pc_id_remap = new Dictionary<int, int>
    {
        { 295, GMD_EVENT_ID_GMK_TRUMP_ROAD_CARD },
        { 302, GMD_EVENT_ID_GMK_TP_TURN_BOARD },
        { 303, GMD_EVENT_ID_GMK_TP_TURN_ROW },
        { 304, GMD_EVENT_ID_GMK_TP_TURN_COLUMN },
        { 308, GMD_EVENT_ID_GMK_TRUMP_STAGE_S },
        { 309, GMD_EVENT_ID_GMK_TRUMP_STAGE_H },
        { 310, GMD_EVENT_ID_GMK_TRUMP_STAGE_D },
        { 311, GMD_EVENT_ID_GMK_TRUMP_STAGE_C },
        { 314, GMD_EVENT_ID_GMK_TRUMP_STAGE_S5 },
        { 315, GMD_EVENT_ID_GMK_TRUMP_STAGE_H7 },
        { 316, GMD_EVENT_ID_GMK_TRUMP_STAGE_D10 },
        { 317, GMD_EVENT_ID_GMK_TRUMP_STAGE_C9 },

        { 296, GMD_EVENT_ID_GMK_TORCH },
        { 297, GMD_EVENT_ID_GMK_CANDLE },
        { 298, GMD_EVENT_ID_GMK_RAIL_END },
        { 299, GMD_EVENT_ID_GMK_RAIL_END_AREA },
        { 305, GMD_EVENT_ID_GMK_FUSE_LINE },
        { 306, GMD_EVENT_ID_GMK_DYNAMITE },
        { 307, GMD_EVENT_ID_GMK_BOMB_WALL },
        { 312, GMD_EVENT_ID_GMK_LL2_REGION },
        { 313, GMD_EVENT_ID_GMK_LL2_DARK_TRIGGER },
        { 318, GMD_EVENT_ID_GMK_FUSE_LINE_FAR },
        { 319, GMD_EVENT_ID_GMK_DYNAMITE_FAR },
        { 320, GMD_EVENT_ID_GMK_BOMB_WALL_FAR },
    };

    internal static void gmEventExtendTablesForPCLevels()
    {
        Array.Resize(ref g_gm_event_tbl, GMD_EVENT_ID_MAX_PC);
        Array.Resize(ref g_gm_event_size_tbl, GMD_EVENT_ID_MAX_PC);

        // spawn/despawn camera margins, read out of PC's gm_event_size_tbl at 0x5B3C38. the makers
        // stay null until each family is written, which the dispatch already skips over
        g_gm_event_size_tbl[GMD_EVENT_ID_GMK_TRUMP_ROAD_CARD] = 64;

        g_gm_event_tbl[GMD_EVENT_ID_GMK_TRUMP_ROAD_CARD] = new Del__(GmGmkTrumpRoadInit);
        g_gm_event_tbl[GMD_EVENT_ID_NOSET_TRUMP_ROAD_ROUTE] = new Del__(GmGmkTrumpRoadRouteInit);

        for (int id = GMD_EVENT_ID_GMK_TP_TURN_BOARD; id <= GMD_EVENT_ID_GMK_TP_TURN_COLUMN; ++id)
            g_gm_event_size_tbl[id] = 128;

        g_gm_event_tbl[GMD_EVENT_ID_GMK_TP_TURN_BOARD] = new Del__(GmGmkTpTurnInitBoard);
        g_gm_event_tbl[GMD_EVENT_ID_GMK_TP_TURN_ROW] = new Del__(GmGmkTpTurnInitRow);
        g_gm_event_tbl[GMD_EVENT_ID_GMK_TP_TURN_COLUMN] = new Del__(GmGmkTpTurnInitColumn);
        g_gm_event_tbl[GMD_EVENT_ID_NOSET_TP_TURN_PICT] = new Del__(GmGmkTpTurnInitPict);

        // one maker for all eight, the way PC does it. Keeping ours contiguous means the suit is
        // just the offset, where PC has to skip the two Lost Labyrinth ids sitting in its gap
        for (int id = GMD_EVENT_ID_GMK_TRUMP_STAGE_S; id <= GMD_EVENT_ID_GMK_TRUMP_STAGE_C9; ++id)
        {
            g_gm_event_size_tbl[id] = 96;
            g_gm_event_tbl[id] = new Del__(GmGmkTrumpStageInit);
        }

        // never read: both are born through GmEventMgrLocalEventBirth, which calls the maker direct
        g_gm_event_size_tbl[GMD_EVENT_ID_NOSET_TP_TURN_PICT] = 64;
        g_gm_event_size_tbl[GMD_EVENT_ID_NOSET_TRUMP_ROAD_ROUTE] = 64;

        for (int id = GMD_EVENT_ID_GMK_TORCH; id <= GMD_EVENT_ID_GMK_RAIL_END_AREA; ++id)
            g_gm_event_size_tbl[id] = 64;

        for (int id = GMD_EVENT_ID_GMK_FUSE_LINE; id <= GMD_EVENT_ID_GMK_BOMB_WALL; ++id)
            g_gm_event_size_tbl[id] = 128;

        g_gm_event_size_tbl[GMD_EVENT_ID_GMK_LL2_REGION] = 384;
        g_gm_event_size_tbl[GMD_EVENT_ID_GMK_LL2_DARK_TRIGGER] = 384;
        g_gm_event_tbl[GMD_EVENT_ID_GMK_TORCH] = new Del__(GmGmkTorchInit);
        g_gm_event_tbl[GMD_EVENT_ID_GMK_CANDLE] = new Del__(GmGmkCandleInit);
        g_gm_event_tbl[GMD_EVENT_ID_GMK_LL2_REGION] = new Del__(GmGmkLl2RegionInit);
        g_gm_event_tbl[GMD_EVENT_ID_GMK_LL2_DARK_TRIGGER] = new Del__(GmGmkLl2VolumeInit);
        g_gm_event_tbl[GMD_EVENT_ID_GMK_RAIL_END] = new Del__(GmGmkRailEndInit);
        g_gm_event_tbl[GMD_EVENT_ID_GMK_RAIL_END_AREA] = new Del__(GmGmkRailEndAreaInit);
        g_gm_event_size_tbl[GMD_EVENT_ID_GMK_FUSE_LINE_FAR] = 447;
        g_gm_event_size_tbl[GMD_EVENT_ID_GMK_DYNAMITE_FAR] = 447;
        g_gm_event_size_tbl[GMD_EVENT_ID_GMK_BOMB_WALL_FAR] = 448;

        g_gm_event_tbl[GMD_EVENT_ID_GMK_FUSE_LINE] = new Del__(GmGmkFuseLineInit);
        g_gm_event_tbl[GMD_EVENT_ID_GMK_DYNAMITE] = new Del__(GmGmkFuseLineInit);
        g_gm_event_tbl[GMD_EVENT_ID_GMK_BOMB_WALL] = new Del__(GmGmkBombWallInit);
        g_gm_event_tbl[GMD_EVENT_ID_GMK_FUSE_LINE_FAR] = new Del__(GmGmkFuseLineInit);
        g_gm_event_tbl[GMD_EVENT_ID_GMK_DYNAMITE_FAR] = new Del__(GmGmkFuseLineInit);
        g_gm_event_tbl[GMD_EVENT_ID_GMK_BOMB_WALL_FAR] = new Del__(GmGmkBombWallInit);
    }

    internal static void gmEventRemapPcEventIds(GMS_EVE_DATA_EV_HEADER ev_data)
    {
        if (ev_data == null || ev_data.ev_list == null)
            return;

        for (int cell = 0; cell < ev_data.ev_list.Length; ++cell)
        {
            GMS_EVE_DATA_EV_LIST list = ev_data.ev_list[cell];
            if (list == null || list.eve_num == 0)
                continue;

            for (int i = 0; i < list.eve_num; ++i)
            {
                GMS_EVE_RECORD_EVENT rec = list.eve_rec[i];
                int ported;

                if (gm_event_pc_id_remap.TryGetValue(rec.id, out ported))
                    rec.id = (ushort)ported;
                else if (rec.id >= GMD_EVENT_ID_NOSET_START)
                    gmEventDropUnportedPcEvent(rec);
            }
        }
    }

    // everything from 300 up in our table is NOSET: born by code, never placed in a map. so a
    // placed id up there on a PC-era level is a PC gimmick with no port, and letting it through
    // would run whichever of our makers happens to sit at that index. mark the record spent, the
    // same thing the event manager does to one it has already consumed
    private static void gmEventDropUnportedPcEvent(GMS_EVE_RECORD_EVENT rec)
    {
        Debug.WriteLine($"gmEventRemapPcEventIds: PC event id {rec.id} has no port, dropping it");
        rec.pos_x = byte.MaxValue;
    }
}
