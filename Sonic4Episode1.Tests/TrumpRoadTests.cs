namespace Sonic4Episode1.Tests;

// The card road is the one gimmick the stage cannot be finished without, and most of what it needs
// is table data that fails quietly: a missing player state row throws on the first frame of the
// ride, and a gamedat slot outside the widened array reads a null archive at spawn. See
// docs/z2a2-trumproad-re.md.
public class TrumpRoadTests
{
    // PC appended its ride at 67, which is where our own list ended, so the two agree without a
    // remap. GmPlySeqMain indexes the state data table by seq_state every frame, so every
    // character needs the row or the ride throws immediately
    [Fact]
    public void EveryCharacterHasARowForTheRideState()
    {
        Assert.Equal(68, AppMain.GME_PLY_SEQ_STATE_MAX);

        foreach (var states in AppMain.g_gm_ply_seq_state_data_tbl)
        {
            Assert.Equal(AppMain.GME_PLY_SEQ_STATE_MAX, states.Length);

            var ride = states[AppMain.GME_PLY_SEQ_STATE_GMK_TRUMP_ROAD];
            Assert.Equal(AppMain.GMD_PLY_SEQ_STATE_CHECK_ATTR_MTN_SPEED_NOSET, ride.check_attr);
            Assert.Equal(0U, ride.accept_attr);
        }
    }

    // row 66 is the last one both trees share, so it is the check that row 67 was read at the right
    // stride out of PC's table at 0x57F2B0. Only character 0 is pinned: super and trial sonic hold
    // different values here, and only character 0's was read back from the binary
    [Fact]
    public void TheRowBelowTheRideStillMatchesPc()
    {
        var spin_fall = AppMain.g_gm_ply_seq_state_data_tbl[0][66];
        Assert.Equal(0x50000002U, spin_fall.check_attr);
        Assert.Equal(0x10U, spin_fall.accept_attr);
    }

    // The card road draws in a pass of its own. Both draw-order tables are indexed by slot, so a
    // state added without widening them writes past the end, and a state at or above MAX is never
    // reached by the executor's loop
    [Fact]
    public void TheCardRoadPassFitsTheDrawOrderTables()
    {
        Assert.Equal(19u, AppMain.OBD_DRAW_CMD_STATE_MAX);
        Assert.True(AppMain.OBD_DRAW_CMD_STATE_TRUMPROAD < AppMain.OBD_DRAW_CMD_STATE_MAX);
        Assert.True(AppMain.obj_draw_3dnn_command_state_tbl.Length >= AppMain.OBD_DRAW_CMD_STATE_MAX);
        Assert.True(AppMain.obj_draw_3dnn_command_state_exe_end_scene_tbl.Length >= AppMain.OBD_DRAW_CMD_STATE_MAX);
    }

    // Camera 7 is the last slot OBS_CAMERA_SYS has, and PC puts its card-road camera in the same
    // one. Anything that starts allocating another camera before it will silently take it
    [Fact]
    public void TheCardRoadCameraFitsTheCameraArray()
    {
        Assert.Equal(7, AppMain.GME_CAMERA_NO_TRUMPROAD);
        Assert.True(AppMain.GME_CAMERA_NO_TRUMPROAD < new AppMain.OBS_CAMERA_SYS().obj_camera.Length);
    }

    // Not covered by tests, because both need a started game rather than a static table: camera 7's
    // fovy/znear/zfar (set in GmCamera, values in docs/z2a2-trumproad-re.md) and the pass's slot in
    // the execution list (set in gmMainSetNNCommandStateTbl). Both are verified by playing.
    //
    // Nor is the homing whitelist in gmPlayerGetHomingTarget. PC adds exactly one id to
    // it (295, our card) and without it the gimmick is unreachable, since the hit callback only
    // fires while the player is in the homing state. It sits inside a long private function with no
    // seam to drive, so it is verified by playing rather than by a test

    // the three archives are appended past mobile's tail, so the gimmick array has to have grown
    // with them. GmGameDatGetGimmickData indexes it by slot - GMK_START
    [Fact]
    public void TheGamedatSlotsResolveInsideTheWidenedArray()
    {
        int[] slots =
        {
            AppMain.GMD_DWORK_NO_GMK_TRUMPROAD_MDL,
            AppMain.GMD_DWORK_NO_GMK_TRUMPROAD_TEX,
            AppMain.GMD_DWORK_NO_GMK_TRUMPROAD_MTN,
        };

        foreach (int slot in slots)
        {
            Assert.True(slot >= AppMain.GMD_DWORK_NO_GMK_START, $"slot {slot} below GMK_START");
            Assert.True(slot < AppMain.GMD_DWORK_NO_GMK_END, $"slot {slot} past GMK_END");
        }

        Assert.Equal(AppMain.GMD_DWORK_NO_GMK_END, AppMain.GMD_DWORK_NO_DEC_START);
        Assert.True(AppMain.GMD_DWORK_NO_MAX >= AppMain.GMD_DWORK_NO_DEC_END);
    }

    // MTN members 0 and 1 are the card's hit reactions, then ROUTE/GOAL pairs. GOAL is always the
    // member after its ROUTE, so the last route must leave room for one more
    [Fact]
    public void TheRouteMotionMembersLeaveRoomForTheirGoal()
    {
        Assert.Equal(new[] { 2, 4, 6, 8 }, AppMain.gm_gmk_trumproad_route_mtn_idx);

        foreach (int member in AppMain.gm_gmk_trumproad_route_mtn_idx)
            Assert.True(member > 1, "ROUTE members must not collide with the card hit reactions");
    }
}
