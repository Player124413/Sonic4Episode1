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
    private static void dmTitleProcFadeIn(DMS_TITLE_MAIN_WORK main_work)
    {
        if (!IzFadeIsEnd())
            return;
        IzFadeExit();
        AoPresenceSet(AO_PRESENCE_STAGE_ID_TITLE, false);
        if (dm_title_is_title_start)
        {
            main_work.proc_update = (dmTitleProcWaitInput);
            main_work.proc_input = (dmTitleInputProcTitle);
            DmSndBgmPlayerPlayBgm(1);
        }
        else
        {
            main_work.proc_update = (dmTitleProcMainMenuIdle);
            main_work.proc_input = (dmTitleInputProcMainMenu);
            main_work.flag |= 2147483648U;
            DmSndBgmPlayerPlayBgm(0);
        }

        main_work.proc_win_update = (dmTitleProcWindowNodispIdle);
    }

    private static void dmTitleProcWaitInput(DMS_TITLE_MAIN_WORK main_work)
    {
        ++main_work.timer;
        if (main_work.timer > 3600.0)
        {
            main_work.proc_update = (dmTitleProcFadeOut);
            main_work.proc_input = null;
            main_work.timer = 0.0f;
            main_work.flag |= 1048576U;
            IzFadeInitEasy(0U, 1U, 32f);
            DmSndBgmPlayerExit();
            main_work.flag |= 524288U;
        }
        else
        {
            if (main_work.proc_input != null)
                main_work.proc_input(main_work);
            if (((int)main_work.flag & 32) == 0)
                return;
            main_work.proc_update = (dmTitleProcDecideEfct);
            main_work.proc_input = null;
            main_work.timer = 0.0f;
            main_work.flag &= 4294967263U;
            DmSoundPlaySE("Ok");
            DmSndBgmPlayerBgmStop();
        }
    }

    private static void dmTitleProcDecideEfct(DMS_TITLE_MAIN_WORK main_work)
    {
        if (main_work.timer >= 60.0 && AoAccountSetCurrentIdIsFinished())
        {
            if (AoAccountGetCurrentId() >= 0)
            {
                main_work.flag &= 4294966783U;
                main_work.proc_update = (dmTitleProcCheckTrialIdle);
                GsRebootSetTitle();
            }
            else
            {
                main_work.proc_update = (dmTitleProcWaitInput);
                main_work.proc_input = (dmTitleInputProcTitle);
                DmSndBgmPlayerPlayBgm(1);
            }

            main_work.timer = 0.0f;
        }
        else
            ++main_work.timer;
    }

    private static void dmTitleProcCheckTrialIdle(DMS_TITLE_MAIN_WORK main_work)
    {
        if (!GsTrialCheckIsFinished())
            return;
        if (GsTrialIsTrial())
        {
            main_work.proc_update = (dmTitleProcMainMenuOpenWin);
            main_work.proc_input = null;
            main_work.flag |= 2147483648U;
            main_work.flag |= 4096U;
            main_work.flag |= 2097152U;
            main_work.is_init_play = false;
            main_work.slct_menu_num = 3;
            main_work.cur_slct_menu = 0;
            main_work.cur_crsr_pos_y = dm_title_variant.crsr_trial_pos_y_tbl[main_work.cur_slct_menu];
            DmSndBgmPlayerPlayBgm(0);
        }
        else
        {
            DmSave.DmSaveStart(1U, false, false);
            main_work.proc_update = (dmTitleProcFileSlctWaitDataLoad);
            DmSndBgmPlayerPlayBgm(0);
        }
    }

    private static void dmTitleProcFileSlctWaitDataLoad(DMS_TITLE_MAIN_WORK main_work)
    {
        if (!DmSave.DmSaveIsExit())
            return;
        if (DmCmnBackupIsLoadSuccessed())
        {
            main_work.proc_update = (dmTitleProcMainMenuOpenWin);
            main_work.proc_input = null;
            main_work.flag |= 2147483648U;
            main_work.flag |= 4096U;
            main_work.flag |= 2097152U;
            if (GsMainSysIsStageClear(0))
            {
                main_work.is_init_play = false;
                main_work.slct_menu_num = 4;
                main_work.cur_slct_menu = 1;
            }
            else
            {
                main_work.is_init_play = true;
                main_work.slct_menu_num = 3;
                main_work.cur_slct_menu = 0;
            }

            main_work.flag |= 2097152U;
            main_work.cur_crsr_pos_y = !GsTrialIsTrial()
                ? main_work.cur_slct_menu * dm_title_crsr_pos_y_tbl[1] + dm_title_crsr_pos_y_tbl[0]
                : dm_title_variant.crsr_trial_pos_y_tbl[main_work.cur_slct_menu];
            dmTitleSetLoadSysData(main_work);
        }
        else
        {
            int error = XmlStorage.GetLastError();
            main_work.is_init_play = true;
            main_work.slct_menu_num = 3;
            if (error == 1)
            {
                main_work.proc_update = (dmTitleProcFileSlctSaveStartWait);
            }
            else
            {
                main_work.proc_update = (dmTitleProcMainMenuOpenWin);
                main_work.proc_input = null;
                main_work.flag |= 2147483648U;
                main_work.flag |= 4096U;
                main_work.flag |= 2097152U;
                dmTitleSetInitSaveData(main_work);
                dmTitleSetInitSysData(main_work);
            }
        }
    }

    private static void dmTitleProcFileSlctSaveStartWait(DMS_TITLE_MAIN_WORK main_work)
    {
        if (!DmSave.DmSaveIsExit())
            return;
        DmSave.DmSaveStart(4U, true, false);
        main_work.proc_update = (dmTitleProcFileSlctWaitDataSave);
    }

    private static void dmTitleProcFileSlctWaitDataSave(DMS_TITLE_MAIN_WORK main_work)
    {
        if (!DmSave.DmSaveIsExit())
            return;
        if (!DmCmnBackupIsSaveSuccessed())
        {
            main_work.proc_update = (dmTitleProcWaitInput);
            main_work.proc_input = (dmTitleInputProcTitle);
            main_work.flag |= 512U;
            main_work.flag &= 4294966271U;
            main_work.flag &= int.MaxValue;
            main_work.timer = 0.0f;
            DmSndBgmPlayerPlayBgm(1);
        }
        else
        {
            SSave.CreateInstance().Init();
            main_work.is_init_play = true;
            dmTitleSetInitSysData(main_work);
            main_work.proc_update = (dmTitleProcMainMenuOpenWin);
            main_work.proc_input = null;
            main_work.flag |= 2147483648U;
            main_work.flag |= 2097152U;
        }
    }

    private static void dmTitleProcMainMenuOpenWin(DMS_TITLE_MAIN_WORK main_work)
    {
        if (((int)main_work.flag & 4194304) != 0)
        {
            main_work.proc_update = (dmTitleProcMainMenuIdle);
            main_work.proc_input = (dmTitleInputProcMainMenu);
            main_work.mmenu_win_timer = 0.0f;
            main_work.flag |= 1024U;
            main_work.flag &= 4294966783U;
            main_work.flag &= 4290772991U;
            DmTitleOp.DmTitleOpDispRightEnable(false);
            main_work.cur_slct_menu = GsTrialIsTrial() || main_work.is_init_play ? 0 : 1;
            main_work.cur_crsr_pos_y = !GsTrialIsTrial()
                ? main_work.cur_slct_menu * dm_title_crsr_pos_y_tbl[1] + dm_title_crsr_pos_y_tbl[0]
                : dm_title_variant.crsr_trial_pos_y_tbl[main_work.cur_slct_menu];
            SaveState.showResumeWarning();
        }
        else
            dmTitleSetMMenuWinOpenEfct(main_work);
    }

    private static void dmTitleProcMainMenuCloseWin(DMS_TITLE_MAIN_WORK main_work)
    {
        if (((int)main_work.flag & 4194304) != 0)
        {
            AoAccountClearCurrentId();
            SSave.CreateInstance().Init();
            main_work.flag &= int.MaxValue;
            main_work.proc_update = (dmTitleProcWaitInput);
            main_work.proc_input = (dmTitleInputProcTitle);
            main_work.flag |= 512U;
            main_work.flag &= 4294966271U;
            main_work.flag &= 4292870143U;
            DmTitleOp.DmTitleOpDispRightEnable(true);
            DmSndBgmPlayerPlayBgm(1);
            main_work.mmenu_win_timer = 0.0f;
            main_work.flag &= 4292870143U;
            main_work.flag &= 4290772991U;
        }
        else
            dmTitleSetMMenuWinCloseEfct(main_work);
    }

    private static void dmTitleProcMainMenuIdle(DMS_TITLE_MAIN_WORK main_work)
    {
        if (main_work.proc_input != null)
            main_work.proc_input(main_work);
        if (SaveState.shouldResume())
        {
            main_work.cur_slct_menu = 0;
            main_work.flag |= 4U;
        }

        if (((int)main_work.flag & 2) != 0)
        {
            main_work.proc_update = (dmTitleProcMainMenuCloseWin);
            main_work.proc_input = null;
            main_work.mmenu_win_timer = 8f;
            main_work.flag &= 4294966271U;
            main_work.flag &= 4294967293U;
            main_work.flag &= 4294967291U;
            DmSoundPlaySE("Cancel");
        }
        else if (((int)main_work.flag & 4) != 0)
        {
            main_work.proc_update = (dmTitleProcMainMenuDecideEfct);
            main_work.proc_input = null;
            main_work.timer = 0.0f;
            main_work.flag &= 4294967293U;
            main_work.flag &= 4294967291U;
            DmSoundPlaySE("Ok");
        }
        else if (((int)main_work.flag & 2048) != 0)
        {
            main_work.proc_update = (dmTitleProcMainMenuCompBuyIdle);
            main_work.proc_input = null;
            main_work.timer = 0.0f;
            main_work.flag |= 262144U;
            //Upsell.launchUpsellScreen(main_work.buy_scr_work);
            main_work.flag &= 4294965247U;
        }
        else
        {
            if (((int)main_work.flag & 64) != 0 || ((int)main_work.flag & 128) != 0)
            {
                dmTitleSetChngFocusCrsrData(main_work);
                DmSoundPlaySE("Cursol");
                main_work.flag &= 4294967231U;
                main_work.flag &= 4294967167U;
            }

            if (((int)main_work.flag & 256) == 0)
                return;
            dmTitleSetCtrlFocusChangeEfct(main_work);
            if (!dmTitleIsCtrlFocusChangeEfctEnd(main_work))
                return;
            main_work.flag &= 4294967039U;
        }
    }

    private static void dmTitleProcMainMenuCompBuyFadeOut(DMS_TITLE_MAIN_WORK main_work)
    {
        if (IzFadeIsEnd())
        {
            main_work.proc_update = (dmTitleProcMainMenuCompBuyIdle);
            //Upsell.launchUpsellScreen(main_work.buy_scr_work);
            main_work.flag &= int.MaxValue;
            main_work.disp_flag &= 4294967293U;
        }

        if (((int)main_work.flag & 256) == 0)
            return;
        dmTitleSetCtrlFocusChangeEfct(main_work);
        if (!dmTitleIsCtrlFocusChangeEfctEnd(main_work))
            return;
        main_work.flag &= 4294967039U;
    }

    private static void dmTitleProcMainMenuCompBuyIdle(DMS_TITLE_MAIN_WORK main_work)
    {
        //if (Upsell.showUpsell)
        //    return;
        if (DmBuyScreenGetResult(main_work.buy_scr_work) == 0)
        {
            if (!GsTrialIsTrial())
            {
                main_work.is_init_play = true;
                main_work.is_no_save_data = true;
                event_after_buy = true;
            }

            if (((int)main_work.flag & 262144) != 0)
                main_work.flag &= 4294705151U;
            else
                IzFadeInitEasy(1U, 0U, 32f);
            DmSndBgmPlayerPlayBgm(0);
            main_work.proc_update = (dmTitleProcMainMenuCompBuyFadeIn);
            main_work.disp_flag |= 2U;
        }
        else
        {
            if (((int)main_work.flag & 262144) != 0)
                main_work.flag &= 4294705151U;
            else
                IzFadeInitEasy(1U, 0U, 32f);
            DmSndBgmPlayerPlayBgm(0);
            main_work.proc_update = (dmTitleProcMainMenuCompBuyFadeIn);
            main_work.disp_flag |= 2U;
        }
    }

    public static void dmTitleProcMainMenuComUpsellScreenFinished()
    {
        IzFadeInitEasy(1U, 0U, 32f);
        DmSndBgmPlayerPlayBgm(0);
    }

    private static void dmTitleProcMainMenuCompBuyFadeIn(DMS_TITLE_MAIN_WORK main_work)
    {
        if (!IzFadeIsEnd())
            return;
        if (DmBuyScreenGetResult(main_work.buy_scr_work) == 0)
        {
            main_work.proc_update = (dmTitleProcMainMenuIdle);
            main_work.proc_input = (dmTitleInputProcMainMenu);
            main_work.flag |= 2147483648U;
            if (!GsTrialIsTrial())
            {
                main_work.is_init_play = true;
                main_work.is_no_save_data = true;
                event_after_buy = true;
            }
        }
        else
        {
            main_work.proc_update = (dmTitleProcMainMenuIdle);
            main_work.proc_input = (dmTitleInputProcMainMenu);
            main_work.flag |= 2147483648U;
        }

        for (int index = 0; index < 5; ++index)
            main_work.decide_menu_frm[index] = 0.0f;
    }

    private static void dmTitleProcMainMenuTrophyIdle(DMS_TITLE_MAIN_WORK main_work)
    {
    }

    // dmTitle.cpp:3202-3290, the _XBOX arm of the decide effect. LIBRARY and BUY never reach
    // dmTitleSetNextEvent; they are resolved here. Returns true when it has taken the decision.
    private static bool dmTitleDecideConsoleExtraSlot(DMS_TITLE_MAIN_WORK main_work)
    {
        var action = dmTitleMenuAction(main_work);

        if (action == DME_TITLE_MENU_ACTION.BUY)
        {
            main_work.proc_update = (dmTitleProcMainMenuCompBuyIdle);
            main_work.proc_input = null;
            main_work.timer = 0.0f;
            main_work.flag |= DMD_TITLE_FLAG_COMP_NO_DISP_END;

            // TODO(port): dmTitle.cpp:3279 launches the upsell here. left commented to match the
            // two existing buy entry points, which stub it the same way
            //Upsell.launchUpsellScreen(main_work.buy_scr_work);

            main_work.decide_menu_frm[(int)DME_TITLE_MENU_TYPE.DME_TITLE_MENU_TYPE_EXTRA2] = 0.0f;
            main_work.flag &= ~DMD_TITLE_FLAG_BUY_COMP;
            return true;
        }

        if (action != DME_TITLE_MENU_ACTION.LIBRARY)
            return false;

        if (GsTrialIsTrial())
        {
            main_work.proc_update = (dmTitleProcMainMenuCompBuyFadeOut);
            main_work.proc_input = null;
            main_work.timer = 0.0f;

            IzFadeInitEasy(0U, 1U, DMD_TITLE_FADEOUT_TIME);
            DmSndBgmPlayerBgmStop();

            main_work.flag &= ~DMD_TITLE_FLAG_BUY_COMP;
            return true;
        }

        // save invalid and something worth losing, so ask before quitting
        if (!dmTitleIsSaveRunning() && (!main_work.is_init_play || dmTitleIsChangeOptVol()))
        {
            main_work.proc_update = (dmTitleProcNoSaveCheckDelEndGame);
            main_work.announce_flag |= 1U << (int)DME_TITLE_WIN_TYPE.DME_TITLE_WIN_TYPE_DEL_DATA5;
            return true;
        }

        dmTitleStartGameExit(main_work);
        return true;
    }

    private static void dmTitleStartGameExit(DMS_TITLE_MAIN_WORK main_work)
    {
        main_work.proc_update = (dmTitleProcGameExitIdle);
        main_work.timer = 0.0f;

        DmSndBgmPlayerBgmStop();
        IzFadeInitEasy(0U, 1U, DMD_TITLE_FADEOUT_TIME);
    }

    private static void dmTitleProcGameExitIdle(DMS_TITLE_MAIN_WORK main_work)
    {
        ++main_work.timer;

        if (main_work.timer > DMD_TITLE_END_GAME_WAIT_TIME && IzFadeIsEnd())
        {
            main_work.timer = 0.0f;
            main_work.proc_update = null;

            GsReqExit();
        }
    }

    private static void dmTitleProcNoSaveCheckDelEndGame(DMS_TITLE_MAIN_WORK main_work)
    {
        if ((main_work.flag & DMD_TITLE_FLAG_DEL_DATA_BACK_MMENU) != 0)
        {
            main_work.proc_update = (dmTitleProcMainMenuIdle);
            main_work.proc_input = (dmTitleInputProcMainMenu);
            main_work.timer = 0.0f;
            main_work.flag &= ~DMD_TITLE_FLAG_DEL_DATA_BACK_MMENU;

            for (int index = 0; index < (int)DME_TITLE_MENU_TYPE.DME_TITLE_MENU_TYPE_NUM; ++index)
                main_work.decide_menu_frm[index] = 0.0f;
        }
        else if ((main_work.flag & DMD_TITLE_FLAG_DEL_DATA_GAME_END) != 0)
        {
            main_work.flag &= ~DMD_TITLE_FLAG_DEL_DATA_GAME_END;
            dmTitleStartGameExit(main_work);
        }

        if ((main_work.flag & DMD_TITLE_FLAG_MENU_CRSR_EFCT) != 0)
        {
            dmTitleSetCtrlFocusChangeEfct(main_work);

            if (dmTitleIsCtrlFocusChangeEfctEnd(main_work))
                main_work.flag &= ~DMD_TITLE_FLAG_MENU_CRSR_EFCT;
        }
    }

    private static void dmTitleProcMainMenuDecideEfct(DMS_TITLE_MAIN_WORK main_work)
    {
        if (((int)main_work.flag & 256) != 0)
        {
            dmTitleSetCtrlFocusChangeEfct(main_work);
            if (dmTitleIsCtrlFocusChangeEfctEnd(main_work))
                main_work.flag &= 4294967039U;
        }

        int curSlctMenu = 0;

        // 体験版・初期データ・データありで項目を切り替える
        curSlctMenu = dmTitleCrsrToMenuSlot(main_work);

        if (main_work.timer > 15.0 && dmTitleDecideConsoleExtraSlot(main_work))
            return;

        if (main_work.timer > 15.0)
        {
            if (curSlctMenu == 4 && (GsTrialIsTrial() || XBOXLive.signinStatus == XBOXLive.SigninStatus.UpdateNeeded ||
                                     XBOXLive.signinStatus == XBOXLive.SigninStatus.Local))
            {
                for (int index = 0; index < 5; ++index)
                    main_work.decide_menu_frm[index] = 0.0f;
                main_work.flag &= 4294967291U;
                main_work.proc_update = (dmTitleProcMainMenuIdle);
                main_work.proc_input = (dmTitleInputProcMainMenu);
                if (XBOXLive.signinStatus == XBOXLive.SigninStatus.UpdateNeeded)
                {
                    XBOXLive.displayTitleUpdateMessage = true;
                }
                else
                {
                    List<string> stringList = new List<string>();
                    stringList.Add(Sonic4ep1.Strings.ID_OK);
                    string text;

                    if (!GsTrialIsTrial())
                    {
                        text = (XBOXLive.signinStatus != XBOXLive.SigninStatus.Local
                            ? Sonic4ep1.Strings.ID_LB_UPDATE
                            : Sonic4ep1.Strings.ID_LB_OFFLINE);
                    }
                    else
                    {
                        text = Sonic4ep1.Strings.ID_LB_BUY;
                    }

                    AppMain.g_ao_sys_global.is_show_ui = true;
                    Guide.BeginShowMessageBox(" ", text, (IEnumerable<string>)stringList, 0, MessageBoxIcon.Alert, new AsyncCallback(TitleMBResult), (object)null);
                }
            }
            else
            {
                if (curSlctMenu == 0 && SaveState.shouldResume())
                {
                    main_work.proc_update = (dmTitleProcFadeOut);
                }
                else
                {
                    if (curSlctMenu == 0 && !main_work.is_init_play && !GsTrialIsTrial())
                    {
                        if (dmTitleIsSaveRunning())
                        {
                            main_work.proc_update = (dmTitleProcMainMenuDelSaveWin);
                            main_work.announce_flag |= 1U << (int)DME_TITLE_WIN_TYPE.DME_TITLE_WIN_TYPE_DEL_DATA1;
                            if (main_work.iphone_menus)
                                main_work.flag &= 4292869119U;
                            return;
                        }

                        if (main_work.iphone_menus)
                        {
                            main_work.proc_update = (dmTitleProcFadeOut);
                        }
                        else
                        {
                            main_work.proc_update = (dmTitleProcNoSaveCheckInitStart);
                            main_work.announce_flag |= 1U << (int)DME_TITLE_WIN_TYPE.DME_TITLE_WIN_TYPE_DEL_DATA4;
                        }
                        return;
                    }

                    if (GsTrialIsTrial() && curSlctMenu == 1)
                    {
                        main_work.proc_update = (dmTitleProcMainMenuCompBuyFadeOut);
                        main_work.proc_input = null;
                        main_work.timer = 0.0f;
                        IzFadeInitEasy(0U, 1U, 32f);
                        DmSndBgmPlayerBgmStop();
                        return;
                    }

                    main_work.proc_update = (dmTitleProcFadeOut);
                }

                main_work.timer = 0.0f;
                for (int index = 0; index < 5; ++index)
                    main_work.decide_menu_frm[index] = 0.0f;
                IzFadeInitEasy(0U, 1U, 32f);
                if (curSlctMenu != 0)
                    return;
                DmSndBgmPlayerExit();
                main_work.flag |= 524288U;
            }
        }
        else
        {
            ++main_work.decide_menu_frm[curSlctMenu];
            ++main_work.timer;
        }
    }

    private static void TitleMBResult(IAsyncResult userResult)
    {
        g_ao_sys_global.is_show_ui = false;
    }

    private static void dmTitleProcMainMenuDelSaveWin(DMS_TITLE_MAIN_WORK main_work)
    {
        if (((int)main_work.flag & 8192) != 0)
        {
            main_work.proc_update = (dmTitleProcMainMenuIdle);
            main_work.proc_input = (dmTitleInputProcMainMenu);
            main_work.timer = 0.0f;
            main_work.flag &= 4294959103U;
            for (int index = 0; index < 5; ++index)
                main_work.decide_menu_frm[index] = 0.0f;
            main_work.flag |= 2098176U;
        }
        else if (((int)main_work.flag & 16384) != 0)
        {
            SOption instance = SOption.CreateInstance();
            SOption.EControl.Type control = instance.GetControl();
            dmTitleSetInitSaveData(main_work);
            instance.SetControl(control);
            main_work.proc_update = (dmTitleProcSaveInitData);
            DmSave.DmSaveMenuStart(false, false);
            main_work.flag &= 4294950911U;
        }

        if (((int)main_work.flag & 256) == 0)
            return;
        dmTitleSetCtrlFocusChangeEfct(main_work);
        if (!dmTitleIsCtrlFocusChangeEfctEnd(main_work))
            return;
        main_work.flag &= 4294967039U;
    }

    private static void dmTitleProcNoSaveCheckInitStart(DMS_TITLE_MAIN_WORK main_work)
    {
        if ((main_work.flag & DMD_TITLE_FLAG_DEL_DATA_BACK_MMENU) != 0)
        {
            main_work.proc_update = (dmTitleProcMainMenuIdle);
            main_work.proc_input = (dmTitleInputProcMainMenu);
            main_work.timer = 0.0f;
            main_work.flag &= ~DMD_TITLE_FLAG_DEL_DATA_BACK_MMENU;
            for (int index = 0; index < (int)DME_TITLE_MENU_TYPE.DME_TITLE_MENU_TYPE_NUM; ++index)
                main_work.decide_menu_frm[index] = 0.0f;
        }
        else if ((main_work.flag & DMD_TITLE_FLAG_DEL_DATA_INIT_START) != 0)
        {
            dmTitleSetInitSaveData(main_work);
            main_work.proc_update = (dmTitleProcSaveInitData);
            main_work.flag &= ~DMD_TITLE_FLAG_DEL_SAVE_DATA;
        }

        if ((main_work.flag & DMD_TITLE_FLAG_MENU_CRSR_EFCT) == 0)
            return;
        dmTitleSetCtrlFocusChangeEfct(main_work);
        if (!dmTitleIsCtrlFocusChangeEfctEnd(main_work))
            return;
        main_work.flag &= ~DMD_TITLE_FLAG_MENU_CRSR_EFCT;
    }

    private static void dmTitleProcSaveInitData(DMS_TITLE_MAIN_WORK main_work)
    {
        if (!DmSave.DmSaveIsExit())
            return;
        if (!GsTrialIsTrial() && !DmCmnBackupIsSaveSuccessed())
        {
            main_work.proc_update = (dmTitleProcWaitInput);
            main_work.proc_input = (dmTitleInputProcTitle);
            main_work.flag |= 512U;
            main_work.flag &= 4294966271U;
            main_work.flag &= int.MaxValue;
            DmSndBgmPlayerPlayBgm(1);
        }
        else
        {
            main_work.proc_update = (dmTitleProcFadeOut);
            main_work.timer = 0.0f;
            IzFadeInitEasy(0U, 1U, 32f);
            DmSndBgmPlayerExit();
            main_work.flag |= 524288U;
        }
    }

    private static void dmTitleProcFadeOut(DMS_TITLE_MAIN_WORK main_work)
    {
        if (!IzFadeIsEnd())
            return;
        DmTitleOp.DmTitleOpExit();
        main_work.proc_update = (dmTitleProcDataRelease);
        main_work.proc_draw = null;
    }

    private static void dmTitleProcWindowNodispIdle(DMS_TITLE_MAIN_WORK main_work)
    {
        if (main_work.proc_win_input != null)
            main_work.proc_win_input(main_work);
        if (main_work.announce_flag == 0U)
            return;
        main_work.proc_win_update = (dmTitleProcWindowOpenEfct);
        main_work.proc_input = null;
        main_work.proc_win_input = null;
        main_work.win_timer = 0.0f;
        for (int index = 0; index < 5; ++index)
        {
            if ((main_work.announce_flag & (uint)(1 << index)) != 0)
            {
                main_work.win_mode = index;
                break;
            }
        }

        main_work.win_cur_slct = 0;
        DmSoundPlaySE("Window");
        main_work.flag |= 131072U;
    }

    private static void dmTitleProcWindowOpenEfct(DMS_TITLE_MAIN_WORK main_work)
    {
        if (((int)main_work.flag & 65536) != 0)
        {
            main_work.proc_win_update = (dmTitleProcWindowAnnounceIdle);
            main_work.proc_win_input = dm_title_variant.proc_win_input;
            main_work.disp_flag |= 1U;
            main_work.flag &= 4294901759U;
        }
        else
            dmTitleSetWinOpenEfct(main_work);
    }

    private static void dmTitleProcWindowAnnounceIdle(DMS_TITLE_MAIN_WORK main_work)
    {
        if (main_work.proc_win_input != null)
            main_work.proc_win_input(main_work);
        if (main_work.win_mode == 0)
        {
            if (((int)main_work.flag & 4) != 0 && main_work.win_cur_slct == 0)
            {
                main_work.proc_input = null;
                main_work.proc_win_input = null;
                main_work.win_timer = 8f;
                main_work.disp_flag &= 4294967294U;
                main_work.proc_win_update = (dmTitleProcWindowCloseEfct);
                DmSoundPlaySE("Ok");
                main_work.flag &= 4294967291U;
                main_work.flag &= 4294967293U;
            }
            else
            {
                if (((int)main_work.flag & 2) == 0 && (main_work.win_cur_slct != 1 || ((int)main_work.flag & 4) == 0))
                    return;
                main_work.proc_input = null;
                main_work.proc_win_input = null;
                main_work.win_timer = 8f;
                main_work.disp_flag &= 4294967294U;
                main_work.proc_win_update = (dmTitleProcWindowCloseEfct);
                DmSoundPlaySE("Cancel");
                main_work.flag |= 2U;
                main_work.flag &= 4294967291U;
            }
        }
        else
        {
            if (main_work.win_mode != 1)
                return;
            if (((int)main_work.flag & 4) != 0 && main_work.win_cur_slct == 0)
            {
                main_work.proc_input = null;
                main_work.proc_win_input = null;
                main_work.win_timer = 8f;
                main_work.disp_flag &= 4294967294U;
                main_work.proc_win_update = (dmTitleProcWindowCloseEfct);
                DmSoundPlaySE("Ok");
                main_work.flag &= 4294967291U;
                main_work.flag &= 4294967293U;
            }
            else
            {
                if (((int)main_work.flag & 2) == 0 && (main_work.win_cur_slct != 1 || ((int)main_work.flag & 4) == 0))
                    return;
                main_work.proc_input = null;
                main_work.proc_win_input = null;
                main_work.win_timer = 8f;
                main_work.disp_flag &= 4294967294U;
                main_work.proc_win_update = (dmTitleProcWindowCloseEfct);
                DmSoundPlaySE("Cancel");
                main_work.flag |= 2U;
                main_work.flag &= 4294967291U;
            }
        }
    }

    private static void dmTitleProcWindowCloseEfct(DMS_TITLE_MAIN_WORK main_work)
    {
        if (((int)main_work.flag & 65536) != 0)
        {
            main_work.proc_win_update = (dmTitleProcWindowNodispIdle);
            main_work.announce_flag &= (uint)~(1 << main_work.win_mode);
            if (main_work.win_mode == 0 || main_work.win_mode == 1)
            {
                if (((int)main_work.flag & 2) != 0)
                {
                    main_work.flag |= 8192U;
                    main_work.flag &= 4294967293U;
                }
                else if (main_work.win_mode == 0)
                    main_work.announce_flag |= 2U;
                else if (main_work.win_mode == 1)
                    main_work.flag |= 16384U;
            }
            else if (((int)main_work.flag & 2) != 0)
            {
                main_work.flag |= 16777216U;
                main_work.flag &= 4294967293U;
            }
            else if (main_work.win_mode == 2)
                main_work.flag |= 8388608U;
            else if (main_work.win_mode == 3)
                main_work.flag |= 33554432U;
            else
                main_work.flag |= 67108864U;

            main_work.flag &= 4294836223U;
            main_work.flag &= 4294901759U;
        }

        dmTitleSetWinCloseEfct(main_work);
    }

    private static void dmTitleProcDataRelease(DMS_TITLE_MAIN_WORK main_work)
    {
        if (!DmTitleOp.DmTitleOpExitEndCheck())
            return;
        for (int index = 0; index < 2; ++index)
            AoTexRelease(main_work.tex[index]);
        for (int index = 0; index < 4; ++index)
            AoTexRelease(main_work.cmn_tex[index]);
        DmTitleOp.DmTitleOpFlush();
        main_work.proc_update = (dmTitleProcFinish);
    }

    private static void dmTitleProcFinish(DMS_TITLE_MAIN_WORK main_work)
    {
        if (!dmTitleIsTexRelease(main_work))
            return;
        int length1 = main_work.trg_slct.Length;
        for (int index = 0; index < length1; ++index)
            main_work.trg_slct[index].Release();
        int length2 = main_work.trg_answer.Length;
        for (int index = 0; index < length2; ++index)
            main_work.trg_answer[index].Release();
        main_work.trg_return.Release();
        main_work.trg_game.Release();
        for (int index = 0; index < 41; ++index)
        {
            if (main_work.act[index] != null)
            {
                AoActDelete(main_work.act[index]);
                main_work.act[index] = null;
            }
        }

        for (int index = 0; index < 2; ++index)
        {
            if (main_work.arc_amb[index] != null)
                main_work.arc_amb[index] = null;
        }

        for (int index = 0; index < 4; ++index)
        {
            if (main_work.arc_cmn_amb[index] != null)
                main_work.arc_cmn_amb[index] = null;
        }

        DmTitleOp.DmTitleOpRelease();
        main_work.proc_update = (dmTitleProcWaitFinished);
    }

    private static void dmTitleProcWaitFinished(DMS_TITLE_MAIN_WORK main_work)
    {
        if (!DmTitleOp.DmTitleOpReleaseCheck())
            return;
        if (((int)main_work.flag & 524288) != 0)
        {
            if (!DmSndBgmPlayerIsTaskExit())
                return;
            main_work.flag &= 4294443007U;
        }

        main_work.flag |= 1U;
        main_work.proc_update = null;
    }
}
