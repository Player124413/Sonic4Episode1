using static AppMain;

using static GmStartDemo.GME_START_DEMO_DATA_TYPE;

internal static partial class GmStartDemo
{
    // The five languages after SP are WP7 additions: iOS never shipped them, so their members only
    // exist in the wp7-orig CPIT_MAIN (98 members against the HD set's 58) and the indices here run
    // past the end of every other build's copy. gmStartDemoLanguage falls those back to US.
    private static readonly int[][] gm_start_demo_data_amb_id =
    [
        [IDB_CPIT_MAIN_G_START_AMB, IDB_CPIT_MAIN_G_START_JP_AMB],
        [IDB_CPIT_MAIN_G_START_AMB, IDB_CPIT_MAIN_G_START_US_AMB],
        [IDB_CPIT_MAIN_G_START_AMB, IDB_CPIT_MAIN_G_START_FR_AMB],
        [IDB_CPIT_MAIN_G_START_AMB, IDB_CPIT_MAIN_G_START_IT_AMB],
        [IDB_CPIT_MAIN_G_START_AMB, IDB_CPIT_MAIN_G_START_GE_AMB],
        [IDB_CPIT_MAIN_G_START_AMB, IDB_CPIT_MAIN_G_START_SP_AMB],
        [IDB_CPIT_MAIN_G_START_AMB, IDB_CPIT_MAIN_G_START_FI_AMB],
        [IDB_CPIT_MAIN_G_START_AMB, IDB_CPIT_MAIN_G_START_PT_AMB],
        [IDB_CPIT_MAIN_G_START_AMB, IDB_CPIT_MAIN_G_START_RU_AMB],
        [IDB_CPIT_MAIN_G_START_AMB, IDB_CPIT_MAIN_G_START_CN_AMB],
        [IDB_CPIT_MAIN_G_START_AMB, IDB_CPIT_MAIN_G_START_HK_AMB],
    ];

    private static readonly int[][] gm_start_demo_data_ama_id =
    [
        [IDB_CPIT_MAIN_G_START_AMA, IDB_CPIT_MAIN_G_START_JP_AMA],
        [IDB_CPIT_MAIN_G_START_AMA, IDB_CPIT_MAIN_G_START_US_AMA],
        [IDB_CPIT_MAIN_G_START_AMA, IDB_CPIT_MAIN_G_START_FR_AMA],
        [IDB_CPIT_MAIN_G_START_AMA, IDB_CPIT_MAIN_G_START_IT_AMA],
        [IDB_CPIT_MAIN_G_START_AMA, IDB_CPIT_MAIN_G_START_GE_AMA],
        [IDB_CPIT_MAIN_G_START_AMA, IDB_CPIT_MAIN_G_START_SP_AMA],
        [IDB_CPIT_MAIN_G_START_AMA, IDB_CPIT_MAIN_G_START_FI_AMA],
        [IDB_CPIT_MAIN_G_START_AMA, IDB_CPIT_MAIN_G_START_PT_AMA],
        [IDB_CPIT_MAIN_G_START_AMA, IDB_CPIT_MAIN_G_START_RU_AMA],
        [IDB_CPIT_MAIN_G_START_AMA, IDB_CPIT_MAIN_G_START_CN_AMA],
        [IDB_CPIT_MAIN_G_START_AMA, IDB_CPIT_MAIN_G_START_HK_AMA],
    ];

    // the members the console variant supplies, all six of its languages plus the shared pair
    private static readonly int[] gm_start_demo_console_members =
    [
        IDB_CPIT_MAIN_G_START_AMA, IDB_CPIT_MAIN_G_START_AMB,
        IDB_CPIT_MAIN_G_START_FR_AMA, IDB_CPIT_MAIN_G_START_FR_AMB,
        IDB_CPIT_MAIN_G_START_GE_AMA, IDB_CPIT_MAIN_G_START_GE_AMB,
        IDB_CPIT_MAIN_G_START_IT_AMA, IDB_CPIT_MAIN_G_START_IT_AMB,
        IDB_CPIT_MAIN_G_START_JP_AMA, IDB_CPIT_MAIN_G_START_JP_AMB,
        IDB_CPIT_MAIN_G_START_SP_AMA, IDB_CPIT_MAIN_G_START_SP_AMB,
        IDB_CPIT_MAIN_G_START_US_AMA, IDB_CPIT_MAIN_G_START_US_AMB,
    ];

    private static readonly GME_START_DEMO_DATA_TYPE[] gm_start_demo_data_type_cmn =
    [
        GMD_START_DEMO_DATA_TYPE_CMN,
        GMD_START_DEMO_DATA_TYPE_CMN,
        GMD_START_DEMO_DATA_TYPE_CMN,

        GMD_START_DEMO_DATA_TYPE_CMN,
    ];

    private static readonly int[] gm_start_demo_action_id_cmn =
    [
        IDA_G_START_ACT_BG_WHITE,
        IDA_G_START_ROOT_NODE_BG_BLUE,
        IDA_G_START_ROOT_NODE_BG_RED,

        IDA_G_START_ACT_TEX_ZONE,
    ];

    private static readonly string[] gm_start_demo_action_name_cmn =
    [
        "START_DEMO_BG_WHITE",
        "START_DEMO_BG_BLUE",
        "START_DEMO_BG_RED",

        "START_DEMO_TEX_ZONE",
    ];

    private static readonly bool[] gm_start_demo_action_node_flag_cmn =
    [
        false,
        true,
        true,

        false,
    ];

    private static readonly GME_START_DEMO_DATA_TYPE[] gm_start_demo_data_type_zone =
    [
        GMD_START_DEMO_DATA_TYPE_CMN,
    ];

    // SEGA wrote the special stage row as NULL, i.e. action id 0, which is zone 1's name. Read as
    // the "no action" sentinel it declares two lines up instead. Unreachable either way: GmMain
    // routes special stages to GmSplStageStart. see docs/startdemo-port-notes.md
    private static readonly int[][] gm_start_demo_action_id_zone =
    [
        [IDA_G_START_ROOT_NODE_ZONE1],
        [IDA_G_START_ROOT_NODE_ZONE2],
        [IDA_G_START_ROOT_NODE_ZONE3],
        [IDA_G_START_ROOT_NODE_ZONE4],
        [IDA_G_START_ROOT_NODE_FINAL],
        [GMD_START_DEMO_INVLID_ID],
    ];

    private static readonly string[] gm_start_demo_action_name_zone =
    [
        "START_DEMO_TEX_ZNAME",
    ];

    private static readonly bool[] gm_start_demo_action_node_flag_zone =
    [
        true,
    ];

    private static readonly GME_START_DEMO_DATA_TYPE[] gm_start_demo_data_type_act =
    [
        GMD_START_DEMO_DATA_TYPE_CMN,
        GMD_START_DEMO_DATA_TYPE_LANG,
    ];

    // by stage id: four acts per zone, then the special stages and the ending, which never get here
    private static readonly int[] gm_start_demo_act_no =
    [
        0, 1, 2, 3,
        0, 1, 2, 3,
        0, 1, 2, 3,
        0, 1, 2, 3,
        0, 1, 2, 3, 3,
        0, 0, 0, 0, 0, 0, 0,
        0,
    ];

    private static readonly int[][] gm_start_demo_action_id_act =
    [
        [IDA_G_START_ROOT_NODE_NUM_1, IDA_G_START_US_ROOT_NODE_TEX_ACT],
        [IDA_G_START_ROOT_NODE_NUM_2, IDA_G_START_US_ROOT_NODE_TEX_ACT],
        [IDA_G_START_ROOT_NODE_NUM_3, IDA_G_START_US_ROOT_NODE_TEX_ACT],
        [GMD_START_DEMO_INVLID_ID, IDA_G_START_US_ROOT_NODE_TEX_BOSS],
    ];

    private static readonly int[][] gm_start_demo_action_id_message =
    [
        [
            IDA_G_START_US_ROOT_NODE_Z1_ACT1,
            IDA_G_START_US_ROOT_NODE_Z1_ACT2,
            IDA_G_START_US_ROOT_NODE_Z1_ACT3,
            IDA_G_START_US_ROOT_NODE_Z1_BOSS,
        ],
        [
            IDA_G_START_US_ROOT_NODE_Z2_ACT1,
            IDA_G_START_US_ROOT_NODE_Z2_ACT2,
            IDA_G_START_US_ROOT_NODE_Z2_ACT3,
            IDA_G_START_US_ROOT_NODE_Z2_BOSS,
        ],
        [
            IDA_G_START_US_ROOT_NODE_Z3_ACT1,
            IDA_G_START_US_ROOT_NODE_Z3_ACT2,
            IDA_G_START_US_ROOT_NODE_Z3_ACT3,
            IDA_G_START_US_ROOT_NODE_Z3_BOSS,
        ],
        [
            IDA_G_START_US_ROOT_NODE_Z4_ACT1,
            IDA_G_START_US_ROOT_NODE_Z4_ACT2,
            IDA_G_START_US_ROOT_NODE_Z4_ACT3,
            IDA_G_START_US_ROOT_NODE_Z4_BOSS,
        ],
        [
            IDA_G_START_US_ROOT_NODE_FINAL_BOSS,
            IDA_G_START_US_ROOT_NODE_FINAL_BOSS,
            IDA_G_START_US_ROOT_NODE_FINAL_BOSS,
            IDA_G_START_US_ROOT_NODE_FINAL_BOSS,
        ],
        [
            IDA_G_START_US_ROOT_NODE_FINAL_BOSS,
            IDA_G_START_US_ROOT_NODE_FINAL_BOSS,
            IDA_G_START_US_ROOT_NODE_FINAL_BOSS,
            IDA_G_START_US_ROOT_NODE_FINAL_BOSS,
        ],
    ];

    private static readonly string[] gm_start_demo_action_name_act =
    [
        "START_DEMO_NUM",
        "START_DEMO_TEX_ACT",
    ];

    private const string gm_start_demo_action_name_message = "START_DEMO_TEX_MESS";

    private static readonly bool[] gm_start_demo_action_node_flag_act =
    [
        true,
        true,
    ];
}
