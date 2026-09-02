internal partial class AppMain
{
    // The stage-wide half of the flip cards: the deck the faces are rolled from, and the manager
    // that holds recently revealed panels until they either make three of a kind or time out.
    // PC gmGmkTpTurnCreateManager 0x4F7B90, gmGmkTpTurnEntryMatch 0x4F8180. See
    // docs/z2a2-tp-turn-re.md sections 3 and 4.
    //
    // Three of a kind revealed anywhere in the stage inside five seconds is a match: rings pay ten
    // rings, Sonic pays a life. Eggman is a dud and is never even offered.

    private const int GMD_GMK_TP_TURN_DECK_MAX = 128;

    // two pending slots per kind, and kind 0's pair exists only so the indexing stays uniform
    private const int GMD_GMK_TP_TURN_PENDING_PER_KIND = 2;
    private const int GMD_GMK_TP_TURN_PENDING_MAX = 6;

    // five seconds at 60fps
    private const int GMD_GMK_TP_TURN_PENDING_TTL = 300;

    internal const int GMD_GMK_TP_TURN_MATCH_RINGS = 10;

    // byte per record, four 2-bit fields. PC packs both of these into one 4-byte record and
    // leaves the other two bytes unused
    private static byte[] gm_gmk_tp_turn_deck_kinds;
    private static byte[] gm_gmk_tp_turn_deck_revealed;
    private static int gm_gmk_tp_turn_deck_next_id;

    private class GMS_GMK_TP_TURN_PENDING
    {
        public bool used;
        public int kind;
        public VecFx32 pos;
        public int ttl;
        public int card_idx;
    }

    private static GMS_GMK_TP_TURN_PENDING[] gm_gmk_tp_turn_pending;
    private static MTS_TASK_TCB gm_gmk_tp_turn_mgr_tcb;

    private static void gmGmkTpTurnCreateManager()
    {
        gm_gmk_tp_turn_card_timer = new int[GMD_GMK_TP_TURN_CARD_IDX_MAX];
        gm_gmk_tp_turn_deck_kinds = new byte[GMD_GMK_TP_TURN_DECK_MAX];
        gm_gmk_tp_turn_deck_revealed = new byte[GMD_GMK_TP_TURN_DECK_MAX];
        gm_gmk_tp_turn_pending = new GMS_GMK_TP_TURN_PENDING[GMD_GMK_TP_TURN_PENDING_MAX];

        for (int i = 0; i < GMD_GMK_TP_TURN_PENDING_MAX; ++i)
            gm_gmk_tp_turn_pending[i] = new GMS_GMK_TP_TURN_PENDING();

        gmGmkTpTurnResetState();

        if (gm_gmk_tp_turn_mgr_tcb != null)
            return;

        gm_gmk_tp_turn_mgr_tcb = MTM_TASK_MAKE_TCB(
            new GSF_TASK_PROCEDURE(gmGmkTpTurnMgrMain), null, 0U, 0, 18432U, 5, null,
            "GM TRUMP TURN MGR");
    }

    private static void gmGmkTpTurnDestroyManager()
    {
        if (gm_gmk_tp_turn_mgr_tcb != null)
        {
            mtTaskClearTcb(gm_gmk_tp_turn_mgr_tcb);
            gm_gmk_tp_turn_mgr_tcb = null;
        }

        gm_gmk_tp_turn_card_timer = null;
        gm_gmk_tp_turn_deck_kinds = null;
        gm_gmk_tp_turn_deck_revealed = null;
        gm_gmk_tp_turn_pending = null;
    }

    // PC gmGmkTpTurnResetState 0x4F7AD0, the death and restart path. Clearing the deck is what
    // makes every panel in the stage face down again
    private static void gmGmkTpTurnResetState()
    {
        if (gm_gmk_tp_turn_pending != null)
        {
            for (int i = 0; i < GMD_GMK_TP_TURN_PENDING_MAX; ++i)
                gmGmkTpTurnClearPending(gm_gmk_tp_turn_pending[i]);
        }

        if (gm_gmk_tp_turn_card_timer != null)
            System.Array.Clear(gm_gmk_tp_turn_card_timer, 0, gm_gmk_tp_turn_card_timer.Length);

        if (gm_gmk_tp_turn_deck_kinds != null)
        {
            System.Array.Clear(gm_gmk_tp_turn_deck_kinds, 0, gm_gmk_tp_turn_deck_kinds.Length);
            System.Array.Clear(gm_gmk_tp_turn_deck_revealed, 0, gm_gmk_tp_turn_deck_revealed.Length);
        }

        gm_gmk_tp_turn_card_next_idx = GMD_GMK_TP_TURN_CARD_IDX_AUTO_START;
        gm_gmk_tp_turn_deck_next_id = 0;
    }

    private static void gmGmkTpTurnClearPending(GMS_GMK_TP_TURN_PENDING pending)
    {
        pending.used = false;
        pending.kind = 0;
        pending.pos.x = 0;
        pending.pos.y = 0;
        pending.pos.z = 0;
        pending.card_idx = 0;
        pending.ttl = -1;
    }

    // PC gmGmkTpTurnMgrUpdatePending 0x4F75F0. Expiring stale reveals is the whole of the
    // manager's per-frame job
    private static void gmGmkTpTurnMgrMain(MTS_TASK_TCB tcb)
    {
        if (gm_gmk_tp_turn_pending == null)
            return;

        for (int i = 0; i < GMD_GMK_TP_TURN_PENDING_MAX; ++i)
        {
            GMS_GMK_TP_TURN_PENDING pending = gm_gmk_tp_turn_pending[i];
            if (!pending.used)
                continue;

            if (--pending.ttl <= 0)
                gmGmkTpTurnClearPending(pending);
        }
    }

    // The deck id lives in the high byte of the placed record's word_param, which the .EV ships as
    // zero for all forty one cards. Writing it back is what keeps a card's faces, and which of
    // them have already been turned, across a despawn and respawn within one life
    private static int gmGmkTpTurnDeckIdFor(GMS_GMK_TP_TURN_WORK work, bool board)
    {
        GMS_EVE_RECORD_EVENT rec = ((GMS_ENEMY_3D_WORK)work).ene_com.eve_rec;

        int deck_id = (rec.word_param >> 8) & 0xFF;
        if (deck_id != 0)
            return deck_id;

        deck_id = board ? gmGmkTpTurnAllocDeckRecords4() : gmGmkTpTurnAllocDeckRecord();
        rec.word_param = (ushort)(rec.word_param & 0x00FF | deck_id << 8);
        return deck_id;
    }

    // PC gmGmkTpTurnAllocDeckRecord 0x4F7960. Ids run 1..127 and wrap, never to 0, because 0 is
    // what an unassigned record reads as
    private static int gmGmkTpTurnAllocDeckRecord()
    {
        if (++gm_gmk_tp_turn_deck_next_id >= GMD_GMK_TP_TURN_DECK_MAX)
            gm_gmk_tp_turn_deck_next_id = 1;

        int kinds = gmGmkTpTurnPlayModeExcludesSonic() ? 2 : GMD_GMK_TP_TURN_KIND_MAX;
        byte packed = 0;

        for (int col = 0; col < GMD_GMK_TP_TURN_PICT_PER_LINE; ++col)
            packed |= (byte)(mtMathRand() % kinds << 2 * col);

        int id = gm_gmk_tp_turn_deck_next_id;
        gm_gmk_tp_turn_deck_kinds[id] = packed;
        gm_gmk_tp_turn_deck_revealed[id] = 0;
        return id;
    }

    // PC gmGmkTpTurnAllocDeckRecords4 0x4F7A00. A board takes one of eight canned layouts rather
    // than rolling sixteen faces, and consumes four consecutive record ids, one per row
    private static int gmGmkTpTurnAllocDeckRecords4()
    {
        int first = gm_gmk_tp_turn_deck_next_id + 1;
        if (gm_gmk_tp_turn_deck_next_id + 5 >= GMD_GMK_TP_TURN_DECK_MAX)
            first = 1;

        gm_gmk_tp_turn_deck_next_id = first + 3;

        byte[] layout = gm_gmk_tp_turn_board_layout_tbl[mtMathRand() & GMD_GMK_TP_TURN_BOARD_LAYOUT_MAX - 1];
        bool no_sonic = gmGmkTpTurnPlayModeExcludesSonic();

        for (int row = 0; row < GMD_GMK_TP_TURN_PICT_PER_LINE; ++row)
        {
            byte packed = layout[row];

            if (no_sonic)
            {
                for (int col = 0; col < GMD_GMK_TP_TURN_PICT_PER_LINE; ++col)
                {
                    int shift = 2 * col;
                    if ((packed >> shift & 3) == GMD_GMK_TP_TURN_KIND_SONIC)
                        packed = (byte)(packed & ~(3 << shift) | GMD_GMK_TP_TURN_KIND_RING << shift);
                }
            }

            gm_gmk_tp_turn_deck_kinds[first + row] = packed;
            gm_gmk_tp_turn_deck_revealed[first + row] = 0;
        }

        return first;
    }

    // the 1UP reward is blocked outside stages 21-27 in this mode anyway, so the deck drops Sonic
    // rather than dealing a face that could never pay out
    private static bool gmGmkTpTurnPlayModeExcludesSonic()
    {
        return g_gs_main_sys_info.game_mode == 1;
    }

    private static void gmGmkTpTurnMarkDeckRevealed(GMS_GMK_TP_TURN_PICT_WORK work)
    {
        if (work.card == null || gm_gmk_tp_turn_deck_revealed == null)
            return;

        // only the board has rows of its own; every other variant is a single line
        int row = work.card.variant == GMD_GMK_TP_TURN_VARIANT_BOARD ? work.row : 0;
        int record = work.card.deck_id + row;
        if (record < 0 || record >= GMD_GMK_TP_TURN_DECK_MAX)
            return;

        int shift = 2 * work.col;
        gm_gmk_tp_turn_deck_revealed[record] =
            (byte)(gm_gmk_tp_turn_deck_revealed[record] & ~(3 << shift) | 1 << shift);
    }

    // PC gmGmkTpTurnEntryMatch 0x4F8180, reached when a flip animation finishes rather than when
    // the panel is touched, so the reward lands on the reveal
    private static void gmGmkTpTurnEntryMatch(VecFx32 pos, int kind, int card_idx)
    {
        if (gm_gmk_tp_turn_pending == null)
            return;

        if (kind != GMD_GMK_TP_TURN_KIND_RING && kind != GMD_GMK_TP_TURN_KIND_SONIC)
            return;

        int used = 0;
        while (used < GMD_GMK_TP_TURN_PENDING_PER_KIND
            && gm_gmk_tp_turn_pending[GMD_GMK_TP_TURN_PENDING_PER_KIND * kind + used].used)
        {
            ++used;
        }

        if (used < GMD_GMK_TP_TURN_PENDING_PER_KIND)
        {
            GMS_GMK_TP_TURN_PENDING slot =
                gm_gmk_tp_turn_pending[GMD_GMK_TP_TURN_PENDING_PER_KIND * kind + used];

            slot.used = true;
            slot.kind = kind;
            slot.pos = pos;
            slot.ttl = GMD_GMK_TP_TURN_PENDING_TTL;
            slot.card_idx = card_idx;
            return;
        }

        gmGmkTpTurnLightCard(card_idx);

        for (int i = 0; i < GMD_GMK_TP_TURN_PENDING_PER_KIND; ++i)
        {
            GMS_GMK_TP_TURN_PENDING slot =
                gm_gmk_tp_turn_pending[GMD_GMK_TP_TURN_PENDING_PER_KIND * kind + i];

            gmGmkTpTurnLightCard(slot.card_idx);
            gmGmkTpTurnClearPending(slot);
        }

        // TODO(port): PC bursts an effect at all three matched panels here, through the same
        // id-indexed spawner the flip sparkle uses and which this tree has no counterpart for

        gmGmkTpTurnPayMatch(kind);
        GmSoundPlaySE("Cards2");
    }

    private static void gmGmkTpTurnLightCard(int card_idx)
    {
        if (gm_gmk_tp_turn_card_timer == null)
            return;

        if (card_idx < 0 || card_idx >= GMD_GMK_TP_TURN_CARD_IDX_MAX)
            return;

        gm_gmk_tp_turn_card_timer[card_idx] += GMD_GMK_TP_TURN_HIT_FRAMES;
    }

    private static void gmGmkTpTurnPayMatch(int kind)
    {
        GMS_PLAYER_WORK ply_work = g_gm_main_system.ply_work[0];
        if (ply_work == null)
            return;

        if (kind == GMD_GMK_TP_TURN_KIND_RING)
        {
            // the standard hundred-ring 1UP chain applies to the rings this adds
            GmPlayerRingGet(ply_work, GMD_GMK_TP_TURN_MATCH_RINGS);
            return;
        }

        GmPlayerStockGet(ply_work, 1);
        GmSoundPlayJingle1UP(true);
    }
}
