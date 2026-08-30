using static AppMain;

using static StgSlct.DME_STGSLCT_STAGE;
using static StgSlct.DME_STGSLCT_ZONE_TYPE;

internal static partial class StgSlct
{
    private static readonly string[] dm_stgslct_main_lng_amb_name_tbl = [
        GSS_BASE_PATH + "DEMO/STGSLCT/D_STGSLCT_JP.AMB",
        GSS_BASE_PATH + "DEMO/STGSLCT/D_STGSLCT_US.AMB",
        GSS_BASE_PATH + "DEMO/STGSLCT/D_STGSLCT_FR.AMB",
        GSS_BASE_PATH + "DEMO/STGSLCT/D_STGSLCT_IT.AMB",
        GSS_BASE_PATH + "DEMO/STGSLCT/D_STGSLCT_GE.AMB",
        GSS_BASE_PATH + "DEMO/STGSLCT/D_STGSLCT_SP.AMB",
    ];

    private static readonly string[] dm_stgslct_menu_cmn_amb_name_tbl = [
        GSS_BASE_PATH + "DEMO/CMN/D_CMN_BG.AMB",
        GSS_BASE_PATH + "DEMO/CMN/D_CMN_BTN.AMB",
        GSS_BASE_PATH + "DEMO/CMN/D_CMN_OBI.AMB",
        GSS_BASE_PATH + "DEMO/CMN/D_CMN_WIN.AMB",
    ];

    private static readonly string[] dm_stgslct_menu_cmn_lng_amb_name_tbl = [
        GSS_BASE_PATH + "DEMO/CMN/D_CMN_MSG_JP.AMB",
        GSS_BASE_PATH + "DEMO/CMN/D_CMN_MSG_US.AMB",
        GSS_BASE_PATH + "DEMO/CMN/D_CMN_MSG_FR.AMB",
        GSS_BASE_PATH + "DEMO/CMN/D_CMN_MSG_IT.AMB",
        GSS_BASE_PATH + "DEMO/CMN/D_CMN_MSG_GE.AMB",
        GSS_BASE_PATH + "DEMO/CMN/D_CMN_MSG_SP.AMB",
    ];

    // per act page: first stage index, stage count
    private static readonly uint[][] dm_stgslct_zone_act_num_tbl = [
        [0, 4],
        [4, 4],
        [8, 4],
        [12, 4],
        [16, 1],
        [17, 7],
    ];

    private static readonly uint[] dm_stgslct_act_zone_no_tbl = [
        0, 0, 0, 0,
        1, 1, 1, 1,
        2, 2, 2, 2,
        3, 3, 3, 3,
        4,
        5, 5, 5, 5, 5, 5, 5,
    ];

    private static readonly uint[] dm_stgslct_eme_get_act_no_tbl = [
         0,
         0,  1,  2,
         4,  5,  6,
         8,  9, 10,
        12, 13, 14,
    ];

    private static readonly uint[] dm_stgslct_zone_array_act_tbl = [
        0, 1, 2, 3,
        0, 1, 2, 3,
        0, 1, 2, 3,
        0, 1, 2, 3,
        0,
        0, 1, 2, 3, 4, 5, 6,
    ];

    // [is_final_open][zone][direction], direction being up, down, left, right.
    // is_final_open indexes this directly: 0 none, 1 FINAL_OPEN, 2 SPECIAL_OPEN, 3 ALL_OPEN,
    // which is exactly the four-way cascade the C++ writes out at each use site.
    private static readonly DME_STGSLCT_ZONE_TYPE[][][] dm_stgslct_zone_input_dir_tbl = [
        [
            [DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_2,     DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_3],
            [DME_STGSLCT_ZONE_TYPE_1,     DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_4],
            [DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_4,     DME_STGSLCT_ZONE_TYPE_1,     DME_STGSLCT_ZONE_TYPE_NONE],
            [DME_STGSLCT_ZONE_TYPE_3,     DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_2,     DME_STGSLCT_ZONE_TYPE_NONE],
            [DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_NONE],
            [DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_NONE],
        ],
        [
            [DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_2,     DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_FINAL],
            [DME_STGSLCT_ZONE_TYPE_1,     DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_FINAL],
            [DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_4,     DME_STGSLCT_ZONE_TYPE_FINAL, DME_STGSLCT_ZONE_TYPE_NONE],
            [DME_STGSLCT_ZONE_TYPE_3,     DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_FINAL, DME_STGSLCT_ZONE_TYPE_NONE],
            [DME_STGSLCT_ZONE_TYPE_1,     DME_STGSLCT_ZONE_TYPE_2,     DME_STGSLCT_ZONE_TYPE_1,     DME_STGSLCT_ZONE_TYPE_3],
            [DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_NONE],
        ],
        [
            [DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_2,     DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_SPE],
            [DME_STGSLCT_ZONE_TYPE_1,     DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_SPE],
            [DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_4,     DME_STGSLCT_ZONE_TYPE_SPE,   DME_STGSLCT_ZONE_TYPE_NONE],
            [DME_STGSLCT_ZONE_TYPE_3,     DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_SPE,   DME_STGSLCT_ZONE_TYPE_NONE],
            [DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_NONE],
            [DME_STGSLCT_ZONE_TYPE_1,     DME_STGSLCT_ZONE_TYPE_2,     DME_STGSLCT_ZONE_TYPE_1,     DME_STGSLCT_ZONE_TYPE_3],
        ],
        [
            [DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_2,     DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_FINAL],
            [DME_STGSLCT_ZONE_TYPE_1,     DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_SPE],
            [DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_4,     DME_STGSLCT_ZONE_TYPE_FINAL, DME_STGSLCT_ZONE_TYPE_NONE],
            [DME_STGSLCT_ZONE_TYPE_3,     DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_SPE,   DME_STGSLCT_ZONE_TYPE_NONE],
            [DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_SPE,   DME_STGSLCT_ZONE_TYPE_1,     DME_STGSLCT_ZONE_TYPE_3],
            [DME_STGSLCT_ZONE_TYPE_FINAL, DME_STGSLCT_ZONE_TYPE_NONE,  DME_STGSLCT_ZONE_TYPE_2,     DME_STGSLCT_ZONE_TYPE_4],
        ],
    ];

    // [is_final_open][zone][xy], same indexing as above. mobile is the console layout nudged:
    // x by -50 left column, -10 right, -30 centre; y by -30 top row, -20 bottom.
    private static readonly float[][][] dm_stgslct_zone_disp_pos_tbl_console = [
        [[160.0f, 150.0f], [160.0f, 350.0f], [560.0f, 150.0f], [560.0f, 350.0f], [360.0f, -280.0f], [360.0f, 780.0f]],
        [[ 80.0f, 150.0f], [ 80.0f, 350.0f], [640.0f, 150.0f], [640.0f, 350.0f], [360.0f,  260.0f], [360.0f, 780.0f]],
        [[ 80.0f, 150.0f], [ 80.0f, 350.0f], [640.0f, 150.0f], [640.0f, 350.0f], [360.0f, -280.0f], [360.0f, 260.0f]],
        [[ 80.0f, 150.0f], [ 80.0f, 350.0f], [640.0f, 150.0f], [640.0f, 350.0f], [360.0f,  150.0f], [360.0f, 350.0f]],
    ];

    private static readonly float[][][] dm_stgslct_zone_disp_pos_tbl_mobile = [
        [[160.0f - 50.0f, 150.0f - 30.0f], [160.0f - 50.0f, 350.0f - 20.0f], [560.0f - 10.0f, 150.0f - 30.0f], [560.0f - 10.0f, 350.0f - 20.0f], [360.0f - 30.0f, -280.0f - 30.0f], [360.0f - 30.0f, 780.0f - 20.0f]],
        [[ 80.0f - 50.0f, 150.0f - 30.0f], [ 80.0f - 50.0f, 350.0f - 20.0f], [640.0f - 10.0f, 150.0f - 30.0f], [640.0f - 10.0f, 350.0f - 20.0f], [360.0f - 30.0f,  260.0f - 30.0f], [360.0f - 30.0f, 780.0f - 20.0f]],
        [[ 80.0f - 50.0f, 150.0f - 30.0f], [ 80.0f - 50.0f, 350.0f - 20.0f], [640.0f - 10.0f, 150.0f - 30.0f], [640.0f - 10.0f, 350.0f - 20.0f], [360.0f - 30.0f, -280.0f - 30.0f], [360.0f - 30.0f, 260.0f - 20.0f]],
        [[ 80.0f - 50.0f, 150.0f - 30.0f], [ 80.0f - 50.0f, 350.0f - 20.0f], [640.0f - 10.0f, 150.0f - 30.0f], [640.0f - 10.0f, 350.0f - 20.0f], [360.0f - 30.0f,  150.0f - 30.0f], [360.0f - 30.0f, 350.0f - 20.0f]],
    ];

    private static readonly float[][][] dm_stgslct_zone_nodisp_pos_tbl_console = [
        [[-416.0f, 100.0f], [-416.0f, 400.0f], [1120.0f, 100.0f], [1120.0f, 400.0f], [360.0f, -280.0f], [360.0f, 780.0f]],
        [[-416.0f, 100.0f], [-416.0f, 400.0f], [1120.0f, 100.0f], [1120.0f, 400.0f], [360.0f, -280.0f], [360.0f, 780.0f]],
        [[-416.0f, 100.0f], [-416.0f, 400.0f], [1120.0f, 100.0f], [1120.0f, 400.0f], [360.0f, -280.0f], [360.0f, 780.0f]],
        [[-416.0f, 100.0f], [-416.0f, 380.0f], [1120.0f, 120.0f], [1120.0f, 380.0f], [360.0f, -280.0f], [360.0f, 780.0f]],
    ];

    private static readonly float[][][] dm_stgslct_zone_nodisp_pos_tbl_mobile = [
        [[-416.0f - 50.0f, 100.0f - 30.0f], [-416.0f - 50.0f, 400.0f - 20.0f], [1120.0f - 10.0f, 100.0f - 30.0f], [1120.0f - 10.0f, 400.0f - 20.0f], [360.0f - 30.0f, -280.0f - 30.0f], [360.0f - 30.0f, 780.0f - 20.0f]],
        [[-416.0f - 50.0f, 100.0f - 30.0f], [-416.0f - 50.0f, 400.0f - 20.0f], [1120.0f - 10.0f, 100.0f - 30.0f], [1120.0f - 10.0f, 400.0f - 20.0f], [360.0f - 30.0f, -280.0f - 30.0f], [360.0f - 30.0f, 780.0f - 20.0f]],
        [[-416.0f - 50.0f, 100.0f - 30.0f], [-416.0f - 50.0f, 400.0f - 20.0f], [1120.0f - 10.0f, 100.0f - 30.0f], [1120.0f - 10.0f, 400.0f - 20.0f], [360.0f - 30.0f, -280.0f - 30.0f], [360.0f - 30.0f, 780.0f - 20.0f]],
        [[-416.0f - 50.0f, 100.0f - 30.0f], [-416.0f - 50.0f, 380.0f - 20.0f], [1120.0f - 10.0f, 120.0f - 30.0f], [1120.0f - 10.0f, 380.0f - 20.0f], [360.0f - 30.0f, -280.0f - 30.0f], [360.0f - 30.0f, 780.0f - 20.0f]],
    ];

    private static readonly float[] dm_stgslct_act_disp_y_pos_tbl_console = [
        160.0f, 160.0f, 160.0f, 160.0f,
        160.0f, 160.0f, 160.0f, 160.0f,
        160.0f, 160.0f, 160.0f, 160.0f,
        160.0f, 160.0f, 160.0f, 160.0f,
        320.0f,
        160.0f, 160.0f, 160.0f, 160.0f, 160.0f, 160.0f, 160.0f,
    ];

    private static readonly float[] dm_stgslct_act_disp_y_pos_tbl_mobile = [
        190.0f, 190.0f, 190.0f, 190.0f,
        190.0f, 190.0f, 190.0f, 190.0f,
        190.0f, 190.0f, 190.0f, 190.0f,
        190.0f, 190.0f, 190.0f, 190.0f,
        320.0f,
        190.0f, 190.0f, 190.0f, 190.0f, 190.0f, 190.0f, 190.0f,
    ];

    // console shows four rows, mobile five, so these two are different lengths
    private static readonly float[] dm_stgslct_act_tab_disp_y_pos_tbl_console = [
        160.0f,
        160.0f + 88.0f * -1.0f,
        160.0f + 88.0f * -2.0f,
        160.0f + 88.0f * -3.0f,
    ];

    private static readonly float[] dm_stgslct_act_tab_disp_y_pos_tbl_mobile = [
        dm_stgslct_act_disp_y_pos_tbl_mobile[(int)DME_STGSLCT_STAGE_1_1] - (DMD_STGSLCT_ACT_TABLE_DIST_Y_MOBILE - 8) * 0,
        dm_stgslct_act_disp_y_pos_tbl_mobile[(int)DME_STGSLCT_STAGE_1_1] - (DMD_STGSLCT_ACT_TABLE_DIST_Y_MOBILE - 8) * 1,
        dm_stgslct_act_disp_y_pos_tbl_mobile[(int)DME_STGSLCT_STAGE_1_1] - (DMD_STGSLCT_ACT_TABLE_DIST_Y_MOBILE - 8) * 2,
        dm_stgslct_act_disp_y_pos_tbl_mobile[(int)DME_STGSLCT_STAGE_1_1] - (DMD_STGSLCT_ACT_TABLE_DIST_Y_MOBILE - 8) * 3,
        dm_stgslct_act_disp_y_pos_tbl_mobile[(int)DME_STGSLCT_STAGE_1_1] - (DMD_STGSLCT_ACT_TABLE_DIST_Y_MOBILE - 8) * 4,
    ];

    private static readonly float[] dm_stgslct_act_crsr_disp_y_pos_tbl = [
        160.0f,
        160.0f + 88.0f * 1.0f,
        160.0f + 88.0f * 2.0f,
        160.0f + 88.0f * 3.0f,
    ];

    // per window type: title frame, message frame, pattern number
    private static readonly float[][] dm_stgslct_win_act_frm_tbl = [
        [2.0f, 0.0f, 1.0f],
        [3.0f, 0.0f, 0.0f],
        [1.0f, 1.0f, 0.0f],
        [1.0f, 2.0f, 0.0f],
        [1.0f, 3.0f, 0.0f],
        [1.0f, 4.0f, 0.0f],
        [1.0f, 5.0f, 0.0f],
        [1.0f, 6.0f, 0.0f],
        [1.0f, 7.0f, 0.0f],
        [1.0f, 7.0f, 0.0f],
    ];

    // row 0 left/top active, row 1 right/bottom active
    private static readonly float[][] dm_stgslct_win_disp_slct_frm_tbl = [
        [0.0f, 1.0f],
        [1.0f, 0.0f],
    ];

    private static readonly float[] dm_stgslct_back_text_length_tbl = [
        -39.0f,
        -53.0f,
        -69.0f,
        -77.0f,
        -70.0f,
        -55.0f,
    ];

    // window line, cancel btn, X btn, Y btn, message, message2, super sonic message, option,
    // main menu, title text, back, OK, YES, NO
    private static readonly float[][] dm_stgslct_win_act_pos_tbl_console = [
        [DMD_STGSLCT_SIZE_HALF_WIDTH +  42.0f, 280.0f],
        [DMD_STGSLCT_SIZE_HALF_WIDTH + 182.0f, 264.0f],
        [DMD_STGSLCT_SIZE_HALF_WIDTH - 136.0f, 400.0f],
        [DMD_STGSLCT_SIZE_HALF_WIDTH -  90.0f, 400.0f],
        [DMD_STGSLCT_SIZE_HALF_WIDTH,          360.0f],
        [DMD_STGSLCT_SIZE_HALF_WIDTH,          344.0f],
        [DMD_STGSLCT_SIZE_HALF_WIDTH,          360.0f],
        [DMD_STGSLCT_SIZE_HALF_WIDTH,          344.0f],
        [DMD_STGSLCT_SIZE_HALF_WIDTH,          404.0f],
        [DMD_STGSLCT_SIZE_HALF_WIDTH -  80.0f, 274.0f],
        [DMD_STGSLCT_SIZE_HALF_WIDTH + 202.0f, 264.0f],
        [DMD_STGSLCT_SIZE_HALF_WIDTH,          420.0f],
        [DMD_STGSLCT_SIZE_HALF_WIDTH -  88.0f, 420.0f],
        [DMD_STGSLCT_SIZE_HALF_WIDTH +  88.0f, 420.0f],
    ];

    private static readonly float[][] dm_stgslct_win_act_pos_tbl_mobile = [
        [DMD_STGSLCT_SIZE_HALF_WIDTH +  42.0f, 280.0f],
        [DMD_STGSLCT_SIZE_HALF_WIDTH + 278.0f, 224.0f],
        [DMD_STGSLCT_SIZE_HALF_WIDTH - 136.0f, 400.0f],
        [DMD_STGSLCT_SIZE_HALF_WIDTH -  90.0f, 400.0f],
        [DMD_STGSLCT_SIZE_HALF_WIDTH,          360.0f],
        [DMD_STGSLCT_SIZE_HALF_WIDTH,          248.0f],
        [DMD_STGSLCT_SIZE_HALF_WIDTH,          360.0f],
        [DMD_STGSLCT_SIZE_HALF_WIDTH,          344.0f],
        [DMD_STGSLCT_SIZE_HALF_WIDTH,          404.0f],
        [DMD_STGSLCT_SIZE_HALF_WIDTH -  80.0f, 274.0f],
        [DMD_STGSLCT_SIZE_HALF_WIDTH + 298.0f, 224.0f],
        [DMD_STGSLCT_SIZE_HALF_WIDTH,          420.0f],
        [DMD_STGSLCT_SIZE_HALF_WIDTH - 176.0f, 420.0f],
        [DMD_STGSLCT_SIZE_HALF_WIDTH + 176.0f, 420.0f],
    ];

    private static readonly float[] dm_stgslct_act_table_disp_id_tbl = [
        0.0f, 1.0f, 2.0f, 3.0f,
        0.0f, 1.0f, 2.0f, 3.0f,
        0.0f, 1.0f, 2.0f, 3.0f,
        0.0f, 1.0f, 2.0f, 3.0f,
        4.0f,
        5.0f, 5.0f, 5.0f, 5.0f, 5.0f, 5.0f, 5.0f,
    ];

    private static readonly float[] dm_stgslct_disp_msg_id_table_tbl = [
         0.0f,  1.0f,  2.0f,  3.0f,
         4.0f,  5.0f,  6.0f,  7.0f,
         8.0f,  9.0f, 10.0f, 11.0f,
        12.0f, 13.0f, 14.0f, 15.0f,
        16.0f,
         5.0f,  5.0f,  5.0f,  5.0f,  5.0f,  5.0f,  5.0f,
    ];

    private static readonly float[] dm_stgslct_act_disp_pos_y_tbl = [
        0.0f, 1.0f, 2.0f, 3.0f,
        0.0f, 1.0f, 2.0f, 3.0f,
        0.0f, 1.0f, 2.0f, 3.0f,
        0.0f, 1.0f, 2.0f, 3.0f,
        0.0f,
        0.0f, 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f,
    ];

    private static readonly float[] dm_stgslct_act_num_disp_id_tbl = [
        0.0f, 1.0f, 2.0f, 0.0f,
        0.0f, 1.0f, 2.0f, 0.0f,
        0.0f, 1.0f, 2.0f, 0.0f,
        0.0f, 1.0f, 2.0f, 0.0f,
        0.0f,
        0.0f, 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f,
    ];

    // indexed by DME_STGSLCT_ACT_REG, 130 entries, and index-locked to it
    private static readonly uint[] dm_stgslct_act_id_tbl_console = [
        ConsoleIds.Z_BG_LT,
        ConsoleIds.Z_BG_LB,
        ConsoleIds.Z_BG_RT,
        ConsoleIds.Z_BG_RB,
        ConsoleIds.TAB_NORMAL_L,
        ConsoleIds.TAB_NORMAL_R,
        ConsoleIds.ICON_SONIC,
        ConsoleIds.TEX_SONIC,
        ConsoleIds.NUM_A_100,
        ConsoleIds.NUM_A_10,
        ConsoleIds.NUM_A_1,
        ConsoleIds.EMER_TAB1,
        ConsoleIds.ICON_EMER1,
        ConsoleIds.ICON_EMER2,
        ConsoleIds.ICON_EMER3,
        ConsoleIds.ICON_EMER4,
        ConsoleIds.ICON_EMER5,
        ConsoleIds.ICON_EMER6,
        ConsoleIds.ICON_EMER7,
        ConsoleIds.TEX_ZNAME,

        ConsoleIds.JP_TEX_TIME,
        ConsoleIds.JP_TEX_SCORE,
        ConsoleIds.JP_TEX_MENU,
        ConsoleIds.TEX_EME,
        ConsoleIds.JP_TEX_ZNAME,
        ConsoleIds.JP_TEX_MESS,
        ConsoleIds.JP_TEX_MESS,

        ConsoleIds.ZONE_SCR1,
        ConsoleIds.ZONE_SCR2,
        ConsoleIds.ZONE_SCR3,
        ConsoleIds.ZONE_SCR4,
        ConsoleIds.ZONE_SCR5,
        ConsoleIds.ZONE_SCR6,
        ConsoleIds.ZONE_SCR1_A,
        ConsoleIds.ZONE_SCR1_B,
        ConsoleIds.ZONE_SCR1_C,
        ConsoleIds.ZONE_SCR2_A,
        ConsoleIds.ZONE_SCR2_B,
        ConsoleIds.ZONE_SCR2_C,
        ConsoleIds.ZONE_SCR3_A,
        ConsoleIds.ZONE_SCR3_B,
        ConsoleIds.ZONE_SCR3_C,
        ConsoleIds.ZONE_SCR4_A,
        ConsoleIds.ZONE_SCR4_B,
        ConsoleIds.ZONE_SCR4_C,
        ConsoleIds.ZONE_TAB,
        ConsoleIds.ZONE_TEXT,
        ConsoleIds.JP_ZONE_TEXT,
        ConsoleIds.ZONE_COVER1,
        ConsoleIds.ZONE_COVER2,
        ConsoleIds.ZONE_COVER3,
        ConsoleIds.ZONE_CURSOR2,
        ConsoleIds.ZONE_CURSOR1,
        ConsoleIds.ZONE_CURSOR3,

        ConsoleIds.ICON1,
        ConsoleIds.ICON2,
        ConsoleIds.ICON3,
        ConsoleIds.ICON4,
        ConsoleIds.ICON5,
        ConsoleIds.ICON6,
        ConsoleIds.ARROW1,
        ConsoleIds.ARROW2,
        ConsoleIds.TAB_TIME_L,
        ConsoleIds.TAB_TIME_C,
        ConsoleIds.TAB_TIME_R,
        ConsoleIds.TAB_TIME_L2,
        ConsoleIds.TAB_TIME_C2,
        ConsoleIds.TAB_TIME_R2,
        ConsoleIds.BACK_TAB,
        ConsoleIds.ACT1_TAB_B,
        ConsoleIds.ACT1_TAB_A,
        ConsoleIds.ACT1_TAB_C,
        ConsoleIds.ACT1_SCR,
        ConsoleIds.ACT1_SCR_BG,
        ConsoleIds.JP_ACT1_TEX_ACT,
        ConsoleIds.ACT1_NUM_B,
        ConsoleIds.JP_ACT1_MESS,
        ConsoleIds.ACT1_LINE,
        ConsoleIds.ACT1_ICON_EMER,
        ConsoleIds.SPE_NUM_STAGE,
        ConsoleIds.SPE_EMER,
        ConsoleIds.ACT_CURSOR_UP,
        ConsoleIds.ACT_CURSOR_DOWN,
        ConsoleIds.ACT_CURSOR2,
        ConsoleIds.ACT_CURSOR1,
        ConsoleIds.ACT_CURSOR3,
        ConsoleIds.JP_ACT1_TEX_SCORE,
        ConsoleIds.JP_ACT1_TEX_TIME,
        ConsoleIds.JP_BOSS_TEX_BOSS,
        ConsoleIds.JP_SPE_TEX_STAGE,
        ConsoleIds.ACT1_NUM_A1,
        ConsoleIds.ACT1_NUM_A2,
        ConsoleIds.ACT1_NUM_A3,
        ConsoleIds.ACT1_NUM_A4,
        ConsoleIds.ACT1_NUM_A5,
        ConsoleIds.ACT1_NUM_A6,
        ConsoleIds.ACT1_NUM_A7,
        ConsoleIds.ACT1_NUM_A8,
        ConsoleIds.ACT1_NUM_A9,
        ConsoleIds.ACT1_COVER2,
        ConsoleIds.ACT1_COVER1,
        ConsoleIds.ACT1_COVER3,

        ConsoleIds.JP_TEX_SCTM01A,
        ConsoleIds.JP_TEX_SCTM02A,
        ConsoleIds.JP_TEX_SCTM01B,
        ConsoleIds.JP_TEX_SCTM02B,

        ConsoleIds.JP_TEX_WIN_MSG,
        ConsoleIds.JP_TEX_WIN_MSG2,
        ConsoleIds.JP_TEX_WIN_SSONIC,

        IDA_D_CMN_BG_ACT_BG_WAVE,
        IDA_D_CMN_BG_ACT_BG_DOWN_WHITE,
        IDA_D_CMN_BG_ACT_BG_BLUE,

        IDA_D_CMN_BTN_ACT_BTN_BACK_STGSLCT,
        IDA_D_CMN_BTN_ACT_BTN_LB_STGSLCT,
        IDA_D_CMN_BTN_ACT_BTN_Y_STGSLCT,
        IDA_D_CMN_BTN_ACT_BTN_LEFT_LB_STGSLCT,
        IDA_D_CMN_BTN_ACT_BTN_LEFT_RB_STGSLCT,
        IDA_D_CMN_BTN_ACT_BACK_BTN,
        IDA_D_CMN_BTN_ACT_BTN_X,
        IDA_D_CMN_BTN_ACT_BTN_Y,

        IDA_D_CMN_OBI_ACT_OBI_CENTER,
        IDA_D_CMN_OBI_ACT_OBI_LEFT,
        IDA_D_CMN_OBI_ACT_OBI_RIGHT,
        IDA_D_CMN_WIN_ACT_WIN_LINE_PC,

        IDA_D_CMN_MSG_JP_ACT_TEX_BACK_STGSLCT_PC,
        IDA_D_CMN_MSG_JP_ACT_TEX_WINTITLE_PC,
        IDA_D_CMN_MSG_JP_ACT_TEX_BACK_PC,
        IDA_D_CMN_MSG_JP_ACT_TEX_OK_PC,
        IDA_D_CMN_MSG_JP_ACT_TEX_YES_PC,
        IDA_D_CMN_MSG_JP_ACT_TEX_NO_PC,
    ];

    // indexed by DME_STGSLCT_ACT, 116 entries, and index-locked to it
    private static readonly uint[] dm_stgslct_act_id_tbl_mobile = [
        MobileIds.Z_BG_LT,
        MobileIds.Z_BG_LB,
        MobileIds.Z_BG_RT,
        MobileIds.Z_BG_RB,
        MobileIds.TAB_NORMAL_L,
        MobileIds.ICON_SONIC,
        MobileIds.NUM_A_100,
        MobileIds.NUM_A_10,
        MobileIds.NUM_A_1,
        MobileIds.EMER_TAB1,
        MobileIds.ICON_EMER1,
        MobileIds.ICON_EMER2,
        MobileIds.ICON_EMER3,
        MobileIds.ICON_EMER4,
        MobileIds.ICON_EMER5,
        MobileIds.ICON_EMER6,
        MobileIds.ICON_EMER7,
        MobileIds.TEX_ZNAME,

        MobileIds.JP_TEX_ZNAME,

        MobileIds.ZONE_SCR1,
        MobileIds.ZONE_SCR2,
        MobileIds.ZONE_SCR3,
        MobileIds.ZONE_SCR4,
        MobileIds.ZONE_SCR5,
        MobileIds.ZONE_SCR6,
        MobileIds.ZONE_SCR1_A,
        MobileIds.ZONE_SCR1_B,
        MobileIds.ZONE_SCR1_C,
        MobileIds.ZONE_SCR2_A,
        MobileIds.ZONE_SCR2_B,
        MobileIds.ZONE_SCR2_C,
        MobileIds.ZONE_SCR3_A,
        MobileIds.ZONE_SCR3_B,
        MobileIds.ZONE_SCR3_C,
        MobileIds.ZONE_SCR4_A,
        MobileIds.ZONE_SCR4_B,
        MobileIds.ZONE_SCR4_C,
        MobileIds.ZONE_TAB,
        MobileIds.ZONE_TEXT,
        MobileIds.JP_ZONE_TEXT,
        MobileIds.ZONE_COVER1,
        MobileIds.ZONE_COVER2,
        MobileIds.ZONE_COVER3,

        MobileIds.ICON1,
        MobileIds.ICON2,
        MobileIds.ICON3,
        MobileIds.ICON4,
        MobileIds.ICON5,
        MobileIds.ICON6,
        MobileIds.ARROW1,
        MobileIds.ARROW2,
        MobileIds.TAB_TIME_L,
        MobileIds.TAB_TIME_C,
        MobileIds.TAB_TIME_R,
        MobileIds.TAB_TIME_L2,
        MobileIds.TAB_TIME_C2,
        MobileIds.TAB_TIME_R2,
        MobileIds.TAB_TIME_MOVE,
        MobileIds.ACT1_TAB_B,
        MobileIds.ACT1_TAB_A,
        MobileIds.ACT1_TAB_C,
        MobileIds.ACT1_SCR,
        MobileIds.ACT1_SCR_BG,
        MobileIds.JP_ACT1_TEX_ACT,
        MobileIds.ACT1_NUM_B,
        MobileIds.JP_ACT1_MESS,
        MobileIds.ACT1_LINE,
        MobileIds.ACT1_ICON_EMER,
        MobileIds.SPE_NUM_STAGE,
        MobileIds.SPE_EMER,
        MobileIds.ACT_CURSOR_UP,
        MobileIds.ACT_CURSOR_DOWN,
        MobileIds.JP_ACT1_TEX_SCORE,
        MobileIds.JP_ACT1_TEX_TIME,
        MobileIds.JP_BOSS_TEX_BOSS,
        MobileIds.JP_SPE_TEX_STAGE,
        MobileIds.ACT1_NUM_A1,
        MobileIds.ACT1_NUM_A2,
        MobileIds.ACT1_NUM_A3,
        MobileIds.ACT1_NUM_A4,
        MobileIds.ACT1_NUM_A5,
        MobileIds.ACT1_NUM_A6,
        MobileIds.ACT1_NUM_A7,
        MobileIds.ACT1_NUM_A8,
        MobileIds.ACT1_NUM_A9,
        MobileIds.ACT1_COVER2,
        MobileIds.ACT1_COVER1,
        MobileIds.ACT1_COVER3,

        MobileIds.JP_TEX_SCTM01A,
        MobileIds.JP_TEX_SCTM02A,

        MobileIds.JP_TEX_WIN_MSG,
        MobileIds.JP_TEX_WIN_MSG2,
        MobileIds.JP_TEX_WIN_SSONIC,

        IDA_D_CMN_BG_ACT_BG_WAVE,
        IDA_D_CMN_BG_ACT_BG_DOWN_WHITE,
        IDA_D_CMN_BG_ACT_BG_BLUE,

        IDA_D_CMN_BTN_ACT_BTN_BACK_STGSLCT,
        IDA_D_CMN_BTN_ACT_BTN_LB_STGSLCT,
        IDA_D_CMN_BTN_ACT_BTN_Y_STGSLCT,
        IDA_D_CMN_BTN_ACT_BTN_LEFT_LB_STGSLCT,
        IDA_D_CMN_BTN_ACT_BTN_LEFT_RB_STGSLCT,
        IDA_D_CMN_BTN_ACT_BACK_BTN,
        IDA_D_CMN_BTN_ACT_BTN_X,
        IDA_D_CMN_BTN_ACT_BTN_Y,

        IDA_D_CMN_WIN_ACT_STGSLCT_BACK_L,
        IDA_D_CMN_WIN_ACT_STGSLCT_BACK_C,
        IDA_D_CMN_WIN_ACT_BTN01_L,
        IDA_D_CMN_WIN_ACT_BTN01_C,
        IDA_D_CMN_WIN_ACT_BTN01_R,
        IDA_D_CMN_WIN_ACT_BTN01_L,
        IDA_D_CMN_WIN_ACT_BTN01_C,
        IDA_D_CMN_WIN_ACT_BTN01_R,

        IDA_D_CMN_MSG_JP_ACT_TEX_BACK_STGSLCT,
        IDA_D_CMN_MSG_JP_ACT_TEX_BACK,
        IDA_D_CMN_MSG_JP_ACT_TEX_YES,
        IDA_D_CMN_MSG_JP_ACT_TEX_NO,
    ];

    // built by name rather than hand-written, because 131 pairs typed out is 131 chances to put one
    // sprite in the wrong slot
    private static int[] dmStgSlctBuildActIndex(System.Type target)
    {
        var keys = (int)DME_STGSLCT_ACT_REG.ACT_NUM + 1;
        var index = new int[keys];

        for (int key = 0; key < keys; key++)
        {
            var name = System.Enum.GetName(typeof(DME_STGSLCT_ACT_REG), key);

            index[key] = name != null && System.Enum.IsDefined(target, name)
                ? (int)System.Enum.Parse(target, name)
                : -1;
        }

        return index;
    }

    private static int[] dmStgSlctBuildActSlots(int act_num, System.Func<int, int> slot_for)
    {
        var slots = new int[act_num];

        for (int i = 0; i < act_num; i++)
            slots[i] = slot_for(i);

        return slots;
    }

    private static int dmStgSlctConsoleActSlot(int i)
    {
        var act = (DME_STGSLCT_ACT_REG)i;

        if (act >= DME_STGSLCT_ACT_REG.ACT_TEX_FIX_BACK) return DMD_STGSLCT_SLOT_CMN_MSG;
        if (act >= DME_STGSLCT_ACT_REG.ACT_WIN_LINE) return DMD_STGSLCT_SLOT_CMN_WIN;
        if (act >= DME_STGSLCT_ACT_REG.ACT_OBI_C) return DMD_STGSLCT_SLOT_CMN_OBI;
        if (act >= DME_STGSLCT_ACT_REG.ACT_BTN_CANCEL1) return DMD_STGSLCT_SLOT_CMN_BTN;
        if (act >= DME_STGSLCT_ACT_REG.ACT_WAVE_BG) return DMD_STGSLCT_SLOT_CMN_BG;
        if (act >= DME_STGSLCT_ACT_REG.ACT_TEX_BIG_TIME) return DMD_STGSLCT_SLOT_LANG_DATA;

        var localised =
            act >= DME_STGSLCT_ACT_REG.ACT_TEX_MODE_TIME && act <= DME_STGSLCT_ACT_REG.ACT_TEX_MENU
            || act >= DME_STGSLCT_ACT_REG.ACT_TEX_ZONE_UP_S && act <= DME_STGSLCT_ACT_REG.ACT_TEX_OBI2
            || act >= DME_STGSLCT_ACT_REG.ACT_TAB_TEX_SCORE && act <= DME_STGSLCT_ACT_REG.ACT_TAB_TEX_SPE_STAGE
            || act == DME_STGSLCT_ACT_REG.ACT_TAB_ZONE_TEXT_S
            || act == DME_STGSLCT_ACT_REG.ACT_TAB_TEXT
            || act == DME_STGSLCT_ACT_REG.ACT_TAB_MESS;

        return localised ? DMD_STGSLCT_SLOT_LANG_DATA : DMD_STGSLCT_SLOT_CMN_DATA;
    }

    // the touch button runs sit above ACT_BTN_CANCEL1, so they have to be tested before it
    private static int dmStgSlctMobileActSlot(int i)
    {
        var act = (DME_STGSLCT_ACT)i;

        if (act >= DME_STGSLCT_ACT.ACT_TEX_FIX_BACK) return DMD_STGSLCT_SLOT_CMN_MSG;
        if (act >= DME_STGSLCT_ACT.ACT_BACK_BTN01_L && act <= DME_STGSLCT_ACT.ACT_BACK_BTN01_R) return DMD_STGSLCT_SLOT_CMN_WIN;
        if (act >= DME_STGSLCT_ACT.ACT_YES_BTN_L && act <= DME_STGSLCT_ACT.ACT_NO_BTN_R) return DMD_STGSLCT_SLOT_CMN_WIN;
        if (act >= DME_STGSLCT_ACT.ACT_BTN_CANCEL1) return DMD_STGSLCT_SLOT_CMN_BTN;
        if (act >= DME_STGSLCT_ACT.ACT_WAVE_BG) return DMD_STGSLCT_SLOT_CMN_BG;
        if (act >= DME_STGSLCT_ACT.ACT_TEX_BIG_TIME) return DMD_STGSLCT_SLOT_LANG_DATA;

        var localised =
            act == DME_STGSLCT_ACT.ACT_TEX_ZONE_UP_S
            || act >= DME_STGSLCT_ACT.ACT_TAB_TEX_SCORE && act <= DME_STGSLCT_ACT.ACT_TAB_TEX_SPE_STAGE
            || act == DME_STGSLCT_ACT.ACT_TAB_ZONE_TEXT_S
            || act == DME_STGSLCT_ACT.ACT_TAB_TEXT
            || act == DME_STGSLCT_ACT.ACT_TAB_MESS;

        return localised ? DMD_STGSLCT_SLOT_LANG_DATA : DMD_STGSLCT_SLOT_CMN_DATA;
    }

    // these two must stay in this file, below every table they name. static field initialisers run
    // in textual order within a file, but the order across a partial class's files is not defined,
    // so putting them next to the descriptor type would bind these to null.
    private static readonly DMS_STGSLCT_VARIANT dm_stgslct_variant_console = new DMS_STGSLCT_VARIANT
    {
        act_id_tbl = dm_stgslct_act_id_tbl_console,
        act_slot_tbl = dmStgSlctBuildActSlots((int)DME_STGSLCT_ACT_REG.ACT_NUM, dmStgSlctConsoleActSlot),
        act_num = (int)DME_STGSLCT_ACT_REG.ACT_NUM,
        zone_disp_pos_tbl = dm_stgslct_zone_disp_pos_tbl_console,
        zone_nodisp_pos_tbl = dm_stgslct_zone_nodisp_pos_tbl_console,
        act_disp_y_pos_tbl = dm_stgslct_act_disp_y_pos_tbl_console,
        act_tab_disp_y_pos_tbl = dm_stgslct_act_tab_disp_y_pos_tbl_console,
        win_act_pos_tbl = dm_stgslct_win_act_pos_tbl_console,
        draw_prio_win = DMD_STGSLCT_DRAW_PRIO_WIN_CONSOLE,
        act_table_top_pos_x = DMD_STGSLCT_ACT_TABLE_TOP_POS_X_CONSOLE,
        act_table_dist_y = DMD_STGSLCT_ACT_TABLE_DIST_Y_CONSOLE,
        window_size_w = DMD_STGSLCT_WINDOW_SIZE_W_CONSOLE,
        is_console = true,
        fs_variant = AMD_FS_VARIANT_CONSOLE,
        act_index = dmStgSlctBuildActIndex(typeof(DME_STGSLCT_ACT_REG)),
        act_tab_table2 = (int)DME_STGSLCT_ACT_REG.ACT_TAB_TABLE2,
        act_tab_table3 = (int)DME_STGSLCT_ACT_REG.ACT_TAB_TABLE3,
        act_tab_scr = (int)DME_STGSLCT_ACT_REG.ACT_TAB_SCR,
        act_tab_mess = (int)DME_STGSLCT_ACT_REG.ACT_TAB_MESS,
        input_zone_select = dmStgSlctInputProcZoneSelectPc,
        input_stage_select = dmStgSlctInputProcStageSelectPc,
        input_win_disp_idle = dmStgSlctInputProcWinDispIdlePc,
    };

    private static readonly DMS_STGSLCT_VARIANT dm_stgslct_variant_mobile = new DMS_STGSLCT_VARIANT
    {
        act_id_tbl = dm_stgslct_act_id_tbl_mobile,
        act_slot_tbl = dmStgSlctBuildActSlots((int)DME_STGSLCT_ACT.ACT_NUM, dmStgSlctMobileActSlot),
        act_num = (int)DME_STGSLCT_ACT.ACT_NUM,
        zone_disp_pos_tbl = dm_stgslct_zone_disp_pos_tbl_mobile,
        zone_nodisp_pos_tbl = dm_stgslct_zone_nodisp_pos_tbl_mobile,
        act_disp_y_pos_tbl = dm_stgslct_act_disp_y_pos_tbl_mobile,
        act_tab_disp_y_pos_tbl = dm_stgslct_act_tab_disp_y_pos_tbl_mobile,
        win_act_pos_tbl = dm_stgslct_win_act_pos_tbl_mobile,
        draw_prio_win = DMD_STGSLCT_DRAW_PRIO_WIN_MOBILE,
        act_table_top_pos_x = DMD_STGSLCT_ACT_TABLE_TOP_POS_X_MOBILE,
        act_table_dist_y = DMD_STGSLCT_ACT_TABLE_DIST_Y_MOBILE,
        window_size_w = DMD_STGSLCT_WINDOW_SIZE_W_MOBILE,
        is_console = false,
        fs_variant = AMD_FS_VARIANT_BASE,
        act_index = dmStgSlctBuildActIndex(typeof(DME_STGSLCT_ACT)),
        act_tab_table2 = (int)DME_STGSLCT_ACT.ACT_TAB_TABLE2,
        act_tab_table3 = (int)DME_STGSLCT_ACT.ACT_TAB_TABLE3,
        act_tab_scr = (int)DME_STGSLCT_ACT.ACT_TAB_SCR,
        act_tab_mess = (int)DME_STGSLCT_ACT.ACT_TAB_MESS,
        input_zone_select = dmStgSlctInputProcZoneSelectIphone,
        input_stage_select = dmStgSlctInputProcStageSelectIphone,
        input_win_disp_idle = dmStgSlctInputProcWinDispIdleIphone,
    };
}
