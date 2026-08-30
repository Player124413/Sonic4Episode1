using System;
using System.Collections.Generic;
using er;
using er.web;
using gs.backup;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

using static AppMain;

internal static partial class DmTitle
{
    private static void dmTitleSetChngFocusCrsrData(DMS_TITLE_MAIN_WORK main_work)
    {
        if (((int)main_work.flag & 64) != 0)
        {
            main_work.prev_slct_menu = main_work.cur_slct_menu;
            main_work.cur_slct_menu =
                dmTitleGetRevisedMenuFocus(main_work.cur_slct_menu, -1, main_work.slct_menu_num);
            main_work.src_crsr_pos_y = main_work.cur_crsr_pos_y;
            main_work.dst_crsr_pos_y = !GsTrialIsTrial()
                ? dm_title_crsr_pos_y_tbl[0] + main_work.cur_slct_menu * dm_title_crsr_pos_y_tbl[1]
                : dm_title_variant.crsr_trial_pos_y_tbl[main_work.cur_slct_menu];
            main_work.flag |= 256U;
        }

        if (((int)main_work.flag & 128) == 0)
            return;
        main_work.prev_slct_menu = main_work.cur_slct_menu;
        main_work.cur_slct_menu = dmTitleGetRevisedMenuFocus(main_work.cur_slct_menu, 1, main_work.slct_menu_num);
        main_work.src_crsr_pos_y = main_work.cur_crsr_pos_y;
        main_work.dst_crsr_pos_y = !GsTrialIsTrial()
            ? dm_title_crsr_pos_y_tbl[0] + main_work.cur_slct_menu * dm_title_crsr_pos_y_tbl[1]
            : dm_title_variant.crsr_trial_pos_y_tbl[main_work.cur_slct_menu];
        main_work.flag |= 256U;
    }

    private static int dmTitleGetRevisedMenuFocus(int idx, int diff, int menu_num)
    {
        int num = idx + diff;
        if (num < 0)
            num = menu_num - 1;
        if (num >= menu_num)
            num = 0;
        return num;
    }

    private static void dmTitleSetCtrlFocusChangeEfct(DMS_TITLE_MAIN_WORK main_work)
    {
        float[] numArray1 = new float[2];
        float[] numArray2 = new float[2]
        {
            0.0f,
            main_work.dst_crsr_pos_y - main_work.src_crsr_pos_y
        };
        numArray1[1] = numArray2[1] / 8f;
        main_work.cur_crsr_pos_y += numArray1[1];
    }

    private static void dmTitleSetWinOpenEfct(DMS_TITLE_MAIN_WORK main_work)
    {
        if (main_work.win_timer > 8.0)
        {
            main_work.flag |= 65536U;
            main_work.win_timer = 0.0f;
            for (uint index = 0; index < 2U; ++index)
                main_work.win_size_rate[(int)index] = 1f;
        }
        else
            ++main_work.win_timer;

        for (uint index = 0; index < 2U; ++index)
        {
            main_work.win_size_rate[(int)index] = main_work.win_timer == 0.0 ? 1f : main_work.win_timer / 8f;
            if (main_work.win_size_rate[(int)index] > 1.0)
                main_work.win_size_rate[(int)index] = 1f;
        }
    }

    private static void dmTitleSetWinCloseEfct(DMS_TITLE_MAIN_WORK main_work)
    {
        for (uint index = 0; index < 2U; ++index)
            main_work.win_size_rate[(int)index] = main_work.win_timer == 0.0 ? 0.0f : main_work.win_timer / 8f;
        if (main_work.win_timer < 0.0)
        {
            main_work.flag |= 65536U;
            main_work.win_timer = 0.0f;
            for (uint index = 0; index < 2U; ++index)
                main_work.win_size_rate[(int)index] = 0.0f;
        }
        else
            --main_work.win_timer;
    }

    private static void dmTitleSetMMenuWinOpenEfct(DMS_TITLE_MAIN_WORK main_work)
    {
        if (0.0 == main_work.mmenu_win_timer)
            DmSoundPlaySE("Window");
        if (main_work.mmenu_win_timer > 8.0)
        {
            main_work.flag |= 4194304U;
            main_work.mmenu_win_timer = 0.0f;
            for (uint index = 0; index < 2U; ++index)
                main_work.mmenu_win_size_rate[(int)index] = 1f;
        }
        else
            ++main_work.mmenu_win_timer;

        for (uint index = 0; index < 2U; ++index)
        {
            main_work.mmenu_win_size_rate[(int)index] =
                main_work.mmenu_win_timer == 0.0 ? 1f : main_work.mmenu_win_timer / 8f;
            if (main_work.mmenu_win_size_rate[(int)index] > 1.0)
                main_work.mmenu_win_size_rate[(int)index] = 1f;
        }
    }

    private static void dmTitleSetMMenuWinCloseEfct(DMS_TITLE_MAIN_WORK main_work)
    {
        for (uint index = 0; index < 2U; ++index)
            main_work.mmenu_win_size_rate[(int)index] =
                main_work.mmenu_win_timer == 0.0 ? 0.0f : main_work.mmenu_win_timer / 8f;
        if (main_work.mmenu_win_timer < 0.0)
        {
            main_work.flag |= 4194304U;
            main_work.mmenu_win_timer = 0.0f;
            for (uint index = 0; index < 2U; ++index)
                main_work.mmenu_win_size_rate[(int)index] = 0.0f;
        }
        else
            --main_work.mmenu_win_timer;
    }

    private static bool dmTitleIsCtrlFocusChangeEfctEnd(DMS_TITLE_MAIN_WORK main_work)
    {
        float num = main_work.dst_crsr_pos_y - main_work.src_crsr_pos_y;
        if (main_work.cur_crsr_pos_y >= (double)main_work.dst_crsr_pos_y && num >= 0.0)
        {
            main_work.cur_crsr_pos_y = main_work.dst_crsr_pos_y;
            main_work.timer = 0.0f;
            return true;
        }

        if (main_work.cur_crsr_pos_y <= (double)main_work.dst_crsr_pos_y && num <= 0.0)
        {
            main_work.cur_crsr_pos_y = main_work.dst_crsr_pos_y;
            main_work.timer = 0.0f;
            return true;
        }

        ++main_work.timer;
        return false;
    }
}
