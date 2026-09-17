internal partial class AppMain
{
    internal class GMS_GMK_DYNAMITE_WORK : IOBS_OBJECT_WORK
    {
        public GMS_ENEMY_3D_WORK gmk_work;
        public int chain;
        public int row;
        public uint fuse_flag;
        public int entry_dir;
        public int exit_dir;

        // ev rect_w, which the two families read differently: a line piece burns at param/10 times
        // material motion speed, a dynamite waits param frames after the blast before lighting row+1
        public int param;

        public int fuse_timer;
        public int blast_row_timer;
        public int blast_rect_timer;
        public GSS_SND_SE_HANDLE h_snd_fuse;

        public GMS_GMK_DYNAMITE_WORK()
        {
            this.gmk_work = new GMS_ENEMY_3D_WORK(this);
        }

        public static explicit operator OBS_OBJECT_WORK(GMS_GMK_DYNAMITE_WORK work)
        {
            return work.gmk_work.ene_com.obj_work;
        }

        public OBS_OBJECT_WORK Cast()
        {
            return this.gmk_work.ene_com.obj_work;
        }
    }
}
