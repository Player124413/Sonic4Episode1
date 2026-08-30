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
    private static void dmTitleLoadFontData(DMS_TITLE_MAIN_WORK main_work)
    {
        GsFontBuild();
        main_work.proc_update = (dmTitleIsLoadFontData);
    }

    private static void dmTitleIsLoadFontData(DMS_TITLE_MAIN_WORK main_work)
    {
        if (!GsFontIsBuilded())
            return;
        main_work.proc_update = (dmTitleLoadRequest);
    }

    private static void dmTitleLoadRequest(DMS_TITLE_MAIN_WORK main_work)
    {
        main_work.arc_amb_fs[0] = amFsReadBackground(DMD_TITLE_DATA_FILENAME);
        main_work.arc_amb_fs[1] = amFsReadBackground(dm_title_file_lng_amb_name_tbl[GsEnvGetLanguage()]);
        for (int index = 0; index < 3; ++index)
            main_work.arc_cmn_amb_fs[index] = amFsReadBackground(dm_title_menu_cmn_amb_name_tbl[index]);
        main_work.arc_cmn_amb_fs[3] = amFsReadBackground(dm_title_menu_cmn_lng_amb_name_tbl[GsEnvGetLanguage()]);
        DmTitleOp.DmTitleOpLoad();
        main_work.proc_update = (dmTitleProcLoadWait);
    }

    private static void dmTitleProcLoadWait(DMS_TITLE_MAIN_WORK main_work)
    {
        if (!dmTitleIsDataLoad(main_work))
            return;

        for (int index1 = 0; index1 < 2; ++index1)
        {
            main_work.arc_amb[index1] = readAMBFile(main_work.arc_amb_fs[index1]);
            main_work.ama[index1] = readAMAFile(amBindGet(main_work.arc_amb[index1], 0));

            string sPath;
            main_work.amb[index1] = readAMBFile(amBindGet(main_work.arc_amb[index1], 1, out sPath));
            main_work.amb[index1].dir = sPath;
            amFsClearRequest(main_work.arc_amb_fs[index1]);
            main_work.arc_amb_fs[index1] = null;

            AoTexBuild(main_work.tex[index1], main_work.amb[index1]);
            AoTexLoad(main_work.tex[index1]);
        }

        for (int index = 0; index < 4; ++index)
        {
            main_work.arc_cmn_amb[index] = readAMBFile(main_work.arc_cmn_amb_fs[index]);
            main_work.cmn_ama[index] = readAMAFile(amBindGet(main_work.arc_cmn_amb[index], 0));
            string sPath;
            main_work.cmn_amb[index] = readAMBFile(amBindGet(main_work.arc_cmn_amb[index], 1, out sPath));
            main_work.cmn_amb[index].dir = sPath;
            amFsClearRequest(main_work.arc_cmn_amb_fs[index]);
            main_work.arc_cmn_amb_fs[index] = null;
            AoTexBuild(main_work.cmn_tex[index], main_work.cmn_amb[index]);
            AoTexLoad(main_work.cmn_tex[index]);
        }

        DmTitleOp.DmTitleOpBuild();
        DmSndBgmPlayerInit();
        main_work.proc_update = (dmTitleProcTexBuildWait);
    }

    static void dmTitleProcTexBuildWait(DMS_TITLE_MAIN_WORK main_work)
    {
        var dm_title_g_dm_act_id_tbl = dm_title_variant.act_id_tbl;
        // テクスチャ構築完了判定
        if (dmTitleIsTexLoad(main_work)
            && DmSndBgmPlayerIsSndSysBuild())
        {
            var slots = dm_title_variant.act_slot_tbl;

            for (int i = 0; i < slots.Length; i++)
            {
                A2S_AMA_HEADER ama;
                AOS_TEXTURE tex;

                if (slots[i] < 0)
                {
                    var index = -slots[i] - 1;
                    ama = main_work.ama[index];
                    tex = main_work.tex[index];
                }
                else
                {
                    ama = main_work.cmn_ama[slots[i]];
                    tex = main_work.cmn_tex[slots[i]];
                }

                // 構築
                AoActSetTexture(AoTexGetTexList(tex));
                main_work.act[i] = AoActCreate(ama, dm_title_g_dm_act_id_tbl[i]);
            }

            if (main_work.iphone_menus)
            {
                {   //選択肢
                    int[] c_act_id_table = { (int)DME_TITLE_ACT.ACT_BTN_C, (int)DME_TITLE_ACT.ACT_BTN_C2, (int)DME_TITLE_ACT.ACT_BTN_C3 };
                    for (int i = 0, max = arrayof(c_act_id_table); i < max; ++i)
                    {
                        main_work.trg_slct[i] = new CTrgAoAction();
                        main_work.trg_slct[i].Create(main_work.act[c_act_id_table[i]]);
                    }
                }
                {   //ウインドウ・はい/いいえ
                    int[] c_act_id_table = { (int)DME_TITLE_ACT.ACT_WIN_NO_BTN_C, (int)DME_TITLE_ACT.ACT_WIN_YES_BTN_C };
                    for (int i = 0, max = arrayof(c_act_id_table); i < max; ++i)
                    {
                        main_work.trg_answer[i] = new CTrgAoAction();
                        main_work.trg_answer[i].Create(main_work.act[c_act_id_table[i]]);
                    }
                }
                {   //戻る
                    int c_act_id = (int)DME_TITLE_ACT.ACT_BACK_BTN_R;
                    main_work.trg_return.Create(main_work.act[c_act_id]);
                }
                {   //他のゲーム		
                    int c_act_id = (int)DME_TITLE_ACT.ACT_GAME_BTN_R;
                    main_work.trg_game.Create(main_work.act[c_act_id]);
                }
            }


            main_work.proc_update = dmTitleProcCheckLoadingEnd;

            DmTitleOp.DmTitleOpInit();

            // オープニング側の初期化内にObjInitがあり、そこでSTATE上書きされるので、再度設定
            AoActSysSetDrawState(DMD_TITLE_DRAW_STATE_ID);
            AoActSysSetDrawStateEnable(true);

        }
    }

    private static void dmTitleProcCheckLoadingEnd(DMS_TITLE_MAIN_WORK main_work)
    {
        if (dm_title_is_title_start)
        {
            main_work.disp_change_time = 40;
            main_work.flag |= 8U;
            main_work.proc_update = (dmTitleProcFadeIn);
            main_work.proc_draw = (dmTitleDrawSetProcDispData);
            main_work.flag |= 512U;
            DmTitleOp.DmTitleOpDispRightEnable(true);
            for (int index = 0; index < 2; ++index)
                main_work.mmenu_win_size_rate[index] = 0.0f;
            IzFadeInitEasy(1U, 0U, 32f);
            XBOXLive.allowShowUpdate = true;
        }
        else
        {
            main_work.proc_update = (dmTitleProcFadeIn);
            dmTitleSetMenuInfo(main_work);
            DmTitleOp.DmTitleOpSetRetOptionState();
            main_work.proc_draw = (dmTitleDrawSetProcDispData);
            main_work.flag |= 1024U;
            main_work.flag |= 2097152U;
            DmTitleOp.DmTitleOpDispRightEnable(false);
            for (int index = 0; index < 2; ++index)
                main_work.mmenu_win_size_rate[index] = 1f;
        }
    }
    private static bool dmTitleIsDataLoad(DMS_TITLE_MAIN_WORK main_work)
    {
        for (int index = 0; index < 2; ++index)
        {
            if (!amFsIsComplete(main_work.arc_amb_fs[index]))
                return false;
        }

        for (int index = 0; index < 4; ++index)
        {
            if (!amFsIsComplete(main_work.arc_cmn_amb_fs[index]))
                return false;
        }

        return DmTitleOp.DmTitleOpLoadCheck();
    }

    private static bool dmTitleIsTexLoad(DMS_TITLE_MAIN_WORK main_work)
    {
        for (int index = 0; index < 2; ++index)
        {
            if (!AoTexIsLoaded(main_work.tex[index]))
                return false;
        }

        for (int index = 0; index < 4; ++index)
        {
            if (!AoTexIsLoaded(main_work.cmn_tex[index]))
                return false;
        }

        return GsFontIsBuilded() && DmTitleOp.DmTitleOpBuildCheck();
    }

    private static bool dmTitleIsTexRelease(DMS_TITLE_MAIN_WORK main_work)
    {
        for (int index = 0; index < 2; ++index)
        {
            if (!AoTexIsReleased(main_work.tex[index]))
                return false;
        }

        for (int index = 0; index < 4; ++index)
        {
            if (!AoTexIsReleased(main_work.cmn_tex[index]))
                return false;
        }

        return DmTitleOp.DmTitleOpFlushCheck();
    }
}
