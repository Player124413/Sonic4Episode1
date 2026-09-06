using System;

internal partial class AppMain
{
    // The card-road ride, player side. PC gmPlayerTrumpRoadRideStart 0x4E4380 and
    // gmPlayerTrumpRoadRideState 0x4E3880; the gimmick half is in GmGmkTrumpRoad.cs.
    //
    // PC appended its ride state at 67, which is exactly where our own list ended, so unlike the
    // event ids this needs no remapping. What it does need is a row in the per-character state data
    // table, because GmPlySeqMain indexes that table by seq_state every frame.
    private const float GMD_PLYGMK_TRUMP_ROAD_MTN_SPEED = 2.5f;

    // super sonic rides at a fixed lean instead of the dash motion
    private const ushort GMD_PLYGMK_TRUMP_ROAD_SUPER_DIR_Z = unchecked((ushort)-3072);


    // Read out of PC's g_gm_ply_seq_state_data_tbl at 0x57F2B0, row 67 of all seven characters:
    // check_attr = MTN_SPEED_NOSET, accept_attr = 0. Nothing transitions out of the ride by input.
    // MTN_SPEED_NOSET is what stops GmPlySeqMain resetting obj_3d.speed to 1.0 every frame after
    // the state function has set it to 2.5. Inert for mobile levels, which never enter state 67
    internal static void gmPlySeqExtendStateDataForPCLevels()
    {
        for (int char_id = 0; char_id < g_gm_ply_seq_state_data_tbl.Length; ++char_id)
        {
            GMS_PLY_SEQ_STATE_DATA[] states = g_gm_ply_seq_state_data_tbl[char_id];
            if (states.Length >= GME_PLY_SEQ_STATE_MAX)
                continue;

            Array.Resize(ref states, GME_PLY_SEQ_STATE_MAX);
            states[GME_PLY_SEQ_STATE_GMK_TRUMP_ROAD] =
                new GMS_PLY_SEQ_STATE_DATA(GMD_PLY_SEQ_STATE_CHECK_ATTR_MTN_SPEED_NOSET, 0U);
            g_gm_ply_seq_state_data_tbl[char_id] = states;
        }
    }

    internal static void GmPlySeqInitTrumpRoad(GMS_PLAYER_WORK ply_work, OBS_OBJECT_WORK road_obj)
    {
        GmPlySeqChangeSequenceState(ply_work, GME_PLY_SEQ_STATE_GMK_TRUMP_ROAD);
        GmPlayerActionChange(ply_work, GME_PLY_ACT_STATE_DASH_1);

        OBS_OBJECT_WORK obj_work = ply_work.obj_work;
        gmPlySeqTrumpRoadSetRidePose(ply_work);

        obj_work.flag |= OBD_OBJECT_NOHIT;
        obj_work.disp_flag = obj_work.disp_flag & ~OBD_DISP_HFLIP | OBD_DISP_REPEAT;
        ply_work.player_flag |= GMD_PLF_WALK_SMK_EFCT_OFF | GMD_PLF_RESET_FALL_PARAM;
        obj_work.spd.x = 0;
        obj_work.spd.y = 0;
        obj_work.spd_m = 0;
        // NOCOL alone is enough. Obj.cs:889 gates ppCol on it, and GmPlySeqMoveFunc gates every
        // movement type on the check_attr MOVE_ bits, which state 67 has none of, so the move
        // function is already a no-op for the whole ride
        obj_work.move_flag = obj_work.move_flag & ~(OBD_MOVE_FALL | OBD_MOVE_NOCOL) | OBD_MOVE_NOCOL;

        gmPlySeqTrumpRoadSetRectsLive(ply_work, gmGmkTrumpRoadWorkOf(road_obj), false);

        ply_work.seq_func = new seq_func_delegate(gmPlySeqGmkMainTrumpRoad);
        ply_work.gmk_obj = road_obj;
    }

    // Rect collision is a separate system from ppCol, so OBD_MOVE_NOCOL leaves the rider able to
    // trip a gimmick's rect mid-ride, which would change its sequence state and end the ride.
    // Inferred, not observed: PC sets a u16 inside its player rect_work to 3 for the duration of
    // the ride and 0 after, and 3 is the rect count, but that offset could not be mapped onto our
    // struct. This is that effect written our way. Nothing has been seen to depend on it yet
    private static void gmPlySeqTrumpRoadSetRectsLive(
        GMS_PLAYER_WORK ply_work,
        GMS_GMK_TRUMPROAD_WORK road,
        bool live)
    {
        if (!live)
        {
            uint mask = 0;
            for (int i = 0; i < GMD_PLAYER_RECT_NUM; ++i)
            {
                if ((ply_work.rect_work[i].flag & 4U) != 0)
                    mask |= 1U << i;
                ply_work.rect_work[i].flag &= ~4U;
            }

            if (road != null)
                road.ride_rect_mask = mask;
            return;
        }

        uint restore = road == null ? 0 : road.ride_rect_mask;
        for (int i = 0; i < GMD_PLAYER_RECT_NUM; ++i)
        {
            if ((restore & 1U << i) != 0)
                ply_work.rect_work[i].flag |= 4U;
        }
    }

    private static void gmPlySeqTrumpRoadSetRidePose(GMS_PLAYER_WORK ply_work)
    {
        OBS_OBJECT_WORK obj_work = ply_work.obj_work;
        if ((ply_work.player_flag & GMD_PLF_SUPER_SONIC) != 0)
        {
            obj_work.dir.z = GMD_PLYGMK_TRUMP_ROAD_SUPER_DIR_Z;
            return;
        }

        obj_work.dir.z = 0;
        obj_work.obj_3d.speed[0] = GMD_PLYGMK_TRUMP_ROAD_MTN_SPEED;
        obj_work.obj_3d.speed[1] = GMD_PLYGMK_TRUMP_ROAD_MTN_SPEED;
    }

    private static void gmPlySeqGmkMainTrumpRoad(GMS_PLAYER_WORK ply_work)
    {
        OBS_OBJECT_WORK obj_work = ply_work.obj_work;

        // re-applied every frame: GmPlySeqInitTrumpRoad's pose does not survive the action system
        if ((ply_work.player_flag & GMD_PLF_SUPER_SONIC) == 0)
            gmPlySeqTrumpRoadSetRidePose(ply_work);

        GMS_GMK_TRUMPROAD_WORK road = gmGmkTrumpRoadWorkOf(ply_work.gmk_obj);
        int prev_pos_x = obj_work.prev_pos.x;

        if (gmGmkTrumpRoadRidePlayer(ply_work))
        {
            if (road != null)
                road.ride_prev_pos_x = prev_pos_x;
            return;
        }

        gmPlySeqTrumpRoadRideEnd(ply_work, road);
    }

    private static void gmPlySeqTrumpRoadRideEnd(
        GMS_PLAYER_WORK ply_work,
        GMS_GMK_TRUMPROAD_WORK road)
    {
        OBS_OBJECT_WORK obj_work = ply_work.obj_work;

        // back to the pre-ride rect state first, then let the normal state entry work from there
        gmPlySeqTrumpRoadSetRectsLive(ply_work, road, true);

        GmPlySeqChangeSequence(ply_work, GME_PLY_SEQ_STATE_FW);

        ply_work.player_flag &= ~(GMD_PLF_WALK_SMK_EFCT_OFF | GMD_PLF_RESET_FALL_PARAM);
        obj_work.obj_3d.command_state = OBD_DRAW_CMD_STATE_3DNN;
        nnMakeUnitMatrix(obj_work.obj_3d.user_obj_mtx_r);
        obj_work.disp_flag &= ~OBD_DISP_NODIRFLIP;
        obj_work.flag &= ~OBD_OBJECT_NOHIT;
        obj_work.move_flag = obj_work.move_flag
            & ~(OBD_MOVE_UNDER | OBD_MOVE_FALL | OBD_MOVE_NOCOL) | OBD_MOVE_FALL;
        obj_work.dir.z = 0;
        obj_work.spd.x = 0;
        obj_work.spd.y = 0;

        // a small pop up off the road, then carry the last frame's horizontal travel out as speed
        obj_work.pos.y -= GMD_PLYGMK_TRUMP_ROAD_EXIT_POP;
        obj_work.spd_m = road == null ? 0 : obj_work.pos.x - road.ride_prev_pos_x;
    }

    private const int GMD_PLYGMK_TRUMP_ROAD_EXIT_POP = 61440;
}
