internal partial class AppMain
{
    // One of the four flip panels on a card. Spawned only by its parent card, through
    // GmEventMgrLocalEventBirth, and dies with it through the object system's parent link.
    internal class GMS_GMK_TP_TURN_PICT_WORK : IOBS_OBJECT_WORK
    {
        public readonly GMS_ENEMY_3D_WORK gmk_work;

        // 0 Eggman, 1 Ring, 2 Sonic, and also the MDL member the face is drawn from
        public int kind;

        public int col;
        public int row;
        public bool revealed;

        // degrees per frame, signed. The sign is what picks the spin direction
        public float spin_speed;

        // 0 Y axis, 1 X axis tumble, 2 and 3 the two diagonals
        public int spin_mode;

        public int flip_timer;

        // PC accumulates the spin into a quaternion it keeps inside the obj3d, at a field the
        // mobile decompile of OBS_ACTION3D_NN_WORK does not carry. It lives here instead and is
        // pushed into obj_3d.user_obj_mtx_r each frame, which is the same thing PC's draw does
        // with the flag it sets alongside it
        public NNS_QUATERNION spin;

        public GMS_GMK_TP_TURN_WORK card;

        public GMS_GMK_TP_TURN_PICT_WORK()
        {
            this.gmk_work = new GMS_ENEMY_3D_WORK(this);
        }

        public OBS_OBJECT_WORK Cast()
        {
            return this.gmk_work.ene_com.obj_work;
        }

        public static explicit operator OBS_OBJECT_WORK(GMS_GMK_TP_TURN_PICT_WORK work)
        {
            return work.gmk_work.ene_com.obj_work;
        }

        public static explicit operator GMS_ENEMY_3D_WORK(GMS_GMK_TP_TURN_PICT_WORK work)
        {
            return work.gmk_work;
        }
    }
}
