internal partial class AppMain
{
    // a timed card platform
    internal class GMS_GMK_TRUMPSTAGE_WORK : IOBS_OBJECT_WORK
    {
        public readonly GMS_ENEMY_3D_WORK gmk_work;

        public int suit;

        // ev.left, clamped. staggers this card 1/32 of a cycle per step against the others
        public int stagger;

        // ev.flag & 3, the dwell length select
        public int period;

        public float idle_up_time;
        public float idle_down_time;
        public float cycle_time;
        public float phase_ofst;

        // where in the cycle this card is, in frames. the one-shot uses it as its own counter
        public int cycle_pos;

        public float flip_manual_time;
        public float flip_manual_rate;
        public float flip_away_time;
        public float solid_off_time;

        // second-segment frame rate for the cyclic flip, motion speed for the one-shot
        public float flip_auto_rate;

        public float sp_accel;
        public float sp_speed_min;
        public float flip_back_time;
        public float flip_back_rate;
        public float solid_on_time;

        // -1 until the first phase entry, then 0..3 for the cyclic phases and 4 for the one-shot
        public int phase;

        // counts down once the player is stood on a triggered card, then the card commits
        public int trigger_timer;

        // the one-shot hands the motion back to the auto-advance partway through, and this is
        // what stops it doing that again every frame afterwards
        public bool sp_speed_latched;

        public GMS_GMK_TRUMPSTAGE_WORK()
        {
            this.gmk_work = new GMS_ENEMY_3D_WORK(this);
        }

        public OBS_OBJECT_WORK Cast()
        {
            return this.gmk_work.ene_com.obj_work;
        }

        public static explicit operator OBS_OBJECT_WORK(GMS_GMK_TRUMPSTAGE_WORK work)
        {
            return work.gmk_work.ene_com.obj_work;
        }

        public static explicit operator GMS_ENEMY_3D_WORK(GMS_GMK_TRUMPSTAGE_WORK work)
        {
            return work.gmk_work;
        }
    }
}
