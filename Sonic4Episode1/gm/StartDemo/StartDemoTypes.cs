using System;

using static AppMain;

internal static partial class GmStartDemo
{
    internal enum GME_START_DEMO_DATA_TYPE
    {
        GMD_START_DEMO_DATA_TYPE_CMN = 0,
        GMD_START_DEMO_DATA_TYPE_LANG,

        GMD_START_DEMO_DATA_TYPE_NUM
    }

    internal enum GME_START_DEMO_ACTION_INDEX_CMN
    {
        GMD_START_DEMO_ACTION_INDEX_CMN_BG_WHITE = 0,
        GMD_START_DEMO_ACTION_INDEX_CMN_BG_BLUE,
        GMD_START_DEMO_ACTION_INDEX_CMN_BG_RED,

        GMD_START_DEMO_ACTION_INDEX_CMN_TEX_ZONE,

        GMD_START_DEMO_ACTION_INDEX_CMN_NUM
    }

    internal enum GME_START_DEMO_ACTION_INDEX_ZONE
    {
        GMD_START_DEMO_ACTION_INDEX_ZONE_TEX_ZNAME = 0,

        GMD_START_DEMO_ACTION_INDEX_ZONE_NUM
    }

    internal enum GME_START_DEMO_ACTION_INDEX_ACT
    {
        GMD_START_DEMO_ACTION_INDEX_ACT_NUMBER = 0,
        GMD_START_DEMO_ACTION_INDEX_ACT_TEX_ACT,

        GMD_START_DEMO_ACTION_INDEX_ACT_NUM
    }

    internal class GMS_START_DEMO_DATA : IClearable
    {
        public readonly AOS_TEXTURE[] aos_texture =
            New<AOS_TEXTURE>((int)GME_START_DEMO_DATA_TYPE.GMD_START_DEMO_DATA_TYPE_NUM);

        public readonly AMS_AMB_HEADER[] demo_amb =
            new AMS_AMB_HEADER[(int)GME_START_DEMO_DATA_TYPE.GMD_START_DEMO_DATA_TYPE_NUM];

        public bool flag_regist;

        public void Clear()
        {
            Array.Clear(this.demo_amb, 0, this.demo_amb.Length);
            ClearArray(this.aos_texture);
            this.flag_regist = false;
        }
    }

    internal class GMS_START_DEMO_WORK
    {
        public readonly GMS_COCKPIT_2D_WORK[] action_obj_work_cmn =
            new GMS_COCKPIT_2D_WORK[(int)GME_START_DEMO_ACTION_INDEX_CMN.GMD_START_DEMO_ACTION_INDEX_CMN_NUM];

        public readonly GMS_COCKPIT_2D_WORK[] action_obj_work_zone =
            new GMS_COCKPIT_2D_WORK[(int)GME_START_DEMO_ACTION_INDEX_ZONE.GMD_START_DEMO_ACTION_INDEX_ZONE_NUM];

        public readonly GMS_COCKPIT_2D_WORK[] action_obj_work_act =
            new GMS_COCKPIT_2D_WORK[(int)GME_START_DEMO_ACTION_INDEX_ACT.GMD_START_DEMO_ACTION_INDEX_ACT_NUM];

        public GMS_COCKPIT_2D_WORK action_obj_work_message;

        public uint counter;
        public uint flag;

        public _update_ update;

        public delegate void _update_(GMS_START_DEMO_WORK work);
    }

    internal class GMS_START_DEMO_MGR : IClearable
    {
        public MTS_TASK_TCB main_tcb;

        public void Clear()
        {
            this.main_tcb = null;
        }
    }
}
