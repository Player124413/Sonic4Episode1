namespace Sonic4Episode1.Tests;

// The card platforms are all arithmetic and no tables, so what fails quietly here is a number
// rather than a row: a parameter set that is a frame out still flips, it just stops landing on the
// end of its motion, and a stagger that rounds the wrong way desynchronises every card in the
// stage against level time without ever looking broken. Every constant below was read out of
// gmGmkTrumpStageInit and its phase functions.
public class TrumpStageTests
{
    private const ushort CYCLIC = 0;
    private const ushort TRIGGERED = AppMain.GMD_GMK_TRUMPSTAGE_FLAG_TRIGGERED;
    private const ushort FAST = AppMain.GMD_GMK_TRUMPSTAGE_FLAG_FAST;

    // the motion length every parameter set is tuned to consume
    private const float MOTION_FRAMES = 60f;

    private static AppMain.GMS_GMK_TRUMPSTAGE_WORK WorkFor(ushort flag, int stagger = 0)
    {
        var work = new AppMain.GMS_GMK_TRUMPSTAGE_WORK();
        var gmk_work = (AppMain.GMS_ENEMY_3D_WORK)work;

        // stands in for ObjObjectCopyAction3dNNModel, which is what hangs the obj3d off the object
        // in the real maker. Without it the phase entries have no motion channel to write
        gmk_work.ene_com.obj_work.obj_3d = gmk_work.obj_3d;

        work.stagger = AppMain.MTM_MATH_CLIP(stagger, 0, AppMain.GMD_GMK_TRUMPSTAGE_STAGGER_MAX);
        work.period = flag & AppMain.GMD_GMK_TRUMPSTAGE_FLAG_PERIOD;
        AppMain.gmGmkTrumpStageSetParams(work, flag);

        return work;
    }

    // Eight event ids, eight one-member archives, and the suit is the offset into both. PC has to
    // skip 312/313 because its ids are not contiguous; ours are, so a drift here would silently
    // deal every card the wrong face
    [Theory]
    [InlineData(AppMain.GMD_EVENT_ID_GMK_TRUMP_STAGE_S, 0)]
    [InlineData(AppMain.GMD_EVENT_ID_GMK_TRUMP_STAGE_H, 1)]
    [InlineData(AppMain.GMD_EVENT_ID_GMK_TRUMP_STAGE_D, 2)]
    [InlineData(AppMain.GMD_EVENT_ID_GMK_TRUMP_STAGE_C, 3)]
    [InlineData(AppMain.GMD_EVENT_ID_GMK_TRUMP_STAGE_S5, 4)]
    [InlineData(AppMain.GMD_EVENT_ID_GMK_TRUMP_STAGE_H7, 5)]
    [InlineData(AppMain.GMD_EVENT_ID_GMK_TRUMP_STAGE_D10, 6)]
    [InlineData(AppMain.GMD_EVENT_ID_GMK_TRUMP_STAGE_C9, 7)]
    public void EachIdSelectsItsOwnSuit(int id, int suit)
    {
        Assert.Equal(suit, id - AppMain.GMD_EVENT_ID_GMK_TRUMP_STAGE_S);
        Assert.InRange(suit, 0, AppMain.GMD_GMK_TRUMPSTAGE_SUIT_MAX - 1);

        int slot = AppMain.GMD_DWORK_NO_GMK_TRUMPSTAGE_S_MDL + suit;
        Assert.True(slot < AppMain.GMD_DWORK_NO_GMK_TRUMPSTAGE_TEX, "suit ran into the TEX slot");
    }

    [Fact]
    public void TheElevenArchiveSlotsResolveInsideTheWidenedArray()
    {
        int[] slots =
        {
            AppMain.GMD_DWORK_NO_GMK_TRUMPSTAGE_S_MDL,
            AppMain.GMD_DWORK_NO_GMK_TRUMPSTAGE_C9_MDL,
            AppMain.GMD_DWORK_NO_GMK_TRUMPSTAGE_TEX,
            AppMain.GMD_DWORK_NO_GMK_TRUMPSTAGE_MTN,
            AppMain.GMD_DWORK_NO_GMK_TRUMPSTAGE_MAT,
        };

        foreach (int slot in slots)
        {
            Assert.True(slot >= AppMain.GMD_DWORK_NO_GMK_START, $"slot {slot} below GMK_START");
            Assert.True(slot < AppMain.GMD_DWORK_NO_GMK_END, $"slot {slot} past GMK_END");
        }

        Assert.Equal(AppMain.GMD_DWORK_NO_GMK_TRUMPSTAGE_S_MDL + AppMain.GMD_GMK_TRUMPSTAGE_SUIT_MAX,
            AppMain.GMD_DWORK_NO_GMK_TRUMPSTAGE_TEX);

        Assert.Equal(AppMain.GMD_DWORK_NO_GMK_END, AppMain.GMD_DWORK_NO_DEC_START);
        Assert.True(AppMain.GMD_DWORK_NO_MAX >= AppMain.GMD_DWORK_NO_DEC_END);
    }

    // flag & 3 picks the dwell, and the flipped dwell is always half of it
    [Theory]
    [InlineData(0, 300f, 150f)]
    [InlineData(1, 240f, 120f)]
    [InlineData(2, 180f, 90f)]
    [InlineData(3, 120f, 60f)]
    public void ThePeriodSelectsTheDwell(int period, float idle_up, float idle_down)
    {
        var work = WorkFor((ushort)period);

        Assert.Equal(idle_up, work.idle_up_time);
        Assert.Equal(idle_down, work.idle_down_time);
    }

    [Theory]
    [InlineData(CYCLIC, 74f, 30f, 284f)]
    [InlineData(CYCLIC | FAST, 10f, 10f, 200f)]
    public void TheCycleIsTheFourPhasesAddedUp(ushort flag, float away, float back, float cycle)
    {
        var work = WorkFor((ushort)(flag | 3));

        Assert.Equal(away, work.flip_away_time);
        Assert.Equal(back, work.flip_back_time);
        Assert.Equal(cycle, work.cycle_time);
    }

    // Both cyclic sets drive the flip by hand to frame 6 and then at a second rate to the end of
    // the motion, landing on 60 exactly on the phase's last frame. Getting either rate wrong makes
    // the card snap or stall at the end of its flip rather than failing
    [Theory]
    [InlineData(CYCLIC)]
    [InlineData(CYCLIC | FAST)]
    public void EveryFlipConsumesExactlyOneMotion(ushort flag)
    {
        var work = WorkFor((ushort)(flag | 3));

        float handover = work.flip_manual_rate * work.flip_manual_time;
        Assert.Equal(AppMain.GMD_GMK_TRUMPSTAGE_FLIP_HANDOVER_FRAME, handover, 3);

        float away_end = (work.flip_away_time - work.flip_manual_time) * work.flip_auto_rate + handover;
        Assert.Equal(MOTION_FRAMES, away_end, 3);

        Assert.Equal(MOTION_FRAMES, work.flip_back_rate * work.flip_back_time, 3);
    }

    // The triggered pair leave the cyclic-only durations alone, so anything reading them on that
    // path is reading a zero. Nothing does, and this is what says so
    [Theory]
    [InlineData(TRIGGERED)]
    [InlineData(TRIGGERED | FAST)]
    public void TheTriggeredSetsLeaveTheCyclicDurationsUnset(ushort flag)
    {
        var work = WorkFor((ushort)(flag | 3));

        Assert.Equal(0f, work.flip_away_time);
        Assert.Equal(0f, work.flip_back_time);
        Assert.Equal(0f, work.flip_back_rate);
        Assert.Equal(0f, work.solid_on_time);
        Assert.Equal(1f, work.sp_speed_min);
    }

    // the one-shot's wind-up nudges the motion a single frame before handing it to the auto-play,
    // which is what makes the card wobble before it goes rather than dropping flat
    [Theory]
    [InlineData(TRIGGERED, 1f)]
    [InlineData(TRIGGERED | FAST, 1f)]
    public void TheOneShotWindUpIsOneFrameOfMotion(ushort flag, float frames)
    {
        var work = WorkFor((ushort)(flag | 3));

        Assert.Equal(frames, work.flip_manual_rate * work.flip_manual_time, 3);
    }

    // sub_4136D0 is ceil away from zero, not round to nearest. With cycle 200 the two disagree at
    // stagger 2, and a card that is one frame out of phase with the rest of the stage is exactly
    // the kind of thing nobody notices until a jump does not land
    [Theory]
    [InlineData(0, 194f)]
    [InlineData(1, 188f)]
    [InlineData(2, 182f)]
    [InlineData(31, 0f)]
    public void TheStaggerOffsetIsCeiled(int stagger, float ofst)
    {
        var work = WorkFor(CYCLIC | FAST | 3, stagger);

        Assert.Equal(200f, work.cycle_time);
        Assert.Equal(ofst, work.phase_ofst);
    }

    // ev.left is signed and unbounded; PC folds negatives to 0 and everything past 31 to 31
    [Theory]
    [InlineData(-5, 0)]
    [InlineData(0, 0)]
    [InlineData(30, 30)]
    [InlineData(31, 31)]
    [InlineData(120, 31)]
    public void TheStaggerIsClampedIntoTheCycle(int left, int stagger)
    {
        Assert.Equal(stagger,
            AppMain.MTM_MATH_CLIP(left, 0, AppMain.GMD_GMK_TRUMPSTAGE_STAGGER_MAX));
    }

    // Drives a real card through a whole cycle on the global clock and reads back which phase it
    // landed in and whether it was landable. The boundaries and the two solidity windows are the
    // whole behaviour of the gimmick, and none of it is visible from the tables
    [Fact]
    public void ACycleWalksThePhasesAndTheSolidWindows()
    {
        uint saved = AppMain.g_gm_main_system.sync_time;

        try
        {
            var work = WorkFor(CYCLIC | FAST | 3);
            var obj_work = (AppMain.OBS_OBJECT_WORK)work;
            var platform = ((AppMain.GMS_ENEMY_3D_WORK)work).ene_com.col_work.obj_col;

            work.phase = AppMain.GMD_GMK_TRUMPSTAGE_PHASE_NONE;
            work.phase_ofst = 0f;

            // sync_time + cycle + 0, so frame 0 of the clock is position 0 of the cycle
            for (uint frame = 0; frame < 200; ++frame)
            {
                AppMain.g_gm_main_system.sync_time = frame;
                AppMain.gmGmkTrumpStageStateCycle(obj_work);

                Assert.Equal((int)frame, work.cycle_pos);
                Assert.Equal(ExpectedPhase(frame), work.phase);
                Assert.Equal(ExpectedSolid(frame), platform.obj != null);
            }
        }
        finally
        {
            AppMain.g_gm_main_system.sync_time = saved;
        }
    }

    // 120 face up, 10 flipping away, 60 face down, 10 flipping back
    private static int ExpectedPhase(uint pos)
    {
        if (pos < 120)
            return AppMain.GMD_GMK_TRUMPSTAGE_PHASE_IDLE_UP;
        if (pos < 130)
            return AppMain.GMD_GMK_TRUMPSTAGE_PHASE_FLIP_AWAY;
        if (pos < 190)
            return AppMain.GMD_GMK_TRUMPSTAGE_PHASE_IDLE_DOWN;

        return AppMain.GMD_GMK_TRUMPSTAGE_PHASE_FLIP_BACK;
    }

    // landable for the face-up dwell and the first 8 of the 10 flip-away frames, then gone until
    // 6 frames into the flip back
    private static bool ExpectedSolid(uint pos)
    {
        if (pos < 128)
            return true;

        return pos >= 196;
    }

    // The landable box grows back over 17 frames instead of popping in, and the height is both the
    // box and the ramp. A platform re-enabled at full height would catch the player early
    [Fact]
    public void TheSolidWindowRampsBackFromZero()
    {
        uint saved = AppMain.g_gm_main_system.sync_time;

        try
        {
            var work = WorkFor(CYCLIC | FAST | 3);
            var obj_work = (AppMain.OBS_OBJECT_WORK)work;
            var platform = ((AppMain.GMS_ENEMY_3D_WORK)work).ene_com.col_work.obj_col;

            work.phase = AppMain.GMD_GMK_TRUMPSTAGE_PHASE_NONE;
            work.phase_ofst = 0f;
            platform.width = AppMain.GMD_GMK_TRUMPSTAGE_PLATFORM_W;
            platform.height = AppMain.GMD_GMK_TRUMPSTAGE_PLATFORM_H;

            AppMain.g_gm_main_system.sync_time = 150;
            AppMain.gmGmkTrumpStageStateCycle(obj_work);
            Assert.Equal(0, platform.height);

            for (uint frame = 196; frame < 200; ++frame)
            {
                AppMain.g_gm_main_system.sync_time = frame;
                AppMain.gmGmkTrumpStageStateCycle(obj_work);
            }

            Assert.Equal(4, platform.height);
            Assert.Equal(AppMain.GMD_GMK_TRUMPSTAGE_PLATFORM_W, platform.width);
        }
        finally
        {
            AppMain.g_gm_main_system.sync_time = saved;
        }
    }

    // Not covered here, because they need a loaded stage: the suit models, the motion attach and
    // the triggered variant's rider detection. All three are verified by playing.
}
