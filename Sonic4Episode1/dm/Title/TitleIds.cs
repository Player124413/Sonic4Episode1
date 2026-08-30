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
    public const int IDA_D_TITLE_ACT_BTN_L = 0;
    public const int IDA_D_TITLE_ACT_BTN_C = 1;
    public const int IDA_D_TITLE_ACT_BTN_R = 2;
    public const int IDA_D_TITLE_ACT_BTN02_L = 3;
    public const int IDA_D_TITLE_ACT_BTN02_C = 4;
    public const int IDA_D_TITLE_ACT_GAME_BTN_L = 5;
    public const int IDA_D_TITLE_ACT_GAME_BTN_C = 6;
    public const int IDA_D_TITLE_JP_ACT_TEX_HAZIME = 0;
    public const int IDA_D_TITLE_JP_ACT_TEX_TUZUKI = 1;
    public const int IDA_D_TITLE_JP_ACT_TEX_OPTION = 2;
    public const int IDA_D_TITLE_JP_ACT_TEX_KANZENBAN = 3;
    public const int IDA_D_TITLE_JP_ACT_TEX_WIN_MSG1 = 4;
    public const int IDA_D_TITLE_JP_ACT_TEX_WIN_MSG2 = 5;
    public const int IDA_D_TITLE_JP_ACT_TEX_START = 6;
    public const int IDA_D_TITLE_JP_ACT_TEX_MODORU = 7;
    public const int IDA_D_TITLE_JP_ACT_TEX_GAME = 8;
    public const int IDA_D_TITLE_JP_ACT_TEX_ACHIEVEMENTS = 9;
    public const int IDA_D_TITLE_JP_ACT_TEX_LEADERBOARDS = 10;
    public const int IDA_D_TITLE_OP_ACT_LOGO01 = 0;
    public const int IDA_D_TITLE_OP_ACT_LOGO02_YUBI = 1;
    public const int IDA_D_TITLE_OP_ACT_LOGO03 = 2;
    public const int IDA_D_TITLE_OP_ACT_LOGO04 = 3;
    public const int IDA_D_TITLE_OP_ACT_SEGALOGO = 4;

    const int IDA_D_TITLE_JP_ACT_TEX_HAZIME_PC = 0; // tex_hazime
    const int IDA_D_TITLE_JP_ACT_TEX_HAZIME2_PC = 1; // tex_hazime2
    const int IDA_D_TITLE_JP_ACT_TEX_TUZUKI_PC = 2; // tex_tuzuki
    const int IDA_D_TITLE_JP_ACT_TEX_TUZUKI2_PC = 3; // tex_tuzuki2
    const int IDA_D_TITLE_JP_ACT_TEX_OPTION_PC = 4; // tex_option
    const int IDA_D_TITLE_JP_ACT_TEX_OPTION2_PC = 5; // tex_option2
    const int IDA_D_TITLE_JP_ACT_TEX_RANKING_PC = 6; // tex_ranking
    const int IDA_D_TITLE_JP_ACT_TEX_RANKING2_PC = 7; // tex_ranking2
    const int IDA_D_TITLE_JP_ACT_TEX_ZISSEKI_PC = 8; // tex_zisseki
    const int IDA_D_TITLE_JP_ACT_TEX_ZISSEKI2_PC = 9; // tex_zisseki2
    const int IDA_D_TITLE_JP_ACT_TEX_LIBRARY_PC = 10; // tex_library
    const int IDA_D_TITLE_JP_ACT_TEX_LIBRARY2_PC = 11; // tex_library2
    const int IDA_D_TITLE_JP_ACT_TEX_KANZENBAN_PC = 12; // tex_kanzenban
    const int IDA_D_TITLE_JP_ACT_TEX_KANZENBAN2_PC = 13; // tex_kanzenban2
    const int IDA_D_TITLE_JP_ACT_TEX_WIN_MSG1_PC = 14; // tex_win_msg1
    const int IDA_D_TITLE_JP_ACT_TEX_WIN_MSG2_PC = 15; // tex_win_msg2
    const int IDA_D_TITLE_JP_ACT_TEX_START_PC = 16; // tex_start
    const int IDA_D_TITLE_JP_ACT_TEX_WIN_MSG3_PC = 17; // tex_win_msg3

    const int IDA_D_CMN_WIN_ACT_WIN_LINE = IDA_D_CMN_WIN_ACT_WIN_LINE_PC;

    const int IDA_D_TITLE_ACT_CURSOR_PC = 0;
}
