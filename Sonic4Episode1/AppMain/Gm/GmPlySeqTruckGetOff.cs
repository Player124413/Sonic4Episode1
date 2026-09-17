internal partial class AppMain
{
    private const int GMD_PLYGMK_TRUCK_GETOFF_SPD_X = 32768;
    private const int GMD_PLYGMK_TRUCK_GETOFF_NOMOVE_TIME = 32768;

    // PC 0x4E42D0. Two xrefs in the whole binary, both the minecart's nose sensor, so this is the
    // rail-end dismount and not the ordinary spring launch despite borrowing its cue. The player is
    // thrown 8.0 forward and out at whatever the caller passes, into seq state 68, which is the
    // spring state with the air steering taken off
    internal static void GmPlySeqInitTruckGetOff(GMS_PLAYER_WORK ply_work, int spd_y)
    {
        GmPlySeqChangeSequenceState(ply_work, GME_PLY_SEQ_STATE_GMK_TRUCK_GET_OFF);
        GmPlayerActionChange(ply_work, GME_PLY_ACT_STATE_JUMP_SPIN);
        ply_work.obj_work.disp_flag |= OBD_DISP_REPEAT;
        GmPlySeqGmkInitGmkJump(ply_work, GMD_PLYGMK_TRUCK_GETOFF_SPD_X, spd_y, false);
        GmPlySeqSetNoJumpMoveTime(ply_work, GMD_PLYGMK_TRUCK_GETOFF_NOMOVE_TIME);
        GmSoundPlaySE("Spring");
        GMM_PAD_VIB_SMALL();
    }
}
