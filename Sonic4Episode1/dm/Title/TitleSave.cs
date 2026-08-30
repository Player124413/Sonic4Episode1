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
    private static void dmTitleSetLoadSysData(DMS_TITLE_MAIN_WORK main_work)
    {
        GSS_MAIN_SYS_INFO mainSysInfo = GsGetMainSysInfo();
        uint num = 0;

        SOption instance1 = SOption.CreateInstance();
        int volumeBgm = (int)instance1.GetVolumeBgm();
        int volumeSe = (int)instance1.GetVolumeSe();
        mainSysInfo.bgm_volume = volumeBgm / 10;
        mainSysInfo.se_volume = volumeSe / 10;
        DmSoundSetVolumeBGM(mainSysInfo.bgm_volume);
        DmSoundSetVolumeSE(mainSysInfo.se_volume);

        mainSysInfo.game_flag |= GSD_MAINSYS_GAME_FLAG_PAD_VIB_ENABLE;
        SSpecial instance2 = SSpecial.CreateInstance();
        for (int index = 0; index < 7; ++index)
        {
            if (instance2[(ushort)index].IsGetEmerald())
                num |= (uint)(1 << index);
        }

        if (num == (uint)sbyte.MaxValue)
            mainSysInfo.game_flag |= GSD_MAINSYS_GAME_FLAG_7_CHAOS_EMERALD;
        else
            mainSysInfo.game_flag &= ~GSD_MAINSYS_GAME_FLAG_7_CHAOS_EMERALD;

        SSystem instance3 = SSystem.CreateInstance();
        mainSysInfo.rest_player_num = instance3.PlayerStock;
        mainSysInfo.ene_kill_count = instance3.Killed;
        mainSysInfo.final_clear_count = instance3.ClearCount;
        switch (SOption.CreateInstance().GetControl())
        {
            case SOption.EControl.Type.VirtualPadDown:
                mainSysInfo.game_flag &= ~GSD_MAINSYS_GAME_FLAG_INPUT_IS_FLICK;
                mainSysInfo.game_flag |= GSD_MAINSYS_GAME_FLAG_INPUT_CLASSIC;
                break;
            case SOption.EControl.Type.VirtualPadUp:
                mainSysInfo.game_flag |= GSD_MAINSYS_GAME_FLAG_INPUT_IS_FLICK;
                mainSysInfo.game_flag |= GSD_MAINSYS_GAME_FLAG_INPUT_CLASSIC;
                break;
            default:
                mainSysInfo.game_flag &= ~GSD_MAINSYS_GAME_FLAG_INPUT_IS_FLICK;
                mainSysInfo.game_flag &= ~GSD_MAINSYS_GAME_FLAG_INPUT_CLASSIC;
                break;
        }

        mainSysInfo.is_spe_clear = false;
        mainSysInfo.is_first_play = false;
    }

    private static void dmTitleSetInitSysData(DMS_TITLE_MAIN_WORK main_work)
    {
        GSS_MAIN_SYS_INFO mainSysInfo = GsGetMainSysInfo();
        SOption instance = SOption.CreateInstance();
        int volumeBgm = (int)instance.GetVolumeBgm();
        int volumeSe = (int)instance.GetVolumeSe();
        mainSysInfo.bgm_volume = volumeBgm / 10f;
        mainSysInfo.se_volume = volumeSe / 10f;
        DmSoundSetVolumeBGM(mainSysInfo.bgm_volume);
        DmSoundSetVolumeSE(mainSysInfo.se_volume);
        instance.IsVibration();
        mainSysInfo.game_flag |= GSD_MAINSYS_GAME_FLAG_PAD_VIB_ENABLE;
        mainSysInfo.game_flag &= GSD_MAINSYS_GAME_FLAG_7_CHAOS_EMERALD;
        mainSysInfo.rest_player_num = GSD_MAINSYS_PLAYER_REST_DEF;
        mainSysInfo.ene_kill_count = 0;
        mainSysInfo.final_clear_count = 0;
        mainSysInfo.is_spe_clear = false;
        mainSysInfo.is_first_play = false;
    }

    private static void dmTitleSetMenuInfo(DMS_TITLE_MAIN_WORK main_work)
    {
        if (GsTrialIsTrial())
        {
            main_work.slct_menu_num = dm_title_variant.slct_menu_num_trial;
            main_work.is_init_play = true;
        }
        else if (!GsMainSysIsStageClear(GSD_MAIN_STAGE_ID_1_1))
        {
            main_work.slct_menu_num = dm_title_variant.slct_menu_num_first;
            main_work.is_init_play = true;
        }
        else
        {
            main_work.slct_menu_num = dm_title_variant.slct_menu_num_played;
            main_work.is_init_play = false;
        }
    }

    private static void dmTitleSetFirstPlayData(DMS_TITLE_MAIN_WORK main_work)
    {
        GSS_MAIN_SYS_INFO mainSysInfo = GsGetMainSysInfo();
        SSystem instance = SSystem.CreateInstance();
        mainSysInfo.rest_player_num = GSD_MAINSYS_PLAYER_REST_DEF;
        instance.PlayerStock = mainSysInfo.rest_player_num;
        dmTitleSetLoadSysData(main_work);
    }

    private static void dmTitleSetInitSaveData(DMS_TITLE_MAIN_WORK main_work)
    {
        GsGetMainSysInfo().Save.Init();
    }

    private static bool dmTitleIsSaveRunning()
    {
        return GsGetMainSysInfo().is_save_run != 0U;
    }

    private static bool dmTitleIsChangeOptVol()
    {
        SOption instance = SOption.CreateInstance();
        return instance.GetVolumeBgm() != 100U || instance.GetVolumeSe() != 100U;
    }
}
