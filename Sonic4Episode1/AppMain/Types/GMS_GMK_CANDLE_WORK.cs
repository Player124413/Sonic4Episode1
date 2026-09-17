internal partial class AppMain
{
    internal class GMS_GMK_CANDLE_WORK : IOBS_OBJECT_WORK
    {
        public GMS_ENEMY_3D_WORK gmk_work;
        public GMS_EFFECT_3DES_WORK flame;
        public int region;
        public int burn_timer;

        public GMS_GMK_CANDLE_WORK()
        {
            this.gmk_work = new GMS_ENEMY_3D_WORK(this);
        }

        public static explicit operator OBS_OBJECT_WORK(GMS_GMK_CANDLE_WORK work)
        {
            return work.gmk_work.ene_com.obj_work;
        }

        public OBS_OBJECT_WORK Cast()
        {
            return this.gmk_work.ene_com.obj_work;
        }
    }
}
