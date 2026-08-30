using static AppMain;

using static DmSave.DME_SAVE_ACT;
using static DmSave.DME_SAVE_WIN;

internal static partial class DmSave
{
    private static void dmSaveLoadFontData(DMS_SAVE_MAIN_WORK main_work)
    {
        if (SyGetEvtInfo().cur_evt_id == 10)
            GsFontBuild(false);
        else
            GsFontBuild();

        main_work.proc_menu_update = new DMS_SAVE_MAIN_WORK._proc_(dmSaveIsLoadFontData);
    }

    private static void dmSaveIsLoadFontData(DMS_SAVE_MAIN_WORK main_work)
    {
        if (!GsFontIsBuilded())
            return;

        main_work.proc_menu_update = new DMS_SAVE_MAIN_WORK._proc_(dmSaveLoadRequest);
    }

    private static void dmSaveLoadRequest(DMS_SAVE_MAIN_WORK main_work)
    {
        main_work.arc_cmn_amb_fs[0] = amFsReadBackgroundAs(
            DMD_MANUAL_CMN_DATA_FILENAME, dm_save_variant.fs_variant);

        main_work.arc_cmn_amb_fs[1] = amFsReadBackgroundAs(
            dm_save_menu_cmn_lng_amb_name_tbl[GsEnvGetLanguage()], dm_save_variant.fs_variant);

        main_work.proc_menu_update = new DMS_SAVE_MAIN_WORK._proc_(dmSaveProcLoadWait);
    }

    private static void dmSaveProcLoadWait(DMS_SAVE_MAIN_WORK main_work)
    {
        if (dmSaveIsDataLoad(main_work) == 0)
            return;

        for (int index = 0; index < 2; ++index)
        {
            main_work.arc_cmn_amb[index] = readAMBFile(main_work.arc_cmn_amb_fs[index]);
            main_work.arc_cmn_amb_fs[index] = null;
            main_work.cmn_ama[index] = readAMAFile(amBindGet(main_work.arc_cmn_amb[index], 0));

            string sPath;
            main_work.cmn_amb[index] = readAMBFile(amBindGet(main_work.arc_cmn_amb[index], 1, out sPath));
            main_work.cmn_amb[index].dir = sPath;

            amFsClearRequest(main_work.arc_cmn_amb_fs[index]);
            main_work.arc_cmn_amb_fs[index] = null;

            AoTexBuild(main_work.cmn_tex[index], main_work.cmn_amb[index]);
            AoTexLoad(main_work.cmn_tex[index]);
        }

        if (dm_save_is_snd_build)
            DmSoundBuild();

        main_work.proc_menu_update = new DMS_SAVE_MAIN_WORK._proc_(dmSaveProcTexBuildWait);
    }

    private static void dmSaveProcTexBuildWait(DMS_SAVE_MAIN_WORK main_work)
    {
        if (dmSaveIsTexLoad(main_work) != 1)
            return;

        for (int index = 0; index < 2; ++index)
            dm_save_cmn_tex[index] = main_work.cmn_tex[index];

        main_work.proc_menu_update = new DMS_SAVE_MAIN_WORK._proc_(dmSaveProcCreateAct);
    }

    private static void dmSaveProcCreateAct(DMS_SAVE_MAIN_WORK main_work)
    {
        for (uint index = 0; index < ACT_COUNT; ++index)
        {
            A2S_AMA_HEADER ama;
            AOS_TEXTURE tex;

            if (index >= (uint)ACT_TEX_WINTITLE)
            {
                ama = main_work.cmn_ama[1];
                tex = main_work.cmn_tex[1];
            }
            else
            {
                ama = main_work.cmn_ama[0];
                tex = main_work.cmn_tex[0];
            }

            AoActSetTexture(AoTexGetTexList(tex));
            main_work.act[(int)index] = AoActCreate(ama, dm_save_variant.act_id_tbl[(int)index]);
            dm_save_act[(int)index] = main_work.act[(int)index];
        }

        if ((dm_save_msg_flag & (1 << (int)DME_SAVE_WIN_NOW_SAVING)) != 0)
            main_work.flag |= DMD_SAVE_FLAG_SIGN_OUT_EXIT;

        main_work.proc_menu_update = new DMS_SAVE_MAIN_WORK._proc_(dmSaveProcWindowNodispIdle);
    }

    private static void dmSaveProcWindowNodispIdle(DMS_SAVE_MAIN_WORK main_work)
    {
        if ((main_work.flag & DMD_SAVE_FLAG_DISP_MENU) != 0 || main_work.announce_flag != 0U)
        {
            main_work.win_timer = 0;

            for (uint index = (uint)DME_SAVE_WIN_DATA_LOADING; index < (uint)DME_SAVE_WIN_NUM; ++index)
            {
                if ((main_work.announce_flag & 1U << (int)index) != 0)
                {
                    dm_save_win_mode = main_work.win_mode = (int)index;
                    break;
                }
            }

            if (main_work.win_mode == (int)DME_SAVE_WIN_DATA_LOADING)
            {
                DmCmnBackupLoad();
                main_work.proc_input = null;
                main_work.proc_menu_update = new DMS_SAVE_MAIN_WORK._proc_(dmSaveProcWindowOpenWaitLoadIdle);
            }
            else if (main_work.win_mode == (int)DME_SAVE_WIN_ATTENTION_SAVE)
            {
                main_work.proc_input = new DMS_SAVE_MAIN_WORK._proc_(dmSaveInputProcWindow);
                main_work.proc_menu_update = new DMS_SAVE_MAIN_WORK._proc_(dmSaveProcWindowOpenEfct);
                main_work.proc_draw = new DMS_SAVE_MAIN_WORK._draw_(DmSaveWinSelectDraw);
                dm_save_draw_reserve = true;
                DmSoundPlaySE("Window");
            }
            else if (main_work.win_mode == (int)DME_SAVE_WIN_NOW_SAVING)
            {
                DmCmnBackupSave(dm_save_first_save, false, false);
                main_work.proc_input = null;
                main_work.proc_menu_update = new DMS_SAVE_MAIN_WORK._proc_(dmSaveProcWindowOpenWaitIdle);
                main_work.proc_draw = null;
                dm_save_draw_reserve = false;
            }
            else if (main_work.win_mode == (int)DME_SAVE_WIN_TRIAL_OUT_SAVE)
            {
                DmCmnBackupSave(dm_save_first_save, true, false);
                main_work.proc_input = null;
                main_work.proc_menu_update = new DMS_SAVE_MAIN_WORK._proc_(dmSaveProcWindowOpenWaitIdle);
                main_work.proc_draw = null;
                dm_save_draw_reserve = false;
            }

            main_work.flag &= ~DMD_SAVE_FLAG_DISP_MENU;
        }
        else
        {
            main_work.proc_menu_update = new DMS_SAVE_MAIN_WORK._proc_(dmSaveProcStopDraw);
            dm_save_draw_reserve = false;
        }
    }

    private static void dmSaveProcWindowOpenWaitLoadIdle(DMS_SAVE_MAIN_WORK main_work)
    {
        // TODO(port): console gates this on !DmCmnBackupIsLoadFinished() (dmSave.cpp:1070). that
        // predicate is a stub returning true here, so the arm would skip the window every time
        main_work.proc_menu_update = new DMS_SAVE_MAIN_WORK._proc_(dmSaveProcWindowOpenEfct);
        main_work.proc_draw = new DMS_SAVE_MAIN_WORK._draw_(DmSaveWinSelectDraw);
        DmSoundPlaySE("Window");
        dm_save_draw_reserve = true;
    }

    private static void dmSaveProcWindowOpenWaitIdle(DMS_SAVE_MAIN_WORK main_work)
    {
        // TODO(port): same as the load side, dmSave.cpp:1113
        main_work.proc_menu_update = new DMS_SAVE_MAIN_WORK._proc_(dmSaveProcWindowOpenEfct);
        main_work.proc_draw = new DMS_SAVE_MAIN_WORK._draw_(DmSaveWinSelectDraw);
        DmSoundPlaySE("Window");
        dm_save_draw_reserve = true;
    }

    private static void dmSaveProcWindowOpenEfct(DMS_SAVE_MAIN_WORK main_work)
    {
        if ((main_work.flag & DMD_SAVE_FLAG_WIN_EFCT_END) != 0)
        {
            main_work.proc_menu_update = new DMS_SAVE_MAIN_WORK._proc_(dmSaveProcWindowAnnounceIdle);
            main_work.disp_flag |= DMD_SAVE_DISP_FLAG_WIN_ACT;
            dm_save_disp_flag = main_work.disp_flag;
            main_work.flag &= ~DMD_SAVE_FLAG_WIN_EFCT_END;
        }
        else
            dmSaveSetWinOpenEfct(main_work);
    }

    private static void dmSaveProcWindowAnnounceIdle(DMS_SAVE_MAIN_WORK main_work)
    {
        if (main_work.proc_input != null)
            main_work.proc_input(main_work);

        if (main_work.win_mode == (int)DME_SAVE_WIN_DATA_LOADING)
        {
            if (DmCmnBackupIsLoadFinished())
            {
                main_work.win_timer = DMD_SAVE_WIN_EFCT_TIME;

                if (main_work.timer >= DMD_SAVE_LOAD_MIN_TIME)
                {
                    main_work.disp_flag &= ~DMD_SAVE_DISP_FLAG_WIN_ACT;
                    dm_save_disp_flag = main_work.disp_flag;
                    main_work.proc_menu_update = new DMS_SAVE_MAIN_WORK._proc_(dmSaveProcWindowCloseEfct);
                    main_work.timer = 0;
                }
            }
            else if (AoSysMsgIsShow())
            {
                main_work.win_timer = DMD_SAVE_WIN_EFCT_TIME;
                main_work.disp_flag &= ~DMD_SAVE_DISP_FLAG_WIN_ACT;
                dm_save_disp_flag = main_work.disp_flag;
                main_work.proc_menu_update = new DMS_SAVE_MAIN_WORK._proc_(dmSaveProcWindowCloseEfct);
                main_work.timer = 0;
            }
            else
                main_work.timer = 0;
        }
        else if (main_work.win_mode == (int)DME_SAVE_WIN_ATTENTION_SAVE)
        {
            if ((main_work.flag & DMD_SAVE_FLAG_DECIDE) != 0)
            {
                main_work.proc_input = null;
                main_work.proc_menu_update = new DMS_SAVE_MAIN_WORK._proc_(dmSaveProcWindowCloseEfct);
                main_work.win_timer = DMD_SAVE_WIN_EFCT_TIME;
                main_work.disp_flag &= ~DMD_SAVE_DISP_FLAG_WIN_ACT;
                DmSoundPlaySE("Ok");
                main_work.flag &= ~DMD_SAVE_FLAG_DECIDE;
                main_work.timer = 0;
            }
        }
        else if (main_work.win_mode == (int)DME_SAVE_WIN_NOW_SAVING)
        {
            if (DmCmnBackupIsSaveFinished())
            {
                main_work.win_timer = DMD_SAVE_WIN_EFCT_TIME;

                if (main_work.timer >= DMD_SAVE_SAVE_MIN_TIME)
                {
                    main_work.proc_input = null;
                    main_work.disp_flag &= ~DMD_SAVE_DISP_FLAG_WIN_ACT;
                    main_work.proc_menu_update = new DMS_SAVE_MAIN_WORK._proc_(dmSaveProcWindowCloseEfct);
                    main_work.timer = 0;
                }
            }
            else
                main_work.timer = 0;
        }
        else if (main_work.win_mode == (int)DME_SAVE_WIN_TRIAL_OUT_SAVE)
        {
            if (DmCmnBackupIsSaveFinished())
            {
                main_work.win_timer = DMD_SAVE_WIN_EFCT_TIME;

                if (main_work.timer >= DMD_SAVE_SAVE_MIN_TIME)
                {
                    main_work.proc_input = null;
                    main_work.disp_flag &= ~DMD_SAVE_DISP_FLAG_WIN_ACT;
                    main_work.proc_menu_update = new DMS_SAVE_MAIN_WORK._proc_(dmSaveProcWindowCloseEfct);
                    main_work.timer = 0;
                }
            }
            else
                main_work.timer = 0;
        }
        else
        {
            main_work.proc_menu_update = new DMS_SAVE_MAIN_WORK._proc_(dmSaveProcWindowCloseEfct);
            main_work.disp_flag &= ~DMD_SAVE_DISP_FLAG_WIN_ACT;
        }

        dm_save_disp_flag = main_work.disp_flag;
        ++main_work.timer;
    }

    private static void dmSaveProcWindowCloseEfct(DMS_SAVE_MAIN_WORK main_work)
    {
        if ((main_work.flag & DMD_SAVE_FLAG_WIN_EFCT_END) != 0)
        {
            main_work.proc_menu_update = main_work.win_mode != (int)DME_SAVE_WIN_ATTENTION_SAVE
                ? (main_work.win_mode != (int)DME_SAVE_WIN_DATA_LOADING
                    ? new DMS_SAVE_MAIN_WORK._proc_(dmSaveProcWindowNodispIdle)
                    : new DMS_SAVE_MAIN_WORK._proc_(dmSaveProcWaitLoadEnd))
                : new DMS_SAVE_MAIN_WORK._proc_(dmSaveProcWaitSeStop);

            main_work.announce_flag &= (uint)~(1 << main_work.win_mode);
            main_work.flag &= ~DMD_SAVE_FLAG_WIN_EFCT_END;
            main_work.proc_draw = null;
            dm_save_draw_reserve = false;
        }

        dmSaveSetWinCloseEfct(main_work);
    }

    private static void dmSaveProcWaitSeStop(DMS_SAVE_MAIN_WORK main_work)
    {
        if (main_work.timer > DMD_SAVE_SE_END_WAIT_TIME)
        {
            main_work.proc_menu_update = new DMS_SAVE_MAIN_WORK._proc_(dmSaveProcWindowNodispIdle);
            main_work.timer = 0;
        }
        else
            ++main_work.timer;
    }

    private static void dmSaveProcWaitLoadEnd(DMS_SAVE_MAIN_WORK main_work)
    {
        if (!DmCmnBackupIsLoadFinished())
            return;

        main_work.proc_menu_update = new DMS_SAVE_MAIN_WORK._proc_(dmSaveProcWindowNodispIdle);
        main_work.timer = 0;
    }

    private static void dmSaveProcStopDraw(DMS_SAVE_MAIN_WORK main_work)
    {
        main_work.proc_menu_update = new DMS_SAVE_MAIN_WORK._proc_(dmSaveProcDataRelease);
    }

    private static void dmSaveProcDataRelease(DMS_SAVE_MAIN_WORK main_work)
    {
        for (int index = 0; index < 2; ++index)
            AoTexRelease(main_work.cmn_tex[index]);

        if (dm_save_is_snd_build)
        {
            DmSoundExit();
            DmSoundFlush();
        }

        main_work.proc_menu_update = new DMS_SAVE_MAIN_WORK._proc_(dmSaveProcFinish);
    }

    private static void dmSaveProcFinish(DMS_SAVE_MAIN_WORK main_work)
    {
        if (dmSaveIsTexRelease(main_work) != 1)
            return;

        for (int index = 0; index < 2; ++index)
            dm_save_cmn_tex[index] = null;

        for (int index = 0; index < ACT_COUNT; ++index)
        {
            if (main_work.act[index] != null)
            {
                AoActDelete(main_work.act[index]);
                main_work.act[index] = null;
            }

            dm_save_act[index] = null;
        }

        for (int index = 0; index < 2; ++index)
        {
            if (main_work.arc_cmn_amb[index] != null)
                main_work.arc_cmn_amb[index] = null;
        }

        main_work.flag |= DMD_SAVE_FLAG_EXIT;
        main_work.proc_menu_update = null;
    }

    private static void dmSaveInputProcWindow(DMS_SAVE_MAIN_WORK main_work)
    {
        if (AoAccountGetCurrentId() < 0)
        {
            if (AoPad.AoPadSomeoneStand(ControllerConsts.CONFIRM) < 0)
                return;

            main_work.flag |= DMD_SAVE_FLAG_DECIDE;
        }
        else
        {
            if ((AoPad.AoPadStand() & ControllerConsts.CONFIRM) == 0)
                return;

            main_work.flag |= DMD_SAVE_FLAG_DECIDE;
        }
    }

    private static int dmSaveIsDataLoad(DMS_SAVE_MAIN_WORK main_work)
    {
        for (int index = 0; index < 2; ++index)
        {
            if (!amFsIsComplete(main_work.arc_cmn_amb_fs[index]))
                return 0;
        }

        return 1;
    }

    private static int dmSaveIsTexLoad(DMS_SAVE_MAIN_WORK main_work)
    {
        for (int index = 0; index < 2; ++index)
        {
            if (!AoTexIsLoaded(main_work.cmn_tex[index]))
                return 0;
        }

        return dm_save_is_snd_build && !DmSoundBuildCheck() ? 0 : 1;
    }

    private static int dmSaveIsTexRelease(DMS_SAVE_MAIN_WORK main_work)
    {
        for (int index = 0; index < 2; ++index)
        {
            if (!AoTexIsReleased(main_work.cmn_tex[index]))
                return 0;
        }

        return 1;
    }

    private static void dmSaveSetWinOpenEfct(DMS_SAVE_MAIN_WORK main_work)
    {
        if (main_work.win_timer > DMD_SAVE_WIN_EFCT_TIME)
        {
            main_work.flag |= DMD_SAVE_FLAG_WIN_EFCT_END;
            main_work.win_timer = 0;

            for (uint index = 0; index < 2U; ++index)
                main_work.win_size_rate[(int)index] = DMD_SAVE_WIN_DEF_RATE;
        }
        else
            ++main_work.win_timer;

        for (uint index = 0; index < 2U; ++index)
        {
            main_work.win_size_rate[(int)index] = main_work.win_timer == 0 ? 1f : main_work.win_timer / 8f;

            if (main_work.win_size_rate[(int)index] > 1.0)
                main_work.win_size_rate[(int)index] = 1f;

            dm_save_win_size_rate[(int)index] = main_work.win_size_rate[(int)index];
        }
    }

    private static void dmSaveSetWinCloseEfct(DMS_SAVE_MAIN_WORK main_work)
    {
        for (uint index = 0; index < 2U; ++index)
        {
            main_work.win_size_rate[(int)index] = main_work.win_timer == 0 ? 0.0f : main_work.win_timer / 8f;
            dm_save_win_size_rate[(int)index] = main_work.win_size_rate[(int)index];
        }

        if (main_work.win_timer < 0)
        {
            main_work.flag |= DMD_SAVE_FLAG_WIN_EFCT_END;
            main_work.win_timer = 0;

            for (uint index = 0; index < 2U; ++index)
            {
                main_work.win_size_rate[(int)index] = 0.0f;
                dm_save_win_size_rate[(int)index] = main_work.win_size_rate[(int)index];
            }
        }
        else
            --main_work.win_timer;
    }
}
