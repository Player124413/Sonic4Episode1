using static AppMain;

using static GmFix.GME_FIX_PART_TYPE;
using static GmFix.GME_FIX_PLAN;
using static GmFix.GME_FIX_TEX_IDX;

internal static partial class GmFix
{
    private const int TEX_NUM = (int)GME_FIX_TEX_IDX_MAX;
    private const int PART_NUM = (int)GME_FIX_PART_TYPE_MAX;

    // Everything gmFix.cpp puts behind an #ifdef, picked once in GmFixBuildDataInit. The decompile
    // only ever carried the mobile arm, so the console side of each of these is new.
    // see docs/gmfix-port-notes.md
    internal sealed class GMS_FIX_VARIANT
    {
        // the transposed 29/30 pair
        public int[] timer_timeattack_act_id_tbl;

        // console declares these flat where mobile indexes them by plan
        public int[] score_stage22_act_id_tbl;
        public int[] challenge_act_id_tbl;

        // and therefore whether there is a plan at all: GME_FIX_PLAN is mobile-only
        public bool has_virtual_pad;

        public bool timer_timeattack_x_offset;
        public bool score_narrowscreen_offset;

        public bool is_console;
    }

    private static GMS_FIX_VARIANT gm_fix_variant = gm_fix_variant_mobile;

    private static MTS_TASK_TCB gm_fix_tcb;
    private static readonly AMS_AMB_HEADER[] gm_fix_texamb_list = new AMS_AMB_HEADER[TEX_NUM];
    private static readonly AOS_TEXTURE[] gm_fix_textures = New<AOS_TEXTURE>(TEX_NUM);

    // slot 2 is the SPST_<lang> member, the only one that can sit past the end of the archive
    private static int gmFixLanguage()
    {
        return GmGameDatCockpitLanguage(gm_fix_tex_amb_idx_tbl, 2);
    }

    // G_FIX and SPST move together or not at all
    private static bool gmFixIsConsoleDataComplete()
    {
        var lang = gmFixLanguage();

        return amFsAllMemberVariant(
            AMD_FS_VARIANT_CONSOLE,
            GMD_FIX_DATA_FILENAME,
            gm_fix_ama_amb_idx_tbl[lang][0],
            gm_fix_tex_amb_idx_tbl[lang][0],
            gm_fix_ama_amb_idx_tbl[lang][1],
            gm_fix_tex_amb_idx_tbl[lang][1],
            gm_fix_ama_amb_idx_tbl[lang][2],
            gm_fix_tex_amb_idx_tbl[lang][2]);
    }

    public static void GmFixBuildDataInit()
    {
        gm_fix_variant = gmFixIsConsoleDataComplete() ? gm_fix_variant_console : gm_fix_variant_mobile;

        for (int i = 0; i < TEX_NUM; i++)
            gm_fix_textures[i].Clear();

        var lang = gmFixLanguage();

        for (int i = 0; i < TEX_NUM; i++)
        {
            gm_fix_texamb_list[i] = (AMS_AMB_HEADER)ObjDataLoadAmbIndex(
                null, gm_fix_tex_amb_idx_tbl[lang][i], GmGameDatGetCockpitData());

            AoTexBuild(gm_fix_textures[i], gm_fix_texamb_list[i]);
            AoTexLoad(gm_fix_textures[i]);
        }
    }

    public static bool GmFixBuildDataLoop()
    {
        for (int i = 0; i < TEX_NUM; i++)
        {
            if (!AoTexIsLoaded(gm_fix_textures[i]))
                return false;
        }

        return true;
    }

    public static void GmFixFlushDataInit()
    {
        for (int i = 0; i < TEX_NUM; i++)
            AoTexRelease(gm_fix_textures[i]);
    }

    public static bool GmFixFlushDataLoop()
    {
        var done = true;

        for (int i = 0; i < TEX_NUM; i++)
        {
            if (gm_fix_texamb_list[i] == null)
                continue;

            if (!AoTexIsReleased(gm_fix_textures[i]))
            {
                done = false;
                continue;
            }

            gm_fix_texamb_list[i] = null;
            gm_fix_textures[i].Clear();
        }

        return done;
    }

    public static void GmFixInit()
    {
        gm_fix_tcb = MTM_TASK_MAKE_TCB(
            new GSF_TASK_PROCEDURE(gmFixProcMain),
            new GSF_TASK_PROCEDURE(gmFixDest),
            0U,
            GMD_FIX_TASK_PAUSELEVEL,
            GMD_TASK_PRIO_FIX,
            GMD_TASK_GROUP_FIX,
            () => new GMS_FIX_MGR_WORK(),
            "GM_FIX_MGR");

        var mgr_work = (GMS_FIX_MGR_WORK)gm_fix_tcb.work;
        mgr_work.Clear();

        var init_func_tbl = gmFixPartInitFuncTbl(mgr_work);

        for (int i = 0; i < PART_NUM; i++)
            init_func_tbl[i]?.Invoke(mgr_work);
    }

    private static GMF_FIX_PART_INIT_FUNC[] gmFixPartInitFuncTbl(GMS_FIX_MGR_WORK mgr_work)
    {
        if (gmFixIsSpecialStage())
        {
            mgr_work.flag |= GMD_FIX_MGR_FLAG_TIMER_COUNTDOWN;
            return gm_fix_ss_part_init_func_tbl;
        }

        if (!gmFixIsTimeAttack())
            return gm_fix_part_init_func_tbl;

        return gmFixIsStage22() ? gm_fix_ta_s22_part_init_func_tbl : gm_fix_ta_part_init_func_tbl;
    }

    public static void GmFixExit()
    {
        if (gm_fix_tcb == null)
            return;

        mtTaskClearTcb(gm_fix_tcb);
        gm_fix_tcb = null;
    }

    // console has no per-pad-part flags to set, so it sets HIDE and nothing else
    public static void GmFixSetDisp(bool enable)
    {
        if (!gm_fix_variant.has_virtual_pad)
        {
            var mgr_work = gmFixMgrWork();
            if (mgr_work != null)
                gmFixSetHide(mgr_work, GMD_FIX_MGR_FLAG_HIDE, enable);

            return;
        }

        GmFixSetDispEx(enable, enable, enable, enable, enable);
    }

    public static void GmFixSetDispEx(
        bool enable, bool enable_ss, bool enable_pause, bool enable_action, bool enable_move)
    {
        var mgr_work = gmFixMgrWork();
        if (mgr_work == null)
            return;

        gmFixSetHide(mgr_work, GMD_FIX_MGR_FLAG_HIDE, enable);
        gmFixSetHide(mgr_work, GMD_FIX_MGR_FLAG_HIDE_VIRTUAL_PAD_PART_SUPER_SONIC, enable_ss);
        gmFixSetHide(mgr_work, GMD_FIX_MGR_FLAG_HIDE_VIRTUAL_PAD_PART_PAUSE, enable_pause);
        gmFixSetHide(mgr_work, GMD_FIX_MGR_FLAG_HIDE_VIRTUAL_PAD_PART_ACTION, enable_action);
        gmFixSetHide(mgr_work, GMD_FIX_MGR_FLAG_HIDE_VIRTUAL_PAD_PART_MOVE_PAD, enable_move);
    }

    private static void gmFixSetHide(GMS_FIX_MGR_WORK mgr_work, uint flag, bool enable)
    {
        if (enable)
            mgr_work.flag &= ~flag;
        else
            mgr_work.flag |= flag;
    }

    public static void GmFixRequestTimerFlash()
    {
        var mgr_work = gmFixMgrWork();
        if (mgr_work == null)
            return;

        mgr_work.req_flag |= GMD_FIX_MGR_REQUEST_FLAG_TIMER_FLASH_ACT;
    }

    private static GMS_FIX_MGR_WORK gmFixMgrWork()
    {
        return gm_fix_tcb == null ? null : (GMS_FIX_MGR_WORK)gm_fix_tcb.work;
    }

    private static bool gmFixIsHidden(uint flag)
    {
        var mgr_work = gmFixMgrWork();
        return mgr_work == null || (mgr_work.flag & flag) != 0;
    }

    private static void gmFixSubpartOutFunc(OBS_OBJECT_WORK obj_work)
    {
        if (gmFixIsHidden(GMD_FIX_MGR_FLAG_HIDE))
            return;

        ObjDrawActionSummary(obj_work);
    }

    // the C++ has one template instantiated per flag; the decompile made four one-method classes
    private static MPP_VOID_OBS_OBJECT_WORK gmFixVirtualPadOutFunc(uint flag)
    {
        return obj_work =>
        {
            if (gmFixIsHidden(flag))
                return;

            ObjDrawActionSummary(obj_work);
        };
    }

    private static void gmFixSetFrameStatic(OBS_OBJECT_WORK obj_work, float frame)
    {
        obj_work.obj_2d.frame = frame;
        obj_work.obj_2d.speed = 0.0f;
    }

    private static void gmFixUpdatePart(GMS_FIX_PART_WORK part_work)
    {
        if ((part_work.flag & GMD_FIX_PART_FLAG_ACTIVE) == 0)
            return;

        if (gmFixCheckClear(part_work))
        {
            gmFixUnregisterPart(part_work);
            return;
        }

        part_work.proc_update?.Invoke(part_work);

        if ((part_work.flag & GMD_FIX_PART_FLAG_NODISP) == 0)
            part_work.proc_disp?.Invoke(part_work);
    }

    private static bool gmFixCheckClear(GMS_FIX_PART_WORK part_work)
    {
        return gm_fix_tcb == null
            || part_work.parent_mgr == null
            || (part_work.parent_mgr.flag & GMD_FIX_MGR_FLAG_CLEAR) != 0;
    }

    private static bool gmFixIsSpecialStage()
    {
        switch (GsGetMainSysInfo().stage_id)
        {
            case 21:
            case 22:
            case 23:
            case 24:
            case 25:
            case 26:
            case 27:
                return true;

            default:
                return false;
        }
    }

    private static bool gmFixIsTimeAttack()
    {
        return GsGetMainSysInfo().game_mode == GSD_GAME_MODE_TIME_ATTACK;
    }

    // named for the act, not the stage id: act 2-2 is stage 5
    private static bool gmFixIsStage22()
    {
        return GsGetMainSysInfo().stage_id == 5;
    }

    private static bool gmFixIsStageTruck()
    {
        return GsGetMainSysInfo().stage_id == 9;
    }

    // plans 1 and 2 are the mobile layouts and both reach ids 45+, which console G_FIX does not
    // have. INPUT_CLASSIC selects plan 2 and StgSlct sets it on console, so this cannot be left to
    // the flags. see docs/gmfix-port-notes.md
    // mobile-only: the console build has no GME_FIX_PLAN and no table indexed by one. Only the
    // virtual pad reaches this, and that part does not exist on console either.
    private static GME_FIX_PLAN gmFixGetPlan()
    {
        return GmFixPlanFor(
            gmFixIsStageTruck() || gmFixIsSpecialStage(),
            GsGetMainSysInfo().game_flag);
    }

    internal static GME_FIX_PLAN GmFixPlanFor(bool flick_stage, uint game_flag)
    {
        if (flick_stage)
        {
            return (game_flag & GSD_MAINSYS_GAME_FLAG_INPUT_IS_FLICK) != 0
                ? GME_FIX_PLAN_FLICK
                : GME_FIX_PLAN_TILT;
        }

        return (game_flag & GSD_MAINSYS_GAME_FLAG_INPUT_CLASSIC) != 0
            ? GME_FIX_PLAN_VIRTUAL_PAD
            : GME_FIX_PLAN_TILT;
    }

    private static short gmFixGetRingNum()
    {
        var ply_work = g_gm_main_system.ply_work[0];
        return ply_work != null ? ply_work.ring_num : (short)0;
    }

    private static uint gmFixGetScore()
    {
        var ply_work = g_gm_main_system.ply_work[0];
        return ply_work != null ? ply_work.score : 0U;
    }

    private static uint gmFixGetGameTime()
    {
        return g_gm_main_system.game_time >= GMD_FIX_GAME_TIME_MAX
            ? GMD_FIX_GAME_TIME_MAX
            : g_gm_main_system.game_time;
    }

    private static uint gmFixGetChallengeNum()
    {
        uint rest = 0;
        if (g_gm_main_system.player_rest_num[0] > 0U)
            rest = g_gm_main_system.player_rest_num[0] - 1U;

        return (uint)MTM_MATH_CLIP((int)rest, 0, GMD_FIX_CHALLENGE_NUM_MAX);
    }

    private static void gmFixInitBlink(GMS_FIX_PART_WORK part_work, uint on_time, uint off_time)
    {
        part_work.blink_timer = on_time + off_time;
        part_work.blink_on_time = on_time;
        part_work.blink_off_time = off_time;
    }

    private static bool gmFixUpdateBlink(GMS_FIX_PART_WORK part_work)
    {
        if (part_work.blink_timer != 0U)
            part_work.blink_timer--;

        var on = part_work.blink_timer >= part_work.blink_off_time;

        if (part_work.blink_timer == 0U)
            part_work.blink_timer = part_work.blink_on_time + part_work.blink_off_time;

        return on;
    }

    private static void gmFixDest(MTS_TASK_TCB tcb)
    {
    }

    private static void gmFixProcMain(MTS_TASK_TCB tcb)
    {
        var mgr_work = (GMS_FIX_MGR_WORK)tcb.work;

        for (int i = 0; i < PART_NUM; i++)
        {
            if (mgr_work.part_work[i] != null)
                gmFixUpdatePart(mgr_work.part_work[i]);
        }
    }

    private static void gmFixRegisterPart(
        GMS_FIX_MGR_WORK mgr_work, GMS_FIX_PART_WORK part_work, GME_FIX_PART_TYPE part_type)
    {
        mgr_work.part_work[(int)part_type] = part_work;
        part_work.part_type = part_type;
        part_work.parent_mgr = mgr_work;
        part_work.flag |= GMD_FIX_PART_FLAG_ACTIVE;
    }

    private static void gmFixUnregisterPart(GMS_FIX_PART_WORK part_work)
    {
        if (part_work.parent_mgr != null)
            part_work.parent_mgr.part_work[(int)part_work.part_type] = null;

        part_work.part_type = GME_FIX_PART_TYPE_NONE;
        part_work.parent_mgr = null;
        part_work.flag &= ~GMD_FIX_PART_FLAG_ACTIVE;
    }

    private static bool gmFixProcessRequest(GMS_FIX_MGR_WORK mgr_work, uint req_flag_bit)
    {
        if ((mgr_work.req_flag & req_flag_bit) == 0)
            return false;

        mgr_work.req_flag &= ~req_flag_bit;
        return true;
    }

    // every subpart is built the same way: a cockpit 2D work bound to one G_FIX action, frozen on
    // frame 0, drawn through an out-func that consults the manager's hide flags
    private static GMS_COCKPIT_2D_WORK gmFixCreateSubpart(
        string name, int act_id, MPP_VOID_OBS_OBJECT_WORK out_func,
        int ama_slot = (int)GME_FIX_TEX_IDX_FIX, int tex_slot = (int)GME_FIX_TEX_IDX_FIX)
    {
        var ama_id = gm_fix_ama_amb_idx_tbl[gmFixLanguage()][ama_slot];

        // a variant that names an action its archive does not have would otherwise store a null
        // act and fall over frames later inside ObjDrawAction2DAMA
        gmFixAssertActionExists(name, ama_id, act_id);

        var obj_work = GmCockpit.GMM_COCKPIT_CREATE_WORK(() => new GMS_COCKPIT_2D_WORK(), null, 0, name);
        var subpart = (GMS_COCKPIT_2D_WORK)obj_work;

        obj_work.ppOut = out_func;

        ObjObjectAction2dAMALoadSetTexlist(
            obj_work, subpart.obj_2d, null, null,
            ama_id,
            GmGameDatGetCockpitData(),
            AoTexGetTexList(gm_fix_textures[tex_slot]),
            (uint)act_id, 0);

        gmFixSetFrameStatic(obj_work, 0.0f);
        return subpart;
    }

    private static void gmFixAssertActionExists(string name, int ama_id, int act_id)
    {
        var ama = readAMAFile(ObjDataLoadAmbIndex(null, ama_id, GmGameDatGetCockpitData()));
        if (ama != null && act_id < ama.act_num)
            return;

        var act_num = ama == null ? 0 : (int)ama.act_num;
        throw new System.InvalidOperationException(
            $"{name}: action {act_id} is not in CPIT_MAIN member {ama_id}, which has {act_num}. "
            + $"variant is {(gm_fix_variant.is_console ? "console" : "mobile")}");
    }
}
