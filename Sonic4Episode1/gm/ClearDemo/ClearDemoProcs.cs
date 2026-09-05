using er;

using static AppMain;

internal static partial class GmClearDemo
{
    private static void gmClearDemoProcMoveEfct(GMS_CLRDM_MAIN_WORK main_work)
    {
        if (AoActIsEndTrs(main_work.tex_total_act.obj_2d.act))
        {
            main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcPrevCalcScore);
            amFlagOn(ref main_work.flag, 32U);
            amFlagOn(ref main_work.flag, 512U);
        }

        if (AoActIsEndTrs(main_work.tex_time_act.obj_2d.act))
            amFlagOn(ref main_work.flag, 128U);
        if (AoActIsEndTrs(main_work.tex_ring_act.obj_2d.act))
            amFlagOn(ref main_work.flag, 256U);
        gmClearDemoSetNextPromptInput(main_work);
        gmClearDemoSetSortBufAct(main_work);
        gmClearDemoSetScoreData(main_work);
        gmClearDemoUpdateAct(main_work);
    }

    private static void gmClearDemoProcPrevCalcScore(GMS_CLRDM_MAIN_WORK main_work)
    {
        main_work.timer += main_work.count;
        if (main_work.timer > 150.0)
        {
            main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcCalcScore);
            main_work.proc_calc_score = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoSetCalcScore);
            main_work.timer = 0.0f;
        }

        if (gm_clrdm_variant.is_skip_pressed())
        {
            setBackKeyRequest(false);
            amFlagOn(ref main_work.flag, 4U);
        }

        gmClearDemoSetNextPromptInput(main_work);
        gmClearDemoSetSortBufAct(main_work);
        gmClearDemoSetScoreData(main_work);
        gmClearDemoUpdateAct(main_work);
    }

    private static void gmClearDemoProcCalcScore(GMS_CLRDM_MAIN_WORK main_work)
    {
        main_work.timer += main_work.count;
        if (((int) main_work.flag & 4) != 0 || ((int) main_work.flag & 8) != 0)
        {
            main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcWaitDispSonic);
            main_work.time_score[1] = main_work.time_score[0];
            main_work.ring_score[1] = main_work.ring_score[0];
            main_work.total_score[1] = main_work.total_score[0];
            main_work.proc_calc_score = null;
            amFlagOff(ref main_work.flag, 4U);
            amFlagOff(ref main_work.flag, 8U);
            if (main_work.total_score[1] >= 10000U)
            {
                amFlagOn(ref main_work.flag, 16U);
                main_work.idle_time = 270;
            }
            else
                main_work.idle_time = 120;

            HgTrophyTryAcquisition(0);
            main_work.timer = 0.0f;
        }

        // pad check is a WP7 addition on the touch arm; the C++ doesn't have it, so it stays
        // mobile-only instead of widening the console skip to any held button
        if (gm_clrdm_variant.is_skip_pressed()
            || (gm_clrdm_variant.has_touch_buttons && AoPad.AoPadADirect() != 0))
        {
            setBackKeyRequest(false);
            amFlagOn(ref main_work.flag, 4U);
        }

        if (main_work.proc_calc_score != null && main_work.timer > 1.0)
        {
            main_work.proc_calc_score(main_work);
            main_work.timer = 0.0f;
        }

        gmClearDemoSetNextPromptInput(main_work);
        gmClearDemoSetSortBufAct(main_work);
        gmClearDemoSetScoreData(main_work);
        gmClearDemoUpdateAct(main_work);
        if (main_work.proc_calc_score == null)
        {
            GmSoundPlaySE("Result2");
        }
        else
        {
            if (0.0 != main_work.timer)
                return;
            GmSoundPlaySE("Result1");
        }
    }

    private static void gmClearDemoProcWaitDispSonic(GMS_CLRDM_MAIN_WORK main_work)
    {
        if (main_work.timer >= 60.0)
        {
            main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcDispIdle);
            main_work.timer = 0.0f;
            if (((int) main_work.flag & 16) == 0)
                return;
            GmSoundPlayJingle(0U);
            GmPlayerStockGet(g_gm_main_system.ply_work[0], 1);
        }
        else
        {
            gmClearDemoSetNextPromptInput(main_work);
            gmClearDemoSetSortBufAct(main_work);
            gmClearDemoSetScoreData(main_work);
            gmClearDemoUpdateAct(main_work);
            main_work.timer += main_work.count;
        }
    }

    private static void gmClearDemoProcDispIdle(GMS_CLRDM_MAIN_WORK main_work)
    {
        if (main_work.timer >= (double) main_work.idle_time)
        {
            main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcFadeOut);
            IzFadeInitEasy(0U, 1U, 32f);
            gmClearDemoSettleNextPrompt(main_work);
        }

        if (((int) main_work.flag & 16) != 0)
            gmClearDemoSetFlashSonic(main_work);
        gmClearDemoSetNextPromptInput(main_work);
        gmClearDemoSetSortBufAct(main_work);
        gmClearDemoSetScoreData(main_work);
        gmClearDemoUpdateAct(main_work);
        main_work.timer += main_work.count;
    }

    private static void gmClearDemoProcFadeOut(GMS_CLRDM_MAIN_WORK main_work)
    {
        if (IzFadeIsEnd())
        {
            main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcFinish);
            main_work.timer = 0.0f;
        }
        else
        {
            if (((int) main_work.flag & 16) != 0)
                gmClearDemoSetFlashSonic(main_work);
            if (main_work.game_mode == 0)
            {
                gmClearDemoSetNextPromptInput(main_work);
                gmClearDemoSetSortBufAct(main_work);
                gmClearDemoSetScoreData(main_work);
                gmClearDemoUpdateAct(main_work);
            }

            main_work.timer += main_work.count;
        }
    }

    private static void gmClearDemoProcFinish(GMS_CLRDM_MAIN_WORK main_work)
    {
        main_work.proc_update = null;
        main_work.trg_retry.Release();
        main_work.trg_retry.Destructor();
        main_work.trg_back.Release();
        main_work.trg_back.Destructor();
        amFlagOn(ref main_work.flag, 1U);
    }

    private static void gmClearDemoProcTimeMoveEfct(GMS_CLRDM_MAIN_WORK main_work)
    {
        if (AoActIsEndTrs(main_work.tex_big_time_act.obj_2d.act))
        {
            main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcTimeWaitTimeEfct);
            for (int index = 0; index < 7; ++index)
            {
                ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.record_time_num_act[index], main_work.record_time_num_act[index].obj_2d, null, null, 30, GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[0]), (uint) (49 + index), 0);
                ((OBS_OBJECT_WORK) main_work.record_time_num_act[index]).scale.x = 4096;
                ((OBS_OBJECT_WORK) main_work.record_time_num_act[index]).scale.y = 4096;
            }

            gmClearDemoSetClearTimeRecord(main_work);
            gmClearDemoSetSortBufTimeAct(main_work);
            amFlagOn(ref main_work.flag, 1024U);
            amFlagOn(ref main_work.flag, 2048U);
        }

        gmClearDemoSetTimeAtkSortBufAct(main_work);
        gmClearDemoUpdateAct(main_work);
    }

    private static void gmClearDemoProcTimeWaitTimeEfct(GMS_CLRDM_MAIN_WORK main_work)
    {
        if (main_work.timer > 60.0)
        {
            if (((int) main_work.flag & 4096) != 0)
            {
                main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcTimeMoveNewRecord);
                main_work.tex_new_record_act.obj_2d.speed = 1f;
                main_work.idle_time = 180;
            }
            else
            {
                main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcTimeDispEffect);
                main_work.idle_time = 120;
            }

            HgTrophyTryAcquisition(0);
            main_work.timer = 0.0f;
        }

        gmClearDemoSetTimeAtkSortBufAct(main_work);
        gmClearDemoUpdateAct(main_work);
        if (main_work.timer > 120.0)
            return;
        main_work.timer += main_work.count;
    }

    private static void gmClearDemoProcTimeMoveNewRecord(GMS_CLRDM_MAIN_WORK main_work)
    {
        if (AoActIsEndTrs(main_work.tex_new_record_act.obj_2d.act))
        {
            main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcTimeDispEffect);
            GmSoundPlayJingle(5U);
        }

        gmClearDemoSetTimeAtkSortBufAct(main_work);
        gmClearDemoUpdateAct(main_work);
    }

    private static void gmClearDemoProcTimeDispEffect(GMS_CLRDM_MAIN_WORK main_work)
    {
        main_work.timer += main_work.count;
        if (gm_clrdm_variant.is_time_skip_pressed())
            main_work.flag |= 4U;
        if (main_work.timer > (double) main_work.idle_time || ((int) main_work.flag & 4) != 0)
        {
            main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcChangeRetryOut);
            IzFadeInitEasy(0U, 1U, 32f);
            main_work.timer = 0.0f;
            main_work.flag &= 4294967291U;
        }

        gmClearDemoTimeFlushEffect(main_work);
        gmClearDemoSetTimeAtkSortBufAct(main_work);
    }

    private static void gmClearDemoProcRetryStart(GMS_CLRDM_MAIN_WORK main_work)
    {
        main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcChangeRetryOut);
        IzFadeInitEasy(0U, 1U, 32f);
        main_work.timer = 0.0f;
    }

    private static void gmClearDemoProcChangeRetryOut(GMS_CLRDM_MAIN_WORK main_work)
    {
        if (IzFadeIsEnd())
        {
            if (main_work.nodisp_check)
            {
                IzFadeInitEasy(0U, 0U, 32f);
                main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcChangeRetryIn);
                main_work.nodisp_check = false;
                return;
            }

            gmClearDemoSetRetryDispInfo(main_work);
            gmClearDemoSetRetrySortBufAct(main_work);
            main_work.nodisp_check = true;
        }

        gmClearDemoSetTimeAtkSortBufAct(main_work);
    }

    private static void gmClearDemoProcChangeRetryIn(GMS_CLRDM_MAIN_WORK main_work)
    {
        if (!IzFadeIsEnd())
            return;
        main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcWaitSelectRetry);
    }

    private static void gmClearDemoProcWaitSelectRetry(GMS_CLRDM_MAIN_WORK main_work)
    {
        if (gm_clrdm_variant.sets_bg_color_black)
            gmClearDemoSetBgColorBlack();
        gmClearDemoSetRetryInput(main_work);
        if (((int) main_work.flag & 4) != 0)
        {
            main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcWaitRetrySonicRunEfct);
            GmPlySeqChangeTRetryAcc(g_gm_main_system.ply_work[0]);
        }
        else
        {
            if (((int) main_work.flag & 2) == 0)
                return;
            main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcFadeOut);
            IzFadeInitEasy(0U, 1U, 32f);
        }
    }

    private static void gmClearDemoProcWaitRetrySonicRunEfct(GMS_CLRDM_MAIN_WORK main_work)
    {
        if (gm_clrdm_variant.sets_bg_color_black)
            gmClearDemoSetBgColorBlack();
        if (ObjObjectViewOutCheck(g_gm_main_system.ply_work[0].obj_work) == 0)
            return;
        main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcFadeOut);
        main_work.flag |= 8192U;
        IzFadeInitEasy(0U, 1U, 32f);
    }

    private static void gmClearDemoSetRetryInput(GMS_CLRDM_MAIN_WORK main_work)
    {
        gm_clrdm_variant.proc_retry_input(main_work);
    }

    // cur_retry_slct is 0 for retry and 1 for back to the act select
    private static void gmClearDemoSetRetryInputPad(GMS_CLRDM_MAIN_WORK main_work)
    {
        if ((AoPad.AoPadStand() & BUTTON.CANCEL) != 0)
        {
            main_work.flag |= GMD_CLRDM_FLAG_CANCEL;
            GmSoundPlaySE("Cancel");
            return;
        }

        if ((AoPad.AoPadStand() & BUTTON.CONFIRM) != 0)
        {
            main_work.flag |= main_work.cur_retry_slct == 0
                ? GMD_CLRDM_FLAG_SKIP
                : GMD_CLRDM_FLAG_CANCEL;

            GmSoundPlaySE("Ok");
            return;
        }

        if ((AoPad.AoPadMStand() & BUTTON.UP) != 0)
        {
            if (main_work.cur_retry_slct != 0)
                GmSoundPlaySE("Cursol");

            main_work.cur_retry_slct = 0;
            main_work.tex_retry_act.obj_2d.frame = 1.0f;
            main_work.tex_back_slct_act.obj_2d.frame = 0.0f;
        }
        else if ((AoPad.AoPadMStand() & BUTTON.DOWN) != 0)
        {
            if (main_work.cur_retry_slct != 1)
                GmSoundPlaySE("Cursol");

            main_work.cur_retry_slct = 1;
            main_work.tex_retry_act.obj_2d.frame = 0.0f;
            main_work.tex_back_slct_act.obj_2d.frame = 1.0f;
        }
    }

    private static void gmClearDemoSetRetryInputTouch(GMS_CLRDM_MAIN_WORK main_work)
    {
        if (isBackKeyPressed())
        {
            main_work.flag |= 2U;
            setBackKeyRequest(false);
            GmSoundPlaySE("Ok");
        }
        else
        {
            CTrgAoAction trgRetry = main_work.trg_retry;
            trgRetry.Update();
            if (trgRetry.GetState(0U)[10] && trgRetry.GetState(0U)[1])
            {
                main_work.flag |= 4U;
                int index1 = 0;
                for (int index2 = arrayof(main_work.btn_retry); index1 < index2; ++index1)
                {
                    main_work.btn_retry[index1].obj_2d.frame = 2f;
                    main_work.btn_retry[index1].obj_2d.speed = 1f;
                }

                GmSoundPlaySE("Ok");
            }
            else if (trgRetry.GetState(0U)[0])
            {
                int index1 = 0;
                for (int index2 = arrayof(main_work.btn_retry); index1 < index2; ++index1)
                    main_work.btn_retry[index1].obj_2d.frame = 1f;
            }
            else if (trgRetry.GetState(0U)[13])
            {
                int index1 = 0;
                for (int index2 = arrayof(main_work.btn_retry); index1 < index2; ++index1)
                    main_work.btn_retry[index1].obj_2d.frame = 0.0f;
            }

            CTrgAoAction trgBack = main_work.trg_back;
            trgBack.Update();
            if (trgBack.GetState(0U)[10] && trgBack.GetState(0U)[1])
            {
                main_work.flag |= 2U;
                int index1 = 0;
                for (int index2 = arrayof(main_work.btn_back); index1 < index2; ++index1)
                {
                    main_work.btn_back[index1].obj_2d.frame = 2f;
                    main_work.btn_back[index1].obj_2d.speed = 1f;
                }

                GmSoundPlaySE("Ok");
            }
            else if (trgBack.GetState(0U)[2])
            {
                int index1 = 0;
                for (int index2 = arrayof(main_work.btn_back); index1 < index2; ++index1)
                    main_work.btn_back[index1].obj_2d.frame = 1f;
            }
            else
            {
                if (!trgBack.GetState(0U)[13])
                    return;
                int index1 = 0;
                for (int index2 = arrayof(main_work.btn_back); index1 < index2; ++index1)
                    main_work.btn_back[index1].obj_2d.frame = 0.0f;
            }
        }
    }

    private static void gmClearDemoSetRetryDispInfo(GMS_CLRDM_MAIN_WORK main_work)
    {
        g_gm_main_system.ply_work[0].player_flag |= GMD_PLF_TATK_RETRY;
        GMS_PLAYER_WORK gmsPlayerWork = g_gm_main_system.ply_work[0];
        GmSoundStopStageBGM(64);
        GmSoundStopJingle(64);
        GmSoundStopBGMJingle(64);
        GsSoundStopSe();
        GmMapSetDisp(false);
        GmFix.GmFixSetDisp(false);
        GmObjSetAllObjectNoDisp();
        GmRingGetWork().flag |= 1U;
        g_gm_main_system.game_flag |= 8192U;
        g_gm_main_system.water_level = ushort.MaxValue;
        g_gm_main_system.game_flag &= 4294964223U;
        gmsPlayerWork.obj_work.flag &= 4294967167U;
        gmsPlayerWork.obj_work.disp_flag &= 4294967263U;
        OBS_CAMERA obsCamera = ObjCameraGet(g_obj.glb_camera_id);
        ObjCameraSetUserFunc(0, new OBJF_CAMERA_USER_FUNC(GmCameraFunc));
        GmCameraScaleSet(1f, 1f);
        obsCamera.roll = 0;
        gmsPlayerWork.gmk_flag &= 4294934527U;
        gmsPlayerWork.obj_work.pos.x = FXM_FLOAT_TO_FX32(obsCamera.pos.x);
        gmsPlayerWork.obj_work.pos.y = FXM_FLOAT_TO_FX32(obsCamera.pos.y * -1f);
        GmCamScrLimitSetDirect(new GMS_EVE_RECORD_EVENT()
        {
            flag = 15,
            left = -96,
            top = -85,
            width = 192,
            height = 112
        }, FXM_FLOAT_TO_FX32(obsCamera.pos.x), FXM_FLOAT_TO_FX32(obsCamera.pos.y * -1f));
        GmPlySeqChangeTRetryFw(g_gm_main_system.ply_work[0]);
        GmObjSetObjectNoFunc(4U);
        GmObjSetObjectNoFunc(8U);
        GmObjSetObjectNoFunc(16U);
        GmObjSetObjectNoFunc(32U);
        GmObjSetObjectNoFunc(64U);
        GmObjSetObjectNoFunc(128U);
        GmObjSetObjectNoFunc(256U);
        GMM_PAD_VIB_STOP();
        ((OBS_OBJECT_WORK) main_work.tex_retry_act).disp_flag &= 4294967263U;
        ((OBS_OBJECT_WORK) main_work.tex_back_slct_act).disp_flag &= 4294967263U;
        ((OBS_OBJECT_WORK) main_work.bg_retry).disp_flag &= 4294967263U;
        // console leaves DISP_CLEAR_TIME set, and gmClearDemoProcChangeRetryOut falls through to
        // SetTimeAtkSortBufAct so the seven digits come straight back the same frame. that's why
        // normal-stage retry shows the run time and special-stage, which returns before that call,
        // does not.
        if (gm_clrdm_variant.hides_clear_time_on_retry)
        {
            amFlagOff(ref main_work.flag, 1024U);
            main_work.flag &= 4294966271U;
            for (int index = 0; index < arrayof(main_work.record_time_num_act); ++index)
                ((OBS_OBJECT_WORK) main_work.record_time_num_act[index]).disp_flag |= 32U;
        }

        if (gm_clrdm_variant.has_touch_buttons)
        {
            int index1 = 0;
            for (int index2 = arrayof(main_work.btn_retry); index1 < index2; ++index1)
                ((OBS_OBJECT_WORK) main_work.btn_retry[index1]).disp_flag &= 4294967263U;
            int index3 = 0;
            for (int index2 = arrayof(main_work.btn_back); index3 < index2; ++index3)
                ((OBS_OBJECT_WORK) main_work.btn_back[index3]).disp_flag &= 4294967263U;
        }

        if (gm_clrdm_variant.clears_nn_command_state_tbl)
            ObjDrawClearNNCommandStateTbl();
        ObjDrawSetNNCommandStateTbl(0U, 1U, false);
        ObjDrawSetNNCommandStateTbl(1U, 2U, false);
        ObjDrawSetNNCommandStateTbl(2U, 3U, true);
        ObjDrawSetNNCommandStateTbl(3U, 5U, true);
        ObjDrawSetNNCommandStateTbl(4U, 11U, true);
        ObjDrawSetNNCommandStateTbl(5U, 12U, true);
        ObjDrawSetNNCommandStateTbl(6U, 9U, true);
        ObjDrawSetNNCommandStateTbl(7U, 4U, true);
        ObjDrawSetNNCommandStateTbl(8U, 8U, true);
        ObjDrawSetNNCommandStateTbl(9U, 7U, true);
        ObjDrawSetNNCommandStateTbl(10U, 10U, true);
        ObjDrawSetNNCommandStateTbl(11U, 6U, true);
        ObjDrawSetNNCommandStateTbl(12U, 0U, true);
    }

    private static void gmClearDemoProcSpeScoreMoveEfct(GMS_CLRDM_MAIN_WORK main_work)
    {
        if (AoActIsEndTrs(main_work.tex_total_act.obj_2d.act))
        {
            main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcSpeScorePrevCalcScore);
            amFlagOn(ref main_work.flag, 32U);
            amFlagOn(ref main_work.flag, 512U);
        }

        if (AoActIsEndTrs(main_work.tex_time_act.obj_2d.act))
            amFlagOn(ref main_work.flag, 128U);
        if (AoActIsEndTrs(main_work.tex_ring_act.obj_2d.act))
            amFlagOn(ref main_work.flag, 256U);
        gmClearDemoSetNextPromptInput(main_work);
        gmClearDemoSetSortBufAct(main_work);
        gmClearDemoSetScoreData(main_work);
        gmClearDemoUpdateAct(main_work);
    }

    private static void gmClearDemoProcSpeScorePrevCalcScore(GMS_CLRDM_MAIN_WORK main_work)
    {
        main_work.timer += main_work.count;
        if (main_work.timer > 150.0)
        {
            main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcSpeScoreCalcScore);
            main_work.proc_calc_score = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoSetCalcScore);
            main_work.timer = 0.0f;
        }

        if (gm_clrdm_variant.is_skip_pressed())
        {
            setBackKeyRequest(false);
            amFlagOn(ref main_work.flag, 4U);
        }

        gmClearDemoSetNextPromptInput(main_work);
        gmClearDemoSetSortBufAct(main_work);
        gmClearDemoSetScoreData(main_work);
        gmClearDemoUpdateAct(main_work);
    }

    private static void gmClearDemoProcSpeScoreCalcScore(GMS_CLRDM_MAIN_WORK main_work)
    {
        main_work.timer += main_work.count;
        if (((int) main_work.flag & 4) != 0 || ((int) main_work.flag & 8) != 0)
        {
            main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcSpeScoreWaitDispSonic);
            main_work.time_score[1] = main_work.time_score[0];
            main_work.ring_score[1] = main_work.ring_score[0];
            main_work.total_score[1] = main_work.total_score[0];
            main_work.proc_calc_score = null;
            amFlagOff(ref main_work.flag, 4U);
            amFlagOff(ref main_work.flag, 8U);
            if (main_work.total_score[1] >= 10000U)
            {
                amFlagOn(ref main_work.flag, 16U);
                main_work.idle_time = 270;
            }
            else
                main_work.idle_time = 120;

            HgTrophyTryAcquisition(0);
            main_work.timer = 0.0f;
        }

        if (gm_clrdm_variant.is_skip_pressed())
        {
            setBackKeyRequest(false);
            amFlagOn(ref main_work.flag, 4U);
        }

        if (main_work.proc_calc_score != null && main_work.timer > 1.0)
        {
            main_work.proc_calc_score(main_work);
            main_work.timer = 0.0f;
        }

        gmClearDemoSetNextPromptInput(main_work);
        gmClearDemoSetSortBufAct(main_work);
        gmClearDemoSetScoreData(main_work);
        gmClearDemoUpdateAct(main_work);
        if (main_work.proc_calc_score == null)
        {
            GmSoundPlaySE("Result2");
        }
        else
        {
            if (0.0 != main_work.timer)
                return;
            GmSoundPlaySE("Result1");
        }
    }

    private static void gmClearDemoProcSpeScoreWaitDispSonic(GMS_CLRDM_MAIN_WORK main_work)
    {
        if (main_work.timer >= 60.0)
        {
            main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcSpeScoreDispIdle);
            main_work.timer = 0.0f;
            if (((int) main_work.flag & 16) == 0)
                return;
            GmSoundPlayJingle(0U);
            GmPlayerStockGet(g_gm_main_system.ply_work[0], 1);
        }
        else
        {
            gmClearDemoSetNextPromptInput(main_work);
            gmClearDemoSetSortBufAct(main_work);
            gmClearDemoSetScoreData(main_work);
            gmClearDemoUpdateAct(main_work);
            main_work.timer += main_work.count;
        }
    }

    private static void gmClearDemoProcSpeScoreDispIdle(GMS_CLRDM_MAIN_WORK main_work)
    {
        if (main_work.timer >= (double) main_work.idle_time)
        {
            main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcSpeScoreFadeOut);
            IzFadeInitEasy(0U, 3U, 32f);
            gmClearDemoSettleNextPrompt(main_work);
        }

        if (((int) main_work.flag & 16) != 0)
            gmClearDemoSetFlashSonic(main_work);
        gmClearDemoSetNextPromptInput(main_work);
        gmClearDemoSetSortBufAct(main_work);
        gmClearDemoSetScoreData(main_work);
        gmClearDemoUpdateAct(main_work);
        main_work.timer += main_work.count;
    }

    private static void gmClearDemoProcSpeScoreFadeOut(GMS_CLRDM_MAIN_WORK main_work)
    {
        if (IzFadeIsEnd())
        {
            main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcSpeScoreFinish);
            main_work.timer = 0.0f;
        }
        else
        {
            if (((int) main_work.flag & 16) != 0)
                gmClearDemoSetFlashSonic(main_work);
            gmClearDemoSetNextPromptInput(main_work);
            gmClearDemoSetSortBufAct(main_work);
            gmClearDemoSetScoreData(main_work);
            gmClearDemoUpdateAct(main_work);
            main_work.timer += main_work.count;
        }
    }

    private static void gmClearDemoProcSpeScoreFinish(GMS_CLRDM_MAIN_WORK main_work)
    {
        main_work.proc_update = null;
        amFlagOn(ref main_work.flag, 1U);
    }

    private static void gmClearDemoProcSpeTimeMoveEfct(GMS_CLRDM_MAIN_WORK main_work)
    {
        if (AoActIsEndTrs(main_work.tex_big_time_act.obj_2d.act))
        {
            main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcSpeTimeWaitTimeEfct);
            for (int index = 0; index < 7; ++index)
            {
                ObjObjectAction2dAMALoadSetTexlist((OBS_OBJECT_WORK) main_work.record_time_num_act[index], main_work.record_time_num_act[index].obj_2d, null, null, 30, GmGameDatGetCockpitData(), AoTexGetTexList(main_work.tex[0]), (uint) (49 + index), 0);
                ((OBS_OBJECT_WORK) main_work.record_time_num_act[index]).scale.x = 4096;
                ((OBS_OBJECT_WORK) main_work.record_time_num_act[index]).scale.y = 4096;
            }

            gmClearDemoSetClearTimeRecord(main_work);
            gmClearDemoSetSortBufTimeAct(main_work);
            amFlagOn(ref main_work.flag, 1024U);
            amFlagOn(ref main_work.flag, 2048U);
        }

        gmClearDemoSetTimeAtkSortBufAct(main_work);
        gmClearDemoUpdateAct(main_work);
    }

    private static void gmClearDemoProcSpeTimeWaitTimeEfct(GMS_CLRDM_MAIN_WORK main_work)
    {
        if (main_work.timer > 60.0)
        {
            if (((int) main_work.flag & 4096) != 0)
            {
                main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcSpeTimeTimeMoveNewRecord);
                main_work.tex_new_record_act.obj_2d.speed = 1f;
                main_work.idle_time = 180;
            }
            else
            {
                main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcSpeTimeDispEffect);
                main_work.idle_time = 120;
            }

            HgTrophyTryAcquisition(0);
            main_work.timer = 0.0f;
        }

        gmClearDemoSetTimeAtkSortBufAct(main_work);
        gmClearDemoUpdateAct(main_work);
        if (main_work.timer > 120.0)
            return;
        main_work.timer += main_work.count;
    }

    private static void gmClearDemoProcSpeTimeTimeMoveNewRecord(GMS_CLRDM_MAIN_WORK main_work)
    {
        if (AoActIsEndTrs(main_work.tex_new_record_act.obj_2d.act))
        {
            main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcSpeTimeDispEffect);
            GmSoundPlayJingle(5U);
        }

        gmClearDemoSetTimeAtkSortBufAct(main_work);
        gmClearDemoUpdateAct(main_work);
    }

    private static void gmClearDemoProcSpeTimeDispEffect(GMS_CLRDM_MAIN_WORK main_work)
    {
        main_work.timer += main_work.count;
        if ((AoPad.AoPadStand() & BUTTON.CONFIRM) != 0)
            main_work.flag |= 4U;
        if (main_work.timer > (double) main_work.idle_time || ((int) main_work.flag & 4) != 0)
        {
            main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcSpeTimeChangeRetryOut);
            IzFadeInitEasy(0U, 1U, 32f);
            main_work.timer = 0.0f;
            main_work.flag &= 4294967291U;
        }

        gmClearDemoTimeFlushEffect(main_work);
        gmClearDemoSetTimeAtkSortBufAct(main_work);
    }

    private static void gmClearDemoProcSpeTimeChangeRetryOut(GMS_CLRDM_MAIN_WORK main_work)
    {
        if (IzFadeIsEnd())
        {
            main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcSpeTimeChangeRetryIn);
            IzFadeInitEasy(0U, 0U, 32f);
            gmClearDemoSetRetryDispInfo(main_work);
            gmClearDemoSetRetrySortBufAct(main_work);
            main_work.timer = 0.0f;
        }
        else
            gmClearDemoSetTimeAtkSortBufAct(main_work);
    }

    private static void gmClearDemoProcSpeTimeChangeRetryIn(GMS_CLRDM_MAIN_WORK main_work)
    {
        if (!IzFadeIsEnd())
            return;
        main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcSpeTimeWaitSelectRetry);
    }

    private static void gmClearDemoProcSpeTimeWaitSelectRetry(GMS_CLRDM_MAIN_WORK main_work)
    {
        if (gm_clrdm_variant.sets_bg_color_black)
            gmClearDemoSetBgColorBlack();
        gmClearDemoSetRetryInput(main_work);
        if (((int) main_work.flag & 4) != 0)
        {
            main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcWaitRetrySonicRunEfct);
            GmPlySeqChangeTRetryAcc(g_gm_main_system.ply_work[0]);
        }
        else
        {
            if (((int) main_work.flag & 2) == 0)
                return;
            main_work.proc_update = new GMS_CLRDM_MAIN_WORK._WorkDelegate(gmClearDemoProcFadeOut);
            IzFadeInitEasy(0U, 1U, 32f);
        }
    }
}
