using static AppMain;

internal static partial class DmSave
{
    internal enum DME_SAVE_ACT
    {
        ACT_WIN_LINE = 0,
        ACT_TEX_WINTITLE,
        ACT_TEX_MSG1,
        ACT_TEX_MSG2,
        ACT_TEX_MSG3,
        ACT_TEX_OK,

        ACT_NUM,
        ACT_NONE
    }

    internal enum DME_SAVE_WIN
    {
        DME_SAVE_WIN_DATA_LOADING = 0,
        DME_SAVE_WIN_ATTENTION_SAVE,
        DME_SAVE_WIN_NOW_SAVING,
        DME_SAVE_WIN_TRIAL_OUT_SAVE,

        DME_SAVE_WIN_NUM,
        DME_SAVE_WIN_NONE
    }

    internal class DMS_SAVE_MAIN_WORK
    {
        public readonly AMS_FS[] arc_cmn_amb_fs = new AMS_FS[2];
        public readonly AMS_AMB_HEADER[] arc_cmn_amb = new AMS_AMB_HEADER[2];
        public readonly A2S_AMA_HEADER[] cmn_ama = new A2S_AMA_HEADER[2];
        public readonly AMS_AMB_HEADER[] cmn_amb = new AMS_AMB_HEADER[2];
        public readonly AOS_TEXTURE[] cmn_tex = New<AOS_TEXTURE>(2);
        public readonly AOS_ACTION[] act = new AOS_ACTION[(int)DME_SAVE_ACT.ACT_NUM];

        // the C++ sizes this 5 against a 6-action enum and never reads it
        public readonly float[][] win_act_pos = New<float>(5, 2);

        public readonly float[] win_size_rate = new float[2];

        public _proc_ proc_input;
        public _proc_ proc_menu_update;
        public _draw_ proc_draw;

        public uint flag;
        public uint announce_flag;
        public uint disp_flag;

        public int state;

        public int timer;
        public int win_timer;

        public int win_mode;
        public int win_cur_slct;

        public uint draw_state;

        public GSS_SND_SCB bgm_scb;

        public delegate void _proc_(DMS_SAVE_MAIN_WORK main_work);

        public delegate void _draw_();
    }

    internal class DMS_SAVE_MGR
    {
        public MTS_TASK_TCB tcb;

        internal void Clear()
        {
            this.tcb = null;
        }
    }

    internal sealed class DMS_SAVE_VARIANT
    {
        public uint[] act_id_tbl;

        // DMD_SAVE_DISP_SCALE_TEXT is defined only for _WII and _IPHONE, so console has no scale
        // at all and 1.0 makes both the window multiply and the per-action AoActAcmApplyScale
        // no-ops
        public float text_scale;

        // only mobile has the GE special case that widens the non-attention window
        public bool ge_widens_window;

        // what every archive this screen loads gets pinned to. the global am_fs_variant cannot be
        // trusted here: it stays "console" even when this screen has fallen back to mobile, and
        // D_CMN_WIN would then arrive with ids the mobile table cannot index
        public string fs_variant;
    }
}
