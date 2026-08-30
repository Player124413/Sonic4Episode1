using System;
using System.Collections.Generic;
using er;
using er.web;
using gs.backup;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

using static AppMain;

internal static partial class DmTitle
{
    public const int DMD_TITLE_TASK_PAUSELEVEL = 0;
    public const int DMD_TITLE_TASK_PRIO_MAIN = 8192;
    public const int DMD_TITLE_TASK_GROUP_MAIN = 10;
    public const int DMD_TITLE_DRAW_PRIO_ACT = 33792;
    public const int DMD_TITLE_DRAW_PRIO_WIN = 34816;
    public const int DMD_TITLE_DRAW_PRIO_WIN_FIX = 35840;
    public const int DMD_TITLE_FILE_PATH_NUM_MAX = 60;
    public const int DMD_TITLE_IDLE_DISP_TIME = 40;
    public const int DMD_TITLE_IDLE_NONE_TIME = 30;
    public const int DMD_TITLE_DECIDE_EFCT_TIME = 60;
    public const int DMD_TITLE_DECIDE_BLINK_TIME = 4;
    public const int DMD_TITLE_AMA_DATA_ARC_ID = 0;
    public const int DMD_TITLE_AMB_DATA_ARC_ID = 1;

    // dmTitle.cpp:120-134. the 304 arm is the #else that iPhone lands in; console is 264
    private const float DMD_TITLE_CRSR_DFLT_POS_X_PC = 264f;
    private const float DMD_TITLE_CRSR_DFLT_POS_X_IOS = 304f;

    // the C++ console menu is seven items; RANK and ZISSEKI are dropped here because both
    // dead-end, so 5/6/6 becomes 3/4/4. see docs/dmtitle-port-notes.md
    private const int DMD_TITLE_SLCT_MENU_NUM_FIRST_PC = 3;
    private const int DMD_TITLE_SLCT_MENU_NUM_PLAYED_PC = 4;
    private const int DMD_TITLE_SLCT_MENU_NUM_TRIAL_PC = 4;

    private const int DMD_TITLE_SLCT_MENU_NUM_FIRST_IOS = 3;
    private const int DMD_TITLE_SLCT_MENU_NUM_PLAYED_IOS = 4;
    private const int DMD_TITLE_SLCT_MENU_NUM_TRIAL_IOS = 3;

    public const float DMD_TITLE_CRSR_MOVE_TIME = 8f;

    public const int DMD_TITLE_DRAW_STATE_ID = 10;

    public const float DMD_TITLE_SIZE_WIDTH = 960.0f;
    public const float DMD_TITLE_SIZE_HEIGHT = 720.0f;
    public const float DMD_TITLE_SIZE_HALF_WIDTH = 480.0f;
    public const float DMD_TITLE_SIZE_HALF_HEIGHT = 360.0f;

    public const float DMD_TITLE_WIN_EFCT_TIME = 8.0f;
    public const float DMD_TITLE_WINDOW_SIZE_H = 180.0f;
    public const float DMD_TITLE_WIN_DEF_RATE = 1.0f;

    public const int DMD_TITLE_PUSH_START_DISP_TIME = 3600;
    public const int DMD_TITLE_END_GAME_WAIT_TIME = 60;

    private const float DMD_TITLE_WINDOW_SIZE_W_PC = 380.0f;
    private const float DMD_TITLE_WINDOW_SIZE_W_IOS = 420.0f;

    private const float DMD_TITLE_DISP_SCALE_TEXT_PC = 1.0f;
    private const float DMD_TITLE_DISP_SCALE_TEXT_IOS = 1.5f * 1.125f;

    private const float DMD_TITLE_MMENU_WIN_SIZE_W_PC = 486.0f;

    // dmTitle.cpp:162 gives the _PC || _XBOX window 486x288, but that 288 is sized for the seven
    // menu items that arm has. Dropping RANK and ZISSEKI takes two 50-unit slots out, so 188.
    // That lands on SEGA's own _PS3 value (dmTitle.cpp:165), whose menu is also five items with
    // the same 3/4/4 counts, which is the cross-check rather than the source.
    private const float DMD_TITLE_MMENU_WIN_SIZE_H_PC = 188.0f;

    private const float DMD_TITLE_MMENU_WIN_SIZE_W_IOS = 800.0f;
    private const float DMD_TITLE_MMENU_WIN_SIZE_H_IOS = 540.0f;
    private const float DMD_TITLE_MMENU_WIN_SIZE_W_2 = DMD_TITLE_MMENU_WIN_SIZE_W_IOS;
    private const float DMD_TITLE_MMENU_WIN_SIZE_H_2 = DMD_TITLE_MMENU_WIN_SIZE_H_IOS - 180.0f;


    public const float DMD_TITLE_FADEIN_TIME = 32f;
    public const float DMD_TITLE_FADEOUT_TIME = 32f;

    public const int DMD_TITLE_BGM_FADEIN_TIME = 32;
    public const int DMD_TITLE_BGM_FADEOUT_TIME = 32;

    public const uint DMD_TITLE_FLAG_EXIT = 1;
    public const uint DMD_TITLE_FLAG_CANCEL = 2;
    public const uint DMD_TITLE_FLAG_DECIDE = 4;
    public const uint DMD_TITLE_FLAG_DISP_TEXT = 8;
    public const uint DMD_TITLE_FLAG_DECIDE_EFCT = 16;
    public const uint DMD_TITLE_FLAG_GAME_START = 32;
    public const uint DMD_TITLE_FLAG_MENU_UP_INPUT = 64;
    public const uint DMD_TITLE_FLAG_MENU_DOWN_INPUT = 128;
    public const uint DMD_TITLE_FLAG_MENU_CRSR_EFCT = 256;
    public const uint DMD_TITLE_FLAG_DISP_TITLE = 512;
    public const uint DMD_TITLE_FLAG_DISP_MAINMENU = 1024;
    public const uint DMD_TITLE_FLAG_BUY_COMP = 2048;
    public const uint DMD_TITLE_FLAG_EXIST_SAVE_DATA = 4096;
    public const uint DMD_TITLE_FLAG_BACK_DEL_SAVE = 8192;
    public const uint DMD_TITLE_FLAG_DEL_SAVE_DATA = 16384;
    public const uint DMD_TITLE_FLAG_NEW_GAME_START = 32768;
    public const uint DMD_TITLE_FLAG_WIN_EFCT_END = 65536;
    public const uint DMD_TITLE_FLAG_WIN_DRAW_START = 131072;
    public const uint DMD_TITLE_FLAG_COMP_NO_DISP_END = 262144;
    public const uint DMD_TITLE_FLAG_DEMO_SND_END = 524288;
    public const uint DMD_TITLE_FLAG_DISP_LOOP_END = 1048576;
    public const uint DMD_TITLE_FLAG_DISP_MMENU_WIN = 2097152;
    public const uint DMD_TITLE_FLAG_MMENU_WIN_EFCT_END = 4194304;
    public const uint DMD_TITLE_FLAG_DEL_DATA_BACK_TITLE = 8388608;
    public const uint DMD_TITLE_FLAG_DEL_DATA_BACK_MMENU = 16777216;
    public const uint DMD_TITLE_FLAG_DEL_DATA_INIT_START = 33554432;
    public const uint DMD_TITLE_FLAG_DEL_DATA_GAME_END = 67108864;
    public const uint DME_TITLE_FLAG_NEXT_EVT_TITLE = 1073741824;
    public const uint DMD_TITLE_FLAG_SIGN_OUT_EXIT = 2147483648;

    public const uint DMD_TITLE_DISP_FLAG_WIN_ACT = 1;
    public const uint DMD_TITLE_DISP_FLAG_ALL_ACTION = 2;
}
