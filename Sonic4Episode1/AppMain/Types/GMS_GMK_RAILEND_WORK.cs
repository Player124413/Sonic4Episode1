internal partial class AppMain
{
    internal class GMS_GMK_RAILEND_WORK : IOBS_OBJECT_WORK
    {
        public readonly OBS_ACTION3D_NN_WORK obj_3d_b = new OBS_ACTION3D_NN_WORK();
        public readonly GMS_ENEMY_3D_WORK gmk_work;
        public VecFx32 head_ofst;
        public OBS_OBJECT_WORK truck;

        public GMS_GMK_RAILEND_WORK()
        {
            this.gmk_work = new GMS_ENEMY_3D_WORK(this);
        }

        public static explicit operator OBS_OBJECT_WORK(GMS_GMK_RAILEND_WORK work)
        {
            return work.gmk_work.ene_com.obj_work;
        }

        public OBS_OBJECT_WORK Cast()
        {
            return this.gmk_work.ene_com.obj_work;
        }
    }
}
