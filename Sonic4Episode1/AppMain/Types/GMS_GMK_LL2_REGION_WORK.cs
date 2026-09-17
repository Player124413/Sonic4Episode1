internal partial class AppMain
{
    internal class GMS_GMK_LL2_REGION_WORK : IOBS_OBJECT_WORK
    {
        public GMS_ENEMY_COM_WORK gmk_work;
        public int region;
        public int size_x;
        public int size_y;
        public uint state;
        public int ramp;

        public GMS_GMK_LL2_REGION_WORK()
        {
            this.gmk_work = new GMS_ENEMY_COM_WORK(this);
        }

        public static explicit operator OBS_OBJECT_WORK(GMS_GMK_LL2_REGION_WORK work)
        {
            return work.gmk_work.obj_work;
        }

        public OBS_OBJECT_WORK Cast()
        {
            return this.gmk_work.obj_work;
        }
    }
}
