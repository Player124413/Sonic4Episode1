using System;

using static AppMain;

internal static partial class DmTitleOp
{
    internal class DMS_TITLEOP_MGR_WORK
    {
        public OBS_OBJECT_WORK[] obj_work = new OBS_OBJECT_WORK[5];
        public AOS_ACTION[] act = new AOS_ACTION[7];
        public int frame;
        public uint flag;
        public float finger_frame;

        public void Clear()
        {
            this.frame = 0;
            this.flag = 0U;
            Array.Clear(obj_work, 0, this.obj_work.Length);
            Array.Clear(act, 0, this.act.Length);
            this.finger_frame = 0.0f;
        }
    }

    internal class DMS_TITLEOP_OBJ_3DNN_WORK : IOBS_OBJECT_WORK
    {
        public OBS_ACTION3D_NN_WORK obj_3d = new OBS_ACTION3D_NN_WORK();
        public readonly OBS_OBJECT_WORK obj_work;
        public DMS_TITLEOP_ROCK_SETTING[] rock_setting;
        public int rock_setting_num;
        public float sky_rot;

        public OBS_OBJECT_WORK Cast()
        {
            return this.obj_work;
        }

        public static explicit operator OBS_OBJECT_WORK(
          DMS_TITLEOP_OBJ_3DNN_WORK work)
        {
            return work.obj_work;
        }

        public static explicit operator DMS_TITLEOP_OBJ_3DNN_WORK(
          OBS_OBJECT_WORK work)
        {
            return (DMS_TITLEOP_OBJ_3DNN_WORK)work.holder;
        }

        public DMS_TITLEOP_OBJ_3DNN_WORK()
        {
            this.obj_work = OBS_OBJECT_WORK.Create(this);
        }
    }

    internal class DMS_TITLEOP_ROCK_SETTING
    {
        public VecFx32 pos = new VecFx32();
        public VecFx32 scale = new VecFx32();

        public DMS_TITLEOP_ROCK_SETTING(uint x1, uint y1, uint z1, uint x2, uint y2, uint z2)
        {
            this.pos = new VecFx32((int)x1, (int)y1, (int)z1);
            this.scale = new VecFx32((int)x2, (int)y2, (int)z2);
        }
    }
}
