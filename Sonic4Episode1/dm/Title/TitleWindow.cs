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
    private static void dmTitleWinSelectDraw(DMS_TITLE_MAIN_WORK main_work)
    {
        var dm_title_win_act_pos_tbl = dm_title_variant.win_act_pos_tbl;
        var ACT_WIN_NO_BTN_L = (int)DME_TITLE_ACT.ACT_WIN_NO_BTN_L;
        var ACT_WIN_YES_BTN_R = (int)DME_TITLE_ACT.ACT_WIN_YES_BTN_R;
        var ACT_TEX_YES = dmTitleActIdx(DME_TITLE_ACT_REG.ACT_TEX_YES);
        var ACT_TEX_NO = dmTitleActIdx(DME_TITLE_ACT_REG.ACT_TEX_NO);
        var ACT_WIN_TEX_MSG1 = dmTitleActIdx(DME_TITLE_ACT_REG.ACT_WIN_TEX_MSG1);
        var ACT_WIN_TEX_MSG2 = dmTitleActIdx(DME_TITLE_ACT_REG.ACT_WIN_TEX_MSG2);

        // -1 on mobile, which has none of the three. the decompile mapped them to 0, drawing
        // ACT_BTN_L in their place; every use below is guarded instead
        var ACT_WIN_TEX_MSG3 = dmTitleActIdx(DME_TITLE_ACT_REG.ACT_WIN_TEX_MSG3);
        var ACT_WIN_LINE = dmTitleActIdx(DME_TITLE_ACT_REG.ACT_WIN_LINE);
        var ACT_TEX_WINTITLE = dmTitleActIdx(DME_TITLE_ACT_REG.ACT_TEX_WINTITLE);

        // ウインドウ用AO描画プライオリティ設定
        AoActSysSetDrawTaskPrio(DMD_TITLE_DRAW_PRIO_WIN_FIX);

        // ウインドウ描画
        AoWinSysDrawState(AOD_WIN_TYPE_A
                         , AoTexGetTexList(main_work.cmn_tex[2])          // ◆
                         , 0
                         , DMD_TITLE_SIZE_WIDTH / 2.0f          // ウインドウ中心X
                         , DMD_TITLE_SIZE_HEIGHT / 2.0f         // ウインドウ中心Y
                         , dm_title_variant.window_size_w * dm_title_variant.disp_scale_text * main_work.win_size_rate[0]            // ウインドウ横サイズ
                         , DMD_TITLE_WINDOW_SIZE_H * dm_title_variant.disp_scale_text * main_work.win_size_rate[1]            // ウインドウ縦サイズ
                         , DMD_TITLE_DRAW_STATE_ID              // 描画STATE
                         );


        // ウインドウ内の項目描画
        if ((main_work.disp_flag & DMD_TITLE_DISP_FLAG_WIN_ACT) == DMD_TITLE_DISP_FLAG_WIN_ACT)
        {
            // 共通ウインドウACT登録
            AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[2]));
            //		AoActSortRegAction(main_work.act[ACT_WIN_LINE]);

            AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[3]));
            //		AoActSortRegAction(main_work.act[ACT_TEX_WINTITLE]);

            switch ((DME_TITLE_WIN_TYPE)main_work.win_mode)
            {
                case DME_TITLE_WIN_TYPE.DME_TITLE_WIN_TYPE_DEL_DATA1:
                    // アクション登録
                    if (main_work.iphone_menus)
                    {
                        for (int i = ACT_WIN_NO_BTN_L, max = ACT_WIN_YES_BTN_R + 1; i < max; ++i)
                        {
                            AoActSortRegAction(main_work.act[i]);
                        }
                    }

                    AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
                    AoActSortRegAction(main_work.act[ACT_WIN_TEX_MSG1]);
                    //			AoActSortRegAction(main_work.act[ACT_WIN_TEX_MSG2]);

                    AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[3]));
                    AoActSortRegAction(main_work.act[ACT_TEX_YES]);
                    AoActSortRegAction(main_work.act[ACT_TEX_NO]);

                    if (!main_work.iphone_menus)
                    {
                        // 初期表示フレーム設定
                        if (main_work.win_cur_slct == 0)
                        {
                            AoActSetFrame(main_work.act[ACT_TEX_YES], 0.0f);
                            AoActSetFrame(main_work.act[ACT_TEX_NO], 1.0f);
                        }

                        else
                        {
                            AoActSetFrame(main_work.act[ACT_TEX_YES], 1.0f);
                            AoActSetFrame(main_work.act[ACT_TEX_NO], 0.0f);
                        }
                    }

                    break;

                case DME_TITLE_WIN_TYPE.DME_TITLE_WIN_TYPE_DEL_DATA2:
                    // アクション登録
                    if (main_work.iphone_menus)
                    {
                        for (int i = ACT_WIN_NO_BTN_L, max = ACT_WIN_YES_BTN_R + 1; i < max; ++i)
                        {
                            AoActSortRegAction(main_work.act[i]);
                        }
                    }
                    AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
                    AoActSortRegAction(main_work.act[ACT_WIN_TEX_MSG2]);

                    AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[3]));
                    AoActSortRegAction(main_work.act[ACT_TEX_YES]);
                    AoActSortRegAction(main_work.act[ACT_TEX_NO]);
                    if (!main_work.iphone_menus)
                    {
                        // 初期表示フレーム設定
                        if (main_work.win_cur_slct == 0)
                        {
                            AoActSetFrame(main_work.act[ACT_TEX_YES], 0.0f);
                            AoActSetFrame(main_work.act[ACT_TEX_NO], 1.0f);
                        }

                        else
                        {
                            AoActSetFrame(main_work.act[ACT_TEX_YES], 1.0f);
                            AoActSetFrame(main_work.act[ACT_TEX_NO], 0.0f);
                        }
                    }

                    break;
                case DME_TITLE_WIN_TYPE.DME_TITLE_WIN_TYPE_DEL_DATA3:
                    if (ACT_WIN_TEX_MSG3 >= 0)
                    {
                        AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
                        AoActSortRegAction(main_work.act[ACT_WIN_TEX_MSG3]);
                    }

                    AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[3]));
                    AoActSortRegAction(main_work.act[ACT_TEX_YES]);
                    AoActSortRegAction(main_work.act[ACT_TEX_NO]);

                    // 初期表示フレーム設定
                    if (main_work.win_cur_slct == 0)
                    {
                        AoActSetFrame(main_work.act[ACT_TEX_YES], 0.0f);
                        AoActSetFrame(main_work.act[ACT_TEX_NO], 1.0f);
                    }

                    else
                    {
                        AoActSetFrame(main_work.act[ACT_TEX_YES], 1.0f);
                        AoActSetFrame(main_work.act[ACT_TEX_NO], 0.0f);
                    }

                    break;

                case DME_TITLE_WIN_TYPE.DME_TITLE_WIN_TYPE_DEL_DATA4:
                case DME_TITLE_WIN_TYPE.DME_TITLE_WIN_TYPE_DEL_DATA5:
                    if (ACT_WIN_TEX_MSG3 >= 0)
                    {
                        AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
                        AoActSortRegAction(main_work.act[ACT_WIN_TEX_MSG3]);
                    }

                    AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[3]));
                    AoActSortRegAction(main_work.act[ACT_TEX_YES]);
                    AoActSortRegAction(main_work.act[ACT_TEX_NO]);

                    // 初期表示フレーム設定
                    if (main_work.win_cur_slct == 0)
                    {
                        AoActSetFrame(main_work.act[ACT_TEX_YES], 0.0f);
                        AoActSetFrame(main_work.act[ACT_TEX_NO], 1.0f);
                    }

                    else
                    {
                        AoActSetFrame(main_work.act[ACT_TEX_YES], 1.0f);
                        AoActSetFrame(main_work.act[ACT_TEX_NO], 0.0f);
                    }

                    break;
                default:
                    // 例外
                    MTM_ASSERT(0);
                    break;
            }

            if (!main_work.iphone_menus)
            {
                // ACCUMURATEでトランスさせる
                AoActAcmPush();

                AoActAcmInit();
                AoActAcmApplyTrans(dm_title_win_act_pos_tbl[0, 0]
                                   , dm_title_win_act_pos_tbl[0, 1]
                                   , 0
                                   );

                AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[2]));

                // フレーム更新はSetFrameのみで行う
                AoActUpdate(main_work.act[ACT_WIN_LINE], 0.0f);

                AoActAcmPop();
            }


            AoActAcmPush();

            for (int i = 0; i < 3; i++)
            {
                AoActAcmInit();
                AoActAcmApplyTrans(dm_title_win_act_pos_tbl[i + 1, 0]
                                   , dm_title_win_act_pos_tbl[i + 1, 1]
                                   , 0
                                   );

                if (main_work.iphone_menus)
                {
                    if (i <= ACT_WIN_TEX_MSG2 - ACT_WIN_TEX_MSG1)
                    {
                        AoActAcmApplyScale(dm_title_variant.disp_scale_text, dm_title_variant.disp_scale_text);
                    }
                }

                // フレーム更新はSetFrameのみで行う
                AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
                AoActUpdate(main_work.act[ACT_WIN_TEX_MSG1 + i], 0.0f);
            }

#if !_IPHONE
            for (int i = 0; i < 3; i++)
            {
#else //!_IPHONE
		    for (int i = 1; i < 3; i++) {
			    const int ACT_TEX_WINTITLE = ACT_TEX_YES - 1; //ループが1から始まるのでYES/NOが表示される様に相殺する
#endif //!_IPHONE
                AoActAcmInit();
                AoActAcmApplyTrans(dm_title_win_act_pos_tbl[i + 4, 0]
                                   , dm_title_win_act_pos_tbl[i + 4, 1]
                                   , 0
                                   );

                // フレーム更新はSetFrameのみで行う
                AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[3]));
                AoActUpdate(main_work.act[ACT_TEX_WINTITLE + i], 0.0f);

                if (main_work.iphone_menus)
                {
                    //        AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[2]));
                    //        const int c_btn_base_table[3][2] = {
                    //            { 0, -1}
                    //, { ACT_WIN_YES_BTN_L, ACT_WIN_YES_BTN_R}
                    //, { ACT_WIN_NO_BTN_L, ACT_WIN_NO_BTN_R}
                    //        }
                    //        ;
                    //        AoActUpdate(main_work.act[c_btn_base_table[i][0] + 1], 0.0f); //当たり判定更新
                    //        const int c_btn_trg[3] = { -1, 1, 0 };
                    //        er::CTrgAoAction & answer = main_work.trg_answer[c_btn_trg[i]];
                    //        if (0 <= c_btn_trg[i])
                    //        {
                    //            answer.Update();
                    //        }
                    //        float set_frame = ((answer.GetState(0)[er::CTrgState::EState::On]) ? 1.0f : 0.0f);
                    //        for (int k = c_btn_base_table[i][0], max = c_btn_base_table[i][1] + 1; k < max; ++k)
                    //        {
                    //            AoActSetFrame(main_work.act[k], set_frame);
                    //            AoActUpdate(main_work.act[k], 0.0f);
                    //        }
                }
            }

            AoActAcmPop();

            // ソート実行
            AoActSortExecute();

            // ソートバッファ描画
            AoActSortDraw();

            // ソートバッファ全解除
            AoActSortUnregAll();
        }
    }

    private static void dmTitleTaskDraw(AMS_TCB tcb)
    {
        AoActDrawPre();
        amDrawExecCommand(10U);
        amDrawEndScene();
    }
}
