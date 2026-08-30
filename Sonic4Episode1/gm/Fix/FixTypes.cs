using System;

using static AppMain;

internal static partial class GmFix
{
    internal enum GME_FIX_PART_TYPE
    {
        GME_FIX_PART_TYPE_RINGCOUNT = 0,
        GME_FIX_PART_TYPE_SCORE,
        GME_FIX_PART_TYPE_TIMER,
        GME_FIX_PART_TYPE_CHALLENGE,
        GME_FIX_PART_TYPE_VIRTUAL_PAD,

        GME_FIX_PART_TYPE_MAX,
        GME_FIX_PART_TYPE_NONE = -1
    }

    // the C++ declares GME_FIX_AMA_IDX separately with the same members and values
    internal enum GME_FIX_TEX_IDX
    {
        GME_FIX_TEX_IDX_FIX = 0,
        GME_FIX_TEX_IDX_SSFIX_CMN,
        GME_FIX_TEX_IDX_SSFIX_LANG,

        GME_FIX_TEX_IDX_MAX
    }

    // which on-screen control layout the HUD is laid out for. TILT is also what a pad plays with
    internal enum GME_FIX_PLAN
    {
        GME_FIX_PLAN_TILT = 0,
        GME_FIX_PLAN_FLICK,
        GME_FIX_PLAN_VIRTUAL_PAD,

        GME_FIX_PLAN_MAX
    }

    internal enum GME_FIX_RING_SUBPART
    {
        GME_FIX_RING_SUBPART_ICON = 0,
        GME_FIX_RING_SUBPART_DIGIT_100,
        GME_FIX_RING_SUBPART_DIGIT_10,
        GME_FIX_RING_SUBPART_DIGIT_1,

        GME_FIX_RING_SUBPART_MAX
    }

    internal enum GME_FIX_SCORE_SUBPART
    {
        GME_FIX_SCORE_SUBPART_TAB = 0,
        GME_FIX_SCORE_SUBPART_NUM_1,
        GME_FIX_SCORE_SUBPART_NUM_2,
        GME_FIX_SCORE_SUBPART_NUM_3,
        GME_FIX_SCORE_SUBPART_NUM_4,
        GME_FIX_SCORE_SUBPART_NUM_5,
        GME_FIX_SCORE_SUBPART_NUM_6,
        GME_FIX_SCORE_SUBPART_NUM_7,
        GME_FIX_SCORE_SUBPART_NUM_8,
        GME_FIX_SCORE_SUBPART_NUM_9,

        GME_FIX_SCORE_SUBPART_MAX
    }

    internal enum GME_FIX_TIMER_SUBPART
    {
        GME_FIX_TIMER_SUBPART_ICON = 0,
        GME_FIX_TIMER_SUBPART_NUM_10000,
        GME_FIX_TIMER_SUBPART_NUM_AP1,
        GME_FIX_TIMER_SUBPART_NUM_1000,
        GME_FIX_TIMER_SUBPART_NUM_100,
        GME_FIX_TIMER_SUBPART_NUM_AP2,
        GME_FIX_TIMER_SUBPART_NUM_10,
        GME_FIX_TIMER_SUBPART_NUM_1,

        GME_FIX_TIMER_SUBPART_MAX
    }

    internal enum GME_FIX_CHALLENGE_SUBPART
    {
        GME_FIX_CHALLENGE_SUBPART_SONIC = 0,
        GME_FIX_CHALLENGE_SUBPART_STRING,
        GME_FIX_CHALLENGE_SUBPART_NUM_100,
        GME_FIX_CHALLENGE_SUBPART_NUM_10,
        GME_FIX_CHALLENGE_SUBPART_NUM_1,

        GME_FIX_CHALLENGE_SUBPART_MAX
    }

    internal enum GME_FIX_VIRTUAL_PAD_SUBPART
    {
        GME_FIX_VIRTUAL_PAD_SUBPART_SSONIC = 0,
        GME_FIX_VIRTUAL_PAD_SUBPART_PAUSE,
        GME_FIX_VIRTUAL_PAD_SUBPART_ACTION,
        GME_FIX_VIRTUAL_PAD_SUBPART_MOVE,

        GME_FIX_VIRTUAL_PAD_SUBPART_MAX
    }

    // The C++ puts GMS_FIX_PART_WORK first in each part struct and casts between the two. The
    // decompile emulated that with a `holder` object and a pair of explicit operators per part;
    // inheritance says the same thing and the casts become ordinary ones.
    internal class GMS_FIX_PART_WORK
    {
        public GMS_FIX_MGR_WORK parent_mgr;
        public GME_FIX_PART_TYPE part_type;
        public uint flag;

        public uint blink_timer;
        public uint blink_on_time;
        public uint blink_off_time;

        public _proc_ proc_update;
        public _proc_ proc_disp;

        public virtual void Clear()
        {
            this.parent_mgr = null;
            this.part_type = GME_FIX_PART_TYPE.GME_FIX_PART_TYPE_RINGCOUNT;
            this.flag = 0U;
            this.blink_timer = this.blink_on_time = this.blink_off_time = 0U;
            this.proc_update = this.proc_disp = null;
        }

        public delegate void _proc_(GMS_FIX_PART_WORK part_work);
    }

    internal class GMS_FIX_PART_RINGCOUNT : GMS_FIX_PART_WORK
    {
        public readonly GMS_COCKPIT_2D_WORK[] sub_parts =
            new GMS_COCKPIT_2D_WORK[(int)GME_FIX_RING_SUBPART.GME_FIX_RING_SUBPART_MAX];

        public readonly int[] digit_list = new int[GMD_FIX_PART_RINGCOUNT_DIGIT_NUM];

        public override void Clear()
        {
            base.Clear();
            Array.Clear(this.sub_parts, 0, this.sub_parts.Length);
            Array.Clear(this.digit_list, 0, this.digit_list.Length);
        }
    }

    internal class GMS_FIX_PART_SCORE : GMS_FIX_PART_WORK
    {
        public readonly GMS_COCKPIT_2D_WORK[] sub_parts =
            new GMS_COCKPIT_2D_WORK[(int)GME_FIX_SCORE_SUBPART.GME_FIX_SCORE_SUBPART_MAX];

        public readonly int[] digit_list = new int[GMD_FIX_PART_SCORE_DIGIT_NUM];

        public override void Clear()
        {
            base.Clear();
            Array.Clear(this.sub_parts, 0, this.sub_parts.Length);
            Array.Clear(this.digit_list, 0, this.digit_list.Length);
        }
    }

    internal class GMS_FIX_PART_TIMER : GMS_FIX_PART_WORK
    {
        public readonly GMS_COCKPIT_2D_WORK[] sub_parts =
            new GMS_COCKPIT_2D_WORK[(int)GME_FIX_TIMER_SUBPART.GME_FIX_TIMER_SUBPART_MAX];

        public readonly int[] digit_list = new int[GMD_FIX_PART_TIMER_ALL_DIGIT_NUM];

        public uint timer_flag;

        public float digit_frame_ofst;
        public float deco_char_frame_ofst;

        public ushort cur_sec;

        public float fade_ratio;
        public float scale_ratio;
        public uint flash_act_phase;

        public override void Clear()
        {
            base.Clear();
            Array.Clear(this.sub_parts, 0, this.sub_parts.Length);
            Array.Clear(this.digit_list, 0, this.digit_list.Length);
            this.timer_flag = 0U;
            this.digit_frame_ofst = this.deco_char_frame_ofst = 0.0f;
            this.cur_sec = 0;
            this.fade_ratio = this.scale_ratio = 0.0f;
            this.flash_act_phase = 0U;
        }
    }

    internal class GMS_FIX_PART_CHALLENGE : GMS_FIX_PART_WORK
    {
        public readonly GMS_COCKPIT_2D_WORK[] sub_parts =
            new GMS_COCKPIT_2D_WORK[(int)GME_FIX_CHALLENGE_SUBPART.GME_FIX_CHALLENGE_SUBPART_MAX];

        public readonly int[] digit_list = new int[GMD_FIX_PART_CHALLENGE_DIGIT_NUM];

        public override void Clear()
        {
            base.Clear();
            Array.Clear(this.sub_parts, 0, this.sub_parts.Length);
            Array.Clear(this.digit_list, 0, this.digit_list.Length);
        }
    }

    internal class GMS_FIX_PART_VIRTUAL_PAD : GMS_FIX_PART_WORK
    {
        public readonly GMS_COCKPIT_2D_WORK[] sub_parts =
            new GMS_COCKPIT_2D_WORK[(int)GME_FIX_VIRTUAL_PAD_SUBPART.GME_FIX_VIRTUAL_PAD_SUBPART_MAX];

        public readonly float[] pause_icon_frame = new float[2];

        public override void Clear()
        {
            base.Clear();
            Array.Clear(this.sub_parts, 0, this.sub_parts.Length);
            this.pause_icon_frame[0] = this.pause_icon_frame[1] = 0.0f;
        }
    }

    internal class GMS_FIX_MGR_WORK
    {
        public readonly GMS_FIX_PART_WORK[] part_work =
            new GMS_FIX_PART_WORK[(int)GME_FIX_PART_TYPE.GME_FIX_PART_TYPE_MAX];

        public readonly GMS_FIX_PART_RINGCOUNT part_ringcount = new GMS_FIX_PART_RINGCOUNT();
        public readonly GMS_FIX_PART_SCORE part_score = new GMS_FIX_PART_SCORE();
        public readonly GMS_FIX_PART_TIMER part_timer = new GMS_FIX_PART_TIMER();
        public readonly GMS_FIX_PART_CHALLENGE part_challenge = new GMS_FIX_PART_CHALLENGE();
        public readonly GMS_FIX_PART_VIRTUAL_PAD part_virtual_pad = new GMS_FIX_PART_VIRTUAL_PAD();

        public uint flag;
        public uint req_flag;

        public void Clear()
        {
            this.flag = this.req_flag = 0U;
            Array.Clear(this.part_work, 0, this.part_work.Length);
            this.part_ringcount.Clear();
            this.part_score.Clear();
            this.part_timer.Clear();
            this.part_challenge.Clear();
            this.part_virtual_pad.Clear();
        }
    }

    internal struct SKeyToFrame
    {
        public int key;
        public float frame;
    }
}
