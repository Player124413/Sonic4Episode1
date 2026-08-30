using static AppMain;

using static DmSave.DME_SAVE_ACT;
using static DmSave.DME_SAVE_WIN;

internal static partial class DmSave
{
    private static (int w, int h) dmSaveWindowSize(DMS_SAVE_VARIANT variant, int lang, bool attention)
    {
        if (attention)
        {
            return (
                (int)((DMD_SAVE_WINDOW_SIZE_W + 64) * variant.text_scale),
                (int)((DMD_SAVE_WINDOW_SIZE_H + dm_save_win_size_y_tbl[lang]) * variant.text_scale));
        }

        var base_w = variant.ge_widens_window && lang == GSD_LANGUAGE_GE
            ? DMD_SAVE_WINDOW_SIZE_W + 64
            : DMD_SAVE_WINDOW_SIZE_W;

        return (
            (int)(base_w * variant.text_scale),
            (int)(DMD_SAVE_WINDOW_SIZE_H * variant.text_scale));
    }

    internal static (int w, int h) WindowSizeForTest(bool console, int lang, bool attention)
    {
        return dmSaveWindowSize(SaveVariantForTest(console), lang, attention);
    }

    public static void DmSaveWinSelectDraw()
    {
        AoActSysSetDrawTaskPrio(DMD_SAVE_DRAW_PRIO_WIN_FIX);

        var lang = GsEnvGetLanguage();
        var scale = dm_save_variant.text_scale;

        var attention = (dm_save_msg_flag & (1 << (int)DME_SAVE_WIN_ATTENTION_SAVE)) != 0;
        var (tmp_size_w, tmp_size_h) = dmSaveWindowSize(dm_save_variant, lang, attention);

        uint tmp_win_tex_id = !dm_save_is_task_draw
            ? ((dm_save_msg_flag & (1 << (int)DME_SAVE_WIN_ATTENTION_SAVE)) == 0 ? 0U : 1U)
            : 1U;

        if (dm_save_is_draw_state != 0U)
        {
            AoWinSysDrawState(
                0,
                AoTexGetTexList(dm_save_cmn_tex[0]),
                tmp_win_tex_id,
                DMD_SAVE_SIZE_WIDTH / 2f,
                DMD_SAVE_SIZE_HEIGHT / 2f,
                tmp_size_w * dm_save_win_size_rate[0],
                tmp_size_h * dm_save_win_size_rate[1],
                dm_save_draw_state);
        }
        else
        {
            // TODO(wam): the 0.9 has no counterpart in the C++ (dmSave.cpp:1602 is a plain
            // multiply). WP7 tweak or decompiler artefact, unknown which, so it stays on both arms
            AoWinSysDrawTask(
                0,
                AoTexGetTexList(dm_save_cmn_tex[0]),
                tmp_win_tex_id,
                DMD_SAVE_SIZE_WIDTH / 2f,
                DMD_SAVE_SIZE_HEIGHT / 2f,
                tmp_size_w * dm_save_win_size_rate[0],
                (float)(tmp_size_h * (double)dm_save_win_size_rate[1] * 0.899999976158142),
                DMD_SAVE_DRAW_PRIO_WIN);
        }

        if ((dm_save_disp_flag & DMD_SAVE_DISP_FLAG_WIN_ACT) != 0)
        {
            switch (dm_save_win_mode)
            {
                case (int)DME_SAVE_WIN_DATA_LOADING:
                    AoActSetTexture(AoTexGetTexList(dm_save_cmn_tex[1]));
                    AoActSortRegAction(dm_save_act[(int)ACT_TEX_MSG1]);
                    break;

                case (int)DME_SAVE_WIN_ATTENTION_SAVE:
                    AoActSetTexture(AoTexGetTexList(dm_save_cmn_tex[1]));
                    AoActSortRegAction(dm_save_act[(int)ACT_TEX_MSG2]);
                    AoActSortRegAction(dm_save_act[(int)ACT_TEX_OK]);
                    break;

                case (int)DME_SAVE_WIN_NOW_SAVING:
                    AoActSetTexture(AoTexGetTexList(dm_save_cmn_tex[1]));
                    AoActSortRegAction(dm_save_act[(int)ACT_TEX_MSG3]);
                    break;

                case (int)DME_SAVE_WIN_TRIAL_OUT_SAVE:
                    AoActSetTexture(AoTexGetTexList(dm_save_cmn_tex[1]));
                    AoActSortRegAction(dm_save_act[(int)ACT_TEX_MSG3]);
                    break;
            }

            AoActAcmPush();

            int tmp_tex_dist = lang == 0 ? 0 : dm_save_win_size_y_tbl[lang] / 2;

            for (int index = 0; index < ACT_COUNT; ++index)
            {
                AOS_TEXTURE tex = index < (int)ACT_TEX_WINTITLE ? dm_save_cmn_tex[0] : dm_save_cmn_tex[1];

                AoActAcmInit();
                AoActAcmApplyTrans(
                    dm_save_win_act_pos_tbl[index][0],
                    dm_save_win_act_pos_tbl[index][1],
                    0.0f);

                if ((dm_save_msg_flag & (1 << (int)DME_SAVE_WIN_ATTENTION_SAVE)) != 0)
                {
                    if (index == ACT_COUNT - 1)
                        AoActAcmApplyTrans(0.0f, 16f + tmp_tex_dist, 0.0f);
                    else if (index == 0 || index == 1)
                        AoActAcmApplyTrans(-32f, tmp_tex_dist * -1, 0.0f);
                }

                AoActAcmApplyScale(scale, scale);

                AoActSetTexture(AoTexGetTexList(tex));
                AoActUpdate(dm_save_act[index], 0.0f);
            }

            AoActAcmPop();

            AoActSortExecute();
            AoActSortDraw();
            AoActSortUnregAll();
        }

        if (dm_save_is_draw_state == 0U || !dm_save_is_task_draw)
            return;

        amDrawMakeTask(new TaskProc(dmSaveTaskDraw), DMD_SAVE_DRAW_TASK_PRIO, 0U);
    }

    private static void dmSaveTaskDraw(AMS_TCB tcb)
    {
        AoActDrawPre();
        amDrawExecCommand(dm_save_draw_state);
        amDrawEndScene();
    }
}
