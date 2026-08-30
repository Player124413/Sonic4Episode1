using static AppMain;

internal static partial class StgSlct
{
    // everything that differs between the two builds, picked once in dmStgSlctInit. the act enum
    // and its id table are index-locked to each other, so they only ever travel as a pair.
    // which loaded archive each action is created from. The C++ decides this per action with a
    // cascade of index comparisons whose thresholds differ per variant; resolving it once into a
    // table means dmStgSlctProcCreateAct stops caring which build it is running.
    public const int DMD_STGSLCT_SLOT_CMN_DATA = 0;
    public const int DMD_STGSLCT_SLOT_LANG_DATA = 1;
    public const int DMD_STGSLCT_SLOT_CMN_BG = 2;
    public const int DMD_STGSLCT_SLOT_CMN_BTN = 3;
    public const int DMD_STGSLCT_SLOT_CMN_OBI = 4;
    public const int DMD_STGSLCT_SLOT_CMN_WIN = 5;
    public const int DMD_STGSLCT_SLOT_CMN_MSG = 6;

    internal sealed class DMS_STGSLCT_VARIANT
    {
        public uint[] act_id_tbl;
        public int[] act_slot_tbl;
        public int act_num;

        public float[][][] zone_disp_pos_tbl;
        public float[][][] zone_nodisp_pos_tbl;
        public float[] act_disp_y_pos_tbl;
        public float[] act_tab_disp_y_pos_tbl;
        public float[][] win_act_pos_tbl;

        public int draw_prio_win;
        public float act_table_top_pos_x;
        public float act_table_dist_y;
        public float window_size_w;

        // The two ACT enums share member names but not values. Shared code names actions with the
        // console enum, which is the superset for everything shared code touches, and this maps
        // that name onto whichever index the running variant uses. -1 for an action the variant
        // does not have, which throws on use rather than quietly indexing the wrong sprite.
        public int[] act_index;

        public int act_tab_table2;
        public int act_tab_table3;
        public int act_tab_scr;
        public int act_tab_mess;

        // the three input procs the C++ forks whole
        public DMS_STGSLCT_MAIN_WORK._proc_ input_zone_select;
        public DMS_STGSLCT_MAIN_WORK._proc_ input_stage_select;
        public DMS_STGSLCT_MAIN_WORK._proc_ input_win_disp_idle;

        public bool is_console;

        // what every archive this screen loads gets pinned to. the global am_fs_variant cannot be
        // trusted here: it stays "console" even when this screen has fallen back to mobile, and a
        // shared archive like D_CMN_WIN would then arrive with ids the mobile tables cannot index.
        public string fs_variant;
    }

    private static DMS_STGSLCT_VARIANT dm_stgslct_variant;

    // shared draw code names actions with the console enum and goes through these
    private static int dmStgSlctActIdx(DME_STGSLCT_ACT_REG key)
    {
        return dm_stgslct_variant.act_index[(int)key];
    }

    private static AOS_ACTION dmStgSlctAct(DMS_STGSLCT_MAIN_WORK main_work, DME_STGSLCT_ACT_REG key)
    {
        return main_work.act[dm_stgslct_variant.act_index[(int)key]];
    }

    // the six archives this screen loads. D_CMN_BG/BTN/OBI have identical ids on both sides, but a
    // partial variant set still falls back wholesale: mixing two builds' art is not worth the risk
    // of also mixing their ids one day.
    private static bool dmStgSlctIsConsoleDataComplete()
    {
        var lng = GsEnvGetLanguage();

        return amFsAllVariant(
            AMD_FS_VARIANT_CONSOLE,
            DMD_STGSLCT_DATA_FILENAME,
            dm_stgslct_main_lng_amb_name_tbl[lng],
            dm_stgslct_menu_cmn_lng_amb_name_tbl[lng],
            dm_stgslct_menu_cmn_amb_name_tbl[0],
            dm_stgslct_menu_cmn_amb_name_tbl[1],
            dm_stgslct_menu_cmn_amb_name_tbl[2],
            dm_stgslct_menu_cmn_amb_name_tbl[3]);
    }

    internal static DMS_STGSLCT_VARIANT StgSlctVariantForTest(bool console)
    {
        return console ? dm_stgslct_variant_console : dm_stgslct_variant_mobile;
    }

    internal static int RevisedZoneNoForTest(int idx, int diff, int is_final_open, int is_spe_open)
    {
        return dmStgSlctGetRevisedZoneNo(idx, diff, is_final_open, is_spe_open);
    }

    private static bool dm_stgslct_is_stage_start;

    public static void DmStgSlctStart(object arg)
    {
        dmStgSlctInit();
    }

    private static void dmStgSlctInit()
    {
        AoActSysSetDrawStateEnable(false);

        var main_work = (DMS_STGSLCT_MAIN_WORK)MTM_TASK_MAKE_TCB(
            new GSF_TASK_PROCEDURE(dmStgSlctProcMain),
            new GSF_TASK_PROCEDURE(dmStgSlctDest),
            0U,
            DMD_STGSLCT_TASK_PAUSELEVEL,
            DMD_STGSLCT_TASK_PRIO_MAIN,
            DMD_STGSLCT_TASK_GROUP_MAIN,
            () => new DMS_STGSLCT_MAIN_WORK(),
            "STGSLCT_MAIN").work;

        dm_stgslct_variant = dmStgSlctIsConsoleDataComplete()
            ? dm_stgslct_variant_console
            : dm_stgslct_variant_mobile;

        main_work.act = new AOS_ACTION[dm_stgslct_variant.act_num];

        if (!dm_stgslct_variant.is_console)
            main_work.touch = new StgSlctTouch();

        main_work.is_jp_region = GeEnvGetDecideKey() == GSE_DECIDE_KEY.GSD_DECIDE_KEY_O;

        dmStgSlctSetInitData(main_work);
        dmStgSlctSetHiScore(main_work);
        dmStgSlctSetClearInfo(main_work);
        dmStgSlctSetAnnounceMsg(main_work);

        main_work.tex_u[0] = main_work.tex_u[1] =
        main_work.tex_v[0] = main_work.tex_v[1] = 0.1f;

        main_work.proc_menu_update = dmStgSlctLoadFontData;
    }

    private static void dmStgSlctProcMain(MTS_TASK_TCB tcb)
    {
        var main_work = (DMS_STGSLCT_MAIN_WORK)tcb.work;

        if ((main_work.flag & DMD_STGSLCT_FLAG_EXIT) != 0)
        {
            mtTaskClearTcb(tcb);
            dmStgSlctSetNextEvt(main_work);
        }

        if ((main_work.flag & DMD_STGSLCT_FLAG_SIGN_OUT_EXIT) == DMD_STGSLCT_FLAG_SIGN_OUT_EXIT
            && !AoAccountIsCurrentEnable())
        {
            main_work.proc_menu_update = dmStgSlctProcFadeOut;

            main_work.flag &= ~DMD_STGSLCT_FLAG_SIGN_OUT_EXIT;
            main_work.next_evt = (int)DME_STGSLCT_NEXT_EVT.DME_STGSLCT_NEXT_EVT_TITLE;

            IzFadeInitEasy(
                IZE_FADE_SET_TYPE_TAKEOEVER,
                IZE_FADE_TYPE_BLACK_FADEOUT,
                DMD_STGSLCT_FADEOUT_TIME);

            DmSndBgmPlayerExit();
            main_work.flag |= DMD_STGSLCT_FLAG_DEMO_SND_END;

            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_DECIDE);
            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_CANCEL);
            main_work.proc_input = null;
            main_work.proc_win_input = null;
            main_work.win_timer = 0;
            main_work.win_cur_slct = 0;
            main_work.win_mode = (int)DME_STGSLCT_WIN.DME_STGSLCT_WIN_STG_SLCT;

            if (main_work.touch != null)
                main_work.touch.win_is_disp_cover = false;
        }

        dmStgSlctSetBgFadeEfct(main_work);

        main_work.proc_win_update?.Invoke(main_work);
        main_work.proc_menu_update?.Invoke(main_work);
        main_work.proc_draw?.Invoke(main_work);
    }

    private static void dmStgSlctDest(MTS_TASK_TCB tcb)
    {
    }

    private static void dmStgSlctSetNextEvt(DMS_STGSLCT_MAIN_WORK main_work)
    {
        var set_play_act = (ushort)main_work.cur_stage;

        switch (main_work.next_evt)
        {
            case (int)DME_STGSLCT_NEXT_EVT.DME_STGSLCT_NEXT_EVT_MAINGAME:
                g_gs_main_sys_info.stage_id = set_play_act;
                g_gs_main_sys_info.prev_stage_id = 0xffff;
                g_gs_main_sys_info.char_id[0] = GSD_CHAR_ID_SONIC;
                g_gs_main_sys_info.game_mode = (int)main_work.cur_game_mode;

                dmStgSlctSetChaosEmeraldFlag(main_work);
                dmStgSlctSetInputFlag();

                g_gs_main_sys_info.game_flag &= ~GSD_MAINSYS_GAME_FLAG_SPECIAL_STAGE;

                GmMainGSInit();
                break;

            case (int)DME_STGSLCT_NEXT_EVT.DME_STGSLCT_NEXT_EVT_SPESTE:
                g_gs_main_sys_info.stage_id = (ushort)(set_play_act + 4);
                g_gs_main_sys_info.prev_stage_id = 0xffff;

                dmStgSlctSetChaosEmeraldFlag(main_work);

                g_gs_main_sys_info.game_mode = (int)main_work.cur_game_mode;

                dmStgSlctSetInputFlag();

                g_gs_main_sys_info.game_flag &= ~GSD_MAINSYS_GAME_FLAG_SPECIAL_STAGE;
                break;

            case (int)DME_STGSLCT_NEXT_EVT.DME_STGSLCT_NEXT_EVT_RANKING:
                dm_stgslct_is_stage_start = true;

                if (main_work.cur_zone == (uint)DME_STGSLCT_ZONE_TYPE.DME_STGSLCT_ZONE_TYPE_SPE)
                    g_gs_main_sys_info.stage_id = (ushort)(set_play_act + 4);
                else
                    g_gs_main_sys_info.stage_id = set_play_act;

                g_gs_main_sys_info.prev_stage_id = 0xffff;

                g_gs_main_sys_info.game_mode =
                    main_work.cur_game_mode == (uint)DME_STGSLCT_PLAY_MODE.DME_STGSLCT_PLAY_MODE_NORMAL
                        ? (int)GSD_GAME_MODE_STORY
                        : (int)GSD_GAME_MODE_TIME_ATTACK;
                break;
        }

        SyDecideEvtCase((short)main_work.next_evt);
        SyChangeNextEvt();
    }

    private static void dmStgSlctSetChaosEmeraldFlag(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (main_work.is_clear_stage[(int)DME_STGSLCT_STAGE.DME_STGSLCT_STAGE_S_7] == 1)
            g_gs_main_sys_info.game_flag |= GSD_MAINSYS_GAME_FLAG_7_CHAOS_EMERALD;
        else
            g_gs_main_sys_info.game_flag &= ~GSD_MAINSYS_GAME_FLAG_7_CHAOS_EMERALD;
    }

    private static void dmStgSlctSetInputFlag()
    {
        if (dm_stgslct_variant.is_console)
            g_gs_main_sys_info.game_flag |= GSD_MAINSYS_GAME_FLAG_INPUT_CLASSIC;
    }
}
