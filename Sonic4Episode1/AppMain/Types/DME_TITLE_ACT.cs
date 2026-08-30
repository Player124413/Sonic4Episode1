#define _PC

internal partial class AppMain
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
}

