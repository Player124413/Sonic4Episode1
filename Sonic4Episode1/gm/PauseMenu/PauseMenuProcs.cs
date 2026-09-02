using static AppMain;

using static GmPauseMenu.GME_PMENU_ACT;
using static GmPauseMenu.GME_PMENU_RESULT;
using static GmPauseMenu.GME_PMENU_SE;

internal static partial class GmPauseMenu
{
    private static void gmPmProcBegin(GMS_PMENU_WORK work)
    {
        var gwork = gmPmGetGlbWork();

        for (int i = 0; i < (int)ACT_NUM; ++i)
        {
            var act = (GME_PMENU_ACT)i;

            A2S_AMA_HEADER ama;
            AOS_TEXTURE tex;
            if (act >= ACT_PLNG_S)
            {
                ama = gwork.file_plng_ama;
                tex = gwork.tex_plng;
            }
            else if (act >= ACT_PLF_S)
            {
                ama = gwork.file_plf_ama;
                tex = gwork.tex_plf;
            }
            else if (act >= ACT_LNG_S)
            {
                ama = gwork.file_lng_ama;
                tex = gwork.tex_lng;
            }
            else
            {
                ama = gwork.file_cmn_ama;
                tex = gwork.tex_cmn;
            }

            AoActSetTexture(AoTexGetTexList(tex));

            var act_id = gm_pm_act_id_tbl[i];

            // with the world map unreachable the same slot carries the main menu label instead
            if (!gmPmGlbIsBackEnable())
            {
                if (act == ACT_PL_BTN_WORLD)
                    act_id = ConsoleIds.IDA_G_PAUSE_P_L_ACT_BTN_MMENU;
                else if (act == ACT_PL_BTN_WORLD_S)
                    act_id = ConsoleIds.IDA_G_PAUSE_P_L_ACT_BTN_MMENU_S;
            }

            if (!gmPmGlbIsSpecialStage() && act == ACT_L_MSG_RETRY)
                act_id = ConsoleIds.IDA_G_PAUSE_L_ACT_MSG_RETRY2;

            work.act[i] = AoActCreate(ama, (uint)act_id);
        }

        gmPmSetProc(gmPmProcEfctIn);
    }

    private static void gmPmProcEfctIn(GMS_PMENU_WORK work)
    {
        var count = gmPmGetProcCount();

        gmPmDrawEfctIn(work, (float)count / GMD_PMENU_EFCT_IN_TIME);

        if (count >= GMD_PMENU_EFCT_IN_TIME)
            gmPmSetProc(gmPmProcSelect);
    }

    private static void gmPmProcSelect(GMS_PMENU_WORK work)
    {
        var is_select = false;

        if (gmPmPadMRepeat().HasFlag(BUTTON.UP))
        {
            if (work.select > 0)
            {
                work.select -= 1;
                is_select = true;
            }
            else if (gmPmPadMStand().HasFlag(BUTTON.UP))
            {
                work.select = GMD_PMENU_SEL_NUM - 1;
                is_select = true;
            }
        }

        if (gmPmPadMRepeat().HasFlag(BUTTON.DOWN))
        {
            if (work.select < GMD_PMENU_SEL_NUM - 1)
            {
                work.select += 1;
                is_select = true;
            }
            else if (gmPmPadMStand().HasFlag(BUTTON.DOWN))
            {
                work.select = 0;
                is_select = true;
            }
        }

        if (is_select)
            gmPmSndPlaySe(GMD_PMENU_SE_CURSOR);

        gmPmDrawSelect(work, work.select);

        if (gmPmPadStand().HasFlag(BUTTON.CONFIRM))
        {
            work.result = gm_pm_sel_result_tbl[work.select];

            // the world map slot answers main menu instead when the world map is not reachable,
            // which is the same swap gmPmProcBegin made to its label
            if (work.result == GME_PMENU_RESULT_BACK && !gmPmGlbIsBackEnable())
                work.result = GME_PMENU_RESULT_MAINMENU;

            if (work.result < GME_PMENU_RESULT_NUM)
            {
                gmPmSetProc(gmPmProcDecide);
                gmPmSndPlaySe(GMD_PMENU_SE_DECIDE);
            }
        }
        else if (gmPmPadStand().HasFlag(BUTTON.CANCEL)
                 || gmPmPadStand().HasFlag(BUTTON.START))
        {
            work.result = GME_PMENU_RESULT_CANCEL;
            gmPmSetProc(gmPmProcEfctOut);
            gmPmSndPlaySe(GMD_PMENU_SE_CANCEL);
        }
    }

    private static void gmPmProcDecide(GMS_PMENU_WORK work)
    {
        var count = gmPmGetProcCount();

        gmPmDrawSelect(work, (count % 8) < 4 ? GMD_PMENU_SEL_NONE : work.select);

        if (count >= GMD_PMENU_EFCT_DECIDE_TIME)
            gmPmSetProc(gmPmProcEfctOut);
    }

    private static void gmPmProcEfctOut(GMS_PMENU_WORK work)
    {
        var count = gmPmGetProcCount();

        gmPmDrawEfctIn(work, 1.0f - ((float)count / GMD_PMENU_EFCT_OUT_TIME));

        if (count < GMD_PMENU_EFCT_OUT_TIME)
            return;

        if (work.result == GME_PMENU_RESULT_RETRY)
            gmPmSetProc(gmPmProcRetryIn);
        else if (work.result == GME_PMENU_RESULT_BACK)
            gmPmSetProc(gmPmProcBackIn);
        else if (work.result == GME_PMENU_RESULT_MAINMENU)
            gmPmSetProc(gmPmProcMainMenuIn);
        else
            gmPmSetProc(gmPmProcEnd);
    }

    // ---- the three confirm dialogs, identical but for their message and their next proc

    private static void gmPmConfirmIn(GMS_PMENU_WORK work, GMS_PMENU_PROC select)
    {
        var count = gmPmGetProcCount();

        gmPmDrawWindowEfct(work, (float)count / GMD_PMENU_EFCT_WIN_TIME);

        if (count >= GMD_PMENU_EFCT_WIN_TIME)
        {
            // "no" is where the cursor starts, so a stray press cannot quit the stage
            work.select_sub = 1;
            gmPmSetProc(select);
        }
    }

    private static void gmPmConfirmSelect(
        GMS_PMENU_WORK work, GME_PMENU_ACT message, GMS_PMENU_PROC decide, GMS_PMENU_PROC out_proc)
    {
        var is_select = false;

        if (gmPmPadMStand().HasFlag(BUTTON.LEFT))
        {
            work.select_sub = 0;
            is_select = true;
        }

        if (gmPmPadMStand().HasFlag(BUTTON.RIGHT))
        {
            work.select_sub = 1;
            is_select = true;
        }

        if (is_select)
            gmPmSndPlaySe(GMD_PMENU_SE_CURSOR);

        gmPmDrawWindowMessage(work, message, work.select_sub);

        if (gmPmPadStand().HasFlag(BUTTON.CONFIRM))
        {
            if (work.select_sub != 0)
                work.result = GME_PMENU_RESULT_NONE;

            gmPmSetProc(decide);
            gmPmSndPlaySe(GMD_PMENU_SE_DECIDE);
        }
        else if (gmPmPadStand().HasFlag(BUTTON.CANCEL))
        {
            work.result = GME_PMENU_RESULT_NONE;
            gmPmSetProc(out_proc);
            gmPmSndPlaySe(GMD_PMENU_SE_CANCEL);
        }
    }

    private static void gmPmConfirmDecide(
        GMS_PMENU_WORK work, GME_PMENU_ACT message, GMS_PMENU_PROC out_proc)
    {
        var count = gmPmGetProcCount();

        gmPmDrawWindowMessage(
            work, message, (count % 8) < 4 ? GMD_PMENU_SEL_SUB_NONE : work.select_sub);

        if (count >= GMD_PMENU_EFCT_DECIDE_TIME)
            gmPmSetProc(out_proc);
    }

    // answering "no" set result to NONE, which is past NUM, and that is what sends the menu all the
    // way back through its 24-frame entry animation rather than returning to the list directly
    private static void gmPmConfirmOut(GMS_PMENU_WORK work)
    {
        var count = gmPmGetProcCount();

        gmPmDrawWindowEfct(work, 1.0f - ((float)count / GMD_PMENU_EFCT_WIN_TIME));

        if (count < GMD_PMENU_EFCT_WIN_TIME)
            return;

        gmPmSetProc(work.result >= GME_PMENU_RESULT_NUM ? gmPmProcEfctIn : gmPmProcEnd);
    }

    private static void gmPmProcRetryIn(GMS_PMENU_WORK work)
    {
        gmPmConfirmIn(work, gmPmProcRetrySelect);
    }

    private static void gmPmProcRetrySelect(GMS_PMENU_WORK work)
    {
        gmPmConfirmSelect(work, ACT_L_MSG_RETRY, gmPmProcRetryDecide, gmPmProcRetryOut);
    }

    private static void gmPmProcRetryDecide(GMS_PMENU_WORK work)
    {
        gmPmConfirmDecide(work, ACT_L_MSG_RETRY, gmPmProcRetryOut);
    }

    private static void gmPmProcRetryOut(GMS_PMENU_WORK work)
    {
        gmPmConfirmOut(work);
    }

    private static void gmPmProcBackIn(GMS_PMENU_WORK work)
    {
        gmPmConfirmIn(work, gmPmProcBackSelect);
    }

    private static void gmPmProcBackSelect(GMS_PMENU_WORK work)
    {
        gmPmConfirmSelect(work, ACT_L_MSG_WOLRD, gmPmProcBackDecide, gmPmProcBackOut);
    }

    private static void gmPmProcBackDecide(GMS_PMENU_WORK work)
    {
        gmPmConfirmDecide(work, ACT_L_MSG_WOLRD, gmPmProcBackOut);
    }

    private static void gmPmProcBackOut(GMS_PMENU_WORK work)
    {
        gmPmConfirmOut(work);
    }

    private static void gmPmProcMainMenuIn(GMS_PMENU_WORK work)
    {
        gmPmConfirmIn(work, gmPmProcMainMenuSelect);
    }

    private static void gmPmProcMainMenuSelect(GMS_PMENU_WORK work)
    {
        gmPmConfirmSelect(work, ACT_L_MSG_MMENU, gmPmProcMainMenuDecide, gmPmProcMainMenuOut);
    }

    private static void gmPmProcMainMenuDecide(GMS_PMENU_WORK work)
    {
        gmPmConfirmDecide(work, ACT_L_MSG_MMENU, gmPmProcMainMenuOut);
    }

    private static void gmPmProcMainMenuOut(GMS_PMENU_WORK work)
    {
        gmPmConfirmOut(work);
    }

    private static void gmPmProcEnd(GMS_PMENU_WORK work)
    {
        gmPmGetGlbWork().result = work.result;
        gmPmSetProc(null);
    }
}
