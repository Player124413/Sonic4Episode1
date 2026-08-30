using static AppMain;

internal static partial class DmLoading
{
    internal enum DME_LOADING_DATA_TYPE
    {
        DME_LOADING_DATA_TYPE_CMN_DATA = 0,

        DME_LOADING_DATA_TYPE_MAX,
        DME_LOADING_DATA_TYPE_NONE,
    }

    // ACT_STOP sits between ACT_RUN and ACT_PERIOD1 in D_LOAD.AMA and is not created, which is why
    // dm_act_id_tbl skips id 4. see docs/startup-screen-variants.md
    internal enum DME_LOADING_ACT
    {
        ACT_BG_WHITE = 0,
        ACT_BG_BOTTOM,
        ACT_OBI,
        ACT_RUN,
        ACT_PERIOD1,
        ACT_PERIOD2,
        ACT_PERIOD3,

        ACT_TEXT_LOAD,

        ACT_NUM,
        ACT_NONE,
    }

    internal class DMS_LOADING_MAIN_WORK
    {
        public readonly AOS_ACTION[] act = new AOS_ACTION[(int)DME_LOADING_ACT.ACT_NUM];

        public _proc_ proc_update;
        public _proc_ proc_draw;

        public float timer;
        public uint flag;

        public float sonic_set_frame;
        public float sonic_pos_x;

        public bool is_maingame_load;
        public uint draw_state;

        public int lang_id;

        public delegate void _proc_(DMS_LOADING_MAIN_WORK work);
    }

    internal class DMS_LOADING_MGR
    {
        public MTS_TASK_TCB tcb;
    }
}
