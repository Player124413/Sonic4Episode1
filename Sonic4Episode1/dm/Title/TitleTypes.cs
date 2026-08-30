using er;

using static AppMain;

internal static partial class DmTitle
{
    internal enum DME_TITLE_ACT
    {
        ACT_BTN_L = 0,
        ACT_BTN_C,
        ACT_BTN_R,
        ACT_BTN_L2,
        ACT_BTN_C2,
        ACT_BTN_R2,
        ACT_BTN_L3,
        ACT_BTN_C3,
        ACT_BTN_R3,
        ACT_BTN_L4,
        ACT_BTN_C4, // 0x0000000A
        ACT_BTN_R4, // 0x0000000B
        ACT_BTN_L5, // 0x0000000C
        ACT_BTN_C5, // 0x0000000D
        ACT_BTN_R5, // 0x0000000E
        ACT_BACK_BTN_L, // 0x0000000F
        ACT_BACK_BTN_R, // 0x00000010
        ACT_GAME_BTN_L, // 0x00000011
        ACT_GAME_BTN_R, // 0x00000012
        ACT_LANG, // 0x00000013
        // C++ aliased these onto the member above; keep them equal or every value after drifts
        ACT_TEX_START = ACT_LANG, // 0x00000013
        ACT_NONE, // 0x00000014
        ACT_TEX_GAME = ACT_NONE, // 0x00000014
        ACT_TEX_TUDUKI, // 0x00000015
        ACT_TEX_OPTION, // 0x00000016
        ACT_TEX_KANZENBAN, // 0x00000017
        ACT_TEX_ACHIEVEMENTS, // 0x00000018
        ACT_TEX_LEADERBOARDS, // 0x00000019
        ACT_TEX_TOP_BACK, // 0x0000001A
        ACT_TEX_TOP_GAME, // 0x0000001B
        ACT_WIN_TEX_MSG1, // 0x0000001C
        ACT_WIN_TEX_MSG2, // 0x0000001D
        ACT_BTN_CANCEL, // 0x0000001E
        ACT_BTN_X, // 0x0000001F
        ACT_WIN_NO_BTN_L, // 0x00000020
        ACT_WIN_NO_BTN_C, // 0x00000021
        ACT_WIN_NO_BTN_R, // 0x00000022
        ACT_WIN_YES_BTN_L, // 0x00000023
        ACT_WIN_YES_BTN_C, // 0x00000024
        ACT_WIN_YES_BTN_R, // 0x00000025
        ACT_TEX_YES, // 0x00000026
        ACT_TEX_NO, // 0x00000027
        ACT_TEX_BACK, // 0x00000028
        ACT_NUM, // 0x00000029
    }

    internal enum DME_TITLE_ACT_REG
    {
        ACT_MENU_CRSR = 0,
	    ACT_TEX_START,
	    ACT_TEX_GAME,
	    ACT_TEX_GAME_L,
	    ACT_TEX_TUDUKI,
	    ACT_TEX_TUDUKI_L,
	    ACT_TEX_OPTION,
	    ACT_TEX_OPTION_L,
	    ACT_TEX_LIBRARY,
	    ACT_TEX_LIBRARY_L,
	    ACT_TEX_BUY,
	    ACT_TEX_BUY_L,
	    ACT_WIN_TEX_MSG1,
	    ACT_WIN_TEX_MSG2,
	    ACT_WIN_TEX_MSG3,
        // メニュー共通データ
        ACT_BTN_CANCEL,         //!< 
        ACT_BTN_X,              //!< 
        ACT_WIN_LINE,           //!< 
        ACT_TEX_WINTITLE,       //!< 
        ACT_TEX_YES,            //!< 
        ACT_TEX_NO,             //!< 
        ACT_TEX_BACK,           //!< 
        ACT_TEX_OK,             //!< 


        ACT_NUM,

        ACT_LANG = ACT_TEX_START,

        ACT_NONE
    }

    internal enum DME_TITLE_DATA_TYPE
    {
        DME_TITLE_DATA_TYPE_CMN_DATA,
        DME_TITLE_DATA_TYPE_LANG_DATA,
        DME_TITLE_DATA_TYPE_MAX,
        DME_TITLE_DATA_TYPE_NONE,
    }

    internal enum DME_TITLE_MENU_TYPE
    {
        DME_TITLE_MENU_TYPE_START,
        DME_TITLE_MENU_TYPE_TUDUKI,
        DME_TITLE_MENU_TYPE_OPTION,

        // the last two slots hold different items per build, so they are named by position and
        // dm_title_variant.menu_actions says what each one actually does
        DME_TITLE_MENU_TYPE_EXTRA1,
        DME_TITLE_MENU_TYPE_EXTRA2,

        DME_TITLE_MENU_TYPE_NUM,
        DME_TITLE_MENU_TYPE_NONE,
    }

    // what a menu slot does, as opposed to where it sits. console fills EXTRA1/EXTRA2 with
    // LIBRARY and BUY; WP7 mobile fills them with the two Xbox Live panels it added
    internal enum DME_TITLE_MENU_ACTION
    {
        NEW_GAME,
        CONTINUE,
        OPTION,
        ACHIEVEMENTS,
        LEADERBOARDS,
        LIBRARY,
        BUY,
    }

    internal enum DME_TITLE_WIN_TYPE
    {
        DME_TITLE_WIN_TYPE_DEL_DATA1,
        DME_TITLE_WIN_TYPE_DEL_DATA2,
        DME_TITLE_WIN_TYPE_DEL_DATA3,
        DME_TITLE_WIN_TYPE_DEL_DATA4,
        DME_TITLE_WIN_TYPE_DEL_DATA5,
        DME_TITLE_WIN_TYPE_MAX,
        DME_TITLE_WIN_TYPE_NONE,
    }

    internal class DMS_TITLE_MAIN_WORK
    {
        public readonly AMS_FS[] arc_cmn_amb_fs = new AMS_FS[4];
        public readonly AMS_AMB_HEADER[] arc_cmn_amb = new AMS_AMB_HEADER[4];
        public readonly A2S_AMA_HEADER[] cmn_ama = new A2S_AMA_HEADER[4];
        public readonly AMS_AMB_HEADER[] cmn_amb = new AMS_AMB_HEADER[4];
        public readonly AOS_TEXTURE[] cmn_tex = New<AOS_TEXTURE>(4);
        public readonly DMS_BUY_SCR_WORK buy_scr_work = new DMS_BUY_SCR_WORK();
        public readonly AMS_FS[] arc_amb_fs = new AMS_FS[2];
        public readonly AMS_FS[] file_arc_amb_fs = new AMS_FS[2];
        public readonly AMS_FS[] user_arc_amb_fs = new AMS_FS[2];
        public readonly AMS_AMB_HEADER[] arc_amb = new AMS_AMB_HEADER[2];
        public readonly AMS_AMB_HEADER[] file_arc_amb = new AMS_AMB_HEADER[2];
        public readonly AMS_AMB_HEADER[] user_arc_amb = new AMS_AMB_HEADER[2];
        public readonly A2S_AMA_HEADER[] ama = new A2S_AMA_HEADER[2];
        public readonly AMS_AMB_HEADER[] amb = new AMS_AMB_HEADER[2];
        public readonly AOS_TEXTURE[] tex = New<AOS_TEXTURE>(2);
        public readonly AOS_TEXTURE win_tex = new AOS_TEXTURE();
        public readonly AOS_ACTION[] act = new AOS_ACTION[41];
        public readonly float[] decide_menu_frm = new float[5];
        public readonly float[] mmenu_win_size_rate = new float[2];
        public readonly float[] win_size_rate = new float[2];
        public readonly CTrgAoAction[] trg_slct = New<CTrgAoAction>(5);
        public readonly CTrgAoAction[] trg_answer = New<CTrgAoAction>(2);
        public readonly CTrgAoAction trg_return = new CTrgAoAction();
        public readonly CTrgAoAction trg_game = new CTrgAoAction();
        public AMS_FS cmn_win_amb_fs;
        public AMS_FS win_amb_fs;
        public AMS_AMB_HEADER cmn_win_amb;
        public AMS_AMB_HEADER win_amb;
        public _proc_ proc_input;
        public _proc_ proc_update;
        public _proc_ proc_win_input;
        public _proc_ proc_win_update;
        public _proc_ proc_draw;
        public float timer;
        public float disp_timer;
        public float win_timer;
        public float mmenu_win_timer;
        public uint flag;
        public int disp_change_time;
        public uint announce_flag;
        public int win_mode;
        public int cur_slct_menu;
        public int prev_slct_menu;
        public int win_cur_slct;
        public int next_evt;
        public bool is_init_play;
        public bool is_jp_region;
        public bool is_no_save_data;
        public float cur_crsr_pos_y;
        public float src_crsr_pos_y;
        public float dst_crsr_pos_y;
        public int slct_menu_num;
        public uint disp_flag;
        public uint flag_prev;

        public bool iphone_menus;

        public delegate void _proc_(DMS_TITLE_MAIN_WORK work);
    }
}
