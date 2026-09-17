internal partial class AppMain
{
    internal class GMS_GMK_TORCH_WORK : IOBS_OBJECT_WORK
    {
        public GMS_ENEMY_3D_WORK gmk_work;

        // PC chains these through the effect works' own user field; ours holds them here because
        // that field is a uint in this tree, not a pointer
        public readonly GMS_EFFECT_3DES_WORK[] flame = new GMS_EFFECT_3DES_WORK[3];
        public int flame_idx = -1;

        public GMS_GMK_TORCH_WORK()
        {
            this.gmk_work = new GMS_ENEMY_3D_WORK(this);
        }

        public static explicit operator OBS_OBJECT_WORK(GMS_GMK_TORCH_WORK work)
        {
            return work.gmk_work.ene_com.obj_work;
        }

        public OBS_OBJECT_WORK Cast()
        {
            return this.gmk_work.ene_com.obj_work;
        }
    }
}
