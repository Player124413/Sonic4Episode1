using static AppMain;

using static GmPauseMenu.GME_PMENU_ACT;

internal static partial class GmPauseMenu
{
    // SEGA's is (H/2 - (64*2-32)) + 51*i, six rows centred on H/2 + 32. With four rows the same
    // expression sits the block high, so it gets the missing rows' half-height back.
    private static float gmPmDrawGetSelShowPos(int i)
    {
        return GMD_PMENU_SEL_CENTER_Y
               + GMD_PMENU_SEL_PITCH_Y * (i - (GMD_PMENU_SEL_NUM - 1) * 0.5f);
    }

    private static void gmPmDrawAct(GMS_PMENU_WORK work, GME_PMENU_ACT act_id)
    {
        var act = work.act[(int)act_id];
        gmPmUtilSetTexture(act_id);
        AoActUpdate(act);
        AoActSortRegAction(act);
    }

    // the back-button glyph and its label are centred as a pair, hence the per-language width
    private static void gmPmDrawBackButton(GMS_PMENU_WORK work, float x, float y)
    {
        AoActAcmApplyTrans(x, y, 0.0f);

        // on console both act slots carry the same single mark_back id, so this picks the same
        // sprite either way. Kept because it is what decides the glyph everywhere else.
        gmPmDrawAct(work, GeEnvGetDecideKey() == GSE_DECIDE_KEY.GSD_DECIDE_KEY_O
            ? ACT_P_BACK_MARK_X
            : ACT_P_BACK_MARK_O);

        AoActAcmApplyTrans(16.0f, -16.0f, 0.0f);
        gmPmDrawAct(work, ACT_L_BTN_BACK);
    }

    // rate 0 to 1 on the way in, 1 to 0 on the way out. Buttons fly up from below the screen one
    // after another; everything else fades in over the back half.
    private static void gmPmDrawEfctIn(GMS_PMENU_WORK work, float rate)
    {
        AoActAcmPush();

        AOS_ACT_COL col = default;
        col.r = col.g = col.b = 255;
        col.a = rate < 0.5f ? (byte)0 : (byte)(255 * ((rate - 0.5f) * 2.0f));

        if (col.a > 0)
        {
            AoActAcmInit();
            AoActAcmApplyColor(col);
            AoActAcmApplyTrans(GMD_PMENU_SCREEN_W * 0.5f, GMD_PMENU_SEL_CENTER_Y, 0.0f);
            gmPmDrawAct(work, ACT_BTN_BACK);
        }

        var first_pos = -1.0f;
        var last_pos = -1.0f;

        for (int i = 0; i < GMD_PMENU_SEL_NUM; ++i)
        {
            var mrate = (0.5f / GMD_PMENU_SEL_NUM) * i;
            if (mrate > rate)
                continue;

            var rrate = (rate - mrate) * 2.0f;
            if (rrate > 1.0f)
                rrate = 1.0f;

            rrate = 1.0f - rrate;
            rrate = 1.0f - (rrate * rrate);

            var pos = gmPmDrawGetSelShowPos(i);
            if (first_pos < 0.0f)
                first_pos = pos;

            if (rrate < 1.0f)
                pos += GMD_PMENU_SCREEN_H - (GMD_PMENU_SCREEN_H * rrate);

            if (i == GMD_PMENU_SEL_NUM - 1)
                last_pos = pos;

            AoActAcmInit();
            AoActAcmApplyTrans(GMD_PMENU_SCREEN_W * 0.5f, pos - 16.0f, 0.0f);
            gmPmDrawAct(work, ACT_BTN_BASE1 + (i * 2));

            AoActAcmApplyTrans(0.0f, 8.0f, 0.0f);
            gmPmDrawAct(work, gm_pm_sel_act_tbl[i]);
        }

        if (col.a > 0)
        {
            AoActAcmInit();
            AoActAcmApplyColor(col);
            AoActAcmApplyTrans(GMD_PMENU_SCREEN_W * 0.5f, first_pos - 64.0f, 0.0f);
            gmPmDrawAct(work, ACT_TITLE_BASE);

            AoActAcmApplyTrans(0.0f, -16.0f, 0.0f);
            gmPmDrawAct(work, ACT_L_TITLE);

            var bb_pos_x = (GMD_PMENU_SCREEN_W * 0.5f)
                           - (gm_pmenu_bb_width_tbl[GsEnvGetLanguage()] / 2.0f) + 16.0f;

            AoActAcmInit();
            AoActAcmApplyColor(col);
            gmPmDrawBackButton(work, bb_pos_x, last_pos + 48.0f);
        }

        AoActAcmPop();
    }

    // select is a row index, or GMD_PMENU_SEL_NONE for the decide flash's unhighlighted frame
    private static void gmPmDrawSelect(GMS_PMENU_WORK work, int select)
    {
        AoActAcmPush();

        AoActAcmInit();
        AoActAcmApplyTrans(GMD_PMENU_SCREEN_W * 0.5f, GMD_PMENU_SEL_CENTER_Y, 0.0f);
        gmPmDrawAct(work, ACT_BTN_BACK);

        var first_pos = -1.0f;
        var last_pos = -1.0f;

        for (int i = 0; i < GMD_PMENU_SEL_NUM; ++i)
        {
            var pos = gmPmDrawGetSelShowPos(i);
            if (first_pos < 0.0f)
                first_pos = pos;
            last_pos = pos;

            AoActAcmInit();
            AoActAcmApplyTrans(GMD_PMENU_SCREEN_W * 0.5f, pos - 16.0f, 0.0f);

            // the plate and the label each have a plain and a selected sprite, one apart
            var selected = i == select ? 1 : 0;
            gmPmDrawAct(work, ACT_BTN_BASE1 + (i * 2) + selected);

            AoActAcmApplyTrans(0.0f, 8.0f, 0.0f);
            gmPmDrawAct(work, gm_pm_sel_act_tbl[i] + selected);
        }

        AoActAcmInit();
        AoActAcmApplyTrans(GMD_PMENU_SCREEN_W * 0.5f, first_pos - 64.0f, 0.0f);
        gmPmDrawAct(work, ACT_TITLE_BASE);

        AoActAcmApplyTrans(0.0f, -16.0f, 0.0f);
        gmPmDrawAct(work, ACT_L_TITLE);

        var bb_pos_x = (GMD_PMENU_SCREEN_W * 0.5f)
                       - (gm_pmenu_bb_width_tbl[GsEnvGetLanguage()] / 2.0f) + 16.0f;

        AoActAcmInit();
        gmPmDrawBackButton(work, bb_pos_x, last_pos + 48.0f);

        AoActAcmPop();
    }

    private static void gmPmDrawWindowEfctPrev(object param)
    {
        AoActDrawPre();
    }

    // not an action: AoWinSys stretches a 32px corner texture into a 9-slice frame as three
    // triangle strips. The user func re-establishes the 2D camera for the command state first.
    private static void gmPmDrawWindowEfct(GMS_PMENU_WORK work, float rate)
    {
        var gwork = gmPmGetGlbWork();

        ObjDraw3DNNUserFunc(gmPmDrawWindowEfctPrev, null, 0, OBD_DRAW_CMD_STATE_2DAMA);

        AoWinSysDrawState(
            0, AoTexGetTexList(gwork.tex_win), 0U,
            GMD_PMENU_SCREEN_W * 0.5f, GMD_PMENU_SCREEN_H * 0.5f,
            GMD_PMENU_WIN_W * rate, GMD_PMENU_WIN_H * rate,
            AoActSysGetDrawState());
    }

    // select is 0 for yes, 1 for no, or GMD_PMENU_SEL_SUB_NONE for the decide flash
    private static void gmPmDrawWindowMessage(
        GMS_PMENU_WORK work, GME_PMENU_ACT act_no, int select)
    {
        gmPmDrawWindowEfct(work, 1.0f);

        AoActAcmPush();

        const float win_l = (GMD_PMENU_SCREEN_W - GMD_PMENU_WIN_W) / 2.0f;
        const float win_t = (GMD_PMENU_SCREEN_H - GMD_PMENU_WIN_H) / 2.0f;
        const float win_r = win_l + GMD_PMENU_WIN_W;
        const float center_x = GMD_PMENU_SCREEN_W * 0.5f;
        const float win_rt = win_t - 28.0f;

        AoActAcmInit();
        gmPmDrawBackButton(
            work,
            win_r - gm_pmenu_bb_width_tbl[GsEnvGetLanguage()] + 16.0f,
            win_rt + 26.0f);

        AoActAcmInit();
        AoActAcmApplyTrans(center_x, win_t + 26.0f, 0.0f);
        gmPmDrawAct(work, act_no);

        AoActAcmInit();
        AoActAcmApplyTrans(center_x - 60.0f, win_t + 26.0f + 128.0f + 10.0f, 0.0f);
        gmPmDrawAct(work, select == 0 ? ACT_L_DLG_YES_S : ACT_L_DLG_YES);

        AoActAcmInit();
        AoActAcmApplyTrans(center_x + 60.0f, win_t + 26.0f + 128.0f + 10.0f, 0.0f);
        gmPmDrawAct(work, select == 1 ? ACT_L_DLG_NO_S : ACT_L_DLG_NO);

        AoActAcmPop();
    }
}
