using er;

using static AppMain;

internal static partial class StgSlct
{
    public enum DME_STGSLCT_NEXT_EVT
    {
        DME_STGSLCT_NEXT_EVT_MAINGAME = 0,
        DME_STGSLCT_NEXT_EVT_SPESTE,
        DME_STGSLCT_NEXT_EVT_RANKING,
        DME_STGSLCT_NEXT_EVT_MAINMENU,
        DME_STGSLCT_NEXT_EVT_TITLE,

        DME_STGSLCT_NEXT_EVT_MAX
    }

    public enum DME_STGSLCT_DATA_TYPE
    {
        DME_STGSLCT_DATA_TYPE_CMN_DATA = 0,
        DME_STGSLCT_DATA_TYPE_LANG_DATA,

        DME_STGSLCT_DATA_TYPE_MAX,
        DME_STGSLCT_DATA_TYPE_NONE
    }

    public enum DME_STGSLCT_ZONE_TYPE
    {
        DME_STGSLCT_ZONE_TYPE_1 = 0,
        DME_STGSLCT_ZONE_TYPE_2,
        DME_STGSLCT_ZONE_TYPE_3,
        DME_STGSLCT_ZONE_TYPE_4,
        DME_STGSLCT_ZONE_TYPE_FINAL,
        DME_STGSLCT_ZONE_TYPE_SPE,

        DME_STGSLCT_ZONE_TYPE_NUM,
        DME_STGSLCT_ZONE_TYPE_NONE
    }

    public enum DME_STGSLCT_MODE_STATE
    {
        DME_STGSLCT_MODE_STATE_ZONE_SLCT = 0,
        DME_STGSLCT_MODE_STATE_ACT_SLCT,

        DME_STGSLCT_MODE_STATE_NUM,
        DME_STGSLCT_MODE_STATE_NONE
    }

    public enum DME_STGSLCT_PLAY_MODE
    {
        DME_STGSLCT_PLAY_MODE_NORMAL = 0,
        DME_STGSLCT_PLAY_MODE_TIME_ATK,

        DME_STGSLCT_PLAY_MODE_NUM,
        DME_STGSLCT_PLAY_MODE_NONE
    }

    public enum DME_STGSLCT_WIN
    {
        DME_STGSLCT_WIN_MENU = 0,
        DME_STGSLCT_WIN_STG_SLCT,
        DME_STGSLCT_WIN_1_1_CLEAR,
        DME_STGSLCT_WIN_BOSS1_CAN_PLAY,
        DME_STGSLCT_WIN_BOSS2_CAN_PLAY,
        DME_STGSLCT_WIN_BOSS3_CAN_PLAY,
        DME_STGSLCT_WIN_BOSS4_CAN_PLAY,
        DME_STGSLCT_WIN_FINAL_CAN_SLCT,
        DME_STGSLCT_WIN_SSONIC_SLCT,
        DME_STGSLCT_WIN_SPESTE_CAN_SLCT,

        DME_STGSLCT_WIN_NUM,
        DME_STGSLCT_WIN_NONE
    }

    public enum DME_STGSLCT_ACT_PAGE
    {
        DME_STGSLCT_ACT_PAGE_1 = 0,
        DME_STGSLCT_ACT_PAGE_2,
        DME_STGSLCT_ACT_PAGE_3,
        DME_STGSLCT_ACT_PAGE_4,
        DME_STGSLCT_ACT_PAGE_FINAL,
        DME_STGSLCT_ACT_PAGE_SPE,

        DME_STGSLCT_ACT_PAGE_NUM,
        DME_STGSLCT_ACT_PAGE_NONE
    }

    public enum DME_STGSLCT_STAGE
    {
        DME_STGSLCT_STAGE_1_1 = 0,
        DME_STGSLCT_STAGE_1_2,
        DME_STGSLCT_STAGE_1_3,
        DME_STGSLCT_STAGE_1_B,
        DME_STGSLCT_STAGE_2_1,
        DME_STGSLCT_STAGE_2_2,
        DME_STGSLCT_STAGE_2_3,
        DME_STGSLCT_STAGE_2_B,
        DME_STGSLCT_STAGE_3_1,
        DME_STGSLCT_STAGE_3_2,
        DME_STGSLCT_STAGE_3_3,
        DME_STGSLCT_STAGE_3_B,
        DME_STGSLCT_STAGE_4_1,
        DME_STGSLCT_STAGE_4_2,
        DME_STGSLCT_STAGE_4_3,
        DME_STGSLCT_STAGE_4_B,
        DME_STGSLCT_STAGE_F_1,
        DME_STGSLCT_STAGE_S_1,
        DME_STGSLCT_STAGE_S_2,
        DME_STGSLCT_STAGE_S_3,
        DME_STGSLCT_STAGE_S_4,
        DME_STGSLCT_STAGE_S_5,
        DME_STGSLCT_STAGE_S_6,
        DME_STGSLCT_STAGE_S_7,

        DME_STGSLCT_STAGE_NUM,
        DME_STGSLCT_STAGE_NONE
    }

    // mobile action set, 116 entries. paired with act_id_tbl_mobile: declaration order is data,
    // it decides which archive each action is built from, so nothing here may be reordered.
    // ACT_NONE deliberately lands on ACT_TAB_COVER2, see docs/stgslct-action-ids.md
    public enum DME_STGSLCT_ACT
    {
        ACT_ZONE_BG_LT = 0,
        ACT_ZONE_BG_LB,
        ACT_ZONE_BG_RT,
        ACT_ZONE_BG_RB,
        ACT_TAB_MODE_L,
        ACT_ICON_SONIC,
        ACT_REST_NUM_100,
        ACT_REST_NUM_10,
        ACT_REST_NUM_1,
        ACT_TAB_EMER,
        ACT_ICON_EMER_1,
        ACT_ICON_EMER_2,
        ACT_ICON_EMER_3,
        ACT_ICON_EMER_4,
        ACT_ICON_EMER_5,
        ACT_ICON_EMER_6,
        ACT_ICON_EMER_7,
        ACT_TEX_ZONE_UP,

        ACT_TEX_ZONE_UP_S,

        ACT_TAB_ZONE_SCR1,
        ACT_TAB_ZONE_SCR2,
        ACT_TAB_ZONE_SCR3,
        ACT_TAB_ZONE_SCR4,
        ACT_TAB_ZONE_SCR5,
        ACT_TAB_ZONE_SCR6,
        ACT_TAB_ZONE_SCR1_1a,
        ACT_TAB_ZONE_SCR1_2a,
        ACT_TAB_ZONE_SCR1_3a,
        ACT_TAB_ZONE_SCR2_1a,
        ACT_TAB_ZONE_SCR2_2a,
        ACT_TAB_ZONE_SCR2_3a,
        ACT_TAB_ZONE_SCR3_1a,
        ACT_TAB_ZONE_SCR3_2a,
        ACT_TAB_ZONE_SCR3_3a,
        ACT_TAB_ZONE_SCR4_1a,
        ACT_TAB_ZONE_SCR4_2a,
        ACT_TAB_ZONE_SCR4_3a,
        ACT_TAB_ZONE_TAB,
        ACT_TAB_ZONE_TEXT,
        ACT_TAB_ZONE_TEXT_S,
        ACT_TAB_ZONE_COVER1,
        ACT_TAB_ZONE_COVER2,
        ACT_TAB_ZONE_COVER3,

        ACT_ICON_DOWN_1,
        ACT_ICON_DOWN_2,
        ACT_ICON_DOWN_3,
        ACT_ICON_DOWN_4,
        ACT_ICON_DOWN_5,
        ACT_ICON_DOWN_6,
        ACT_ICON_L_ARROW,
        ACT_ICON_R_ARROW,
        ACT_TAB_STATE_L,
        ACT_TAB_STATE_C,
        ACT_TAB_STATE_R,
        ACT_TAB_STATE_L2,
        ACT_TAB_STATE_C2,
        ACT_TAB_STATE_R2,
        ACT_TAB_STATE_MOVE,
        ACT_TAB_TABLE2,
        ACT_TAB_TABLE1,
        ACT_TAB_TABLE3,
        ACT_TAB_SCR,
        ACT_TAB_SCR_BG,
        ACT_TAB_TEXT,
        ACT_TAB_A_NUM,
        ACT_TAB_MESS,
        ACT_TAB_LINE,
        ACT_TAB_ICON_EMER,
        ACT_TAB_NUM_SPE_STAGE,
        ACT_TAB_ICON_SPE_EMER,
        ACT_TAB_CURSOR_UP,
        ACT_TAB_CURSOR_DOWN,
        ACT_TAB_TEX_SCORE,
        ACT_TAB_TEX_TIME,
        ACT_TAB_TEX_BOSS,
        ACT_TAB_TEX_SPE_STAGE,
        ACT_TAB_S_NUM1,
        ACT_TAB_S_NUM2,
        ACT_TAB_S_NUM3,
        ACT_TAB_S_NUM4,
        ACT_TAB_S_NUM5,
        ACT_TAB_S_NUM6,
        ACT_TAB_S_NUM7,
        ACT_TAB_S_NUM8,
        ACT_TAB_S_NUM9,
        ACT_TAB_COVER2,
        ACT_TAB_COVER1,
        ACT_TAB_COVER3,

        ACT_TEX_BIG_TIME,
        ACT_TEX_BIG_SCORE,

        ACT_WIN_TEX_MSG,
        ACT_WIN_TEX_MSG2,
        ACT_WIN_TEX_MSG_SSONIC,

        ACT_WAVE_BG,
        ACT_DOWN_BG,
        ACT_BLUE_BG,

        ACT_BTN_CANCEL1,
        ACT_BTN_LB,
        ACT_BTN_MENU,
        ACT_BTN_LB_ARROW,
        ACT_BTN_RB_ARROW,
        ACT_BTN_CANCEL2,
        ACT_BTN_X,
        ACT_BTN_Y,

        ACT_BACK_BTN01_L,
        ACT_BACK_BTN01_R,
        ACT_YES_BTN_L,
        ACT_YES_BTN_C,
        ACT_YES_BTN_R,
        ACT_NO_BTN_L,
        ACT_NO_BTN_C,
        ACT_NO_BTN_R,

        ACT_TEX_FIX_BACK,
        ACT_TEX_BACK1,
        ACT_TEX_YES,
        ACT_TEX_NO,

        ACT_NUM,

        ACT_TAB_START = ACT_TAB_TABLE2,
        ACT_TAB_END = ACT_TAB_S_NUM9,

        ACT_NONE
    }

    // console action set, 130 entries. adds the cursors, the obi banner, the mode text effects and
    // the window chrome; drops the eight touch button sprites. paired with act_id_tbl_console.
    public enum DME_STGSLCT_ACT_REG
    {
        ACT_ZONE_BG_LT = 0,
        ACT_ZONE_BG_LB,
        ACT_ZONE_BG_RT,
        ACT_ZONE_BG_RB,
        ACT_TAB_MODE_L,
        ACT_TAB_MODE_R,
        ACT_ICON_SONIC,
        ACT_TEX_SONIC,
        ACT_REST_NUM_100,
        ACT_REST_NUM_10,
        ACT_REST_NUM_1,
        ACT_TAB_EMER,
        ACT_ICON_EMER_1,
        ACT_ICON_EMER_2,
        ACT_ICON_EMER_3,
        ACT_ICON_EMER_4,
        ACT_ICON_EMER_5,
        ACT_ICON_EMER_6,
        ACT_ICON_EMER_7,
        ACT_TEX_ZONE_UP,

        ACT_TEX_MODE_TIME,
        ACT_TEX_MODE_SCORE,
        ACT_TEX_MENU,
        ACT_TEX_EMER,
        ACT_TEX_ZONE_UP_S,
        ACT_TEX_OBI1,
        ACT_TEX_OBI2,

        ACT_TAB_ZONE_SCR1,
        ACT_TAB_ZONE_SCR2,
        ACT_TAB_ZONE_SCR3,
        ACT_TAB_ZONE_SCR4,
        ACT_TAB_ZONE_SCR5,
        ACT_TAB_ZONE_SCR6,
        ACT_TAB_ZONE_SCR1_1a,
        ACT_TAB_ZONE_SCR1_2a,
        ACT_TAB_ZONE_SCR1_3a,
        ACT_TAB_ZONE_SCR2_1a,
        ACT_TAB_ZONE_SCR2_2a,
        ACT_TAB_ZONE_SCR2_3a,
        ACT_TAB_ZONE_SCR3_1a,
        ACT_TAB_ZONE_SCR3_2a,
        ACT_TAB_ZONE_SCR3_3a,
        ACT_TAB_ZONE_SCR4_1a,
        ACT_TAB_ZONE_SCR4_2a,
        ACT_TAB_ZONE_SCR4_3a,
        ACT_TAB_ZONE_TAB,
        ACT_TAB_ZONE_TEXT,
        ACT_TAB_ZONE_TEXT_S,
        ACT_TAB_ZONE_COVER1,
        ACT_TAB_ZONE_COVER2,
        ACT_TAB_ZONE_COVER3,
        ACT_TAB_ZONE_CURSOR2,
        ACT_TAB_ZONE_CURSOR1,
        ACT_TAB_ZONE_CURSOR3,

        ACT_ICON_DOWN_1,
        ACT_ICON_DOWN_2,
        ACT_ICON_DOWN_3,
        ACT_ICON_DOWN_4,
        ACT_ICON_DOWN_5,
        ACT_ICON_DOWN_6,
        ACT_ICON_L_ARROW,
        ACT_ICON_R_ARROW,
        ACT_TAB_STATE_L,
        ACT_TAB_STATE_C,
        ACT_TAB_STATE_R,
        ACT_TAB_STATE_L2,
        ACT_TAB_STATE_C2,
        ACT_TAB_STATE_R2,
        ACT_TAB_STATE_MOVE,
        ACT_TAB_TABLE2,
        ACT_TAB_TABLE1,
        ACT_TAB_TABLE3,
        ACT_TAB_SCR,
        ACT_TAB_SCR_BG,
        ACT_TAB_TEXT,
        ACT_TAB_A_NUM,
        ACT_TAB_MESS,
        ACT_TAB_LINE,
        ACT_TAB_ICON_EMER,
        ACT_TAB_NUM_SPE_STAGE,
        ACT_TAB_ICON_SPE_EMER,
        ACT_TAB_CURSOR_UP,
        ACT_TAB_CURSOR_DOWN,
        ACT_TAB_CURSOR_2,
        ACT_TAB_CURSOR_1,
        ACT_TAB_CURSOR_3,
        ACT_TAB_TEX_SCORE,
        ACT_TAB_TEX_TIME,
        ACT_TAB_TEX_BOSS,
        ACT_TAB_TEX_SPE_STAGE,
        ACT_TAB_S_NUM1,
        ACT_TAB_S_NUM2,
        ACT_TAB_S_NUM3,
        ACT_TAB_S_NUM4,
        ACT_TAB_S_NUM5,
        ACT_TAB_S_NUM6,
        ACT_TAB_S_NUM7,
        ACT_TAB_S_NUM8,
        ACT_TAB_S_NUM9,
        ACT_TAB_COVER2,
        ACT_TAB_COVER1,
        ACT_TAB_COVER3,

        ACT_TEX_BIG_TIME,
        ACT_TEX_BIG_SCORE,
        ACT_TEX_TIME_EFCT,
        ACT_TEX_SCORE_EFCT,

        ACT_WIN_TEX_MSG,
        ACT_WIN_TEX_MSG2,
        ACT_WIN_TEX_MSG_SSONIC,

        ACT_WAVE_BG,
        ACT_DOWN_BG,
        ACT_BLUE_BG,

        ACT_BTN_CANCEL1,
        ACT_BTN_LB,
        ACT_BTN_MENU,
        ACT_BTN_LB_ARROW,
        ACT_BTN_RB_ARROW,
        ACT_BTN_CANCEL2,
        ACT_BTN_X,
        ACT_BTN_Y,

        ACT_OBI_C,
        ACT_OBI_L,
        ACT_OBI_R,
        ACT_WIN_LINE,

        ACT_TEX_FIX_BACK,
        ACT_TEX_WINTITLE,
        ACT_TEX_BACK1,
        ACT_TEX_OK,
        ACT_TEX_YES,
        ACT_TEX_NO,

        ACT_NUM,

        ACT_TAB_START = ACT_TAB_TABLE2,
        ACT_TAB_END = ACT_TAB_S_NUM9,

        ACT_NONE
    }

    internal class DMS_STGSLCT_MAIN_WORK
    {
        public readonly AMS_FS[] arc_cmn_amb_fs = new AMS_FS[5];
        public readonly AMS_AMB_HEADER[] arc_cmn_amb = new AMS_AMB_HEADER[5];
        public readonly A2S_AMA_HEADER[] cmn_ama = new A2S_AMA_HEADER[5];
        public readonly AMS_AMB_HEADER[] cmn_amb = new AMS_AMB_HEADER[5];
        public readonly AOS_TEXTURE[] cmn_tex = New<AOS_TEXTURE>(5);
        public readonly AMS_FS[] arc_amb_fs = new AMS_FS[2];
        public readonly AMS_AMB_HEADER[] arc_amb = new AMS_AMB_HEADER[2];
        public readonly A2S_AMA_HEADER[] ama = new A2S_AMA_HEADER[2];
        public readonly AMS_AMB_HEADER[] amb = new AMS_AMB_HEADER[2];
        public readonly AOS_TEXTURE[] tex = New<AOS_TEXTURE>(2);
        public readonly AOS_TEXTURE win_tex = new AOS_TEXTURE();

        // sized from the variant's ACT_NUM, so it cannot be allocated here
        public AOS_ACTION[] act;

        // null on console
        public StgSlctTouch touch;

        public readonly int[] n_sonic_hi_score = new int[17];
        public readonly int[] s_sonic_hi_score = new int[17];
        public readonly int[] n_sonic_record_time = new int[17];
        public readonly int[] s_sonic_record_time = new int[17];
        public readonly int[] hi_score = new int[24];
        public readonly int[] record_time = new int[24];
        public readonly int[] is_clear_stage = new int[24];
        public readonly uint[] eme_stage_no = new uint[7];

        // 13 rows against dm_stgslct_win_act_pos_tbl's 14. SEGA wrote it as [14-1], deliberately
        public readonly float[,] win_act_pos = new float[13, 2];

        public readonly float[] win_size_rate = new float[2];
        public readonly float[][] zone_pos = New<float>(6, 2);
        public readonly float[] move_spd = new float[2];
        public readonly float[] act_top_pos_x = new float[24];
        public readonly float[] act_top_pos_y = new float[24];
        public readonly float[] act_move_src = new float[2];
        public readonly float[] act_move_dest = new float[2];
        public readonly float[] act_move_pos_src = new float[24];
        public readonly float[] act_move_pos_dst = new float[24];
        public readonly float[] obi_pos = new float[2];
        public readonly AMS_PARAM_DRAW_PRIMITIVE up_bg_vrtx = new AMS_PARAM_DRAW_PRIMITIVE();
        public readonly float[] tex_u = new float[2];
        public readonly float[] tex_v = new float[2];

        public AMS_FS win_amb_fs;
        public AMS_AMB_HEADER win_amb;

        public _proc_ proc_win_input;
        public _proc_ proc_input;
        public _proc_ proc_win_update;
        public _proc_ proc_menu_update;
        public _proc_ proc_draw;

        public int timer;
        public uint flag;
        public int state;
        public float win_timer;
        public uint disp_flag;
        public int bg_timer;
        public int zone_scr_timer;
        public uint announce_flag;
        public int next_evt;
        public int prev_evt;
        public int is_final_open;
        public uint get_emerald;
        public uint cur_game_mode;
        public uint player_stock;
        public int win_mode;
        public int win_cur_slct;
        public uint cur_zone;
        public uint chng_zone;
        public uint efct_time;
        public uint efct_out_flag;
        public float chaos_eme_pos_y;
        public float mode_tex_pos_y;
        public float mode_tex_frm;
        public int cur_stage;
        public int prev_stage;
        public int cur_vrtcl_stage;
        public int prev_vrtcl_stage;
        public int crsr_idx;
        public int crsr_prev_idx;
        public float crsr_pos_y;
        public float crsr_move_src;
        public float crsr_move_dst;
        public int focus_disp_no;
        public int prev_disp_no;
        public int decide_zone_efct_dist_x;
        public int decide_zone_efct_dist_y;
        public AOS_ACT_COL bg_fade;
        public uint cur_bg_id;
        public uint next_bg_id;
        public uint zone_scr_id;
        public uint mode_tex_move_frm;
        public uint btn_l_disp_frm;
        public uint btn_r_disp_frm;
        public bool is_jp_region;

        public delegate void _proc_(DMS_STGSLCT_MAIN_WORK work);
    }
}
