using gs.backup;
using static AppMain;

using static DmSave.DME_SAVE_ACT;
using static DmSave.DME_SAVE_WIN;

internal static partial class DmSave
{
    private static readonly DMS_SAVE_MGR dm_save_mgr = new DMS_SAVE_MGR();
    private static DMS_SAVE_MGR dm_save_mgr_p;

    private static uint dm_save_draw_state;
    private static uint dm_save_msg_flag;
    private static bool dm_save_first_save;

    private static readonly AOS_TEXTURE[] dm_save_cmn_tex = new AOS_TEXTURE[2];
    private static readonly AOS_ACTION[] dm_save_act = new AOS_ACTION[ACT_COUNT];

    private static uint dm_save_disp_flag;
    private static uint dm_save_is_draw_state;
    private static readonly float[] dm_save_win_size_rate = new float[2];
    private static int dm_save_win_mode;

    private static bool dm_save_draw_reserve;
    private static bool dm_save_is_task_draw;
    private static bool dm_save_is_snd_build;

    // resolved once in dmSaveInit, never per frame
    private static DMS_SAVE_VARIANT dm_save_variant = dm_save_variant_mobile;

    // both archives this screen loads, including the current language's. a partial variant set
    // falls back wholesale, so RU picks mobile rather than reading console D_CMN_WIN ids
    private static bool dmSaveIsConsoleDataComplete()
    {
        return amFsAllVariant(
            AMD_FS_VARIANT_CONSOLE,
            DMD_MANUAL_CMN_DATA_FILENAME,
            dm_save_menu_cmn_lng_amb_name_tbl[GsEnvGetLanguage()]);
    }

    internal static DMS_SAVE_VARIANT SaveVariantForTest(bool console)
    {
        return console ? dm_save_variant_console : dm_save_variant_mobile;
    }

    public static void DmSaveStart(uint disp_flag, bool is_first_save, bool is_task_draw)
    {
        switch (SyGetEvtInfo().cur_evt_id)
        {
            case 6:
                break;
            case 9:
                break;
            case 11:
                break;
            default:
                if ((disp_flag & (1 << (int)DME_SAVE_WIN_NOW_SAVING)) != 0 && GsTrialIsTrial())
                    break;
                dm_save_mgr.Clear();
                dm_save_mgr_p = dm_save_mgr;
                dm_save_msg_flag = disp_flag;
                dm_save_first_save = is_first_save;
                dm_save_is_task_draw = is_task_draw;
                dm_save_is_snd_build = false;
                dmSaveInit();
                break;
        }
    }

    public static void DmSaveAttenMsgStart()
    {
        dm_save_mgr.Clear();
        dm_save_mgr_p = dm_save_mgr;
        dm_save_msg_flag = 1 << (int)DME_SAVE_WIN_ATTENTION_SAVE;
        dm_save_first_save = false;
        dm_save_is_task_draw = false;
        dm_save_is_snd_build = true;
        dmSaveInit();
    }

    public static void DmSaveMenuStart(bool is_task_draw, bool is_snd_build)
    {
        switch (SyGetEvtInfo().cur_evt_id)
        {
            case 6:
                break;
            case 9:
                break;
            case 11:
                break;
            default:
                if (!AoAccountIsCurrentEnable())
                    break;
                dmSaveSetSysDataForBackup();
                if (GsTrialIsTrial() || dmSaveIsSaveNecessary())
                    break;
                dm_save_mgr.Clear();
                dm_save_mgr_p = dm_save_mgr;
                dm_save_msg_flag = 1 << (int)DME_SAVE_WIN_NOW_SAVING;
                dm_save_first_save = false;
                dm_save_is_task_draw = is_task_draw;
                dm_save_is_snd_build = is_snd_build;
                dmSaveInit();
                break;
        }
    }

    public static bool DmSaveIsExit()
    {
        return dm_save_mgr_p == null || dm_save_mgr_p.tcb == null;
    }

    public static bool DmSaveIsDraw()
    {
        return dm_save_draw_reserve;
    }

    private static void dmSaveInit()
    {
        dm_save_variant = dmSaveIsConsoleDataComplete()
            ? dm_save_variant_console
            : dm_save_variant_mobile;

        dm_save_mgr_p.tcb = MTM_TASK_MAKE_TCB(
            new GSF_TASK_PROCEDURE(dmSaveProcMain),
            new GSF_TASK_PROCEDURE(dmSaveDest),
            0U,
            DMD_SAVE_TASK_PAUSELEVEL,
            DMD_SAVE_TASK_PRIO_MAIN,
            DMD_SAVE_TASK_GROUP_MAIN,
            () => new DMS_SAVE_MAIN_WORK(),
            "SAVE_TASK");

        DMS_SAVE_MAIN_WORK work = (DMS_SAVE_MAIN_WORK)dm_save_mgr_p.tcb.work;

        dm_save_disp_flag = 0U;
        dm_save_is_draw_state = 0U;
        dm_save_win_mode = 0;
        dm_save_draw_reserve = false;

        for (int index = 0; index < 2; ++index)
        {
            dm_save_win_size_rate[index] = 0.0f;
            dm_save_cmn_tex[index] = null;
        }

        for (int index = 0; index < ACT_COUNT; ++index)
            dm_save_act[index] = null;

        work.announce_flag = dm_save_msg_flag;
        work.draw_state = AoActSysGetDrawStateEnable() ? 1U : 0U;
        dm_save_draw_state = work.draw_state == 0U ? 0U : AoActSysGetDrawState();

        for (int index = 0; index < 2; ++index)
            dm_save_cmn_tex[index] = null;

        dm_save_is_draw_state = work.draw_state;
        work.proc_menu_update = new DMS_SAVE_MAIN_WORK._proc_(dmSaveLoadFontData);
    }

    private static void dmSaveSetSysDataForBackup()
    {
        GSS_MAIN_SYS_INFO mainSysInfo = GsGetMainSysInfo();
        SSystem instance = SSystem.CreateInstance();
        instance.Killed = mainSysInfo.ene_kill_count;
        instance.PlayerStock = mainSysInfo.rest_player_num;
    }

    private static bool dmSaveIsSaveNecessary()
    {
        return false;
    }

    private static void dmSaveProcMain(MTS_TASK_TCB tcb)
    {
        DMS_SAVE_MAIN_WORK work = (DMS_SAVE_MAIN_WORK)tcb.work;

        if ((work.flag & DMD_SAVE_FLAG_EXIT) != 0)
        {
            mtTaskClearTcb(tcb);
            dm_save_disp_flag = 0U;
            dm_save_is_draw_state = 0U;
            dm_save_win_mode = 0;
            dm_save_is_task_draw = false;

            for (int index = 0; index < 2; ++index)
                dm_save_win_size_rate[index] = 0.0f;

            dm_save_mgr_p = null;
        }

        if ((work.flag & DMD_SAVE_FLAG_SIGN_OUT_EXIT) != 0 && !AoAccountIsCurrentEnable())
        {
            work.proc_menu_update = new DMS_SAVE_MAIN_WORK._proc_(dmSaveProcStopDraw);
            work.proc_input = null;
            work.proc_draw = null;
            dm_save_draw_reserve = false;
            work.flag &= ~DMD_SAVE_FLAG_SIGN_OUT_EXIT;
        }
        else
        {
            if (work.proc_menu_update != null)
                work.proc_menu_update(work);

            if (work.proc_draw == null || AoSysMsgIsShow())
                return;

            work.proc_draw();
        }
    }

    private static void dmSaveDest(MTS_TASK_TCB tcb)
    {
    }
}
