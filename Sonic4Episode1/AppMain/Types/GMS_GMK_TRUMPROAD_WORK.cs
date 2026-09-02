internal partial class AppMain
{
    // Shared by the giant card (event 380) and the road it spawns (event 393); PC gives both the
    // same 0x8E0 work and uses whichever half it needs. See docs/z2a2-trumproad-re.md.
    internal class GMS_GMK_TRUMPROAD_WORK : IOBS_OBJECT_WORK
    {
        public readonly GMS_ENEMY_3D_WORK gmk_work;

        // the card
        public ushort hp;
        public bool hit_latch;

        // the road. The capture only runs if bmcb_mgr is hooked to the obj3d and the snm work's
        // link is appended to it, which is why both live here rather than just the snm work
        public readonly GMS_BS_CMN_BMCB_MGR bmcb_mgr = new GMS_BS_CMN_BMCB_MGR();
        public readonly GMS_BS_CMN_SNM_WORK snm_work = new GMS_BS_CMN_SNM_WORK();
        public bool snm_ready;
        public int snm_reg_id;
        public int route;
        public OBS_OBJECT_WORK card_obj;

        // PC keeps the rider's previous position in the player work, in fields its struct has and
        // ours does not. Only the x is ever read back, and only by the road, so it lives here
        public int ride_prev_pos_x;

        // which of the player's rects were live when the ride started, so the ride can switch them
        // off without guessing what to switch back on
        public uint ride_rect_mask;

        public GMS_GMK_TRUMPROAD_WORK()
        {
            this.gmk_work = new GMS_ENEMY_3D_WORK(this);
            this.snm_reg_id = -1;
        }

        public OBS_OBJECT_WORK Cast()
        {
            return this.gmk_work.ene_com.obj_work;
        }

        public static explicit operator OBS_OBJECT_WORK(GMS_GMK_TRUMPROAD_WORK work)
        {
            return work.gmk_work.ene_com.obj_work;
        }

        public static explicit operator GMS_ENEMY_3D_WORK(GMS_GMK_TRUMPROAD_WORK work)
        {
            return work.gmk_work;
        }
    }
}
