using System;

using static AppMain;

internal static partial class GmOver
{
    internal enum GME_OVER_TYPE
    {
        GME_OVER_TYPE_GAMEOVER = 0,
        GME_OVER_TYPE_TIMEOVER,

        GME_OVER_TYPE_MAX
    }

    internal enum GME_OVER_TEX_IDX
    {
        GME_OVER_TEX_IDX_COMMON = 0,
        GME_OVER_TEX_IDX_STRING,

        GME_OVER_TEX_IDX_MAX
    }

    // the C++ declares GME_OVER_AMA_IDX separately with the same members and values; one enum does
    internal enum GME_OVER_STRING_SUBPART
    {
        GME_OVER_STRING_SUBPART_GOV_GAME = 0,
        GME_OVER_STRING_SUBPART_GOV_OVER,
        GME_OVER_STRING_SUBPART_TOV_TIME,
        GME_OVER_STRING_SUBPART_TOV_OVER,

        GME_OVER_STRING_SUBPART_MAX
    }

    internal enum GME_OVER_FADEOUT_SUBPART
    {
        GME_OVER_FADEOUT_SUBPART_GOV_FOUT = 0,
        GME_OVER_FADEOUT_SUBPART_TOV_FOUT,

        GME_OVER_FADEOUT_SUBPART_MAX
    }

    internal class GMS_OVER_MGR_WORK
    {
        public readonly GMS_COCKPIT_2D_WORK[] string_sub_parts =
            new GMS_COCKPIT_2D_WORK[(int)GME_OVER_STRING_SUBPART.GME_OVER_STRING_SUBPART_MAX];

        public readonly GMS_COCKPIT_2D_WORK[] fadeout_sub_parts =
            new GMS_COCKPIT_2D_WORK[(int)GME_OVER_FADEOUT_SUBPART.GME_OVER_FADEOUT_SUBPART_MAX];

        public _proc_ proc_update;

        // runs even when proc_update has stopped
        public _proc_ proc_disp;

        public uint wait_timer;

        public void Clear()
        {
            this.proc_update = null;
            this.proc_disp = null;
            this.wait_timer = 0U;
            Array.Clear(this.string_sub_parts, 0, this.string_sub_parts.Length);
            Array.Clear(this.fadeout_sub_parts, 0, this.fadeout_sub_parts.Length);
        }

        public delegate void _proc_(GMS_OVER_MGR_WORK mgr_work);
    }
}
