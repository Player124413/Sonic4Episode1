internal partial class AppMain
{
    // One placed flip card: a decorative frame built from up to ten mirrored segment models, plus
    // four pict children spawned as local events. PC gives it a 0x3A40 work with the segments
    // inline; see docs/z2a2-tp-turn-re.md.
    internal class GMS_GMK_TP_TURN_WORK : IOBS_OBJECT_WORK
    {
        public readonly GMS_ENEMY_3D_WORK gmk_work;

        // slot into gm_gmk_tp_turn_card_timer, shared with any other card the designer grouped
        public int card_idx;

        public int variant;

        // which gm_gmk_tp_turn_deck record the faces came from. The board takes four consecutive
        // ids, one per row, so this is the first of them
        public int deck_id;

        // set by any child mid-flip and consumed the same frame by the card state, which is what
        // keeps the card alive while a panel is still spinning
        public bool child_flipping;

        // PC draws its frame from ten obj3d slots inside the card work rather than through the
        // object's own obj_3d, and the card's draw walks them itself
        public readonly OBS_ACTION3D_NN_WORK[] frame_obj_3d;

        // last material member loaded into each slot, so the base/lit swap only reloads the slots
        // that actually changed
        public readonly int[] frame_mat_idx;

        // the two deck record bytes for the line being spawned, kinds and revealed flags, staged
        // here because the pict maker is reached through the local event birth and cannot be
        // handed anything but the packed byte
        public byte deck_kinds;
        public byte deck_revealed;

        public GMS_GMK_TP_TURN_WORK()
        {
            this.gmk_work = new GMS_ENEMY_3D_WORK(this);
            this.frame_obj_3d = new OBS_ACTION3D_NN_WORK[GMD_GMK_TP_TURN_FRAME_MAX];
            this.frame_mat_idx = new int[GMD_GMK_TP_TURN_FRAME_MAX];

            for (int slot = 0; slot < GMD_GMK_TP_TURN_FRAME_MAX; ++slot)
            {
                this.frame_obj_3d[slot] = new OBS_ACTION3D_NN_WORK();
                this.frame_mat_idx[slot] = -1;
            }
        }

        public OBS_OBJECT_WORK Cast()
        {
            return this.gmk_work.ene_com.obj_work;
        }

        public static explicit operator OBS_OBJECT_WORK(GMS_GMK_TP_TURN_WORK work)
        {
            return work.gmk_work.ene_com.obj_work;
        }

        public static explicit operator GMS_ENEMY_3D_WORK(GMS_GMK_TP_TURN_WORK work)
        {
            return work.gmk_work;
        }
    }
}
