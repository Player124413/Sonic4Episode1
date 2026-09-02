using System;

internal partial class AppMain
{
    // The timed card platforms. PC gmGmkTrumpStageInit 0x4F9600, event ids 308-311 and 314-317,
    // ours 384-391. Behaviour, work layout and provenance are in docs/z2a2-trumpstage-re.md.
    //
    // A card flips face-up to face-away and back on the global gameplay clock, staggered against
    // its neighbours by the event's left byte, and is landable for most of the face-up half of
    // that. Cards with flag bit 2 instead wait for the player to stand on them and then drop away
    // once, for good.

    internal const int GMD_GMK_TRUMPSTAGE_SUIT_MAX = 8;

    // members of GMK_TRUMPSTAGE_MTN. 0's frame 0 is the face-up pose and 1's is the face-away
    // pose, which is why the two dwell phases park on the flip that starts where they want to sit
    internal const int GMD_GMK_TRUMPSTAGE_MTN_FLIP_AWAY = 0;
    internal const int GMD_GMK_TRUMPSTAGE_MTN_FLIP_BACK = 1;
    internal const int GMD_GMK_TRUMPSTAGE_MTN_SP = 2;

    internal const int GMD_GMK_TRUMPSTAGE_PHASE_NONE = -1;
    internal const int GMD_GMK_TRUMPSTAGE_PHASE_IDLE_UP = 0;
    internal const int GMD_GMK_TRUMPSTAGE_PHASE_FLIP_AWAY = 1;
    internal const int GMD_GMK_TRUMPSTAGE_PHASE_IDLE_DOWN = 2;
    internal const int GMD_GMK_TRUMPSTAGE_PHASE_FLIP_BACK = 3;
    internal const int GMD_GMK_TRUMPSTAGE_PHASE_SP = 4;

    internal const ushort GMD_GMK_TRUMPSTAGE_FLAG_PERIOD = 3;
    internal const ushort GMD_GMK_TRUMPSTAGE_FLAG_TRIGGERED = 4;
    internal const ushort GMD_GMK_TRUMPSTAGE_FLAG_FAST = 8;

    internal const int GMD_GMK_TRUMPSTAGE_STAGGER_MAX = 31;

    internal const ushort GMD_GMK_TRUMPSTAGE_PLATFORM_W = 64;
    internal const ushort GMD_GMK_TRUMPSTAGE_PLATFORM_H = 17;

    // both parameter sets hand the flip over to the second rate at this frame, 60 * 0.1 and
    // 4 * 1.5 alike
    internal const float GMD_GMK_TRUMPSTAGE_FLIP_HANDOVER_FRAME = 6.0f;

    internal const int GMD_GMK_TRUMPSTAGE_TRIGGER_WAIT = 60;
    internal const int GMD_GMK_TRUMPSTAGE_TRIGGER_WAIT_FAST = 25;

    public static void GmGmkTrumpStageBuild()
    {
        AMS_AMB_HEADER tex = readAMBFile(GmGameDatGetGimmickData(GMD_DWORK_NO_GMK_TRUMPSTAGE_TEX));

        g_gm_gmk_trumpstage_obj_3d_list = new OBS_ACTION3D_NN_WORK[GMD_GMK_TRUMPSTAGE_SUIT_MAX][];

        for (int suit = 0; suit < GMD_GMK_TRUMPSTAGE_SUIT_MAX; ++suit)
        {
            AMS_AMB_HEADER mdl = readAMBFile(
                GmGameDatGetGimmickData(GMD_DWORK_NO_GMK_TRUMPSTAGE_S_MDL + suit));

            g_gm_gmk_trumpstage_obj_3d_list[suit] = GmGameDBuildRegBuildModel(mdl, tex, 0U);
        }
    }

    public static void GmGmkTrumpStageFlush()
    {
        for (int suit = 0; suit < GMD_GMK_TRUMPSTAGE_SUIT_MAX; ++suit)
        {
            AMS_AMB_HEADER mdl = readAMBFile(
                GmGameDatGetGimmickData(GMD_DWORK_NO_GMK_TRUMPSTAGE_S_MDL + suit));

            GmGameDBuildRegFlushModel(g_gm_gmk_trumpstage_obj_3d_list[suit], mdl.file_num);
        }

        g_gm_gmk_trumpstage_obj_3d_list = null;
    }

    private static OBS_OBJECT_WORK GmGmkTrumpStageInit(
      GMS_EVE_RECORD_EVENT eve_rec,
      int pos_x,
      int pos_y,
      byte type)
    {
        UNREFERENCED_PARAMETER(type);

        GMS_GMK_TRUMPSTAGE_WORK work = (GMS_GMK_TRUMPSTAGE_WORK)GMM_ENEMY_CREATE_WORK(
            eve_rec, pos_x, pos_y, () => new GMS_GMK_TRUMPSTAGE_WORK(), "GMK_TRUMPSTAGE");

        GMS_ENEMY_3D_WORK gmk_work = (GMS_ENEMY_3D_WORK)work;
        OBS_OBJECT_WORK obj_work = gmk_work.ene_com.obj_work;

        work.suit = eve_rec.id - GMD_EVENT_ID_GMK_TRUMP_STAGE_S;
        work.stagger = MTM_MATH_CLIP(eve_rec.left, 0, GMD_GMK_TRUMPSTAGE_STAGGER_MAX);
        work.period = eve_rec.flag & GMD_GMK_TRUMPSTAGE_FLAG_PERIOD;

        gmGmkTrumpStageSetParams(work, eve_rec.flag);

        ObjObjectCopyAction3dNNModel(
            obj_work, g_gm_gmk_trumpstage_obj_3d_list[work.suit][0], gmk_work.obj_3d);

        obj_work.flag |= OBD_OBJECT_NORELEASE_3D;

        ObjObjectAction3dNNMotionLoad(
            obj_work, 0, false, ObjDataGet(GMD_DWORK_NO_GMK_TRUMPSTAGE_MTN), null, 0, null);

        gmGmkTrumpStageSetMotion(work, GMD_GMK_TRUMPSTAGE_MTN_FLIP_AWAY);

        obj_work.pos.z = -262144;
        obj_work.move_flag |= OBD_MOVE_NOMOVE | OBD_MOVE_NOCOL;
        obj_work.disp_flag |= OBD_DISP_NODIRFLIP;
        gmk_work.ene_com.enemy_flag |= GMD_ENEMY_FLAG_NOPRESSDIE;

        gmGmkTrumpStageSetupPlatform(work);

        work.phase = GMD_GMK_TRUMPSTAGE_PHASE_NONE;

        if ((eve_rec.flag & GMD_GMK_TRUMPSTAGE_FLAG_TRIGGERED) != 0)
        {
            work.trigger_timer = 0;
            obj_work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkTrumpStageStateWaitRider);
            return obj_work;
        }

        obj_work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkTrumpStageStateCycle);

        // run one tick now so the card is already in its phase for the frame it first draws on
        gmGmkTrumpStageStateCycle(obj_work);

        return obj_work;
    }

    // Four parameter sets off flag bits 2 and 3. The triggered pair leave the cyclic-only
    // durations at zero, which is safe because nothing on that path reads them
    internal static void gmGmkTrumpStageSetParams(GMS_GMK_TRUMPSTAGE_WORK work, ushort flag)
    {
        bool fast = (flag & GMD_GMK_TRUMPSTAGE_FLAG_FAST) != 0;

        if ((flag & GMD_GMK_TRUMPSTAGE_FLAG_TRIGGERED) != 0)
        {
            work.flip_manual_time = fast ? 4.0f : 60.0f;
            work.flip_manual_rate = fast ? 0.25f : 1.0f / 60.0f;
            work.solid_off_time = fast ? 8.0f : 64.0f;
            work.flip_auto_rate = fast ? 2.0f : 1.0f;
            work.sp_accel = fast ? 0.01f : 0.0f;
            work.sp_speed_min = 1.0f;
        }
        else
        {
            work.flip_manual_time = fast ? 4.0f : 60.0f;
            work.flip_manual_rate = fast ? 1.5f : 0.1f;
            work.flip_away_time = fast ? 10.0f : 74.0f;
            work.solid_off_time = fast ? 8.0f : 64.0f;
            work.flip_auto_rate = fast ? 9.0f : 27.0f / 7.0f;
            work.flip_back_time = fast ? 10.0f : 30.0f;
            work.flip_back_rate = fast ? 6.0f : 2.0f;
            work.solid_on_time = fast ? 6.0f : 20.0f;
        }

        work.idle_up_time = (float)Math.Ceiling(120.0 + 60.0 * (3 - work.period));
        work.idle_down_time = (float)Math.Ceiling(work.idle_up_time * 0.5);
        work.cycle_time = (float)Math.Ceiling(
            work.idle_down_time + work.idle_up_time + work.flip_away_time + work.flip_back_time);
        work.phase_ofst = (float)Math.Ceiling(
            work.cycle_time * (GMD_GMK_TRUMPSTAGE_STAGGER_MAX - work.stagger) * 0.03125);
    }

    // The landable surface. Not a rect: this is the collision object every platform gimmick in the
    // tree uses, and leaving diff_data null is deliberate, it puts a flat card on the plain box
    // path instead of the height-map one
    private static void gmGmkTrumpStageSetupPlatform(GMS_GMK_TRUMPSTAGE_WORK work)
    {
        OBS_COLLISION_OBJ platform = ((GMS_ENEMY_3D_WORK)work).ene_com.col_work.obj_col;

        platform.obj = (OBS_OBJECT_WORK)work;
        platform.width = GMD_GMK_TRUMPSTAGE_PLATFORM_W;
        platform.height = GMD_GMK_TRUMPSTAGE_PLATFORM_H;
        platform.ofst_x = -(short)(GMD_GMK_TRUMPSTAGE_PLATFORM_W / 2);
        platform.ofst_y = -1;
    }

    private static void gmGmkTrumpStageSetMotion(GMS_GMK_TRUMPSTAGE_WORK work, int mtn_id)
    {
        OBS_OBJECT_WORK obj_work = (OBS_OBJECT_WORK)work;
        OBS_ACTION3D_NN_WORK obj_3d = obj_work.obj_3d;

        obj_work.disp_flag &= ~(OBD_DISP_REPEAT | OBD_DISP_END);

        if (obj_3d.motion != null)
            ObjDrawAction3dActionSet3DNN(obj_3d, mtn_id, 0);

        obj_3d.speed[1] = 0.0f;
        obj_3d.speed[0] = 0.0f;
    }

    private static void gmGmkTrumpStageSetSolid(GMS_GMK_TRUMPSTAGE_WORK work, bool solid)
    {
        OBS_COLLISION_OBJ platform = ((GMS_ENEMY_3D_WORK)work).ene_com.col_work.obj_col;

        if (solid)
        {
            platform.obj = (OBS_OBJECT_WORK)work;
            return;
        }

        platform.obj = null;
        platform.height = 0;
    }

    // The landable box grows back over 17 frames after every re-solidify rather than popping in,
    // which is why the height doubles as the ramp
    private static void gmGmkTrumpStageRampSolidity(GMS_GMK_TRUMPSTAGE_WORK work)
    {
        OBS_COLLISION_OBJ platform = ((GMS_ENEMY_3D_WORK)work).ene_com.col_work.obj_col;

        if (platform.obj == null || platform.height >= GMD_GMK_TRUMPSTAGE_PLATFORM_H)
            return;

        ++platform.height;
    }

    // The frame is driven by hand rather than by the motion speed, so the state writes it and the
    // draw's own update adds a zero speed on top and clamps
    private static void gmGmkTrumpStageSetFrame(GMS_GMK_TRUMPSTAGE_WORK work, float frame)
    {
        OBS_ACTION3D_NN_WORK obj_3d = ((OBS_OBJECT_WORK)work).obj_3d;

        obj_3d.frame[0] = frame;
        obj_3d.mat_frame = obj_3d.frame[0];
    }

    internal static void gmGmkTrumpStageStateCycle(OBS_OBJECT_WORK obj_work)
    {
        GMS_GMK_TRUMPSTAGE_WORK work = (GMS_GMK_TRUMPSTAGE_WORK)obj_work;

        float cycle = work.cycle_time;
        float clock = g_gm_main_system.sync_time + cycle + work.phase_ofst;
        float turns = (float)Math.Truncate(clock / cycle);

        work.cycle_pos = (int)(clock - turns * cycle);

        float pos = work.cycle_pos;
        float flip_away_end = work.idle_up_time + work.flip_away_time;

        if (pos < work.idle_up_time)
            gmGmkTrumpStagePhaseIdleUp(work);
        else if (pos < flip_away_end)
            gmGmkTrumpStagePhaseFlipAway(work);
        else if (pos < flip_away_end + work.idle_down_time)
            gmGmkTrumpStagePhaseIdleDown(work);
        else
            gmGmkTrumpStagePhaseFlipBack(work);

        gmGmkTrumpStageRampSolidity(work);
    }

    private static void gmGmkTrumpStagePhaseIdleUp(GMS_GMK_TRUMPSTAGE_WORK work)
    {
        if (work.phase == GMD_GMK_TRUMPSTAGE_PHASE_IDLE_UP)
            return;

        work.phase = GMD_GMK_TRUMPSTAGE_PHASE_IDLE_UP;
        gmGmkTrumpStageSetMotion(work, GMD_GMK_TRUMPSTAGE_MTN_FLIP_AWAY);
        gmGmkTrumpStageSetSolid(work, true);
    }

    private static void gmGmkTrumpStagePhaseFlipAway(GMS_GMK_TRUMPSTAGE_WORK work)
    {
        if (work.phase != GMD_GMK_TRUMPSTAGE_PHASE_FLIP_AWAY)
        {
            work.phase = GMD_GMK_TRUMPSTAGE_PHASE_FLIP_AWAY;
            gmGmkTrumpStageSetMotion(work, GMD_GMK_TRUMPSTAGE_MTN_FLIP_AWAY);
        }

        float t = work.cycle_pos - work.idle_up_time;

        if (t > work.flip_manual_time)
            gmGmkTrumpStageSetFrame(work,
                (t - work.flip_manual_time) * work.flip_auto_rate + GMD_GMK_TRUMPSTAGE_FLIP_HANDOVER_FRAME);
        else
            gmGmkTrumpStageSetFrame(work, work.flip_manual_rate * t);

        gmGmkTrumpStageSetSolid(work, t < work.solid_off_time);
    }

    private static void gmGmkTrumpStagePhaseIdleDown(GMS_GMK_TRUMPSTAGE_WORK work)
    {
        if (work.phase == GMD_GMK_TRUMPSTAGE_PHASE_IDLE_DOWN)
            return;

        work.phase = GMD_GMK_TRUMPSTAGE_PHASE_IDLE_DOWN;
        gmGmkTrumpStageSetMotion(work, GMD_GMK_TRUMPSTAGE_MTN_FLIP_BACK);
        gmGmkTrumpStageSetSolid(work, false);
    }

    private static void gmGmkTrumpStagePhaseFlipBack(GMS_GMK_TRUMPSTAGE_WORK work)
    {
        if (work.phase != GMD_GMK_TRUMPSTAGE_PHASE_FLIP_BACK)
        {
            work.phase = GMD_GMK_TRUMPSTAGE_PHASE_FLIP_BACK;
            gmGmkTrumpStageSetMotion(work, GMD_GMK_TRUMPSTAGE_MTN_FLIP_BACK);
        }

        float t = work.cycle_pos - (work.idle_down_time + work.idle_up_time + work.flip_away_time);

        gmGmkTrumpStageSetFrame(work, work.flip_back_rate * t);
        gmGmkTrumpStageSetSolid(work, t >= work.solid_on_time);
    }

    private static void gmGmkTrumpStageStateWaitRider(OBS_OBJECT_WORK obj_work)
    {
        GMS_GMK_TRUMPSTAGE_WORK work = (GMS_GMK_TRUMPSTAGE_WORK)obj_work;
        GMS_ENEMY_COM_WORK ene_com = ((GMS_ENEMY_3D_WORK)work).ene_com;

        if (work.trigger_timer == 0)
        {
            OBS_OBJECT_WORK rider = ene_com.col_work.obj_col.rider_obj;

            if (rider != null && rider.obj_type == 1)
                work.trigger_timer = (ene_com.eve_rec.flag & GMD_GMK_TRUMPSTAGE_FLAG_FAST) != 0
                    ? GMD_GMK_TRUMPSTAGE_TRIGGER_WAIT_FAST
                    : GMD_GMK_TRUMPSTAGE_TRIGGER_WAIT;
        }

        if (work.trigger_timer != 0)
        {
            --work.trigger_timer;

            if (work.trigger_timer <= 0)
            {
                work.cycle_pos = (int)work.idle_up_time;
                obj_work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkTrumpStageStateOneShot);
            }
        }

        gmGmkTrumpStageRampSolidity(work);
    }

    private static void gmGmkTrumpStageStateOneShot(OBS_OBJECT_WORK obj_work)
    {
        GMS_GMK_TRUMPSTAGE_WORK work = (GMS_GMK_TRUMPSTAGE_WORK)obj_work;

        gmGmkTrumpStagePhaseSpFlip(work);

        // the card stays gone. nothing brings it back, the work just lives until it scrolls off
        if ((obj_work.disp_flag & OBD_DISP_END) != 0)
            obj_work.ppFunc = null;

        gmGmkTrumpStageRampSolidity(work);
    }

    private static void gmGmkTrumpStagePhaseSpFlip(GMS_GMK_TRUMPSTAGE_WORK work)
    {
        OBS_ACTION3D_NN_WORK obj_3d = ((OBS_OBJECT_WORK)work).obj_3d;

        if (work.phase != GMD_GMK_TRUMPSTAGE_PHASE_SP)
        {
            work.sp_speed_latched = false;
            work.phase = GMD_GMK_TRUMPSTAGE_PHASE_SP;
            gmGmkTrumpStageSetMotion(work, GMD_GMK_TRUMPSTAGE_MTN_SP);
            work.cycle_pos = 0;
        }

        ++work.cycle_pos;
        float t = work.cycle_pos;

        if (t <= work.flip_manual_time)
        {
            gmGmkTrumpStageSetFrame(work, work.flip_manual_rate * t);
        }
        else if (work.sp_speed_latched)
        {
            obj_3d.speed[0] += work.sp_accel;

            if (obj_3d.speed[0] < work.sp_speed_min)
                obj_3d.speed[0] = work.sp_speed_min;

            obj_3d.mat_speed = obj_3d.speed[0];
            obj_3d.speed[1] = obj_3d.mat_speed;
        }
        else
        {
            // one frame of handover: the motion stops being driven by hand and starts playing
            obj_3d.mat_speed = work.flip_auto_rate;
            obj_3d.speed[1] = obj_3d.mat_speed;
            obj_3d.speed[0] = obj_3d.speed[1];
            work.sp_speed_latched = true;
        }

        gmGmkTrumpStageSetSolid(work, t < work.solid_off_time);
    }
}
