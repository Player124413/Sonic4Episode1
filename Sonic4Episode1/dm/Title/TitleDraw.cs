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
    // ==========================================================================
    // dmTitleDrawSetProcDispData
    /*!
        表示設定処理
     */
    // ==========================================================================
    private static void dmTitleDrawSetProcDispData(DMS_TITLE_MAIN_WORK main_work)
    {
        float tmp_win_size = 0;

        // 描画フラグONならば
        if ((main_work.disp_flag & DMD_TITLE_DISP_FLAG_ALL_ACTION) == DMD_TITLE_DISP_FLAG_ALL_ACTION)
        {
            // 共通描画設定部
            AoActSysSetDrawTaskPrio(0x8000);
            DmTitleOp.DmTitleOpDraw2D();

            AoActSortExecute();
            AoActSortDraw();
            AoActSortUnregAll();

            // 場面別描画設定部
            if ((main_work.flag & DMD_TITLE_FLAG_DISP_TITLE) == DMD_TITLE_FLAG_DISP_TITLE
                && DmTitleOp.DmTitleOpIsLogoActFinish())
            {
                dmTitleDrawProcTitle(main_work);
            }

            // メインメニュー用ウインドウ
            if ((main_work.flag & DMD_TITLE_FLAG_DISP_MMENU_WIN) == DMD_TITLE_FLAG_DISP_MMENU_WIN)
            {
                // ウインドウ用AO描画プライオリティ設定
                AoActSysSetDrawTaskPrio(DMD_TITLE_DRAW_PRIO_WIN_FIX);

                if (!GsTrialIsTrial() && main_work.is_init_play)
                {
                    tmp_win_size = -50;
                }
                else
                {
                    tmp_win_size = 0;
                }

                // サインアウト中に開閉演出中だった場合はウインドウを非表示
                if (main_work.mmenu_win_size_rate[0] < 0.9
                    && !AoAccountIsCurrentEnable())
                {
                    // 空処理
                }
                else
                {
                    // ウインドウ描画
                    if (!main_work.iphone_menus)
                    {
                        AoWinSysDrawState(AOD_WIN_TYPE_A
                                         , AoTexGetTexList(main_work.cmn_tex[2])          // ◆
                                         , 0
                                         , DMD_TITLE_SIZE_WIDTH / 2.0f          // ウインドウ中心X
                                         , DMD_TITLE_SIZE_HEIGHT / 2.0f + 130.0f         // ウインドウ中心Y
                                         , dm_title_variant.mmenu_win_size_w * main_work.mmenu_win_size_rate[0]                   // ウインドウ横サイズ
                                         , dm_title_variant.mmenu_win_size_h * main_work.mmenu_win_size_rate[1] + tmp_win_size    // ウインドウ縦サイズ
                                         , DMD_TITLE_DRAW_STATE_ID              // 描画STATE
                                         );
                    }
                    else
                    {
                        float[] win_size = { dm_title_variant.mmenu_win_size_w, dm_title_variant.mmenu_win_size_h };
                        bool is_trial = GsTrialIsTrial();
                        bool has_save_data = !main_work.is_init_play;
                        bool is_disp_2nd_elm = ((is_trial) ? true : has_save_data); //for SONIC4_TRIAL_EXIBITION set it false
                        if (!is_disp_2nd_elm)
                        {
                            win_size[AMD_X] = dm_title_variant.mmenu_win_size_w2.Value;
                            win_size[AMD_Y] = dm_title_variant.mmenu_win_size_h2.Value;
                        }
                        {//black bg rect
                            AoWinSysDrawState(AOD_WIN_TYPE_A
                                         , AoTexGetTexList(main_work.cmn_tex[2])          // ◆
                                         , 0
                                         , DMD_TITLE_SIZE_WIDTH / 2.0f          // ウインドウ中心X
                                         , 311.0f                                        // ウインドウ中心Y
                                         , win_size[AMD_X] * main_work.mmenu_win_size_rate[0]                  // ウインドウ横サイズ
                                         , win_size[AMD_Y] * main_work.mmenu_win_size_rate[1] + tmp_win_size   // ウインドウ縦サイズ
                                         , DMD_TITLE_DRAW_STATE_ID              // 描画STATE
                                         );
                        }


                    }
                }
                {
                    if ((main_work.flag & DMD_TITLE_FLAG_DISP_MAINMENU) == DMD_TITLE_FLAG_DISP_MAINMENU)
                    {
                        if (main_work.iphone_menus)
                        {
                            if ((main_work.flag_prev & DMD_TITLE_FLAG_DISP_MAINMENU) == 0)
                            {
                                //DMD_TITLE_FLAG_DISP_MAINMENUのONエッジ					
                                main_work.trg_return.ResetState();
                                for (int k = (int)DME_TITLE_ACT.ACT_BACK_BTN_L, max = (int)DME_TITLE_ACT.ACT_BACK_BTN_R + 1; k < max; ++k)
                                {
                                    AoActSetFrame(main_work.act[k], 0.0f);
                                }
                                //戻るボタンの演出を戻す	
                                main_work.trg_game.ResetState();
                                for (int k = (int)DME_TITLE_ACT.ACT_GAME_BTN_L, max = (int)DME_TITLE_ACT.ACT_GAME_BTN_R + 1; k < max; ++k)
                                {
                                    AoActSetFrame(main_work.act[k], 0.0f);
                                }
                            }
                        }
                        dmTitleDrawProcMainMenu(main_work);//draw softkeys and menu items
                    }
                }

                if ((main_work.flag & DMD_TITLE_FLAG_WIN_DRAW_START) == DMD_TITLE_FLAG_WIN_DRAW_START)
                {
                    // ウインドウ描画
                    dmTitleWinSelectDraw(main_work);
                }
            }
        }

        // 描画タスク生成
        amDrawMakeTask(dmTitleTaskDraw, 0x8000, 0);
    }

    private static void dmTitleDrawProcTitle(DMS_TITLE_MAIN_WORK main_work)
    {
        var ACT_TEX_START = main_work.iphone_menus ? (int)DME_TITLE_ACT.ACT_TEX_START : (int)DME_TITLE_ACT_REG.ACT_TEX_START;

        AoActSysSetDrawTaskPrio(33792U);
        AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
        if ((main_work.flag & DMD_TITLE_FLAG_DISP_TEXT) == DMD_TITLE_FLAG_DISP_TEXT)
        {
            AoActSortRegAction(main_work.act[ACT_TEX_START]);
        }

        if (((int)main_work.flag & 16) != 0)
        {
            if (main_work.disp_timer >= 4.0)
            {
                main_work.disp_timer = 0.0f;
                main_work.flag ^= 8U;
            }
            else
                ++main_work.disp_timer;
        }
        else if (main_work.disp_timer >= (double)main_work.disp_change_time)
        {
            main_work.flag ^= 8U;
            main_work.disp_change_time = ((int)main_work.flag & 8) == 0 ? 30 : 40;
            main_work.disp_timer = 0.0f;
        }
        else
            ++main_work.disp_timer;

        if (GeEnvGetDecideKey() == GSE_DECIDE_KEY.GSD_DECIDE_KEY_O)
            AoActSetFrame(main_work.act[ACT_TEX_START], 0.0f);
        else
            AoActSetFrame(main_work.act[ACT_TEX_START], 1f);

        AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
        AoActUpdate(main_work.act[ACT_TEX_START], 0.0f);
        AoActSortExecute();
        AoActSortDraw();
        AoActSortUnregAll();
    }
}
