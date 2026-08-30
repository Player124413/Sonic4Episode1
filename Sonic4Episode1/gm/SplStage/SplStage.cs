using System;

using static AppMain;

// the special stage session controller, gmSplStage.cpp. It loads no archive of its own: the ring
// and time overlay is SPST, which belongs to GmFix. The one #ifdef in the file is at :426 and every
// reachable arm of it is the same line. see docs/gmfix-port-notes.md
internal static partial class GmSplStage
{
    private static MTS_TASK_TCB gm_spl_tcb;

    private static ushort gmSplStageId()
    {
        return (ushort)(g_gs_main_sys_info.stage_id - GSD_MAIN_STAGE_ID_SS1);
    }

    public static void GmSplStageStart()
    {
        var stage = gmSplStageId();

        gm_spl_tcb = MTM_TASK_MAKE_TCB(
            new GSF_TASK_PROCEDURE(gmSplStageFadeInWait), null, 0U, 0,
            GMD_TASK_PRIO_MAIN_PRE + 0x050, GMD_TASK_GROUP_MAIN_PRE,
            () => new GMS_SPL_STG_WORK(), "SPL_STG_CTRL");

        var work = (GMS_SPL_STG_WORK)gm_spl_tcb.work;
        work.counter = 0U;
        work.light_vec.x = GMD_SPL_STG_BASE_LIGHT_POS_X;
        work.light_vec.y = GMD_SPL_STG_BASE_LIGHT_POS_Y;
        work.light_vec.z = GMD_SPL_STG_BASE_LIGHT_POS_Z;
        work.get_ring = 0;

        g_gm_main_system.game_flag &= ~(GMD_GAME_FLAG_SPL_CHAOSGET
                                        | GMD_GAME_FLAG_SPL_FAILED
                                        | GMD_GAME_FLAG_SPL_TIMEOVER);

        IzFadeInitEasy(1U, 3U, 8f);

        g_gm_main_system.game_time = gm_spl_stage_init_time[stage];
        g_gm_main_system.game_flag |= GMD_GAME_FLAG_START_DEMO;

        Array.Clear(gm_gmk_ss_switch, 0, gm_gmk_ss_switch.Length);
    }

    public static void GmSplStageExit()
    {
        if (gm_spl_tcb == null)
            return;

        mtTaskClearTcb(gm_spl_tcb);
        gm_spl_tcb = null;

        g_gm_main_system.game_flag &= ~(GMD_GAME_FLAG_SPL_CHAOSGET
                                        | GMD_GAME_FLAG_SPL_FAILED
                                        | GMD_GAME_FLAG_SPL_TIMEOVER);
    }

    public static void GmSplStageSetLight()
    {
        var col = new NNS_RGBA(1f, 1f, 1f, 1f);
        var light_vec = GlobalPool<NNS_VECTOR>.Alloc();

        light_vec.x = GMD_SPL_STG_OPT1_LIGHT_POS_X;
        light_vec.y = GMD_SPL_STG_OPT1_LIGHT_POS_Y;
        light_vec.z = GMD_SPL_STG_OPT1_LIGHT_POS_Z;

        nnNormalizeVector(light_vec, light_vec);
        ObjDrawSetParallelLight(NNE_LIGHT_1, ref col, 1f, light_vec);
    }

    public static GMS_SPL_STG_WORK GmSplStageGetWork()
    {
        return (GMS_SPL_STG_WORK)gm_spl_tcb.work;
    }

    public static void GmSplStageSwSet(uint sw_no)
    {
        sw_no &= GMD_SPL_STG_SW_MAX - 1;
        gm_gmk_ss_switch[(int)sw_no] = 1;
    }

    public static bool GmSplStageSwCheck(uint sw_no)
    {
        sw_no &= GMD_SPL_STG_SW_MAX - 1;
        return gm_gmk_ss_switch[(int)sw_no] != 0;
    }

    public static ushort GmSplStageRingGateNumGet(ushort gate_id)
    {
        return gm_spl_stage_ringgate_num[gmSplStageId()][gate_id];
    }

    private static void gmSplStageFadeInWait(MTS_TASK_TCB tcb)
    {
        var work = (GMS_SPL_STG_WORK)tcb.work;

        // a bare 30 in the C++ too, not GMD_SPL_STAGE_FADEIN_TIME; that one is the fade's duration
        ++work.counter;
        if (work.counter <= 30U)
            return;

        work.counter = 0U;
        mtTaskChangeTcbProcedure(tcb, new GSF_TASK_PROCEDURE(gmSplStageFadeInWait2));
        IzFadeInitEasy(0U, 2U, GMD_SPL_STAGE_FADEIN_TIME);
    }

    private static void gmSplStageFadeInWait2(MTS_TASK_TCB tcb)
    {
        if (ObjObjectPauseCheck(0U) != 0U || !IzFadeIsEnd())
            return;

        IzFadeExit();

        var ply_work = g_gm_main_system.ply_work[0];
        ply_work.obj_work.move_flag &= 4294958847U;
        ply_work.nudge_di_timer = 0;
        ply_work.nudge_timer = 0;
        ply_work.nudge_ofst_x = 0;

        g_gm_main_system.game_flag |= GMD_GAME_FLAG_COUNT_GAME_TIME;
        g_gm_main_system.game_flag &= ~GMD_GAME_FLAG_START_DEMO;

        if (GmStartMsgIsExe())
            GmStartMsgInit();

        AoPresenceSet();
        mtTaskChangeTcbProcedure(tcb, new GSF_TASK_PROCEDURE(gmSplStagePlayEndChk));
    }

    private static void gmSplStagePlayEndChk(MTS_TASK_TCB tcb)
    {
        var work = (GMS_SPL_STG_WORK)tcb.work;
        var ply_work = g_gm_main_system.ply_work[0];
        var obj_camera = ObjCameraGet(0);

        if (ObjObjectPauseCheck(0U) != 0U)
            return;

        gmSplStageLightCtrl(work);
        work.flag &= ~GMD_SPL_STAGE_BOUNCE_HIT;
        gmSplStageNudgeCtrl();
        gmSplStageRingGateChk(work);

        if ((g_gm_main_system.game_flag & (GMD_GAME_FLAG_SPL_CHAOSGET
                                           | GMD_GAME_FLAG_SPL_FAILED
                                           | GMD_GAME_FLAG_SPL_TIMEOVER)) == 0)
        {
            return;
        }

        g_gm_main_system.game_flag &= ~GMD_GAME_FLAG_COUNT_GAME_TIME;
        ply_work.obj_work.flag |= 130U;
        ply_work.obj_work.move_flag |= 8448U;
        ply_work.obj_work.disp_flag &= 4294967294U;
        ply_work.player_flag |= GMD_PLF_NOKEY;

        if ((g_gm_main_system.game_flag
             & (GMD_GAME_FLAG_SPL_FAILED | GMD_GAME_FLAG_SPL_TIMEOVER)) != 0)
        {
            GMM_PAD_VIB_MID_TIME(GMD_SPL_STAGE_ROLL_FADEWAIT);
        }

        work.roll = obj_camera.roll;
        work.roll_spd = GMD_SPL_STAGE_ROLL_SPD_INIT;
        work.counter = 0U;
        mtTaskChangeTcbProcedure(tcb, new GSF_TASK_PROCEDURE(gmSplStageRolling));
    }

    private static void gmSplStageRolling(MTS_TASK_TCB tcb)
    {
        var work = (GMS_SPL_STG_WORK)tcb.work;
        var obj_camera = ObjCameraGet(0);

        if (ObjObjectPauseCheck(0U) != 0U || obj_camera == null)
            return;

        work.roll_spd += GMD_SPL_STAGE_ROLL_SPD_ACL;
        work.roll += work.roll_spd;
        obj_camera.roll = work.roll;

        ++work.counter;
        if (work.counter == GMD_SPL_STAGE_ROLL_FADEWAIT)
            IzFadeInitEasy(0U, 3U, 30f);

        if (work.counter <= GMD_SPL_STAGE_ROLL_FADEWAIT || !IzFadeIsEnd())
            return;

        IzFadeExit();
        IzFadeRestoreDrawSetting();
        GmObjSetAllObjectNoDisp();
        GmRingGetWork().flag |= 1U;
        GmFix.GmFixSetDisp(false);

        work.flag |= GMD_SPL_STAGE_RESULT;
        work.counter = 1U;
        mtTaskChangeTcbProcedure(tcb, new GSF_TASK_PROCEDURE(gmSplStageGotoEnd));

        obj_camera.roll = 0;
        g_gm_main_system.pseudofall_dir = 0;
    }

    private static void gmSplStageGotoEnd(MTS_TASK_TCB tcb)
    {
        var work = (GMS_SPL_STG_WORK)tcb.work;

        if (work.counter != 0U)
        {
            --work.counter;
        }
        else
        {
            g_gm_main_system.game_flag |= GMD_GAME_FLAG_CLEAR;
            mtTaskChangeTcbProcedure(tcb, new GSF_TASK_PROCEDURE(gmSplStageEnd));
        }
    }

    private static void gmSplStageEnd(MTS_TASK_TCB tcb)
    {
        UNREFERENCED_PARAMETER(tcb);
    }

    // both lights turn with pseudofall_dir, which is what keeps them fixed relative to the screen
    // while the stage rotates under Sonic
    private static void gmSplStageLightCtrl(GMS_SPL_STG_WORK tcb_work)
    {
        var col = new NNS_RGBA(1f, 1f, 1f, 1f);

        UNREFERENCED_PARAMETER(tcb_work);

        var base_vec = gmSplStageLightRot(
            GMD_SPL_STG_BASE_LIGHT_POS_X,
            GMD_SPL_STG_BASE_LIGHT_POS_Y,
            GMD_SPL_STG_BASE_LIGHT_POS_Z);

        nnNormalizeVector(base_vec, base_vec);
        ObjDrawSetParallelLight(NNE_LIGHT_0, ref col, 1f, base_vec);

        var opt_vec = gmSplStageLightRot(
            GMD_SPL_STG_OPT1_LIGHT_POS_X,
            GMD_SPL_STG_OPT1_LIGHT_POS_Y,
            GMD_SPL_STG_OPT1_LIGHT_POS_Z);

        nnNormalizeVector(opt_vec, opt_vec);
        ObjDrawSetParallelLight(NNE_LIGHT_1, ref col, 1f, opt_vec);
    }

    private static NNS_VECTOR gmSplStageLightRot(float pos_x, float pos_y, float pos_z)
    {
        var rotated = GlobalPool<NNS_VECTOR>.Alloc();

        var x_sin = pos_x * nnSin(-g_gm_main_system.pseudofall_dir);
        var x_cos = pos_x * nnCos(-g_gm_main_system.pseudofall_dir);
        var y_sin = pos_y * nnSin(-g_gm_main_system.pseudofall_dir);
        var y_cos = pos_y * nnCos(-g_gm_main_system.pseudofall_dir);

        rotated.x = x_cos - y_sin;
        rotated.y = x_sin + y_cos;
        rotated.z = pos_z;

        return rotated;
    }

    private static void gmSplStageNudgeCtrl()
    {
        var ply_work = g_gm_main_system.ply_work[0];
        var obj_camera = ObjCameraGet(0);

        if (ply_work.nudge_timer != 0)
        {
            var dest_x = (ply_work.nudge_timer * GMD_PL_SS_NUDGING_WIDTH << FX32_SHIFT)
                         / GMD_PL_SS_NUDGING_TIME;

            if ((ply_work.nudge_timer & 2) != 0)
                dest_x = -dest_x;

            var dest_y = 0;

            // the C++ offsets Sonic's display position rather than his real one; the alternative
            // is in the file behind #if 0, with a note that it breaks collision
            GmObjGetRotPosXY(
                dest_x, dest_y,
                ref ply_work.obj_work.ofst.x, ref ply_work.obj_work.ofst.y,
                g_gm_main_system.pseudofall_dir);

            GmObjGetRotPosXY(
                dest_x, dest_y, ref dest_x, ref dest_y,
                (ushort)-g_gm_main_system.pseudofall_dir);

            obj_camera.ofst.x = FXM_FX32_TO_FLOAT(dest_x);
            obj_camera.ofst.y = FXM_FX32_TO_FLOAT(dest_y);

            --ply_work.nudge_timer;
        }
        else
        {
            obj_camera.ofst.x = 0.0f;
            obj_camera.ofst.y = 0.0f;
            ply_work.obj_work.ofst.x = 0;
            ply_work.obj_work.ofst.y = 0;
            ply_work.nudge_ofst_x = 0;
        }
    }

    private static void gmSplStageRingGateChk(GMS_SPL_STG_WORK tcb_work)
    {
        var ply_work = g_gm_main_system.ply_work[0];
        var was = tcb_work.get_ring;

        for (ushort gate_id = 0; gate_id < GMD_SS_RINGGATE_MAX; ++gate_id)
        {
            if ((tcb_work.get_ring & 1 << gate_id) != 0)
                continue;

            var needed = GmSplStageRingGateNumGet(gate_id);
            if (needed == GMD_SS_RINGGATE_NONE)
                break;

            if (needed <= (ushort)ply_work.ring_num)
                tcb_work.get_ring |= (ushort)(1U << gate_id);
        }

        if (was == tcb_work.get_ring)
            return;

        GmSoundPlaySE("Special7");
    }
}
