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
    // The menu text sprites carry baked positions from the C++'s seven-item console layout, on a
    // dm_title_crsr_pos_y_tbl[1] grid: GAME 0, TUDUKI 1, OPTION 2, RANK 3, ZISSEKI 4, LIBRARY 5,
    // BUY 6. RANK and ZISSEKI are not in the enum, so LIBRARY and BUY need pulling up into the hole
    // they leave. Each caller passes how many slots it still needs, because the init_slct_dist
    // groups below already close part of the gap and close different amounts per menu state.
    private static float dmTitleMenuSlotShift(int act, float slots)
    {
        return act >= (int)DME_TITLE_ACT_REG.ACT_TEX_LIBRARY
            ? -slots * dm_title_crsr_pos_y_tbl[1]
            : 0f;
    }

    private static void dmTitleDrawProcMainMenu(DMS_TITLE_MAIN_WORK main_work)
    {
        if (main_work.iphone_menus)
        {
            bool is_trial = GsTrialIsTrial();
            bool has_save_data = !main_work.is_init_play;
            dmTitleDrawProcMainMenuIphone(main_work, has_save_data, is_trial);
        }
        else
        {
            float init_slct_dist = 0.0f;
            int init_dst_menu_data = 0;

            AoActSysSetDrawTaskPrio(DMD_TITLE_DRAW_PRIO_ACT);

            AoActSetTexture(AoTexGetTexList(main_work.tex[0]));

            AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_MENU_CRSR]);

            AoActSetTexture(AoTexGetTexList(main_work.tex[1]));

            if (GsTrialIsTrial())
            {
                dmTitleDrawProcMainMenuTrial(main_work);
            }

            else if (main_work.is_init_play)
            {
                dmTitleDrawProcMainMenuFirst(main_work);
            }

            else
            {
                dmTitleDrawProcMainMenuPlayed(main_work);
            }


            for (int i = (int)DME_TITLE_ACT_REG.ACT_TEX_GAME; i < (int)DME_TITLE_ACT_REG.ACT_BTN_CANCEL; i++)
            {
                if (main_work.act[i] != null)
                    AoActSetFrame(main_work.act[i], 1.0f);
            }

            // 決定演出用フレーム設定
            AoActSetFrame(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_GAME_L]
                          , main_work.decide_menu_frm[(int)DME_TITLE_MENU_TYPE.DME_TITLE_MENU_TYPE_START]);
            AoActSetFrame(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_OPTION_L]
                          , main_work.decide_menu_frm[(int)DME_TITLE_MENU_TYPE.DME_TITLE_MENU_TYPE_OPTION]);


            AoActSetTexture(AoTexGetTexList(main_work.tex[0]));

            AoActAcmPush();
            AoActAcmInit();
            AoActAcmApplyTrans(dm_title_variant.crsr_dflt_pos_x
                               , main_work.cur_crsr_pos_y
                               , 0.0f
                               );

            if (!GsTrialIsTrial() && main_work.is_init_play)
            {
                AoActAcmApplyTrans(0.0f, 85.0f, 0.0f);
            }
            else
            {
                AoActAcmApplyTrans(0.0f, 60.0f, 0.0f);
            }

            AoActUpdate(main_work.act[(int)DME_TITLE_ACT_REG.ACT_MENU_CRSR], 0.0f);
            AoActAcmPop();

            //#if _PC || _XBOX
            AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[0]));
            AoActUpdate(main_work.act[(int)DME_TITLE_ACT_REG.ACT_BTN_X], 0.0f);
            //#endif

            AoActSetTexture(AoTexGetTexList(main_work.tex[1]));

            // ここでPFごとの表示アクションを設定
#if _PC || _XBOX
    init_dst_menu_data = ACT_TEX_BUY_L;
#elif _PS3
    init_dst_menu_data = ACT_TEX_ZISSEKI_L;
#elif _WII
    init_dst_menu_data = ACT_TEX_RANK_L;
#endif
            init_dst_menu_data = (int)DME_TITLE_ACT_REG.ACT_TEX_BUY_L;

            if (GsTrialIsTrial())
            {
                AoActAcmPush();

                for (int i = (int)DME_TITLE_ACT_REG.ACT_TEX_GAME; i < (int)DME_TITLE_ACT_REG.ACT_BTN_CANCEL; i++)
                {
                    AoActAcmInit();

                    if (i == (int)DME_TITLE_ACT_REG.ACT_TEX_GAME_L
                        || i == (int)DME_TITLE_ACT_REG.ACT_TEX_GAME)
                    {
                        init_slct_dist = 50.0f;
                    }
                    else
                    {
                        init_slct_dist = 0.0f;
                    }

                    // this branch gives LIBRARY the same delta as OPTION, so the whole hole is still open
                    AoActAcmApplyTrans(0.0f, init_slct_dist + dmTitleMenuSlotShift(i, 2f), 0.0f);
                    AoActUpdate(main_work.act[i], 0.0f);
                }
                AoActAcmPop();
            }
            else if (GsTrialIsTrial() || main_work.is_init_play)
            {
                AoActAcmPush();

                for (int i = (int)DME_TITLE_ACT_REG.ACT_TEX_GAME; i < (int)DME_TITLE_ACT_REG.ACT_BTN_CANCEL; i++)
                {
                    AoActAcmInit();
                    if (i >= (int)DME_TITLE_ACT_REG.ACT_TEX_LIBRARY && i <= init_dst_menu_data)
                    {
                        init_slct_dist = -50.0f;
                    }
                    else if (i >= (int)DME_TITLE_ACT_REG.ACT_TEX_OPTION && i <= init_dst_menu_data)
                    {
                        init_slct_dist = 0.0f;
                    }
                    else
                    {
                        init_slct_dist = 50.0f;
                    }

                    // these two already pull LIBRARY down one group relative to OPTION, so one slot left
                    AoActAcmApplyTrans(0.0f, init_slct_dist + dmTitleMenuSlotShift(i, 1f), 0.0f);

                    if (!GsTrialIsTrial() && main_work.is_init_play)
                    {
                        AoActAcmApplyTrans(0.0f, 35.0f, 0.0f);
                    }

                    AoActUpdate(main_work.act[i], 0.0f);
                }
                AoActAcmPop();
            }
            else
            {
                AoActAcmPush();
                for (int i = (int)DME_TITLE_ACT_REG.ACT_TEX_GAME; i < (int)DME_TITLE_ACT_REG.ACT_BTN_CANCEL; i++)
                {
                    AoActAcmInit();
                    if (i >= (int)DME_TITLE_ACT_REG.ACT_TEX_LIBRARY && i <= init_dst_menu_data)
                    {
                        init_slct_dist = 0.0f;
                    }
                    else
                    {
                        init_slct_dist = 50.0f;
                    }

                    // these two already pull LIBRARY down one group relative to OPTION, so one slot left
                    AoActAcmApplyTrans(0.0f, init_slct_dist + dmTitleMenuSlotShift(i, 1f), 0.0f);
                    AoActAcmApplyTrans(0.0f, 10.0f, 0.0f);
                    AoActUpdate(main_work.act[i], 0.0f);
                }
                AoActAcmPop();
            }

            // ソート描画
            AoActSortExecute();
            AoActSortDraw();
            AoActSortUnregAll();
        }
    }



    // ==========================================================================
    // dmTitleDrawProcMainMenuTrial
    /*!
        メインメニュー時表示設定処理
     */
    // ==========================================================================
    static void dmTitleDrawProcMainMenuTrial(DMS_TITLE_MAIN_WORK main_work)
    {
        AoActSetTexture(AoTexGetTexList(main_work.tex[1]));

        switch (main_work.cur_slct_menu)
        {
            case 0:
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_GAME_L]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_OPTION]);
                //AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_LIBRARY]);
                AoActSetFrame(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_GAME], 0.0f);
                break;
            case 1:
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_GAME]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_OPTION_L]);
                //AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_LIBRARY]);
                AoActSetFrame(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_OPTION_L], 0.0f);
                break;
            case 2:
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_GAME]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_OPTION]);
                //AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_LIBRARY]);
                AoActSetFrame(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_LIBRARY], 0.0f);
                break;
            case 3:
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_GAME]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_OPTION]);
                //AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_LIBRARY]);
                break;
            default:
                break;
        }
    }



    // ==========================================================================
    // dmTitleDrawProcMainMenuFirst
    /*!
        メインメニュー時表示設定処理
     */
    // ==========================================================================
    static void dmTitleDrawProcMainMenuFirst(DMS_TITLE_MAIN_WORK main_work)
    {
        AoActSetTexture(AoTexGetTexList(main_work.tex[1]));

        switch (main_work.cur_slct_menu)
        {
            case 0:
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_GAME_L]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_OPTION]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_LIBRARY]);
                AoActSetFrame(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_GAME], 0.0f);
                break;
            case 1:
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_GAME]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_OPTION_L]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_LIBRARY]);
                AoActSetFrame(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_OPTION], 0.0f);
                break;
            case 2:
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_GAME]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_OPTION]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_LIBRARY_L]);
                AoActSetFrame(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_LIBRARY], 0.0f);
                break;
            default:
                break;
        }

    }

    // ==========================================================================
    // dmTitleDrawProcMainMenuPlayed
    /*!
        メインメニュー時表示設定処理
     */
    // ==========================================================================
    static void dmTitleDrawProcMainMenuPlayed(DMS_TITLE_MAIN_WORK main_work)
    {
        AoActSetTexture(AoTexGetTexList(main_work.tex[1]));

        switch (main_work.cur_slct_menu)
        {
            case 0:
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_GAME_L]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_TUDUKI]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_OPTION]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_LIBRARY]);
                AoActSetFrame(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_GAME_L], 0.0f);
                break;
            case 1:
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_GAME]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_TUDUKI_L]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_OPTION]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_LIBRARY]);
                AoActSetFrame(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_OPTION], 0.0f);
                break;
            case 2:
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_GAME]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_TUDUKI]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_OPTION_L]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_LIBRARY]);
                AoActSetFrame(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_OPTION], 0.0f);
                break;
            case 3:
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_GAME]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_TUDUKI]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_OPTION]);
                AoActSortRegAction(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_LIBRARY_L]);
                AoActSetFrame(main_work.act[(int)DME_TITLE_ACT_REG.ACT_TEX_LIBRARY], 0.0f);
                break;
            default:
                break;
        }

    }


    private static void CLocalBtnBase_Update(
        AOS_ACTION src,
        ArrayPointer<AOS_ACTION> btn,
        CTrgAoAction trg)
    {
        CLocalBtnBase_Update(src, btn, trg, 0.0f);
    }

    private static void CLocalBtnBase_Update(
        AOS_ACTION src,
        ArrayPointer<AOS_ACTION> btn,
        CTrgAoAction trg,
        float frame)
    {
        AoActAcmPush();
        AoActAcmInit();
        int[] numArray = new int[2]
        {
            (int) src.sprite.center_x,
            (int) src.sprite.center_y
        };
        AoActAcmApplyTrans(numArray[0], numArray[1], 1f);
        AoActUpdate(btn[0], frame);
        AoActUpdate(btn[1], frame);
        AoActUpdate(btn[2], frame);
        trg.Update();
        AoActAcmPop();
    }

    private static void CLocalBtnBase_Draw(ArrayPointer<AOS_ACTION> btn)
    {
        AoActSortRegAction(btn[0]);
        AoActSortRegAction(btn[1]);
        AoActSortRegAction(btn[2]);
    }

    private static void CLocalBtnBase_SetFrame(ArrayPointer<AOS_ACTION> btn, float frame)
    {
        AoActSetFrame(btn[0], frame);
        AoActSetFrame(btn[1], frame);
        AoActSetFrame(btn[2], frame);
    }

    private static void dmTitleDrawProcMainMenuIphone(
        DMS_TITLE_MAIN_WORK main_work,
        bool has_save_data,
        bool is_trial)
    {
        AoActSysSetDrawTaskPrio(33792U);
        int[] numArray1 = new int[3] { 20, 21, 22 };
        bool flag = has_save_data;
        int[] numArray2 = new int[3] { 0, 3, 6 };
        float[] numArray3 = new float[3];
        if (is_trial)
        {
            numArray1[1] = 23;
            flag = true;
        }

        if ((dmTitleProcMainMenuDecideEfct) == main_work.proc_update)
            numArray3[main_work.cur_slct_menu] = 1f;
        else if ((!IzFadeIsExe() || IzFadeIsEnd()) &&
                 (!((dmTitleProcSaveInitData) == main_work.proc_update) &&
                  (131072 & (int)main_work.flag) == 0))
        {
            for (int index = 0; index < 3; ++index)
            {
                float frame;
                if (!main_work.trg_slct[index].GetState(0U)[10] || !main_work.trg_slct[index].GetState(0U)[1])
                    frame = !main_work.trg_slct[index].GetState(0U)[0] ? 0.0f : 1f;
                else
                    frame = 2f;
                AoActSetFrame(main_work.act[numArray1[index]], frame);
                CLocalBtnBase_SetFrame(new ArrayPointer<AOS_ACTION>(main_work.act, numArray2[index]), frame);
            }

            float frame1;
            if (!main_work.trg_return.GetState(0U)[10] || !main_work.trg_return.GetState(0U)[1])
                if (!main_work.trg_return.GetState(0U)[0])
                    frame1 = 2.0 > main_work.act[15].frame ? 0.0f : main_work.act[15].frame;
                else
                    frame1 = 1f;
            else
                frame1 = 2f;

            AoActSetFrame(main_work.act[26], frame1);
            int index1 = 15;
            for (int index2 = 17; index1 < index2; ++index1)
                AoActSetFrame(main_work.act[index1], frame1);
            float frame2;

            if (!main_work.trg_game.GetState(0U)[10] || !main_work.trg_game.GetState(0U)[1])
                if (!main_work.trg_game.GetState(0U)[0])
                    frame2 = 2.0 > main_work.act[17].frame ? 0.0f : main_work.act[17].frame;
                else
                    frame2 = 1f;
            else
                frame2 = 2f;

            AoActSetFrame(main_work.act[27], frame2);
            int index3 = 17;
            for (int index2 = 19; index3 < index2; ++index3)
                AoActSetFrame(main_work.act[index3], frame2);
        }

        if (!LiveFeature.isInterrupted())
        {
            var dm_title_g_dm_act_id_tbl = dm_title_variant.act_id_tbl;

            if (!flag)
            {
                main_work.ama[1].act_tbl[(int)dm_title_g_dm_act_id_tbl[20]].mtn.trs_tbl[0].trs_y = 250f;
                main_work.ama[1].act_tbl[(int)dm_title_g_dm_act_id_tbl[22]].mtn.trs_tbl[0].trs_y = 360f;
                //main_work.ama[1].act_tbl[(int)dm_title_g_dm_act_id_tbl[24]].mtn.trs_tbl[0].trs_y = 470f;
                //main_work.ama[1].act_tbl[(int)dm_title_g_dm_act_id_tbl[25]].mtn.trs_tbl[0].trs_y = 580f;
            }
            else
                main_work.ama[1].act_tbl[(int)dm_title_g_dm_act_id_tbl[21]].mtn.trs_tbl[0].trs_y = 305f;

            AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
            AoActUpdate(main_work.act[numArray1[0]], numArray3[0]);
            if (flag)
            {
                AoActUpdate(main_work.act[numArray1[1]], numArray3[1]);
                main_work.act[numArray1[1]].sprite.center_y = 305f;
            }

            AoActUpdate(main_work.act[numArray1[2]], numArray3[2]);
            //AoActUpdate(main_work.act[numArray1[3]], numArray3[3]);
            //AoActUpdate(main_work.act[numArray1[4]], numArray3[4]);
            AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
            CLocalBtnBase_Update(main_work.act[numArray1[0]],
                new ArrayPointer<AOS_ACTION>(main_work.act, numArray2[0]), main_work.trg_slct[0], numArray3[0]);
            if (flag)
                CLocalBtnBase_Update(main_work.act[numArray1[1]],
                    new ArrayPointer<AOS_ACTION>(main_work.act, numArray2[1]), main_work.trg_slct[1], numArray3[1]);
            CLocalBtnBase_Update(main_work.act[numArray1[2]],
                new ArrayPointer<AOS_ACTION>(main_work.act, numArray2[2]), main_work.trg_slct[2], numArray3[2]);
            // CLocalBtnBase_Update(main_work.act[numArray1[3]], new ArrayPointer<AOS_ACTION>(main_work.act, numArray2[3]), main_work.trg_slct[3], numArray3[3]);
            // CLocalBtnBase_Update(main_work.act[numArray1[4]], new ArrayPointer<AOS_ACTION>(main_work.act, numArray2[4]), main_work.trg_slct[4], numArray3[4]);
        }

        float frame3 = 2.0 <= main_work.act[15].frame ? 1f : 0.0f;
        AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
        AoActUpdate(main_work.act[26], frame3);
        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
        int index4 = 15;
        for (int index1 = 17; index4 < index1; ++index4)
            AoActUpdate(main_work.act[index4], frame3);
        main_work.trg_return.Update();
        float frame4 = 2.0 <= main_work.act[17].frame ? 1f : 0.0f;
        AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
        AoActUpdate(main_work.act[27], frame4);
        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
        int index5 = 17;
        for (int index1 = 19; index5 < index1; ++index5)
            AoActUpdate(main_work.act[index5], frame4);
        main_work.trg_game.Update();
        if (!LiveFeature.isInterrupted())
        {
            CLocalBtnBase_Draw(new ArrayPointer<AOS_ACTION>(main_work.act, numArray2[0]));
            AoActSortRegAction(main_work.act[numArray1[0]]);
            main_work.act[numArray1[0]].sprite.center_y -= 3f;
            if (flag)
            {
                CLocalBtnBase_Draw(new ArrayPointer<AOS_ACTION>(main_work.act, numArray2[1]));
                AoActSortRegAction(main_work.act[numArray1[1]]);
                main_work.act[numArray1[1]].sprite.center_y -= 3f;
            }

            CLocalBtnBase_Draw(new ArrayPointer<AOS_ACTION>(main_work.act, numArray2[2]));
            AoActSortRegAction(main_work.act[numArray1[2]]);
            main_work.act[numArray1[2]].sprite.center_y -= 3f;
            // CLocalBtnBase_Draw(new ArrayPointer<AOS_ACTION>(main_work.act, numArray2[3]));
            // AoActSortRegAction(main_work.act[numArray1[3]]);
            // main_work.act[numArray1[3]].sprite.center_y -= 3f;
            // CLocalBtnBase_Draw(new ArrayPointer<AOS_ACTION>(main_work.act, numArray2[4]));
            // AoActSortRegAction(main_work.act[numArray1[4]]);
            // main_work.act[numArray1[4]].sprite.center_y -= 3f;
        }

        if ((2097152 & (int)main_work.flag) != 0 && (131072 & (int)main_work.flag) == 0)
        {
            int index1 = 15;
            for (int index2 = 17; index1 < index2; ++index1)
                AoActSortRegAction(main_work.act[index1]);
            AoActSortRegAction(main_work.act[26]);
            int index3 = 17;
            for (int index2 = 19; index3 < index2; ++index3)
                AoActSortRegAction(main_work.act[index3]);
            AoActSortRegAction(main_work.act[27]);
        }

        AoActSortExecute();
        AoActSortDraw();
        AoActSortUnregAll();
    }
}
