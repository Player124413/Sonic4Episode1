using static AppMain;

internal static partial class GmClearDemo
{
    private static void gmClearDemoSetInitDispAct(GMS_CLRDM_MAIN_WORK main_work)
    {
        for (int index = 0; index < 5; ++index)
        {
            ((OBS_OBJECT_WORK) main_work.time_num_act[index]).disp_flag |= 32U;
            ((OBS_OBJECT_WORK) main_work.ring_num_act[index]).disp_flag |= 32U;
        }

        for (int index = 0; index < 9; ++index)
            ((OBS_OBJECT_WORK) main_work.total_num_act[index]).disp_flag |= 32U;
        for (int index = 0; index < 3; ++index)
            ((OBS_OBJECT_WORK) main_work.line_act[index]).disp_flag |= 32U;
        ((OBS_OBJECT_WORK) main_work.sonic_icon_act).disp_flag |= 32U;
        ((OBS_OBJECT_WORK) main_work.sonic_icon_act2).disp_flag |= 32U;
    }

    private static void gmClearDemoSetSortBufAct(GMS_CLRDM_MAIN_WORK main_work)
    {
        if (((int) main_work.flag & 32) == 0)
        {
            for (int index = 0; index < 3; ++index)
                ((OBS_OBJECT_WORK) main_work.line_act[index]).disp_flag |= 32U;
        }
        else
        {
            for (int index = 0; index < 3; ++index)
                ((OBS_OBJECT_WORK) main_work.line_act[index]).disp_flag &= 4294967263U;
        }

        if (((int) main_work.flag & 64) == 0)
            ((OBS_OBJECT_WORK) main_work.sonic_icon_act).disp_flag |= 32U;
        else
            ((OBS_OBJECT_WORK) main_work.sonic_icon_act).disp_flag &= 4294967263U;
        gmClearDemoSetSortBufScore(main_work);
    }

    private static void gmClearDemoSetTimeAtkSortBufAct(GMS_CLRDM_MAIN_WORK main_work)
    {
        if (((int) main_work.flag & 1024) != 0)
        {
            for (int index = 0; index < 7; ++index)
                ((OBS_OBJECT_WORK) main_work.record_time_num_act[index]).disp_flag &= 4294967263U;
        }

        // console picks the icon frame here every time this runs; mobile sets it once when the
        // action is loaded. Same outcome, since Sonic cannot change state on the results screen.
        if (gm_clrdm_variant.time_sonic_icon_per_frame)
        {
            main_work.time_sonic_icon_act.obj_2d.frame = GMM_MAIN_USE_SUPER_SONIC() ? 1.0f : 0.0f;
            main_work.time_sonic_icon_act.obj_2d.speed = 0.0f;
        }

        gmClearDemoSetSortBufScore(main_work);
    }

    private static void gmClearDemoSetRetryInitAct(GMS_CLRDM_MAIN_WORK main_work)
    {
        for (int index = 0; index < 5; ++index)
            ((OBS_OBJECT_WORK) main_work.tex_up_act[index]).disp_flag |= 32U;
        for (int index = 0; index < 7; ++index)
            ((OBS_OBJECT_WORK) main_work.record_time_num_act[index]).disp_flag |= 32U;
        ((OBS_OBJECT_WORK) main_work.tex_big_time_act).disp_flag |= 32U;
        ((OBS_OBJECT_WORK) main_work.time_sonic_icon_act).disp_flag |= 32U;
        ((OBS_OBJECT_WORK) main_work.tex_new_record_act).disp_flag |= 32U;
        // console starts the cursor on RETRY, so its label comes up highlighted; the touch build
        // leaves both flat because the buttons carry the state instead
        main_work.tex_retry_act.obj_2d.frame = gm_clrdm_variant.retry_default_frame;
        main_work.tex_retry_act.obj_2d.speed = 0.0f;
        main_work.tex_back_slct_act.obj_2d.frame = 0.0f;
        main_work.tex_back_slct_act.obj_2d.speed = 0.0f;

        if (gm_clrdm_variant.has_touch_buttons)
        {
            int index1 = 0;
            for (int index2 = arrayof(main_work.btn_retry); index1 < index2; ++index1)
            {
                main_work.btn_retry[index1].obj_2d.frame = 0.0f;
                main_work.btn_retry[index1].obj_2d.speed = 0.0f;
            }

            int index3 = 0;
            for (int index2 = arrayof(main_work.btn_back); index3 < index2; ++index3)
            {
                main_work.btn_back[index3].obj_2d.frame = 0.0f;
                main_work.btn_back[index3].obj_2d.speed = 0.0f;
            }
        }
    }

    private static void gmClearDemoSetRetrySortBufAct(GMS_CLRDM_MAIN_WORK main_work)
    {
        if (main_work.is_clear_spe_stg)
        {
            for (int index = 0; index < 3; ++index)
                ((OBS_OBJECT_WORK) main_work.tex_spst_up_act[index]).disp_flag |= 32U;
        }
        else
        {
            for (int index = 0; index < 5; ++index)
                ((OBS_OBJECT_WORK) main_work.tex_up_act[index]).disp_flag |= 32U;
        }

        for (int index = 0; index < 7; ++index)
            ((OBS_OBJECT_WORK) main_work.record_time_num_act[index]).disp_flag |= 32U;
        ((OBS_OBJECT_WORK) main_work.tex_big_time_act).disp_flag |= 32U;
        ((OBS_OBJECT_WORK) main_work.time_sonic_icon_act).disp_flag |= 32U;
        ((OBS_OBJECT_WORK) main_work.tex_new_record_act).disp_flag |= 32U;
        ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.bg_retry, main_work.bg_retry.obj_2d, null, null, 30, GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[0]), 99U, 0);

        // actions 100-105 are the six nine-slice button pieces, and they are the whole of what the
        // iPhone G_RSLT header adds over the console one
        if (gm_clrdm_variant.has_touch_buttons)
        {
            int index1 = 0;
            for (int index2 = arrayof(main_work.btn_retry); index1 < index2; ++index1)
                ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.btn_retry[index1], main_work.btn_retry[index1].obj_2d, null, null, 30, GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[0]), (uint) (100 + index1), 0);
            int index3 = 0;
            for (int index2 = arrayof(main_work.btn_back); index3 < index2; ++index3)
                ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.btn_back[index3], main_work.btn_back[index3].obj_2d, null, null, 30, GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[0]), (uint) (103 + index3), 0);
        }

        ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.tex_retry_act, main_work.tex_retry_act.obj_2d, null, null, g_gm_clear_demo_data_ama_id[gmClearDemoLanguage()], GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[1]), 16U, 0);
        ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.tex_back_slct_act, main_work.tex_back_slct_act.obj_2d, null, null, g_gm_clear_demo_data_ama_id[gmClearDemoLanguage()], GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[1]), 17U, 0);

        main_work.tex_retry_act.obj_2d.frame = gm_clrdm_variant.retry_default_frame;
        main_work.tex_retry_act.obj_2d.speed = 0.0f;
        main_work.tex_back_slct_act.obj_2d.frame = 0.0f;
        main_work.tex_back_slct_act.obj_2d.speed = 0.0f;

        if (!gm_clrdm_variant.has_touch_buttons)
            return;

        int index4 = 0;
        for (int index2 = arrayof(main_work.btn_retry); index4 < index2; ++index4)
        {
            main_work.btn_retry[index4].obj_2d.frame = 0.0f;
            main_work.btn_retry[index4].obj_2d.speed = 0.0f;
        }

        int index5 = 0;
        for (int index2 = arrayof(main_work.btn_back); index5 < index2; ++index5)
        {
            main_work.btn_back[index5].obj_2d.frame = 0.0f;
            main_work.btn_back[index5].obj_2d.speed = 0.0f;
        }

        main_work.trg_retry.Create(main_work.btn_retry[1].obj_2d.act);
        main_work.trg_back.Create(main_work.btn_back[1].obj_2d.act);
    }

    private static void gmClearDemoSetSortBufScore(GMS_CLRDM_MAIN_WORK main_work)
    {
        if (((int) main_work.flag & 128) != 0)
            gmClearDemoSetSortBufScoreAct(main_work.time_num_act, main_work.time_score[1], 5U);
        if (((int) main_work.flag & 256) != 0)
            gmClearDemoSetSortBufScoreAct(main_work.ring_num_act, main_work.ring_score[1], 5U);
        if (((int) main_work.flag & 512) == 0)
            return;
        gmClearDemoSetSortBufScoreAct(main_work.total_num_act, main_work.total_score[1], 9U);
    }

    private static void gmClearDemoSetSortBufScoreAct(
        GMS_COCKPIT_2D_WORK[] score_act,
        uint score,
        uint digits)
    {
        int num = 1;
        if (score < 10U)
        {
            for (int index = 0; index < (int) digits - 1; ++index)
                ((OBS_OBJECT_WORK) score_act[index]).disp_flag |= 32U;
            ((OBS_OBJECT_WORK) score_act[(int) (digits - 1U)]).disp_flag &= 4294967263U;
        }
        else
        {
            if (score < 10U)
                return;
            for (int index1 = 0; index1 < (int) digits; ++index1)
            {
                for (int index2 = 0; index2 < (int) (digits - index1 - 1L); ++index2)
                    num *= 10;
                if (score < (uint) num)
                    ((OBS_OBJECT_WORK) score_act[index1]).disp_flag |= 32U;
                else
                    ((OBS_OBJECT_WORK) score_act[index1]).disp_flag &= 4294967263U;
                num = 1;
            }
        }
    }

    private static void gmClearDemoSetSortBufTimeAct(GMS_CLRDM_MAIN_WORK main_work)
    {
        int num1 = main_work.time_sec < 10 ? 0 : main_work.time_sec / 10;
        int num2 = main_work.time_msec < 10 ? 0 : main_work.time_msec / 10;
        int num3 = main_work.time_sec % 10;
        int num4 = main_work.time_msec % 10;
        main_work.record_time_num_act[0].obj_2d.frame = main_work.time_min;
        main_work.record_time_num_act[2].obj_2d.frame = num1;
        main_work.record_time_num_act[3].obj_2d.frame = num3;
        main_work.record_time_num_act[5].obj_2d.frame = num2;
        main_work.record_time_num_act[6].obj_2d.frame = num4;
        main_work.record_time_num_act[1].obj_2d.frame = 0.0f;
        main_work.record_time_num_act[4].obj_2d.frame = 0.0f;
    }

    private static void gmClearDemoUpdateAct(GMS_CLRDM_MAIN_WORK main_work)
    {
    }

    private static void gmClearDemoTimeFlushEffect(GMS_CLRDM_MAIN_WORK main_work)
    {
    }

    private static void gmClearDemoSetFlashSonic(GMS_CLRDM_MAIN_WORK main_work)
    {
        if (((int) main_work.flag & 64) != 0)
        {
            if (main_work.flash_timer >= 30.0)
            {
                amFlagOff(ref main_work.flag, 64U);
                main_work.flash_timer = 0.0f;
            }
        }
        else if (main_work.flash_timer >= 10.0)
        {
            amFlagOn(ref main_work.flag, 64U);
            main_work.flash_timer = 0.0f;
        }

        main_work.flash_timer += main_work.count;
    }
}
