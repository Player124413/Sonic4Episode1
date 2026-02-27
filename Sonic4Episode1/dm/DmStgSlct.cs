#define _IPHONE
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using accel;
using er;
using gs.backup;

using static AoPad;

using static AppMain;
using static AppMain.GSE_EVT_ID;

using static DmStgSlct.DME_STGSLCT_ACT; // << this will have to change
using static DmStgSlct.DME_STGSLCT_ACT_PAGE;
using static DmStgSlct.DME_STGSLCT_DATA_TYPE;
using static DmStgSlct.DME_STGSLCT_MODE_STATE;
using static DmStgSlct.DME_STGSLCT_NEXT_EVT;
using static DmStgSlct.DME_STGSLCT_PLATFORM_TYPE;
using static DmStgSlct.DME_STGSLCT_PLAY_MODE;
using static DmStgSlct.DME_STGSLCT_STAGE;
using static DmStgSlct.DME_STGSLCT_WIN;
using static DmStgSlct.DME_STGSLCT_ZONE_TYPE;

internal static class DmStgSlct
{
    private static object mtTaskGetTcbWork(MTS_TASK_TCB tcb)
    {
        return tcb.work;
    }

    //// データヘッダ
    //#if !_IPHONE
    //	#include "common/ace/D_STGSLCT.HMA"
    //	#include "common/ace/D_STGSLCT_JP.HMA"
    //#else //!_IPHONE
    //	#include "ace/D_STGSLCT.HMA"
    //	#include "ace/D_STGSLCT_JP.HMA"
    //#endif //!_IPHONE

    //// 共通データヘッダ
    //#if !_IPHONE
    //#include "common/ace/D_CMN_BG.HMA"
    //#include "common/ace/D_CMN_BTN.HMA"
    //#include "common/ace/D_CMN_OBI.HMA"
    //#include "common/ace/D_CMN_WIN.HMA"
    //#include "common/ace/D_CMN_MSG_JP.HMA"
    //#else //!_IPHONE
    //#include "ace/D_CMN_BG.HMA"
    //#include "ace/D_CMN_BTN.HMA"
    //#include "ace/D_CMN_WIN.HMA"
    //#include "ace/D_CMN_MSG_JP.HMA"
    //#endif //!_IPHONE

    private static CActionDraw stgSelect_act_draw_sys = new CActionDraw();

    public const string GSS_BASE_PATH = "";

    public const int DMD_STGSLCT_TASK_PAUSELEVEL = 0;
    public const int DMD_STGSLCT_TASK_PRIO_MAIN = 0x2000;
    public const int DMD_STGSLCT_TASK_GROUP_MAIN = 0;

    public const int DMD_STGSLCT_FILE_PATH_NUM_MAX = 60;

    public const string DMD_STGSLCT_CMN_DATA_FILENAME = GSS_BASE_PATH + "DEMO/CMN/D_CMN_WIN.AMB";
    public const string DMD_STGSLCT_DATA_FILENAME = GSS_BASE_PATH + "DEMO/STGSLCT/D_STGSLCT.AMB";

    public const float DMD_STGSLCT_SIZE_WIDTH = 960.0f;
    public const float DMD_STGSLCT_SIZE_HEIGHT = 720.0f;
    public const float DMD_STGSLCT_SIZE_HALF_WIDTH = 480.0f;
    public const float DMD_STGSLCT_SIZE_HALF_HEIGHT = 360.0f;

    public const int DMD_STGSLCT_DISP_SCORE_DIGIT_NUM = 9;

    // プライオリティ設定
    public const int DMD_STGSLCT_DRAW_PRIO_ZONE = 0x2000;
    public const int DMD_STGSLCT_DRAW_PRIO_STAGE = 0x2000;
    public const int DMD_STGSLCT_DRAW_PRIO_BG = 0x1000;
    public const int DMD_STGSLCT_DRAW_PRIO_FIX = 0x3000;
#if !_IPHONE
    public const int DMD_STGSLCT_DRAW_PRIO_WIN = 0x3000;
#else
    public const int DMD_STGSLCT_DRAW_PRIO_WIN = 0x3500;
#endif
    public const int DMD_STGSLCT_DRAW_PRIO_WIN_FIX = 0x4000;

    // 表示関連
#if _WII
    public const float DMD_STGSLCT_ACT_TABLE_TOP_POS_X = 151.0f;
#elif _IPHONE
    public const float DMD_STGSLCT_ACT_TABLE_TOP_POS_X = 100.0f;
#else
    public const float DMD_STGSLCT_ACT_TABLE_TOP_POS_X = 170.0f;
#endif

#if !_IPHONE
    public const float DMD_STGSLCT_ACT_TABLE_DIST_Y = 96.0f;
#else
    public const float DMD_STGSLCT_ACT_TABLE_DIST_Y = 96.0f + 22.0f;
#endif

    public const int DMD_STGSLCT_INIT_SCORE_NUM = 1000000000;
    public const int DMD_STGSLCT_INIT_RECORD_TIME_NUM = 36000;

    public const int DMD_STGSLCT_CONV_SCORE_RATE = 10;

    public const int DMD_STGSLCT_DOWN_ACT_DISP_POS = 0;
    public const int DMD_STGSLCT_DOWN_ACT_NODISP_POS = 180;
    public const int DMD_STGSLCT_MODE_TEX_FRAME_MAX = 60;

    public const float DMD_STGSLCT_STAGE_TAB_NODISP_POS_X = 1120.0f;

    public const float DMD_STGSLCT_NO_ACTIVE_NUM_ID = 10.0f;
    public const float DMD_STGSLCT_TIME_COLON_ID = 11.0f;

    // ウインドウ関連
#if !_IPHONE
    public const float DMD_STGSLCT_WINDOW_SIZE_W = 380.0f;
    public const float DMD_STGSLCT_WINDOW_SIZE_H = 180.0f;
#else
    public const float DMD_STGSLCT_WINDOW_SIZE_W = 420.0f;
    public const float DMD_STGSLCT_WINDOW_SIZE_H = 180.0f;
#endif
    public const float DMD_STGSLCT_WIN_DEF_RATE = 1.0f;

    // フェード関連
    public const float DMD_STGSLCT_FADEIN_TIME = 32.0f;
    public const float DMD_STGSLCT_FADEOUT_TIME = 32.0f;

    public const int DMD_STGSLCT_BGM_FADEIN_TIME = 32;
    public const int DMD_STGSLCT_BGM_FADEOUT_TIME = 32;

    public const int DMD_STGSLCT_BG_FADE_SPEED = 20;

    // 演出関連
    public const float DMD_STGSLCT_ZONE_EFCT_TIME = 16.0f;
    public const float DMD_STGSLCT_ACT_EFCT_TIME = 16.0f;
    public const float DMD_STGSLCT_WIN_EFCT_TIME = 8.0f;
    public const float DMD_STGSLCT_OBI_MOVE_START_POS = 1120.0f; // (1216.0f)
    public const float DMD_STGSLCT_OBI_MOVE_END_POS = -1120.0f;
    public const float DMD_STGSLCT_OBI_MOVE_SPEED = -3.0f;
    public const float DMD_STGSLCT_ACT_VRTCL_CHNG_DIST = 128.0f;
    public const int DMD_STGSLCT_ACT_VRTCL_CHNG_NUM = 3;
    public const float DMD_STGSLCT_ACT_VRTCL_MOVE_TIME = 12.0f;
    public const float DMD_STGSLCT_CRSR_MOVE_TIME = 8.0f;

    public const int DMD_STGSLCT_ZONE_FINAL_OPEN = (1 << 0);
    public const int DMD_STGSLCT_ZONE_SPECIAL_OPEN = (1 << 1);
    public const int DMD_STGSLCT_ZONE_ALL_OPEN = ((1 << 0) | (1 << 1));

    public const int DMD_STGSLCT_ZONE_DECIDE_EFCT_ON = 6;
    public const int DMD_STGSLCT_ZONE_DECIDE_EFCT_TIME = 10;
    public const int DMD_STGSLCT_ZONE_DECIDE_EFCT_ON_POS_X = 8;
    public const int DMD_STGSLCT_ZONE_DECIDE_EFCT_ON_POS_Y = 8;

    public const int DMD_STGSLCT_ZONE_SCR_ID_NUM = 3;
    public const int DMD_STGSLCT_ZONE_SCR_CHANGE_ALL_FRM = 360;
    public const int DMD_STGSLCT_ZONE_CHNG_BTN_END_FRM = 12;

    // フラグ関連
    public const int DMD_STGSLCT_FLAG_EXIT = (1 << 0);
    public const int DMD_STGSLCT_FLAG_CANCEL = (1 << 1);
    public const int DMD_STGSLCT_FLAG_DECIDE = (1 << 2);
    public const int DMD_STGSLCT_FLAG_DISP_MENU = (1 << 3);
    public const int DMD_STGSLCT_FLAG_WIN_EFCT_END = (1 << 4);
    public const int DMD_STGSLCT_FLAG_ACT_CHNG_ZONE = (1 << 5);
    public const int DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL = (1 << 6);
    public const int DMD_STGSLCT_FLAG_ACT_CHNG_CRSR = (1 << 7);
    public const int DMD_STGSLCT_FLAG_ACT_RE_CHNG_ZONE = (1 << 8);
    public const int DMD_STGSLCT_FLAG_ACT_RE_CHNG_VRTCL = (1 << 9);
    public const int DMD_STGSLCT_FLAG_ACT_RE_CHNG_CRSR = (1 << 10);
    public const int DMD_STGSLCT_FLAG_UP_CHNG_CRSR = (1 << 11);
    public const int DMD_STGSLCT_FLAG_DOWN_CHNG_CRSR = (1 << 12);
    public const int DMD_STGSLCT_FLAG_EME_TBL_MOVE_NODISP = (1 << 13);
    public const int DMD_STGSLCT_FLAG_EME_TBL_MOVE_DISP = (1 << 14);
    public const int DMD_STGSLCT_FLAG_MODE_TEX_MOVE_NODISP = (1 << 15);
    public const int DMD_STGSLCT_FLAG_MODE_TEX_MOVE_DISP = (1 << 16);
    public const int DMD_STGSLCT_FLAG_WHITE_FLASH_EFCT = (1 << 17);
    public const int DMD_STGSLCT_FLAG_DECIDE_ZONE_TABLE_EFCT = (1 << 18);
    public const int DMD_STGSLCT_FLAG_CHANGE_EVT_RANKING = (1 << 19);
    public const int DMD_STGSLCT_FLAG_CHANGE_DISP_BG = (1 << 20);
    public const int DMD_STGSLCT_FLAG_BG_FADE_EFCT = (1 << 21);
    public const int DMD_STGSLCT_FLAG_ACT_PUSH_L_BTN = (1 << 22);
    public const int DMD_STGSLCT_FLAG_ACT_PUSH_R_BTN = (1 << 23);
    public const int DMD_STGSLCT_FLAG_PREV_EVT_RANKING = (1 << 24);
    public const int DMD_STGSLCT_FLAG_DEMO_SND_END = (1 << 25);
    public const int DMD_STGSLCT_FLAG_CHECK_SIGN_OUT_OK = (1 << 30);
    public const int DMD_STGSLCT_FLAG_SIGN_OUT_EXIT = (1 << 31);

    // アクション表示フラグ関連
    public const int DMD_STGSLCT_DISP_FLAG_WIN_ACT = (1 << 0);
    public const int DMD_STGSLCT_DISP_FLAG_LR_ARROW = (1 << 1);
    public const int DMD_STGSLCT_DISP_FLAG_ACT_CRSR = (1 << 2);

    public const int IDA_D_STGSLCT_ACT_TAB_NORMAL_L = 0;
    public const int IDA_D_STGSLCT_ACT_ICON_SONIC = 1;
    public const int IDA_D_STGSLCT_ACT_NUM_A_100 = 2;
    public const int IDA_D_STGSLCT_ACT_NUM_A_10 = 3;
    public const int IDA_D_STGSLCT_ACT_NUM_A_1 = 4;
    public const int IDA_D_STGSLCT_ACT_ICON1 = 5;
    public const int IDA_D_STGSLCT_ACT_ICON2 = 6;
    public const int IDA_D_STGSLCT_ACT_ICON3 = 7;
    public const int IDA_D_STGSLCT_ACT_ICON4 = 8;
    public const int IDA_D_STGSLCT_ACT_ICON5 = 9;
    public const int IDA_D_STGSLCT_ACT_ICON6 = 10;
    public const int IDA_D_STGSLCT_ACT_EMER_TAB1 = 11;
    public const int IDA_D_STGSLCT_ACT_ICON_EMER1 = 12;
    public const int IDA_D_STGSLCT_ACT_ICON_EMER2 = 13;
    public const int IDA_D_STGSLCT_ACT_ICON_EMER3 = 14;
    public const int IDA_D_STGSLCT_ACT_ICON_EMER4 = 15;
    public const int IDA_D_STGSLCT_ACT_ICON_EMER5 = 16;
    public const int IDA_D_STGSLCT_ACT_ICON_EMER6 = 17;
    public const int IDA_D_STGSLCT_ACT_ICON_EMER7 = 18;
    public const int IDA_D_STGSLCT_ACT_ARROW1 = 19;
    public const int IDA_D_STGSLCT_ACT_ARROW2 = 20;
    public const int IDA_D_STGSLCT_ACT_ACT1_TAB_A = 21;
    public const int IDA_D_STGSLCT_ACT_ACT1_TAB_B = 22;
    public const int IDA_D_STGSLCT_ACT_ACT1_TAB_C = 23;
    public const int IDA_D_STGSLCT_ACT_ACT1_SCR = 24;
    public const int IDA_D_STGSLCT_ACT_ACT1_SCR_BG = 25;
    public const int IDA_D_STGSLCT_ACT_ACT1_NUM_B = 26;
    public const int IDA_D_STGSLCT_ACT_ACT1_LINE = 27;
    public const int IDA_D_STGSLCT_ACT_ACT1_ICON_EMER = 28;
    public const int IDA_D_STGSLCT_ACT_ACT_CURSOR_UP = 29;
    public const int IDA_D_STGSLCT_ACT_ACT_CURSOR_DOWN = 30;
    public const int IDA_D_STGSLCT_ACT_ACT1_NUM_A1 = 31;
    public const int IDA_D_STGSLCT_ACT_ACT1_NUM_A2 = 32;
    public const int IDA_D_STGSLCT_ACT_ACT1_NUM_A3 = 33;
    public const int IDA_D_STGSLCT_ACT_ACT1_NUM_A4 = 34;
    public const int IDA_D_STGSLCT_ACT_ACT1_NUM_A5 = 35;
    public const int IDA_D_STGSLCT_ACT_ACT1_NUM_A6 = 36;
    public const int IDA_D_STGSLCT_ACT_ACT1_NUM_A7 = 37;
    public const int IDA_D_STGSLCT_ACT_ACT1_NUM_A8 = 38;
    public const int IDA_D_STGSLCT_ACT_ACT1_NUM_A9 = 39;
    public const int IDA_D_STGSLCT_ACT_ACT1_COVER1 = 40;
    public const int IDA_D_STGSLCT_ACT_ACT1_COVER2 = 41;
    public const int IDA_D_STGSLCT_ACT_ACT1_COVER3 = 42;
    public const int IDA_D_STGSLCT_ACT_SPE_NUM_STAGE = 43;
    public const int IDA_D_STGSLCT_ACT_SPE_EMER = 44;
    public const int IDA_D_STGSLCT_ACT_ZONE_SCR1 = 45;
    public const int IDA_D_STGSLCT_ACT_ZONE_SCR2 = 46;
    public const int IDA_D_STGSLCT_ACT_ZONE_SCR3 = 47;
    public const int IDA_D_STGSLCT_ACT_ZONE_SCR4 = 48;
    public const int IDA_D_STGSLCT_ACT_ZONE_SCR5 = 49;
    public const int IDA_D_STGSLCT_ACT_ZONE_SCR6 = 50;
    public const int IDA_D_STGSLCT_ACT_ZONE_TAB = 51;
    public const int IDA_D_STGSLCT_ACT_ZONE_COVER1 = 52;
    public const int IDA_D_STGSLCT_ACT_ZONE_COVER2 = 53;
    public const int IDA_D_STGSLCT_ACT_ZONE_COVER3 = 54;
    public const int IDA_D_STGSLCT_ACT_TAB_TIME_L = 55;
    public const int IDA_D_STGSLCT_ACT_TAB_TIME_C = 56;
    public const int IDA_D_STGSLCT_ACT_TAB_TIME_R = 57;
    public const int IDA_D_STGSLCT_ACT_Z_BG_LT = 58;
    public const int IDA_D_STGSLCT_ACT_Z_BG_LB = 59;
    public const int IDA_D_STGSLCT_ACT_Z_BG_RT = 60;
    public const int IDA_D_STGSLCT_ACT_Z_BG_RB = 61;
    public const int IDA_D_STGSLCT_ACT_ZONE_SCR1_A = 62;
    public const int IDA_D_STGSLCT_ACT_ZONE_SCR1_B = 63;
    public const int IDA_D_STGSLCT_ACT_ZONE_SCR1_C = 64;
    public const int IDA_D_STGSLCT_ACT_ZONE_SCR2_A = 65;
    public const int IDA_D_STGSLCT_ACT_ZONE_SCR2_B = 66;
    public const int IDA_D_STGSLCT_ACT_ZONE_SCR2_C = 67;
    public const int IDA_D_STGSLCT_ACT_ZONE_SCR3_A = 68;
    public const int IDA_D_STGSLCT_ACT_ZONE_SCR3_B = 69;
    public const int IDA_D_STGSLCT_ACT_ZONE_SCR3_C = 70;
    public const int IDA_D_STGSLCT_ACT_ZONE_SCR4_A = 71;
    public const int IDA_D_STGSLCT_ACT_ZONE_SCR4_B = 72;
    public const int IDA_D_STGSLCT_ACT_ZONE_SCR4_C = 73;
    public const int IDA_D_STGSLCT_ACT_TAB_TIME_L2 = 74;
    public const int IDA_D_STGSLCT_ACT_TAB_TIME_C2 = 75;
    public const int IDA_D_STGSLCT_ACT_TAB_TIME_R2 = 76;
    public const int IDA_D_STGSLCT_ACT_TEX_ZNAME = 77;
    public const int IDA_D_STGSLCT_ACT_ZONE_TEXT = 78;
    public const int IDA_D_STGSLCT_ACT_TAB_TIME_MOVE = 79;
    public const int IDA_D_STGSLCT_JP_ACT_TEX_ZNAME = 0;
    public const int IDA_D_STGSLCT_JP_ACT_ACT1_TEX_ACT = 1;
    public const int IDA_D_STGSLCT_JP_ACT_ACT1_MESS = 2;
    public const int IDA_D_STGSLCT_JP_ACT_ACT1_TEX_SCORE = 3;
    public const int IDA_D_STGSLCT_JP_ACT_ACT1_TEX_TIME = 4;
    public const int IDA_D_STGSLCT_JP_ACT_BOSS_TEX_BOSS = 5;
    public const int IDA_D_STGSLCT_JP_ACT_SPE_TEX_STAGE = 6;
    public const int IDA_D_STGSLCT_JP_ACT_ZONE_TEXT = 7;
    public const int IDA_D_STGSLCT_JP_ACT_TEX_WIN_MSG = 8;
    public const int IDA_D_STGSLCT_JP_ACT_TEX_WIN_SSONIC = 9;
    public const int IDA_D_STGSLCT_JP_ACT_TEX_SCTM01A = 10;
    public const int IDA_D_STGSLCT_JP_ACT_TEX_SCTM02A = 11;
    public const int IDA_D_STGSLCT_JP_ACT_TEX_WIN_MSG2 = 12;

#if _WII
    public const float DMD_STGSLCT_DISP_SCALE_TEXT = 1.5f;
#elif _IPHONE
    public const float DMD_STGSLCT_DISP_SCALE_TEXT = 1.5f * 1.125f;
#endif

#if _PS3 || _XBOX || _PC
    public const float DMD_STGSLCT_OBI_MSG_SCALE_SIZE = 1.2f;
#endif

    public enum DME_STGSLCT_NEXT_EVT
    {
        DME_STGSLCT_NEXT_EVT_MAINGAME = 0,  //!< 通常ACT(BOSS含む)
        DME_STGSLCT_NEXT_EVT_SPESTE,        //!< スペステ
        DME_STGSLCT_NEXT_EVT_RANKING,       //!< ランキング
        DME_STGSLCT_NEXT_EVT_MAINMENU,      //!< メインメニュー
        DME_STGSLCT_NEXT_EVT_TITLE,         //!< タイトル

        DME_STGSLCT_NEXT_EVT_MAX
    }


    public enum DME_STGSLCT_DATA_TYPE
    {
        DME_STGSLCT_DATA_TYPE_CMN_DATA = 0,     //!< 共通データ
        DME_STGSLCT_DATA_TYPE_LANG_DATA,        //!< 言語別データ

        DME_STGSLCT_DATA_TYPE_MAX,
        DME_STGSLCT_DATA_TYPE_NONE
    }

    //! プラットフォームタイプ
    public enum DME_STGSLCT_PLATFORM_TYPE
    {
        DME_STGSLCT_PLATFORM_TYPE_PC = 0,       //!< PC
        DME_STGSLCT_PLATFORM_TYPE_XBOX,         //!< XBOX
        DME_STGSLCT_PLATFORM_TYPE_PS3,          //!< PS3
        DME_STGSLCT_PLATFORM_TYPE_WII,          //!< Wii
        DME_STGSLCT_PLATFORM_TYPE_IPHONE,       //!< iphone

        DME_STGSLCT_PLATFORM_TYPE_MAX,
        DME_STGSLCT_PLATFORM_TYPE_NONE
    }


    //! ZONEタイプ
    public enum DME_STGSLCT_ZONE_TYPE
    {
        DME_STGSLCT_ZONE_TYPE_1 = 0,        //!< 
        DME_STGSLCT_ZONE_TYPE_2,            //!< 
        DME_STGSLCT_ZONE_TYPE_3,            //!< 
        DME_STGSLCT_ZONE_TYPE_4,            //!< 
        DME_STGSLCT_ZONE_TYPE_FINAL,        //!< 
        DME_STGSLCT_ZONE_TYPE_SPE,          //!< 

        DME_STGSLCT_ZONE_TYPE_NUM,
        DME_STGSLCT_ZONE_TYPE_NONE
    }


    //! 現在のモードSTATE
    public enum DME_STGSLCT_MODE_STATE
    {
        DME_STGSLCT_MODE_STATE_ZONE_SLCT = 0,   //!< ゾーン選択
        DME_STGSLCT_MODE_STATE_ACT_SLCT,        //!< ACT選択

        DME_STGSLCT_MODE_STATE_NUM,
        DME_STGSLCT_MODE_STATE_NONE
    }


    //! 現在のPLAYモード
    public enum DME_STGSLCT_PLAY_MODE
    {
        DME_STGSLCT_PLAY_MODE_NORMAL = 0,       //!< ノーマルモード
        DME_STGSLCT_PLAY_MODE_TIME_ATK,         //!< タイムアタック

        DME_STGSLCT_PLAY_MODE_NUM,
        DME_STGSLCT_PLAY_MODE_NONE
    }


    //! ウインドウ表示パターンタイプ
    public enum DME_STGSLCT_WIN
    {
        DME_STGSLCT_WIN_MENU = 0,           //!< 
        DME_STGSLCT_WIN_STG_SLCT,           //!< 
        DME_STGSLCT_WIN_1_1_CLEAR,          //!< 
        DME_STGSLCT_WIN_BOSS1_CAN_PLAY,     //!< 
        DME_STGSLCT_WIN_BOSS2_CAN_PLAY,     //!< 
        DME_STGSLCT_WIN_BOSS3_CAN_PLAY,     //!< 
        DME_STGSLCT_WIN_BOSS4_CAN_PLAY,     //!< 
        DME_STGSLCT_WIN_FINAL_CAN_SLCT,     //!< 
        DME_STGSLCT_WIN_SSONIC_SLCT,        //!< 
        DME_STGSLCT_WIN_SPESTE_CAN_SLCT,    //!< 

        DME_STGSLCT_WIN_NUM,
        DME_STGSLCT_WIN_NONE
    }



    //! ZONEタイプ
    public enum DME_STGSLCT_ACT_PAGE
    {
        DME_STGSLCT_ACT_PAGE_1 = 0,     //!< 
        DME_STGSLCT_ACT_PAGE_2,         //!< 
        DME_STGSLCT_ACT_PAGE_3,         //!< 
        DME_STGSLCT_ACT_PAGE_4,         //!< 
        DME_STGSLCT_ACT_PAGE_FINAL,     //!< 
        DME_STGSLCT_ACT_PAGE_SPE,       //!< 

        DME_STGSLCT_ACT_PAGE_NUM,
        DME_STGSLCT_ACT_PAGE_NONE
    }


    //! 現在のモードSTATE		仮で用意	gsMainSysに用意され次第、そちらに移行
    public enum DME_STGSLCT_STAGE
    {
        DME_STGSLCT_STAGE_1_1 = 0,      //!< 
        DME_STGSLCT_STAGE_1_2,          //!< 
        DME_STGSLCT_STAGE_1_3,          //!< 
        DME_STGSLCT_STAGE_1_B,          //!< 
        DME_STGSLCT_STAGE_2_1,          //!< 
        DME_STGSLCT_STAGE_2_2,          //!< 
        DME_STGSLCT_STAGE_2_3,          //!< 
        DME_STGSLCT_STAGE_2_B,          //!< 
        DME_STGSLCT_STAGE_3_1,          //!< 
        DME_STGSLCT_STAGE_3_2,          //!< 
        DME_STGSLCT_STAGE_3_3,          //!< 
        DME_STGSLCT_STAGE_3_B,          //!< 
        DME_STGSLCT_STAGE_4_1,          //!< 
        DME_STGSLCT_STAGE_4_2,          //!< 
        DME_STGSLCT_STAGE_4_3,          //!< 
        DME_STGSLCT_STAGE_4_B,          //!< 
        DME_STGSLCT_STAGE_F_1,          //!< 
        DME_STGSLCT_STAGE_S_1,          //!< 
        DME_STGSLCT_STAGE_S_2,          //!< 
        DME_STGSLCT_STAGE_S_3,          //!< 
        DME_STGSLCT_STAGE_S_4,          //!< 
        DME_STGSLCT_STAGE_S_5,          //!< 
        DME_STGSLCT_STAGE_S_6,          //!< 
        DME_STGSLCT_STAGE_S_7,          //!< 

        DME_STGSLCT_STAGE_NUM,
        DME_STGSLCT_STAGE_NONE
    }



    //! アクションテーブル
    public enum DME_STGSLCT_ACT
    {
        // モード共通・言語共通
        ACT_ZONE_BG_LT = 0, //!< 
        ACT_ZONE_BG_LB,     //!< 
        ACT_ZONE_BG_RT,     //!< 
        ACT_ZONE_BG_RB,     //!< 
        ACT_TAB_MODE_L,     //!< 
#if !_IPHONE
        ACT_TAB_MODE_R,     //!<
#endif //!_IPHONE
        ACT_ICON_SONIC,     //!< 
#if !_IPHONE
        ACT_TEX_SONIC,      //!<
#endif //!_IPHONE
        ACT_REST_NUM_100,   //!< 
        ACT_REST_NUM_10,    //!< 
        ACT_REST_NUM_1,     //!< 
        ACT_TAB_EMER,       //!< 
        ACT_ICON_EMER_1,    //!< 
        ACT_ICON_EMER_2,    //!< 
        ACT_ICON_EMER_3,    //!< 
        ACT_ICON_EMER_4,    //!< 
        ACT_ICON_EMER_5,    //!< 
        ACT_ICON_EMER_6,    //!< 
        ACT_ICON_EMER_7,    //!< 
        ACT_TEX_ZONE_UP,    //!< 

        // モード共通・言語別
#if !_IPHONE
        ACT_TEX_MODE_TIME,  //!<
        ACT_TEX_MODE_SCORE, //!<
        ACT_TEX_MENU,       //!<
        ACT_TEX_EMER,       //!<
#endif //!_IPHONE
        ACT_TEX_ZONE_UP_S,  //!< 
#if !_IPHONE
        ACT_TEX_OBI1,       //!<
        ACT_TEX_OBI2,       //!<
#endif //!_IPHONE

        // ZONE選択・言語共通
        ACT_TAB_ZONE_SCR1,      //!< 
        ACT_TAB_ZONE_SCR2,      //!< 
        ACT_TAB_ZONE_SCR3,      //!< 
        ACT_TAB_ZONE_SCR4,      //!< 
        ACT_TAB_ZONE_SCR5,      //!< 
        ACT_TAB_ZONE_SCR6,      //!< 
        ACT_TAB_ZONE_SCR1_1a,   //!< 
        ACT_TAB_ZONE_SCR1_2a,   //!< 
        ACT_TAB_ZONE_SCR1_3a,   //!< 
        ACT_TAB_ZONE_SCR2_1a,   //!< 
        ACT_TAB_ZONE_SCR2_2a,   //!< 
        ACT_TAB_ZONE_SCR2_3a,   //!< 
        ACT_TAB_ZONE_SCR3_1a,   //!< 
        ACT_TAB_ZONE_SCR3_2a,   //!< 
        ACT_TAB_ZONE_SCR3_3a,   //!< 
        ACT_TAB_ZONE_SCR4_1a,   //!< 
        ACT_TAB_ZONE_SCR4_2a,   //!< 
        ACT_TAB_ZONE_SCR4_3a,   //!< 
        ACT_TAB_ZONE_TAB,       //!< 
        ACT_TAB_ZONE_TEXT,      //!< 
        ACT_TAB_ZONE_TEXT_S,    //!< 
        ACT_TAB_ZONE_COVER1,    //!< 
        ACT_TAB_ZONE_COVER2,    //!< 
        ACT_TAB_ZONE_COVER3,    //!<
#if !_IPHONE
        ACT_TAB_ZONE_CURSOR2,   //!< 
        ACT_TAB_ZONE_CURSOR1,   //!< 
        ACT_TAB_ZONE_CURSOR3,   //!< 
#endif //!_IPHONE

        // STAGE選択・言語共通
        ACT_ICON_DOWN_1,        //!< 
        ACT_ICON_DOWN_2,        //!< 
        ACT_ICON_DOWN_3,        //!< 
        ACT_ICON_DOWN_4,        //!< 
        ACT_ICON_DOWN_5,        //!< 
        ACT_ICON_DOWN_6,        //!< 
        ACT_ICON_L_ARROW,       //!< 
        ACT_ICON_R_ARROW,       //!< 
        ACT_TAB_STATE_L,        //!< 
        ACT_TAB_STATE_C,        //!< 
        ACT_TAB_STATE_R,        //!< 
        ACT_TAB_STATE_L2,       //!< 
        ACT_TAB_STATE_C2,       //!< 
        ACT_TAB_STATE_R2,       //!< 
        ACT_TAB_STATE_MOVE,     //!< 
        ACT_TAB_TABLE2,         //!< 
        ACT_TAB_TABLE1,         //!< 
        ACT_TAB_TABLE3,         //!< 
        ACT_TAB_SCR,            //!< 
        ACT_TAB_SCR_BG,         //!< 
        ACT_TAB_TEXT,           //!< 
        ACT_TAB_A_NUM,          //!< 
        ACT_TAB_MESS,           //!< 
        ACT_TAB_LINE,           //!< 
        ACT_TAB_ICON_EMER,      //!< 
        ACT_TAB_NUM_SPE_STAGE,  //!< 
        ACT_TAB_ICON_SPE_EMER,  //!< 
        ACT_TAB_CURSOR_UP,      //!< 
        ACT_TAB_CURSOR_DOWN,    //!< 
#if !_IPHONE
        ACT_TAB_CURSOR_2,       //!<
        ACT_TAB_CURSOR_1,       //!<
        ACT_TAB_CURSOR_3,       //!<
#endif //!_IPHONE
        ACT_TAB_TEX_SCORE,      //!< 
        ACT_TAB_TEX_TIME,       //!< 
        ACT_TAB_TEX_BOSS,       //!< 
        ACT_TAB_TEX_SPE_STAGE,  //!< 
        ACT_TAB_S_NUM1,         //!< 
        ACT_TAB_S_NUM2,         //!< 
        ACT_TAB_S_NUM3,         //!< 
        ACT_TAB_S_NUM4,         //!< 
        ACT_TAB_S_NUM5,         //!< 
        ACT_TAB_S_NUM6,         //!< 
        ACT_TAB_S_NUM7,         //!< 
        ACT_TAB_S_NUM8,         //!< 
        ACT_TAB_S_NUM9,         //!< 
        ACT_TAB_COVER2,         //!< 
        ACT_TAB_COVER1,         //!< 
        ACT_TAB_COVER3,         //!< 

        ACT_TEX_BIG_TIME,       //!< 
        ACT_TEX_BIG_SCORE,      //!< 
#if !_IPHONE
        ACT_TEX_TIME_EFCT,      //!<
        ACT_TEX_SCORE_EFCT,     //!<
#endif //!_IPHONE

        // ウインドウ関連・言語共通
        ACT_WIN_TEX_MSG,        //!< 
        ACT_WIN_TEX_MSG2,       //!< 
        ACT_WIN_TEX_MSG_SSONIC, //!< 

        // メニュー共通データ
        ACT_WAVE_BG,            //!< 
        ACT_DOWN_BG,            //!< 
        ACT_BLUE_BG,            //!<

        ACT_BTN_CANCEL1,        //!< 
        ACT_BTN_LB,             //!< 
        ACT_BTN_MENU,           //!< 
        ACT_BTN_LB_ARROW,       //!< 
        ACT_BTN_RB_ARROW,       //!< 
        ACT_BTN_CANCEL2,        //!< 
        ACT_BTN_X,              //!< 
        ACT_BTN_Y,              //!< 

#if !_IPHONE
        ACT_OBI_C,              //!< 
        ACT_OBI_L,              //!< 
        ACT_OBI_R,              //!< 

        ACT_WIN_LINE,           //!< 
#else //!_IPHONE
        ACT_BACK_BTN01_L,
        ACT_BACK_BTN01_R,

        ACT_YES_BTN_L,
        ACT_YES_BTN_C,
        ACT_YES_BTN_R,
        ACT_NO_BTN_L,
        ACT_NO_BTN_C,
        ACT_NO_BTN_R,
#endif //!_IPHONE

        ACT_TEX_FIX_BACK,       //!< 
#if !_IPHONE
        ACT_TEX_WINTITLE,       //!< 
#endif //!_IPHONE
        ACT_TEX_BACK1,          //!< 
#if !_IPHONE
        ACT_TEX_OK,             //!< 
#endif //!_IPHONE
        ACT_TEX_YES,            //!< 
        ACT_TEX_NO,             //!< 



        ACT_NUM,

        // ACTテーブルのアクション番号開始・終わり
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
        public readonly AOS_ACTION[] act = new AOS_ACTION[116];
        public readonly CTrgAoAction[] trg_zone = New<CTrgAoAction>(6);
        public readonly CTrgAoAction[] trg_act = New<CTrgAoAction>(7);
        public readonly CTrgAoAction[] trg_act_tab = New<CTrgAoAction>(6);
        public readonly CTrgAoAction[] trg_act_lr = New<CTrgAoAction>(2);
        public readonly CTrgFlick trg_act_move = new CTrgFlick();
        public readonly CTrgAoAction[] trg_mode = New<CTrgAoAction>(2);
        public readonly CTrgAoAction trg_cancel = new CTrgAoAction();
        public readonly CTrgAoAction[] trg_answer = New<CTrgAoAction>(2);
        public readonly int[] n_sonic_hi_score = new int[17];
        public readonly int[] s_sonic_hi_score = new int[17];
        public readonly int[] n_sonic_record_time = new int[17];
        public readonly int[] s_sonic_record_time = new int[17];
        public readonly int[] hi_score = new int[24];
        public readonly int[] record_time = new int[24];
        public readonly int[] is_clear_stage = new int[24];
        public readonly uint[] eme_stage_no = new uint[7];
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
        public readonly float[] act_tab_state_move_base_pos = new float[2];
        public readonly float[] obi_pos = new float[2];
        public readonly AMS_PARAM_DRAW_PRIMITIVE up_bg_vrtx = new AMS_PARAM_DRAW_PRIMITIVE();
        public readonly float[] tex_u = new float[2];
        public readonly float[] tex_v = new float[2];
        public AMS_FS win_amb_fs;
        public AMS_AMB_HEADER win_amb;
        public _proc_win_input_ proc_win_input;
        public _proc_input_ proc_input;
        public _proc_win_update_ proc_win_update;
        public _proc_menu_update_ proc_menu_update;
        public _proc_draw_ proc_draw;
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
        public bool win_is_disp_cover;
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
        public bool is_disp_cover;
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

        public delegate void _proc_win_input_(DMS_STGSLCT_MAIN_WORK work);

        public delegate void _proc_input_(DMS_STGSLCT_MAIN_WORK work);

        public delegate void _proc_win_update_(DMS_STGSLCT_MAIN_WORK work);

        public delegate void _proc_menu_update_(DMS_STGSLCT_MAIN_WORK work);

        public delegate void _proc_draw_(DMS_STGSLCT_MAIN_WORK work);
    }


    // ----- Static Variables --------------------（スタティック変数の定義：局所）
    // 各国別AMBファイルパステーブル
    private static readonly string[] dm_stgslct_main_lng_amb_name_tbl = [
        GSS_BASE_PATH + "DEMO/STGSLCT/D_STGSLCT_JP.AMB",
        GSS_BASE_PATH + "DEMO/STGSLCT/D_STGSLCT_US.AMB",
        GSS_BASE_PATH + "DEMO/STGSLCT/D_STGSLCT_FR.AMB",
        GSS_BASE_PATH + "DEMO/STGSLCT/D_STGSLCT_IT.AMB",
        GSS_BASE_PATH + "DEMO/STGSLCT/D_STGSLCT_GE.AMB",
        GSS_BASE_PATH + "DEMO/STGSLCT/D_STGSLCT_SP.AMB",
    ];


    // メニュー共通AMBファイルパステーブル
    private static readonly string[] dm_stgslct_menu_cmn_amb_name_tbl = [
        GSS_BASE_PATH + "DEMO/CMN/D_CMN_BG.AMB",
        GSS_BASE_PATH + "DEMO/CMN/D_CMN_BTN.AMB",
        GSS_BASE_PATH + "DEMO/CMN/D_CMN_OBI.AMB",
        GSS_BASE_PATH + "DEMO/CMN/D_CMN_WIN.AMB",
    ];

    // 各国別メニュー共通AMBファイルパステーブル
    private static readonly string[] dm_stgslct_menu_cmn_lng_amb_name_tbl = [
        GSS_BASE_PATH + "DEMO/CMN/D_CMN_MSG_JP.AMB",
        GSS_BASE_PATH + "DEMO/CMN/D_CMN_MSG_US.AMB",
        GSS_BASE_PATH + "DEMO/CMN/D_CMN_MSG_FR.AMB",
        GSS_BASE_PATH + "DEMO/CMN/D_CMN_MSG_IT.AMB",
        GSS_BASE_PATH + "DEMO/CMN/D_CMN_MSG_GE.AMB",
        GSS_BASE_PATH + "DEMO/CMN/D_CMN_MSG_SP.AMB",
    ];


    // ZONEごとのACT数テーブル
    private static readonly uint[][] dm_stgslct_zone_act_num_tbl = [
        [0,  4],
        [4,  4],
        [8,  4],
        [12, 4],
        [16, 1],
        [17, 7],
    ];


    // ZONEごとのACT縦切り替えテーブル
    private static readonly uint[] dm_stgslct_act_chng_vrtcl_num_tbl = [
        0,
        0,
        0,
        0,
        0,
        4,
    ];


    // STAGEごとのZONE番号テーブル
    private static readonly uint[] dm_stgslct_act_zone_no_tbl = [
        0, 0, 0, 0,
        1, 1, 1, 1,
        2, 2, 2, 2,
        3, 3, 3, 3,
        4,
        5, 5, 5, 5, 5, 5, 5,
    ];


    // エメラルドGETステージテーブル
    private static readonly uint[] dm_stgslct_eme_get_act_no_tbl = [
         0,
         0,  1,  2,
         4,  5,  6,
         8,  9, 10,
        12, 13, 14,
    ];


    // GSにあるステージID配列への変換テーブル
    private static readonly ushort[][] dm_stgslct_conv_stage_no_tbl = [
        [ 0,  1,  2,  3, 0xffff, 0xffff, 0xffff],
        [ 4,  5,  6,  7, 0xffff, 0xffff, 0xffff],
        [ 8,  9, 10, 11, 0xffff, 0xffff, 0xffff],
        [12, 13, 14, 15, 0xffff, 0xffff, 0xffff],
        [16, 16, 16, 16,     16, 0xffff, 0xffff],
        [21, 22, 23, 24,     25,     26,     27],
    ];


    // 各ゾーンのACT配列テーブル
    private static readonly uint[] dm_stgslct_zone_array_act_tbl = [
        0, 1, 2, 3,						// ZONE1
	    0, 1, 2, 3,						// ZONE2
	    0, 1, 2, 3,						// ZONE3
	    0, 1, 2, 3,						// ZONE4
	    0,								// FINAL STAGE
	    0, 1, 2, 3, 4, 5, 6,			// SPECIAL STAGE
    ];


    // ZONE選択での十字キー入力遷移先テーブル(4つVer)
    private static readonly DME_STGSLCT_ZONE_TYPE[][] dm_stgslct_n_zone_input_dir_tbl = [
	// 			↑							↓
	// 				←							→
	    [DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_2
            , DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_3],		// ZONE1
	    [DME_STGSLCT_ZONE_TYPE_1, DME_STGSLCT_ZONE_TYPE_NONE
            , DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_4],		// ZONE2
	    [DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_4
            , DME_STGSLCT_ZONE_TYPE_1, DME_STGSLCT_ZONE_TYPE_NONE],		// ZONE3
	    [DME_STGSLCT_ZONE_TYPE_3, DME_STGSLCT_ZONE_TYPE_NONE
            , DME_STGSLCT_ZONE_TYPE_2, DME_STGSLCT_ZONE_TYPE_NONE],		// ZONE4
	    [DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_NONE
            , DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_NONE],	// FINAL
	    [DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_NONE
            , DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_NONE],	// SPE
    ];


    // ZONE選択での十字キー入力遷移先テーブル(5つVer)
    private static readonly DME_STGSLCT_ZONE_TYPE[][] dm_stgslct_s_zone_input_dir_tbl = [
        // 			↑							↓
        // 				←							→
        [DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_2
        , DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_SPE],	// ZONE1
	    [DME_STGSLCT_ZONE_TYPE_1, DME_STGSLCT_ZONE_TYPE_NONE
            , DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_SPE],	// ZONE2
	    [DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_4
            , DME_STGSLCT_ZONE_TYPE_SPE, DME_STGSLCT_ZONE_TYPE_NONE],	// ZONE3
	    [DME_STGSLCT_ZONE_TYPE_3, DME_STGSLCT_ZONE_TYPE_NONE
            , DME_STGSLCT_ZONE_TYPE_SPE, DME_STGSLCT_ZONE_TYPE_NONE],	// ZONE4
	    [DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_NONE
            , DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_NONE],	// FINAL
	    [DME_STGSLCT_ZONE_TYPE_1, DME_STGSLCT_ZONE_TYPE_2
            , DME_STGSLCT_ZONE_TYPE_1, DME_STGSLCT_ZONE_TYPE_3],		// SPE
    ];


    // ZONE選択での十字キー入力遷移先テーブル(5つVer)
    private static readonly DME_STGSLCT_ZONE_TYPE[][] dm_stgslct_f_zone_input_dir_tbl = [
        // 			↑							↓
        // 				←							→
        [DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_2
        , DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_FINAL],	// ZONE1
	    [DME_STGSLCT_ZONE_TYPE_1, DME_STGSLCT_ZONE_TYPE_NONE
            , DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_FINAL],	// ZONE2
	    [DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_4
            , DME_STGSLCT_ZONE_TYPE_FINAL, DME_STGSLCT_ZONE_TYPE_NONE],	// ZONE3
	    [DME_STGSLCT_ZONE_TYPE_3, DME_STGSLCT_ZONE_TYPE_NONE
            , DME_STGSLCT_ZONE_TYPE_FINAL, DME_STGSLCT_ZONE_TYPE_NONE],	// ZONE4
	    [DME_STGSLCT_ZONE_TYPE_1, DME_STGSLCT_ZONE_TYPE_2
            , DME_STGSLCT_ZONE_TYPE_1, DME_STGSLCT_ZONE_TYPE_3],		// FINAL
	    [DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_NONE
            , DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_NONE],	// SPE
    ];


    // ZONE選択での十字キー入力遷移先テーブル(6つVer)
    private static readonly DME_STGSLCT_ZONE_TYPE[][] dm_stgslct_a_zone_input_dir_tbl = [
        // 			↑							↓
        // 				←							→
        [DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_2
        , DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_FINAL],	// ZONE1
	    [DME_STGSLCT_ZONE_TYPE_1, DME_STGSLCT_ZONE_TYPE_NONE
            , DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_SPE],	// ZONE2
	    [DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_4
            , DME_STGSLCT_ZONE_TYPE_FINAL, DME_STGSLCT_ZONE_TYPE_NONE],	// ZONE3
	    [DME_STGSLCT_ZONE_TYPE_3, DME_STGSLCT_ZONE_TYPE_NONE
            , DME_STGSLCT_ZONE_TYPE_SPE, DME_STGSLCT_ZONE_TYPE_NONE],	// ZONE4
	    [DME_STGSLCT_ZONE_TYPE_NONE, DME_STGSLCT_ZONE_TYPE_SPE
            , DME_STGSLCT_ZONE_TYPE_1, DME_STGSLCT_ZONE_TYPE_3],		// FINAL
	    [DME_STGSLCT_ZONE_TYPE_FINAL, DME_STGSLCT_ZONE_TYPE_NONE
            , DME_STGSLCT_ZONE_TYPE_2, DME_STGSLCT_ZONE_TYPE_4],		// SPE
    ];



    // ZONE表示位置テーブル(FINAL無)
    private static readonly float[][] dm_stgslct_n_zone_disp_pos_tbl = [
#if !_IPHONE
	    [160.0f, 150.0f],
        [160.0f, 350.0f],
        [560.0f, 150.0f],
        [560.0f, 350.0f],
        [360.0f,-280.0f],
        [360.0f, 780.0f],
#else //!_IPHONE
	    [160.0f-50.0f, 150.0f-30.0f],
        [160.0f-50.0f, 350.0f-20.0f],
        [560.0f-10.0f, 150.0f-30.0f],
        [560.0f-10.0f, 350.0f-20.0f],
        [360.0f-30.0f,-280.0f-30.0f],
        [360.0f-30.0f, 780.0f-20.0f],
#endif //!_IPHONE
    ];


    // ZONE非表示位置テーブル(FINAL無)
    private static readonly float[][] dm_stgslct_n_zone_nodisp_pos_tbl = [
#if !_IPHONE
        [-416.0f, 100.0f],
        [-416.0f, 400.0f],
        [1120.0f, 100.0f],
        [1120.0f, 400.0f],
        [360.0f, -280.0f],
        [360.0f,  780.0f],
#else //!_IPHONE
	    [-416.0f-50.0f, 100.0f -30.0f],
        [-416.0f-50.0f, 400.0f -20.0f],
        [1120.0f-10.0f, 100.0f -30.0f],
        [1120.0f-10.0f, 400.0f -20.0f],
        [360.0f -30.0f, -280.0f-30.0f],
        [360.0f -30.0f,  780.0f-20.0f],
#endif //!_IPHONE
    ];


    // ZONE表示位置テーブル(FINAL解放)
    private static readonly float[][] dm_stgslct_f_zone_disp_pos_tbl = [
#if !_IPHONE
        [80.0f, 150.0f],
        [80.0f, 350.0f],
        [640.0f, 150.0f],
        [640.0f, 350.0f],
        [360.0f, 260.0f],
        [360.0f, 780.0f],
#else //!_IPHONE
	    [80.0f -50.0f, 150.0f-30.0f],
        [80.0f -50.0f, 350.0f-20.0f],
        [640.0f-10.0f, 150.0f-30.0f],
        [640.0f-10.0f, 350.0f-20.0f],
        [360.0f-30.0f, 260.0f-30.0f],
        [360.0f-30.0f, 780.0f-20.0f],
#endif //!_IPHONE
    ];


    // ZONE非表示位置テーブル(FINAL解放)
    private static readonly float[][] dm_stgslct_f_zone_nodisp_pos_tbl = [
#if !_IPHONE
        [-416.0f, 100.0f],
        [-416.0f, 400.0f],
        [1120.0f, 100.0f],
        [1120.0f, 400.0f],
        [360.0f, -280.0f],
        [360.0f,  780.0f],
#else //!_IPHONE
	    [-416.0f-50.0f, 100.0f -30.0f],
        [-416.0f-50.0f, 400.0f -20.0f],
        [1120.0f-10.0f, 100.0f -30.0f],
        [1120.0f-10.0f, 400.0f -20.0f],
        [360.0f -30.0f, -280.0f-30.0f],
        [360.0f -30.0f,  780.0f-20.0f],
#endif //!_IPHONE
    ];


    // ZONE表示位置テーブル(スペ解放)
    private static readonly float[][] dm_stgslct_s_zone_disp_pos_tbl = [
#if !_IPHONE
        [80.0f, 150.0f],
        [80.0f, 350.0f],
        [640.0f, 150.0f],
        [640.0f, 350.0f],
        [360.0f,-280.0f],
        [360.0f, 260.0f],
#else //!_IPHONE
	    [80.0f -50.0f, 150.0f-30.0f],
        [80.0f -50.0f, 350.0f-20.0f],
        [640.0f-10.0f, 150.0f-30.0f],
        [640.0f-10.0f, 350.0f-20.0f],
        [360.0f-30.0f,-280.0f-30.0f],
        [360.0f-30.0f, 260.0f-20.0f],
#endif //!_IPHONE
    ];


    // ZONE非表示位置テーブル(スペ解放)
    private static readonly float[][] dm_stgslct_s_zone_nodisp_pos_tbl = [
#if !_IPHONE
        [-416.0f, 100.0f],
        [-416.0f, 400.0f],
        [1120.0f, 100.0f],
        [1120.0f, 400.0f],
        [360.0f, -280.0f],
        [360.0f,  780.0f],
#else //!_IPHONE
	    [-416.0f-50.0f, 100.0f -30.0f],
        [-416.0f-50.0f, 400.0f -20.0f],
        [1120.0f-10.0f, 100.0f -30.0f],
        [1120.0f-10.0f, 400.0f -20.0f],
        [360.0f -30.0f, -280.0f-30.0f],
        [360.0f -30.0f,  780.0f-20.0f],
#endif //!_IPHONE
    ];


    // ZONE表示位置テーブル(6つVer)
    private static readonly float[][] dm_stgslct_a_zone_disp_pos_tbl = [
#if !_IPHONE
        [80.0f, 150.0f],
        [80.0f, 350.0f],
        [640.0f, 150.0f],
        [640.0f, 350.0f],
        [360.0f, 150.0f],
        [360.0f, 350.0f],
#else //!_IPHONE
	    [80.0f -50.0f, 150.0f-30.0f],
        [80.0f -50.0f, 350.0f-20.0f],
        [640.0f-10.0f, 150.0f-30.0f],
        [640.0f-10.0f, 350.0f-20.0f],
        [360.0f-30.0f, 150.0f-30.0f],
        [360.0f-30.0f, 350.0f-20.0f],
#endif //!_IPHONE
    ];


    // ZONE非表示位置テーブル(6つVersion)
    private static readonly float[][] dm_stgslct_a_zone_nodisp_pos_tbl = [
#if !_IPHONE
        [-416.0f, 100.0f],
        [-416.0f, 380.0f],
        [1120.0f, 120.0f],
        [1120.0f, 380.0f],
        [360.0f, -280.0f],
        [360.0f,  780.0f],
#else //!_IPHONE
	    [-416.0f-50.0f, 100.0f -30.0f],
        [-416.0f-50.0f, 380.0f -20.0f],
        [1120.0f-10.0f, 120.0f -30.0f],
        [1120.0f-10.0f, 380.0f -20.0f],
        [360.0f -30.0f, -280.0f-30.0f],
        [360.0f -30.0f,  780.0f-20.0f],
#endif //!_IPHONE
    ];



    // ZONEごとのACTトップ表示位置Yテーブル
    private static readonly float[] dm_stgslct_act_disp_y_pos_tbl = [
#if !_IPHONE
        160.0f, 160.0f, 160.0f, 160.0f,
        160.0f, 160.0f, 160.0f, 160.0f,
        160.0f, 160.0f, 160.0f, 160.0f,
        160.0f, 160.0f, 160.0f, 160.0f,
        320.0f,
        160.0f, 160.0f, 160.0f, 160.0f, 160.0f, 160.0f, 160.0f
#else //!_IPHONE
	    190.0f, 190.0f, 190.0f, 190.0f,
        190.0f, 190.0f, 190.0f, 190.0f,
        190.0f, 190.0f, 190.0f, 190.0f,
        190.0f, 190.0f, 190.0f, 190.0f,
        320.0f,
        190.0f, 190.0f, 190.0f, 190.0f, 190.0f, 190.0f, 190.0f
#endif //!_IPHONE
    ];


    // ZONEごとのACTテーブル表示位置Yテーブル
#if !_IPHONE
    private static readonly float[] dm_stgslct_act_tab_disp_y_pos_tbl = [
    160.0f,
    160.0f + 88.0f * -1.0f,
    160.0f + 88.0f * -2.0f,
    160.0f + 88.0f * -3.0f,
];
#else //!_IPHONE
    private static readonly float[] dm_stgslct_act_tab_disp_y_pos_tbl = [
        dm_stgslct_act_disp_y_pos_tbl[(int)DME_STGSLCT_STAGE_1_1] - (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8) * 0,
        dm_stgslct_act_disp_y_pos_tbl[(int)DME_STGSLCT_STAGE_1_1] - (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8) * 1,
        dm_stgslct_act_disp_y_pos_tbl[(int)DME_STGSLCT_STAGE_1_1] - (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8) * 2,
        dm_stgslct_act_disp_y_pos_tbl[(int)DME_STGSLCT_STAGE_1_1] - (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8) * 3,
        dm_stgslct_act_disp_y_pos_tbl[(int)DME_STGSLCT_STAGE_1_1] - (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8) * 4,
    ];
#endif //!_IPHONE



    // ZONEごとのカーソル表示位置Yテーブル
    private static readonly float[] dm_stgslct_act_crsr_disp_y_pos_tbl = [
        160.0f,
        160.0f + 88.0f * 1.0f,
        160.0f + 88.0f * 2.0f,
        160.0f + 88.0f * 3.0f,
    ];


    // 表示ウインドウアクションIDテーブル
    private static readonly float[][] dm_stgslct_win_act_frm_tbl = [
        // タイトル			メッセージ			OK
        [2.0f, 0.0f, 1.0f],		// メニュー
	    [3.0f, 0.0f, 0.0f],		// アクト決定
	    [1.0f, 1.0f, 0.0f],		// ACT選択可能メッセージ
	    [1.0f, 2.0f, 0.0f],		// BOSSACT選択可能メッセージ
	    [1.0f, 3.0f, 0.0f],		// BOSSACT選択可能メッセージ
	    [1.0f, 4.0f, 0.0f],		// BOSSACT選択可能メッセージ
	    [1.0f, 5.0f, 0.0f],		// BOSSACT選択可能メッセージ
	    [1.0f, 6.0f, 0.0f],		// FINALZONE選択可能メッセージ
	    [1.0f, 7.0f, 0.0f],		// スーパーソニック変身可能メッセージ
	    [1.0f, 7.0f, 0.0f],		// SPECIALZONE選択可能メッセージ
    //	フレーム、フレーム、パターン番号
    ];


    // ウインドウ選択肢用表示フレームテーブル(現状２つ用)
    private static readonly float[][] dm_stgslct_win_disp_slct_frm_tbl = [
        [0.0f, 1.0f],		// 左(上)がアクティブ
	    [1.0f, 0.0f],		// 右(下)がアクティブ
    ];



    // ACT縦並びの表示位置テーブル
    private static readonly float[] dm_stgslct_vrtcl_disp_pos_y_tbl = [
        DMD_STGSLCT_ACT_VRTCL_CHNG_DIST * 0.0f,
        DMD_STGSLCT_ACT_VRTCL_CHNG_DIST * 1.0f,
        DMD_STGSLCT_ACT_VRTCL_CHNG_DIST * 2.0f,
        DMD_STGSLCT_ACT_VRTCL_CHNG_DIST * 3.0f,
    ];


    private static readonly float[] dm_stgslct_back_text_length_tbl = [
        -39.0f,
        -53.0f,
        -69.0f,
        -77.0f,
        -70.0f,
        -55.0f,
    ];

#if !(_WII || _IPHONE)
    private static readonly float[][] dm_stgslct_win_act_pos_tbl = [
        [DMD_STGSLCT_SIZE_HALF_WIDTH + 42.0f, 280.0f],	// ウインドウ内のライン
	    [DMD_STGSLCT_SIZE_HALF_WIDTH + 182.0f, 264.0f],	// キャンセルボタン
	    [DMD_STGSLCT_SIZE_HALF_WIDTH - 136.0f, 400.0f],	// Xボタン
	    [DMD_STGSLCT_SIZE_HALF_WIDTH - 90.0f, 400.0f],	// Yボタン
	    [DMD_STGSLCT_SIZE_HALF_WIDTH, 360.0f],			// メッセージ
	    [DMD_STGSLCT_SIZE_HALF_WIDTH, 344.0f],			// メッセージ2
	    [DMD_STGSLCT_SIZE_HALF_WIDTH, 360.0f],			// スーパーソニックメッセージ
	    [DMD_STGSLCT_SIZE_HALF_WIDTH, 344.0f],			// オプション
	    [DMD_STGSLCT_SIZE_HALF_WIDTH, 404.0f],			// メインメニュー
	    [DMD_STGSLCT_SIZE_HALF_WIDTH - 80.0f, 274.0f],	// タイトルテキスト
	    [DMD_STGSLCT_SIZE_HALF_WIDTH + 202.0f, 264.0f],	// 戻る
	    [DMD_STGSLCT_SIZE_HALF_WIDTH, 420.0f],			// OK
	    [DMD_STGSLCT_SIZE_HALF_WIDTH - 88.0f, 420.0f],	// YES
	    [DMD_STGSLCT_SIZE_HALF_WIDTH + 88.0f, 420.0f],	// NO
    ];
#elif _WII
    private static readonly float[][] dm_stgslct_win_act_pos_tbl = [
	    [DMD_STGSLCT_SIZE_HALF_WIDTH + 42.0f, 280.0f],	// ウインドウ内のライン
	    [DMD_STGSLCT_SIZE_HALF_WIDTH + 278.0f, 224.0f],	// キャンセルボタン
	    [DMD_STGSLCT_SIZE_HALF_WIDTH - 136.0f, 400.0f],	// Xボタン
	    [DMD_STGSLCT_SIZE_HALF_WIDTH - 90.0f, 400.0f],	// Yボタン
	    [DMD_STGSLCT_SIZE_HALF_WIDTH, 360.0f],			// メッセージ
	    [DMD_STGSLCT_SIZE_HALF_WIDTH, 328.0f],			// メッセージ2
	    [DMD_STGSLCT_SIZE_HALF_WIDTH, 360.0f],			// スーパーソニックメッセージ
	    [DMD_STGSLCT_SIZE_HALF_WIDTH, 344.0f],			// オプション
	    [DMD_STGSLCT_SIZE_HALF_WIDTH, 404.0f],			// メインメニュー
	    [DMD_STGSLCT_SIZE_HALF_WIDTH - 80.0f, 274.0f],	// タイトルテキスト
	    [DMD_STGSLCT_SIZE_HALF_WIDTH + 298.0f, 224.0f],	// 戻る
	    [DMD_STGSLCT_SIZE_HALF_WIDTH, 420.0f],			// OK
	    [DMD_STGSLCT_SIZE_HALF_WIDTH - 88.0f, 420.0f],	// YES
	    [DMD_STGSLCT_SIZE_HALF_WIDTH + 88.0f, 420.0f],	// NO
    ];
#else // #if _IPHONE
    private static readonly float[][] dm_stgslct_win_act_pos_tbl = [
        [DMD_STGSLCT_SIZE_HALF_WIDTH + 42.0f, 280.0f],	// ウインドウ内のライン
	    [DMD_STGSLCT_SIZE_HALF_WIDTH + 278.0f, 224.0f],	// キャンセルボタン
	    [DMD_STGSLCT_SIZE_HALF_WIDTH - 136.0f, 400.0f],	// Xボタン
	    [DMD_STGSLCT_SIZE_HALF_WIDTH - 90.0f, 400.0f],	// Yボタン
	    [DMD_STGSLCT_SIZE_HALF_WIDTH, 360.0f],			// メッセージ
	    [DMD_STGSLCT_SIZE_HALF_WIDTH, 248.0f],			// メッセージ2
	    [DMD_STGSLCT_SIZE_HALF_WIDTH, 360.0f],			// スーパーソニックメッセージ
	    [DMD_STGSLCT_SIZE_HALF_WIDTH, 344.0f],			// オプション
	    [DMD_STGSLCT_SIZE_HALF_WIDTH, 404.0f],			// メインメニュー
	    [DMD_STGSLCT_SIZE_HALF_WIDTH - 80.0f, 274.0f],	// タイトルテキスト
	    [DMD_STGSLCT_SIZE_HALF_WIDTH + 298.0f, 224.0f],	// 戻る
	    [DMD_STGSLCT_SIZE_HALF_WIDTH, 420.0f],			// OK
	    [DMD_STGSLCT_SIZE_HALF_WIDTH - 176.0f, 420.0f],	// YES
	    [DMD_STGSLCT_SIZE_HALF_WIDTH + 176.0f, 420.0f],	// NO
    ];
#endif // #if !(_WII || _IPHONE)

    // 各ステージの設定アクションID共通テーブル
    private static readonly float[] dm_stgslct_act_table_disp_id_tbl = [
        0.0f, 1.0f, 2.0f, 3.0f,						// ZONE1
	    0.0f, 1.0f, 2.0f, 3.0f,						// ZONE2
	    0.0f, 1.0f, 2.0f, 3.0f,						// ZONE3
	    0.0f, 1.0f, 2.0f, 3.0f,						// ZONE4
	    4.0f,										// FINAL STAGE
	    5.0f, 5.0f, 5.0f, 5.0f, 5.0f, 5.0f, 5.0f,	// SPECIAL STAGE
    ];


    // 各ステージのメッセージアクションID共通テーブル
    private static readonly float[] dm_stgslct_disp_msg_id_table_tbl = [
        0.0f, 1.0f, 2.0f, 3.0f,						// ZONE1
	    4.0f, 5.0f, 6.0f, 7.0f,						// ZONE2
	    8.0f, 9.0f, 10.0f, 11.0f,					// ZONE3
	    12.0f, 13.0f, 14.0f, 15.0f,					// ZONE4
	    16.0f,										// FINAL STAGE
	    5.0f, 5.0f, 5.0f, 5.0f, 5.0f, 5.0f, 5.0f,	// SPECIAL STAGE
    ];


    // 各ステージのY座標テーブル
    private static readonly float[] dm_stgslct_act_disp_pos_y_tbl = [
        0.0f, 1.0f, 2.0f, 3.0f,						// ZONE1
	    0.0f, 1.0f, 2.0f, 3.0f,						// ZONE2
	    0.0f, 1.0f, 2.0f, 3.0f,						// ZONE3
	    0.0f, 1.0f, 2.0f, 3.0f,						// ZONE4
	    0.0f,										// FINAL STAGE
	    0.0f, 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f,	// SPECIAL STAGE
    ];


    // 各ステージのACTアクションIDテーブル
    private static readonly float[] dm_stgslct_act_num_disp_id_tbl = [
        0.0f, 1.0f, 2.0f, 0.0f,						// ZONE1
	    0.0f, 1.0f, 2.0f, 0.0f,						// ZONE2
	    0.0f, 1.0f, 2.0f, 0.0f,						// ZONE3
	    0.0f, 1.0f, 2.0f, 0.0f,						// ZONE4
	    0.0f,										// FINAL STAGE
	    0.0f, 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f,	// SPECIAL STAGE
    ];

    // アクションIDテーブル(初期状態)
    private static readonly uint[] g_dm_act_id_tbl = [
        IDA_D_STGSLCT_ACT_Z_BG_LT,
        IDA_D_STGSLCT_ACT_Z_BG_LB,
        IDA_D_STGSLCT_ACT_Z_BG_RT,
        IDA_D_STGSLCT_ACT_Z_BG_RB,
        IDA_D_STGSLCT_ACT_TAB_NORMAL_L,
    #if !_IPHONE
	    IDA_D_STGSLCT_ACT_TAB_NORMAL_R,
    #endif //!_IPHONE
	    IDA_D_STGSLCT_ACT_ICON_SONIC,
    #if !_IPHONE
	    IDA_D_STGSLCT_ACT_TEX_SONIC,
    #endif //!_IPHONE
	    IDA_D_STGSLCT_ACT_NUM_A_100,
        IDA_D_STGSLCT_ACT_NUM_A_10,
        IDA_D_STGSLCT_ACT_NUM_A_1,
        IDA_D_STGSLCT_ACT_EMER_TAB1,
        IDA_D_STGSLCT_ACT_ICON_EMER1,
        IDA_D_STGSLCT_ACT_ICON_EMER2,
        IDA_D_STGSLCT_ACT_ICON_EMER3,
        IDA_D_STGSLCT_ACT_ICON_EMER4,
        IDA_D_STGSLCT_ACT_ICON_EMER5,
        IDA_D_STGSLCT_ACT_ICON_EMER6,
        IDA_D_STGSLCT_ACT_ICON_EMER7,
        IDA_D_STGSLCT_ACT_TEX_ZNAME,

	    // モード共通・言語別
    #if !_IPHONE
	    IDA_D_STGSLCT_JP_ACT_TEX_TIME,
        IDA_D_STGSLCT_JP_ACT_TEX_SCORE,
        IDA_D_STGSLCT_JP_ACT_TEX_MENU,
        IDA_D_STGSLCT_ACT_TEX_EME,
    #endif //!_IPHONE
	    IDA_D_STGSLCT_JP_ACT_TEX_ZNAME,		// ZONE名
    #if !_IPHONE
	    IDA_D_STGSLCT_JP_ACT_TEX_MESS,
        IDA_D_STGSLCT_JP_ACT_TEX_MESS,
    #endif //!_IPHONE

	    // ZONE選択・言語共通
	    IDA_D_STGSLCT_ACT_ZONE_SCR1,
        IDA_D_STGSLCT_ACT_ZONE_SCR2,
        IDA_D_STGSLCT_ACT_ZONE_SCR3,
        IDA_D_STGSLCT_ACT_ZONE_SCR4,
        IDA_D_STGSLCT_ACT_ZONE_SCR5,
        IDA_D_STGSLCT_ACT_ZONE_SCR6,
        IDA_D_STGSLCT_ACT_ZONE_SCR1_A,
        IDA_D_STGSLCT_ACT_ZONE_SCR1_B,
        IDA_D_STGSLCT_ACT_ZONE_SCR1_C,
        IDA_D_STGSLCT_ACT_ZONE_SCR2_A,
        IDA_D_STGSLCT_ACT_ZONE_SCR2_B,
        IDA_D_STGSLCT_ACT_ZONE_SCR2_C,
        IDA_D_STGSLCT_ACT_ZONE_SCR3_A,
        IDA_D_STGSLCT_ACT_ZONE_SCR3_B,
        IDA_D_STGSLCT_ACT_ZONE_SCR3_C,
        IDA_D_STGSLCT_ACT_ZONE_SCR4_A,
        IDA_D_STGSLCT_ACT_ZONE_SCR4_B,
        IDA_D_STGSLCT_ACT_ZONE_SCR4_C,
        IDA_D_STGSLCT_ACT_ZONE_TAB,
        IDA_D_STGSLCT_ACT_ZONE_TEXT,
        IDA_D_STGSLCT_JP_ACT_ZONE_TEXT,
        IDA_D_STGSLCT_ACT_ZONE_COVER1,
        IDA_D_STGSLCT_ACT_ZONE_COVER2,
        IDA_D_STGSLCT_ACT_ZONE_COVER3,
    #if !_IPHONE
	    IDA_D_STGSLCT_ACT_ZONE_CURSOR2,
        IDA_D_STGSLCT_ACT_ZONE_CURSOR1,
        IDA_D_STGSLCT_ACT_ZONE_CURSOR3,
    #endif //!_IPHONE

	    // ACT選択
	    IDA_D_STGSLCT_ACT_ICON1,
        IDA_D_STGSLCT_ACT_ICON2,
        IDA_D_STGSLCT_ACT_ICON3,
        IDA_D_STGSLCT_ACT_ICON4,
        IDA_D_STGSLCT_ACT_ICON5,
        IDA_D_STGSLCT_ACT_ICON6,
        IDA_D_STGSLCT_ACT_ARROW1,
        IDA_D_STGSLCT_ACT_ARROW2,
        IDA_D_STGSLCT_ACT_TAB_TIME_L,
        IDA_D_STGSLCT_ACT_TAB_TIME_C,
        IDA_D_STGSLCT_ACT_TAB_TIME_R,
        IDA_D_STGSLCT_ACT_TAB_TIME_L2,
        IDA_D_STGSLCT_ACT_TAB_TIME_C2,
        IDA_D_STGSLCT_ACT_TAB_TIME_R2,
        IDA_D_STGSLCT_ACT_TAB_TIME_MOVE,
        IDA_D_STGSLCT_ACT_ACT1_TAB_B,
        IDA_D_STGSLCT_ACT_ACT1_TAB_A,
        IDA_D_STGSLCT_ACT_ACT1_TAB_C,
        IDA_D_STGSLCT_ACT_ACT1_SCR,
        IDA_D_STGSLCT_ACT_ACT1_SCR_BG,
        IDA_D_STGSLCT_JP_ACT_ACT1_TEX_ACT,
        IDA_D_STGSLCT_ACT_ACT1_NUM_B,
        IDA_D_STGSLCT_JP_ACT_ACT1_MESS,
        IDA_D_STGSLCT_ACT_ACT1_LINE,
        IDA_D_STGSLCT_ACT_ACT1_ICON_EMER,
        IDA_D_STGSLCT_ACT_SPE_NUM_STAGE,
        IDA_D_STGSLCT_ACT_SPE_EMER,
        IDA_D_STGSLCT_ACT_ACT_CURSOR_UP,
        IDA_D_STGSLCT_ACT_ACT_CURSOR_DOWN,
    #if !_IPHONE
	    IDA_D_STGSLCT_ACT_ACT_CURSOR2,
        IDA_D_STGSLCT_ACT_ACT_CURSOR1,
        IDA_D_STGSLCT_ACT_ACT_CURSOR3,
    #endif //!_IPHONE
	    IDA_D_STGSLCT_JP_ACT_ACT1_TEX_SCORE,
        IDA_D_STGSLCT_JP_ACT_ACT1_TEX_TIME,
        IDA_D_STGSLCT_JP_ACT_BOSS_TEX_BOSS,
        IDA_D_STGSLCT_JP_ACT_SPE_TEX_STAGE,
        IDA_D_STGSLCT_ACT_ACT1_NUM_A1,
        IDA_D_STGSLCT_ACT_ACT1_NUM_A2,
        IDA_D_STGSLCT_ACT_ACT1_NUM_A3,
        IDA_D_STGSLCT_ACT_ACT1_NUM_A4,
        IDA_D_STGSLCT_ACT_ACT1_NUM_A5,
        IDA_D_STGSLCT_ACT_ACT1_NUM_A6,
        IDA_D_STGSLCT_ACT_ACT1_NUM_A7,
        IDA_D_STGSLCT_ACT_ACT1_NUM_A8,
        IDA_D_STGSLCT_ACT_ACT1_NUM_A9,
        IDA_D_STGSLCT_ACT_ACT1_COVER2,
        IDA_D_STGSLCT_ACT_ACT1_COVER1,
        IDA_D_STGSLCT_ACT_ACT1_COVER3,

        IDA_D_STGSLCT_JP_ACT_TEX_SCTM01A,
        IDA_D_STGSLCT_JP_ACT_TEX_SCTM02A,
    #if !_IPHONE
	    IDA_D_STGSLCT_JP_ACT_TEX_SCTM01B,
        IDA_D_STGSLCT_JP_ACT_TEX_SCTM02B,
    #endif //!_IPHONE

	    IDA_D_STGSLCT_JP_ACT_TEX_WIN_MSG,
        IDA_D_STGSLCT_JP_ACT_TEX_WIN_MSG2,
        IDA_D_STGSLCT_JP_ACT_TEX_WIN_SSONIC,

	    // メニュー共通データ
	    IDA_D_CMN_BG_ACT_BG_WAVE,				//!< 
	    IDA_D_CMN_BG_ACT_BG_DOWN_WHITE,			//!< 
	    IDA_D_CMN_BG_ACT_BG_BLUE,				//!<
	
	    IDA_D_CMN_BTN_ACT_BTN_BACK_STGSLCT,		//!< 
	    IDA_D_CMN_BTN_ACT_BTN_LB_STGSLCT,		//!< 
	    IDA_D_CMN_BTN_ACT_BTN_Y_STGSLCT,		//!< 
	    IDA_D_CMN_BTN_ACT_BTN_LEFT_LB_STGSLCT,	//!< 
	    IDA_D_CMN_BTN_ACT_BTN_LEFT_RB_STGSLCT,	//!< 
	    IDA_D_CMN_BTN_ACT_BACK_BTN,				//!<
    #if !_WII
	    IDA_D_CMN_BTN_ACT_BTN_X,				//!< 
	    IDA_D_CMN_BTN_ACT_BTN_Y,				//!<
    #else
	    IDA_D_CMN_BTN_ACT_BACK_BTN,				//!< 
	    IDA_D_CMN_BTN_ACT_BACK_BTN,				//!<
    #endif
	
    #if !_IPHONE
	    IDA_D_CMN_OBI_ACT_OBI_CENTER,			//!< 
	    IDA_D_CMN_OBI_ACT_OBI_LEFT,				//!< 
	    IDA_D_CMN_OBI_ACT_OBI_RIGHT,			//!< 
	
	    IDA_D_CMN_WIN_ACT_WIN_LINE,				//!< 
    #else //!_IPHONE
	    IDA_D_CMN_WIN_ACT_STGSLCT_BACK_L,
        IDA_D_CMN_WIN_ACT_STGSLCT_BACK_C,

        IDA_D_CMN_WIN_ACT_BTN01_L,
        IDA_D_CMN_WIN_ACT_BTN01_C,
        IDA_D_CMN_WIN_ACT_BTN01_R,
        IDA_D_CMN_WIN_ACT_BTN01_L,
        IDA_D_CMN_WIN_ACT_BTN01_C,
        IDA_D_CMN_WIN_ACT_BTN01_R,
    #endif //!_IPHONE
	
	    IDA_D_CMN_MSG_JP_ACT_TEX_BACK_STGSLCT,	//!< 
    #if !_IPHONE
	    IDA_D_CMN_MSG_JP_ACT_TEX_WINTITLE,		//!< 
    #endif //!_IPHONE
	    IDA_D_CMN_MSG_JP_ACT_TEX_BACK,			//!< 
    #if !_IPHONE
	    IDA_D_CMN_MSG_JP_ACT_TEX_OK,			//!< 
    #endif //!_IPHONE
	    IDA_D_CMN_MSG_JP_ACT_TEX_YES,			//!< 
	    IDA_D_CMN_MSG_JP_ACT_TEX_NO,			//!< 
    ];


    private static bool dm_stgslct_is_stage_start = false;


    // ----- Global Functions ----------------------（グローバル関数の定義：外部）
    // ==========================================================================
    // DmStgSlctStart
    /*!
        ステージ選択画面開始処理
     */
    // ==========================================================================
    public static void DmStgSlctStart(object arg)
    {
        UNREFERENCED_PARAMETER(arg);

        dmStgSlctInit();
    }


    // ----- Static Functions --------------------（スタティック関数の定義：局所）
    // ==========================================================================
    // dmStgSlctInit
    /*!
        ステージ選択画面初期化処理
     */
    // ==========================================================================
    private static void dmStgSlctInit()
    {
        // アクションシステム初期化
        AoActSysSetDrawStateEnable(false);      // どのデモを開始する際も必ず設定

        // メインタスク作成

        //mppAchievementSupport.get().event_LoseRingCounterClear();//sss - bad

        DMS_STGSLCT_MAIN_WORK main_work = (DMS_STGSLCT_MAIN_WORK)MTM_TASK_MAKE_TCB(
            new GSF_TASK_PROCEDURE(dmStgSlctProcMain), new GSF_TASK_PROCEDURE(dmStgSlctDest), 0U, 0, 8192U, 0,
            () => new DMS_STGSLCT_MAIN_WORK(), "STGSLCT_MAIN").work;

        // 初期化処理があればここに記述
        // リージョンデータ取得(ボタン表示切り替え用)
        if (GeEnvGetDecideKey() == GSE_DECIDE_KEY.GSD_DECIDE_KEY_O)
        {
            main_work.is_jp_region = true;
        }
        else
        {
            main_work.is_jp_region = false;
        }

#if MTD_DEBUG
	// ※※※ここでステージ全解放入力があるかのチェックを行う		※デバッグ時のみ有効
#if !_IPHONE
	if (AoPadDirect() & KEY_R_UP) {
#else //!_IPHONE
	if (amTpIsTouchOn(2)) {
#endif //!_IPHONE
		dmStgSlctSetAllOpenStage(main_work);
	}
#endif



        // 初期化設定
        dmStgSlctSetInitData(main_work);

        // ハイスコア設定(レコードタイムも)
        dmStgSlctSetHiScore(main_work);

        // 各ステージのクリア情報設定(ここからどのステージが選択できるかを判別)
        dmStgSlctSetClearInfo(main_work);

#if MPPDEBUG_OPEN_ALL			
        if (!false)
        {
            {//qqq//test -- open all
                for (int iii = 0; iii <= 23; iii++)
                {
                    main_work.is_clear_stage[iii] = true;
                    /*
                    if(true) {//test2 time att
                        main_work.record_time[iii] = 6000+iii;
                        main_work.hi_score[iii] = 5000+iii;
                    }*/
                }
                main_work.is_final_open = DMD_STGSLCT_ZONE_ALL_OPEN;
            }
        }
#endif


        // 初期アナウンス表示設定(前イベント、グローバル変数から取得)
        dmStgSlctSetAnnounceMsg(main_work);

        main_work.tex_u[0] = main_work.tex_u[1] =
        main_work.tex_v[0] = main_work.tex_v[1] = 0.1f;

        // プロシージャ設定
        main_work.proc_menu_update = dmStgSlctLoadFontData;


        //	DmLoadingStart();

        // タスク開始
        //	amTaskStart(tcb);
    }



    // ==========================================================================
    // dmStgSlctSetInitData
    /*!
        必要初期化設定処理
     */
    // ==========================================================================
    private static void dmStgSlctSetInitData(DMS_STGSLCT_MAIN_WORK main_work)
    {
        short tmp_prev_evt = 0;

        UNREFERENCED_PARAMETER(main_work);

        tmp_prev_evt = SyGetEvtInfo().old_evt_id;

        // ZONE選択かACT選択かのどちらから始めるかを設定
        if (tmp_prev_evt == (short)GSD_EVT_ID_MAINGAME
            || tmp_prev_evt == (short)GSD_EVT_ID_SPSTAGE_BRANCH)
        {
            dm_stgslct_is_stage_start = true;

            // ゲームモード設定
            main_work.cur_game_mode = (uint)g_gs_main_sys_info.game_mode;
        }
        else if (tmp_prev_evt == (short)GSD_EVT_ID_TITLE
                 || tmp_prev_evt == (short)GSD_EVT_ID_MAINMENU)
        {
            dm_stgslct_is_stage_start = false;

            // ゲームモード設定(デフォルト設定)
            main_work.cur_game_mode = (uint)0;
        }
        else if (tmp_prev_evt == (short)GSD_EVT_ID_RANKING)
        {
            // ACT選択からしか遷移できないため、ACT選択からスタート
            dm_stgslct_is_stage_start = true;

            // ゲームモード設定
            main_work.cur_game_mode = (uint)g_gs_main_sys_info.game_mode;

            // ランキングから戻ったことを示すフラグON
            main_work.flag |= DMD_STGSLCT_FLAG_PREV_EVT_RANKING;
        }
        else
        {
            // 上記のパターン以外はありえないので現状アサート	◆
            //		MTM_ASSERT(0);
            dm_stgslct_is_stage_start = false;

            // ゲームモード設定(デフォルト設定)
            main_work.cur_game_mode = (uint)0;
        }

        // BGフェード初期値設定
        main_work.bg_fade.r = 255;
        main_work.bg_fade.g = 255;
        main_work.bg_fade.b = 255;
        main_work.bg_fade.a = 0;

        if (dm_stgslct_is_stage_start)
        {
            if (g_gs_main_sys_info.stage_id < GSD_MAIN_STAGE_ID_FINAL_1)
            {
                main_work.cur_stage = dmStgSlctSetNextFocusAct(main_work, g_gs_main_sys_info.stage_id);

                //			main_work.cur_stage = g_gs_main_sys_info.stage_id;

                main_work.cur_zone = dm_stgslct_act_zone_no_tbl[main_work.cur_stage];
                main_work.chng_zone = (uint)DME_STGSLCT_ACT_PAGE_NONE;

                main_work.focus_disp_no = 0;
                main_work.crsr_idx = (int)(main_work.cur_stage % 4);
            }
            else if (g_gs_main_sys_info.stage_id >= GSD_MAIN_STAGE_ID_ENDING)
            {
                // エンディングステージからステセレには来ないため、アサート
                MTM_ASSERT(0);

                // 仮設定
                main_work.cur_stage = 0;
                main_work.cur_zone = 0;
                main_work.chng_zone = (uint)DME_STGSLCT_ACT_PAGE_NONE;

                main_work.focus_disp_no = 0;
                main_work.crsr_idx = 0;
            }
            else if (g_gs_main_sys_info.stage_id >= GSD_MAIN_STAGE_ID_SS1)
            {
                // 通常ACTからスペステリングに入った場合
                if (g_gs_main_sys_info.prev_stage_id != 0xffff)
                {
                    main_work.cur_stage = dmStgSlctSetNextFocusAct(main_work, g_gs_main_sys_info.prev_stage_id);
                    main_work.cur_zone = dm_stgslct_act_zone_no_tbl[main_work.cur_stage];
                    main_work.chng_zone = (uint)DME_STGSLCT_ACT_PAGE_NONE;

                    main_work.focus_disp_no = 0;
                    main_work.crsr_idx = (int)(main_work.cur_stage % 4);
                }
                // ステージ選択からスペステを選んだ場合
                else
                {
                    main_work.cur_stage = (int)(dm_stgslct_zone_act_num_tbl[(int)DME_STGSLCT_ACT_PAGE_SPE][0]
                                            + (g_gs_main_sys_info.stage_id - GSD_MAIN_STAGE_ID_SS1));
                    main_work.cur_zone = (uint)DME_STGSLCT_ZONE_TYPE_SPE;
                    main_work.chng_zone = (uint)DME_STGSLCT_ACT_PAGE_NONE;

                    if (main_work.cur_stage <= (int)DME_STGSLCT_STAGE_S_4)
                    {
                        main_work.focus_disp_no = (int)(main_work.cur_stage - DME_STGSLCT_STAGE_S_1);
                        main_work.crsr_idx = 0;
                    }
                    else
                    {
                        main_work.focus_disp_no = 3;
                        main_work.crsr_idx = (int)(main_work.cur_stage - DME_STGSLCT_STAGE_S_4);
                    }
                }
            }
            else
            {
                main_work.cur_stage = (int)DME_STGSLCT_STAGE_F_1;
                main_work.cur_zone = (uint)DME_STGSLCT_ZONE_TYPE_FINAL;
                main_work.chng_zone = (uint)DME_STGSLCT_ACT_PAGE_NONE;

                main_work.focus_disp_no = 0;
                main_work.crsr_idx = 0;
            }

            if (main_work.cur_zone != (uint)DME_STGSLCT_ACT_PAGE_FINAL)
            {
                main_work.crsr_pos_y = dm_stgslct_act_crsr_disp_y_pos_tbl[main_work.crsr_idx];
            }
            else
            {
                main_work.crsr_pos_y = 320.0f;
            }
        }

        // アニメーション初期化
        main_work.btn_l_disp_frm = DMD_STGSLCT_ZONE_CHNG_BTN_END_FRM;
        main_work.btn_r_disp_frm = DMD_STGSLCT_ZONE_CHNG_BTN_END_FRM;

        // 背景設定
        main_work.cur_bg_id = main_work.cur_zone;

        // 帯テキストの初期表示位置設定
        main_work.obi_pos[0] = 0.0f;
        main_work.obi_pos[1] = DMD_STGSLCT_OBI_MOVE_START_POS;
    }



    // ==========================================================================
    // dmStgSlctSetHiScore
    /*!
        ハイスコア設定処理
     */
    // ==========================================================================
    private static void dmStgSlctSetHiScore(DMS_STGSLCT_MAIN_WORK main_work)
    {
        UNREFERENCED_PARAMETER(main_work);

        // セーブデータ内のハイスコアを設定


    }



    // ==========================================================================
    // dmStgSlctSetClearInfo
    /*!
        クリア情報設定処理
     */
    // ==========================================================================
    private static void dmStgSlctSetClearInfo(DMS_STGSLCT_MAIN_WORK main_work)
    {
        uint no_active = 0;
        uint j = 0;

        // 通常ステージデータインスタンス作成
        //gs.backup.SStage & data
        //    = gs.backup.SStage.CreateInstance();
        var data = SStage.CreateInstance();

        for (int i = 0; i <= GSD_MAIN_STAGE_ID_FINAL_1; i++)
        {
            // 通常ソニックのハイスコア取得
            main_work.n_sonic_hi_score[i] = (int)data[i].GetHighScore(false);

            // スーパーソニックのハイスコア取得
            main_work.s_sonic_hi_score[i] = (int)data[i].GetHighScore(true);

            // ２種のソニックのハイスコアがどちらも初期値でない場合
            if (main_work.n_sonic_hi_score[i] != DMD_STGSLCT_INIT_SCORE_NUM
                && main_work.s_sonic_hi_score[i] != DMD_STGSLCT_INIT_SCORE_NUM)
            {
                // ハイスコアが高い方を設定
                if (main_work.n_sonic_hi_score[i] >= main_work.s_sonic_hi_score[i])
                {
                    // 通常ソニックの方が高い場合(同値含む)
                    main_work.hi_score[i] = main_work.n_sonic_hi_score[i];
                }
                else
                {
                    // スーパーソニックの方が高い場合
                    main_work.hi_score[i] = main_work.s_sonic_hi_score[i];
                }
            }

            else
            {
                if (main_work.n_sonic_hi_score[i] == DMD_STGSLCT_INIT_SCORE_NUM
                    && main_work.s_sonic_hi_score[i] == DMD_STGSLCT_INIT_SCORE_NUM)
                {
                    // 初期値設定(どちらも初期値のため、通常ソニックの値を設定)
                    main_work.hi_score[i] = main_work.n_sonic_hi_score[i];
                }

                else if (main_work.n_sonic_hi_score[i] == DMD_STGSLCT_INIT_SCORE_NUM)
                {
                    // 通常ソニックが初期値のため、スパソニのスコアを設定
                    main_work.hi_score[i] = main_work.s_sonic_hi_score[i];
                }
                else if (main_work.s_sonic_hi_score[i] == DMD_STGSLCT_INIT_SCORE_NUM)
                {
                    // スパソニが初期値のため、通常ソニックのスコアを設定
                    main_work.hi_score[i] = main_work.n_sonic_hi_score[i];
                }
                else
                {
                    MTM_ASSERT(0);
                    // ここに来る場合はありえないためASSERTをかけ、保険で通常ソニックの値を設定
                    main_work.hi_score[i] = main_work.n_sonic_hi_score[i];
                }
            }

            if (main_work.hi_score[i] > DMD_STGSLCT_INIT_SCORE_NUM)
            {
                main_work.hi_score[i] = DMD_STGSLCT_INIT_SCORE_NUM - 1;
            }


            // 通常ソニックのレコード取得
            main_work.n_sonic_record_time[i] = (int)data[i].GetFastTime(false);

            // スーパーソニックのレコード取得
            main_work.s_sonic_record_time[i] = (int)data[i].GetFastTime(true);

            // ２種のソニックのレコードタイムがどちらも初期値でない場合
            if (main_work.n_sonic_record_time[i] != DMD_STGSLCT_INIT_RECORD_TIME_NUM
                && main_work.s_sonic_record_time[i] != DMD_STGSLCT_INIT_RECORD_TIME_NUM)
            {
                // ハイスコアが高い方を設定
                if (main_work.n_sonic_record_time[i] <= main_work.s_sonic_record_time[i])
                {
                    // 通常ソニックの方が高い場合(同値含む)
                    main_work.record_time[i] = main_work.n_sonic_record_time[i];
                }
                else
                {
                    // スーパーソニックの方が高い場合
                    main_work.record_time[i] = main_work.s_sonic_record_time[i];
                }
            }

            else
            {
                if (main_work.n_sonic_record_time[i] == DMD_STGSLCT_INIT_RECORD_TIME_NUM
                    && main_work.s_sonic_record_time[i] == DMD_STGSLCT_INIT_RECORD_TIME_NUM)
                {
                    // 初期値設定(どちらも初期値のため、通常ソニックの値を設定)
                    main_work.record_time[i] = main_work.n_sonic_record_time[i];
                }

                else if (main_work.n_sonic_record_time[i] == DMD_STGSLCT_INIT_RECORD_TIME_NUM)
                {
                    // 通常ソニックが初期値のため、スパソニのスコアを設定
                    main_work.record_time[i] = main_work.s_sonic_record_time[i];
                }
                else if (main_work.s_sonic_record_time[i] == DMD_STGSLCT_INIT_RECORD_TIME_NUM)
                {
                    // スパソニが初期値のため、通常ソニックのスコアを設定
                    main_work.record_time[i] = main_work.n_sonic_record_time[i];
                }
                else
                {
                    MTM_ASSERT(0);
                    // ここに来る場合はありえないためASSERTをかけ、保険で通常ソニックの値を設定
                    main_work.record_time[i] = main_work.n_sonic_record_time[i];
                }
            }

            // 通常ステージのレコードタイム取得(通常ソニック)
            //		main_work.record_time[i] = (int)data[i].GetFastTime(false);

            if (main_work.record_time[i] > 36000)
            {
                main_work.record_time[i] = 35999;
            }
        }


        // スペシャルステージデータインスタンス作成
        var spe_data = SSpecial.CreateInstance();

        for (int i = 0; i < 7; i++)
        {
            // スペシャルステージのハイスコア取得
            main_work.hi_score[(int)(i + DME_STGSLCT_STAGE_S_1)] = (int)spe_data[i].GetHighScore();

            if (main_work.hi_score[(int)(i + DME_STGSLCT_STAGE_S_1)] > DMD_STGSLCT_INIT_SCORE_NUM)
            {
                main_work.hi_score[(int)(i + DME_STGSLCT_STAGE_S_1)] = DMD_STGSLCT_INIT_SCORE_NUM - 1;
            }

            // スペシャルステージのレコードタイム取得
            main_work.record_time[(int)(i + DME_STGSLCT_STAGE_S_1)]
                = (int)spe_data[i].GetFastTime();

            if (main_work.record_time[i] > DMD_STGSLCT_INIT_RECORD_TIME_NUM)
            {
                main_work.record_time[i] = DMD_STGSLCT_INIT_RECORD_TIME_NUM - 1;
            }
        }


        // 各ステージのクリア設定処理(セーブデータから)
        for (int i = 0; i < (int)DME_STGSLCT_STAGE_NUM; i++)
        {
            // 仮の設定処理
            if (main_work.hi_score[i] == DMD_STGSLCT_INIT_SCORE_NUM
                && main_work.record_time[i] == DMD_STGSLCT_INIT_RECORD_TIME_NUM)
            {
                main_work.is_clear_stage[i] = 0;
            }
            else
            {
                main_work.is_clear_stage[i] = 1;
            }
        }


        for (int i = 0; i < (int)DME_STGSLCT_STAGE_NUM; i++)
        {

            // スペシャルステージ
            if (i > (int)DME_STGSLCT_STAGE_F_1)
            {
                if (0 == main_work.is_clear_stage[i])
                {
                    // スペシャルステージを選択不可にする
                    main_work.is_clear_stage[i] = -1;
                }
            }
            if (i == 17)
            {
                main_work.is_clear_stage[i] = 1;
            }

            // FINALステージ
            else if (i == (int)DME_STGSLCT_STAGE_F_1)
            {
                for (j = (uint)DME_STGSLCT_STAGE_1_B; j < (uint)DME_STGSLCT_STAGE_F_1; j += 4)
                {
                    if (main_work.is_clear_stage[j] != 1)
                    {
                        no_active = 1;
                    }
                }

                if (no_active != 0)
                {
                    // FINALステージを選択不可にする(表示させない)
                    main_work.is_clear_stage[i] = -1;
                }
            }
            // BOSSステージ
            else if (i < (int)DME_STGSLCT_STAGE_F_1 && ((i + 1) % 4 == 0))
            {
                for (j = 0; j < 3; j++)
                {
                    if (0 == main_work.is_clear_stage[i - 3 + j])
                    {
                        no_active = 1;
                    }
                }

                if (no_active != 0)
                {
                    main_work.is_clear_stage[i] = -1;
                }
            }

            no_active = 0;
        }


        if (main_work.is_clear_stage[(int)DME_STGSLCT_STAGE_1_B] == 1
            && main_work.is_clear_stage[(int)DME_STGSLCT_STAGE_2_B] == 1
            && main_work.is_clear_stage[(int)DME_STGSLCT_STAGE_3_B] == 1
            && main_work.is_clear_stage[(int)DME_STGSLCT_STAGE_4_B] == 1)
        {
            main_work.is_final_open |= DMD_STGSLCT_ZONE_FINAL_OPEN;
        }

        // 残機数設定用にシステムデータ取得
        //	gs.backup.SSystem &sys_data = gs.backup.SSystem.CreateInstance();

        // 残機数設定
        main_work.player_stock = g_gs_main_sys_info.rest_player_num;//sys_data.GetPlayerStock();

        // カオスエメラルド取得設定
        for (int i = 0; i < 7; i++)
        {
            if (spe_data[i].IsGetEmerald())
            {
                // カオスエメラルドを一つでも所持していたらスペシャルゾーン解放
                main_work.is_final_open |= DMD_STGSLCT_ZONE_SPECIAL_OPEN;

                main_work.get_emerald |= 1u << i;

                main_work.eme_stage_no[i] = dm_stgslct_eme_get_act_no_tbl[(int)spe_data[i].GetEmeraldStage()];
            }
            else
            {
                main_work.is_clear_stage[(int)(DME_STGSLCT_STAGE_S_1 + i)] = -1;
            }
        }
    }



    // ==========================================================================
    // dmStgSlctSetAnnounceMsg
    /*!
        画面遷移時に表示するアナウンス設定処理
     */
    // ==========================================================================
    private static void dmStgSlctSetAnnounceMsg(DMS_STGSLCT_MAIN_WORK main_work)
    {
        UNREFERENCED_PARAMETER(main_work);

        // アナウンス表示判定用にシステムデータ取得
        gs.backup.SSystem sys_data = gs.backup.SSystem.CreateInstance();

        // ステセレオープンアナウンス
        if (sys_data.IsAnnounce(gs.backup.SSystem.EAnnounce.OpenZoneSelect) == false
            && main_work.is_clear_stage[0] == 1)
        {
            main_work.announce_flag |= 1u << (int)DME_STGSLCT_WIN_1_1_CLEAR;

            sys_data.SetAnnounce(gs.backup.SSystem.EAnnounce.OpenZoneSelect, true);
        }

        // BOSS1オープンアナウンス
        if (sys_data.IsAnnounce(gs.backup.SSystem.EAnnounce.OpenZone1Boss) == false
            && main_work.is_clear_stage[0] == 1
            && main_work.is_clear_stage[1] == 1
            && main_work.is_clear_stage[2] == 1)
        {
            main_work.announce_flag |= 1u << (int)DME_STGSLCT_WIN_BOSS1_CAN_PLAY;

            sys_data.SetAnnounce(gs.backup.SSystem.EAnnounce.OpenZone1Boss, true);
        }

        // BOSS2オープンアナウンス
        if (sys_data.IsAnnounce(gs.backup.SSystem.EAnnounce.OpenZone2Boss) == false
            && main_work.is_clear_stage[4] == 1
            && main_work.is_clear_stage[5] == 1
            && main_work.is_clear_stage[6] == 1)
        {
            main_work.announce_flag |= 1u << (int)DME_STGSLCT_WIN_BOSS2_CAN_PLAY;

            sys_data.SetAnnounce(gs.backup.SSystem.EAnnounce.OpenZone2Boss, true);
        }

        // BOSS3オープンアナウンス
        if (sys_data.IsAnnounce(gs.backup.SSystem.EAnnounce.OpenZone3Boss) == false
            && main_work.is_clear_stage[8] == 1
            && main_work.is_clear_stage[9] == 1
            && main_work.is_clear_stage[10] == 1)
        {
            main_work.announce_flag |= 1u << (int)DME_STGSLCT_WIN_BOSS3_CAN_PLAY;

            sys_data.SetAnnounce(gs.backup.SSystem.EAnnounce.OpenZone3Boss, true);
        }

        // BOSS4オープンアナウンス
        if (sys_data.IsAnnounce(gs.backup.SSystem.EAnnounce.OpenZone4Boss) == false
            && main_work.is_clear_stage[12] == 1
            && main_work.is_clear_stage[13] == 1
            && main_work.is_clear_stage[14] == 1)
        {
            main_work.announce_flag |= 1u << (int)DME_STGSLCT_WIN_BOSS4_CAN_PLAY;

            sys_data.SetAnnounce(gs.backup.SSystem.EAnnounce.OpenZone4Boss, true);
        }

        // FINALステージオープンアナウンス
        if (sys_data.IsAnnounce(gs.backup.SSystem.EAnnounce.OpenFinalZone) == false
            && main_work.is_clear_stage[3] == 1      // 全てのボスステージクリアならば
            && main_work.is_clear_stage[7] == 1
            && main_work.is_clear_stage[11] == 1
            && main_work.is_clear_stage[15] == 1)
        {
            main_work.announce_flag |= 1u << (int)DME_STGSLCT_WIN_FINAL_CAN_SLCT;

            sys_data.SetAnnounce(gs.backup.SSystem.EAnnounce.OpenFinalZone, true);
        }

        // スーパーソニックオープンアナウンス
        if (sys_data.IsAnnounce(gs.backup.SSystem.EAnnounce.OpenSuperSonic) == false
            && main_work.is_clear_stage[23] == 1)
        {
            main_work.announce_flag |= 1u << (int)DME_STGSLCT_WIN_SSONIC_SLCT;

            sys_data.SetAnnounce(gs.backup.SSystem.EAnnounce.OpenSuperSonic, true);
        }

        // スペステオープンアナウンス
        if (sys_data.IsAnnounce(gs.backup.SSystem.EAnnounce.OpenSpecialStage) == false
            && (main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN) != 0)
        {
            main_work.announce_flag |= 1u << (int)DME_STGSLCT_WIN_SPESTE_CAN_SLCT;

            sys_data.SetAnnounce(gs.backup.SSystem.EAnnounce.OpenSpecialStage, true);
        }

    }



    // ==========================================================================
    // dmStgSlctProcMain
    /*!
        ステージ選択画面メインプロシージャ処理
     */
    // ==========================================================================
    private static void dmStgSlctProcMain(MTS_TASK_TCB tcb)
    {
        DMS_STGSLCT_MAIN_WORK main_work;

        // ワーク取得
        main_work = (DMS_STGSLCT_MAIN_WORK)mtTaskGetTcbWork(tcb);

        // 終了処理
        if ((main_work.flag & DMD_STGSLCT_FLAG_EXIT) != 0)
        {
            // タスククリア
            mtTaskClearTcb(tcb);

            // イベント遷移用設定
            dmStgSlctSetNextEvt(main_work);
        }

        // システム関連処理(サインアウト時はタイトルへ戻す)
        if ((main_work.flag & DMD_STGSLCT_FLAG_SIGN_OUT_EXIT) == DMD_STGSLCT_FLAG_SIGN_OUT_EXIT
            && !AoAccountIsCurrentEnable())
        {

            main_work.proc_menu_update = dmStgSlctProcFadeOut;

            // サインアウト終了フラグOFF
            main_work.flag &= ~DMD_STGSLCT_FLAG_SIGN_OUT_EXIT;
            main_work.next_evt = (int)DME_STGSLCT_NEXT_EVT_TITLE;

            // フェード終了		※問題あれば有効にする
            //		IzFadeExit();

            // フェードアウト開始
            IzFadeInitEasy(IZE_FADE_SET_TYPE_TAKEOEVER
                           , IZE_FADE_TYPE_BLACK_FADEOUT
                           , DMD_STGSLCT_FADEOUT_TIME
                           );

            // BGMフェードアウト開始
            DmSndBgmPlayerExit();
            main_work.flag |= DMD_STGSLCT_FLAG_DEMO_SND_END;

            // ウインドウ遷移関連設定
            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_DECIDE);
            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_CANCEL);
            main_work.proc_input = null;
            main_work.proc_win_input = null;
            main_work.win_timer = 0;
            main_work.win_cur_slct = 0;
            main_work.win_mode = (int)DME_STGSLCT_WIN_STG_SLCT;
#if _IPHONE
            main_work.win_is_disp_cover = false;
#endif //_IPHONE
        }

        // BGフェード処理
        dmStgSlctSetBgFadeEfct(main_work);

        // ウインドウ処理用プロシージャ
        if (main_work.proc_win_update != null)
        {
            main_work.proc_win_update(main_work);
        }

        // メニュー処理用プロシージャ
        if (main_work.proc_menu_update != null)
        {
            main_work.proc_menu_update(main_work);
        }

        // 描画設定プロシージャ
        if (main_work.proc_draw != null)
        {
            main_work.proc_draw(main_work);
        }

    }


    // ==========================================================================
    // dmStgSlctDest
    /*!
        ステージ選択画面終了処理
     */
    // ==========================================================================
    private static void dmStgSlctDest(MTS_TASK_TCB tcb)
    {
        UNREFERENCED_PARAMETER(tcb);

    }



    // ==========================================================================
    // dmStgSlctSetNextEvt
    /*!
        次のイベント遷移設定処理
     */
    // ==========================================================================
    private static void dmStgSlctSetNextEvt(DMS_STGSLCT_MAIN_WORK main_work)
    {
        short set_next_evt = 0;
        ushort set_play_act = 0;

        // ステージID変換
        set_play_act = (ushort)main_work.cur_stage;

        // 遷移先別設定処理
        switch (main_work.next_evt)
        {
            case (int)DME_STGSLCT_NEXT_EVT_MAINGAME:
                // ステージID設定
                g_gs_main_sys_info.stage_id = set_play_act;
                g_gs_main_sys_info.prev_stage_id = 0xffff;

                // 初期状態のキャラ設定(ノーマルソニック)
                g_gs_main_sys_info.char_id[0] = GSD_CHAR_ID_SONIC;

                // ゲームモード設定(スコアかタイムか)
                g_gs_main_sys_info.game_mode = (int)main_work.cur_game_mode;

                // 最後のスペステがクリアしている場合、スパソニフラグON
                if (main_work.is_clear_stage[23] == 1)
                {
                    g_gs_main_sys_info.game_flag |= GSD_MAINSYS_GAME_FLAG_7_CHAOS_EMERALD;
                }
                else
                {
                    g_gs_main_sys_info.game_flag &= ~GSD_MAINSYS_GAME_FLAG_7_CHAOS_EMERALD;
                }

                // コントロール設定
#if !_IPHONE
                g_gs_main_sys_info.game_flag |= GSD_MAINSYS_GAME_FLAG_INPUT_CLASSIC;
#endif //!_IPHONE

                // スペステフラグOFF(ゲームへ遷移する際は必ずフラグOFF)
                g_gs_main_sys_info.game_flag &= ~GSD_MAINSYS_GAME_FLAG_SPECIAL_STAGE;

                // ゲーム開始前初期化
                GmMainGSInit();

                break;

            case (int)DME_STGSLCT_NEXT_EVT_SPESTE:
                // ステージID設定
                g_gs_main_sys_info.stage_id = (ushort)(set_play_act + 4);
                g_gs_main_sys_info.prev_stage_id = 0xffff;

                // 最後のスペステがクリアしている場合、スパソニフラグON
                if (main_work.is_clear_stage[23] == 1)
                {
                    g_gs_main_sys_info.game_flag |= GSD_MAINSYS_GAME_FLAG_7_CHAOS_EMERALD;
                }
                else
                {
                    g_gs_main_sys_info.game_flag &= ~GSD_MAINSYS_GAME_FLAG_7_CHAOS_EMERALD;
                }

                // ゲームモード設定(スコアかタイムか)
                g_gs_main_sys_info.game_mode = (int)main_work.cur_game_mode;

                // コントロール設定
#if !_IPHONE
                g_gs_main_sys_info.game_flag |= GSD_MAINSYS_GAME_FLAG_INPUT_CLASSIC;
#endif //!_IPHONE

                // スペステフラグOFF(ゲームへ遷移する際は必ずフラグOFF)
                g_gs_main_sys_info.game_flag &= ~GSD_MAINSYS_GAME_FLAG_SPECIAL_STAGE;

                break;

            case (int)DME_STGSLCT_NEXT_EVT_RANKING:
                dm_stgslct_is_stage_start = true;

                // ここでステージIDを一旦設定(指定したステージのランキングを開くため)
                if (main_work.cur_zone == (uint)DME_STGSLCT_ZONE_TYPE_SPE)
                {
                    g_gs_main_sys_info.stage_id = (ushort)(set_play_act + 4);
                    g_gs_main_sys_info.prev_stage_id = 0xffff;
                }
                else
                {
                    g_gs_main_sys_info.stage_id = set_play_act;
                    g_gs_main_sys_info.prev_stage_id = 0xffff;
                }

                // ゲームモード設定
                if (main_work.cur_game_mode == (uint)DME_STGSLCT_PLAY_MODE_NORMAL)
                {
                    g_gs_main_sys_info.game_mode = (int)GSD_GAME_MODE_STORY;
                }
                else
                {
                    g_gs_main_sys_info.game_mode = (int)GSD_GAME_MODE_TIME_ATTACK;
                }

                break;

            case (int)DME_STGSLCT_NEXT_EVT_MAINMENU:
                break;

            case (int)DME_STGSLCT_NEXT_EVT_TITLE:
                break;

            default:
                break;
        }

        // イベント遷移設定
        set_next_evt = (short)main_work.next_evt;

        SyDecideEvtCase(set_next_evt);
        SyChangeNextEvt();
    }



    // ==========================================================================
    // dmStgSlctLoadFontData
    /*!
        フォントデータ読み込みリクエスト処理
     */
    // ==========================================================================
    private static void dmStgSlctLoadFontData(DMS_STGSLCT_MAIN_WORK main_work)
    {
        // gsFont構築
        GsFontBuild();

        main_work.proc_menu_update = dmStgSlctIsLoadFontData;
    }



    // ==========================================================================
    // dmStgSlctIsLoadFontData
    /*!
        フォントデータ読み込み終了チェック処理
     */
    // ==========================================================================
    private static void dmStgSlctIsLoadFontData(DMS_STGSLCT_MAIN_WORK main_work)
    {
        // gsFont構築
        if (GsFontIsBuilded())
        {
            main_work.proc_menu_update = dmStgSlctLoadRequest;

            return;
        }
    }



    // ==========================================================================
    // dmStgSlctLoadRequest
    /*!
        ファイル読み込みリクエスト処理
     */
    // ==========================================================================
    private static void dmStgSlctLoadRequest(DMS_STGSLCT_MAIN_WORK main_work)
    {
        // ファイル読み込み開始
        main_work.arc_amb_fs[0] = amFsReadBackground(GSS_BASE_PATH + "DEMO/STGSLCT/D_STGSLCT.AMB");
        main_work.arc_amb_fs[1] = amFsReadBackground(dm_stgslct_main_lng_amb_name_tbl[GsEnvGetLanguage()]);

        // メニュー共通データ読み込み
        for (int i = 0; i < 4; i++)
        {
            main_work.arc_cmn_amb_fs[i] = amFsReadBackground(dm_stgslct_menu_cmn_amb_name_tbl[i]);
        }

        main_work.arc_cmn_amb_fs[4] = amFsReadBackground(dm_stgslct_menu_cmn_lng_amb_name_tbl[GsEnvGetLanguage()]);

        // 次へ遷移
        main_work.proc_menu_update = dmStgSlctProcLoadWait;
    }


    // ==========================================================================
    // dmStgSlctProcLoadWait
    /*!
        ファイル読み込み待ち処理
     */
    // ==========================================================================
    private static void dmStgSlctProcLoadWait(DMS_STGSLCT_MAIN_WORK main_work)
    {
        // ファイル読み込み完了待ち
        if (dmStgSlctIsDataLoad(main_work))
        {       // ファイル読込み完了チェック関数にする

            // ファイル取得
            for (int i = 0; i < 2; i++)
            {
                main_work.arc_amb[i] = readAMBFile(main_work.arc_amb_fs[i]);
                main_work.arc_amb_fs[i] = null;

                //amBindConv((u8*)main_work.arc_amb[i]);

                // AMBファイルロード
                main_work.ama[i] = readAMAFile(amBindGet(main_work.arc_amb[i], 0));
                main_work.amb[i] = readAMBFile(amBindGet(main_work.arc_amb[i], 1, out var sPath));
                main_work.amb[i].dir = sPath;

                // リクエストクリア
                amFsClearRequest(main_work.arc_amb_fs[i]);
                main_work.arc_amb_fs[i] = null;

                // アドレス変換
                //amConvertAddress(main_work.ama[i]);
                //amConvertAddress(main_work.amb[i]);

                // テクスチャ構築開始
                AoTexBuild(main_work.tex[i], main_work.amb[i]);
                AoTexLoad(main_work.tex[i]);
            }

            //
            GsFontBuild();

            // サウンド構築
            DmSndBgmPlayerInit();
            //		DmSoundBuild();

            // 次へ遷移
            main_work.proc_menu_update = dmStgSlctProcLoadWait2;
        }
    }



    // ==========================================================================
    // dmStgSlctProcLoadWait2
    /*!
        ファイル読み込み待ち処理		◆暫定対応
     */
    // ==========================================================================
    private static void dmStgSlctProcLoadWait2(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (dmStgSlctIsTexLoad(main_work)
            && DmSndBgmPlayerIsSndSysBuild())
        {
            // メニュー共通データ
            for (int i = 0; i < 5; i++)
            {
                main_work.arc_cmn_amb[i] = readAMBFile(main_work.arc_cmn_amb_fs[i]);
                main_work.arc_cmn_amb_fs[i] = null;

                //amBindConv((u8*)main_work.arc_cmn_amb[i]);

                // AMBファイルロード
                main_work.cmn_ama[i] = readAMAFile(amBindGet(main_work.arc_cmn_amb[i], 0));
                main_work.cmn_amb[i] = readAMBFile(amBindGet(main_work.arc_cmn_amb[i], 1, out var sPath));
                main_work.cmn_amb[i].dir = sPath;

                // リクエストクリア
                amFsClearRequest(main_work.arc_cmn_amb_fs[i]);
                main_work.arc_cmn_amb_fs[i] = null;

                // アドレス変換
                //amConvertAddress(main_work.cmn_ama[i]);
                //amConvertAddress(main_work.cmn_amb[i]);

                // テクスチャ構築開始
                AoTexBuild(main_work.cmn_tex[i], main_work.cmn_amb[i]);
                AoTexLoad(main_work.cmn_tex[i]);
            }

            // 次へ遷移
            main_work.proc_menu_update = dmStgSlctProcTexBuildWait;
        }
    }



    // ==========================================================================
    // dmStgSlctProcTexBuildWait
    /*!
        テクスチャ構築待ち処理
     */
    // ==========================================================================
    private static void dmStgSlctProcTexBuildWait(DMS_STGSLCT_MAIN_WORK main_work)
    {
        // テクスチャ構築完了判定
        if (dmStgSlctIsTexLoad2(main_work)
            && DmSndBgmPlayerIsSndSysBuild())
        {

            // 次へ遷移
            main_work.proc_menu_update = dmStgSlctProcCheckLoadingEnd;

            // ローディング終了設定
            //		DmLoadingSetLoadComplete();

            // ここに仮のセーブ待ち処理追加
            DmSaveMenuStart(true, false);

            //		DmSoundInit();
        }
    }



    // ==========================================================================
    // dmStgSlctProcCheckLoadingEnd
    /*!
        ローディング終了待ち処理
     */
    // ==========================================================================
    private static void dmStgSlctProcCheckLoadingEnd(DMS_STGSLCT_MAIN_WORK main_work)
    {
        // テクスチャ構築完了判定
        if (DmSaveIsExit())
        {
            // 次へ遷移
            main_work.proc_menu_update = dmStgSlctProcCreateAct;

            DmSndBgmPlayerPlayBgm(DME_SND_BGM_PLAYER_IDX_MENU);
            //		DmSoundPlayMenuBGM(DME_SOUND_BGM_IDX_MENU
            //						   , DMD_STGSLCT_BGM_FADEIN_TIME);

            // サインアウト終了フラグON
            main_work.flag |= unchecked((uint)DMD_STGSLCT_FLAG_SIGN_OUT_EXIT);
        }
    }



    // ==========================================================================
    // dmStgSlctProcCreateAct
    /*!
        アクション生成処理
     */
    // ==========================================================================
    private static void dmStgSlctProcCreateAct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        // ファイル選別
        A2S_AMA_HEADER ama;
        AOS_TEXTURE tex;

        // アクション構築
        for (uint i = 0; i < (uint)ACT_NUM; ++i)
        {
            if (i >= (uint)ACT_TEX_FIX_BACK)
            {
                ama = main_work.cmn_ama[4];
                tex = main_work.cmn_tex[4];
            }
#if !_IPHONE
            else if (i >=  (uint)ACT_WIN_LINE)
            {
                ama = main_work.cmn_ama[3];
                tex = main_work.cmn_tex[3];
            }
            else if (i >=  (uint)ACT_OBI_C)
            {
                ama = main_work.cmn_ama[2];
                tex = main_work.cmn_tex[2];
            }
#else //!_IPHONE
            else if ((uint)ACT_BACK_BTN01_L <= i && i <= (uint)ACT_BACK_BTN01_R)
            {
                ama = main_work.cmn_ama[3];
                tex = main_work.cmn_tex[3];
            }
            else if ((uint)ACT_YES_BTN_L <= i && i <= (uint)ACT_NO_BTN_R)
            {
                ama = main_work.cmn_ama[3];
                tex = main_work.cmn_tex[3];
            }
#endif //!_IPHONE
            else if (i >= (uint)ACT_BTN_CANCEL1)
            {
                ama = main_work.cmn_ama[1];
                tex = main_work.cmn_tex[1];
            }
            else if (i >= (uint)ACT_WAVE_BG)
            {
                ama = main_work.cmn_ama[0];
                tex = main_work.cmn_tex[0];
            }

            else if (i >= (uint)ACT_TEX_BIG_TIME)
            {
                ama = main_work.ama[(int)DME_STGSLCT_DATA_TYPE_LANG_DATA];
                tex = main_work.tex[(int)DME_STGSLCT_DATA_TYPE_LANG_DATA];
            }
#if !_IPHONE
            else if ((i >=  (uint)ACT_TEX_MODE_TIME && i <=  (uint)ACT_TEX_MENU)
                     || (i >=  (uint)ACT_TEX_ZONE_UP_S && i <=  (uint)ACT_TEX_OBI2)
#else //!_IPHONE
            else if (i is (uint)ACT_TEX_ZONE_UP_S
#endif //!_IPHONE
                         or >= (uint)ACT_TAB_TEX_SCORE and <= (uint)ACT_TAB_TEX_SPE_STAGE or (uint)ACT_TAB_ZONE_TEXT_S
                         or (uint)ACT_TAB_TEXT
                         or (uint)ACT_TAB_MESS)
            {
                ama = main_work.ama[(int)DME_STGSLCT_DATA_TYPE_LANG_DATA];
                tex = main_work.tex[(int)DME_STGSLCT_DATA_TYPE_LANG_DATA];
            }
            else
            {
                ama = main_work.ama[(int)DME_STGSLCT_DATA_TYPE_CMN_DATA];
                tex = main_work.tex[(int)DME_STGSLCT_DATA_TYPE_CMN_DATA];
            }

            // 構築
            AoActSetTexture(AoTexGetTexList(tex));
            main_work.act[i] = AoActCreate(ama, g_dm_act_id_tbl[i]);
        }

#if _IPHONE
        {
            AoActUpdate(main_work.act[(int)ACT_TAB_STATE_MOVE], 0.0f);
            main_work.act_tab_state_move_base_pos[AMD_X] = main_work.act[(int)ACT_TAB_STATE_MOVE].sprite.center_x;
            main_work.act_tab_state_move_base_pos[AMD_Y] = main_work.act[(int)ACT_TAB_STATE_MOVE].sprite.center_y;
        }
#endif //_IPHONE
#if _IPHONE    //当たり判定構築          
        {
            int index1 = 0;
            for (int index2 = arrayof(main_work.trg_zone); index1 < index2; ++index1)
                main_work.trg_zone[index1].Create(main_work.act[37]);
            int index3 = 0;
            for (int index2 = arrayof(main_work.trg_act); index3 < index2; ++index3)
                main_work.trg_act[index3].Create(main_work.act[58]);
            int index4 = 0;
            for (int index2 = arrayof(main_work.trg_act_tab); index4 < index2; ++index4)
                main_work.trg_act_tab[index4].Create(main_work.act[43 + index4]);
            int index5 = 0;
            for (int index2 = arrayof(main_work.trg_act_lr); index5 < index2; ++index5)
            {
                CTrgAoAction ctrgAoAction = main_work.trg_act_lr[index5];
                int index6 = index5 == 0 ? 49 : 50;
                ctrgAoAction.Create(main_work.act[index6]);
            }

            CTrgFlick trgActMove = main_work.trg_act_move;
            trgActMove.Create(50, 90, (int)AMD_SCREEN_2D_WIDTH - 100, 258);
            trgActMove.SetMoveThreshold(4);
            int index7 = 0;
            for (int index2 = arrayof(main_work.trg_mode); index7 < index2; ++index7)
            {
                CTrgAoAction ctrgAoAction = main_work.trg_mode[index7];
                int index6 = index7 == 0 ? 52 : 55;
                ctrgAoAction.Create(main_work.act[index6]);
            }

            main_work.trg_cancel.Create(main_work.act[105]);
            int index8 = 0;
            for (int index2 = arrayof(main_work.trg_answer); index8 < index2; ++index8)
            {
                CTrgAoAction ctrgAoAction = main_work.trg_answer[index8];
                int index6 = index8 == 0 ? 110 : 107;
                ctrgAoAction.Create(main_work.act[index6]);
            }
        }
#endif //_IPHONE	//当たり判定構築

        // イベント遷移
        main_work.proc_menu_update = dmStgSlctProcSetDispEfctData;
    }



    // ==========================================================================
    // dmStgSlctProcSetDispEfctData
    /*!
        表示演出周り設定処理
     */
    // ==========================================================================
    private static void dmStgSlctProcSetDispEfctData(DMS_STGSLCT_MAIN_WORK main_work)
    {
        uint act_num_start = 0;
        uint act_num_end = 0;

        act_num_start = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
        act_num_end = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] + act_num_start;

        // もし何かの解放メッセージ表示がある場合、強制的にZONE選択にする

        // もし何かの解放メッセージ表示がある場合、白フラッシュ演出が出るようにする
        // ※その際、ウインドウ表示開始は白フラッシュが終ってからにする
        if (((main_work.announce_flag & 1 << (int)DME_STGSLCT_WIN_1_1_CLEAR)) != 0
            || (main_work.announce_flag & 1 << (int)DME_STGSLCT_WIN_SPESTE_CAN_SLCT) != 0
            || (main_work.announce_flag & 1 << (int)DME_STGSLCT_WIN_FINAL_CAN_SLCT) != 0)
        {
            // 白フラッシュフラグON
            main_work.flag |= DMD_STGSLCT_FLAG_WHITE_FLASH_EFCT;
        }

        // 


        // 最初に表示する状態を設定
        if (dm_stgslct_is_stage_start
            && 0 == (main_work.flag & DMD_STGSLCT_FLAG_WHITE_FLASH_EFCT))
        {
            // ACTセレクトの描画プロシージャ設定
            main_work.proc_draw = dmStgSlctStageSelectDraw;
            main_work.state = (int)DME_STGSLCT_MODE_STATE_ACT_SLCT;
        }
        else
        {
            // ZONEセレクトの描画プロシージャ設定
            main_work.proc_draw = dmStgSlctZoneSelectDraw;
            main_work.state = (int)DME_STGSLCT_MODE_STATE_ZONE_SLCT;
#if false
            main_work.cur_zone = main_work.cur_zone = uint.MaxValue;
#endif //_IPHONE
        }

        // ※この初期表示位置については、初期状態がZONE選択かACT選択かで分かれる
        if (main_work.state == (int)DME_STGSLCT_MODE_STATE_ZONE_SLCT)
        {
            if (main_work.is_final_open == (int)DMD_STGSLCT_ZONE_ALL_OPEN)
            {
                // ZONEテーブルの初期表示位置設定
                for (uint i = 0; i < (int)DME_STGSLCT_ZONE_TYPE_NUM; i++)
                {
                    main_work.zone_pos[i][0] = dm_stgslct_a_zone_disp_pos_tbl[i][0];
                    main_work.zone_pos[i][1] = dm_stgslct_a_zone_disp_pos_tbl[i][1];
                }
            }

            else if ((main_work.is_final_open & (int)DMD_STGSLCT_ZONE_SPECIAL_OPEN) != 0)
            {
                // ZONEテーブルの初期表示位置設定
                for (uint i = 0; i < (int)DME_STGSLCT_ZONE_TYPE_NUM; i++)
                {
                    main_work.zone_pos[i][0] = dm_stgslct_s_zone_disp_pos_tbl[i][0];
                    main_work.zone_pos[i][1] = dm_stgslct_s_zone_disp_pos_tbl[i][1];
                }
            }

            else if ((main_work.is_final_open & (int)DMD_STGSLCT_ZONE_FINAL_OPEN) != 0)
            {
                // ZONEテーブルの初期表示位置設定
                for (uint i = 0; i < (int)DME_STGSLCT_ZONE_TYPE_NUM; i++)
                {
                    main_work.zone_pos[i][0] = dm_stgslct_f_zone_disp_pos_tbl[i][0];
                    main_work.zone_pos[i][1] = dm_stgslct_f_zone_disp_pos_tbl[i][1];
                }
            }
            else
            {
                // ZONEテーブルの初期表示位置設定
                for (uint i = 0; i < (int)DME_STGSLCT_ZONE_TYPE_NUM; i++)
                {
                    main_work.zone_pos[i][0] = dm_stgslct_n_zone_disp_pos_tbl[i][0];
                    main_work.zone_pos[i][1] = dm_stgslct_n_zone_disp_pos_tbl[i][1];
                }
            }

            // ACTテーブルの外側表示位置設定
            for (uint j = 0; j < (int)DME_STGSLCT_STAGE_NUM; j++)
            {
                main_work.act_top_pos_x[j] = DMD_STGSLCT_STAGE_TAB_NODISP_POS_X;
                main_work.act_top_pos_y[j] = dm_stgslct_act_disp_y_pos_tbl[j];
                //											+ (float)(dm_stgslct_act_disp_pos_y_tbl[j] * DMD_STGSLCT_ACT_TABLE_DIST_Y);
            }

            // モードテキスト、エメラルドテーブルの初期表示位置設定
            main_work.mode_tex_pos_y = DMD_STGSLCT_DOWN_ACT_NODISP_POS;
            main_work.chaos_eme_pos_y = DMD_STGSLCT_DOWN_ACT_DISP_POS;

            // ZONE選択から始まる場合は初めから左上・右上のテキストを非表示
            main_work.mode_tex_move_frm = 0;
        }
        // ACT選択から始まるとき
        else
        {
            // ZONEテーブルの初期表示位置設定
            if (main_work.is_final_open == DMD_STGSLCT_ZONE_ALL_OPEN)
            {
                // ZONEテーブルの初期表示位置設定
                for (uint i = 0; i < (int)DME_STGSLCT_ZONE_TYPE_NUM; i++)
                {
                    main_work.zone_pos[i][0] = dm_stgslct_a_zone_nodisp_pos_tbl[i][0];
                    main_work.zone_pos[i][1] = dm_stgslct_a_zone_nodisp_pos_tbl[i][1];
                }
            }

            else if ((main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN) != 0)
            {
                // ZONEテーブルの初期表示位置設定
                for (uint i = 0; i < (int)DME_STGSLCT_ZONE_TYPE_NUM; i++)
                {
                    main_work.zone_pos[i][0] = dm_stgslct_s_zone_nodisp_pos_tbl[i][0];
                    main_work.zone_pos[i][1] = dm_stgslct_s_zone_nodisp_pos_tbl[i][1];
                }
            }

            else if ((main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN) != 0)
            {
                // ZONEテーブルの初期表示位置設定
                for (uint i = 0; i < (int)DME_STGSLCT_ZONE_TYPE_NUM; i++)
                {
                    main_work.zone_pos[i][0] = dm_stgslct_f_zone_nodisp_pos_tbl[i][0];
                    main_work.zone_pos[i][1] = dm_stgslct_f_zone_nodisp_pos_tbl[i][1];
                }
            }
            else
            {
                // ZONEテーブルの初期表示位置設定
                for (uint i = 0; i < (int)DME_STGSLCT_ZONE_TYPE_NUM; i++)
                {
                    main_work.zone_pos[i][0] = dm_stgslct_n_zone_nodisp_pos_tbl[i][0];
                    main_work.zone_pos[i][1] = dm_stgslct_n_zone_nodisp_pos_tbl[i][1];
                }
            }



            // ACTテーブルの内側表示位置設定
            for (uint j = act_num_start; j < act_num_end; j++)
            {
                main_work.act_top_pos_x[j] = DMD_STGSLCT_ACT_TABLE_TOP_POS_X;
                if (main_work.cur_zone != (uint)DME_STGSLCT_ZONE_TYPE_SPE)
                {
                    main_work.act_top_pos_y[j] = dm_stgslct_act_disp_y_pos_tbl[j];
                }
                else
                {
                    main_work.act_top_pos_y[j] = dm_stgslct_act_tab_disp_y_pos_tbl[main_work.focus_disp_no];
                }
                //											+ (float)(dm_stgslct_act_disp_pos_y_tbl[j] * DMD_STGSLCT_ACT_TABLE_DIST_Y);

                //			main_work.act_top_pos_y[j] = dm_stgslct_act_disp_y_pos_tbl[j]
                //											+ (float)(dm_stgslct_act_disp_pos_y_tbl[j] * DMD_STGSLCT_ACT_TABLE_DIST_Y);
            }

#if _IPHONE
            dmStgSlctStageSelectChngZoneSetInZoneScroll(main_work, main_work.cur_stage);
#endif //_IPHONE		

            // モードテキスト、エメラルドテーブルの初期表示位置設定
            main_work.mode_tex_pos_y = DMD_STGSLCT_DOWN_ACT_DISP_POS;
            main_work.chaos_eme_pos_y = DMD_STGSLCT_DOWN_ACT_NODISP_POS;

            // ACT選択から始まる場合は初めから左上・右上のテキストを表示
            main_work.mode_tex_move_frm = 5;
        }

        main_work.crsr_pos_y = dm_stgslct_act_disp_y_pos_tbl[0];

        // テクスチャセットまで出来たので描画プロシージャを設定
        main_work.proc_draw = dmStgSlctProcActDraw;

        // フェード処理開始
        IzFadeInitEasy(IZE_FADE_SET_TYPE_NORMAL
                       , IZE_FADE_TYPE_BLACK_FADEIN
                       , DMD_STGSLCT_FADEIN_TIME
                       );

        // イベント遷移
        main_work.proc_menu_update = dmStgSlctProcFadeIn;
    }



    // ==========================================================================
    // dmStgSlctProcFadeIn
    /*!
        フェードイン中処理
     */
    // ==========================================================================
    private static void dmStgSlctProcFadeIn(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (IzFadeIsEnd())
        {
            // フェード終了
            IzFadeExit();

            // ここで白演出があれば演出へ遷移し、なければ通常処理
            if ((main_work.flag & DMD_STGSLCT_FLAG_WHITE_FLASH_EFCT) != 0)
            {
                // 白フェードアウト設定
                IzFadeInitEasy(IZE_FADE_SET_TYPE_NORMAL
                               , IZE_FADE_TYPE_WHITE_FADEOUT
                               , DMD_STGSLCT_FADEIN_TIME
                               );

                // 白フラッシュ演出シーケンスへ
                main_work.proc_menu_update = dmStgSlctProcSetWhiteFlashEfct;
            }

            else
            {
                // 通常設定処理へ
                main_work.proc_menu_update = dmStgSlctProcSetSlctStartData;
            }
        }
    }



    // ==========================================================================
    // dmStgSlctProcSetWhiteFlashEfct
    /*!
        白フラッシュ演出中処理
     */
    // ==========================================================================
    private static void dmStgSlctProcSetWhiteFlashEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (IzFadeIsEnd())
        {
            // フェード処理開始
            IzFadeInitEasy(IZE_FADE_SET_TYPE_NORMAL
                           , IZE_FADE_TYPE_WHITE_FADEIN
                           , DMD_STGSLCT_FADEIN_TIME
                           );

            // ここで隠れていた分のゾーンを表示にする


            // 白フラッシュ演出フラグOFF
            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_WHITE_FLASH_EFCT);

            // 白フェードアウト終了待ち処理へ
            main_work.proc_menu_update = dmStgSlctProcIsWhiteFlashEfctEnd;
        }
    }



    // ==========================================================================
    // dmStgSlctProcIsWhiteFlashEfctEnd
    /*!
        白フラッシュ演出中処理
     */
    // ==========================================================================
    private static void dmStgSlctProcIsWhiteFlashEfctEnd(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (IzFadeIsEnd())
        {
            // フェード終了
            IzFadeExit();

            // ZONE選択へ
            main_work.proc_menu_update = dmStgSlctProcSetSlctStartData;
        }
    }



    // ==========================================================================
    // dmStgSlctProcSetSlctStartData
    /*!
        各選択画面へ遷移するための設定処理
     */
    // ==========================================================================
    private static void dmStgSlctProcSetSlctStartData(DMS_STGSLCT_MAIN_WORK main_work)
    {
        // ウインドウアナウンスがあればウインドウプロシージャ設定
        main_work.proc_win_update = dmStgSlctProcWindowNodispIdle;

        // ※この初期表示位置については、初期状態がZONE選択かACT選択かで分かれる
        if (main_work.state == (int)DME_STGSLCT_MODE_STATE_ZONE_SLCT)
        {
            // 入力処理設定		仮
            main_work.proc_input = dmStgSlctInputProcZoneSelect;

            // ZONE選択へ
            main_work.proc_menu_update = dmStgSlctProcZoneSelectIdle;
        }
        else
        {
            // 入力処理設定		仮
            main_work.proc_input = dmStgSlctInputProcStageSelect;

            // ZONE選択へ
            main_work.proc_menu_update = dmStgSlctProcStageSelectIdle;

            main_work.disp_flag |= DMD_STGSLCT_DISP_FLAG_LR_ARROW;
        }

        // 初期ZONEスクリーンID設定
        main_work.zone_scr_id = 0;
    }



    // ==========================================================================
    // dmStgSlctProcZoneSelectIdle
    /*!
        ゾーン選択時の入力待ち中処理
     */
    // ==========================================================================
    private static void dmStgSlctProcZoneSelectIdle(DMS_STGSLCT_MAIN_WORK main_work)
    {
        // 入力処理
        if (main_work.proc_input != null)
        {
            main_work.proc_input(main_work);
        }

#if _IPHONE
        {   //戻る演出
            er.CTrgAoAction cancel = main_work.trg_cancel;
            float frame = main_work.act[(int)ACT_BACK_BTN01_L].frame;
            if (cancel.GetState(0)[er.CTrgState.EState.Up] && cancel.GetState(0)[er.CTrgState.EState.Prev])
            {
                frame = 2.0f;
            }
            else if (cancel.GetState(0)[er.CTrgState.EState.On])
            {
                frame = 1.0f;
            }
            else if (2.0f <= frame)
            {
                //決定演出中なら続ける
            }
            else
            {
                frame = 0.0f;
            }

            AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[3]));
            for (int i = (int)ACT_BACK_BTN01_L; i <= (int)ACT_BACK_BTN01_R; i++)
            {
                AoActSetFrame(main_work.act[i], frame);
                AoActUpdate(main_work.act[i], 0.0f);
            }
        }
#endif //_IPHONE

        // キャンセルフラグONならば
        if ((main_work.flag & DMD_STGSLCT_FLAG_CANCEL) != 0)
        {
            // フラグOFF
            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_DECIDE);
            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_CANCEL);

            main_work.proc_win_input = null;
            main_work.proc_input = null;

            main_work.proc_win_update = null;
            main_work.proc_menu_update = dmStgSlctProcFadeOut;

            // メインメニューへイベント遷移先設定
            main_work.next_evt = (int)DME_STGSLCT_NEXT_EVT_MAINMENU;

            // フェードアウト開始
            IzFadeInitEasy(IZE_FADE_SET_TYPE_NORMAL
                           , IZE_FADE_TYPE_BLACK_FADEOUT
                           , DMD_STGSLCT_FADEOUT_TIME
                           );

            DmSoundPlaySE("Cancel");

            // BGMフェードアウト開始
#if (GSD_DEBUG_DEMO_SELECT) || (GSD_DEBUG_DEMO_SELECT_TOP)
		DmSndBgmPlayerExit();
		main_work.flag |= DMD_STGSLCT_FLAG_DEMO_SND_END;
#endif
            //		DmSoundStopBGM(DMD_STGSLCT_BGM_FADEOUT_TIME);

            return;
        }

        // 決定フラグONならば
        if ((main_work.flag & DMD_STGSLCT_FLAG_DECIDE) != 0)
        {

            DmSoundPlaySE("Ok");

            main_work.proc_menu_update = dmStgSlctProcZoneSelectDecideEfct;

            // 決定ZONEテーブルの移動演出フラグON
            main_work.flag |= DMD_STGSLCT_FLAG_DECIDE_ZONE_TABLE_EFCT;

            main_work.timer = 0;

            return;
        }

        // ZONEスクリーン切り替え演出
        dmStgSlctSetZoneScrChangeEfct(main_work);
    }



    // ==========================================================================
    // dmStgSlctProcZoneSelectDecideEfct
    /*!
        ゾーン選択時のゾーンテーブル掃け演出中処理
     */
    // ==========================================================================
    private static void dmStgSlctProcZoneSelectDecideEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        main_work.timer++;

        // 演出設定処理
        dmStgSlctSetDecideZoneEfctPos(main_work);

        // 演出終了チェック
        if (dmStgSlctIsDecideZoneEfctPos(main_work))
        {
            main_work.proc_menu_update = dmStgSlctProcZoneSelectOutEfct;

            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_DECIDE);

            // 掃け演出開始フラグ(アクティブは開始をずらす)
            for (uint i = 0; i < (uint)DME_STGSLCT_ZONE_TYPE_NUM; i++)
            {
                if (main_work.cur_zone != i)
                {
                    main_work.efct_out_flag |= (uint)(1 << (int)i);
                }
            }

            main_work.decide_zone_efct_dist_x = 0;
            main_work.decide_zone_efct_dist_y = 0;

            main_work.cur_stage = (int)dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
            main_work.prev_stage = main_work.cur_stage;

            main_work.proc_input = null;

            main_work.timer = 0;

#if _IPHONE
            main_work.trg_act_move.ResetState();
#endif // _IPHONE

            // フラグOFF
            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_DECIDE);
            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_CANCEL);

            return;
        }

        // ZONEスクリーン切り替え演出
        dmStgSlctSetZoneScrChangeEfct(main_work);
    }



    // ==========================================================================
    // dmStgSlctProcZoneSelectOutEfct
    /*!
        ゾーン選択時のゾーンテーブル掃け演出中処理
     */
    // ==========================================================================
    private static void dmStgSlctProcZoneSelectOutEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        uint act_num_start = 0;
        uint act_num_end = 0;

        act_num_start = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
        act_num_end = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] + act_num_start;

        // 掃け演出が終了したら
        if (dmStgSlctIsZonePosOutEfct(main_work))
        {
            main_work.proc_menu_update = dmStgSlctProcStageSelectInEfct;

            main_work.efct_time = 0;

            // 指定座標に設定
            for (uint i = 0; i < (uint)DME_STGSLCT_ZONE_TYPE_NUM; i++)
            {
                if (main_work.cur_zone != i)
                {
                    main_work.zone_pos[i][0] = dm_stgslct_a_zone_nodisp_pos_tbl[i][0];
                    main_work.zone_pos[i][1] = dm_stgslct_a_zone_nodisp_pos_tbl[i][1];
                }
            }

            // ACT側も演出用の座標に設定
            for (uint i = act_num_start; i < act_num_end; i++)
            {
                main_work.act_top_pos_x[i] = DMD_STGSLCT_STAGE_TAB_NODISP_POS_X;

                if (main_work.cur_zone != (uint)DME_STGSLCT_ZONE_TYPE_SPE)
                {
                    main_work.act_top_pos_y[i] = dm_stgslct_act_disp_y_pos_tbl[i];
                }
                else
                {
                    main_work.act_top_pos_y[i] = dm_stgslct_act_tab_disp_y_pos_tbl[0];
                }
            }

            main_work.state = (int)DME_STGSLCT_MODE_STATE_ACT_SLCT;

            // ZONE選択へ戻ると左上・右上のテキストのACTIONフレームを0に戻す(リセット)
            main_work.mode_tex_move_frm = 0;

#if _IPHONE
            main_work.is_disp_cover = false; //アクトセレクトのカバーを外す
            dmStgSlctStageSelectChngZoneSetInZoneScroll(main_work);
#endif //_IPHONE

            return;
        }

        // 掃け演出処理
        dmStgSlctSetZonePosOutEfct(main_work);

        // アクティブ分の演出開始フラグON
        if (main_work.efct_time == 20)
        {
            main_work.efct_out_flag |= 1u << (int)main_work.cur_zone;
        }

        // タイマー更新
        main_work.efct_time++;

        // ZONEスクリーン切り替え演出
        dmStgSlctSetZoneScrChangeEfct(main_work);
    }



    // ==========================================================================
    // dmStgSlctProcStageSelectInEfct
    /*!
        ステージ選択時のステージテーブル入り演出中処理
     */
    // ==========================================================================
    private static void dmStgSlctProcStageSelectInEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        // 入り演出処理
        dmStgSlctSetStagePosInEfct(main_work);

        // 入り演出が終了したら
        if (dmStgSlctIsStagePosInEfct(main_work))
        {
            main_work.proc_menu_update = dmStgSlctProcStageSelectIdle;

            if (main_work.proc_win_update == dmStgSlctProcWindowNodispIdle)
            {
                main_work.proc_input = dmStgSlctInputProcStageSelect;
            }

            main_work.disp_flag |= DMD_STGSLCT_DISP_FLAG_LR_ARROW;

            // カーソル初期設定
            main_work.crsr_idx = 0;

            if (main_work.cur_zone != (uint)DME_STGSLCT_ACT_PAGE_FINAL)
            {
                main_work.crsr_pos_y = dm_stgslct_act_crsr_disp_y_pos_tbl[main_work.crsr_idx];
            }
            else
            {
                main_work.crsr_pos_y = 320.0f;
            }

#if !_IPHONE
            main_work.focus_disp_no = 0;
#endif //!_IPHONE

            return;
        }

        // ACT選択に切り替え時は左上・右上のテキストを移動開始
        main_work.mode_tex_move_frm++;

        if (main_work.mode_tex_move_frm > 5)
        {
            main_work.mode_tex_move_frm = 5;
        }


        // 座標再設定

    }



    // ==========================================================================
    // dmStgSlctProcStageSelectIdle
    /*!
        ステージ選択時の入力待ち中処理
     */
    // ==========================================================================
    private static void dmStgSlctProcStageSelectIdle(DMS_STGSLCT_MAIN_WORK main_work)
    {
        // 入力処理
        if (main_work.proc_input != null)
        {
            main_work.proc_input(main_work);
        }

#if _IPHONE
        {   //戻る演出
            er.CTrgAoAction cancel = main_work.trg_cancel;
            float frame = main_work.act[(int)ACT_BACK_BTN01_L].frame;
            if (!main_work.trg_act_move.GetState(0)[er.CTrgState.EState.DragAndDrop] && cancel.GetState(0)[er.CTrgState.EState.Lock])
            {
                if (cancel.GetState(0)[er.CTrgState.EState.Up] && cancel.GetState(0)[er.CTrgState.EState.Prev])
                {
                    frame = 2.0f;
                }
                else if (cancel.GetState(0)[er.CTrgState.EState.On])
                {
                    frame = 1.0f;
                }
                else
                {
                    frame = 0.0f;
                }
            }
            else if (2.0f <= frame)
            {
                //決定演出中なら続ける
            }
            else
            {
                frame = 0.0f;
            }

            AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[3]));
            for (int i = (int)ACT_BACK_BTN01_L; i <= (int)ACT_BACK_BTN01_R; i++)
            {
                AoActSetFrame(main_work.act[i], frame);
                AoActUpdate(main_work.act[i], 0.0f);
            }
        }
#endif //_IPHONE

        // キャンセルフラグONならば
        if ((main_work.flag & DMD_STGSLCT_FLAG_CANCEL) != 0)
        {
            main_work.proc_menu_update = dmStgSlctProcStageSelectOutEfct;

            DmSoundPlaySE("Cancel");

            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_CANCEL);

            main_work.proc_input = null;

            main_work.disp_flag &= unchecked((uint)~DMD_STGSLCT_DISP_FLAG_LR_ARROW);

            return;
        }

        // 決定フラグONならば
        if ((main_work.flag & DMD_STGSLCT_FLAG_DECIDE) != 0)
        {
            //		main_work.proc_menu_update = dmStgSlctProcStageSelectOutEfct;

#if true
            if (!dmStgSlctIsCanSelectAct(main_work))
            {
                main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_DECIDE);

#if _IPHONE  //アクトセレクトのカバーを外す                 
                main_work.is_disp_cover = false;
#endif //_IPHONE	//アクトセレクトのカバーを外す
                return;
            }
#else
            if (main_work.is_clear_stage[main_work.cur_stage] == -1)
            {
                main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_DECIDE);

#if _IPHONE  //アクトセレクトのカバーを外す                 
                main_work.is_disp_cover = false;
#endif //_IPHONE	//アクトセレクトのカバーを外す
                return;
            }
#endif

            DmSoundPlaySE("Ok");

            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_DECIDE);

            // ACT決定確認ウインドウ状態へ
            main_work.proc_win_update = dmStgSlctProcWindowOpenEfct;

            // ウインドウ遷移関連設定
            main_work.proc_input = null;
            main_work.proc_win_input = null;
            main_work.win_timer = 0;
            main_work.win_cur_slct = 0;
            main_work.win_mode = (int)DME_STGSLCT_WIN_STG_SLCT;
#if _IPHONE
            main_work.win_is_disp_cover = false;
            main_work.win_timer = -10.5f; //ウインドウの開きを10フレーム遅らす(かつインクリメントで0.0fと通らない様に)
#endif //_IPHONE

            return;
        }

        // ランキング遷移ボタン押下時
        if ((main_work.flag & DMD_STGSLCT_FLAG_CHANGE_EVT_RANKING) != 0)
        {

            main_work.proc_menu_update = dmStgSlctProcFadeOut;

            // フェードアウト開始
            IzFadeInitEasy(IZE_FADE_SET_TYPE_NORMAL
                           , IZE_FADE_TYPE_BLACK_FADEOUT
                           , DMD_STGSLCT_FADEOUT_TIME
                           );

            // ランキングへ遷移イベント設定
            main_work.next_evt = (int)DME_STGSLCT_NEXT_EVT_RANKING;

            // ボタン押下時
            DmSoundPlaySE("Ok");

            // BGMフェードアウト開始
#if (GSD_DEBUG_DEMO_SELECT) || (GSD_DEBUG_DEMO_SELECT_TOP)
		DmSndBgmPlayerExit();
		main_work.flag |= DMD_STGSLCT_FLAG_DEMO_SND_END;
#endif
            //		DmSoundStopBGM(DMD_STGSLCT_BGM_FADEOUT_TIME);

            // フラグOFF
            main_work.proc_input = null;
            main_work.proc_win_input = null;
            main_work.win_timer = 0;
            main_work.win_cur_slct = 0;
            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_CHANGE_EVT_RANKING);

            return;
        }

        // ZONE切り替え
        if ((main_work.flag & DMD_STGSLCT_FLAG_ACT_CHNG_ZONE) != 0)
        {
            main_work.proc_menu_update = dmStgSlctProcStageSelectChngZone;

            // ウインドウ遷移関連設定
            main_work.proc_input = dmStgSlctInputProcStageSelectChngZone;//null;

            DmSoundPlaySE("Cursol");

            main_work.timer = 0;
#if _IPHONE
            dmStgSlctStageSelectChngZoneSetInZoneScroll(main_work);
#endif //_IPHONE

            return;
        }

        //#if !_IPHONE
        if ((main_work.flag & DMD_STGSLCT_FLAG_UP_CHNG_CRSR) != 0
            || (main_work.flag & DMD_STGSLCT_FLAG_DOWN_CHNG_CRSR) != 0)
        {
            //	if (main_work.flag & DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL
            //		|| main_work.flag & DMD_STGSLCT_FLAG_ACT_CHNG_CRSR) {
            main_work.proc_menu_update = dmStgSlctProcStageSelectChngVrtclAct;

            // ウインドウ遷移関連設定
            main_work.proc_input = dmStgSlctInputProcStageSelectMove;//null;

            DmSoundPlaySE("Cursol");

            dmStgSlctSetFocusChangeEfctData(main_work);

            main_work.timer = 0;

            // フラグOFF
            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_UP_CHNG_CRSR);
            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_DOWN_CHNG_CRSR);

            return;
        }
        //#else //!_IPHONE
        if ((main_work.flag & DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL) != 0)
        {
            main_work.proc_menu_update = dmStgSlctProcStageSelectChngVrtclAct;

            // ウインドウ遷移関連設定
            main_work.proc_input = dmStgSlctInputProcStageSelectMove;//null;

            dmStgSlctSetFocusChangeEfctData(main_work);

            main_work.timer = 0;

            return;
        }
        //#endif //!_IPHONE
    }



    // ==========================================================================
    // dmStgSlctProcStageSelectChngZone
    /*!
        ステージ選択時のゾーン切り替え演出中処理
     */
    // ==========================================================================
    private static void dmStgSlctProcStageSelectChngZone(DMS_STGSLCT_MAIN_WORK main_work)
    {
        // 入力処理
        if (main_work.proc_input != null)
        {
            main_work.proc_input(main_work);
        }

        // 掃け演出が終了したら
        if (dmStgSlctIsStageZoneChangeEfct(main_work))
        {
            // ここの遷移はキャンセルか決定かで分岐する
            main_work.proc_menu_update = dmStgSlctProcStageSelectIdle;

            if (main_work.proc_win_update == dmStgSlctProcWindowNodispIdle)
            {
                main_work.proc_input = dmStgSlctInputProcStageSelect;
            }

            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_ACT_CHNG_ZONE);      // ◆

            return;
        }

        // 掃け演出処理
        dmStgSlctSetStageZoneChangeEfct(main_work);

        // 座標再設定

    }



    // ==========================================================================
    // dmStgSlctProcStageSelectChngVrtclAct
    /*!
        ステージ選択時のゾーン切り替え演出中処理
     */
    // ==========================================================================
    private static void dmStgSlctProcStageSelectChngVrtclAct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        // 入力処理
        if (main_work.proc_input != null)
        {
            main_work.proc_input(main_work);
        }

        if ((main_work.flag & DMD_STGSLCT_FLAG_UP_CHNG_CRSR) != 0
            || (main_work.flag & DMD_STGSLCT_FLAG_DOWN_CHNG_CRSR) != 0)
        {

            dmStgSlctSetFocusChangeEfctData(main_work);

            DmSoundPlaySE("Cursol");

            main_work.timer = 0;

            // フラグOFF
            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_UP_CHNG_CRSR);
            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_DOWN_CHNG_CRSR);

            return;
        }

        // 掃け演出が終了したら
        if (0 == (main_work.flag & DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL)
            && 0 == (main_work.flag & DMD_STGSLCT_FLAG_ACT_CHNG_CRSR))
        {
            // ここの遷移はキャンセルか決定かで分岐する
            main_work.proc_menu_update = dmStgSlctProcStageSelectIdle;

            if (main_work.proc_win_update == dmStgSlctProcWindowNodispIdle)
            {
                main_work.proc_input = dmStgSlctInputProcStageSelect;
            }

            // 座標再設定処理

            main_work.timer = 0;

            return;
        }

        // 掃け演出処理
        if ((main_work.flag & DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL) != 0)
        {
            dmStgSlctSetStageVrtclChangeEfct(main_work);

            if (dmStgSlctIsStageVrtclChangeEfct(main_work))
            {
                main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL);
            }
        }

        // カーソル移動
        if ((main_work.flag & DMD_STGSLCT_FLAG_ACT_CHNG_CRSR) != 0)
        {
            dmStgSlctSetStageCrsrChangeEfct(main_work);

            if (dmStgSlctIsStageCrsrChangeEfct(main_work))
            {
                main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_ACT_CHNG_CRSR);
            }
        }

        // 座標再設定
        main_work.timer++;
    }



    // ==========================================================================
    // dmStgSlctProcStageSelectOutEfct
    /*!
        ステージ選択時のステージテーブル掃け演出中処理
     */
    // ==========================================================================
    private static void dmStgSlctProcStageSelectOutEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        // 掃け演出が終了したら
        if (dmStgSlctIsStagePosOutEfct(main_work))
        {
            // ここの遷移はキャンセルか決定かで分岐する
            main_work.proc_menu_update = dmStgSlctProcZoneSelectInEfct;

            // STATEをZONE選択に設定
            main_work.state = (int)DME_STGSLCT_MODE_STATE_ZONE_SLCT;

            // ZONEテーブルの位置を設定
            for (int i = 0; i < (int)DME_STGSLCT_ZONE_TYPE_NUM; i++)
            {
                if (main_work.is_final_open == DMD_STGSLCT_ZONE_ALL_OPEN)
                {
                    // ZONEテーブルの初期表示位置設定
                    main_work.zone_pos[i][0] = dm_stgslct_a_zone_nodisp_pos_tbl[i][0];
                    main_work.zone_pos[i][1] = dm_stgslct_a_zone_nodisp_pos_tbl[i][1];
                }

                else if ((main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN) != 0)
                {
                    // ZONEテーブルの初期表示位置設定
                    main_work.zone_pos[i][0] = dm_stgslct_s_zone_nodisp_pos_tbl[i][0];
                    main_work.zone_pos[i][1] = dm_stgslct_s_zone_nodisp_pos_tbl[i][1];
                }

                else if ((main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN) != 0)
                {
                    // ZONEテーブルの初期表示位置設定
                    main_work.zone_pos[i][0] = dm_stgslct_f_zone_nodisp_pos_tbl[i][0];
                    main_work.zone_pos[i][1] = dm_stgslct_f_zone_nodisp_pos_tbl[i][1];
                }
                else
                {
                    // ZONEテーブルの初期表示位置設定
                    main_work.zone_pos[i][0] = dm_stgslct_n_zone_nodisp_pos_tbl[i][0];
                    main_work.zone_pos[i][1] = dm_stgslct_n_zone_nodisp_pos_tbl[i][1];
                }
            }

            // ZONE選択ではスペステがないため、その分をCLIPする
            //		if (main_work.cur_zone > DME_STGSLCT_ACT_PAGE_FINAL
            //			|| main_work.cur_zone < DME_STGSLCT_ACT_PAGE_1) {
            //			// 範囲外の値の場合は全てZONE１に設定
            //			main_work.cur_zone = DME_STGSLCT_ACT_PAGE_1;
            //		}

            // ZONE選択へ戻ると左上・右上のテキストのACTIONフレームを0に戻す(リセット)
            main_work.mode_tex_move_frm = 0;

            // 初期ZONEスクリーンID設定
            main_work.zone_scr_id = 0;

#if _IPHONE   //ゾーンセレクトのカバーを外す                 
            main_work.is_disp_cover = false;
#endif //_IPHONE	//ゾーンセレクトのカバーを外す

            return;
        }

        // 掃け演出処理
        dmStgSlctSetStagePosOutEfct(main_work);


        // ACT選択に切り替え時は左上・右上のテキストを移動開始
        main_work.mode_tex_move_frm++;

        if (main_work.mode_tex_move_frm > 10)
        {
            main_work.mode_tex_move_frm = 10;
        }


        // 座標再設定

    }



    // ==========================================================================
    // dmStgSlctProcZoneSelectInEfct
    /*!
        ゾーン選択時のゾーンテーブル入り演出中処理
     */
    // ==========================================================================
    private static void dmStgSlctProcZoneSelectInEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        // 入り演出が終了したら
        if (dmStgSlctIsZonePosInEfct(main_work))
        {
            main_work.proc_menu_update = dmStgSlctProcZoneSelectIdle;

            // 座標設定(保険)
            for (int i = 0; i < (int)DME_STGSLCT_ZONE_TYPE_NUM; i++)
            {
                if (main_work.is_final_open == DMD_STGSLCT_ZONE_ALL_OPEN)
                {
                    // ZONEテーブルの初期表示位置設定
                    for (uint j = 0; j < (int)DME_STGSLCT_ZONE_TYPE_NUM; j++)
                    {
                        main_work.zone_pos[j][0] = dm_stgslct_a_zone_disp_pos_tbl[j][0];
                        main_work.zone_pos[j][1] = dm_stgslct_a_zone_disp_pos_tbl[j][1];
                    }
                }

                else if ((main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN) != 0)
                {
                    // ZONEテーブルの初期表示位置設定
                    for (uint k = 0; k < (int)DME_STGSLCT_ZONE_TYPE_NUM; k++)
                    {
                        main_work.zone_pos[k][0] = dm_stgslct_s_zone_disp_pos_tbl[k][0];
                        main_work.zone_pos[k][1] = dm_stgslct_s_zone_disp_pos_tbl[k][1];
                    }
                }

                else if ((main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN) != 0)
                {
                    // ZONEテーブルの初期表示位置設定
                    for (uint l = 0; l < (int)DME_STGSLCT_ZONE_TYPE_NUM; l++)
                    {
                        main_work.zone_pos[l][0] = dm_stgslct_f_zone_disp_pos_tbl[l][0];
                        main_work.zone_pos[l][1] = dm_stgslct_f_zone_disp_pos_tbl[l][1];
                    }
                }
                else
                {
                    // ZONEテーブルの初期表示位置設定
                    for (uint m = 0; m < (int)DME_STGSLCT_ZONE_TYPE_NUM; m++)
                    {
                        main_work.zone_pos[m][0] = dm_stgslct_n_zone_disp_pos_tbl[m][0];
                        main_work.zone_pos[m][1] = dm_stgslct_n_zone_disp_pos_tbl[m][1];
                    }
                }
            }

            if (main_work.proc_win_update == dmStgSlctProcWindowNodispIdle)
            {
                main_work.proc_input = dmStgSlctInputProcZoneSelect;
            }

            return;
        }

        // 入り演出処理
        dmStgSlctSetZonePosInEfct(main_work);

        // 座標再設定

#if false
        main_work.cur_zone = uint.MaxValue;
#endif //_IPHONE
    }



    // ==========================================================================
    // dmStgSlctProcWindowNodispIdle
    /*!
        ウインドウ非表示待ち中処理
     */
    // ==========================================================================
    private static void dmStgSlctProcWindowNodispIdle(DMS_STGSLCT_MAIN_WORK main_work)
    {
        // ウインドウ入力処理
        if (main_work.proc_win_input != null)
        {
            main_work.proc_win_input(main_work);
        }

        // メニュー遷移フラグONならば
        if ((main_work.flag & DMD_STGSLCT_FLAG_DISP_MENU) != 0
            || main_work.announce_flag != 0)
        {
            main_work.proc_win_update = dmStgSlctProcWindowOpenEfct;

            // 通常処理の入力処理をなくす(二重入力を防ぐため)
            main_work.proc_input = null;

            // ウインドウ開閉演出時は入力処理なし
            main_work.proc_win_input = null;

            // ウインドウ演出用タイマー初期化
            main_work.win_timer = 0;

            // ウインドウ選択変数設定
            for (uint i = (uint)DME_STGSLCT_WIN_1_1_CLEAR; i < (uint)DME_STGSLCT_WIN_NUM; i++)
            {
                if ((main_work.announce_flag & 1u << (int)i) != 0)
                {
                    main_work.win_mode = (int)i;
                    break;
                }
            }
            //		main_work.win_cur_slct = 0;

            // ウインドウ演出中フラグON
            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_DISP_MENU);
            //		main_work.flag |= DMD_STGSLCT_FLAG_WIN_EFCT;

            DmSoundPlaySE("Window");
        }


    }




    // ==========================================================================
    // dmStgSlctProcWindowOpenEfct
    /*!
        ウインドウオープン中処理
     */
    // ==========================================================================
    private static void dmStgSlctProcWindowOpenEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        // 演出終了チェック
        if ((main_work.flag & DMD_STGSLCT_FLAG_WIN_EFCT_END) != 0)
        {
            // ウインドウのプロシージャ設定
            main_work.proc_win_update = dmStgSlctProcWindowAnnounceIdle;

            // 入力処理設定
            main_work.proc_win_input = dmStgSlctInputProcWinDispIdle;

#if _IPHONE
            ArrayPointer<AOS_ACTION> arrayPointer1 = new ArrayPointer<AOS_ACTION>(main_work.act, 106);
            ArrayPointer<AOS_ACTION> arrayPointer2 = new ArrayPointer<AOS_ACTION>(main_work.act, 112);
            while (arrayPointer1 != arrayPointer2)
            {
                AoActSetFrame(~arrayPointer1, 0.0f);
                ++arrayPointer1;
            }

            int index1 = 0;
            for (int index2 = arrayof(main_work.trg_answer); index1 < index2; ++index1)
                main_work.trg_answer[index1].ResetState();
#endif //_IPHONE

            // ウインドウ内アクション表示フラグON
            main_work.disp_flag |= DMD_STGSLCT_DISP_FLAG_WIN_ACT;

            // ウインドウ演出中フラグOFF
            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_WIN_EFCT_END);
        }
        else
        {
            // ウインドウオープン演出処理
            dmStgSlctSetWinOpenEfct(main_work);
            //		main_work.flag |= DMD_STGSLCT_FLAG_WIN_EFCT_END;
        }

        // ウインドウ描画
        dmStgSlctWinSelectDraw(main_work);
    }



    // ==========================================================================
    // dmStgSlctProcWindowAnnounceIdle
    /*!
        ウインドウ入力待ち処理
     */
    // ==========================================================================
    private static void dmStgSlctProcWindowAnnounceIdle(DMS_STGSLCT_MAIN_WORK main_work)
    {
        // ウインドウ入力処理
        if (main_work.proc_win_input != null)
        {
            main_work.proc_win_input(main_work);
        }

        // ACT決定ウインドウ以外の場合
        if (main_work.win_mode == (int)DME_STGSLCT_WIN_MENU)
        {
            if ((main_work.flag & DMD_STGSLCT_FLAG_DECIDE) != 0)
            {
                // 通常処理の入力処理をなくす(二重入力を防ぐため)
                main_work.proc_input = null;

                // ウインドウ開閉演出時は入力処理なし
                main_work.proc_win_input = null;

                // ウインドウ演出時間設定
                main_work.win_timer = DMD_STGSLCT_WIN_EFCT_TIME;

                main_work.proc_menu_update = dmStgSlctProcFadeOut;

                // フェードアウト開始
                IzFadeInitEasy(IZE_FADE_SET_TYPE_NORMAL
                               , IZE_FADE_TYPE_BLACK_FADEOUT
                               , DMD_STGSLCT_FADEOUT_TIME
                               );

                // オプション決定時
                if (main_work.win_cur_slct == 0)
                {
                    // オプションへ遷移イベント設定
                    main_work.next_evt = (int)DME_STGSLCT_NEXT_EVT_RANKING;

                    DmSoundPlaySE("Ok");
                }
                // メインメニューへ戻る決定時
                else
                {
                    // メインメニューへイベント遷移先設定
                    main_work.next_evt = (int)DME_STGSLCT_NEXT_EVT_MAINMENU;

                    DmSoundPlaySE("Ok");
                }

                // BGMフェードアウト開始
#if (GSD_DEBUG_DEMO_SELECT) || (GSD_DEBUG_DEMO_SELECT_TOP)
			DmSndBgmPlayerExit();
			main_work.flag |= DMD_STGSLCT_FLAG_DEMO_SND_END;
#endif
                //			DmSoundStopBGM(DMD_STGSLCT_BGM_FADEOUT_TIME);

                // フラグOFF
                main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_DECIDE);
                main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_CANCEL);
            }
            else if ((main_work.flag & DMD_STGSLCT_FLAG_CANCEL) != 0)
            {
                // 通常処理の入力処理をなくす(二重入力を防ぐため)
                main_work.proc_input = null;

                // ウインドウ開閉演出時は入力処理なし
                main_work.proc_win_input = null;

                // ウインドウ演出時間設定
                main_work.win_timer = DMD_STGSLCT_WIN_EFCT_TIME;
                //			main_work.flag |= DMD_STGSLCT_FLAG_WIN_EFCT;

                // ウインドウ内アクション表示フラグOFF
                main_work.disp_flag &= unchecked((uint)~DMD_STGSLCT_DISP_FLAG_WIN_ACT);

                main_work.proc_win_update = dmStgSlctProcWindowCloseEfct;

                DmSoundPlaySE("Cancel");

                // フラグOFF
                main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_DECIDE);
                main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_CANCEL);
            }
        }
        else if (main_work.win_mode != (int)DME_STGSLCT_WIN_STG_SLCT)
        {
            // メニュー遷移フラグONならば
            if ((main_work.flag & DMD_STGSLCT_FLAG_DECIDE) != 0
                || (main_work.flag & DMD_STGSLCT_FLAG_CANCEL) != 0)
            {
                // 通常処理の入力処理をなくす(二重入力を防ぐため)
                main_work.proc_input = null;

                // ウインドウ開閉演出時は入力処理なし
                main_work.proc_win_input = null;

                // ウインドウ演出時間設定
                main_work.win_timer = DMD_STGSLCT_WIN_EFCT_TIME;
                //			main_work.flag |= DMD_STGSLCT_FLAG_WIN_EFCT;

                // ウインドウ内アクション表示フラグOFF
                main_work.disp_flag &= unchecked((uint)~DMD_STGSLCT_DISP_FLAG_WIN_ACT);

                main_work.proc_win_update = dmStgSlctProcWindowCloseEfct;

                DmSoundPlaySE("Ok");

                // フラグOFF
                main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_DECIDE);
                main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_CANCEL);
            }
        }

        // ACT決定ウインドウの場合
        else
        {
            if ((main_work.flag & DMD_STGSLCT_FLAG_DECIDE) != 0
                && 0 == main_work.win_cur_slct)
            {
                // 通常処理の入力処理をなくす(二重入力を防ぐため)
                main_work.proc_input = null;

                // ウインドウ開閉演出時は入力処理なし
                main_work.proc_win_input = null;

                // ウインドウ演出時間設定
                main_work.win_timer = DMD_STGSLCT_WIN_EFCT_TIME;
                //			main_work.flag |= DMD_STGSLCT_FLAG_WIN_EFCT;

                //			main_work.proc_win_update = null;
                main_work.proc_menu_update = dmStgSlctProcFadeOut;

                // フェードアウト開始
                IzFadeInitEasy(IZE_FADE_SET_TYPE_NORMAL
                               , IZE_FADE_TYPE_BLACK_FADEOUT
                               , DMD_STGSLCT_FADEOUT_TIME
                               );

                // ステージ番号でスペステが通常かをチェック
                if (main_work.cur_zone == (uint)DME_STGSLCT_ACT_PAGE_SPE)
                {
                    // スペステイベント遷移先設定
                    main_work.next_evt = (int)DME_STGSLCT_NEXT_EVT_SPESTE;
                }
                else
                {
                    // 通常ACTイベント遷移先設定
                    main_work.next_evt = (int)DME_STGSLCT_NEXT_EVT_MAINGAME;
                }

                // BGMフェードアウト開始
                DmSndBgmPlayerExit();
                main_work.flag |= DMD_STGSLCT_FLAG_DEMO_SND_END;

                DmSoundPlaySE("Ok");

                // フラグOFF
                main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_DECIDE);
                main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_CANCEL);
            }

            else if ((main_work.flag & DMD_STGSLCT_FLAG_DECIDE) != 0
                || (main_work.flag & DMD_STGSLCT_FLAG_CANCEL) != 0)
            {
                // 通常処理の入力処理をなくす(二重入力を防ぐため)
                main_work.proc_input = null;

                // ウインドウ開閉演出時は入力処理なし
                main_work.proc_win_input = null;

                // ウインドウ演出時間設定
                main_work.win_timer = DMD_STGSLCT_WIN_EFCT_TIME;

                // ウインドウ内アクション表示フラグOFF
                main_work.disp_flag &= unchecked((uint)~DMD_STGSLCT_DISP_FLAG_WIN_ACT);

                main_work.proc_win_update = dmStgSlctProcWindowCloseEfct;

                if ((main_work.flag & DMD_STGSLCT_FLAG_CANCEL) != 0)
                {
                    DmSoundPlaySE("Cancel");
                }
                else
                {
                    DmSoundPlaySE("Ok");
                }

                // フラグOFF
                main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_DECIDE);
                main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_CANCEL);
            }
        }



        // ウインドウ描画
        dmStgSlctWinSelectDraw(main_work);
    }



    // ==========================================================================
    // dmStgSlctProcWindowCloseEfct
    /*!
        ウインドウクローズ中処理
     */
    // ==========================================================================
    private static void dmStgSlctProcWindowCloseEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        // 演出終了チェック
        if ((main_work.flag & DMD_STGSLCT_FLAG_WIN_EFCT_END) != 0)
        {
            // ウインドウのプロシージャ設定
            main_work.proc_win_update = dmStgSlctProcWindowNodispIdle;

            // アナウンス分のフラグOFF
            main_work.announce_flag &= (uint)~(1 << main_work.win_mode);

            if (0 == main_work.announce_flag)
            {
                if (main_work.state == (int)DME_STGSLCT_MODE_STATE_ZONE_SLCT)
                {
                    main_work.proc_input = dmStgSlctInputProcZoneSelect;
                }
                else if (main_work.state == (int)DME_STGSLCT_MODE_STATE_ACT_SLCT)
                {
                    main_work.proc_input = dmStgSlctInputProcStageSelect;
#if _IPHONE    //アクトセレクトのカバーを外す                 
                    main_work.is_disp_cover = false;
#endif //_IPHONE	//アクトセレクトのカバーを外す
                }
            }

            // ウインドウ演出中フラグOFF
            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_WIN_EFCT_END);
        }

        // ウインドウオープン演出処理
        dmStgSlctSetWinCloseEfct(main_work);
        //	main_work.flag |= DMD_STGSLCT_FLAG_WIN_EFCT_END;

        // ウインドウ描画
        dmStgSlctWinSelectDraw(main_work);
    }



    // ==========================================================================
    // dmStgSlctProcFadeOut
    /*!
        フェードアウト中処理
     */
    // ==========================================================================
    private static void dmStgSlctProcFadeOut(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (IzFadeIsEnd())
        {
            // 遷移先なし
            main_work.proc_win_update = null;
            main_work.proc_menu_update = dmStgSlctProcStopDraw;
            main_work.proc_draw = null;

            main_work.timer = 0;

            //		DmSoundExit();

            return;
        }
    }

    // ==========================================================================
    // dmStgSlctProcStopDraw
    /*!
        描画停止処理
     */
    // ==========================================================================
    private static void dmStgSlctProcStopDraw(DMS_STGSLCT_MAIN_WORK main_work)
    {
        main_work.proc_menu_update = dmStgSlctProcDataRelease;
    }

    // ==========================================================================
    // dmStgSlctProcDataRelease
    /*!
        ファイル解放リクエスト処理
     */
    // ==========================================================================
    private static void dmStgSlctProcDataRelease(DMS_STGSLCT_MAIN_WORK main_work)
    {
        // テクスチャ解放
        for (int i = 0; i < (int)DME_STGSLCT_DATA_TYPE_MAX; i++)
        {
            AoTexRelease(main_work.tex[i]);
        }

        // メニュー共通テクスチャ解放
        for (int i = 0; i < 5; i++)
        {
            AoTexRelease(main_work.cmn_tex[i]);
        }

        //	DmSoundFlush();

        // 次へ遷移
        main_work.proc_menu_update = dmStgSlctProcFinish;
    }


    // ==========================================================================
    // dmStgSlctProcFinish
    /*!
        終了処理
     */
    // ==========================================================================
    private static void dmStgSlctProcFinish(DMS_STGSLCT_MAIN_WORK main_work)
    {
        // テクスチャ解放完了判定
        if (dmStgSlctIsTexRelease(main_work))
        {
#if _IPHONE //当たり判定解放          
            {
                for (int index = 0; index < main_work.trg_zone.Length; ++index)
                    main_work.trg_zone[index].Release();
                for (int index = 0; index < main_work.trg_act.Length; ++index)
                    main_work.trg_act[index].Release();
                for (int index = 0; index < main_work.trg_act_tab.Length; ++index)
                    main_work.trg_act_tab[index].Release();
                for (int index = 0; index < main_work.trg_act_lr.Length; ++index)
                    main_work.trg_act_lr[index].Release();
                main_work.trg_act_move.Release();
                for (int index = 0; index < main_work.trg_mode.Length; ++index)
                    main_work.trg_mode[index].Release();
                main_work.trg_cancel.Release();
                for (int index = 0; index < main_work.trg_answer.Length; ++index)
                    main_work.trg_answer[index].Release();
            }
#endif //_IPHONE	//当たり判定解放

            for (int i = 0; i < (int)ACT_NUM; i++)
            {
                if (main_work.act[i] != null)
                {
                    AoActDelete(main_work.act[i]);
                    main_work.act[i] = null;
                }
            }

            // AMB解放
            for (int i = 0; i < 2; i++)
            {
                // ファイル解放
                if (main_work.arc_amb[i] != null)
                {
                    amMemFree(main_work.arc_amb[i]);
                    main_work.arc_amb[i] = null;
                }
            }

            // アクション解放
            for (int i = 0; i < 5; i++)
            {
                // ファイル解放
                if (main_work.arc_cmn_amb[i] != null)
                {
                    amMemFree(main_work.arc_cmn_amb[i]);
                    main_work.arc_cmn_amb[i] = null;
                }
            }

            // 終了処理へ
            //		main_work.flag |= DMD_STGSLCT_FLAG_EXIT;
            main_work.proc_win_update = null;
            main_work.proc_menu_update = dmStgSlctProcWaitFinished;
        }
    }



    // ==========================================================================
    // dmStgSlctProcWaitFinished
    /*!
        終了処理
     */
    // ==========================================================================
    private static void dmStgSlctProcWaitFinished(DMS_STGSLCT_MAIN_WORK main_work)
    {
        // サウンド終了フラグONならば
        if ((main_work.flag & DMD_STGSLCT_FLAG_DEMO_SND_END) != 0)
        {
            if (DmSndBgmPlayerIsTaskExit())
            {
                // 終了処理へ
                main_work.flag |= DMD_STGSLCT_FLAG_EXIT;
                main_work.proc_win_update = null;
                main_work.proc_menu_update = null;

                main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_DEMO_SND_END);
            }
        }
        else
        {
            // 終了処理へ
            main_work.flag |= DMD_STGSLCT_FLAG_EXIT;
            main_work.proc_win_update = null;
            main_work.proc_menu_update = null;
        }
    }



    // ==========================================================================
    // dmStgSlctInputProcZoneSelect
    /*!
        ZONE選択用入力プロシージャ処理(据え置き版)
     */
    // ==========================================================================
    private static void dmStgSlctInputProcZoneSelect(DMS_STGSLCT_MAIN_WORK main_work)
    {
        uint tmp_cur_zone = main_work.cur_zone;
        if (tmp_cur_zone == uint.MaxValue)
            tmp_cur_zone = 0;

        // キャンセル処理=
        if (AoPad.AoPadMStand().HasFlag(ControllerConsts.CANCEL) || main_work.trg_cancel.GetState(0)[er.CTrgState.EState.Up] && main_work.trg_cancel.GetState(0)[er.CTrgState.EState.Prev])
        {
            main_work.flag |= DMD_STGSLCT_FLAG_CANCEL;

            return;
        }

        // ステージ選択への遷移処理
        if (AoPad.AoPadMStand().HasFlag(ControllerConsts.CONFIRM))
        {
            main_work.flag |= DMD_STGSLCT_FLAG_DECIDE;

            return;
        }

        // ゲームモード切替
#if false
	if (AoPadStand() & KEY_R_LEFT) {
		main_work.cur_game_mode ^= DME_STGSLCT_PLAY_MODE_TIME_ATK;
		
		DmSoundPlaySE("Cursol");
		
		return;
	}
#endif


        //#if !_IPHONE
        // 十字キー操作
        else if (AoPad.AoPadMStand().HasFlag(ControllerConsts.UP))
        {
            // 選択ZONE切り替え
            if (main_work.is_final_open == DMD_STGSLCT_ZONE_ALL_OPEN)
            {
                main_work.cur_zone = (uint)dm_stgslct_a_zone_input_dir_tbl[tmp_cur_zone][0];
            }
            else if ((main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN) != 0)
            {
                main_work.cur_zone = (uint)dm_stgslct_s_zone_input_dir_tbl[tmp_cur_zone][0];
            }
            else if ((main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN) != 0)
            {
                main_work.cur_zone = (uint)dm_stgslct_f_zone_input_dir_tbl[tmp_cur_zone][0];
            }
            else
            {
                main_work.cur_zone = (uint)dm_stgslct_n_zone_input_dir_tbl[tmp_cur_zone][0];
            }

            if (main_work.cur_zone != (uint)DME_STGSLCT_ZONE_TYPE_NONE)
            {
                DmSoundPlaySE("Cursol");
                main_work.zone_scr_id = 0;
            }
            // 切り替えフラグ？
        }
        else if (AoPad.AoPadMStand().HasFlag(ControllerConsts.DOWN))
        {
            // 選択ZONE切り替え
            if (main_work.is_final_open == DMD_STGSLCT_ZONE_ALL_OPEN)
            {
                main_work.cur_zone = (uint)dm_stgslct_a_zone_input_dir_tbl[tmp_cur_zone][1];
            }
            else if ((main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN) != 0)
            {
                main_work.cur_zone = (uint)dm_stgslct_s_zone_input_dir_tbl[tmp_cur_zone][1];
            }
            else if ((main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN) != 0)
            {
                main_work.cur_zone = (uint)dm_stgslct_f_zone_input_dir_tbl[tmp_cur_zone][1];
            }
            else
            {
                main_work.cur_zone = (uint)dm_stgslct_n_zone_input_dir_tbl[tmp_cur_zone][1];
            }

            if (main_work.cur_zone != (uint)DME_STGSLCT_ZONE_TYPE_NONE)
            {
                DmSoundPlaySE("Cursol");
                main_work.zone_scr_id = 0;
            }
            //		main_work.cur_zone = dm_stgslct_zone_input_dir_tbl[tmp_cur_zone][1];
        }
        else if (AoPad.AoPadMStand().HasFlag(ControllerConsts.LEFT))
        {
            // 選択ZONE切り替え
            if (main_work.is_final_open == DMD_STGSLCT_ZONE_ALL_OPEN)
            {
                main_work.cur_zone = (uint)dm_stgslct_a_zone_input_dir_tbl[tmp_cur_zone][2];
            }
            else if ((main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN) != 0)
            {
                main_work.cur_zone = (uint)dm_stgslct_s_zone_input_dir_tbl[tmp_cur_zone][2];
            }
            else if ((main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN) != 0)
            {
                main_work.cur_zone = (uint)dm_stgslct_f_zone_input_dir_tbl[tmp_cur_zone][2];
            }
            else
            {
                main_work.cur_zone = (uint)dm_stgslct_n_zone_input_dir_tbl[tmp_cur_zone][2];
            }

            if (main_work.cur_zone != (uint)DME_STGSLCT_ZONE_TYPE_NONE)
            {
                DmSoundPlaySE("Cursol");
                main_work.zone_scr_id = 0;
            }
            //		main_work.cur_zone = dm_stgslct_zone_input_dir_tbl[tmp_cur_zone][2];
        }
        else if (AoPad.AoPadMStand().HasFlag(ControllerConsts.RIGHT))
        {
            // 選択ZONE切り替え
            if (main_work.is_final_open == DMD_STGSLCT_ZONE_ALL_OPEN)
            {
                main_work.cur_zone = (uint)dm_stgslct_a_zone_input_dir_tbl[tmp_cur_zone][3];
            }
            else if ((main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN) != 0)
            {
                main_work.cur_zone = (uint)dm_stgslct_s_zone_input_dir_tbl[tmp_cur_zone][3];
            }
            else if ((main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN) != 0)
            {
                main_work.cur_zone = (uint)dm_stgslct_f_zone_input_dir_tbl[tmp_cur_zone][3];
            }
            else
            {
                main_work.cur_zone = (uint)dm_stgslct_n_zone_input_dir_tbl[tmp_cur_zone][3];
            }

            if (main_work.cur_zone != (uint)DME_STGSLCT_ZONE_TYPE_NONE)
            {
                DmSoundPlaySE("Cursol");
                main_work.zone_scr_id = 0;
            }
            //		main_work.cur_zone = dm_stgslct_zone_input_dir_tbl[tmp_cur_zone][3];
        }
        //#else //!_IPHONE
        for (int i = 0; i < arrayof(main_work.trg_zone); ++i)
        {
            DME_STGSLCT_ZONE_TYPE[] c_zone_table = [  DME_STGSLCT_ZONE_TYPE_1
                                                    ,   DME_STGSLCT_ZONE_TYPE_2
                                                    ,   DME_STGSLCT_ZONE_TYPE_3
                                                    ,   DME_STGSLCT_ZONE_TYPE_4
                                                    ,   DME_STGSLCT_ZONE_TYPE_FINAL
                                                    ,   DME_STGSLCT_ZONE_TYPE_SPE
                                                    ];
            var zone = main_work.trg_zone[i];
            if (zone.GetState(0)[er.CTrgState.EState.Up] && zone.GetState(0)[er.CTrgState.EState.Prev])
            {
                //選択
                main_work.cur_zone = (uint)c_zone_table[i];
                main_work.flag |= DMD_STGSLCT_FLAG_DECIDE;
                break;
            }
            else if (zone.GetState(0)[er.CTrgState.EState.Stand])
            {
                //タップ中
                main_work.cur_zone = (uint)c_zone_table[i];
                main_work.is_disp_cover = true;
                break;
            }
            else if (zone.GetState(0)[er.CTrgState.EState.Out])
            {
                //タップキャンセル
#if false
                main_work.cur_zone = uint.MaxValue;
#else
                main_work.cur_zone = 0;
#endif
                main_work.is_disp_cover = false;
            }
        }
        //#endif //!_IPHONE

        if (main_work.cur_zone == (uint)DME_STGSLCT_ZONE_TYPE_NONE)
        {
            main_work.cur_zone = tmp_cur_zone;
        }
    }



    // ==========================================================================
    // dmStgSlctInputProcStageSelect
    /*!
        STAGE選択用入力プロシージャ処理(据え置き版)
     */
    // ==========================================================================
    private static void dmStgSlctInputProcStageSelect(DMS_STGSLCT_MAIN_WORK main_work)
    {
        uint src = 0;
        uint dst = 0;
        uint act_comp = 0;
        int is_final_open = 0;
        int is_spe_open = 0;
        int edge = 0;

        src = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
        dst = src + dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1];
        act_comp = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] - 1;

        edge = (int)DME_STGSLCT_ACT_PAGE_4;

        // FINALオープン状態かどうか
        if (main_work.is_clear_stage[(int)DME_STGSLCT_STAGE_1_B] == 1
            && main_work.is_clear_stage[(int)DME_STGSLCT_STAGE_2_B] == 1
            && main_work.is_clear_stage[(int)DME_STGSLCT_STAGE_3_B] == 1
            && main_work.is_clear_stage[(int)DME_STGSLCT_STAGE_4_B] == 1)
        {
            is_final_open = 1;
            edge = (int)DME_STGSLCT_ACT_PAGE_FINAL;
        }

        // スペステオープン状態かどうか
        for (uint i = (uint)DME_STGSLCT_STAGE_S_1; i < (uint)DME_STGSLCT_STAGE_NUM; i++)
        {
            if (main_work.is_clear_stage[i] != -1)
            {
                is_spe_open = 1;
                edge = (int)DME_STGSLCT_ACT_PAGE_SPE;
                break;
            }
            else
            {
                is_spe_open = 0;
            }
        }

        // キャンセル処理
        if (AoPad.AoPadStand().HasFlag(ControllerConsts.CANCEL) || (!main_work.trg_act_move.GetState(0)[er.CTrgState.EState.DragAndDrop]
                    && main_work.trg_cancel.GetState(0)[er.CTrgState.EState.Lock]
                    && main_work.trg_cancel.GetState(0)[er.CTrgState.EState.Up]
                    && main_work.trg_cancel.GetState(0)[er.CTrgState.EState.Prev]))
        {
            main_work.flag |= DMD_STGSLCT_FLAG_CANCEL;

            return;
        }

        // ステージ決定処理
        if (AoPad.AoPadStand().HasFlag(ControllerConsts.DECIDE))
        {
            main_work.flag |= DMD_STGSLCT_FLAG_DECIDE;

            return;
        }

        // ゲームモード切替
        if (AoPad.AoPadStand().HasFlag(ControllerConsts.ALT_LEFT))
        {
            main_work.cur_game_mode ^= (uint)DME_STGSLCT_PLAY_MODE_TIME_ATK;

            // ゲームモードテキストアクションのフレーム初期化(アニメーション開始)
            main_work.mode_tex_frm = 1;

            DmSoundPlaySE("Cursol");

            return;
        }

        {
            float frame;
            if (!main_work.trg_act_move.GetState(0)[er.CTrgState.EState.DragAndDrop] && (main_work.trg_mode[0].GetState(0)[er.CTrgState.EState.Lock] || main_work.trg_mode[1].GetState(0)[er.CTrgState.EState.Lock]))
            {
                //ロック中なら
                if (main_work.trg_mode[0].GetState(0)[er.CTrgState.EState.Up] && main_work.trg_mode[0].GetState(0)[er.CTrgState.EState.Prev])
                {
                    //決定
                    frame = 2.0f;
                }
                else if (main_work.trg_mode[1].GetState(0)[er.CTrgState.EState.Up] && main_work.trg_mode[1].GetState(0)[er.CTrgState.EState.Prev])
                {
                    //決定
                    frame = 2.0f;
                }
                else if (main_work.trg_mode[0].GetState(0)[er.CTrgState.EState.On] || main_work.trg_mode[1].GetState(0)[er.CTrgState.EState.On])
                {
                    //ON
                    frame = 1.0f;
                }
                else
                {
                    //OFF
                    frame = 0.0f;
                }
            }
            else if (2.0f < main_work.act[(int)ACT_TAB_STATE_L].frame)
            {
                //決定演出中
                frame = -1.0f;
            }
            else
            {
                //OFF
                frame = 0.0f;
            }
            if (0.0f <= frame)
            {
                if (2.0f == frame)
                {
                    //決定なら
                    main_work.cur_game_mode ^= (uint)DME_STGSLCT_PLAY_MODE_TIME_ATK;
                    DmSoundPlaySE("Cursol");
                }

                ArrayPointer<AOS_ACTION> arrayPointer1 = new ArrayPointer<AOS_ACTION>(main_work.act, 51);
                ArrayPointer<AOS_ACTION> arrayPointer2 = new ArrayPointer<AOS_ACTION>(main_work.act, 57);
                while (arrayPointer1 != arrayPointer2)
                {
                    AoActSetFrame(~arrayPointer1, frame);
                    ++arrayPointer1;
                }

                ArrayPointer<AOS_ACTION> arrayPointer3 = new ArrayPointer<AOS_ACTION>(main_work.act, 88);
                ArrayPointer<AOS_ACTION> arrayPointer4 = new ArrayPointer<AOS_ACTION>(main_work.act, 90);
                while (arrayPointer3 != arrayPointer4)
                {
                    AoActSetFrame(~arrayPointer3, frame);
                    ++arrayPointer3;
                }
            }
        }


        //#if !_IPHONE
        // ランキング切り替え
        dmStgSlctInputChangeEvtRanking(main_work);
        //#endif //!_IPHONE

        //#if _IPHONE
        //アクト枠クリック
        uint act_num = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1];
        if (3 < act_num)
        {
            //アクト数が3を超える場合は3でリミットさせる(タッチ領域がモード切り換えのタッチと被る為)
            act_num = 3;
        }
        for (uint i = (uint)main_work.focus_disp_no, max = i + act_num; i < max; ++i)
        {
            var act = main_work.trg_act[i];
            if (act.GetState(0)[er.CTrgState.EState.Down])
            {
                main_work.cur_stage = (int)(src + i);
                main_work.is_disp_cover = true;
            }
            if (act.GetState(0)[er.CTrgState.EState.Out])
            {
                main_work.is_disp_cover = false;
            }
            if (act.GetState(0)[er.CTrgState.EState.Click])
            {
                main_work.cur_stage = (int)(src + i);
                main_work.is_disp_cover = true;
                main_work.flag |= DMD_STGSLCT_FLAG_DECIDE;
                return;
            }
        }
        //ゾーン移動用変数
        bool is_move_left = false;
        bool is_move_right = false;
        bool is_move_drag = false;
        ref int is_open = ref main_work.is_final_open;
        //ゾーンタブスタンド
        for (int i = 0, max = arrayof(main_work.trg_act_tab); i < max; ++i)
        {
            int zone_index = i;
            if ((DMD_STGSLCT_ZONE_SPECIAL_OPEN & is_open) != 0 && 0 == (DMD_STGSLCT_ZONE_FINAL_OPEN & is_open))
            {
                // FINALが開いてない状態でスペステが開いた状態の場合の補正処理
                switch (i)
                {
                    case (int)DME_STGSLCT_ZONE_TYPE_FINAL: //ファイナル
                                                           //ファイナルの当たりはスペステとして判定する
                        zone_index = (int)DME_STGSLCT_ZONE_TYPE_SPE;
                        break;
                    case (int)DME_STGSLCT_ZONE_TYPE_SPE: //スペステ
                                                         //スペステの当たり判定を取らない
                        continue;
                        break;
                    default:
                        break;
                }
            }
            var act = main_work.trg_act_tab[i];
            if (act.GetState(0)[er.CTrgState.EState.Down])
            {
                if (main_work.cur_zone != zone_index)
                {
                    main_work.chng_zone = main_work.cur_zone;
                    main_work.cur_zone = (uint)zone_index;
                    if ((main_work.chng_zone < main_work.cur_zone))
                    {
                        (is_move_right) = true;
                    }
                    else
                    {
                        (is_move_left) = true;
                    }
                }
            }
        }
        //ゾーンLRボタンリピート
        for (int i = 0, max = arrayof(main_work.trg_act_lr); i < max; ++i)
        {
            er.CTrgAoAction lr = main_work.trg_act_lr[i];

            if (!main_work.trg_act_move.GetState(0)[er.CTrgState.EState.DragAndDrop]
                                        && lr.GetState(0)[er.CTrgState.EState.Repeat]
                                        && lr.GetState(0)[er.CTrgState.EState.Lock])
            {
                main_work.chng_zone = main_work.cur_zone;
                main_work.cur_zone = (uint)dmStgSlctGetRevisedZoneNo((int)main_work.cur_zone
                                                                    , ((0 == i) ? -1 : 1)
                                                                    , DMD_STGSLCT_ZONE_FINAL_OPEN & is_open
                                                                    , DMD_STGSLCT_ZONE_SPECIAL_OPEN & is_open
                                                                    );
                if ((0 == i))
                {
                    (is_move_left) = true;
                }
                else
                {
                    (is_move_right) = true;
                }
            }
        }
        //ムーブエリア
        {
            CTrgStateEx state = main_work.trg_act_move.GetState(0U);
            CArray2<float> dragSpeed = state.GetDragSpeed();
            bool flag1 = false;
            if (3.0 <= Math.Abs(dragSpeed.x) && Math.Abs(dragSpeed.y) < (double)Math.Abs(dragSpeed.x))
            {
                if ((0 <= dragSpeed.x))
                {
                    (is_move_left) = true;
                }
                else
                {
                    (is_move_right) = true;
                }
                main_work.chng_zone = main_work.cur_zone;
                main_work.cur_zone = (uint)dmStgSlctGetRevisedZoneNo((int)main_work.cur_zone
                                                                    , ((is_move_left) ? 1 : -1)
                                                                    , DMD_STGSLCT_ZONE_FINAL_OPEN & is_open
                                                                    , DMD_STGSLCT_ZONE_SPECIAL_OPEN & is_open);
            }
            else if (state[er.CTrgState.EState.Up])
            {
                //上下フリック
                if (state[er.CTrgState.EState.DragAndDrop])
                {
                    //ドラッグ中なら

                    //上にフリックするとカーソルは下に動く
                    int move_min = 0;
                    int move_max;
                    switch (main_work.cur_zone)
                    {
                        case (uint)DME_STGSLCT_ZONE_TYPE_FINAL: //ファイナルステージ
                            move_max = 0;
                            break;
                        case (uint)DME_STGSLCT_ZONE_TYPE_SPE: //スペシャルステージ
                            move_max = 4;
                            break;
                        default:
                            move_max = 1;
                            break;
                    }
                    int focus_disp_no = (int)((dm_stgslct_act_disp_y_pos_tbl[0]) - main_work.act_top_pos_y[src] + (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8) / 2);
                    focus_disp_no -= (int)(dragSpeed.y * (float)(DMD_STGSLCT_ACT_TABLE_DIST_Y - 8) * 0.25f);
                    focus_disp_no /= (int)(DMD_STGSLCT_ACT_TABLE_DIST_Y - 8);
                    focus_disp_no = (int)amClamp(focus_disp_no, move_min, move_max);
                    main_work.prev_disp_no = main_work.focus_disp_no;
                    main_work.focus_disp_no = focus_disp_no;
                    main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL;
                }
            }
            else if (state[er.CTrgState.EState.Move])
            {
                //上下ドラッグ
                IntPair move = state.GetLastMove();
                if ((move.second < 0) || (0 < move.second))
                {
                    for (int i = (int)src; i < dst; ++i)
                    {
                        main_work.act_top_pos_y[i] += move.second * 2;
                    }
                    is_move_drag = true;
                }
            }
        }
        //移動が有ると、アクト決定が発動しない様にトリガのロックを切る
        if (is_move_left || is_move_right || is_move_drag || (DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL & main_work.flag) != 0)
        {
            int index2 = 0;
            for (int index4 = arrayof(main_work.trg_act); index2 < index4; ++index2)
                main_work.trg_act[index2].DelLock();

            main_work.is_disp_cover = false;
        }

        // 十字キー操作
#if true
        if (AoPadMRepeat().HasFlag(ControllerConsts.UP))
        {
            if (AoPadMStand().HasFlag(ControllerConsts.UP)
                || main_work.cur_stage != (int)src)
            {
#else //!_IPHONE
        if (false)
        {
            {
#endif //!_IPHONE

                if (main_work.cur_zone == (uint)DME_STGSLCT_ACT_PAGE_FINAL)
                {
                    return;
                }

                // 選択STAGE切り替え
                if (main_work.crsr_idx == 0)
                {
                    if (act_comp > DMD_STGSLCT_ACT_VRTCL_CHNG_NUM)
                    {
                        // テーブル移動演出フラグON
                        main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL;
                    }

                    else
                    {
                        // カーソル移動フラグON
                        main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_CRSR;
                    }
                }

                else
                {
                    // カーソル移動フラグON
                    main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_CRSR;
                }


                main_work.flag |= DMD_STGSLCT_FLAG_UP_CHNG_CRSR;

            }

            return;
        }

#if true
        else if (AoPadMRepeat().HasFlag(ControllerConsts.DOWN))
        {
            if (AoPadMStand().HasFlag(ControllerConsts.DOWN)
                || main_work.cur_stage != (int)(dst - 1))
            {
#else //false
        else if (false)
        {
            {
#endif //false

                if (main_work.cur_zone == (uint)DME_STGSLCT_ACT_PAGE_FINAL)
                {
                    return;
                }

                // 選択STAGE切り替え
                if (main_work.crsr_idx == 3)
                {
                    if (act_comp > DMD_STGSLCT_ACT_VRTCL_CHNG_NUM)
                    {
                        // テーブル移動演出フラグON
                        main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL;
                    }
                    else
                    {
                        // カーソル移動フラグON
                        main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_CRSR;
                    }
                }

                else
                {
                    // カーソル移動フラグON
                    main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_CRSR;
                }

                main_work.flag |= DMD_STGSLCT_FLAG_DOWN_CHNG_CRSR;
            }

            return;
        }

#if true
#if !_WII
        else if ((AoPadMRepeat().HasFlag(ControllerConsts.LEFT)) || (AoPadMRepeat().HasFlag(ControllerConsts.L)))
        {
            if (((AoPadMStand().HasFlag(ControllerConsts.LEFT)) || (AoPadMStand().HasFlag(ControllerConsts.L)))
#else
	else if (AoPadMRepeat() & GSD_KEY_LEFT) {
		if (AoPadMStand() & GSD_KEY_LEFT
#endif
                || main_work.cur_zone != (uint)DME_STGSLCT_ACT_PAGE_1)
            {
                // 選択ZONE切り替え
                main_work.chng_zone = main_work.cur_zone;
                main_work.cur_zone = (uint)dmStgSlctGetRevisedZoneNo((int)main_work.cur_zone
                                                                    , -1
                                                                    , is_final_open
                                                                    , is_spe_open
                                                                    );
#else //false
        else if (is_move_left)
        {
            {
#endif //false

                src = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
                dst = src + dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1];

                main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_ZONE;
                main_work.act_move_dest[0] = DMD_STGSLCT_STAGE_TAB_NODISP_POS_X;
                main_work.act_move_dest[1] = DMD_STGSLCT_ACT_TABLE_TOP_POS_X;

                // 選択中のステージ切り替え
                if (main_work.cur_zone == (uint)DME_STGSLCT_ACT_PAGE_FINAL)
                {
                    main_work.crsr_pos_y = 320.0f;
                    main_work.crsr_idx = 0;
                }

                if (main_work.chng_zone == (uint)DME_STGSLCT_ACT_PAGE_FINAL)
                {
                    main_work.crsr_pos_y = 160.0f;
                    main_work.crsr_idx = 0;
                }

                main_work.prev_disp_no = main_work.focus_disp_no;
                main_work.focus_disp_no = 0;

                main_work.cur_stage = (int)(main_work.crsr_idx + main_work.focus_disp_no + src);

                // 移動時の座標設定
                dmStgSlctSetActChngZonePosInit(main_work, -1);

                DmSoundPlaySE("Cursol");

                if (AoPad.AoPadMRepeat().HasFlag(ControllerConsts.L)
                    || AoPad.AoPadMStand().HasFlag(ControllerConsts.L))
                {
                    main_work.flag |= DMD_STGSLCT_FLAG_ACT_PUSH_L_BTN;
                    main_work.btn_l_disp_frm = 0;
                }


                main_work.timer = 0;
            }

            return;
        }


#if true
#if !_WII
        else if ((AoPadMRepeat().HasFlag(ControllerConsts.RIGHT)) || (AoPadMRepeat().HasFlag(ControllerConsts.R)))
        {
            if (((AoPadMStand().HasFlag(ControllerConsts.RIGHT)) || (AoPadMStand().HasFlag(ControllerConsts.R)))
#else
	else if (AoPadMRepeat() & GSD_KEY_RIGHT) {
		if (AoPadMStand() & GSD_KEY_RIGHT
#endif
                || main_work.cur_zone != (uint)edge)
            {
                // 選択ZONE切り替え
                main_work.chng_zone = main_work.cur_zone;
                main_work.cur_zone = (uint)dmStgSlctGetRevisedZoneNo((int)main_work.cur_zone
                                                                    , 1
                                                                    , is_final_open
                                                                    , is_spe_open
                                                                    );
#else //false
        else if (is_move_right)
        {
            {
                // 選択ZONE切り替え
#endif //false

                src = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
                dst = src + dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1];

                main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_ZONE;
                main_work.act_move_dest[0] = DMD_STGSLCT_STAGE_TAB_NODISP_POS_X * (-1);
                main_work.act_move_dest[1] = DMD_STGSLCT_ACT_TABLE_TOP_POS_X;

                // 選択中のステージ切り替え
                if (main_work.cur_zone == (uint)DME_STGSLCT_ACT_PAGE_FINAL)
                {
                    main_work.crsr_pos_y = 320.0f;
                    main_work.crsr_idx = 0;
                }

                if (main_work.chng_zone == (uint)DME_STGSLCT_ACT_PAGE_FINAL)
                {
                    main_work.crsr_pos_y = 160.0f;
                    main_work.crsr_idx = 0;
                }

                main_work.prev_disp_no = main_work.focus_disp_no;
                main_work.focus_disp_no = 0;

                main_work.cur_stage = (int)(main_work.crsr_idx + main_work.focus_disp_no + src);

                // 移動時の座標設定
                dmStgSlctSetActChngZonePosInit(main_work, 1);

                DmSoundPlaySE("Cursol");

                if (AoPad.AoPadMRepeat().HasFlag(ControllerConsts.R)
                    || AoPad.AoPadMStand().HasFlag(ControllerConsts.R))
                {
                    main_work.flag |= DMD_STGSLCT_FLAG_ACT_PUSH_R_BTN;
                    main_work.btn_r_disp_frm = 0;
                }


                main_work.timer = 0;
            }

            return;
        }
    }



    // ==========================================================================
    // dmStgSlctInputProcStageSelectChngZone
    /*!
        STAGE選択用入力プロシージャ処理(据え置き版)
     */
    // ==========================================================================
    private static void dmStgSlctInputProcStageSelectChngZone(DMS_STGSLCT_MAIN_WORK main_work)
    {
        uint src = 0;
        uint dst = 0;
        uint act_comp = 0;
        int is_final_open = 0;
        int is_spe_open = 0;
        int edge = 0;

        src = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
        dst = src + dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1];
        act_comp = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] - 1;

        edge = (int)DME_STGSLCT_ACT_PAGE_4;

        // FINALオープン状態かどうか
        if (main_work.is_clear_stage[(int)DME_STGSLCT_STAGE_1_B] == 1
            && main_work.is_clear_stage[(int)DME_STGSLCT_STAGE_2_B] == 1
            && main_work.is_clear_stage[(int)DME_STGSLCT_STAGE_3_B] == 1
            && main_work.is_clear_stage[(int)DME_STGSLCT_STAGE_4_B] == 1)
        {
            is_final_open = 1;
            edge = (int)DME_STGSLCT_ACT_PAGE_FINAL;
        }

        // スペステオープン状態かどうか
        for (uint i = (uint)DME_STGSLCT_STAGE_S_1; i < (uint)DME_STGSLCT_STAGE_NUM; i++)
        {
            if (main_work.is_clear_stage[i] != -1)
            {
                is_spe_open = 1;
                edge = (int)DME_STGSLCT_ACT_PAGE_SPE;
                break;
            }
            else
            {
                is_spe_open = 0;
            }
        }

        // キャンセル処理
        if (AoPad.AoPadStand().HasFlag(ControllerConsts.CANCEL))
        {
            main_work.flag |= DMD_STGSLCT_FLAG_CANCEL;

            return;
        }

        // ステージ決定処理
        if (AoPad.AoPadStand().HasFlag(ControllerConsts.CONFIRM))
        {
            main_work.flag |= DMD_STGSLCT_FLAG_DECIDE;

            return;
        }

        // ゲームモード切替
#if true
        if (AoPad.AoPadStand().HasFlag(ControllerConsts.ALT_LEFT))
        {
            main_work.cur_game_mode ^= (uint)DME_STGSLCT_PLAY_MODE_TIME_ATK;

            // ゲームモードテキストアクションのフレーム初期化(アニメーション開始)
            main_work.mode_tex_frm = 1;

            DmSoundPlaySE("Cursol");

            return;
        }
#endif

        // ランキング切り替え
        dmStgSlctInputChangeEvtRanking(main_work);


#if !_WII
        if ((AoPad.AoPadMRepeat().HasFlag(ControllerConsts.LEFT)) || (AoPad.AoPadMRepeat().HasFlag(ControllerConsts.L)))
        {
            if (((AoPad.AoPadMStand().HasFlag(ControllerConsts.LEFT)) || (AoPad.AoPadMStand().HasFlag(ControllerConsts.L)))
#else
	if (AoPadMRepeat() & GSD_KEY_LEFT) {
		if (AoPadMStand() & GSD_KEY_LEFT
#endif

                || main_work.cur_zone != (uint)DME_STGSLCT_ACT_PAGE_1)
            {
                // 選択ZONE切り替え
                main_work.chng_zone = main_work.cur_zone;
                main_work.cur_zone = (uint)dmStgSlctGetRevisedZoneNo((int)main_work.cur_zone
                                                                    , -1
                                                                    , is_final_open
                                                                    , is_spe_open
                                                                    );

                src = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
                dst = src + dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1];

                main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_ZONE;
                main_work.act_move_dest[0] = DMD_STGSLCT_STAGE_TAB_NODISP_POS_X;
                main_work.act_move_dest[1] = DMD_STGSLCT_ACT_TABLE_TOP_POS_X;

                // 選択中のステージ切り替え
                if (main_work.cur_zone == (uint)DME_STGSLCT_ACT_PAGE_FINAL)
                {
                    main_work.crsr_pos_y = 320.0f;
                    main_work.crsr_idx = 0;
                }

                if (main_work.chng_zone == (uint)DME_STGSLCT_ACT_PAGE_FINAL)
                {
                    main_work.crsr_pos_y = 160.0f;
                    main_work.crsr_idx = 0;
                }

                main_work.prev_disp_no = main_work.focus_disp_no;
                main_work.focus_disp_no = 0;

                main_work.cur_stage = (int)(main_work.crsr_idx + main_work.focus_disp_no + src);

                // 移動時の座標設定
                dmStgSlctSetActChngZonePosInit(main_work, -1);

                DmSoundPlaySE("Cursol");

                if (AoPad.AoPadMRepeat().HasFlag(ControllerConsts.L)
                    || AoPad.AoPadMStand().HasFlag(ControllerConsts.L))
                {
                    main_work.flag |= DMD_STGSLCT_FLAG_ACT_PUSH_L_BTN;
                    main_work.btn_l_disp_frm = 0;
                }

                main_work.timer = 0;
            }

            return;
        }


#if !_WII
        else if ((AoPad.AoPadMRepeat().HasFlag(ControllerConsts.RIGHT)) || (AoPad.AoPadMRepeat().HasFlag(ControllerConsts.R)))
        {
            if (((AoPad.AoPadMStand().HasFlag(ControllerConsts.RIGHT)) || (AoPad.AoPadMStand().HasFlag(ControllerConsts.R)))
#else
	else if (AoPadMRepeat() & GSD_KEY_RIGHT) {
		if (AoPadMStand() & GSD_KEY_RIGHT
#endif
                || main_work.cur_zone != (uint)edge)
            {
                // 選択ZONE切り替え
                main_work.chng_zone = main_work.cur_zone;
                main_work.cur_zone = (uint)dmStgSlctGetRevisedZoneNo((int)main_work.cur_zone
                                                                    , 1
                                                                    , is_final_open
                                                                    , is_spe_open
                                                                    );

                src = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
                dst = src + dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1];

                main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_ZONE;
                main_work.act_move_dest[0] = DMD_STGSLCT_STAGE_TAB_NODISP_POS_X * (-1);
                main_work.act_move_dest[1] = DMD_STGSLCT_ACT_TABLE_TOP_POS_X;

                // 選択中のステージ切り替え
                if (main_work.cur_zone == (uint)DME_STGSLCT_ACT_PAGE_FINAL)
                {
                    main_work.crsr_pos_y = 320.0f;
                    main_work.crsr_idx = 0;
                }

                if (main_work.chng_zone == (uint)DME_STGSLCT_ACT_PAGE_FINAL)
                {
                    main_work.crsr_pos_y = 160.0f;
                    main_work.crsr_idx = 0;
                }

                main_work.prev_disp_no = main_work.focus_disp_no;
                main_work.focus_disp_no = 0;

                main_work.cur_stage = (int)(main_work.crsr_idx + main_work.focus_disp_no + src);

                // 移動時の座標設定
                dmStgSlctSetActChngZonePosInit(main_work, 1);

                DmSoundPlaySE("Cursol");

                if (AoPad.AoPadMRepeat().HasFlag(ControllerConsts.R)
                    || AoPad.AoPadMStand().HasFlag(ControllerConsts.R))
                {
                    main_work.flag |= DMD_STGSLCT_FLAG_ACT_PUSH_R_BTN;
                    main_work.btn_r_disp_frm = 0;
                }

                main_work.timer = 0;
            }

            return;
        }
    }




    // ==========================================================================
    // dmStgSlctInputProcStageSelectMove
    /*!
        STAGE選択用入力プロシージャ処理(据え置き版)
     */
    // ==========================================================================
    private static void dmStgSlctInputProcStageSelectMove(DMS_STGSLCT_MAIN_WORK main_work)
    {
        uint src = 0;
        uint dst = 0;
        uint act_comp = 0;

        src = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
        dst = src + dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1];
        act_comp = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] - 1;

        // キャンセル処理
        if (AoPad.AoPadStand().HasFlag(ControllerConsts.CANCEL))
        {
            main_work.flag |= DMD_STGSLCT_FLAG_CANCEL;

            return;
        }

        // ステージ決定処理
        if (AoPad.AoPadStand().HasFlag(ControllerConsts.CONFIRM))
        {
            main_work.flag |= DMD_STGSLCT_FLAG_DECIDE;

            return;
        }

        // ゲームモード切替
#if true
        if (AoPad.AoPadStand().HasFlag(ControllerConsts.ALT_LEFT))
        {
            main_work.cur_game_mode ^= (uint)DME_STGSLCT_PLAY_MODE_TIME_ATK;

            // ゲームモードテキストアクションのフレーム初期化(アニメーション開始)
            main_work.mode_tex_frm = 1;

            DmSoundPlaySE("Cursol");

            return;
        }
#endif

        // ランキング切り替え
        dmStgSlctInputChangeEvtRanking(main_work);


        // 十字キー操作
        if (AoPad.AoPadMRepeat().HasFlag(ControllerConsts.UP))
        {
            if (AoPad.AoPadMStand().HasFlag(ControllerConsts.UP)
                || main_work.cur_stage != (int)src)
            {

                if (main_work.cur_zone == (uint)DME_STGSLCT_ACT_PAGE_FINAL)
                {
                    return;
                }

                // 選択STAGE切り替え
                if (main_work.crsr_idx == 0)
                {
                    if (act_comp > DMD_STGSLCT_ACT_VRTCL_CHNG_NUM)
                    {
                        // テーブル移動演出フラグON
                        main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL;
                    }

                    else
                    {
                        // カーソル移動フラグON
                        main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_CRSR;
                    }
                }

                else
                {
                    // カーソル移動フラグON
                    main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_CRSR;
                }


                main_work.flag |= DMD_STGSLCT_FLAG_UP_CHNG_CRSR;

            }

            return;
        }

        else if (AoPad.AoPadMRepeat().HasFlag(ControllerConsts.DOWN))
        {
            if (AoPad.AoPadMStand().HasFlag(ControllerConsts.DOWN)
                || main_work.cur_stage != (int)(dst - 1))
            {

                if (main_work.cur_zone == (uint)DME_STGSLCT_ACT_PAGE_FINAL)
                {
                    return;
                }

                // 選択STAGE切り替え
                if (main_work.crsr_idx == 3)
                {
                    if (act_comp > DMD_STGSLCT_ACT_VRTCL_CHNG_NUM)
                    {
                        // テーブル移動演出フラグON
                        main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL;
                    }
                    else
                    {
                        // カーソル移動フラグON
                        main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_CRSR;
                    }
                }

                else
                {
                    // カーソル移動フラグON
                    main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_CRSR;
                }

                main_work.flag |= DMD_STGSLCT_FLAG_DOWN_CHNG_CRSR;

            }

            return;
        }
    }




#if true
    // ==========================================================================
    // dmStgSlctInputChangeEvtRanking
    /*!
        ウインドウ非表示用入力プロシージャ処理(据え置き版)
     */
    // ==========================================================================
    private static void dmStgSlctInputChangeEvtRanking(DMS_STGSLCT_MAIN_WORK main_work)
    {
        // メニュー遷移フラグ処理
#if !_WII
        if (AoPad.AoPadStand().HasFlag(ControllerConsts.ALT_UP))
        {
            main_work.flag |= DMD_STGSLCT_FLAG_CHANGE_EVT_RANKING;

            return;
        }
#else
	// Wii版は処理なし
	UNREFERENCED_PARAMETER(main_work);
#endif


    }
#endif


    // ==========================================================================
    // dmStgSlctInputProcWinDispIdle
    /*!
        ウインドウ表示用入力プロシージャ処理(据え置き版)
     */
    // ==========================================================================
    private static void dmStgSlctInputProcWinDispIdle(DMS_STGSLCT_MAIN_WORK main_work)
    {
        // キャンセル処理
        if (AoPadStand().HasFlag(ControllerConsts.CANCEL))
        {
            main_work.flag |= DMD_STGSLCT_FLAG_CANCEL;

            return;
        }

        // 決定処理
        if (AoPadStand().HasFlag(ControllerConsts.DECIDE))
        {
            if (main_work.win_cur_slct == 0
                || main_work.win_mode == (int)DME_STGSLCT_WIN_MENU)
            {
                main_work.flag |= DMD_STGSLCT_FLAG_DECIDE;
            }
            else
            {
                main_work.flag |= DMD_STGSLCT_FLAG_CANCEL;
            }

            return;
        }


        if (main_work.win_mode == (int)DME_STGSLCT_WIN_MENU)
        {
            // 十字キー操作
            if (AoPadMStand().HasFlag(ControllerConsts.UP))
            {
                if (main_work.win_cur_slct != 0)
                {
                    DmSoundPlaySE("Cursol");
                }

                // 選択項目切り替え
                main_work.win_cur_slct = 0;

                // 切り替えフラグ？
            }
            else if (AoPadMStand().HasFlag(ControllerConsts.DOWN))
            {
                if (main_work.win_cur_slct != 1)
                {
                    DmSoundPlaySE("Cursol");
                }

                // 選択項目切り替え
                main_work.win_cur_slct = 1;
            }
        }


        if (main_work.win_mode == (int)DME_STGSLCT_WIN_STG_SLCT)
        {
            if (AoPadMStand().HasFlag(ControllerConsts.LEFT))
            {
                if (main_work.win_cur_slct != 0)
                {
                    DmSoundPlaySE("Cursol");
                }

                // 選択項目切り替え
                main_work.win_cur_slct = 0;
            }
            else if (AoPadMStand().HasFlag(ControllerConsts.RIGHT))
            {
                if (main_work.win_cur_slct != 1)
                {
                    DmSoundPlaySE("Cursol");
                }

                // 選択項目切り替え
                main_work.win_cur_slct = 1;
            }
        }

        if (main_work.win_mode == (int)DME_STGSLCT_WIN_STG_SLCT)
        {
            //選択肢有り
            for (int i = 0, max = (ACT_TEX_NO - ACT_TEX_YES + 1); i < max; ++i)
            {
                er.CTrgAoAction answer;
                //AOS_ACTION act, act_end;
                int act_idx = 0, act_end_idx = 0;
                int win_cur_slct = 0;
                uint flag = 0;
                switch (ACT_TEX_YES + i)
                {
                    case ACT_TEX_NO:
                        answer = main_work.trg_answer[0];
                        //act = main_work.act[(int)ACT_NO_BTN_L];
                        //act_end = main_work.act[(int)(ACT_NO_BTN_R + 1)];
                        act_idx = (int)ACT_NO_BTN_L;
                        act_end_idx = (int)(ACT_NO_BTN_R + 1);
                        win_cur_slct = 1;
                        flag = DMD_STGSLCT_FLAG_CANCEL;
                        break;
                    case ACT_TEX_YES:
                        answer = main_work.trg_answer[1];
                        //act = main_work.act[(int)ACT_YES_BTN_L];
                        //act_end = main_work.act[(int)(ACT_YES_BTN_R + 1)];
                        act_idx = (int)ACT_YES_BTN_L;
                        act_end_idx = (int)(ACT_YES_BTN_R + 1);
                        win_cur_slct = 0;
                        flag = DMD_STGSLCT_FLAG_DECIDE;
                        break;
                    default:
                        answer = null;
                        break;
                }
                float frame;
                if (answer.GetState(0)[er.CTrgState.EState.Up] && answer.GetState(0)[er.CTrgState.EState.Prev])
                {
                    frame = 2.0f;
                    main_work.win_cur_slct = win_cur_slct;
                    main_work.flag |= flag;
                }
                else if (answer.GetState(0)[er.CTrgState.EState.On])
                {
                    frame = 1.0f;
                }
                else if (2.0f <= (main_work.act[act_idx]).frame)
                {
                    frame = (main_work.act[act_idx]).frame;
                }
                else
                {
                    frame = 0.0f;
                }
                while (act_idx != act_end_idx)
                {
                    AoActSetFrame((main_work.act[act_idx]), frame);
                    AoActUpdate((main_work.act[act_idx]), 0.0f);
                    ++act_idx;
                }
            }
        }
        else
        {
            //選択肢無し
            if (amTpIsTouchPush(0))
            {
                main_work.flag |= DMD_STGSLCT_FLAG_CANCEL;
            }
        }
    }



    // ==========================================================================
    // dmStgSlctProcActDraw
    /*!
        描画設定プロシージャ処理
     */
    // ==========================================================================
    private static void dmStgSlctProcActDraw(DMS_STGSLCT_MAIN_WORK main_work)
    {
        // 帯移動演出用		※常に移動しつづけるのでここに配置
        dmStgSlctSetObiEfctPos(main_work);

        // 共通描画処理は描画時は常に設定
        dmStgSlctCommonDraw(main_work);

        // ZONE選択部分描画設定
        dmStgSlctZoneSelectDraw(main_work);

        if (main_work.state == (int)DME_STGSLCT_MODE_STATE_ACT_SLCT)
        {
            dmStgSlctStageSelectDraw(main_work);
        }

        // FIX部分描画設定
        dmStgSlctCommonFixDraw(main_work);

        // アクションを使用せず、テクスチャを描画(アルファのグラデーション使用のため)
        dmStgSlctMakeVertexAct(main_work, main_work.up_bg_vrtx);
    }


    // ==========================================================================
    // dmStgSlctCommonDraw
    /*!
        共通描画設定処理
     */
    // ==========================================================================
    private static void dmStgSlctCommonDraw(DMS_STGSLCT_MAIN_WORK main_work)
    {
        // ゾーンテーブル用AO描画プライオリティ設定
        AoActSysSetDrawTaskPrio(DMD_STGSLCT_DRAW_PRIO_BG);

        // アクション更新
        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));

#if _IPHONE
        if (uint.MaxValue == main_work.cur_bg_id)
        {
            //ゾーンセレクト用特別背景なら透明にする
            AoActSortRegAction(main_work.act[(int)ACT_WAVE_BG]);
            AoActAcmPush();
            AoActAcmInit();
            AoActAcmApplyFade(main_work.bg_fade);
            AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[0]));
            AoActUpdate(main_work.act[(int)ACT_WAVE_BG], 1.0f);
            AoActAcmPop();
        }
        else //↓に繋げる
#endif //_IPHONE
            for (int i = 0; i < 4; i++)
            {
                AoActSortRegAction(main_work.act[(int)(ACT_ZONE_BG_LT + i)]);

                AoActSetFrame(main_work.act[(int)(ACT_ZONE_BG_LT + i)], (float)main_work.cur_bg_id);
                //		AoActSetFrame(main_work.act[ACT_ZONE_BG_LT + i], (float)main_work.cur_zone);

                AoActAcmPush();

                AoActAcmInit();

                AoActAcmApplyFade(main_work.bg_fade);

                AoActUpdate(main_work.act[(int)(ACT_ZONE_BG_LT + i)], 0.0f);

                AoActAcmPop();
            }

#if _IPHONE
        if (_am_sample_draw_enable)
        {
#endif //_IPHONE
            // ソート実行
            AoActSortExecute();

            // ソートバッファ描画
            AoActSortDraw();
#if _IPHONE
        }
#endif //_IPHONE

        // ソートバッファ全解除
        AoActSortUnregAll();


        // ゾーンテーブル用AO描画プライオリティ設定
        AoActSysSetDrawTaskPrio(0x2c00);

        // アクション登録(登録は全てここで行うようにし、実際の登録するかはフラグにて設定するようにする)
        for (int i = (int)ACT_DOWN_BG; i <= (int)ACT_BLUE_BG; i++)
        {
            AoActSortRegAction(main_work.act[i]);
        }

        // アクション更新
        AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[0]));

        for (int i = (int)ACT_DOWN_BG; i <= (int)ACT_BLUE_BG; i++)
        {
            AoActUpdate(main_work.act[i], 0.0f);
        }

#if _IPHONE
        if (_am_sample_draw_enable)
        {
#endif //_IPHONE
            // ソート実行
            AoActSortExecute();

            // ソートバッファ描画
            AoActSortDraw();
#if _IPHONE
        }
#endif //_IPHONE

        // ソートバッファ全解除
        AoActSortUnregAll();

    }



    // ==========================================================================
    // dmStgSlctCommonFixDraw
    /*!
        共通描画設定処理
     */
    // ==========================================================================
    private static void dmStgSlctCommonFixDraw(DMS_STGSLCT_MAIN_WORK main_work)
    {
        uint open_zone_num = 0;
        float tmp_disp_dist = 0.0f;

        // ゾーンテーブル用AO描画プライオリティ設定
        AoActSysSetDrawTaskPrio(DMD_STGSLCT_DRAW_PRIO_FIX);

        // アクション更新
        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));

        // アクション登録(登録は全てここで行うようにし、実際の登録するかはフラグにて設定するようにする)
        for (int i = (int)ACT_TAB_MODE_L; i <= (int)ACT_TAB_EMER; i++)
        {
            AoActSortRegAction(main_work.act[i]);
        }

        for (int i = (int)ACT_ICON_EMER_1; i <= (int)ACT_ICON_EMER_7; i++)
        {
            if ((main_work.get_emerald & 1 << (i - (int)ACT_ICON_EMER_1)) != 0)
            {
                AoActSortRegAction(main_work.act[i]);
            }
        }

        // アクション更新
        AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[1]));

#if !_IPHONE //iPhoneではボタン類を描画しない
        // ウインドウ表示中は非表示
        if (!main_work.win_size_rate[0] && !main_work.win_size_rate[1])
        {
            AoActSortRegAction(main_work.act[ACT_BTN_CANCEL1]);
        }

        if (main_work.mode_tex_move_frm == 5)
        {
            AoActSortRegAction(main_work.act[ACT_BTN_LB]);

#if !_WII
            AoActSortRegAction(main_work.act[ACT_BTN_MENU]);
#endif
        }

#if !_WII
        if (main_work.state == DME_STGSLCT_MODE_STATE_ACT_SLCT)
        {
            AoActSortRegAction(main_work.act[ACT_BTN_LB_ARROW]);
            AoActSortRegAction(main_work.act[ACT_BTN_RB_ARROW]);

            AoActSetFrame(main_work.act[ACT_BTN_LB_ARROW], (float)main_work.btn_l_disp_frm);
            AoActSetFrame(main_work.act[ACT_BTN_RB_ARROW], (float)main_work.btn_r_disp_frm);
        }

        // 左上・右上のボタン周り表示の演出フレーム処理
        if (main_work.btn_l_disp_frm < DMD_STGSLCT_ZONE_CHNG_BTN_END_FRM)
        {
            main_work.btn_l_disp_frm++;
        }
        else
        {
            main_work.btn_l_disp_frm = DMD_STGSLCT_ZONE_CHNG_BTN_END_FRM;
        }


        if (main_work.btn_r_disp_frm < DMD_STGSLCT_ZONE_CHNG_BTN_END_FRM)
        {
            main_work.btn_r_disp_frm++;
        }
        else
        {
            main_work.btn_r_disp_frm = DMD_STGSLCT_ZONE_CHNG_BTN_END_FRM;
        }
#endif
#endif //!_IPHONE //iPhoneではボタン類を描画しない

        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));

        if (main_work.state == (int)DME_STGSLCT_MODE_STATE_ACT_SLCT)
        {
            if (main_work.cur_game_mode != (uint)DME_STGSLCT_PLAY_MODE_TIME_ATK)
            {
                AoActSortRegAction(main_work.act[(int)ACT_TAB_STATE_L]);
                AoActSortRegAction(main_work.act[(int)ACT_TAB_STATE_C]);
                AoActSortRegAction(main_work.act[(int)ACT_TAB_STATE_R]);

#if !_IPHONE
                AoActSetFrame(main_work.act[ACT_TAB_STATE_L], (float)main_work.mode_tex_frm);
                AoActSetFrame(main_work.act[ACT_TAB_STATE_C], (float)main_work.mode_tex_frm);
                AoActSetFrame(main_work.act[ACT_TAB_STATE_R], (float)main_work.mode_tex_frm);
#endif //!_IPHONE
            }
            else
            {
                AoActSortRegAction(main_work.act[(int)ACT_TAB_STATE_L2]);
                AoActSortRegAction(main_work.act[(int)ACT_TAB_STATE_C2]);
                AoActSortRegAction(main_work.act[(int)ACT_TAB_STATE_R2]);

#if !_IPHONE
                AoActSetFrame(main_work.act[ACT_TAB_STATE_L2], (float)main_work.mode_tex_frm);
                AoActSetFrame(main_work.act[ACT_TAB_STATE_C2], (float)main_work.mode_tex_frm);
                AoActSetFrame(main_work.act[ACT_TAB_STATE_R2], (float)main_work.mode_tex_frm);
#endif //!_IPHONE
            }
        }

#if !_IPHONE   //すでに描画している               
        // アクション更新
        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));

        // アクション登録(登録は全てここで行うようにし、実際の登録するかはフラグにて設定するようにする)
        AoActSortRegAction(main_work.act[ACT_TEX_SONIC]);
#endif //!_IPHONE	//すでに描画している

        // アクション更新
        AoActSetTexture(AoTexGetTexList(main_work.tex[1]));

        // タイムアタックの場合
        if (main_work.cur_game_mode == (uint)DME_STGSLCT_PLAY_MODE_TIME_ATK)
        {
#if !_IPHONE
            AoActSortRegAction(main_work.act[ACT_TEX_MODE_SCORE]);
            AoActSetFrame(main_work.act[ACT_TEX_MODE_SCORE], (float)main_work.mode_tex_move_frm);
#endif //!_IPHONE

            AoActSortRegAction(main_work.act[(int)ACT_TEX_BIG_TIME]);
#if !_IPHONE
            AoActSetFrame(main_work.act[ACT_TEX_BIG_TIME], (float)main_work.mode_tex_frm);
            AoActSortRegAction(main_work.act[ACT_TEX_TIME_EFCT]);
            AoActSetFrame(main_work.act[ACT_TEX_TIME_EFCT], (float)main_work.mode_tex_frm);
#endif //!_IPHONE
        }
        // スコアアタックの場合
        else
        {
#if !_IPHONE
            AoActSortRegAction(main_work.act[ACT_TEX_MODE_TIME]);
            AoActSetFrame(main_work.act[ACT_TEX_MODE_TIME], (float)main_work.mode_tex_move_frm);
#endif //!_IPHONE

            AoActSortRegAction(main_work.act[(int)ACT_TEX_BIG_SCORE]);
#if !_IPHONE
            AoActSetFrame(main_work.act[ACT_TEX_BIG_SCORE], (float)main_work.mode_tex_frm);
            AoActSortRegAction(main_work.act[ACT_TEX_SCORE_EFCT]);
            AoActSetFrame(main_work.act[ACT_TEX_SCORE_EFCT], (float)main_work.mode_tex_frm);
#endif //!_IPHONE
        }

        // モードテキストのフレーム更新
#if !_IPHONE
        if (main_work.mode_tex_frm < DMD_STGSLCT_MODE_TEX_FRAME_MAX)
        {
            main_work.mode_tex_frm++;
        }
#else //!_IPHONE
        main_work.mode_tex_frm = 0.0f;
#endif //!_IPHONE

#if !(_WII || _IPHONE)
        AoActSortRegAction(main_work.act[ACT_TEX_MENU]);
#endif //!(_WII || _IPHONE)

#if !_IPHONE
        for (int i = ACT_TEX_EMER; i < ACT_TEX_ZONE_UP_S; i++)
        {
            if (i == ACT_TEX_EMER)
            {
                AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
            }
            else
            {
                AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
            }

            AoActSortRegAction(main_work.act[i]);
        }
#endif //!_IPHONE

#if _IPHONE
        if ((uint.MaxValue == main_work.cur_bg_id) || ((int)DME_STGSLCT_MODE_STATE_ZONE_SLCT == main_work.state))
        {
            // 言語別データのゾーン名を使用

            //アクトセレクトからゾーンセレクトに戻ってくる時にエッグマンステージが表示される事が有る
            //それの対応する為にゾーンセレクトの入り演出中は強制的にこのゾーン名を使用する
            AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
            AoActSortRegAction(main_work.act[(int)ACT_TEX_ZONE_UP_S]);
        }
        else //↓に繋げる
#endif //_IPHONE
            // スペステ以外なら
            if (main_work.cur_zone != (uint)DME_STGSLCT_ZONE_TYPE_SPE)
        {
            // 共通データのゾーン名を使用
            AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
            AoActSortRegAction(main_work.act[(int)ACT_TEX_ZONE_UP]);
        }
        // スペステなら
        else
        {
            // 言語別データのゾーン名を使用
            AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
            AoActSortRegAction(main_work.act[(int)ACT_TEX_ZONE_UP_S]);
        }

        AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[4]));

        // ウインドウ表示中は非表示
        if (0 == main_work.win_size_rate[0] && 0 == main_work.win_size_rate[1])
        {
            AoActSortRegAction(main_work.act[(int)ACT_TEX_FIX_BACK]);
#if _IPHONE
            AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[3]));
            for (int i = (int)ACT_BACK_BTN01_L; i <= (int)ACT_BACK_BTN01_R; i++)
            {
                AoActSortRegAction(main_work.act[i]);
            }
#endif //_IPHONE
        }

        // 残機設定部分
        dmStgSlctSetSonicStockDispFrame(main_work);


        if (main_work.is_jp_region)
        {
            AoActSetFrame(main_work.act[(int)ACT_BTN_CANCEL1], (float)0.0f);
        }
        else
        {
            AoActSetFrame(main_work.act[(int)ACT_BTN_CANCEL1], (float)1.0f);
        }

#if !_IPHONE
        AoActSetFrame(main_work.act[ACT_TEX_MENU], (float)main_work.mode_tex_move_frm);
#endif //!_IPHONE

        AoActSetFrame(main_work.act[(int)ACT_TEX_ZONE_UP], (float)main_work.cur_zone);
#if _IPHONE
        {
            float frame = 0.0f;
            if ((uint.MaxValue == main_work.cur_bg_id) || ((int)DME_STGSLCT_MODE_STATE_ZONE_SLCT == main_work.state))
            {
                // 言語別データのゾーン名を使用

                //アクトセレクトからゾーンセレクトに戻ってくる時にエッグマンステージが表示される事が有る
                //それの対応する為にゾーンセレクトの入り演出中は強制的にこのゾーン名を使用する
                frame = 1.0f;
            }
            AoActSetFrame(main_work.act[(int)ACT_TEX_ZONE_UP_S], frame);
        }
#endif //_IPHONE


        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));

        // アクション登録(登録は全てここで行うようにし、実際の登録するかはフラグにて設定するようにする)
        for (int i = (int)ACT_TAB_MODE_L; i <= (int)ACT_REST_NUM_1; i++)
        {
            AoActUpdate(main_work.act[i], 0.0f);
        }


        AoActAcmPush();

        for (int i = (int)ACT_TAB_EMER; i <= (int)ACT_ICON_EMER_7; i++)
        {

            AoActAcmInit();
            AoActAcmApplyTrans(0.0f
                               , main_work.chaos_eme_pos_y
                               , 0.0f
                               );

            AoActUpdate(main_work.act[i], 0.0f);
        }

        AoActAcmPop();


        // ボタン
#if !_IPHONE //iPhoneではボタン類を描画しない
        AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[1]));

        for (int i = ACT_BTN_CANCEL1; i <= ACT_BTN_MENU; i++)
        {
            AoActUpdate(main_work.act[i], 0.0f);
        }

#if !_WII
        AoActUpdate(main_work.act[ACT_BTN_LB_ARROW], 0.0f);
        AoActUpdate(main_work.act[ACT_BTN_RB_ARROW], 0.0f);
#endif
#endif //!_IPHONE //iPhoneではボタン類を描画しない

        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
        AoActAcmPush();

#if _IPHONE
        bool is_disp_act_enter_window = false; //アクト決定ウインドウの表示中か
        if ((null != main_work.proc_win_update) && (dmStgSlctProcWindowNodispIdle != main_work.proc_win_update))
        {
            //ウインドウ表示中
            is_disp_act_enter_window = true;
        }
        if (main_work.trg_act_move.GetState(0)[er.CTrgState.EState.DragAndDrop] || is_disp_act_enter_window
                || (!main_work.trg_mode[0].GetState(0)[er.CTrgState.EState.Lock] && !main_work.trg_mode[1].GetState(0)[er.CTrgState.EState.Lock]))
        {
            //アクト決定ウインドウが表示されていたら、もしくはモード切り換えがロックされていなかったら
            //更新
            AoActUpdate(main_work.act[(int)ACT_TAB_STATE_MOVE]);
        }
        float[] tab_state_move_pos = [
            main_work.act[(int)ACT_TAB_STATE_MOVE].sprite.center_x - main_work.act_tab_state_move_base_pos[AMD_X],
            main_work.act[(int)ACT_TAB_STATE_MOVE].sprite.center_y - main_work.act_tab_state_move_base_pos[AMD_Y]
        ];
#endif //_IPHONE
        for (int i = (int)ACT_TAB_STATE_L; i <= (int)ACT_TAB_STATE_R2; i++)
        {

            AoActAcmInit();
            AoActAcmApplyTrans(0.0f
                               , main_work.mode_tex_pos_y
                               , 0.0f
                               );
#if !_IPHONE
            AoActUpdate(main_work.act[i], 0.0f);
#else //!_IPHONE
            AoActAcmApplyTrans(tab_state_move_pos[AMD_X], tab_state_move_pos[AMD_Y], 0.0f);
            float frame = ((2.0f <= main_work.act[i].frame) ? 1.0f : 0.0f);
            AoActUpdate(main_work.act[i], frame);
#endif //!_IPHONE

        }

        AoActAcmPop();

#if !_IPHONE
        AoActSetTexture(AoTexGetTexList(main_work.tex[1]));

        for (int i = ACT_TEX_MODE_TIME; i <= ACT_TEX_EMER; i++)
        {
            if (i != ACT_TEX_EMER)
            {
                AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
                AoActUpdate(main_work.act[i], 0.0f);
            }
            else
            {
                AoActAcmPush();
                AoActAcmInit();
                AoActAcmApplyTrans(0.0f
                                   , main_work.chaos_eme_pos_y
                                   , 0.0f
                                   );

                AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
                AoActUpdate(main_work.act[i], 0.0f);

                AoActAcmPop();
            }
        }
#endif //!_IPHONE
#if _IPHONE    //当たり判定処理      
        int index1 = 0;
        for (int index2 = arrayof(main_work.trg_mode); index1 < index2; ++index1)
            main_work.trg_mode[index1].Update();
#endif //_IPHONE	//当たり判定処理

        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
        AoActUpdate(main_work.act[(int)ACT_TEX_ZONE_UP], 0.0f);

        AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
        AoActUpdate(main_work.act[(int)ACT_TEX_ZONE_UP_S], 0.0f);


        AoActAcmPush();

#if !_IPHONE
        for (int i = ACT_TEX_BIG_TIME; i <= (int)ACT_TEX_SCORE_EFCT; i++)
        {
#else //!_IPHONE
        for (int i = (int)ACT_TEX_BIG_TIME; i <= (int)ACT_TEX_BIG_SCORE; i++)
        {
#endif //!_IPHONE

            AoActAcmInit();
            AoActAcmApplyTrans(0.0f
                               , main_work.mode_tex_pos_y
                               , 0.0f
                               );
#if !_IPHONE
            AoActUpdate(main_work.act[i], 0.0f);
#else //!_IPHONE
            AoActAcmApplyTrans(tab_state_move_pos[AMD_X], tab_state_move_pos[AMD_Y], 0.0f);
            float frame = ((2.0f <= main_work.act[i].frame) ? 1.0f : 0.0f);
            AoActUpdate(main_work.act[i], frame);
#endif //!_IPHONE

        }

        AoActAcmPop();

        // 戻るテキスト
        AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[4]));
        AoActUpdate(main_work.act[(int)ACT_TEX_FIX_BACK], 0.0f);
#if _IPHONE
        AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[3]));
        for (int i = (int)ACT_BACK_BTN01_L; i <= (int)ACT_BACK_BTN01_R; i++)
        {
            float update_frame = ((2.0f <= main_work.act[i].frame) ? 1.0f : 0.0f);
            AoActUpdate(main_work.act[i], update_frame);
        }
#endif //_IPHONE
#if _IPHONE  //当たり判定処理          
        {
            er.CTrgAoAction cancel = main_work.trg_cancel;
            cancel.Update();
        }
#endif //_IPHONE	//当たり判定処理



        if (main_work.is_final_open == DMD_STGSLCT_ZONE_ALL_OPEN)
        {
            open_zone_num = 6;
            tmp_disp_dist = 0.0f;
        }
        else if ((main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN) != 0
                 || (main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN) != 0)
        {
            open_zone_num = 5;
            tmp_disp_dist = 24.0f;
        }
        else if ((main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN) != 0
                 || (main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN) != 0)
        {
            open_zone_num = 5;
            tmp_disp_dist = 24.0f;
        }
        else
        {
            open_zone_num = 4;
            tmp_disp_dist = 64.0f;
        }

        // ダウンアイコン表示
        if (main_work.state == (int)DME_STGSLCT_MODE_STATE_ACT_SLCT)
        {
            AoActSetTexture(AoTexGetTexList(main_work.tex[0]));

            for (uint i = 0; i < open_zone_num; i++)
            {
                AoActSortRegAction(main_work.act[(int)ACT_ICON_DOWN_1 + i]);

                if (main_work.cur_zone == i)
                {
                    AoActSetFrame(main_work.act[(int)ACT_ICON_DOWN_1 + i], 0.0f);
                }
                else
                {
                    AoActSetFrame(main_work.act[(int)ACT_ICON_DOWN_1 + i], 1.0f);
                }

                // FINALが開いてない状態でスペステが開いた状態の場合の補正処理
                if ((main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN) != 0
                    && main_work.cur_zone == 5 && i == 4)
                {
                    if (0 == (main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN))
                    {
                        AoActSetFrame(main_work.act[(int)ACT_ICON_DOWN_1 + i], 0.0f);
                    }
                }

                AoActAcmPush();

                AoActAcmInit();
                AoActAcmApplyTrans(tmp_disp_dist, 0.0f, 0.0f);

                AoActUpdate(main_work.act[(int)ACT_ICON_DOWN_1 + i], 0.0f);

#if _IPHONE   //当たり判定処理          
                {
                    er.CTrgAoAction tab = main_work.trg_act_tab[i];
                    tab.Update();
                }
#endif //_IPHONE	//当たり判定処理

                AoActAcmPop();
            }
        }

#if _IPHONE
        if (_am_sample_draw_enable)
        {
#endif //_IPHONE
            // ソート実行
#if !_IPHONE
        AoActSortExecute();
#else //!_IPHONE
            AoActSortExecuteFix();
#endif //!_IPHONE

            // ソートバッファ描画
            AoActSortDraw();
#if _IPHONE
        }
#endif //_IPHONE

        // ソートバッファ全解除
        AoActSortUnregAll();


#if !_IPHONE
        // 帯描画設定
        AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[2]));

        for (int i = ACT_OBI_C; i <= ACT_OBI_R; i++)
        {
            AoActSortRegAction(main_work.act[i]);
        }

        // 帯
        AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[2]));

        for (int i = ACT_OBI_C; i <= ACT_OBI_R; i++)
        {
            AoActUpdate(main_work.act[i], 0.0f);
        }
#endif //!_IPHONE


        // アクション更新
        AoActSetTexture(AoTexGetTexList(main_work.tex[1]));

        // 帯はループ用に２つ用意
#if !_IPHONE
        for (int i = 0; i < 2; i++)
        {
            AoActSortRegAction(main_work.act[ACT_TEX_OBI1 + i]);

            if (main_work.state == DME_STGSLCT_MODE_STATE_ACT_SLCT
                && main_work.cur_zone == DME_STGSLCT_ZONE_TYPE_SPE)
            {
                AoActSetFrame(main_work.act[ACT_TEX_OBI1 + i], 2.0f);
            }
            else
            {
                AoActSetFrame(main_work.act[ACT_TEX_OBI1 + i], (float)main_work.state);
            }

            // 帯用
            AoActAcmPush();

            AoActAcmInit();
            AoActAcmApplyTrans(main_work.obi_pos[i]
                               , 0
                               , 0
                               );

#if _PS3 || _XBOX || _PC
		AoActAcmApplyScale(DMD_STGSLCT_OBI_MSG_SCALE_SIZE
						   , DMD_STGSLCT_OBI_MSG_SCALE_SIZE);
#endif

            AoActUpdate(main_work.act[ACT_TEX_OBI1 + i], 0.0f);

            AoActAcmPop();
        }
#endif //!_IPHONE

#if _IPHONE
        if (_am_sample_draw_enable)
        {
#endif //_IPHONE
            // ソート実行
            AoActSortExecute();

            // ソートバッファ描画
            AoActSortDraw();
#if _IPHONE
        }
#endif //_IPHONE

        // ソートバッファ全解除
        AoActSortUnregAll();
    }



    // ==========================================================================
    // dmStgSlctSetSonicStockDispFrame
    /*!
        ゾーン選択用描画設定処理
     */
    // ==========================================================================
    private static void dmStgSlctSetSonicStockDispFrame(DMS_STGSLCT_MAIN_WORK main_work)
    {
        int tmp_digit_data = 0;
        int[] tmp_digit = [0, 0, 0];
        int tmp_calc = 1;

        //	tmp_digit_data = (int)(main_work.player_stock);
        tmp_digit_data = (int)(main_work.player_stock - 1); // ※※※一旦、ステセレの残機表示はゲーム中より+1の状態にする

        if (tmp_digit_data < 0)
        {
            tmp_digit_data = 0;
        }
        if (tmp_digit_data > GSD_MAINSYS_PLAYER_REST_MAX - 1)
        {
            tmp_digit_data = (int)(GSD_MAINSYS_PLAYER_REST_MAX - 1);
        }



        // 残機数描画
        for (uint j = 0; j < 3; j++)
        {
            for (uint k = 0; k < 3 - j - 1; k++)
            {
                tmp_calc = tmp_calc * 10;
            }


            if (tmp_digit_data >= tmp_calc)
            {
                tmp_digit[j] = (int)(tmp_digit_data / tmp_calc);
                tmp_digit_data -= (int)((tmp_digit[j]) * tmp_calc);

                //			disp_zero_num = true;
            }
            else
            {
                //			if (disp_zero_num) {
                //				AoActSortRegAction(main_work.act[ACT_TAB_S_NUM1 + j]);
                //			}

                tmp_digit[j] = 0;
            }

            tmp_calc = 1;
        }

        for (uint j = 0; j < 3; j++)
        {
            AoActSetFrame(main_work.act[(int)ACT_REST_NUM_100 + j], (float)tmp_digit[j]);
        }

    }



    // ==========================================================================
    // dmStgSlctZoneSelectDraw
    /*!
        ゾーン選択用描画設定処理
     */
    // ==========================================================================
    private static void dmStgSlctZoneSelectDraw(DMS_STGSLCT_MAIN_WORK main_work)
    {
        uint i = 0;
        int tmp_decide_dst_x = 0;
        int tmp_decide_dst_y = 0;

        // ゾーンテーブル用AO描画プライオリティ設定
        AoActSysSetDrawTaskPrio(DMD_STGSLCT_DRAW_PRIO_ZONE);

        for (i = 0; i < (int)DME_STGSLCT_ZONE_TYPE_NUM; i++)
        {
            // ZONE一つ分の描画設定
            dmStgSlctOneZoneTableDraw(main_work, i);
        }

        // 決定ZONE演出フラグONならば
        if ((main_work.flag & DMD_STGSLCT_FLAG_DECIDE_ZONE_TABLE_EFCT) != 0)
        {
            tmp_decide_dst_x = main_work.decide_zone_efct_dist_x;
            tmp_decide_dst_y = main_work.decide_zone_efct_dist_y;
        }
        else
        {
            tmp_decide_dst_x = 0;
            tmp_decide_dst_y = 0;
        }

#if !_IPHONE   //iPhoneではカーソルを描画しない                        
        // カーソル描画
        if (main_work.state == DME_STGSLCT_MODE_STATE_ZONE_SLCT)
        {
            AoActSetTexture(AoTexGetTexList(main_work.tex[0]));

            for (i = 0; i < 3; i++)
            {
                AoActSortRegAction(main_work.act[ACT_TAB_ZONE_CURSOR2 + i]);
            }

            AoActAcmPush();

            AoActAcmInit();
            AoActAcmApplyTrans(main_work.zone_pos[main_work.cur_zone][0] + tmp_decide_dst_x
                               , main_work.zone_pos[main_work.cur_zone][1] + tmp_decide_dst_y
                               , 0
                               );

            for (i = 0; i < 3; i++)
            {
                AoActUpdate(main_work.act[ACT_TAB_ZONE_CURSOR2 + i]);
            }

            AoActAcmPop();

#if _IPHONE
	if (_am_sample_draw_enable) {
#endif //_IPHONE
            // ソート実行
            AoActSortExecute();

            // ソートバッファ描画
            AoActSortDraw();
#if _IPHONE
	}
#endif //_IPHONE

            // ソートバッファ全解除
            AoActSortUnregAll();
        }
#endif //!_IPHONE	//iPhoneではカーソルを描画しない
    }



    // ==========================================================================
    // dmStgSlctOneZoneTableDraw
    /*!
        ゾーン選択時のゾーンテーブル一つ分描画設定処理
     */
    // ==========================================================================
    private static void dmStgSlctOneZoneTableDraw(DMS_STGSLCT_MAIN_WORK main_work, uint i)
    {
        int tmp_decide_dst_x = 0;
        int tmp_decide_dst_y = 0;

        // ここで白フラッシュ解放演出の場合、指定のゾーンテーブルを非表示にする
        if ((main_work.announce_flag & 1u << (int)DME_STGSLCT_WIN_1_1_CLEAR) != 0)
        {
            if ((main_work.flag & DMD_STGSLCT_FLAG_WHITE_FLASH_EFCT) != 0
                && (i != (uint)DME_STGSLCT_ZONE_TYPE_1))
            {
                return;
            }
        }
        else if ((main_work.announce_flag & 1u << (int)DME_STGSLCT_WIN_FINAL_CAN_SLCT) != 0)
        {
            if ((main_work.flag & DMD_STGSLCT_FLAG_WHITE_FLASH_EFCT) != 0
                && (i == (uint)DME_STGSLCT_ZONE_TYPE_FINAL))
            {
                return;
            }
        }
        else if ((main_work.announce_flag & 1u << (int)DME_STGSLCT_WIN_SPESTE_CAN_SLCT) != 0)
        {
            if ((main_work.flag & DMD_STGSLCT_FLAG_WHITE_FLASH_EFCT) != 0
                && (i == (uint)DME_STGSLCT_ZONE_TYPE_SPE))
            {
                return;
            }
        }


        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));

        AoActSortRegAction(main_work.act[(int)ACT_TAB_ZONE_TAB]);
        AoActSortRegAction(main_work.act[(int)ACT_TAB_ZONE_SCR1 + i]);

        if (i != (uint)DME_STGSLCT_ZONE_TYPE_SPE)
        {
            AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
            AoActSortRegAction(main_work.act[(int)ACT_TAB_ZONE_TEXT]);
        }
        else
        {
            AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
            AoActSortRegAction(main_work.act[(int)ACT_TAB_ZONE_TEXT_S]);
        }

        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));

        // ここでiを見て、アクティブ以外は登録
#if !_IPHONE    //iPhoneではカーソルがあっていないゾーンも明るく表示する                                    
        if (main_work.cur_zone != i)
        {
#else //!_IPHONE	//iPhoneではカーソルがあっていないゾーンも明るく表示する
        if ((main_work.cur_zone != i) && main_work.is_disp_cover)
        {
#endif //!_IPHONE	//iPhoneではカーソルがあっていないゾーンも明るく表示する
            AoActSortRegAction(main_work.act[(int)ACT_TAB_ZONE_COVER2]);
            AoActSortRegAction(main_work.act[(int)ACT_TAB_ZONE_COVER1]);
            AoActSortRegAction(main_work.act[(int)ACT_TAB_ZONE_COVER3]);
        }

        // iを見て、テクスチャを切り替える(フレームで)
#if !_IPHONE   //カーソルがあっていないゾーンもアニメーションさせる                               
        if (main_work.cur_zone != i)
        {
#else //!_IPHONE	//カーソルがあっていないゾーンもアニメーションさせる
        if (false)
        {
#endif //!_IPHONE	//カーソルがあっていないゾーンもアニメーションさせる
            AoActSetFrame(main_work.act[(int)ACT_TAB_ZONE_SCR1 + i], 0.0f);

            AoActSetFrame(main_work.act[(int)ACT_TAB_ZONE_SCR1_1a + i * 3], 0.0f);
            AoActSetFrame(main_work.act[(int)ACT_TAB_ZONE_SCR1_2a + i * 3], 0.0f);
            AoActSetFrame(main_work.act[(int)ACT_TAB_ZONE_SCR1_3a + i * 3], 0.0f);
        }
        else
        {
            AoActSetFrame(main_work.act[(int)ACT_TAB_ZONE_SCR1 + i], (float)main_work.zone_scr_id);

            if (i < 4)
            {
                AoActSortRegAction(main_work.act[(int)ACT_TAB_ZONE_SCR1_1a + i * 3]);
                AoActSortRegAction(main_work.act[(int)ACT_TAB_ZONE_SCR1_2a + i * 3]);
                AoActSortRegAction(main_work.act[(int)ACT_TAB_ZONE_SCR1_3a + i * 3]);

                AoActSetFrame(main_work.act[(int)ACT_TAB_ZONE_SCR1_1a + i * 3], (float)main_work.zone_scr_id);
                AoActSetFrame(main_work.act[(int)ACT_TAB_ZONE_SCR1_2a + i * 3], (float)main_work.zone_scr_id);
                AoActSetFrame(main_work.act[(int)ACT_TAB_ZONE_SCR1_3a + i * 3], (float)main_work.zone_scr_id);
            }
        }

        //	AoActSetFrame(main_work.act[ACT_TAB_ZONE_SCR1 + i], (float)i);
        AoActSetFrame(main_work.act[(int)ACT_TAB_ZONE_TEXT], (float)i);

        // 決定ZONE演出フラグONならば
        if ((main_work.flag & (uint)DMD_STGSLCT_FLAG_DECIDE_ZONE_TABLE_EFCT) != 0
            && main_work.cur_zone == i)
        {
            tmp_decide_dst_x = main_work.decide_zone_efct_dist_x;
            tmp_decide_dst_y = main_work.decide_zone_efct_dist_y;
        }
        else
        {
            tmp_decide_dst_x = 0;
            tmp_decide_dst_y = 0;
        }

        // ACCUMURATEでトランスさせる
        AoActAcmPush();

        AoActAcmInit();
        AoActAcmApplyTrans((float)(main_work.zone_pos[i][0] + tmp_decide_dst_x)
                           , (float)(main_work.zone_pos[i][1] + tmp_decide_dst_y)
                           , 0
                           );

        // テーブル一つ分はトランス値は一定のため、まとめてUpdateする
        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
        AoActUpdate(main_work.act[(int)ACT_TAB_ZONE_TAB], 0.0f);
        AoActUpdate(main_work.act[(int)ACT_TAB_ZONE_SCR1 + i], 0.0f);

#if _IPHONE //当たり判定処理          
        {
            er.CTrgAoAction zone = main_work.trg_zone[i];
            zone.Update();
        }
#endif //_IPHONE	//当たり判定処理

        for (i = (uint)ACT_TAB_ZONE_SCR1_1a; i <= (uint)ACT_TAB_ZONE_SCR4_3a; i++)
        {
            AoActUpdate(main_work.act[i], 0.0f);
        }

        AoActUpdate(main_work.act[(int)ACT_TAB_ZONE_COVER2], 0.0f);
        AoActUpdate(main_work.act[(int)ACT_TAB_ZONE_COVER1], 0.0f);
        AoActUpdate(main_work.act[(int)ACT_TAB_ZONE_COVER3], 0.0f);

        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
        AoActUpdate(main_work.act[(int)ACT_TAB_ZONE_TEXT], 0.0f);

        AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
        AoActUpdate(main_work.act[(int)ACT_TAB_ZONE_TEXT_S], 0.0f);

        AoActAcmPop();

#if _IPHONE
        if (_am_sample_draw_enable)
        {
#endif //_IPHONE
            // ソート実行
            AoActSortExecute();

            // ソートバッファ描画
            AoActSortDraw();
#if _IPHONE
        }
#endif //_IPHONE

        // ソートバッファ全解除
        AoActSortUnregAll();
    }



    // ==========================================================================
    // dmStgSlctStageSelectDraw
    /*!
        ステージ選択用描画設定処理
     */
    // ==========================================================================
    private static void dmStgSlctStageSelectDraw(DMS_STGSLCT_MAIN_WORK main_work)
    {
        uint i = 0;

        // ステージテーブル用AO描画プライオリティ設定
        AoActSysSetDrawTaskPrio(DMD_STGSLCT_DRAW_PRIO_STAGE);

        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));

        // 現在選択中のZONE分のACTテーブル表示
        dmStgSlctSetDrawStageSelectTable(main_work, main_work.cur_zone, true);


        // 切り替え分のZONE分のACTテーブル表示
#if !_IPHONE
        if (main_work.chng_zone != (uint)DME_STGSLCT_ACT_PAGE_NONE)
        {
#else //!_IPHONE
        if ((main_work.flag & DMD_STGSLCT_FLAG_ACT_CHNG_ZONE) != 0 && (main_work.chng_zone != (uint)DME_STGSLCT_ACT_PAGE_NONE))
        {
#endif //!_IPHONE
            dmStgSlctSetDrawStageSelectTable(main_work, main_work.chng_zone);
        }

#if _IPHONE    //当たり判定処理          
        {
            er.CTrgFlick move = main_work.trg_act_move;
            move.Update();
        }
#endif //_IPHONE	//当たり判定処理

        // ここでカーソル表示フラグの設定
        if (main_work.state == (int)DME_STGSLCT_MODE_STATE_ACT_SLCT
            && main_work.cur_zone == (uint)DME_STGSLCT_ZONE_TYPE_SPE)
        {
            main_work.disp_flag |= DMD_STGSLCT_DISP_FLAG_ACT_CRSR;
        }
        else
        {
            main_work.disp_flag &= unchecked((uint)~DMD_STGSLCT_DISP_FLAG_ACT_CRSR);
        }


        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));

#if true
        // カーソル表示
#if !_IPHONE
	if (main_work.disp_flag & DMD_STGSLCT_DISP_FLAG_ACT_CRSR) {
		AoActSortRegAction(main_work.act[ACT_TAB_CURSOR_UP]);
		AoActSortRegAction(main_work.act[ACT_TAB_CURSOR_DOWN]);
	}
#else //!_IPHONE
        if ((main_work.disp_flag & DMD_STGSLCT_DISP_FLAG_LR_ARROW) != 0 && ((uint)DME_STGSLCT_ZONE_TYPE_FINAL != main_work.cur_zone))
        {
            if ((0 < main_work.focus_disp_no) || 0 != (DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL & main_work.flag))
            {
                AoActSortRegAction(main_work.act[(int)ACT_TAB_CURSOR_UP]);
            }
            int c_move_limit = (((uint)DME_STGSLCT_ZONE_TYPE_SPE == main_work.cur_zone) ? 4 : 1);
            if ((main_work.focus_disp_no < c_move_limit) || (DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL & main_work.flag) != 0)
            {
                AoActSortRegAction(main_work.act[(int)ACT_TAB_CURSOR_DOWN]);
            }
        }
#endif //!_IPHONE

        // ACCUMURATEでトランスさせる
        AoActAcmPush();

#if !_IPHONE
	for (i = ACT_TAB_CURSOR_UP; i <= ACT_TAB_CURSOR_3; i++) {
		AoActAcmInit();
		AoActAcmApplyTrans(312.0f
						   , 0.0f
						   , 0.0f
						   );
#else //!_IPHONE
        for (i = (uint)ACT_TAB_CURSOR_UP; i <= (uint)ACT_TAB_CURSOR_DOWN; i++)
        {
            AoActAcmInit();
#endif //!_IPHONE

            // テーブル一つ分はトランス値は一定のため、まとめてUpdateする

            // フレーム更新はSetFrameのみで行う
            AoActUpdate(main_work.act[i], 1.0f);
        }

        AoActAcmPop();
#endif

#if _IPHONE
        if (_am_sample_draw_enable)
        {
#endif //_IPHONE
            // ソート実行
            AoActSortExecute();

            // ソートバッファ描画
            AoActSortDraw();
#if _IPHONE
        }
#endif //_IPHONE

        // ソートバッファ全解除
        AoActSortUnregAll();


        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));

        // 切り替え矢印表示
        if ((main_work.disp_flag & DMD_STGSLCT_DISP_FLAG_LR_ARROW) != 0)
        {
            for (i = 0; i < 2; i++)
            {
                AoActSortRegAction(main_work.act[(int)ACT_ICON_L_ARROW + i]);
                AoActUpdate(main_work.act[(int)ACT_ICON_L_ARROW + i], 1.0f);
            }

#if _IPHONE    //当たり判定処理          
            uint num1 = 0;
            for (int index = arrayof(main_work.trg_act_lr); num1 < index; ++num1)
                main_work.trg_act_lr[(int)num1].Update();
#endif //_IPHONE	//当たり判定処理

#if _IPHONE
            if (_am_sample_draw_enable)
            {
#endif //_IPHONE
                // ソート実行
                AoActSortExecute();

                // ソートバッファ描画
                AoActSortDraw();
#if _IPHONE
            }
#endif //_IPHONE

            // ソートバッファ全解除
            AoActSortUnregAll();
        }

    }



    // ==========================================================================
    // dmStgSlctSetDrawStageSelectTable
    /*!
        ステージ選択用描画設定処理
     */
    // ==========================================================================
    private static void dmStgSlctSetDrawStageSelectTable(DMS_STGSLCT_MAIN_WORK main_work, uint zone, bool is_trg_update = false)
    {
#if !_IPHONE
        uint act_num_start = 0;
        uint act_num_end = 0;
        uint i = 0;
        uint j = 0;
        uint k = 0;

        act_num_start = dm_stgslct_zone_act_num_tbl[zone][0];
        act_num_end = dm_stgslct_zone_act_num_tbl[zone][1] + act_num_start;

        for (i = act_num_start; i < act_num_end; i++)
        {
            AoActSetTexture(AoTexGetTexList(main_work.tex[0]));

            // 台紙描画登録
            for (j = ACT_TAB_TABLE2; j <= ACT_TAB_TABLE3; j++)
            {
                AoActSortRegAction(main_work.act[j]);
            }

            // ステージ別テーブル上のアクション描画登録
            if (zone != DME_STGSLCT_ACT_PAGE_SPE)
            {
                AoActSortRegAction(main_work.act[ACT_TAB_SCR_BG]);
                AoActSortRegAction(main_work.act[ACT_TAB_SCR]);

                if (i != 16)
                {
                    if ((i + 1) % 4 != 0
                        || i == 0)
                    {
                        // 通常ACT
                        AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
                        AoActSortRegAction(main_work.act[ACT_TAB_TEXT]);

                        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
                        AoActSetFrame(main_work.act[ACT_TAB_A_NUM], dm_stgslct_act_num_disp_id_tbl[i]);
                        AoActSortRegAction(main_work.act[ACT_TAB_A_NUM]);
                    }
                    else
                    {
                        // BOSS
                        AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
                        AoActSortRegAction(main_work.act[ACT_TAB_TEX_BOSS]);
                    }
                }
                else
                {
                    // FINAL
                    AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
                    AoActSortRegAction(main_work.act[ACT_TAB_TEX_BOSS]);
                }

                AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
                AoActSortRegAction(main_work.act[ACT_TAB_MESS]);

                AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
                AoActSortRegAction(main_work.act[ACT_TAB_LINE]);
                //			AoActSortRegAction(main_work.act[ACT_TAB_ICON_EMER]);		// エメラルドは既定ステージのみ登録 ◆

                for (k = 0; k < 7; k++)
                {
                    if (main_work.eme_stage_no[k] == i
                        && main_work.get_emerald & 1 << k)
                    {
                        AoActSortRegAction(main_work.act[ACT_TAB_ICON_EMER]);
                        AoActSetFrame(main_work.act[ACT_TAB_ICON_EMER], (float)k);

                        break;
                    }
                }

                // モードを見てSCOREかTIMEの表示を分ける
                if (main_work.cur_game_mode == DME_STGSLCT_PLAY_MODE_NORMAL)
                {
                    AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
                    AoActSortRegAction(main_work.act[ACT_TAB_TEX_SCORE]);

                    AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
                    for (k = 0; k < 9; k++)
                    {
                        //					AoActSortRegAction(main_work.act[ACT_TAB_S_NUM1 + k]);		// ※ここはスコアの桁数分のみ登録する
                    }
                }
                else
                {
                    AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
                    AoActSortRegAction(main_work.act[ACT_TAB_TEX_TIME]);
                    for (k = 0; k < 7; k++)
                    {
                        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
                        AoActSortRegAction(main_work.act[ACT_TAB_S_NUM1 + k]);
                    }
                }
            }

            else
            {
                AoActSetTexture(AoTexGetTexList(main_work.tex[0]));

                if (GsMainSysIsStageClear(GSD_MAIN_STAGE_ID_SS1 + (i - act_num_start)))
                {
                    AoActSortRegAction(main_work.act[ACT_TAB_ICON_SPE_EMER]);
                }

                AoActSortRegAction(main_work.act[ACT_TAB_NUM_SPE_STAGE]);

                AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
                AoActSortRegAction(main_work.act[ACT_TAB_TEX_SPE_STAGE]);
                //			AoActSortRegAction(main_work.act[ACT_TAB_TEX_TIME]);

                // モードを見てSCOREかTIMEの表示を分ける
                if (main_work.cur_game_mode == DME_STGSLCT_PLAY_MODE_NORMAL)
                {
                    AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
                    AoActSortRegAction(main_work.act[ACT_TAB_TEX_SCORE]);

                    AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
                    for (k = 0; k < 9; k++)
                    {
                        //					AoActSortRegAction(main_work.act[ACT_TAB_S_NUM1 + k]);		// ※ここはスコアの桁数分のみ登録する
                    }
                }
                else
                {
                    AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
                    AoActSortRegAction(main_work.act[ACT_TAB_TEX_TIME]);
                    for (k = 0; k < 7; k++)
                    {
                        AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
                        AoActSortRegAction(main_work.act[ACT_TAB_S_NUM1 + k]);
                    }
                }

                AoActSetFrame(main_work.act[ACT_TAB_ICON_SPE_EMER], (float)(i - act_num_start));
                AoActSetFrame(main_work.act[ACT_TAB_NUM_SPE_STAGE], dm_stgslct_act_num_disp_id_tbl[i]);
            }

            // ステージ台紙のアクティブ・非アクティブ設定
            dmStgSlctSetTableActiveInfo(main_work, i);

            // ステージ番号からテキスト、ACT番号、を設定
            AoActSetFrame(main_work.act[ACT_TAB_ZONE_TEXT], dm_stgslct_act_table_disp_id_tbl[i]);

            // スコアを見てフレームを設定


            // エメラルド取得情報からフレームを設定


            // モードを見てSCOREかTIMEの表示を分ける
            dmStgSlctSetScoreDispFrame(main_work, zone, i);

            // ここでiを見て、アクティブ以外は登録
            if (main_work.cur_stage != (int)i)
            {
                AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
                AoActSortRegAction(main_work.act[ACT_TAB_COVER2]);
                AoActSortRegAction(main_work.act[ACT_TAB_COVER1]);
                AoActSortRegAction(main_work.act[ACT_TAB_COVER3]);
            }

            // ACCUMURATEでトランスさせる
            AoActAcmPush();


            for (j = ACT_TAB_START; j <= ACT_TAB_END; j++)
            {
                AoActAcmInit();
                AoActAcmApplyTrans(main_work.act_top_pos_x[i]
                                   , main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8)
                                   , 0
                                   );

                if (j == ACT_TAB_TEXT || j == ACT_TAB_MESS
                    || (j >= ACT_TAB_TEX_SCORE && j <= ACT_TAB_TEX_SPE_STAGE))
                {
                    AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
                }
                else
                {
                    AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
                }

                // フレーム更新はSetFrameのみで行う
                AoActUpdate(main_work.act[j], 0.0f);
            }

            for (j = ACT_TAB_COVER2; j <= ACT_TAB_COVER3; j++)
            {
                AoActAcmInit();

                AoActAcmApplyTrans(main_work.act_top_pos_x[i]
                                   , main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8)
                                   , 0
                                   );

                AoActSetTexture(AoTexGetTexList(main_work.tex[0]));
                AoActUpdate(main_work.act[j], 0.0f);
            }

            AoActAcmPop();

            // ソート実行
            AoActSortExecute();

            // ソートバッファ描画
            AoActSortDraw();

            // ソートバッファ全解除
            AoActSortUnregAll();
        }
#else //!_IPHONE//■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        //アクト単位で書くのではなく、同じテクスチャを使用しているアクションを横断的に描画していく
        uint act_num_start = 0;
        uint act_num_end = 0;
        uint i = 0;
        uint j = 0;
        uint k = 0;

        act_num_start = dm_stgslct_zone_act_num_tbl[zone][0];
        act_num_end = dm_stgslct_zone_act_num_tbl[zone][1] + act_num_start;


        CActionDraw act_draw_sys = stgSelect_act_draw_sys;
        act_draw_sys.Clear();

        //台紙
        AoActSetTexture(AoTexGetTexList(main_work.tex[(int)DME_STGSLCT_DATA_TYPE_CMN_DATA]));
        for (i = act_num_start; i < act_num_end; i++)
        {
            for (j = (uint)ACT_TAB_TABLE2; j <= (int)ACT_TAB_TABLE3; j++)
            {
                dmStgSlctSetTableActiveInfo(main_work, i);
                AoActAcmPush();
                AoActAcmInit();
                AoActAcmApplyTrans(main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8), 0);
                AoActUpdate(main_work.act[j], 0.0f);
                AoActAcmPop();
                act_draw_sys.Entry(main_work.ama[(int)DME_STGSLCT_DATA_TYPE_CMN_DATA], g_dm_act_id_tbl[j], main_work.act[j].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
            }
#if _IPHONE    //当たり判定処理          
            if (is_trg_update)
            {
                //描画の関係上1fに複数回呼ばれる可能性があるので、明示的に1回に押さえる
                if ((act_num_start <= i) && (i < (act_num_start + arrayof(main_work.trg_act))))
                {
                    uint index = i - act_num_start;
                    er.CTrgAoAction act = main_work.trg_act[index];
                    act.Update();
                }
            }
#endif //_IPHONE	//当たり判定処理
        }
        act_draw_sys.Draw();
        act_draw_sys.Clear();

        // ステージ別テーブル上のアクション描画登録
        if (zone != (int)DME_STGSLCT_ACT_PAGE_SPE)
        {
            //スペステ以外
            AoActSetTexture(AoTexGetTexList(main_work.tex[(int)DME_STGSLCT_DATA_TYPE_CMN_DATA]));
            for (i = act_num_start; i < act_num_end; i++)
            {
                AoActUpdate(main_work.act[(int)ACT_TAB_SCR_BG], 0.0f);
                act_draw_sys.Entry(main_work.ama[(int)DME_STGSLCT_DATA_TYPE_CMN_DATA], g_dm_act_id_tbl[(int)ACT_TAB_SCR_BG], main_work.act[(int)ACT_TAB_SCR_BG].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
            }
            for (i = act_num_start; i < act_num_end; i++)
            {
                dmStgSlctSetTableActiveInfo(main_work, i);
                AoActUpdate(main_work.act[(int)ACT_TAB_SCR], 0.0f);
                act_draw_sys.Entry(main_work.ama[(int)DME_STGSLCT_DATA_TYPE_CMN_DATA], g_dm_act_id_tbl[(int)ACT_TAB_SCR], main_work.act[(int)ACT_TAB_SCR].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
            }
            act_draw_sys.Draw();
            act_draw_sys.Clear();

            for (i = act_num_start; i < act_num_end; i++)
            {
                if (i == 16)
                {
                    // FINAL
                    AoActSetTexture(AoTexGetTexList(main_work.tex[(int)DME_STGSLCT_DATA_TYPE_LANG_DATA]));
                    AoActUpdate(main_work.act[(int)ACT_TAB_TEX_BOSS], 0.0f);
                    act_draw_sys.Entry(main_work.ama[(int)DME_STGSLCT_DATA_TYPE_LANG_DATA], g_dm_act_id_tbl[(int)ACT_TAB_TEX_BOSS], main_work.act[(int)ACT_TAB_TEX_BOSS].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
                }
                else if (((i + 1) % 4 != 0) || (i == 0))
                {
                    // 通常ACT
                    AoActSetTexture(AoTexGetTexList(main_work.tex[(int)DME_STGSLCT_DATA_TYPE_LANG_DATA]));
                    AoActUpdate(main_work.act[(int)ACT_TAB_TEXT], 0.0f);
                    act_draw_sys.Entry(main_work.ama[(int)DME_STGSLCT_DATA_TYPE_LANG_DATA], g_dm_act_id_tbl[(int)ACT_TAB_TEXT], main_work.act[(int)ACT_TAB_TEXT].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
                }
                else
                {
                    // BOSS
                    AoActSetTexture(AoTexGetTexList(main_work.tex[(int)DME_STGSLCT_DATA_TYPE_LANG_DATA]));
                    AoActUpdate(main_work.act[(int)ACT_TAB_TEX_BOSS], 0.0f);
                    act_draw_sys.Entry(main_work.ama[(int)DME_STGSLCT_DATA_TYPE_LANG_DATA], g_dm_act_id_tbl[(int)ACT_TAB_TEX_BOSS], main_work.act[(int)ACT_TAB_TEX_BOSS].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
                }
            }
            for (i = act_num_start; i < act_num_end; i++)
            {
                if (i == 16)
                {
                    // FINAL
                }
                else if (((i + 1) % 4 != 0) || (i == 0))
                {
                    // 通常ACT
                    AoActSetTexture(AoTexGetTexList(main_work.tex[(int)DME_STGSLCT_DATA_TYPE_CMN_DATA]));
                    AoActSetFrame(main_work.act[(int)ACT_TAB_A_NUM], dm_stgslct_act_num_disp_id_tbl[i]);
                    AoActUpdate(main_work.act[(int)ACT_TAB_A_NUM], 0.0f);
                    act_draw_sys.Entry(main_work.ama[(int)DME_STGSLCT_DATA_TYPE_CMN_DATA], g_dm_act_id_tbl[(int)ACT_TAB_A_NUM], main_work.act[(int)ACT_TAB_A_NUM].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
                }
                else
                {
                    // BOSS
                }
            }
            act_draw_sys.Draw();
            act_draw_sys.Clear();

            for (i = act_num_start; i < act_num_end; i++)
            {
                // ステージ台紙のアクティブ・非アクティブ設定
                dmStgSlctSetTableActiveInfo(main_work, i);
                AoActSetTexture(AoTexGetTexList(main_work.tex[(int)DME_STGSLCT_DATA_TYPE_LANG_DATA]));
                AoActUpdate(main_work.act[(int)ACT_TAB_MESS], 0.0f);
                act_draw_sys.Entry(main_work.ama[(int)DME_STGSLCT_DATA_TYPE_LANG_DATA], g_dm_act_id_tbl[(int)ACT_TAB_MESS], main_work.act[(int)ACT_TAB_MESS].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
            }

            for (i = act_num_start; i < act_num_end; i++)
            {
                AoActSetTexture(AoTexGetTexList(main_work.tex[(int)DME_STGSLCT_DATA_TYPE_CMN_DATA]));
                AoActUpdate(main_work.act[(int)ACT_TAB_LINE], 0.0f);
                AoActSortRegAction(main_work.act[(int)ACT_TAB_LINE]);
                act_draw_sys.Entry(main_work.ama[(int)DME_STGSLCT_DATA_TYPE_CMN_DATA], g_dm_act_id_tbl[(int)ACT_TAB_LINE], main_work.act[(int)ACT_TAB_LINE].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
            }
            act_draw_sys.Draw();
            act_draw_sys.Clear();

            for (i = act_num_start; i < act_num_end; i++)
            {
                for (k = 0; k < 7; k++)
                {
                    if ((main_work.eme_stage_no[k] == i) && 0 != (main_work.get_emerald & (1u << (int)k)))
                    {
                        AoActUpdate(main_work.act[(int)ACT_TAB_ICON_EMER], 0.0f);
                        AoActSetFrame(main_work.act[(int)ACT_TAB_ICON_EMER], (float)k);
                        act_draw_sys.Entry(main_work.ama[(int)DME_STGSLCT_DATA_TYPE_CMN_DATA], g_dm_act_id_tbl[(int)ACT_TAB_ICON_EMER], main_work.act[(int)ACT_TAB_ICON_EMER].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
                        break;
                    }
                }
            }
            act_draw_sys.Draw();
            act_draw_sys.Clear();

        }
        else
        {
            //スペステ
            AoActSetTexture(AoTexGetTexList(main_work.tex[(int)DME_STGSLCT_DATA_TYPE_LANG_DATA]));
            for (i = act_num_start; i < act_num_end; i++)
            {
                AoActUpdate(main_work.act[(int)ACT_TAB_TEX_SPE_STAGE], 0.0f);
                act_draw_sys.Entry(main_work.ama[(int)DME_STGSLCT_DATA_TYPE_LANG_DATA], g_dm_act_id_tbl[(int)ACT_TAB_TEX_SPE_STAGE], main_work.act[(int)ACT_TAB_TEX_SPE_STAGE].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
            }
            act_draw_sys.Draw();
            act_draw_sys.Clear();

            AoActSetTexture(AoTexGetTexList(main_work.tex[(int)DME_STGSLCT_DATA_TYPE_CMN_DATA]));
            for (i = act_num_start; i < act_num_end; i++)
            {
                if (GsMainSysIsStageClear((int)(GSD_MAIN_STAGE_ID_SS1 + (i - act_num_start))))
                {
                    AoActSetFrame(main_work.act[(int)ACT_TAB_ICON_SPE_EMER], (float)(i - act_num_start));
                    AoActUpdate(main_work.act[(int)ACT_TAB_ICON_SPE_EMER], 0.0f);
                    act_draw_sys.Entry(main_work.ama[(int)DME_STGSLCT_DATA_TYPE_CMN_DATA], g_dm_act_id_tbl[(int)ACT_TAB_ICON_SPE_EMER], main_work.act[(int)ACT_TAB_ICON_SPE_EMER].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
                }
            }
            for (i = act_num_start; i < act_num_end; i++)
            {
                AoActSetFrame(main_work.act[(int)ACT_TAB_NUM_SPE_STAGE], dm_stgslct_act_num_disp_id_tbl[i]);
                AoActUpdate(main_work.act[(int)ACT_TAB_NUM_SPE_STAGE], 0.0f);
                act_draw_sys.Entry(main_work.ama[(int)DME_STGSLCT_DATA_TYPE_CMN_DATA], g_dm_act_id_tbl[(int)ACT_TAB_NUM_SPE_STAGE], main_work.act[(int)ACT_TAB_NUM_SPE_STAGE].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
            }
            act_draw_sys.Draw();
            act_draw_sys.Clear();
        }

        // モードを見てSCOREかTIMEの表示を分ける
        if (main_work.cur_game_mode == (uint)DME_STGSLCT_PLAY_MODE_NORMAL)
        {
            //SCORE
            AoActSetTexture(AoTexGetTexList(main_work.tex[(int)DME_STGSLCT_DATA_TYPE_LANG_DATA]));
            for (i = act_num_start; i < act_num_end; i++)
            {
                AoActUpdate(main_work.act[(int)ACT_TAB_TEX_SCORE], 0.0f);
                act_draw_sys.Entry(main_work.ama[(int)DME_STGSLCT_DATA_TYPE_LANG_DATA], g_dm_act_id_tbl[(int)ACT_TAB_TEX_SCORE], main_work.act[(int)ACT_TAB_TEX_SCORE].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
            }
            for (i = act_num_start; i < act_num_end; i++)
            {
                dmStgSlctSetScoreDispFrame(main_work, zone, i);
                uint m = 0;
                for (; m < DMD_STGSLCT_DISP_SCORE_DIGIT_NUM; m++)
                {
                    if (0 != main_work.act[(int)ACT_TAB_S_NUM1 + m].frame)
                    {
                        break;
                    }
                }
                for (; m < DMD_STGSLCT_DISP_SCORE_DIGIT_NUM; m++)
                {
                    AoActSetTexture(AoTexGetTexList(main_work.tex[(int)DME_STGSLCT_DATA_TYPE_CMN_DATA]));
                    AoActUpdate(main_work.act[(int)ACT_TAB_S_NUM1 + m], 0.0f);
                    act_draw_sys.Entry(main_work.ama[(int)DME_STGSLCT_DATA_TYPE_CMN_DATA], g_dm_act_id_tbl[(int)ACT_TAB_S_NUM1 + m], main_work.act[(int)ACT_TAB_S_NUM1 + m].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
                }
            }
        }
        else
        {
            //TIME
            AoActSetTexture(AoTexGetTexList(main_work.tex[(int)DME_STGSLCT_DATA_TYPE_LANG_DATA]));
            for (i = act_num_start; i < act_num_end; i++)
            {
                AoActUpdate(main_work.act[(int)ACT_TAB_TEX_TIME], 0.0f);
                act_draw_sys.Entry(main_work.ama[(int)DME_STGSLCT_DATA_TYPE_LANG_DATA], g_dm_act_id_tbl[(int)ACT_TAB_TEX_TIME], main_work.act[(int)ACT_TAB_TEX_TIME].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
            }
            for (i = act_num_start; i < act_num_end; i++)
            {
                dmStgSlctSetScoreDispFrame(main_work, zone, i);
                for (k = 0; k < 7; k++)
                {
                    AoActSetTexture(AoTexGetTexList(main_work.tex[(int)DME_STGSLCT_DATA_TYPE_CMN_DATA]));
                    AoActUpdate(main_work.act[(int)ACT_TAB_S_NUM1 + k], 0.0f);
                    act_draw_sys.Entry(main_work.ama[(int)DME_STGSLCT_DATA_TYPE_CMN_DATA], g_dm_act_id_tbl[(int)ACT_TAB_S_NUM1 + k], main_work.act[(int)ACT_TAB_S_NUM1 + k].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
                }
            }
        }
        act_draw_sys.Draw();
        act_draw_sys.Clear();

        //スコア描画
        for (i = act_num_start; i < act_num_end; i++)
        {
            dmStgSlctSetScoreDispFrame(main_work, zone, i);
            if (_am_sample_draw_enable)
            {
                AoActSortExecute();
                AoActSortDraw();
            }
            AoActSortUnregAll();
        }

        // ここでiを見て、アクティブ以外は登録
        for (i = act_num_start; i < act_num_end; i++)
        {
            AoActSetTexture(AoTexGetTexList(main_work.tex[(int)DME_STGSLCT_DATA_TYPE_CMN_DATA]));
            if ((main_work.cur_stage != (int)i) && main_work.is_disp_cover)
            {
                AoActUpdate(main_work.act[(int)ACT_TAB_COVER2], 0.0f);
                AoActUpdate(main_work.act[(int)ACT_TAB_COVER1], 0.0f);
                AoActUpdate(main_work.act[(int)ACT_TAB_COVER3], 0.0f);
                act_draw_sys.Entry(main_work.ama[(int)DME_STGSLCT_DATA_TYPE_CMN_DATA], g_dm_act_id_tbl[(int)ACT_TAB_COVER3], main_work.act[(int)ACT_TAB_COVER3].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
                act_draw_sys.Entry(main_work.ama[(int)DME_STGSLCT_DATA_TYPE_CMN_DATA], g_dm_act_id_tbl[(int)ACT_TAB_COVER1], main_work.act[(int)ACT_TAB_COVER1].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
                act_draw_sys.Entry(main_work.ama[(int)DME_STGSLCT_DATA_TYPE_CMN_DATA], g_dm_act_id_tbl[(int)ACT_TAB_COVER2], main_work.act[(int)ACT_TAB_COVER2].frame, main_work.act_top_pos_x[i], main_work.act_top_pos_y[i] + (i - act_num_start) * (DMD_STGSLCT_ACT_TABLE_DIST_Y - 8));
            }
        }
        act_draw_sys.Draw();
        act_draw_sys.Clear();
#endif //!_IPHONE
    }



    // ==========================================================================
    // dmStgSlctSetScoreDispFrame
    /*!
        各ステージの表示描画フレーム設定処理
     */
    // ==========================================================================
    private static void dmStgSlctSetScoreDispFrame(DMS_STGSLCT_MAIN_WORK main_work, uint zone, uint act_no)
    {
        UNREFERENCED_PARAMETER(zone);

        int tmp_digit_data = 0;
        int[] tmp_digit = [0, 0, 0, 0, 0, 0, 0, 0, 0];
        int tmp_calc = 1;

        ushort time_min = 0;
        ushort time_sec = 0;
        ushort time_msec = 0;
        int[] time_digit = [0, 0];
        float time_digit_data = 0.0f;
        bool disp_zero_num = false;


        if (main_work.cur_game_mode == (uint)DME_STGSLCT_PLAY_MODE_NORMAL)
        {
            if (main_work.hi_score[act_no] != DMD_STGSLCT_INIT_SCORE_NUM)
            {

                tmp_digit_data = (int)main_work.hi_score[act_no];

                // スコア描画
                for (uint j = 0; j < DMD_STGSLCT_DISP_SCORE_DIGIT_NUM; j++)
                {

                    if (DMD_STGSLCT_DISP_SCORE_DIGIT_NUM - j - 1 <= 0)
                    {
                        tmp_digit_data = 1;
                    }
                    else
                    {
                        for (uint k = 0; k < DMD_STGSLCT_DISP_SCORE_DIGIT_NUM - j - 1; k++)
                        {
                            tmp_calc = tmp_calc * 10;
                        }
                    }

                    if (tmp_digit_data >= tmp_calc)
                    {
                        AoActSortRegAction(main_work.act[(uint)ACT_TAB_S_NUM1 + j]);
                        if (j >= DMD_STGSLCT_DISP_SCORE_DIGIT_NUM - 1)
                        {
                            tmp_digit[j] = 0;
                        }
                        else
                        {
                            tmp_digit[j] = (int)(tmp_digit_data / tmp_calc);
                            tmp_digit_data -= (int)((tmp_digit[j]) * tmp_calc - 1);
                        }
                        disp_zero_num = true;
                    }
                    else
                    {
                        if (disp_zero_num)
                        {
                            AoActSortRegAction(main_work.act[(uint)ACT_TAB_S_NUM1 + j]);
                        }

                        tmp_digit[j] = 0;
                    }

                    tmp_calc = 1;
                }

                // スコアの各桁の数字フレーム設定
                for (uint k = 0; k < 9; k++)
                {
                    AoActSetFrame(main_work.act[(uint)ACT_TAB_S_NUM1 + k], (float)tmp_digit[k]);
                }
            }
            else
            {
                for (uint k = 0; k < 9; k++)
                {
                    AoActSortRegAction(main_work.act[(uint)ACT_TAB_S_NUM1 + k]);
                    AoActSetFrame(main_work.act[(uint)ACT_TAB_S_NUM1 + k], DMD_STGSLCT_NO_ACTIVE_NUM_ID);
                }
            }
        }

        else
        {

            if (main_work.record_time[act_no] != DMD_STGSLCT_INIT_RECORD_TIME_NUM)
            {
                // 取得したクリアタイムを分・秒・ミリ秒に変換
                AkUtilFrame60ToTime((uint)main_work.record_time[act_no]
                                    , ref time_min
                                    , ref time_sec
                                    , ref time_msec
                                    );

                time_digit_data = (float)time_sec;

                if (time_digit_data >= 10)
                {
                    time_digit[0] = (int)(time_digit_data / 10.0f);
                    time_digit_data -= (float)(time_digit[0] * 10.0f);
                }
                else
                {
                    time_digit[0] = 0;
                }

                time_digit[1] = (int)time_digit_data;

                // 分
                AoActSetFrame(main_work.act[(int)ACT_TAB_S_NUM1], (float)time_min);

                AoActSetFrame(main_work.act[(int)ACT_TAB_S_NUM2], DMD_STGSLCT_TIME_COLON_ID);

                // 秒
                AoActSetFrame(main_work.act[(int)ACT_TAB_S_NUM3], (float)time_digit[0]);
                AoActSetFrame(main_work.act[(int)ACT_TAB_S_NUM4], (float)time_digit[1]);

                AoActSetFrame(main_work.act[(int)ACT_TAB_S_NUM5], DMD_STGSLCT_TIME_COLON_ID);

                time_digit_data = (float)time_msec;

                if (time_digit_data >= 10)
                {
                    time_digit[0] = (int)(time_digit_data / 10.0f);
                    time_digit_data -= (float)(time_digit[0] * 10.0f);
                }
                else
                {
                    time_digit[0] = 0;
                }

                time_digit[1] = (int)time_digit_data;

                // ミリ秒
                AoActSetFrame(main_work.act[(int)ACT_TAB_S_NUM6], (float)time_digit[0]);
                AoActSetFrame(main_work.act[(int)ACT_TAB_S_NUM7], (float)time_digit[1]);
                //			AoActSetFrame(main_work.act[ACT_TAB_S_NUM8], 10.0f);
                //			AoActSetFrame(main_work.act[ACT_TAB_S_NUM9], 10.0f);

            }
            // こちらは未プレイ時の表示設定(-で全て表示)
            else
            {
                for (uint k = 0; k < 7; k++)
                {
                    if (k == 1 || k == 4)
                    {
                        AoActSetFrame(main_work.act[(uint)ACT_TAB_S_NUM1 + k]
                                      , DMD_STGSLCT_TIME_COLON_ID
                                      );
                    }
                    else
                    {
                        AoActSetFrame(main_work.act[(uint)ACT_TAB_S_NUM1 + k]
                                      , DMD_STGSLCT_NO_ACTIVE_NUM_ID
                                      );
                    }
                }
            }
        }
    }



    // ==========================================================================
    // dmStgSlctWinSelectDraw
    /*!
        ウインドウ用描画設定処理
     */
    // ==========================================================================
    private static void dmStgSlctWinSelectDraw(DMS_STGSLCT_MAIN_WORK main_work)
    {
        float[] tmp_win_size = [0.0f, 0.0f];

        // ウインドウ用AO描画プライオリティ設定
        AoActSysSetDrawTaskPrio(DMD_STGSLCT_DRAW_PRIO_WIN_FIX);

#if _WII || _IPHONE
        tmp_win_size[0] = DMD_STGSLCT_WINDOW_SIZE_W * DMD_STGSLCT_DISP_SCALE_TEXT;
        tmp_win_size[1] = DMD_STGSLCT_WINDOW_SIZE_H * DMD_STGSLCT_DISP_SCALE_TEXT;
#else
        tmp_win_size[0] = DMD_STGSLCT_WINDOW_SIZE_W;
        tmp_win_size[1] = DMD_STGSLCT_WINDOW_SIZE_H;
#endif

        // ウインドウ描画
#if _IPHONE
        switch (main_work.win_mode)
        {
            case (int)DME_STGSLCT_WIN_STG_SLCT: //アクト決定確認
                                                //少し大きくする
#if AMD_DEBUG
		static float c_scale_x[AMD_XY] = {1.01f, 1.27f};
		if (AoPadRepeat() & KEY_R_UP) {
			c_scale_x[AMD_Y] += 0.01f;
		} else if (AoPadRepeat() & KEY_R_DOWN) {
			c_scale_x[AMD_Y] -= 0.01f;
		}
		if (AoPadRepeat() & KEY_R_LEFT) {
			c_scale_x[AMD_X] += 0.01f;
		} else if (AoPadRepeat() & KEY_R_RIGHT) {
			c_scale_x[AMD_X] -= 0.01f;
		}
		amPrintf(60, 44, "%6.2f  %6.2f", c_scale_x[AMD_X], c_scale_x[AMD_Y]);
		amPrintf(59, 41, "%6.2f  %6.2f", c_scale_x[AMD_X], c_scale_x[AMD_Y]);
#else //defined(AMD_DEBUG)
                float[] c_scale_x = [1.01f, 1.27f];
#endif //defined(AMD_DEBUG)
                for (int i = 0; i < AMD_XY; ++i)
                {
                    tmp_win_size[i] *= c_scale_x[i];
                }
                break;
        }
        if ((0.0f < main_work.win_size_rate[0]) && (0.0f < main_work.win_size_rate[1])) //↓下に繋げる
#endif //_IPHONE
            AoWinSysDrawTask(AOD_WIN_TYPE_A
                             , AoTexGetTexList(main_work.cmn_tex[3])
                             , 0
                             , DMD_STGSLCT_SIZE_WIDTH / 2.0f        // ウインドウ中心X
                             , DMD_STGSLCT_SIZE_HEIGHT / 2.0f       // ウインドウ中心Y
                             , tmp_win_size[0] * main_work.win_size_rate[0]         // ウインドウ横サイズ
                             , tmp_win_size[1] * main_work.win_size_rate[1]         // ウインドウ縦サイズ
                             , DMD_STGSLCT_DRAW_PRIO_WIN            // 描画タスク優先度
                             );

        // ウインドウ内の項目描画
        if ((main_work.disp_flag & DMD_STGSLCT_DISP_FLAG_WIN_ACT) != 0)
        {       // ウインドウが表示しきっているならば
                // 共通ウインドウACT登録
            AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[3]));
            //		AoActSortRegAction(main_work.act[ACT_WIN_LINE]);

            AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[4]));
            //		AoActSortRegAction(main_work.act[ACT_TEX_WINTITLE]);


            switch (main_work.win_mode)
            {
                case (int)DME_STGSLCT_WIN_MENU:
#if !_IPHONE
                    // アクション登録
                    AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[1]));
                    AoActSortRegAction(main_work.act[ACT_BTN_CANCEL2]);

                    AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[4]));
                    AoActSortRegAction(main_work.act[ACT_TEX_BACK1]);

                    // 初期表示フレーム設定
                    AoActSetFrame(main_work.act[ACT_TEX_WINTITLE]
                                  , dm_stgslct_win_act_frm_tbl[DME_STGSLCT_WIN_MENU][0]);
                    AoActSetFrame(main_work.act[ACT_BTN_CANCEL2], 0.0f);
                    AoActSetFrame(main_work.act[ACT_TEX_BACK1], 0.0f);
#endif //!_IPHONE

                    break;

                case (int)DME_STGSLCT_WIN_STG_SLCT:
#if !_IPHONE
                    AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[1]));
                    AoActSortRegAction(main_work.act[ACT_BTN_CANCEL2]);
#else //!_IPHONE
                    AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[3]));
                    for (int i = (int)ACT_YES_BTN_L, max = (int)ACT_NO_BTN_R; i <= max; ++i)
                    {
                        AoActSortRegAction(main_work.act[i]);
                    }
#endif //!_IPHONE
                    AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
                    AoActSortRegAction(main_work.act[(int)ACT_WIN_TEX_MSG2]);

                    AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[4]));
                    AoActSortRegAction(main_work.act[(int)ACT_TEX_YES]);
                    AoActSortRegAction(main_work.act[(int)ACT_TEX_NO]);
#if !_IPHONE
                    AoActSortRegAction(main_work.act[ACT_TEX_BACK1]);

                    // 初期表示フレーム設定
                    AoActSetFrame(main_work.act[ACT_TEX_WINTITLE]
                                  , dm_stgslct_win_act_frm_tbl[DME_STGSLCT_WIN_STG_SLCT][0]);
#endif //!_IPHONE

                    if (main_work.cur_zone != (uint)DME_STGSLCT_ZONE_TYPE_SPE)
                    {
                        AoActSetFrame(main_work.act[(int)ACT_WIN_TEX_MSG2], 0.0f);
                    }
                    else
                    {
                        AoActSetFrame(main_work.act[(int)ACT_WIN_TEX_MSG2], 1.0f);
                    }

#if !_IPHONE
                    AoActSetFrame(main_work.act[ACT_TEX_YES]
                                  , dm_stgslct_win_disp_slct_frm_tbl[main_work.win_cur_slct][0]);
                    AoActSetFrame(main_work.act[ACT_TEX_NO]
                                  , dm_stgslct_win_disp_slct_frm_tbl[main_work.win_cur_slct][1]);

                    AoActSetFrame(main_work.act[ACT_BTN_CANCEL2], 0.0f);

                    AoActSetFrame(main_work.act[ACT_TEX_BACK1], 0.0f);
#endif //!_IPHONE

                    break;

                case (int)DME_STGSLCT_WIN_1_1_CLEAR:
                    AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
                    AoActSortRegAction(main_work.act[(int)ACT_WIN_TEX_MSG]);

#if !_IPHONE
                    AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[4]));
                    AoActSortRegAction(main_work.act[ACT_TEX_OK]);

                    // 初期表示フレーム設定
                    AoActSetFrame(main_work.act[ACT_TEX_WINTITLE],
                                  dm_stgslct_win_act_frm_tbl[DME_STGSLCT_WIN_1_1_CLEAR][0]);
#endif //!_IPHONE
                    AoActSetFrame(main_work.act[(int)ACT_WIN_TEX_MSG]
                                  , dm_stgslct_win_act_frm_tbl[(int)DME_STGSLCT_WIN_1_1_CLEAR][1]);
#if !_IPHONE
                    AoActSetFrame(main_work.act[ACT_TEX_OK], 0.0f);
#endif //!_IPHONE

                    break;

                case (int)DME_STGSLCT_WIN_BOSS1_CAN_PLAY:
                    AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
                    AoActSortRegAction(main_work.act[(int)ACT_WIN_TEX_MSG]);

#if !_IPHONE
                    AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[4]));
                    AoActSortRegAction(main_work.act[ACT_TEX_OK]);

                    // 初期表示フレーム設定
                    AoActSetFrame(main_work.act[ACT_TEX_WINTITLE]
                                  , dm_stgslct_win_act_frm_tbl[DME_STGSLCT_WIN_BOSS1_CAN_PLAY][0]);
#endif //!_IPHONE
                    AoActSetFrame(main_work.act[(int)ACT_WIN_TEX_MSG]
                                  , dm_stgslct_win_act_frm_tbl[(int)DME_STGSLCT_WIN_BOSS1_CAN_PLAY][1]);
#if !_IPHONE
                    AoActSetFrame(main_work.act[ACT_TEX_OK], 0.0f);
#endif //!_IPHONE

                    break;

                case (int)DME_STGSLCT_WIN_BOSS2_CAN_PLAY:
                    AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
                    AoActSortRegAction(main_work.act[(int)ACT_WIN_TEX_MSG]);

#if !_IPHONE
                    AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[4]));
                    AoActSortRegAction(main_work.act[ACT_TEX_OK]);

                    // 初期表示フレーム設定
                    AoActSetFrame(main_work.act[ACT_TEX_WINTITLE]
                                  , dm_stgslct_win_act_frm_tbl[DME_STGSLCT_WIN_BOSS2_CAN_PLAY][0]);
#endif //!_IPHONE
                    AoActSetFrame(main_work.act[(int)ACT_WIN_TEX_MSG]
                                  , dm_stgslct_win_act_frm_tbl[(int)DME_STGSLCT_WIN_BOSS2_CAN_PLAY][1]);
#if !_IPHONE
                    AoActSetFrame(main_work.act[ACT_TEX_OK], 0.0f);
#endif //!_IPHONE

                    break;

                case (int)DME_STGSLCT_WIN_BOSS3_CAN_PLAY:
                    AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
                    AoActSortRegAction(main_work.act[(int)ACT_WIN_TEX_MSG]);

#if !_IPHONE
                    AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[4]));
                    AoActSortRegAction(main_work.act[ACT_TEX_OK]);

                    // 初期表示フレーム設定
                    AoActSetFrame(main_work.act[ACT_TEX_WINTITLE]
                                  , dm_stgslct_win_act_frm_tbl[DME_STGSLCT_WIN_BOSS3_CAN_PLAY][0]);
#endif //!_IPHONE
                    AoActSetFrame(main_work.act[(int)ACT_WIN_TEX_MSG]
                                  , dm_stgslct_win_act_frm_tbl[(int)DME_STGSLCT_WIN_BOSS3_CAN_PLAY][1]);
#if !_IPHONE
                    AoActSetFrame(main_work.act[ACT_TEX_OK], 0.0f);
#endif //!_IPHONE

                    break;

                case (int)DME_STGSLCT_WIN_BOSS4_CAN_PLAY:
                    AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
                    AoActSortRegAction(main_work.act[(int)ACT_WIN_TEX_MSG]);

#if !_IPHONE
                    AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[4]));
                    AoActSortRegAction(main_work.act[ACT_TEX_OK]);

                    // 初期表示フレーム設定
                    AoActSetFrame(main_work.act[ACT_TEX_WINTITLE]
                                  , dm_stgslct_win_act_frm_tbl[DME_STGSLCT_WIN_BOSS4_CAN_PLAY][0]);
#endif //!_IPHONE
                    AoActSetFrame(main_work.act[(int)ACT_WIN_TEX_MSG]
                                  , dm_stgslct_win_act_frm_tbl[(int)DME_STGSLCT_WIN_BOSS4_CAN_PLAY][1]);
#if !_IPHONE
                    AoActSetFrame(main_work.act[ACT_TEX_OK], 0.0f);
#endif //!_IPHONE

                    break;

                case (int)DME_STGSLCT_WIN_FINAL_CAN_SLCT:
                    AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
                    AoActSortRegAction(main_work.act[(int)ACT_WIN_TEX_MSG]);

#if !_IPHONE
                    AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[4]));
                    AoActSortRegAction(main_work.act[ACT_TEX_OK]);

                    // 初期表示フレーム設定
                    AoActSetFrame(main_work.act[ACT_TEX_WINTITLE]
                                  , dm_stgslct_win_act_frm_tbl[DME_STGSLCT_WIN_FINAL_CAN_SLCT][0]);
#endif //!_IPHONE
                    AoActSetFrame(main_work.act[(int)ACT_WIN_TEX_MSG]
                                  , dm_stgslct_win_act_frm_tbl[(int)DME_STGSLCT_WIN_FINAL_CAN_SLCT][1]);
#if !_IPHONE
                    AoActSetFrame(main_work.act[ACT_TEX_OK], 0.0f);
#endif //!_IPHONE

                    break;

                case (int)DME_STGSLCT_WIN_SSONIC_SLCT:
                    AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
                    AoActSortRegAction(main_work.act[(int)ACT_WIN_TEX_MSG_SSONIC]);
                    //			AoActSortRegAction(main_work.act[ACT_WIN_TEX_MSG]);

#if !_IPHONE
                    AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[4]));
                    AoActSortRegAction(main_work.act[ACT_TEX_OK]);

                    // 初期表示フレーム設定
                    AoActSetFrame(main_work.act[ACT_TEX_WINTITLE]
                                  , dm_stgslct_win_act_frm_tbl[DME_STGSLCT_WIN_SSONIC_SLCT][0]);
                    //			AoActSetFrame(main_work.act[ACT_WIN_TEX_MSG]
                    //						  , dm_stgslct_win_act_frm_tbl[DME_STGSLCT_WIN_SSONIC_SLCT][1]);
                    AoActSetFrame(main_work.act[ACT_TEX_OK], 0.0f);
#endif //!_IPHONE

                    break;

                case (int)DME_STGSLCT_WIN_SPESTE_CAN_SLCT:
                    AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
                    AoActSortRegAction(main_work.act[(int)ACT_WIN_TEX_MSG]);

#if !_IPHONE
                    AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[4]));
                    AoActSortRegAction(main_work.act[ACT_TEX_OK]);

                    // 初期表示フレーム設定
                    AoActSetFrame(main_work.act[ACT_TEX_WINTITLE]
                                  , dm_stgslct_win_act_frm_tbl[DME_STGSLCT_WIN_SPESTE_CAN_SLCT][0]);
#endif //!_IPHONE
                    AoActSetFrame(main_work.act[(int)ACT_WIN_TEX_MSG]
                                  , dm_stgslct_win_act_frm_tbl[(int)DME_STGSLCT_WIN_SPESTE_CAN_SLCT][1]);
#if !_IPHONE
                    AoActSetFrame(main_work.act[ACT_TEX_OK], 0.0f);
#endif //!_IPHONE

                    break;

                default:
                    // 例外
                    amAssert(0);
                    break;
            }


            if (main_work.is_jp_region)
            {
                AoActSetFrame(main_work.act[(int)ACT_BTN_CANCEL2], (float)0.0f);
            }
            else
            {
                AoActSetFrame(main_work.act[(int)ACT_BTN_CANCEL2], (float)1.0f);
            }


#if !_IPHONE
            // ACCUMURATEでトランスさせる
            AoActAcmPush();

            AoActAcmInit();
            AoActAcmApplyTrans(dm_stgslct_win_act_pos_tbl[0][0]
                               , dm_stgslct_win_act_pos_tbl[0][1]
                               , 0
                               );

            AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[3]));
            AoActUpdate(main_work.act[ACT_WIN_LINE], 0.0f);

            AoActAcmPop();
#endif //!_IPHONE

            AoActAcmPush();

            for (int k = 0; k < 3; k++)
            {
                AoActAcmInit();

                AoActAcmApplyTrans(dm_stgslct_win_act_pos_tbl[k + 1][0]
                                   , dm_stgslct_win_act_pos_tbl[k + 1][1]
                                   , 0
                                   );

                if (k == 0)
                {   // 戻るテキストの言語別の長さをずらす
                    AoActAcmApplyTrans(dm_stgslct_back_text_length_tbl[GsEnvGetLanguage()]
                                       , 0
                                       , 0
                                       );
#if _WII       // Wii版のみ左へ10ピクセルさらにずらせる                                
				AoActAcmApplyTrans(-10.0f, 0, 0);
#endif
                }

                AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[1]));
                AoActUpdate(main_work.act[(int)(ACT_BTN_CANCEL2 + k)], 0.0f);
            }
            AoActAcmPop();

            AoActAcmPush();
            for (int k = 0; k < 5; k++)
            {
                AoActAcmInit();

                AoActAcmApplyTrans(dm_stgslct_win_act_pos_tbl[4 + k][0]
                                   , dm_stgslct_win_act_pos_tbl[4 + k][1]
                                   , 0
                                   );

#if _WII || _IPHONE
                AoActAcmApplyScale(DMD_STGSLCT_DISP_SCALE_TEXT
                                   , DMD_STGSLCT_DISP_SCALE_TEXT);
#endif

                AoActSetTexture(AoTexGetTexList(main_work.tex[1]));
                AoActUpdate(main_work.act[(int)(ACT_WIN_TEX_MSG + k)], 0.0f);
            }
            AoActAcmPop();

            AoActAcmPush();
#if !_IPHONE
            for (int k = 0; k < 5; k++)
            {
#else //!_IPHONE
            const int ACT_TEX_WINTITLE = (int)ACT_TEX_BACK1; //ACT_TEX_WINTITLEのダミーを作成
            for (int k = 0; k < (ACT_TEX_NO - ACT_TEX_BACK1 + 1); k++)
            {
#endif //!_IPHONE
                AoActAcmInit();

#if _WII
			if (k != ACT_TEX_BACK1 - ACT_TEX_WINTITLE) {
				AoActAcmApplyScale(DMD_STGSLCT_DISP_SCALE_TEXT
								   , DMD_STGSLCT_DISP_SCALE_TEXT);
			}
#endif //_WII || _IPHONE

#if _WII
			if (k == ACT_TEX_OK - ACT_TEX_WINTITLE) {
				AoActAcmApplyTrans(0
								   , 32.0f
								   , 0
								   );
			}
#endif //_WII

#if !_IPHONE
                AoActAcmApplyTrans(dm_stgslct_win_act_pos_tbl[9 + k][0]
                                   , dm_stgslct_win_act_pos_tbl[9 + k][1]
                                   , 0
                                   );
#else //!_IPHONE
                AoActAcmApplyTrans(dm_stgslct_win_act_pos_tbl[11 + k][0]
                                   , dm_stgslct_win_act_pos_tbl[11 + k][1]
                                   , 0
                                   );
#endif //!_IPHONE

                if (main_work.win_mode == (int)DME_STGSLCT_WIN_SSONIC_SLCT
                    && k == 2)
                {
                    AoActAcmApplyTrans(0
                                       , 16.0f
                                       , 0
                                       );
                }

                AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[4]));
                AoActUpdate(main_work.act[ACT_TEX_WINTITLE + k], 0.0f);

#if _IPHONE  //当たり判定処理          
                {
                    CTrgAoAction ctrgAoAction;
                    switch (ACT_TEX_WINTITLE + k)
                    {
                        case (int)ACT_TEX_YES:
                            ctrgAoAction = main_work.trg_answer[1];
                            AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[3]));
                            ArrayPointer<AOS_ACTION> arrayPointer1 = new ArrayPointer<AOS_ACTION>(main_work.act, 106);
                            ArrayPointer<AOS_ACTION> arrayPointer2 = new ArrayPointer<AOS_ACTION>(main_work.act, 109);
                            while (arrayPointer1 != arrayPointer2)
                            {
                                float frame = 2.0 <= (~arrayPointer1).frame ? 1f : 0.0f;
                                AoActUpdate(arrayPointer1, frame);
                                ++arrayPointer1;
                            }

                            break;
                        case (int)ACT_TEX_NO:
                            ctrgAoAction = main_work.trg_answer[0];
                            AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[3]));
                            ArrayPointer<AOS_ACTION> arrayPointer3 = new ArrayPointer<AOS_ACTION>(main_work.act, 109);
                            ArrayPointer<AOS_ACTION> arrayPointer4 = new ArrayPointer<AOS_ACTION>(main_work.act, 112);
                            while (arrayPointer3 != arrayPointer4)
                            {
                                float frame = 2.0 <= (~arrayPointer3).frame ? 1f : 0.0f;
                                AoActUpdate(arrayPointer3, frame);
                                ++arrayPointer3;
                            }

                            break;
                        default:
                            ctrgAoAction = null;
                            break;
                    }

                    ctrgAoAction?.Update();
                }
#endif //_IPHONE	//当たり判定処理
            }

            AoActAcmPop();

#if _IPHONE
            if (_am_sample_draw_enable)
            {
#endif //_IPHONE
                // ソート実行
                AoActSortExecute();

                // ソートバッファ描画
                AoActSortDraw();
#if _IPHONE
            }
#endif //_IPHONE

            // ソートバッファ全解除
            AoActSortUnregAll();
        }
    }




    // ==========================================================================
    // dmStgSlctIsDataLoad
    /*!
        データ読み込み完了チェック処理
     */
    // ==========================================================================
    private static bool dmStgSlctIsDataLoad(DMS_STGSLCT_MAIN_WORK main_work)
    {

        for (int i = 0; i < 2; i++)
        {
            if (!amFsIsComplete(main_work.arc_amb_fs[i]))
            {
                return false;
            }
        }

        // メニュー共通データ読み込み
        for (int i = 0; i < 5; i++)
        {
            if (!amFsIsComplete(main_work.arc_cmn_amb_fs[i]))
            {
                return false;
            }
        }


        return true;
    }



    // ==========================================================================
    // dmStgSlctIsTexLoad
    /*!
        テクスチャ構築完了チェック処理
     */
    // ==========================================================================
    private static bool dmStgSlctIsTexLoad(DMS_STGSLCT_MAIN_WORK main_work)
    {

        for (int i = 0; i < (int)DME_STGSLCT_DATA_TYPE_MAX; i++)
        {
            if (!AoTexIsLoaded(main_work.tex[i]))
            {
                // フラグ扱いでON
                return false;
            }
        }

        if (!GsFontIsBuilded())
        {
            return false;
        }

        return true;
    }


    // ==========================================================================
    // dmStgSlctIsTexLoad2
    /*!
        テクスチャ構築完了チェック処理		◆暫定対応
     */
    // ==========================================================================
    private static bool dmStgSlctIsTexLoad2(DMS_STGSLCT_MAIN_WORK main_work)
    {
        // メニュー共通データ
        for (int i = 0; i < 5; i++)
        {
            if (!AoTexIsLoaded(main_work.cmn_tex[i]))
            {
                // フラグ扱いでON
                return false;
            }
        }

        return true;
    }


    // ==========================================================================
    // dmStgSlctIsTexRelease
    /*!
        テクスチャ解放完了チェック処理
     */
    // ==========================================================================
    private static bool dmStgSlctIsTexRelease(DMS_STGSLCT_MAIN_WORK main_work)
    {

        for (int i = 0; i < (int)DME_STGSLCT_DATA_TYPE_MAX; i++)
        {
            if (!AoTexIsReleased(main_work.tex[i]))
            {
                // フラグ扱いでON
                return false;
            }
        }

        // メニュー共通データ
        for (int i = 0; i < 5; i++)
        {
            if (!AoTexIsReleased(main_work.cmn_tex[i]))
            {
                return false;
            }
        }

        return true;
    }



    // ==========================================================================
    // dmStgSlctSetDecideZoneEfctPos
    /*!
        ゾーン選択時のゾーンテーブル決定演出中処理
     */
    // ==========================================================================
    private static void dmStgSlctSetDecideZoneEfctPos(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (main_work.timer < DMD_STGSLCT_ZONE_DECIDE_EFCT_ON)
        {
            main_work.decide_zone_efct_dist_x = 0;
            main_work.decide_zone_efct_dist_y = DMD_STGSLCT_ZONE_DECIDE_EFCT_ON_POS_Y;
        }
        else
        {
            main_work.decide_zone_efct_dist_x = 0;
            main_work.decide_zone_efct_dist_y = 0;
        }
    }



    // ==========================================================================
    // dmStgSlctIsDecideZoneEfctPos
    /*!
        ゾーン選択時のゾーンテーブル決定演出中処理
     */
    // ==========================================================================
    private static bool dmStgSlctIsDecideZoneEfctPos(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (main_work.timer > DMD_STGSLCT_ZONE_DECIDE_EFCT_TIME)
        {
            return true;
        }

        return false;
    }



    // ==========================================================================
    // dmStgSlctSetZonePosOutEfct
    /*!
        ゾーン選択時のゾーンテーブル掃け演出中処理
     */
    // ==========================================================================
    private static void dmStgSlctSetZonePosOutEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        float[] move_dist = [0, 0];
        float[] distance = [0, 0];


        // 入り演出が終了したら
        //	if (dmStgSlctIsZonePosOutEfct(main_work)) {
        //		main_work.proc_menu_update = dmStgSlctProcStageSelectIdle;
        //	}

        // 掃け演出分の座標更新
        for (uint i = 0; i < (uint)DME_STGSLCT_ZONE_TYPE_NUM; i++)
        {
            if ((main_work.efct_out_flag & (1u << (int)i)) != 0
                && i != main_work.cur_zone)
            {
                if (main_work.is_final_open == DMD_STGSLCT_ZONE_ALL_OPEN)
                {
                    // ZONEテーブルの初期表示位置設定
                    distance[0] = dm_stgslct_a_zone_nodisp_pos_tbl[i][0] - dm_stgslct_a_zone_disp_pos_tbl[i][0];
                    distance[1] = dm_stgslct_a_zone_nodisp_pos_tbl[i][1] - dm_stgslct_a_zone_disp_pos_tbl[i][1];
                }

                else if ((main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN) != 0)
                {
                    // ZONEテーブルの初期表示位置設定
                    distance[0] = dm_stgslct_s_zone_nodisp_pos_tbl[i][0] - dm_stgslct_s_zone_disp_pos_tbl[i][0];
                    distance[1] = dm_stgslct_s_zone_nodisp_pos_tbl[i][1] - dm_stgslct_s_zone_disp_pos_tbl[i][1];
                }

                else if ((main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN) != 0)
                {
                    // ZONEテーブルの初期表示位置設定
                    distance[0] = dm_stgslct_f_zone_nodisp_pos_tbl[i][0] - dm_stgslct_f_zone_disp_pos_tbl[i][0];
                    distance[1] = dm_stgslct_f_zone_nodisp_pos_tbl[i][1] - dm_stgslct_f_zone_disp_pos_tbl[i][1];
                }
                else
                {
                    // ZONEテーブルの初期表示位置設定
                    distance[0] = dm_stgslct_n_zone_nodisp_pos_tbl[i][0] - dm_stgslct_n_zone_disp_pos_tbl[i][0];
                    distance[1] = dm_stgslct_n_zone_nodisp_pos_tbl[i][1] - dm_stgslct_n_zone_disp_pos_tbl[i][1];
                }

                move_dist[0] = distance[0] / DMD_STGSLCT_ZONE_EFCT_TIME;
                move_dist[1] = distance[1] / DMD_STGSLCT_ZONE_EFCT_TIME;

                main_work.zone_pos[i][0] += move_dist[0];
                main_work.zone_pos[i][1] += move_dist[1];
            }
        }

        // 決定ZONEの移動演出分


        // エメラルドテーブルの掃け演出分
        dmStgSlctSetEmeTableOutEfct(main_work);
    }




    // ==========================================================================
    // dmStgSlctIsZonePosOutEfct
    /*!
        ゾーン選択時のゾーンテーブル掃け演出中処理
     */
    // ==========================================================================
    private static bool dmStgSlctIsZonePosOutEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {

        // 掃け演出終了チェック
        if (main_work.is_final_open == DMD_STGSLCT_ZONE_ALL_OPEN)
        {
            if (main_work.zone_pos[0][0] > dm_stgslct_a_zone_nodisp_pos_tbl[0][0]
                || main_work.zone_pos[0][1] > dm_stgslct_a_zone_nodisp_pos_tbl[0][1])
            {
                if (main_work.cur_zone != 0)
                {
                    return false;
                }
            }
            if (main_work.zone_pos[1][0] > dm_stgslct_a_zone_nodisp_pos_tbl[1][0]
                || main_work.zone_pos[1][1] > dm_stgslct_a_zone_nodisp_pos_tbl[1][1])
            {
                if (main_work.cur_zone != 1)
                {
                    return false;
                }
            }
            if (main_work.zone_pos[2][0] < dm_stgslct_a_zone_nodisp_pos_tbl[2][0]
                || main_work.zone_pos[2][1] > dm_stgslct_a_zone_nodisp_pos_tbl[2][1])
            {
                if (main_work.cur_zone != 2)
                {
                    return false;
                }
            }
            if (main_work.zone_pos[3][0] < dm_stgslct_a_zone_nodisp_pos_tbl[3][0]
                || main_work.zone_pos[3][1] < dm_stgslct_a_zone_nodisp_pos_tbl[3][1])
            {
                if (main_work.cur_zone != 3)
                {
                    return false;
                }
            }
            if (main_work.zone_pos[4][0] > dm_stgslct_a_zone_nodisp_pos_tbl[4][0]
                || main_work.zone_pos[4][1] < dm_stgslct_a_zone_nodisp_pos_tbl[4][1])
            {
                if (main_work.cur_zone != 4)
                {
                    return false;
                }
            }
            if (main_work.zone_pos[5][0] > dm_stgslct_a_zone_nodisp_pos_tbl[5][0]               // ◆
                || main_work.zone_pos[5][1] > dm_stgslct_a_zone_nodisp_pos_tbl[5][1])
            {
                if (main_work.cur_zone != 5)
                {
                    return false;
                }
            }
        }
        else if ((main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN) != 0)
        {
            if (main_work.zone_pos[0][0] > dm_stgslct_s_zone_nodisp_pos_tbl[0][0]
                || main_work.zone_pos[0][1] > dm_stgslct_s_zone_nodisp_pos_tbl[0][1])
            {
                if (main_work.cur_zone != 0)
                {
                    return false;
                }
            }
            if (main_work.zone_pos[1][0] > dm_stgslct_s_zone_nodisp_pos_tbl[1][0]
                || main_work.zone_pos[1][1] > dm_stgslct_s_zone_nodisp_pos_tbl[1][1])
            {
                if (main_work.cur_zone != 1)
                {
                    return false;
                }
            }
            if (main_work.zone_pos[2][0] < dm_stgslct_s_zone_nodisp_pos_tbl[2][0]
                || main_work.zone_pos[2][1] > dm_stgslct_s_zone_nodisp_pos_tbl[2][1])
            {
                if (main_work.cur_zone != 2)
                {
                    return false;
                }
            }
            if (main_work.zone_pos[3][0] < dm_stgslct_s_zone_nodisp_pos_tbl[3][0]
                || main_work.zone_pos[3][1] < dm_stgslct_s_zone_nodisp_pos_tbl[3][1])
            {
                if (main_work.cur_zone != 3)
                {
                    return false;
                }
            }
            //		if (main_work.zone_pos[4][0] > dm_stgslct_s_zone_nodisp_pos_tbl[4][0]
            //			|| main_work.zone_pos[4][1] < dm_stgslct_s_zone_nodisp_pos_tbl[4][1]) {
            //			if (main_work.cur_zone != 4) {
            //				return false;
            //			}
            //		}
            if (main_work.zone_pos[5][0] > dm_stgslct_s_zone_nodisp_pos_tbl[5][0]
                || main_work.zone_pos[5][1] < dm_stgslct_s_zone_nodisp_pos_tbl[5][1])
            {
                if (main_work.cur_zone != 5)
                {
                    return false;
                }
            }
        }
        else if ((main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN) != 0)
        {
            if (main_work.zone_pos[0][0] > dm_stgslct_f_zone_nodisp_pos_tbl[0][0]
                || main_work.zone_pos[0][1] > dm_stgslct_f_zone_nodisp_pos_tbl[0][1])
            {
                if (main_work.cur_zone != 0)
                {
                    return false;
                }
            }
            if (main_work.zone_pos[1][0] > dm_stgslct_f_zone_nodisp_pos_tbl[1][0]
                || main_work.zone_pos[1][1] > dm_stgslct_f_zone_nodisp_pos_tbl[1][1])
            {
                if (main_work.cur_zone != 1)
                {
                    return false;
                }
            }
            if (main_work.zone_pos[2][0] < dm_stgslct_f_zone_nodisp_pos_tbl[2][0]
                || main_work.zone_pos[2][1] > dm_stgslct_f_zone_nodisp_pos_tbl[2][1])
            {
                if (main_work.cur_zone != 2)
                {
                    return false;
                }
            }
            if (main_work.zone_pos[3][0] < dm_stgslct_f_zone_nodisp_pos_tbl[3][0]
                || main_work.zone_pos[3][1] < dm_stgslct_f_zone_nodisp_pos_tbl[3][1])
            {
                if (main_work.cur_zone != 3)
                {
                    return false;
                }
            }
            if (main_work.zone_pos[4][0] > dm_stgslct_f_zone_nodisp_pos_tbl[4][0]
                || main_work.zone_pos[4][1] < dm_stgslct_f_zone_nodisp_pos_tbl[4][1])
            {
                if (main_work.cur_zone != 4)
                {
                    return false;
                }
            }
        }
        else
        {
            if (main_work.zone_pos[0][0] > dm_stgslct_n_zone_nodisp_pos_tbl[0][0]
                || main_work.zone_pos[0][1] > dm_stgslct_n_zone_nodisp_pos_tbl[0][1])
            {
                if (main_work.cur_zone != 0)
                {
                    return false;
                }
            }
            if (main_work.zone_pos[1][0] > dm_stgslct_n_zone_nodisp_pos_tbl[1][0]
                || main_work.zone_pos[1][1] < dm_stgslct_n_zone_nodisp_pos_tbl[1][1])
            {
                if (main_work.cur_zone != 1)
                {
                    return false;
                }
            }
            if (main_work.zone_pos[2][0] < dm_stgslct_n_zone_nodisp_pos_tbl[2][0]
                || main_work.zone_pos[2][1] > dm_stgslct_n_zone_nodisp_pos_tbl[2][1])
            {
                if (main_work.cur_zone != 2)
                {
                    return false;
                }
            }
            if (main_work.zone_pos[3][0] < dm_stgslct_n_zone_nodisp_pos_tbl[3][0]
                || main_work.zone_pos[3][1] < dm_stgslct_n_zone_nodisp_pos_tbl[3][1])
            {
                if (main_work.cur_zone != 3)
                {
                    return false;
                }
            }
        }

        main_work.efct_out_flag = 0;


        if (!dmStgSlctIsEmeTableOutEfctEnd(main_work))
        {
            return false;
        }

        return true;
    }


    // ==========================================================================
    // dmStgSlctSetStagePosInEfct
    /*!
        ステージ選択時のステージテーブル入り演出中処理
     */
    // ==========================================================================
    private static void dmStgSlctSetStagePosInEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        float[] move_dist = [0, 0];
        float[] distance = [0, 0];
        uint act_num_start = 0;
        uint act_num_end = 0;

        act_num_start = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
        act_num_end = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] + act_num_start;

        // 入り演出分の座標更新
        if (main_work.act_top_pos_x[act_num_start] > DMD_STGSLCT_ACT_TABLE_TOP_POS_X)
        {
            for (uint i = act_num_start; i < act_num_end; i++)
            {
                distance[0] = DMD_STGSLCT_ACT_TABLE_TOP_POS_X - DMD_STGSLCT_STAGE_TAB_NODISP_POS_X;

                move_dist[0] = distance[0] / DMD_STGSLCT_ZONE_EFCT_TIME;

                main_work.act_top_pos_x[i] += move_dist[0];
            }
        }

        // 決定ZONEの移動演出分
        dmStgSlctSetDecideZonePosOutEfct(main_work);

        // ゲームモードテキストの入り演出分
        dmStgSlctSetModeTexInEfct(main_work);
    }


    // ==========================================================================
    // dmStgSlctIsStagePosInEfct
    /*!
        ステージ選択時のステージテーブル入り演出中処理
     */
    // ==========================================================================
    private static bool dmStgSlctIsStagePosInEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        //	uint is_efct_end = 0;
        uint act_num_start = 0;
        uint act_num_end = 0;

        act_num_start = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
        act_num_end = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] + act_num_start;

        if (!dmStgSlctIsModeTexInEfctEnd(main_work))
        {
            return false;
        }

        // 掃け演出終了チェック
        if (main_work.act_top_pos_x[act_num_start] <= DMD_STGSLCT_ACT_TABLE_TOP_POS_X
            && main_work.zone_pos[main_work.cur_zone][0] < dm_stgslct_a_zone_nodisp_pos_tbl[0][0])
        {
            for (uint i = act_num_start; i < act_num_end; i++)
            {
                main_work.act_top_pos_x[i] = DMD_STGSLCT_ACT_TABLE_TOP_POS_X;
            }
            main_work.zone_pos[main_work.cur_zone][0] = dm_stgslct_a_zone_nodisp_pos_tbl[0][0];

            return true;
        }

        return false;
    }



    // ==========================================================================
    // dmStgSlctSetStagePosOutEfct
    /*!
        ステージ選択時のステージテーブル掃け演出中処理
     */
    // ==========================================================================
    private static void dmStgSlctSetStagePosOutEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        float[] move_dist = [0, 0];
        float[] distance = [0, 0];
        uint act_num_start = 0;
        uint act_num_end = 0;

        act_num_start = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
        act_num_end = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] + act_num_start;

        // 入り演出分の座標更新
        for (uint i = act_num_start; i < act_num_end; i++)
        {
            //		if (main_work.efct_out_flag & (1 << i)) {
            distance[0] = DMD_STGSLCT_STAGE_TAB_NODISP_POS_X - DMD_STGSLCT_ACT_TABLE_TOP_POS_X;

            move_dist[0] = distance[0] / DMD_STGSLCT_ZONE_EFCT_TIME;

            main_work.act_top_pos_x[i] += move_dist[0];
            //		}
        }

        // ゲームモードテキストの掃け演出
        dmStgSlctSetModeTexOutEfct(main_work);
    }



    // ==========================================================================
    // dmStgSlctIsStagePosOutEfct
    /*!
        ステージ選択時のステージテーブル掃け演出中処理
     */
    // ==========================================================================
    private static bool dmStgSlctIsStagePosOutEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        //	uint is_efct_end = 0;
        uint act_num_start = 0;
        uint act_num_end = 0;

        act_num_start = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
        act_num_end = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] + act_num_start;

        // ゲームモードテキストの掃け演出終了チェック
        if (!dmStgSlctIsModeTexOutEfctEnd(main_work))
        {
            return false;
        }

        // 掃け演出終了チェック
        if (main_work.act_top_pos_x[act_num_start] >= DMD_STGSLCT_STAGE_TAB_NODISP_POS_X)
        {
            for (uint i = act_num_start; i < act_num_end; i++)
            {
                main_work.act_top_pos_x[i] = DMD_STGSLCT_STAGE_TAB_NODISP_POS_X;
            }
            //		main_work.act_top_pos_x[1] = DMD_STGSLCT_STAGE_TAB_NODISP_POS_X;
            //		main_work.act_top_pos_x[2] = DMD_STGSLCT_STAGE_TAB_NODISP_POS_X;
            //		main_work.act_top_pos_x[3] = DMD_STGSLCT_STAGE_TAB_NODISP_POS_X;

            return true;
        }

        return false;
    }



    // ==========================================================================
    // dmStgSlctSetZonePosInEfct
    /*!
        ゾーン選択時のゾーンテーブル入り演出中処理
     */
    // ==========================================================================
    private static void dmStgSlctSetZonePosInEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        float[] move_dist = [0, 0];
        float[] distance = [0, 0];

        // 掃け演出分の座標更新
        for (int i = 0; i < (int)DME_STGSLCT_ZONE_TYPE_NUM; i++)
        {
            if (main_work.is_final_open == DMD_STGSLCT_ZONE_ALL_OPEN)
            {
                // ZONEテーブルの初期表示位置設定
                distance[0] = dm_stgslct_a_zone_disp_pos_tbl[i][0] - dm_stgslct_a_zone_nodisp_pos_tbl[i][0];
                distance[1] = dm_stgslct_a_zone_disp_pos_tbl[i][1] - dm_stgslct_a_zone_nodisp_pos_tbl[i][1];
            }

            else if ((main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN) != 0)
            {
                // ZONEテーブルの初期表示位置設定
                distance[0] = dm_stgslct_s_zone_disp_pos_tbl[i][0] - dm_stgslct_s_zone_nodisp_pos_tbl[i][0];
                distance[1] = dm_stgslct_s_zone_disp_pos_tbl[i][1] - dm_stgslct_s_zone_nodisp_pos_tbl[i][1];
            }

            else if ((main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN) != 0)
            {
                // ZONEテーブルの初期表示位置設定
                distance[0] = dm_stgslct_f_zone_disp_pos_tbl[i][0] - dm_stgslct_f_zone_nodisp_pos_tbl[i][0];
                distance[1] = dm_stgslct_f_zone_disp_pos_tbl[i][1] - dm_stgslct_f_zone_nodisp_pos_tbl[i][1];
            }
            else
            {
                // ZONEテーブルの初期表示位置設定
                distance[0] = dm_stgslct_n_zone_disp_pos_tbl[i][0] - dm_stgslct_n_zone_nodisp_pos_tbl[i][0];
                distance[1] = dm_stgslct_n_zone_disp_pos_tbl[i][1] - dm_stgslct_n_zone_nodisp_pos_tbl[i][1];
            }

            move_dist[0] = distance[0] / DMD_STGSLCT_ZONE_EFCT_TIME;
            move_dist[1] = distance[1] / DMD_STGSLCT_ZONE_EFCT_TIME;

            main_work.zone_pos[i][0] += move_dist[0];
            main_work.zone_pos[i][1] += move_dist[1];
        }

        // エメラルドテーブルの入り演出設定
        dmStgSlctSetEmeTableInEfct(main_work);

        main_work.timer++;
    }




    // ==========================================================================
    // dmStgSlctIsZonePosInEfct
    /*!
        ゾーン選択時のゾーンテーブル入り演出中処理
     */
    // ==========================================================================
    private static bool dmStgSlctIsZonePosInEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        // 掃け演出終了チェック
        if (main_work.is_final_open == DMD_STGSLCT_ZONE_ALL_OPEN)
        {
            if (main_work.zone_pos[0][0] < dm_stgslct_a_zone_disp_pos_tbl[0][0]
                || main_work.zone_pos[0][1] < dm_stgslct_a_zone_disp_pos_tbl[0][1])
            {
                return false;
            }
            if (main_work.zone_pos[1][0] < dm_stgslct_a_zone_disp_pos_tbl[1][0]
                || main_work.zone_pos[1][1] < dm_stgslct_a_zone_disp_pos_tbl[1][1])
            {
                return false;
            }
            if (main_work.zone_pos[2][0] > dm_stgslct_a_zone_disp_pos_tbl[2][0]
                || main_work.zone_pos[2][1] < dm_stgslct_a_zone_disp_pos_tbl[2][1])
            {
                return false;
            }
            if (main_work.zone_pos[3][0] > dm_stgslct_a_zone_disp_pos_tbl[3][0]
                || main_work.zone_pos[3][1] > dm_stgslct_a_zone_disp_pos_tbl[3][1])
            {
                return false;
            }
            if (main_work.zone_pos[4][0] < dm_stgslct_a_zone_disp_pos_tbl[4][0]
                || main_work.zone_pos[4][1] < dm_stgslct_a_zone_disp_pos_tbl[4][1])
            {
                return false;
            }
            if (main_work.zone_pos[5][0] < dm_stgslct_a_zone_disp_pos_tbl[5][0]             // ◆
                || main_work.zone_pos[5][1] > dm_stgslct_a_zone_disp_pos_tbl[5][1])
            {
                return false;
            }
        }
        else if ((main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN) != 0)
        {
            if (main_work.zone_pos[0][0] < dm_stgslct_s_zone_disp_pos_tbl[0][0]
                || main_work.zone_pos[0][1] < dm_stgslct_s_zone_disp_pos_tbl[0][1])
            {
                return false;
            }
            if (main_work.zone_pos[1][0] < dm_stgslct_s_zone_disp_pos_tbl[1][0]
                || main_work.zone_pos[1][1] < dm_stgslct_s_zone_disp_pos_tbl[1][1])
            {
                return false;
            }
            if (main_work.zone_pos[2][0] > dm_stgslct_s_zone_disp_pos_tbl[2][0]
                || main_work.zone_pos[2][1] < dm_stgslct_s_zone_disp_pos_tbl[2][1])
            {
                return false;
            }
            if (main_work.zone_pos[3][0] > dm_stgslct_s_zone_disp_pos_tbl[3][0]
                || main_work.zone_pos[3][1] > dm_stgslct_s_zone_disp_pos_tbl[3][1])
            {
                return false;
            }
            //		if (main_work.zone_pos[4][0] < dm_stgslct_s_zone_disp_pos_tbl[4][0]
            //			|| main_work.zone_pos[4][1] > dm_stgslct_s_zone_disp_pos_tbl[4][1]) {
            //			return false;
            //		}
            if (main_work.zone_pos[5][0] < dm_stgslct_s_zone_disp_pos_tbl[5][0]
                || main_work.zone_pos[5][1] > dm_stgslct_s_zone_disp_pos_tbl[5][1])
            {
                return false;
            }
        }
        else if ((main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN) != 0)
        {
            if (main_work.zone_pos[0][0] < dm_stgslct_f_zone_disp_pos_tbl[0][0]
                || main_work.zone_pos[0][1] < dm_stgslct_f_zone_disp_pos_tbl[0][1])
            {
                return false;
            }
            if (main_work.zone_pos[1][0] < dm_stgslct_f_zone_disp_pos_tbl[1][0]
                || main_work.zone_pos[1][1] < dm_stgslct_f_zone_disp_pos_tbl[1][1])
            {
                return false;
            }
            if (main_work.zone_pos[2][0] > dm_stgslct_f_zone_disp_pos_tbl[2][0]
                || main_work.zone_pos[2][1] < dm_stgslct_f_zone_disp_pos_tbl[2][1])
            {
                return false;
            }
            if (main_work.zone_pos[3][0] > dm_stgslct_f_zone_disp_pos_tbl[3][0]
                || main_work.zone_pos[3][1] > dm_stgslct_f_zone_disp_pos_tbl[3][1])
            {
                return false;
            }
            if (main_work.zone_pos[4][0] < dm_stgslct_f_zone_disp_pos_tbl[4][0]
                || main_work.zone_pos[4][1] > dm_stgslct_f_zone_disp_pos_tbl[4][1])
            {
                return false;
            }
        }
        else
        {
            if (main_work.zone_pos[0][0] < dm_stgslct_n_zone_disp_pos_tbl[0][0]
                || main_work.zone_pos[0][1] < dm_stgslct_n_zone_disp_pos_tbl[0][1])
            {
                return false;
            }
            if (main_work.zone_pos[1][0] < dm_stgslct_n_zone_disp_pos_tbl[1][0]
                || main_work.zone_pos[1][1] > dm_stgslct_n_zone_disp_pos_tbl[1][1])
            {
                return false;
            }
            if (main_work.zone_pos[2][0] > dm_stgslct_n_zone_disp_pos_tbl[2][0]
                || main_work.zone_pos[2][1] < dm_stgslct_n_zone_disp_pos_tbl[2][1])
            {
                return false;
            }
            if (main_work.zone_pos[3][0] > dm_stgslct_n_zone_disp_pos_tbl[3][0]
                || main_work.zone_pos[3][1] > dm_stgslct_n_zone_disp_pos_tbl[3][1])
            {
                return false;
            }
        }

        // エメラルドテーブルの入り演出が終了したかどうか
        if (!dmStgSlctIsEmeTableInEfctEnd(main_work))
        {
            return false;
        }


        return true;
    }



    // ==========================================================================
    // dmStgSlctSetDecideZonePosOutEfct
    /*!
        ゾーン選択時の決定ゾーンテーブル掃け演出中処理
     */
    // ==========================================================================
    private static void dmStgSlctSetDecideZonePosOutEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        float[] move_dist = [0, 0];
        float[] distance = [0, 0];

        // 入り演出が終了したら
        //	if (dmStgSlctIsZonePosOutEfct(main_work)) {
        //		main_work.proc_menu_update = dmStgSlctProcStageSelectIdle;
        //	}

        // 掃け演出分の座標更新
        if (main_work.is_final_open == DMD_STGSLCT_ZONE_ALL_OPEN)
        {
            // ZONEテーブルの初期表示位置設定
            distance[0] = dm_stgslct_a_zone_nodisp_pos_tbl[0][0] - dm_stgslct_a_zone_disp_pos_tbl[main_work.cur_zone][0];
        }

        else if ((main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN) != 0)
        {
            // ZONEテーブルの初期表示位置設定
            distance[0] = dm_stgslct_s_zone_nodisp_pos_tbl[0][0] - dm_stgslct_s_zone_disp_pos_tbl[main_work.cur_zone][0];
        }

        else if ((main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN) != 0)
        {
            // ZONEテーブルの初期表示位置設定
            distance[0] = dm_stgslct_f_zone_nodisp_pos_tbl[0][0] - dm_stgslct_f_zone_disp_pos_tbl[main_work.cur_zone][0];
        }
        else
        {
            // ZONEテーブルの初期表示位置設定
            distance[0] = dm_stgslct_n_zone_nodisp_pos_tbl[0][0] - dm_stgslct_n_zone_disp_pos_tbl[main_work.cur_zone][0];
        }

        move_dist[0] = distance[0] / DMD_STGSLCT_ZONE_EFCT_TIME;

        main_work.zone_pos[main_work.cur_zone][0] += move_dist[0];

        // 決定ZONEの移動演出分

    }



    // ==========================================================================
    // dmStgSlctSetStageZoneChangeEfct
    /*!
        ステージ選択時のゾーン切り替え時演出中処理
     */
    // ==========================================================================
    private static void dmStgSlctSetStageZoneChangeEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        float[] move_dist = [0, 0];
        float[] distance = [0, 0];
        //	float move_src = 0;
        //	float move_dest = 0;
        uint act_num_start = 0;
        uint act_num_end = 0;

        // 画面外へ掃ける側
        act_num_start = dm_stgslct_zone_act_num_tbl[main_work.chng_zone][0];
        act_num_end = dm_stgslct_zone_act_num_tbl[main_work.chng_zone][1] + act_num_start;

        // 入り演出分の座標更新
        for (uint i = act_num_start; i < act_num_end; i++)
        {

            distance[0] = main_work.act_move_dest[0] - main_work.act_move_src[0];

            move_dist[0] = distance[0] / DMD_STGSLCT_ZONE_EFCT_TIME;

            main_work.act_top_pos_x[i] += move_dist[0];
        }

        // 画面中央へ入る側
        act_num_start = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
        act_num_end = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] + act_num_start;

        // 入り演出分の座標更新
        for (uint i = act_num_start; i < act_num_end; i++)
        {

            distance[1] = main_work.act_move_dest[1] - main_work.act_move_src[1];

            move_dist[1] = distance[1] / DMD_STGSLCT_ZONE_EFCT_TIME;

            main_work.act_top_pos_x[i] += move_dist[1];
        }
    }



    // ==========================================================================
    // dmStgSlctIsStageZoneChangeEfct
    /*!
        ステージ選択時のゾーン切り替え時演出中処理
     */
    // ==========================================================================
    private static bool dmStgSlctIsStageZoneChangeEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        byte result = 0;
        //	uint is_efct_end = 0;
        uint act_num_start = 0;
        uint act_num_end = 0;

        // 画面外へ掃ける側のチェック
        act_num_start = dm_stgslct_zone_act_num_tbl[main_work.chng_zone][0];
        act_num_end = dm_stgslct_zone_act_num_tbl[main_work.chng_zone][1] + act_num_start;

        // 掃け演出終了チェック
        if (main_work.act_move_dest[0] > 0)
        {
            if (main_work.act_top_pos_x[act_num_start] >= main_work.act_move_dest[0])
            {
                for (uint i = act_num_start; i < act_num_end; i++)
                {
                    main_work.act_top_pos_x[i] = main_work.act_move_dest[0];
                }

                result |= 1 << 0;
            }

            // 画面外へ掃ける側のチェック
            act_num_start = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
            act_num_end = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] + act_num_start;

            if (main_work.act_top_pos_x[act_num_start] >= main_work.act_move_dest[1])
            {
                for (uint i = act_num_start; i < act_num_end; i++)
                {
                    main_work.act_top_pos_x[i] = main_work.act_move_dest[1];
                }

                result |= 1 << 1;
            }
        }
        else if (main_work.act_move_dest[0] < 0)
        {
            if (main_work.act_top_pos_x[act_num_start] <= main_work.act_move_dest[0])
            {
                for (uint i = act_num_start; i < act_num_end; i++)
                {
                    main_work.act_top_pos_x[i] = main_work.act_move_dest[0];
                }

                result |= 1 << 0;
            }

            // 画面外へ掃ける側のチェック
            act_num_start = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
            act_num_end = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] + act_num_start;

            if (main_work.act_top_pos_x[act_num_start] <= main_work.act_move_dest[1])
            {
                for (uint i = act_num_start; i < act_num_end; i++)
                {
                    main_work.act_top_pos_x[i] = main_work.act_move_dest[1];
                }

                result |= 1 << 1;
            }
        }

        if (result != 3)
        {
            return false;
        }

        return true;
    }



    // ==========================================================================
    // dmStgSlctSetStageVrtclChangeEfct
    /*!
        ステージ選択時の縦のACT切り替え時演出中処理
     */
    // ==========================================================================
    private static void dmStgSlctSetStageVrtclChangeEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        float[] move_dist = [0, 0];
        float[] distance = [0, 0];
        //	float move_src = 0;
        //	float move_dest = 0;
        uint act_num_start = 0;
        uint act_num_end = 0;

        // 画面外へ掃ける側
        act_num_start = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
        act_num_end = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] + act_num_start;

        // 縦移動する場合
        for (uint i = act_num_start; i < act_num_end; i++)
        {

            distance[1] = main_work.act_move_pos_dst[i] - main_work.act_move_pos_src[i];

            move_dist[1] = distance[1] / DMD_STGSLCT_ACT_VRTCL_MOVE_TIME;

            main_work.act_top_pos_y[i] += move_dist[1];
        }
    }



    // ==========================================================================
    // dmStgSlctIsStageVrtclChangeEfct
    /*!
        ステージ選択時の縦のACT切り替え時演出中処理
     */
    // ==========================================================================
    private static bool dmStgSlctIsStageVrtclChangeEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        uint act_num_start = 0;
        uint act_num_end = 0;

        // 画面外へ掃ける側のチェック
        act_num_start = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
        act_num_end = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] + act_num_start;

        // 掃け演出終了チェック
        if (main_work.timer >= DMD_STGSLCT_ACT_VRTCL_MOVE_TIME)
        {

            for (uint i = act_num_start; i < act_num_end; i++)
            {
                main_work.act_top_pos_y[i] = main_work.act_move_pos_dst[i];
            }

            return true;
        }

        return false;
    }




    // ==========================================================================
    // dmStgSlctSetStageCrsrChangeEfct
    /*!
        ステージ選択時の縦のACT切り替え時演出中処理
     */
    // ==========================================================================
    private static void dmStgSlctSetStageCrsrChangeEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        float move_dist = 0;
        float distance = 0;

        // 縦移動する場合
        distance = main_work.crsr_move_dst - main_work.crsr_move_src;

        move_dist = distance / DMD_STGSLCT_CRSR_MOVE_TIME;

        main_work.crsr_pos_y += move_dist;
    }



    // ==========================================================================
    // dmStgSlctIsStageCrsrChangeEfct
    /*!
        ステージ選択時の縦のACT切り替え時演出中処理
     */
    // ==========================================================================
    private static bool dmStgSlctIsStageCrsrChangeEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        // 掃け演出終了チェック
        if (main_work.timer >= DMD_STGSLCT_CRSR_MOVE_TIME)
        {
            main_work.crsr_pos_y = main_work.crsr_move_dst;

            return true;
        }

        return false;
    }




    // ==========================================================================
    // dmStgSlctSetWinOpenEfct
    /*!
        ウインドウ入り演出時のウインドウサイズ設定処理
     */
    // ==========================================================================
    private static void dmStgSlctSetWinOpenEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
#if _IPHONE
        if ((0.0f <= main_work.win_timer) && (main_work.win_timer < 1.0f))
        {
            DmSoundPlaySE("Window");
        }
#endif //_IPHONE
        if (main_work.win_timer > DMD_STGSLCT_WIN_EFCT_TIME)
        {
            // ウインドウ演出終了
            main_work.flag |= DMD_STGSLCT_FLAG_WIN_EFCT_END;

            main_work.win_timer = 0.0f;

            for (uint i = 0; i < 2; i++)
            {
                main_work.win_size_rate[i] = DMD_STGSLCT_WIN_DEF_RATE;
            }
        }
        else
        {
            // タイマー更新(但しフレームレートが可変になるように修正すること)	◆
            main_work.win_timer++;
        }

        // 掃け演出分のサイズ更新
        for (uint i = 0; i < 2; i++)
        {
            if (main_work.win_timer != 0)
            {
                main_work.win_size_rate[i] = main_work.win_timer / DMD_STGSLCT_WIN_EFCT_TIME;
            }
            else
            {
                main_work.win_size_rate[i] = 1.0f;
            }

            if (main_work.win_size_rate[i] > 1.0f)
            {
                main_work.win_size_rate[i] = 1.0f;
            }
        }



    }



    // ==========================================================================
    // dmStgSlctSetWinCloseEfct
    /*!
        ウインドウ閉め演出時のウインドウサイズ設定処理
     */
    // ==========================================================================
    private static void dmStgSlctSetWinCloseEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {

        // 掃け演出分のサイズ更新
        for (uint i = 0; i < 2; i++)
        {
            if (main_work.win_timer != 0)
            {
                main_work.win_size_rate[i] = main_work.win_timer / DMD_STGSLCT_WIN_EFCT_TIME;
            }
            else
            {
                main_work.win_size_rate[i] = 0.0f;
            }
        }

        if (main_work.win_timer < 0.0f)
        {
            // ウインドウ演出終了
            main_work.flag |= DMD_STGSLCT_FLAG_WIN_EFCT_END;

            main_work.win_timer = 0.0f;

            for (uint i = 0; i < 2; i++)
            {
                main_work.win_size_rate[i] = 0.0f;
            }
        }
        else
        {
            // タイマー更新(但しフレームレートが可変になるように修正すること)	◆
            main_work.win_timer--;
        }
    }



    // ==========================================================================
    // dmStgSlctSetActChngZonePosInit
    /*!
        ACT移動演出時の座標初期化設定処理
     */
    // ==========================================================================
    private static void dmStgSlctSetActChngZonePosInit(DMS_STGSLCT_MAIN_WORK main_work, int diff)
    {
        uint act_num_start = 0;
        uint act_num_end = 0;
        uint i = 0;

        // 画面外へ掃ける側
        act_num_start = dm_stgslct_zone_act_num_tbl[main_work.chng_zone][0];
        act_num_end = dm_stgslct_zone_act_num_tbl[main_work.chng_zone][1] + act_num_start;

        // 入り演出分の座標更新
        for (i = act_num_start; i < act_num_end; i++)
        {
            main_work.act_top_pos_x[i] = DMD_STGSLCT_ACT_TABLE_TOP_POS_X;
        }

        main_work.act_move_src[0] = main_work.act_top_pos_x[act_num_start];

        // 画面中央へ入る側
        act_num_start = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
        act_num_end = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] + act_num_start;

        // 入り演出分の座標更新
        for (i = act_num_start; i < act_num_end; i++)
        {
            if (diff > 0)
            {
                main_work.act_top_pos_x[i] = DMD_STGSLCT_STAGE_TAB_NODISP_POS_X;
                main_work.act_top_pos_y[i] = dm_stgslct_act_disp_y_pos_tbl[i];
                //											+ (float)(dm_stgslct_act_disp_pos_y_tbl[i] * DMD_STGSLCT_ACT_TABLE_DIST_Y);
            }
            else
            {
                main_work.act_top_pos_x[i] = DMD_STGSLCT_STAGE_TAB_NODISP_POS_X * (-1);
                main_work.act_top_pos_y[i] = dm_stgslct_act_disp_y_pos_tbl[i];
                //											+ (float)(dm_stgslct_act_disp_pos_y_tbl[i] * DMD_STGSLCT_ACT_TABLE_DIST_Y);
            }
        }

        main_work.act_move_src[1] = main_work.act_top_pos_x[act_num_start];
    }



    // ==========================================================================
    // dmStgSlctSetFocusChangeEfctData
    /*!
        ステージ選択時の縦のACT切り替え時の設定処理
     */
    // ==========================================================================
    private static void dmStgSlctSetFocusChangeEfctData(DMS_STGSLCT_MAIN_WORK main_work)
    {
        // 上移動・下移動共に必要な変数の設定
        int chng_sign = 0;
        int disp_act_no = 0;
        uint src = 0;
        uint dst = 0;
        uint act_comp = 0;

        src = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
        dst = src + dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1];
        act_comp = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] - 1;

        if ((main_work.flag & DMD_STGSLCT_FLAG_UP_CHNG_CRSR) != 0)
        {
            chng_sign = -1;
        }
        else if ((main_work.flag & DMD_STGSLCT_FLAG_DOWN_CHNG_CRSR) != 0)
        {
            chng_sign = 1;
        }
#if !_IPHONE
        else
        {
            MTM_ASSERT(0);
        }
#endif //!_IPHONE

        // 選択ファイル切り替え
        main_work.prev_stage = main_work.cur_stage;

        main_work.prev_disp_no = main_work.focus_disp_no;
        //	main_work.crsr_prev_idx = main_work.crsr_idx;

        if ((main_work.flag & DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL) != 0)
        {

#if !_IPHONE
            main_work.prev_disp_no = main_work.focus_disp_no;
            main_work.focus_disp_no = dmStgSlctGetRevisedStageVrtclNo(main_work.focus_disp_no
                                                                       , chng_sign
                                                                       , (int)main_work.cur_zone
                                                                       , main_work.crsr_idx
                                                                       );

            for (uint i = src; i < dst; i++)
            {
                main_work.act_move_pos_src[i] = main_work.act_top_pos_y[i];
                main_work.act_move_pos_dst[i] = dm_stgslct_act_tab_disp_y_pos_tbl[main_work.focus_disp_no];
            }
#else //!_IPHONE
            switch (main_work.cur_zone)
            {
                case (uint)DME_STGSLCT_ZONE_TYPE_FINAL: //ファイナルステージ
                    for (uint i = src; i < dst; i++)
                    {
                        main_work.act_move_pos_src[i] = main_work.act_top_pos_y[i];
                        main_work.act_move_pos_dst[i] = dm_stgslct_act_disp_y_pos_tbl[i];
                    }
                    break;
                default:
                    for (uint i = src; i < dst; i++)
                    {
                        main_work.act_move_pos_src[i] = main_work.act_top_pos_y[i];
                        main_work.act_move_pos_dst[i] = dm_stgslct_act_tab_disp_y_pos_tbl[main_work.focus_disp_no];
                    }
                    break;
            }
#endif //!_IPHONE

            if ((main_work.flag & DMD_STGSLCT_FLAG_UP_CHNG_CRSR) != 0
                && (main_work.prev_disp_no == 0 && main_work.focus_disp_no == 4 - 1))
            {
                main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_CRSR;
            }

            if ((main_work.flag & DMD_STGSLCT_FLAG_DOWN_CHNG_CRSR) != 0
                && (main_work.prev_disp_no == 4 - 1 && main_work.focus_disp_no == 0))
            {
                main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_CRSR;
            }

            if ((main_work.focus_disp_no == 3 && main_work.prev_disp_no == 0)
                || (main_work.focus_disp_no == 0 && main_work.prev_disp_no == 3))
            {
                main_work.crsr_prev_idx = main_work.crsr_idx;

                disp_act_no = main_work.prev_disp_no;

                main_work.crsr_idx = dmStgSlctGetRevisedStageCrsrNo(main_work.crsr_idx
                                                                     , chng_sign
                                                                     , (int)main_work.cur_zone
                                                                     , main_work.prev_disp_no
                                                                     );

                main_work.crsr_move_src = dm_stgslct_act_crsr_disp_y_pos_tbl[main_work.crsr_prev_idx];
                main_work.crsr_move_dst = dm_stgslct_act_crsr_disp_y_pos_tbl[main_work.crsr_idx];

                main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_ACT_CHNG_CRSR);
            }
            else if ((main_work.flag & DMD_STGSLCT_FLAG_UP_CHNG_CRSR) != 0
                     && main_work.focus_disp_no == 0
                     && main_work.prev_disp_no == 0)
            {

            }
            else if ((main_work.flag & DMD_STGSLCT_FLAG_DOWN_CHNG_CRSR) != 0
                     && main_work.focus_disp_no == 3
                     && main_work.prev_disp_no == 3)
            {

            }
            else
            {
                main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_ACT_CHNG_CRSR);
            }

        }

        if ((main_work.flag & DMD_STGSLCT_FLAG_ACT_CHNG_CRSR) != 0)
        {

            main_work.crsr_prev_idx = main_work.crsr_idx;

            disp_act_no = main_work.prev_disp_no;

            main_work.crsr_idx = dmStgSlctGetRevisedStageCrsrNo(main_work.crsr_idx
                                                                 , chng_sign
                                                                 , (int)main_work.cur_zone
                                                                 , main_work.prev_disp_no
                                                                 );

            main_work.crsr_move_src = dm_stgslct_act_crsr_disp_y_pos_tbl[main_work.crsr_prev_idx];
            main_work.crsr_move_dst = dm_stgslct_act_crsr_disp_y_pos_tbl[main_work.crsr_idx];
        }

        main_work.cur_stage = (int)(main_work.crsr_idx + main_work.focus_disp_no + src);
    }




    // ===========================================================================
    //	dmStgSlctGetRevisedStageNo
    /*!
        項目の場所を示す変数が最大・最小値を超えた際に補正した(回り込みさせた)値を取得する関数
        通常時のカーソル移動の選択項目に使用。

        @param id		[in] 項目番号
        @param diff		[in] 変化量(移動における)
        @return 補正された項目番号
    */
    // ===========================================================================
    /*
    int dmStgSlctGetRevisedStageNo(int idx, int diff, int zone_no)
    {
        int result;
        int src = 0;
        int dst = 0;

        result = (int)idx + diff;

        src = dm_stgslct_zone_act_num_tbl[zone_no][0];
        dst = src + dm_stgslct_zone_act_num_tbl[zone_no][1];

        // 先頭から一つ戻ると最後に移動
        if (result < src) {
            result = (int)(dst - 1);
        }

        // 最後から一つ進むと先頭に移動
        if (result >= dst) {
            result = src;
        }

        MTM_ASSERT(result >= src && result < dst);

        return result;
    }
    */


    // ===========================================================================
    //	dmStgSlctGetRevisedStageVrtclNo
    /*!
        項目の場所を示す変数が最大・最小値を超えた際に補正した(回り込みさせた)値を取得する関数
        通常時のカーソル移動の選択項目に使用。

        @param id		[in] 項目番号
        @param diff		[in] 変化量(移動における)
        @return 補正された項目番号
    */
    // ===========================================================================
    private static int dmStgSlctGetRevisedStageVrtclNo(int idx, int diff, int zone_no, int crsr_idx)
    {
        int result;
        UNREFERENCED_PARAMETER(zone_no);

        result = (int)idx + diff;

        // 先頭から一つ戻ると最後に移動
        if (result < 0)
        {
            if (crsr_idx == 0)
            {
                result = 3;
            }
            else
            {
                result = 0;
            }
        }

        // 最後から一つ進むと先頭に移動
        if (result >= 4)
        {
            if (crsr_idx == 4 - 1)
            {
                result = 0;
            }
            else
            {
                result = 4 - 1;
            }
        }

        MTM_ASSERT(result >= 0 && result < 4);

        return result;
    }



    // ===========================================================================
    //	dmStgSlctGetRevisedStageCrsrNo
    /*!
        項目の場所を示す変数が最大・最小値を超えた際に補正した(回り込みさせた)値を取得する関数
        通常時のカーソル移動の選択項目に使用。

        @param id		[in] 項目番号
        @param diff		[in] 変化量(移動における)
        @return 補正された項目番号
    */
    // ===========================================================================
    private static int dmStgSlctGetRevisedStageCrsrNo(int idx, int diff, int zone_no, int disp_act)
    {
        int result;
        int tmp_no = 0;
        uint src = 0;
        uint dst = 0;

        src = dm_stgslct_zone_act_num_tbl[zone_no][0];
        dst = src + dm_stgslct_zone_act_num_tbl[zone_no][1];
        result = (int)idx + diff;

        tmp_no = (int)(dm_stgslct_zone_act_num_tbl[zone_no][1] - 4);

        if (tmp_no < 0)
        {
            tmp_no = 0;
        }

        // 先頭から一つ戻ると最後に移動
        if (result < 0)
        {
            if (disp_act == 0)
            {
                result = 3;
            }
            else
            {
                result = 0;
            }
        }

        // 最後から一つ進むと先頭に移動
        if (result > 3)
        {
            if (disp_act == tmp_no)
            {
                result = 0;
            }
            else
            {
                result = 3;
            }
        }

        MTM_ASSERT(result >= 0 && result <= 3);

        return result;
    }



    // ===========================================================================
    //	dmStgSlctGetRevisedZoneNo
    /*!
        項目の場所を示す変数が最大・最小値を超えた際に補正した(回り込みさせた)値を取得する関数
        通常時のカーソル移動の選択項目に使用。

        @param id		[in] 項目番号
        @param diff		[in] 変化量(移動における)
        @return 補正された項目番号
    */
    // ===========================================================================
    private static int dmStgSlctGetRevisedZoneNo(int idx, int diff, int is_final_open, int is_spe_open)
    {
        int result;

        // FINALオープンならば
        if (is_final_open != 0)
        {
            result = (int)idx + diff;

            // FINAL、スペステ共にオープン
            if (is_spe_open != 0)
            {
                // 先頭から一つ戻ると最後に移動
                if (result < (int)DME_STGSLCT_ACT_PAGE_1)
                {
                    result = (int)(DME_STGSLCT_ACT_PAGE_NUM - 1);
                }

                // 最後から一つ進むと先頭に移動
                if (result >= (int)DME_STGSLCT_ACT_PAGE_NUM)
                {
                    result = (int)DME_STGSLCT_ACT_PAGE_1;
                }
            }
            // FINALのみオープン
            else
            {
                // 先頭から一つ戻ると最後に移動
                if (result < (int)DME_STGSLCT_ACT_PAGE_1)
                {
                    result = (int)(DME_STGSLCT_ACT_PAGE_FINAL);
                }

                // 最後から一つ進むと先頭に移動
                if (result > (int)DME_STGSLCT_ACT_PAGE_FINAL)
                {
                    result = (int)DME_STGSLCT_ACT_PAGE_1;
                }
            }
        }
        // スペステのみオープン
        else if (is_spe_open != 0)
        {
            result = (int)idx + diff;

            if (result == (int)DME_STGSLCT_ACT_PAGE_FINAL)
            {
                if (diff > 0)
                {
                    result = (int)DME_STGSLCT_ACT_PAGE_SPE;
                }
                else
                {
                    result = (int)DME_STGSLCT_ACT_PAGE_4;
                }
            }

            // 先頭から一つ戻ると最後に移動
            if (result < (int)DME_STGSLCT_ACT_PAGE_1)
            {
                result = (int)(DME_STGSLCT_ACT_PAGE_NUM - 1);
            }

            // 最後から一つ進むと先頭に移動
            if (result >= (int)DME_STGSLCT_ACT_PAGE_NUM)
            {
                result = (int)DME_STGSLCT_ACT_PAGE_1;
            }
        }
        // FINAL,スペステ共にオープンなし
        else
        {
            result = (int)idx + diff;

            // 先頭から一つ戻ると最後に移動
            if (result < (int)DME_STGSLCT_ACT_PAGE_1)
            {
                result = (int)(DME_STGSLCT_ACT_PAGE_4);
            }

            // 最後から一つ進むと先頭に移動
            if (result > (int)DME_STGSLCT_ACT_PAGE_4)
            {
                result = (int)DME_STGSLCT_ACT_PAGE_1;
            }
        }

        MTM_ASSERT(result >= (int)DME_STGSLCT_ACT_PAGE_1 && result < (int)DME_STGSLCT_ACT_PAGE_NUM);

        return result;
    }



    // ==========================================================================
    // dmStgSlctSetTableActiveInfo
    /*!
        各ACTテーブルのアクティブ情報設定処理
     */
    // ==========================================================================
    private static void dmStgSlctSetTableActiveInfo(DMS_STGSLCT_MAIN_WORK main_work, uint cnt)
    {
        // 台紙の非アクティブ設定
        if (main_work.is_clear_stage[cnt] < 0)
        {
            for (uint j = (uint)ACT_TAB_TABLE2; j <= (uint)ACT_TAB_TABLE3; j++)
            {
                AoActSetFrame(main_work.act[j], 1.0f);
            }
            AoActSetFrame(main_work.act[(int)ACT_TAB_SCR], 17.0f);
            AoActSetFrame(main_work.act[(int)ACT_TAB_MESS], 17.0f);
        }
        // アクティブ
        else
        {
            for (uint j = (uint)ACT_TAB_TABLE2; j <= (uint)ACT_TAB_TABLE3; j++)
            {
                AoActSetFrame(main_work.act[j], 0.0f);
            }
            AoActSetFrame(main_work.act[(int)ACT_TAB_SCR], dm_stgslct_disp_msg_id_table_tbl[cnt]);
            AoActSetFrame(main_work.act[(int)ACT_TAB_MESS], dm_stgslct_disp_msg_id_table_tbl[cnt]);

            // スコアアタックをクリアしていない場合はタイムアタックを非アクティブ設定
            if (main_work.hi_score[cnt] == DMD_STGSLCT_INIT_SCORE_NUM
                && main_work.cur_game_mode == (uint)DME_STGSLCT_PLAY_MODE_TIME_ATK)
            {
                for (uint j = (uint)ACT_TAB_TABLE2; j <= (uint)ACT_TAB_TABLE3; j++)
                {
                    AoActSetFrame(main_work.act[j], 1.0f);
                }
            }
            else
            {
                for (uint j = (uint)ACT_TAB_TABLE2; j <= (uint)ACT_TAB_TABLE3; j++)
                {
                    AoActSetFrame(main_work.act[j], 0.0f);
                }
            }
        }

    }



    // ==========================================================================
    // dmStgSlctIsCanSelectAct
    /*!
        決定したステージがプレイ可能かどうかを返す処理
     */
    // ==========================================================================
    private static bool dmStgSlctIsCanSelectAct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        // 解放されていないステージの場合
        //	if (main_work.cur_stage <= GSD_MAIN_STAGE_ID_FINAL_1) {
        if (main_work.is_clear_stage[main_work.cur_stage] == -1)
        {
            main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_DECIDE);

            return false;
        }
        //	}

        // スコアアタックをクリアしていないステージでタイムアタックを選ぼうとした場合
        if (main_work.hi_score[main_work.cur_stage] == DMD_STGSLCT_INIT_SCORE_NUM
            && main_work.cur_game_mode == (uint)DME_STGSLCT_PLAY_MODE_TIME_ATK)
        {

            return false;
        }

        return true;
    }



    // ===========================================================================
    //	dmStgSlctGetClipStageNoForChngZone
    /*!
        項目の場所を示す変数が最大・最小値を超えた際に補正した(回り込みさせた)値を取得する関数
        通常時のカーソル移動の選択項目に使用。

        @param id		[in] 項目番号
        @param diff		[in] 変化量(移動における)
        @return 補正された項目番号
    */
    // ===========================================================================
    /*
    int dmStgSlctGetClipStageNoForChngZone(int prev_stage, int cur_zone, int prev_zone)
    {
        int result;
        int prev_src = 0;
        int prev_dst = 0;
        int cur_src = 0;
        int cur_dst = 0;

        int up_list_no = 0;

        prev_src = dm_stgslct_zone_act_num_tbl[prev_zone][0];
        prev_dst = prev_src + dm_stgslct_zone_act_num_tbl[prev_zone][1];
        cur_src = dm_stgslct_zone_act_num_tbl[cur_zone][0];
        cur_dst = cur_src + dm_stgslct_zone_act_num_tbl[cur_zone][1];

        up_list_no = prev_stage - prev_src;

        result = up_list_no + cur_src;

        if (result >= cur_dst) {
            result = cur_dst - 1;		// 暫定
        }

        // 保険
        if (result < cur_src) {
            result = cur_src;
        }

        MTM_ASSERT(result >= cur_src && result < cur_dst);

        return result;
    }
    */


    // ==========================================================================
    // dmStgSlctSetObiEfctPos
    /*!
        帯アクションの演出用座標設定処理
     */
    // ==========================================================================
    private static void dmStgSlctSetObiEfctPos(DMS_STGSLCT_MAIN_WORK main_work)
    {
        for (uint i = 0; i < 2; i++)
        {

            // 一定位置に来たら戻る
            if (main_work.obi_pos[i] < DMD_STGSLCT_OBI_MOVE_END_POS)
            {
                main_work.obi_pos[i] = DMD_STGSLCT_OBI_MOVE_START_POS;
            }

            // 移動分座標加算
            main_work.obi_pos[i] += DMD_STGSLCT_OBI_MOVE_SPEED;
        }
    }



    // ==========================================================================
    // dmStgSlctMakeVertexAct
    /*!
        ACTの上に被さる部分の背景描画タスク作成処理
     */
    // ==========================================================================
    private static void dmStgSlctMakeVertexAct(DMS_STGSLCT_MAIN_WORK main_work, AMS_PARAM_DRAW_PRIMITIVE param)
    {
        float n = 0.0f;

        // フォグ設定
        if (main_work.bg_fade.a > 0)
        {
            amDrawSetFogColor(120
                              , 1.0f
                              , 1.0f
                              , 1.0f
                              );

            n = 2.0f - ((float)main_work.bg_fade.a / 255.0f);

            amDrawSetFogRange(120, n, n + 1.0f);
            amDrawSetFog(120, 1);
        }
        else
        {
            amDrawSetFog(120, 0);
        }


        param.mtx = null;
        param.vtxPCT3D = amDrawAlloc_NNS_PRIM3D_PCT(6);
        NNS_PRIM3D_PCT[] v = param.vtxPCT3D.buffer;

        v[0].Pos.x = v[2].Pos.x = v[4].Pos.x = -160.0f;
        v[1].Pos.x = v[3].Pos.x = v[5].Pos.x = v[0].Pos.x + 1024.0f;// + 1280.0f;
        v[0].Pos.y = v[1].Pos.y = 0.0f;
        v[2].Pos.y = v[3].Pos.y = v[0].Pos.y + 64.0f;
        v[4].Pos.y = v[5].Pos.y = v[0].Pos.y + 128.0f;
        v[0].Pos.z = v[1].Pos.z = v[2].Pos.z = v[3].Pos.z = v[4].Pos.z = v[5].Pos.z = -2.0f;
        v[0].Col = v[2].Col = AMD_RGBA8888(255, 255, 255, 255);
        v[1].Col = v[3].Col = AMD_RGBA8888(255, 255, 255, 255);
        v[4].Col = v[5].Col = AMD_RGBA8888(255, 255, 255, 0);

        v[0].Tex.u = v[2].Tex.u = v[4].Tex.u = 0.0f;
        v[1].Tex.u = v[3].Tex.u = v[5].Tex.u = 1.0f;
        v[0].Tex.v = v[1].Tex.v = 0.0f;
        v[2].Tex.v = v[3].Tex.v = 0.125f;
        v[4].Tex.v = v[5].Tex.v = 0.25f;


        param.format3D = NNE_PRIM3D_FMT_PCT;
        param.type = NNE_PRIM_TRIANGLE_STRIP;
        param.count = 6;

        param.texlist = AoTexGetTexList(main_work.tex[0]);
#if !_IPHONE
        param.texId = (int)(42 + main_work.cur_bg_id);
#else //!_IPHONE
        const uint c_bg_tex_idx = 35; //テクスチャの並びが変わると、此処も変更しなければ成らない

        if (uint.MaxValue == main_work.cur_bg_id)
        {
            //ゾーンセレクト用特別背景なら透明にする
            for (int i = 0; i < 6; ++i)
            {
                v[i].Col &= 0xFFFFFF00;
            }
            param.texlist = null;
            param.texId = -1;
        }
        else
        {
            param.texId = (int)(c_bg_tex_idx + main_work.cur_bg_id);
        }
#endif //!_IPHONE
        param.ablend = NNE_PRIM_ALPHABLEND_ON;
        param.zOffset = -1.0f;

        param.uwrap = NNE_PRIM_TEXWRAP_CLAMP;
        param.vwrap = NNE_PRIM_TEXWRAP_CLAMP;

        // ソート設定
        param.sortZ = 0.0f;

#if _PC || _XBOX
	param.bldSrc = NNE_BLENDMODE_SRCALPHA;
	param.bldDst = NNE_BLENDMODE_INVSRCALPHA;
	param.bldMode = NNE_BLENDOP_ADD;
#elif _PS3
	param.bldSrc = NND_BLENDFUNC_PS3_SRC_ALPHA;
	param.bldDst = NND_BLENDFUNC_PS3_ONE_MINUS_SRC_ALPHA;
	param.bldMode = NND_BLENDOP_PS3_FUNC_ADD;
#elif _WII
	param.bldSrc = GX_BL_SRCALPHA;
	param.bldDst = GX_BL_INVSRCALPHA;
	param.bldMode = GX_BM_BLEND;
#endif
        param.aTest = 0;
        param.zMask = 1;
        param.zTest = 0;

        // ソートしない
        param.noSort = 1;

        AoActDrawCorWide(param.vtxPCT3D, 6, AOE_ACT_CORW.AOD_ACT_CORW_CENTER);

        amDrawPrimitive3D(120, param);

        param.mtx = null;
        param.vtxPCT3D = amDrawAlloc_NNS_PRIM3D_PCT(6);
        v = param.vtxPCT3D.buffer;

        v[0].Pos.x = v[2].Pos.x = v[4].Pos.x = -160.0f;
        v[1].Pos.x = v[3].Pos.x = v[5].Pos.x = v[0].Pos.x + 1024.0f;//1280.0f;
        v[0].Pos.y = v[1].Pos.y = 720.0f;
        v[2].Pos.y = v[3].Pos.y = v[0].Pos.y - 128.0f;
        v[4].Pos.y = v[5].Pos.y = v[0].Pos.y - 192.0f;
        v[0].Pos.z = v[1].Pos.z = v[2].Pos.z = v[3].Pos.z = v[4].Pos.z = v[5].Pos.z = -2.0f;
        v[0].Col = v[2].Col = AMD_RGBA8888(255, 255, 255, 255);
        v[1].Col = v[3].Col = AMD_RGBA8888(255, 255, 255, 255);
        v[4].Col = v[5].Col = AMD_RGBA8888(255, 255, 255, 0);

        v[0].Tex.u = v[2].Tex.u = v[4].Tex.u = 0.0f;
        v[1].Tex.u = v[3].Tex.u = v[5].Tex.u = 1.0f;
        v[0].Tex.v = v[1].Tex.v = 0.8125f;
        v[2].Tex.v = v[3].Tex.v = 0.3125f;
        v[4].Tex.v = v[5].Tex.v = 0.0625f;

        param.format3D = NNE_PRIM3D_FMT_PCT;
        param.type = NNE_PRIM_TRIANGLE_STRIP;
        param.count = 6;

        param.texlist = AoTexGetTexList(main_work.tex[0]);
#if !_IPHONE
        param.texId = (int)(48 + main_work.cur_bg_id);
#else //!_IPHONE
        if (uint.MaxValue == main_work.cur_bg_id)
        {   //ゾーンセレクト用特別背景なら透明にする
            for (int i = 0; i < 6; ++i)
            {
                v[i].Col &= 0xFFFFFF00;
            }
            param.texlist = null;
            param.texId = -1;
        }
        else
        {
            param.texId = (int)(c_bg_tex_idx + 6 + main_work.cur_bg_id);
        }
#endif //!_IPHONE
        param.ablend = NNE_PRIM_ALPHABLEND_ON;
        param.zOffset = -1.0f;

        param.uwrap = NNE_PRIM_TEXWRAP_CLAMP;
        param.vwrap = NNE_PRIM_TEXWRAP_CLAMP;

        // ソート設定
        param.sortZ = 0.0f;


#if _PC || _XBOX
	param.bldSrc = NNE_BLENDMODE_SRCALPHA;
	param.bldDst = NNE_BLENDMODE_INVSRCALPHA;
	param.bldMode = NNE_BLENDOP_ADD;
#elif _PS3
	param.bldSrc = NND_BLENDFUNC_PS3_SRC_ALPHA;
	param.bldDst = NND_BLENDFUNC_PS3_ONE_MINUS_SRC_ALPHA;
	param.bldMode = NND_BLENDOP_PS3_FUNC_ADD;
#elif _WII
	param.bldSrc = GX_BL_SRCALPHA;
	param.bldDst = GX_BL_INVSRCALPHA;
	param.bldMode = GX_BM_BLEND;
#endif
        param.aTest = 0;
        param.zMask = 1;
        param.zTest = 0;

        // ソートしない
        param.noSort = 1;


        AoActDrawCorWide(param.vtxPCT3D, 6, AOE_ACT_CORW.AOD_ACT_CORW_CENTER);


        amDrawPrimitive3D(120, param);

        // 
        main_work.tex_u[0] -= 0.024f;
        main_work.tex_u[1] -= 0.024f;
        main_work.tex_v[0] -= 0.006f;
        main_work.tex_v[1] -= 0.006f;


        amDrawMakeTask(dmStgSlctDrawVertexAct, (ushort)(0x2800), (uint)0);

        param.mtx = null;
        param.vtxPCT3D = amDrawAlloc_NNS_PRIM3D_PCT(6);
        v = param.vtxPCT3D.buffer;

        v[0].Pos.x = v[2].Pos.x = v[4].Pos.x = 1024.0f - 160.0f;
        v[1].Pos.x = v[3].Pos.x = v[5].Pos.x = 1280.0f - 160.0f;
        v[0].Pos.y = v[1].Pos.y = 0.0f;
        v[2].Pos.y = v[3].Pos.y = v[0].Pos.y + 64.0f;
        v[4].Pos.y = v[5].Pos.y = v[0].Pos.y + 128.0f;
        v[0].Pos.z = v[1].Pos.z = v[2].Pos.z = v[3].Pos.z = v[4].Pos.z = v[5].Pos.z = -2.0f;
        v[0].Col = v[2].Col = AMD_RGBA8888(255, 255, 255, 255);
        v[1].Col = v[3].Col = AMD_RGBA8888(255, 255, 255, 255);
        v[4].Col = v[5].Col = AMD_RGBA8888(255, 255, 255, 0);

        v[0].Tex.u = v[2].Tex.u = v[4].Tex.u = 0.0f;
        v[1].Tex.u = v[3].Tex.u = v[5].Tex.u = 1.0f;
        v[0].Tex.v = v[1].Tex.v = 0.0f;
        v[2].Tex.v = v[3].Tex.v = 0.125f;
        v[4].Tex.v = v[5].Tex.v = 0.25f;


        param.format3D = NNE_PRIM3D_FMT_PCT;
        param.type = NNE_PRIM_TRIANGLE_STRIP;
        param.count = 6;

        param.texlist = AoTexGetTexList(main_work.tex[0]);
#if !_IPHONE
        param.texId = (int)(54 + main_work.cur_bg_id);
#else //!_IPHONE
        if (uint.MaxValue == main_work.cur_bg_id)
        {   //ゾーンセレクト用特別背景なら透明にする
            for (int i = 0; i < 6; ++i)
            {
                v[i].Col &= 0xFFFFFF00;
            }
            param.texlist = null;
            param.texId = -1;
        }
        else
        {
            param.texId = (int)(c_bg_tex_idx + 12 + main_work.cur_bg_id);
        }
#endif //!_IPHONE
        param.ablend = NNE_PRIM_ALPHABLEND_ON;
        param.zOffset = -1.0f;

        param.uwrap = NNE_PRIM_TEXWRAP_CLAMP;
        param.vwrap = NNE_PRIM_TEXWRAP_CLAMP;

        // ソート設定
        param.sortZ = 0.0f;

#if _PC || _XBOX
	param.bldSrc = NNE_BLENDMODE_SRCALPHA;
	param.bldDst = NNE_BLENDMODE_INVSRCALPHA;
	param.bldMode = NNE_BLENDOP_ADD;
#elif _PS3
	param.bldSrc = NND_BLENDFUNC_PS3_SRC_ALPHA;
	param.bldDst = NND_BLENDFUNC_PS3_ONE_MINUS_SRC_ALPHA;
	param.bldMode = NND_BLENDOP_PS3_FUNC_ADD;
#elif _WII
	param.bldSrc = GX_BL_SRCALPHA;
	param.bldDst = GX_BL_INVSRCALPHA;
	param.bldMode = GX_BM_BLEND;
#endif
        param.aTest = 0;
        param.zMask = 1;
        param.zTest = 0;

        // ソートしない
        param.noSort = 1;

        AoActDrawCorWide(param.vtxPCT3D, 6, AOE_ACT_CORW.AOD_ACT_CORW_CENTER);

        amDrawPrimitive3D(120, param);


        param.mtx = null;
        param.vtxPCT3D = amDrawAlloc_NNS_PRIM3D_PCT(6);
        v = param.vtxPCT3D.buffer;

        v[0].Pos.x = v[2].Pos.x = v[4].Pos.x = 1024.0f - 160.0f;
        v[1].Pos.x = v[3].Pos.x = v[5].Pos.x = 1280.0f - 160.0f;
        v[0].Pos.y = v[1].Pos.y = 720.0f;
        v[2].Pos.y = v[3].Pos.y = v[0].Pos.y - 128.0f;
        v[4].Pos.y = v[5].Pos.y = v[0].Pos.y - 192.0f;
        v[0].Pos.z = v[1].Pos.z = v[2].Pos.z = v[3].Pos.z = v[4].Pos.z = v[5].Pos.z = -2.0f;
        v[0].Col = v[2].Col = AMD_RGBA8888(255, 255, 255, 255);
        v[1].Col = v[3].Col = AMD_RGBA8888(255, 255, 255, 255);
        v[4].Col = v[5].Col = AMD_RGBA8888(255, 255, 255, 0);

        v[0].Tex.u = v[2].Tex.u = v[4].Tex.u = 0.0f;
        v[1].Tex.u = v[3].Tex.u = v[5].Tex.u = 1.0f;
        v[0].Tex.v = v[1].Tex.v = 0.8125f;
        v[2].Tex.v = v[3].Tex.v = 0.3125f;
        v[4].Tex.v = v[5].Tex.v = 0.0625f;

        param.format3D = NNE_PRIM3D_FMT_PCT;
        param.type = NNE_PRIM_TRIANGLE_STRIP;
        param.count = 6;

        param.texlist = AoTexGetTexList(main_work.tex[0]);
#if !_IPHONE
        param.texId = (int)(60 + main_work.cur_bg_id);
#else //!_IPHONE
        if (uint.MaxValue == main_work.cur_bg_id)
        {   //ゾーンセレクト用特別背景なら透明にする
            for (int i = 0; i < 6; ++i)
            {
                v[i].Col &= 0xFFFFFF00;
            }
            param.texlist = null;
            param.texId = -1;
        }
        else
        {
            param.texId = (int)(c_bg_tex_idx + 18 + main_work.cur_bg_id);
        }
#endif //!_IPHONE
        param.ablend = NNE_PRIM_ALPHABLEND_ON;
        param.zOffset = -1.0f;

        param.uwrap = NNE_PRIM_TEXWRAP_CLAMP;
        param.vwrap = NNE_PRIM_TEXWRAP_CLAMP;

        // ソート設定
        param.sortZ = 0.0f;


#if _PC || _XBOX
	param.bldSrc = NNE_BLENDMODE_SRCALPHA;
	param.bldDst = NNE_BLENDMODE_INVSRCALPHA;
	param.bldMode = NNE_BLENDOP_ADD;
#elif _PS3
	param.bldSrc = NND_BLENDFUNC_PS3_SRC_ALPHA;
	param.bldDst = NND_BLENDFUNC_PS3_ONE_MINUS_SRC_ALPHA;
	param.bldMode = NND_BLENDOP_PS3_FUNC_ADD;
#elif _WII
	param.bldSrc = GX_BL_SRCALPHA;
	param.bldDst = GX_BL_INVSRCALPHA;
	param.bldMode = GX_BM_BLEND;
#endif
        param.aTest = 0;
        param.zMask = 1;
        param.zTest = 0;


        AoActDrawCorWide(param.vtxPCT3D, 6, AOE_ACT_CORW.AOD_ACT_CORW_CENTER);

        // ソートしない
        param.noSort = 1;


        amDrawPrimitive3D(120, param);

        // 
        main_work.tex_u[0] -= 0.024f;
        main_work.tex_u[1] -= 0.024f;
        main_work.tex_v[0] -= 0.006f;
        main_work.tex_v[1] -= 0.006f;


        amDrawMakeTask(dmStgSlctDrawVertexAct, (ushort)(0x2800), (uint)0);
    }



    // ==========================================================================
    // dmStgSlctDrawVertexAct
    /*!
        ACTの上に被さる部分の描画設定処理
     */
    // ==========================================================================
    private static void dmStgSlctDrawVertexAct(AMS_TCB tcb_p)
    {
        UNREFERENCED_PARAMETER(tcb_p);

        // 前処理
        AoActDrawPre();
        amDrawExecCommand(120, 0);

        // シーン描画終了(半透明描画開始)
        amDrawEndScene();

    }




    // ==========================================================================
    // dmStgSlctSetEmeTableInEfct
    /*!
        ファイルテーブル掃け演出用座標設定処理

        ここでは基準点(一つ目のファイル)となる座標のみを移動させて
        それ以外は一定の距離で表示位置をずらすようにする
     */
    // ==========================================================================
    private static void dmStgSlctSetEmeTableInEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        float move_dist = 0;
        float distance = 0;

        // 帯移動分
        distance = DMD_STGSLCT_DOWN_ACT_DISP_POS - DMD_STGSLCT_DOWN_ACT_NODISP_POS;

        move_dist = distance / DMD_STGSLCT_ZONE_EFCT_TIME;

        main_work.chaos_eme_pos_y += move_dist;

    }



    // ==========================================================================
    // dmStgSlctIsEmeTableInEfctEnd
    /*!
        帯掃け演出終了チェック処理
     */
    // ==========================================================================
    private static bool dmStgSlctIsEmeTableInEfctEnd(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (main_work.chaos_eme_pos_y <= DMD_STGSLCT_DOWN_ACT_DISP_POS)
        {

            main_work.chaos_eme_pos_y = DMD_STGSLCT_DOWN_ACT_DISP_POS;

            return true;
        }

        return false;
    }



    // ==========================================================================
    // dmStgSlctSetEmeTableOutEfct
    /*!
        ファイルテーブル掃け演出用座標設定処理

        ここでは基準点(一つ目のファイル)となる座標のみを移動させて
        それ以外は一定の距離で表示位置をずらすようにする
     */
    // ==========================================================================
    private static void dmStgSlctSetEmeTableOutEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        float move_dist = 0;
        float distance = 0;

        // 帯移動分
        distance = DMD_STGSLCT_DOWN_ACT_NODISP_POS - DMD_STGSLCT_DOWN_ACT_DISP_POS;

        move_dist = distance / DMD_STGSLCT_ZONE_EFCT_TIME;

        main_work.chaos_eme_pos_y += move_dist;

    }



    // ==========================================================================
    // dmStgSlctIsEmeTableOutEfctEnd
    /*!
        帯掃け演出終了チェック処理
     */
    // ==========================================================================
    private static bool dmStgSlctIsEmeTableOutEfctEnd(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (main_work.chaos_eme_pos_y >= DMD_STGSLCT_DOWN_ACT_NODISP_POS)
        {

            main_work.chaos_eme_pos_y = DMD_STGSLCT_DOWN_ACT_NODISP_POS;

            return true;
        }

        return false;
    }




    // ==========================================================================
    // dmStgSlctSetModeTexInEfct
    /*!
        ファイルテーブル掃け演出用座標設定処理

        ここでは基準点(一つ目のファイル)となる座標のみを移動させて
        それ以外は一定の距離で表示位置をずらすようにする
     */
    // ==========================================================================
    private static void dmStgSlctSetModeTexInEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        float move_dist = 0;
        float distance = 0;

        // 帯移動分
        distance = DMD_STGSLCT_DOWN_ACT_DISP_POS - DMD_STGSLCT_DOWN_ACT_NODISP_POS;

        move_dist = distance / DMD_STGSLCT_ZONE_EFCT_TIME;

        main_work.mode_tex_pos_y += move_dist;

    }



    // ==========================================================================
    // dmStgSlctIsModeTexInEfctEnd
    /*!
        帯掃け演出終了チェック処理
     */
    // ==========================================================================
    private static bool dmStgSlctIsModeTexInEfctEnd(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (main_work.mode_tex_pos_y <= DMD_STGSLCT_DOWN_ACT_DISP_POS)
        {

            main_work.mode_tex_pos_y = DMD_STGSLCT_DOWN_ACT_DISP_POS;

            return true;
        }

        return false;
    }



    // ==========================================================================
    // dmStgSlctSetModeTexOutEfct
    /*!
        ファイルテーブル掃け演出用座標設定処理

        ここでは基準点(一つ目のファイル)となる座標のみを移動させて
        それ以外は一定の距離で表示位置をずらすようにする
     */
    // ==========================================================================
    private static void dmStgSlctSetModeTexOutEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        float move_dist = 0;
        float distance = 0;

        // 帯移動分
        distance = DMD_STGSLCT_DOWN_ACT_NODISP_POS - DMD_STGSLCT_DOWN_ACT_DISP_POS;

        move_dist = distance / DMD_STGSLCT_ZONE_EFCT_TIME;

        main_work.mode_tex_pos_y += move_dist;

    }



    // ==========================================================================
    // dmStgSlctIsModeTexOutEfctEnd
    /*!
        帯掃け演出終了チェック処理
     */
    // ==========================================================================
    private static bool dmStgSlctIsModeTexOutEfctEnd(DMS_STGSLCT_MAIN_WORK main_work)
    {
        if (main_work.mode_tex_pos_y >= DMD_STGSLCT_DOWN_ACT_NODISP_POS)
        {

            main_work.mode_tex_pos_y = DMD_STGSLCT_DOWN_ACT_NODISP_POS;

            return true;
        }

        return false;
    }



#if (MTD_DEBUG)
// ==========================================================================
// dmStgSlctSetAllOpenStage
/*!
	全ステージをオープン状態に設定する処理
 */
// ==========================================================================
private static void dmStgSlctSetAllOpenStage(DMS_STGSLCT_MAIN_WORK main_work)
{
	uint save_hi_score = 1000;
	bool sonic_type = 0;
	
	UNREFERENCED_PARAMETER(main_work);
	
	// 通常ステージデータインスタンス作成
	gs.backup.SStage &data
		= gs.backup.SStage.CreateInstance();
	
	// スペシャルステージデータインスタンス作成
	gs.backup.SSpecial &spe_data
		= gs.backup.SSpecial.CreateInstance();
	
	
	for (uint i = 0; i <= GSD_MAIN_STAGE_ID_FINAL_1; i++) {
		// 通常ステージのハイスコア取得
		data[i].SetHighScore(save_hi_score, sonic_type);
		
	}
	
	for (uint i = 0; i < 7; i++) {
		// スペシャルステージのハイスコア取得
		spe_data[i].SetHighScore(save_hi_score);
		
		spe_data[i].SetEmeraldStage((gs.backup.SSpecialSolo.EEmeraldStage.Type)(i + 1));
	}
}
#endif


    // ==========================================================================
    // dmStgSlctSetBgFadeEfct
    /*!
        BGフェード演出設定処理
     */
    // ==========================================================================
    private static void dmStgSlctSetBgFadeEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        int tmp_bg_fade_a = 0;

        // ビット拡張して0以下と255以上を比較できるようにする
        tmp_bg_fade_a = (int)main_work.bg_fade.a;

        // 現在選択しているZONEとBGのゾーンを比較
        if (main_work.cur_zone != main_work.cur_bg_id)
        {
            // 演出開始フラグON
            main_work.flag |= DMD_STGSLCT_FLAG_BG_FADE_EFCT;

            // 切り替えるBGのIDを設定
            main_work.next_bg_id = main_work.cur_zone;
        }


        // BGのフォグ設定
        if ((main_work.flag & DMD_STGSLCT_FLAG_BG_FADE_EFCT) != 0)
        {

            if (tmp_bg_fade_a < 255)
            {
                tmp_bg_fade_a += DMD_STGSLCT_BG_FADE_SPEED;
            }
            else
            {
                tmp_bg_fade_a = 255;

                // ここでBGを切り替える
                main_work.cur_bg_id = main_work.next_bg_id;

                main_work.flag &= unchecked((uint)~DMD_STGSLCT_FLAG_BG_FADE_EFCT);
            }
        }

        else
        {
            if (tmp_bg_fade_a > 0)
            {
                // ここでは常に下げる
                tmp_bg_fade_a -= DMD_STGSLCT_BG_FADE_SPEED;
            }
            else
            {
                tmp_bg_fade_a = 0;
            }
        }

        tmp_bg_fade_a = MTM_MATH_CLIP(tmp_bg_fade_a, 0, 255);

        // 計算結果を元の変数に戻す
        main_work.bg_fade.a = (byte)tmp_bg_fade_a;

    }



    // ==========================================================================
    // dmStgSlctSetZoneScrChangeEfct
    /*!
        BGフェード演出設定処理
     */
    // ==========================================================================
    private static void dmStgSlctSetZoneScrChangeEfct(DMS_STGSLCT_MAIN_WORK main_work)
    {
        // タイマー更新
        //	main_work.zone_scr_timer++;

        // 切り替え時間が経ったとき
        if (main_work.zone_scr_id > DMD_STGSLCT_ZONE_SCR_CHANGE_ALL_FRM)
        {
            // ゾーンスクリーンID切り替え
            main_work.zone_scr_id = 0;
        }
        else
        {
            main_work.zone_scr_id++;
        }
    }



    // ==========================================================================
    // dmStgSlctSetNextFocusAct
    /*!
        ACTクリア後に次のACTへFOCUSを変更する設定処理
     */
    // ==========================================================================
    private static int dmStgSlctSetNextFocusAct(DMS_STGSLCT_MAIN_WORK main_work, int set_stage_id)
    {
        UNREFERENCED_PARAMETER(main_work);

        uint tmp_stage_id = 0;
        uint tmp_focus_zone = 0;
        uint tmp_zone_act_id = 0;

        uint tmp_check_zone = 0;
        uint tmp_check_act = 0;


        // スペステの場合、変更なし
        if (set_stage_id >= GSD_MAIN_STAGE_ID_SS1)
        {
            tmp_stage_id = (uint)(dm_stgslct_zone_act_num_tbl[(int)DME_STGSLCT_ACT_PAGE_SPE][0]
                            + (set_stage_id - GSD_MAIN_STAGE_ID_SS1));

            return (int)tmp_stage_id;
        }

        if (set_stage_id >= GSD_MAIN_STAGE_ID_FINAL_1)
        {
            tmp_stage_id = GSD_MAIN_STAGE_ID_FINAL_1;

            return (int)tmp_stage_id;
        }

        // ここでACT番号を取得(1-1から4-BOSSまでの間になる)
        tmp_stage_id = (uint)set_stage_id;

#if true
        // 初回プレイでない場合、切り替えなし
        if (g_gs_main_sys_info.is_first_play == false)
        {
            return (int)tmp_stage_id;
        }

        // タイムアタックの場合、切り替えなし(初回プレイじゃない時点でOKかもしれない)
        if (g_gs_main_sys_info.game_mode == GSD_GAME_MODE_TIME_ATTACK)
        {
            return (int)tmp_stage_id;
        }

        // 一つ前のイベントがランキングだった場合、ACTはそのまま
        if ((main_work.flag & DMD_STGSLCT_FLAG_PREV_EVT_RANKING) != 0)
        {
            return (int)tmp_stage_id;
        }
#endif

        // 初めにチェックするZONEを取得
        tmp_focus_zone = dm_stgslct_act_zone_no_tbl[tmp_stage_id];
        tmp_zone_act_id = dm_stgslct_zone_array_act_tbl[tmp_stage_id];

        // 初めにチェックするZONEのクリア状況をチェック
        for (uint i = tmp_stage_id - tmp_zone_act_id;
                 i < tmp_stage_id - tmp_zone_act_id + 4;
                 i++)
        {
            // 指定ゾーンに若い番号から見て、未クリアACTがあればそれを指定
            if (GsMainSysIsStageClear((int)i) == false)
            {
                return (int)i;
            }
        }

        // その他のゾーンのクリア状況をチェック
        for (uint i = 0; i < 4; i++)
        {
            tmp_check_zone = i;

            tmp_check_act = dm_stgslct_zone_act_num_tbl[tmp_check_zone][0];

            for (uint j = 0; j < 4; j++)
            {
                if (GsMainSysIsStageClear((int)(tmp_check_act + j)) == false)
                {
                    return (int)(tmp_check_act + j);
                }
            }
        }

        // 初回プレイステージクリアかつ、他のACT全てクリア済みの場合、FINALへ
        tmp_stage_id = GSD_MAIN_STAGE_ID_FINAL_1;

        // スコアアタックにて、全てクリア済みの状態のとき
        return (int)tmp_stage_id;
    }

#if _IPHONE
    // ==========================================================================
    // dmStgSlctIsBossFocus
    /*!
        ボスフォーカス状態確認

        @return ボスフォーカス状態である
     */
    // ==========================================================================
    private static bool dmStgSlctIsBossFocus(DMS_STGSLCT_MAIN_WORK main_work)
    {
        bool result = false;
        switch (main_work.cur_zone)
        {
            case (uint)DME_STGSLCT_ZONE_TYPE_FINAL: //ファイナルゾーン
            case (uint)DME_STGSLCT_ZONE_TYPE_SPE: //スペシャルステージ
                                                  //何もしない
                break;
            default: //通常ゾーン
                {
                    if (GSD_GAME_MODE_STORY == main_work.cur_game_mode)
                    {
                        //スコアアタックなら
                        uint act_num_start = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
                        uint act_num_end = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] + act_num_start;
                        //クリア数の判定
                        uint clear_stage_num = 0;
                        for (uint i = act_num_start; i < act_num_end; ++i)
                        {
                            if (1 == main_work.is_clear_stage[i])
                            {
                                ++clear_stage_num;
                            }
                        }
                        //クリア数が3ならボスにフォーカス
                        if (3 == clear_stage_num)
                        {
                            result = true;
                        }
                    }
                }
                break;
        }
        return result;
    }

    // ==========================================================================
    // dmStgSlctStageSelectChngZoneSetInZoneScroll
    /*!
        アクトセレクト時のゾーン切り換えに於ける入り演出側ゾーンのスクロール量設定
     */
    // ==========================================================================
    private static void dmStgSlctStageSelectChngZoneSetInZoneScroll(DMS_STGSLCT_MAIN_WORK main_work, int stage = 0)
    {
        uint act_num_start = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][0];
        uint act_num_end = dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1] + act_num_start;

        switch (main_work.cur_zone)
        {
            case (uint)DME_STGSLCT_ZONE_TYPE_FINAL: //ファイナルゾーン
                main_work.focus_disp_no = 0;
                for (uint i = act_num_start; i < act_num_end; ++i)
                {
                    main_work.act_top_pos_y[i] = dm_stgslct_act_disp_y_pos_tbl[i];
                }
                break;
            case (uint)DME_STGSLCT_ZONE_TYPE_SPE: //スペシャルステージ
                {
                    int focus_disp_no = (int)(stage - DME_STGSLCT_STAGE_S_2);
                    main_work.focus_disp_no = MTM_MATH_CLIP(focus_disp_no, 0, 4);
                    for (uint i = act_num_start; i < act_num_end; ++i)
                    {
                        main_work.act_top_pos_y[i] = dm_stgslct_act_tab_disp_y_pos_tbl[main_work.focus_disp_no];
                    }
                }
                break;
            default: //通常ゾーン
                if (dmStgSlctIsBossFocus(main_work))
                {
                    main_work.focus_disp_no = 1;
                }
                else switch (stage)
                    {
                        case (int)DME_STGSLCT_STAGE_1_B:
                        case (int)DME_STGSLCT_STAGE_2_B:
                        case (int)DME_STGSLCT_STAGE_3_B:
                        case (int)DME_STGSLCT_STAGE_4_B:
                            main_work.focus_disp_no = 1;
                            break;
                        default:
                            main_work.focus_disp_no = 0;
                            break;
                    }
                for (uint i = act_num_start; i < act_num_end; ++i)
                {
                    main_work.act_top_pos_y[i] = dm_stgslct_act_tab_disp_y_pos_tbl[main_work.focus_disp_no];
                }
                break;
        }
    }
#endif // _IPHONE

}
