internal partial class AppMain
{
    // the four flip panels on a card. never placed in a map: a card spawns its own through
    // GmEventMgrLocalEventBirth, so the panel's kind, column, row and revealed state arrive
    // packed into the maker's type byte.
    //
    // running into one flips it; the spin axis and direction come from how the player was moving
    // at contact, and when the flip finishes the revealed kind goes to the match logic in
    // GmGmkTpTurnMatch.cs.

    internal const int GMD_GMK_TP_TURN_KIND_EGG = 0;
    internal const int GMD_GMK_TP_TURN_KIND_RING = 1;
    internal const int GMD_GMK_TP_TURN_KIND_SONIC = 2;
    internal const int GMD_GMK_TP_TURN_KIND_MAX = 3;

    internal const int GMD_GMK_TP_TURN_PICT_PER_LINE = 4;

    // half a step, so panel i sits at (2i - 3) times this. 18.0 across, 24.5 down
    private const int GMD_GMK_TP_TURN_PICT_STEP_X_HALF = 73728;
    private const int GMD_GMK_TP_TURN_PICT_STEP_Y_HALF = 100352;

    // panels sit in front of their frame, at an absolute z rather than one relative to the card
    private const int GMD_GMK_TP_TURN_PICT_POS_Z = -393216;

    // variant 5 is the bottom-embedded card, and its whole panel line shifts up with the frame
    private const int GMD_GMK_TP_TURN_PICT_VARIANT5_OFST_Y = -16384;

    // a hair under 180 degrees. The off-by-one is deliberate: it stops the renderer picking the
    // spin direction arbitrarily when the panel is turned exactly edge on
    private const ushort GMD_GMK_TP_TURN_PICT_DIR_HIDDEN = 0x7FFF;

    private const ushort GMD_GMK_TP_TURN_PICT_DEF_FLAG = 0xFFFE;

    // 65536 / 360, degrees per frame into a binary angle
    private const float GMD_GMK_TP_TURN_DEG_TO_ANGLE = 182.04443359375f;

    private const float GMD_GMK_TP_TURN_SPIN_DEG = 15f;

    // 108 frames at 15 degrees is 4.5 turns, so a panel ends face up. The diagonals get 96, a
    // whole 4 turns, because they zero the tumble angle instead of relying on the half turn
    private const int GMD_GMK_TP_TURN_FLIP_FRAMES = 108;
    private const int GMD_GMK_TP_TURN_FLIP_FRAMES_DIAGONAL = 96;

    private const int GMD_GMK_TP_TURN_FLIP_EFFECT_Z_OFST = 0x20000;
    private const int GMD_GMK_TP_TURN_EFFECT_MAX = 8;

    private static int gm_gmk_tp_turn_effect_num;

    // 2.0 units per frame. Below this in both axes the panel tumbles in place instead
    private const int GMD_GMK_TP_TURN_SPIN_SPD_THRESHOLD = 0x2000;

    private const int GMD_GMK_TP_TURN_TUMBLE_CYCLE = 24;

    // the board lays out four lines, one per deck record; everything else is a single line,
    // across for the row variants and down for the column ones
    private static void gmGmkTpTurnSpawnPicts(GMS_GMK_TP_TURN_WORK work)
    {
        OBS_OBJECT_WORK obj_work = (OBS_OBJECT_WORK)work;

        if (work.variant == GMD_GMK_TP_TURN_VARIANT_BOARD)
        {
            work.deck_id = gmGmkTpTurnDeckIdFor(work, true);

            for (int row = 0; row < GMD_GMK_TP_TURN_PICT_PER_LINE; ++row)
            {
                int record = work.deck_id + row;
                work.deck_kinds = gm_gmk_tp_turn_deck_kinds[record];
                work.deck_revealed = gm_gmk_tp_turn_deck_revealed[record];

                int y = obj_work.pos.y + (2 * row - 3) * GMD_GMK_TP_TURN_PICT_STEP_Y_HALF;
                gmGmkTpTurnBirthPictLine(work, obj_work.pos.x, y, row, true);
            }

            return;
        }

        bool horizontal = work.variant == GMD_GMK_TP_TURN_VARIANT_ROW
            || work.variant == 3
            || work.variant == 4;

        work.deck_id = gmGmkTpTurnDeckIdFor(work, false);
        work.deck_kinds = gm_gmk_tp_turn_deck_kinds[work.deck_id];
        work.deck_revealed = gm_gmk_tp_turn_deck_revealed[work.deck_id];

        int line_y = obj_work.pos.y;
        if (work.variant == 5)
            line_y += GMD_GMK_TP_TURN_PICT_VARIANT5_OFST_Y;

        gmGmkTpTurnBirthPictLine(work, obj_work.pos.x, line_y, 0, horizontal);
    }

    private static void gmGmkTpTurnBirthPictLine(
      GMS_GMK_TP_TURN_WORK work,
      int pos_x,
      int pos_y,
      int row,
      bool horizontal)
    {
        OBS_OBJECT_WORK card_obj = (OBS_OBJECT_WORK)work;

        for (int col = 0; col < GMD_GMK_TP_TURN_PICT_PER_LINE; ++col)
        {
            int x = pos_x;
            int y = pos_y;

            if (horizontal)
                x += (2 * col - 3) * GMD_GMK_TP_TURN_PICT_STEP_X_HALF;
            else
                y += (2 * col - 3) * GMD_GMK_TP_TURN_PICT_STEP_Y_HALF;

            int shift = 2 * col;
            int kind = (work.deck_kinds >> shift) & 3;
            int revealed = (work.deck_revealed >> shift) & 1;
            byte packed = (byte)((row << 6) | (col << 4) | (kind << 1) | revealed);

            OBS_OBJECT_WORK pict_obj = GmEventMgrLocalEventBirth(
                GMD_EVENT_ID_NOSET_TP_TURN_PICT, x, y, 0, 0, 0, 0, 0, packed);

            if (pict_obj == null)
                continue;

            pict_obj.pos.z = GMD_GMK_TP_TURN_PICT_POS_Z;

            // The panel never range-checks itself out, so this link is the only thing that ends
            // it: when the card is culled the object system passes its death down to every child
            // that has not asked to outlive its parent. Without it a card scrolling in and out
            // would leak four local event records a time out of a pool of sixty four
            ObjObjectParentSet(pict_obj, card_obj, 0U);
            ((GMS_GMK_TP_TURN_PICT_WORK)pict_obj).card = work;
        }
    }

    private static OBS_OBJECT_WORK GmGmkTpTurnInitPict(
      GMS_EVE_RECORD_EVENT eve_rec,
      int pos_x,
      int pos_y,
      byte type)
    {
        GMS_GMK_TP_TURN_PICT_WORK work = (GMS_GMK_TP_TURN_PICT_WORK)GMM_ENEMY_CREATE_WORK(
            eve_rec, pos_x, pos_y, () => new GMS_GMK_TP_TURN_PICT_WORK(), "GMK_TP_TURN_PICT");

        GMS_ENEMY_3D_WORK gmk_work = (GMS_ENEMY_3D_WORK)work;
        OBS_OBJECT_WORK obj_work = gmk_work.ene_com.obj_work;

        gmk_work.ene_com.rect_work[0].flag &= ~4U;
        gmk_work.ene_com.rect_work[1].flag &= ~4U;

        int kind = (type & 0x0F) >> 1;
        ObjObjectCopyAction3dNNModel(obj_work, g_gm_gmk_tp_turn_obj_3d_list[kind], gmk_work.obj_3d);

        gmGmkTpTurnSetupPict(work, kind, (type & 0x3F) >> 4, type >> 6, (type & 1) != 0);

        return obj_work;
    }

    private static void gmGmkTpTurnSetupPict(
      GMS_GMK_TP_TURN_PICT_WORK work,
      int kind,
      int col,
      int row,
      bool revealed)
    {
        GMS_ENEMY_COM_WORK ene_com = ((GMS_ENEMY_3D_WORK)work).ene_com;
        OBS_OBJECT_WORK obj_work = ene_com.obj_work;

        work.kind = kind;
        work.col = col;
        work.row = row;
        work.revealed = revealed;

        obj_work.flag |= OBD_OBJECT_NOCLIP;
        obj_work.move_flag |= OBD_MOVE_NOMOVE | OBD_MOVE_NOCOL;
        obj_work.ppMove = null;
        obj_work.disp_flag |= OBD_DISP_DIR2DFLIP;

        OBS_RECT_WORK pRec = ene_com.rect_work[2];
        ObjRectWorkZSet(pRec, -16, -16, -500, 16, 16, 500);
        pRec.rect.pos.x = 0;
        pRec.rect.pos.y = 0;
        pRec.rect.pos.z = 0;

        pRec.flag |= 4U;
        pRec.flag = pRec.flag & 0xFFFEF8FBU | 0x404U;
        pRec.ppHit = null;
        pRec.ppDef = new OBS_RECT_WORK_Delegate1(gmGmkTpTurnPictTouch);

        // no attack of its own, and it accepts contact from anything. This is a plain touch box:
        // walking into a panel flips it, no attack state required
        ObjRectAtkSet(pRec, 0, 0);
        ObjRectDefSet(pRec, GMD_GMK_TP_TURN_PICT_DEF_FLAG, 0);

        if (revealed)
            gmGmkTpTurnPictSetRevealed(work);
        else
            gmGmkTpTurnPictSetHidden(work);
    }

    private static void gmGmkTpTurnPictSetRevealed(GMS_GMK_TP_TURN_PICT_WORK work)
    {
        OBS_OBJECT_WORK obj_work = (OBS_OBJECT_WORK)work;

        work.revealed = true;
        work.flip_timer = 0;
        obj_work.disp_flag &= ~OBD_DISP_USERMTX_RIGHT;
        obj_work.ppFunc = null;
        obj_work.dir.x = 0;
        obj_work.dir.y = 0;
        obj_work.dir.z = 0;
    }

    private static void gmGmkTpTurnPictSetHidden(GMS_GMK_TP_TURN_PICT_WORK work)
    {
        OBS_OBJECT_WORK obj_work = (OBS_OBJECT_WORK)work;

        work.revealed = false;
        obj_work.ppFunc = null;
        obj_work.dir.x = 0;
        obj_work.dir.y = GMD_GMK_TP_TURN_PICT_DIR_HIDDEN;
        obj_work.dir.z = 0;
    }

    // any contact at all starts a flip
    private static void gmGmkTpTurnPictTouch(OBS_RECT_WORK pict_rect, OBS_RECT_WORK other_rect)
    {
        GMS_GMK_TP_TURN_PICT_WORK work = (GMS_GMK_TP_TURN_PICT_WORK)pict_rect.parent_obj;

        // the rect only fires once per unbroken contact, but a panel is wide enough to step out of
        // and back into. without this guard you could flip the same panel repeatedly and re-offer
        // its kind to the match every time
        if (work.revealed || work.flip_timer > 0)
            return;

        GMS_PLAYER_WORK ply_work = g_gm_main_system.ply_work[0];
        if (ply_work == null)
            return;

        gmGmkTpTurnMarkDeckRevealed(work);

        OBS_OBJECT_WORK ply_obj = ply_work.obj_work;
        int vx = ply_obj.pos.x - ply_obj.prev_pos.x;
        int vy = ply_obj.pos.y - ply_obj.prev_pos.y;

        int mode = 0;
        float speed = 0f;

        if (vx > GMD_GMK_TP_TURN_SPIN_SPD_THRESHOLD && vy > GMD_GMK_TP_TURN_SPIN_SPD_THRESHOLD)
        {
            mode = 3;
            speed = GMD_GMK_TP_TURN_SPIN_DEG;
        }
        else if (vx > GMD_GMK_TP_TURN_SPIN_SPD_THRESHOLD && vy < -GMD_GMK_TP_TURN_SPIN_SPD_THRESHOLD)
        {
            mode = 2;
            speed = -GMD_GMK_TP_TURN_SPIN_DEG;
        }
        else if (vx < -GMD_GMK_TP_TURN_SPIN_SPD_THRESHOLD && vy < -GMD_GMK_TP_TURN_SPIN_SPD_THRESHOLD)
        {
            mode = 3;
            speed = -GMD_GMK_TP_TURN_SPIN_DEG;
        }
        else if (vx < -GMD_GMK_TP_TURN_SPIN_SPD_THRESHOLD && vy > GMD_GMK_TP_TURN_SPIN_SPD_THRESHOLD)
        {
            mode = 2;
            speed = GMD_GMK_TP_TURN_SPIN_DEG;
        }
        else if (vx > 0)
        {
            mode = 0;
            speed = GMD_GMK_TP_TURN_SPIN_DEG;
        }
        else if (vx < 0)
        {
            mode = 0;
            speed = -GMD_GMK_TP_TURN_SPIN_DEG;
        }
        else if (vy > 0)
        {
            mode = 1;
            speed = GMD_GMK_TP_TURN_SPIN_DEG;
        }
        else if (vy < 0)
        {
            mode = 1;
            speed = -GMD_GMK_TP_TURN_SPIN_DEG;
        }
        else
        {
            // dead stop against a panel re-hides it and then runs a zero-speed flip, so the timer
            // still elapses and the reveal still counts even though nothing visibly moves
            gmGmkTpTurnPictSetHidden(work);
        }

        if (mode >= 2)
            ((OBS_OBJECT_WORK)work).dir.y = 0;

        work.spin_mode = mode;
        work.spin_speed = speed;
        work.flip_timer = mode >= 2
            ? GMD_GMK_TP_TURN_FLIP_FRAMES_DIAGONAL
            : GMD_GMK_TP_TURN_FLIP_FRAMES;

        nnMakeUnitQuaternion(ref work.spin);
        work.revealed = true;
        ((OBS_OBJECT_WORK)work).ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkTpTurnPictStateFlip);

        gmGmkTpTurnBirthFlipEffect(work);
        GmSoundPlaySE("Cards1");
    }

    // which anim plays is picked by the spin, and the two diagonal modes tilt the sparkle to line
    // up with the corner it turned about
    private static void gmGmkTpTurnBirthFlipEffect(GMS_GMK_TP_TURN_PICT_WORK work)
    {
        int efct_idx = 0;
        float tilt = 0f;

        switch (work.spin_mode)
        {
            case 0:
                efct_idx = work.spin_speed <= 0f ? 3 : 4;
                break;

            case 1:
                efct_idx = work.spin_speed <= 0f ? 5 : 0;
                break;

            case 2:
                efct_idx = 5;
                tilt = work.spin_speed <= 0f ? 25f : 205f;
                break;

            case 3:
                efct_idx = 5;
                tilt = work.spin_speed <= 0f ? -25f : -205f;
                break;
        }

        OBS_OBJECT_WORK pict = (OBS_OBJECT_WORK)work;
        OBS_OBJECT_WORK efct = (OBS_OBJECT_WORK)GmEfctZoneEsCreate(
            null, GSD_MAIN_ZONE_TYPE_2, efct_idx);

        efct.pos.x = pict.pos.x;
        efct.pos.y = pict.pos.y;
        efct.pos.z = pict.pos.z + GMD_GMK_TP_TURN_FLIP_EFFECT_Z_OFST;
        efct.dir.z = (ushort)AKM_DEGtoA16(tilt);

        ++gm_gmk_tp_turn_effect_num;
        efct.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkTpTurnFlipEffectState);
        mtTaskChangeTcbDestructor(efct.tcb,
            new GSF_TASK_PROCEDURE(gmGmkTpTurnFlipEffectDest));
    }

    // past the cap the oldest one still alive kills itself, and hands its destructor back so it
    // does not decrement the count a second time on the way out
    private static void gmGmkTpTurnFlipEffectState(OBS_OBJECT_WORK obj_work)
    {
        if (gm_gmk_tp_turn_effect_num > GMD_GMK_TP_TURN_EFFECT_MAX)
        {
            obj_work.flag |= 8U;
            --gm_gmk_tp_turn_effect_num;
            mtTaskChangeTcbDestructor(obj_work.tcb, new GSF_TASK_PROCEDURE(ObjObjectExit));
        }

        GmEffectDefaultMainFuncDeleteAtEnd(obj_work);
    }

    private static void gmGmkTpTurnFlipEffectDest(MTS_TASK_TCB tcb)
    {
        --gm_gmk_tp_turn_effect_num;
        ObjObjectExit(tcb);
    }

    private static void gmGmkTpTurnPictStateFlip(OBS_OBJECT_WORK obj_work)
    {
        GMS_GMK_TP_TURN_PICT_WORK work = (GMS_GMK_TP_TURN_PICT_WORK)obj_work;

        if (work.card != null)
            work.card.child_flipping = true;

        float ax = 0f;
        float ay = 0f;

        switch (work.spin_mode)
        {
            case 0:
                ay = 1f;
                break;

            case 1:
                // the axis flips sign twice per 24 frames, and the panel is turned a half turn
                // with it so the face keeps pointing the right way through the tumble
                int phase = work.flip_timer % GMD_GMK_TP_TURN_TUMBLE_CYCLE;
                if (phase <= 6 || phase > 18)
                {
                    ax = -1f;
                    obj_work.dir.y = 0x8000;
                }
                else
                {
                    ax = 1f;
                    obj_work.dir.y = 0;
                }
                break;

            case 2:
                ax = 2f;
                ay = -3f;
                break;

            case 3:
                ax = 2f;
                ay = 3f;
                break;
        }

        NNS_QUATERNION step;
        nnMakeRotateAxisQuaternion(out step, ax, ay, 0f,
            (short)(int)(work.spin_speed * GMD_GMK_TP_TURN_DEG_TO_ANGLE));
        nnMultiplyQuaternion(ref work.spin, ref work.spin, ref step);

        OBS_ACTION3D_NN_WORK obj_3d = obj_work.obj_3d;
        obj_work.disp_flag |= OBD_DISP_USERMTX_RIGHT;
        nnMakeUnitMatrix(obj_3d.user_obj_mtx_r);
        nnQuaternionMatrix(obj_3d.user_obj_mtx_r, obj_3d.user_obj_mtx_r, ref work.spin);

        if (--work.flip_timer > 0)
            return;

        gmGmkTpTurnPictSetRevealed(work);
        gmGmkTpTurnEntryMatch(obj_work.pos, work.kind, work.card != null ? work.card.card_idx : -1);
    }
}
