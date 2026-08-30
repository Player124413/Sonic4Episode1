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
    private static void dmTitleInputProcTitle(DMS_TITLE_MAIN_WORK main_work)
    {
        var stand = AoPad.AoPadSomeoneMStand(ControllerConsts.START | ControllerConsts.CONFIRM);
        if (amTpIsTouchPush(0) || stand != -1)
        {
            AoAccountSetCurrentIdStart(stand != -1 ? (uint)stand : 0);
            main_work.flag |= DMD_TITLE_FLAG_GAME_START;
        }

        if (!isBackKeyPressed())
            return;
        finish();
        setBackKeyRequest(false);
    }

    private static void dmTitleInputProcMainMenu(DMS_TITLE_MAIN_WORK main_work)
    {
        int length = main_work.trg_slct.Length;
        if (!LiveFeature.isInterrupted())
        {
            int now_slct_menu = 0;
            int cur_edge_menu_top = 0;
            int cur_edge_menu_bottom = 0;

            // 体験版・初期データ・データありで項目を切り替える
            if (main_work.cur_slct_menu > 0)
            {
                if (GsTrialIsTrial()
                    || main_work.is_init_play)
                {
                    now_slct_menu = main_work.cur_slct_menu + 1;
                }
                else
                {
                    now_slct_menu = main_work.cur_slct_menu;
                }
            }

            // キャンセル処理
            if (AoPad.AoPadMStand().HasFlag(ControllerConsts.CANCEL))
            {
                main_work.flag |= DMD_TITLE_FLAG_CANCEL;

                return;
            }

            // 決定処理
            if (AoPad.AoPadMStand().HasFlag(ControllerConsts.CONFIRM) || AoPad.AoPadMStand().HasFlag(ControllerConsts.START))
            {
                main_work.flag |= DMD_TITLE_FLAG_DECIDE;

                return;
            }


            // 体験版時の上下の端に当たるメニュー項目
            if (GsTrialIsTrial())
            {
                cur_edge_menu_top = main_work.slct_menu_num - 1;
                cur_edge_menu_bottom = main_work.slct_menu_num - 2;
            }
            // 製品版時の上下の端に当たるメニュー項目
            else
            {
                cur_edge_menu_top = 0;
                cur_edge_menu_bottom = main_work.slct_menu_num - 1;
            }

            // 十字キー操作
            if (AoPad.AoPadMRepeat().HasFlag(ControllerConsts.UP))
            {
                if (AoPad.AoPadMStand().HasFlag(ControllerConsts.UP)
                    || main_work.cur_slct_menu != cur_edge_menu_top)
                {
                    // 選択ZONE切り替え
                    main_work.flag |= DMD_TITLE_FLAG_MENU_UP_INPUT;
                }
            }
            else if (AoPad.AoPadMRepeat().HasFlag(ControllerConsts.DOWN))
            {
                if (AoPad.AoPadMStand().HasFlag(ControllerConsts.DOWN)
                    || main_work.cur_slct_menu != cur_edge_menu_bottom)
                {
                    // 選択ZONE切り替え
                    main_work.flag |= DMD_TITLE_FLAG_MENU_DOWN_INPUT;
                }
            }

            for (int index = 0; index < length; ++index)
            {
                CTrgAoAction ctrgAoAction = main_work.trg_slct[index];
                if (ctrgAoAction.GetState(0U)[10] && ctrgAoAction.GetState(0U)[1])
                {
                    main_work.cur_slct_menu = index;
                    main_work.flag |= 4U;
                    break;
                }
            }

            CTrgAoAction trgReturn = main_work.trg_return;
            if (trgReturn.GetState(0U)[10] && trgReturn.GetState(0U)[1] || isBackKeyPressed() ||
                AoPad.AoPadMStand().HasFlag(ControllerConsts.B))
            {
                setBackKeyRequest(false);
                //if (LiveFeature.isInterrupted())
                //    LiveFeature.endInterrupt();
                //else
                    main_work.flag |= 2U;
            }


            CTrgAoAction trgGame = main_work.trg_game;
            if (trgGame.GetState(0U)[10] && trgGame.GetState(0U)[1] || AoPad.AoPadMStand().HasFlag(ControllerConsts.Y))
            {
                string url = GsEnvGetLanguage() != 0 ? c_url : "http://sega.jp/kt/microsoft/smart/";
                DmSoundPlaySE("Ok");
                erWeb.StartWeb(url);
            }
        }
    }

    private static void dmTitleInputProcWindowPc(DMS_TITLE_MAIN_WORK main_work)
    {
        var pad = AoPad.AoPadMStand();

        if (pad.HasFlag(ControllerConsts.CANCEL))
            main_work.flag |= DMD_TITLE_FLAG_CANCEL;

        if (pad.HasFlag(ControllerConsts.CONFIRM))
            main_work.flag |= DMD_TITLE_FLAG_DECIDE;

        if (AoPad.AoPadMRepeat().HasFlag(ControllerConsts.LEFT))
        {
            if (main_work.win_cur_slct != 0)
            {
                main_work.win_cur_slct = 0;
                DmSoundPlaySE("Cursol");
            }
        }
        else if (AoPad.AoPadMRepeat().HasFlag(ControllerConsts.RIGHT))
        {
            if (main_work.win_cur_slct != 1)
            {
                main_work.win_cur_slct = 1;
                DmSoundPlaySE("Cursol");
            }
        }
    }

    private static void dmTitleInputProcWindowIphone(DMS_TITLE_MAIN_WORK main_work)
    {
        CTrgAoAction[] trgAnswer = main_work.trg_answer;

        for (int i = 0; i < 2; i++)
            trgAnswer[i].IsSelected = i == main_work.win_cur_slct;

        if (trgAnswer[0].GetState(0U)[10] && trgAnswer[0].GetState(0U)[1])
        {
            main_work.win_cur_slct = 1;
            main_work.flag |= DMD_TITLE_FLAG_DECIDE;
        }

        if (trgAnswer[1].GetState(0U)[10] && trgAnswer[1].GetState(0U)[1])
        {
            main_work.win_cur_slct = 0;
            main_work.flag |= DMD_TITLE_FLAG_DECIDE;
        }
    }
}
