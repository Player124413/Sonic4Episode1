using System;
using System.Collections.Generic;
using System.Diagnostics;

internal partial class AppMain
{
    // PC's event id space agrees with our mobile-lineage one up to 294 and diverges from 295, so a
    // PC-era .EV has to be translated before the dispatch table sees it. What diverges and how it
    // was recovered is in docs/z2a2-card-gimmicks.md; the design is in docs/z2a2-port-plan.md.
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

    internal const int GMD_EVENT_ID_MAX_PC = 394;

    // PC event id -> ours. Only zone 2 act 2's twelve are here. The Lost Labyrinth act 2 family is
    // interleaved with them on PC (296-301, 305-307, 312-313, 318-320) and wants rows adding when
    // that stage is ported; until then those ids are dropped rather than dispatched
    private static readonly Dictionary<int, int> gm_event_pc_id_remap = new Dictionary<int, int>
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
    };

    internal static void gmEventExtendTablesForPcLevels()
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

        for (int id = GMD_EVENT_ID_GMK_TRUMP_STAGE_S; id <= GMD_EVENT_ID_GMK_TRUMP_STAGE_C9; ++id)
            g_gm_event_size_tbl[id] = 96;

        // never read: both are born through GmEventMgrLocalEventBirth, which calls the maker direct
        g_gm_event_size_tbl[GMD_EVENT_ID_NOSET_TP_TURN_PICT] = 64;
        g_gm_event_size_tbl[GMD_EVENT_ID_NOSET_TRUMP_ROAD_ROUTE] = 64;
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
