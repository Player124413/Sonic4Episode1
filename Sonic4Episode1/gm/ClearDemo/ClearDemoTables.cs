using static AppMain;

internal static partial class GmClearDemo
{
    // per-language rows for the stage name and its digits. Italian reads the Spanish number table,
    // which is SEGA's, not a mistake in the port.
    private static readonly int[] dm_clrdm_stage_text_amb_id;
    private static ushort[] dm_clrdm_stage_text_act_id;
    private static readonly ushort[] dm_clrdm_stage_num_act_id;
    private static readonly ushort[] dm_clrdm_stage_ge_num_act_id;
    private static readonly ushort[] dm_clrdm_stage_fr_num_act_id;
    private static readonly ushort[] dm_clrdm_stage_sp_num_act_id;
    private static readonly ushort[] dm_clrdm_stage_tex_act_id;
    private static readonly ushort[] dm_clrdm_stage_ge_tex_act_id;

    // the G_RSLT member per language. Rows past the sixth index CPIT_MAIN members only the WP7
    // archive has, which is what GmGameDatCockpitLanguage falls back from.
    private static readonly int[] g_gm_clear_demo_data_ama_id;
    private static readonly int[] g_gm_clear_demo_data_amb_id;

    private static readonly int[][] dm_clrdm_clear_time_sec_tbl;
    private static readonly uint[] dm_clrdm_clear_time_score_tbl;
    private static readonly int[] dm_clrdm_spe_stg_get_act_no;

    private static readonly GMS_CLRDM_MGR gm_clrdm_mgr;
    private static GMS_CLRDM_MGR gm_clrdm_mgr_p;
    private static readonly AMS_AMB_HEADER[] gm_clrdm_amb;
    private static readonly AOS_TEXTURE[] gm_clrdm_tex;

    // a static constructor rather than field initialisers: this is the decompiler's cctor block
    // moved verbatim, and it aliases arrays between statements in a way initialisers cannot express
    static GmClearDemo()
    {
        dm_clrdm_stage_text_amb_id = new int[]
        {
        0,
        0,
        0,
        1,
        0,
        0,
        0,
        1,
        0,
        0,
        0,
        1,
        0,
        0,
        0,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1
        };
        dm_clrdm_stage_text_act_id = new ushort[]
        {
        4,
        4,
        4,
        5,
        4,
        4,
        4,
        5,
        4,
        4,
        4,
        5,
        4,
        4,
        4,
        5,
        6,
        6,
        6,
        6,
        6,
        4,
        4,
        4,
        4,
        4,
        4,
        4
        };
        dm_clrdm_stage_num_act_id = new ushort[]
        {
        1,
        2,
        3,
        ushort.MaxValue,
        1,
        2,
        3,
        ushort.MaxValue,
        1,
        2,
        3,
        ushort.MaxValue,
        1,
        2,
        3,
        ushort.MaxValue,
        ushort.MaxValue,
        ushort.MaxValue,
        ushort.MaxValue,
        ushort.MaxValue,
        ushort.MaxValue,
        1,
        2,
        3,
        3,
        3,
        3,
        3
        };
        dm_clrdm_stage_ge_num_act_id = new ushort[]
        {
        81,
        82,
        83,
        ushort.MaxValue,
        81,
        82,
        83,
        ushort.MaxValue,
        81,
        82,
        83,
        ushort.MaxValue,
        81,
        82,
        83,
        ushort.MaxValue,
        ushort.MaxValue,
        ushort.MaxValue,
        ushort.MaxValue,
        ushort.MaxValue,
        ushort.MaxValue,
        81,
        82,
        83,
        83,
        83,
        83,
        83
        };
        dm_clrdm_stage_fr_num_act_id = new ushort[]
        {
        93,
        94,
        95,
        ushort.MaxValue,
        93,
        94,
        95,
        ushort.MaxValue,
        93,
        94,
        95,
        ushort.MaxValue,
        93,
        94,
        95,
        ushort.MaxValue,
        ushort.MaxValue,
        ushort.MaxValue,
        ushort.MaxValue,
        ushort.MaxValue,
        ushort.MaxValue,
        93,
        94,
        95,
        95,
        95,
        95,
        95
        };
        dm_clrdm_stage_sp_num_act_id = new ushort[]
        {
        96,
        97,
        98,
        ushort.MaxValue,
        96,
        97,
        98,
        ushort.MaxValue,
        96,
        97,
        98,
        ushort.MaxValue,
        96,
        97,
        98,
        ushort.MaxValue,
        ushort.MaxValue,
        ushort.MaxValue,
        ushort.MaxValue,
        ushort.MaxValue,
        ushort.MaxValue,
        96,
        97,
        98,
        98,
        98,
        98,
        98
        };
        dm_clrdm_stage_tex_act_id = new ushort[]
        {
        0,
        0,
        0,
        1,
        0,
        0,
        0,
        1,
        0,
        0,
        0,
        1,
        0,
        0,
        0,
        1,
        2,
        2,
        2,
        2,
        2,
        13,
        13,
        13,
        13,
        13,
        13,
        13
        };
        dm_clrdm_stage_ge_tex_act_id = new ushort[]
        {
        2,
        2,
        2,
        1,
        2,
        2,
        2,
        1,
        2,
        2,
        2,
        1,
        2,
        2,
        2,
        1,
        2,
        2,
        2,
        2,
        2,
        13,
        13,
        13,
        13,
        13,
        13,
        13
        };
        g_gm_clear_demo_data_ama_id = new int[]
        {
        32,
        34,
        36,
        38,
        40,
        42,
        70,
        72,
        74,
        90,
        92
        };
        g_gm_clear_demo_data_amb_id = new int[]
        {
        33,
        35,
        37,
        39,
        41,
        43,
        71,
        73,
        75,
        91,
        93
        };
        dm_clrdm_clear_time_sec_tbl = new int[][]
        {
        new int[]
        {
            60,
            70,
            80,
            100,
            120,
            150,
            180,
            220,
            260
        },
        new int[]
        {
            70,
            80,
            90,
            110,
            130,
            160,
            190,
            230,
            270
        },
        new int[]
        {
            80,
            90,
            100,
            120,
            140,
            170,
            200,
            240,
            280
        },
        new int[]
        {
            40,
            45,
            50,
            60,
            70,
            90,
            110,
            140,
            170
        },
        new int[]
        {
            80,
            90,
            100,
            120,
            140,
            170,
            200,
            240,
            280
        },
        new int[]
        {
            170,
            180,
            190,
            210,
            230,
            260,
            290,
            330,
            370
        },
        new int[]
        {
            150,
            160,
            170,
            190,
            210,
            240,
            270,
            310,
            350
        },
        new int[]
        {
            60,
            65,
            70,
            80,
            90,
            110,
            130,
            160,
            190
        },
        new int[]
        {
            100,
            110,
            120,
            140,
            160,
            190,
            220,
            260,
            300
        },
        new int[]
        {
            170,
            180,
            190,
            210,
            230,
            260,
            290,
            330,
            370
        },
        new int[]
        {
            120,
            130,
            140,
            160,
            180,
            210,
            240,
            280,
            320
        },
        new int[]
        {
            100,
            105,
            110,
            120,
            130,
            150,
            170,
            200,
            230
        },
        new int[]
        {
            100,
            110,
            120,
            140,
            160,
            190,
            220,
            260,
            300
        },
        new int[]
        {
            180,
            190,
            200,
            220,
            240,
            270,
            300,
            340,
            380
        },
        new int[]
        {
            130,
            140,
            150,
            170,
            190,
            220,
            250,
            290,
            330
        },
        new int[]
        {
            80,
            85,
            90,
            100,
            110,
            130,
            150,
            180,
            210
        },
        new int[]
        {
            360,
            365,
            370,
            380,
            390,
            410,
            430,
            460,
            490
        },
        new int[]
        {
            360,
            365,
            370,
            380,
            390,
            410,
            430,
            460,
            490
        },
        new int[]
        {
            360,
            365,
            370,
            380,
            390,
            410,
            430,
            460,
            490
        },
        new int[]
        {
            360,
            365,
            370,
            380,
            390,
            410,
            430,
            460,
            490
        },
        new int[]
        {
            360,
            365,
            370,
            380,
            390,
            410,
            430,
            460,
            490
        },
        new int[]
        {
            30,
            50,
            60,
            90,
            120,
            180,
            240,
            300,
            360
        },
        new int[]
        {
            30,
            50,
            60,
            90,
            120,
            180,
            240,
            300,
            360
        },
        new int[]
        {
            30,
            50,
            60,
            90,
            120,
            180,
            240,
            300,
            360
        },
        new int[]
        {
            30,
            50,
            60,
            90,
            120,
            180,
            240,
            300,
            360
        },
        new int[]
        {
            30,
            50,
            60,
            90,
            120,
            180,
            240,
            300,
            360
        },
        new int[]
        {
            30,
            50,
            60,
            90,
            120,
            180,
            240,
            300,
            360
        },
        new int[]
        {
            30,
            50,
            60,
            90,
            120,
            180,
            240,
            300,
            360
        },
        new int[]
        {
            30,
            50,
            60,
            90,
            120,
            180,
            240,
            300,
            360
        }
        };
        dm_clrdm_clear_time_score_tbl = new uint[]
        {
        80000U,
        50000U,
        10000U,
        5000U,
        4000U,
        3000U,
        2000U,
        1000U,
        500U
        };
        dm_clrdm_spe_stg_get_act_no = new int[]
        {
        1,
        2,
        3,
        0,
        4,
        5,
        6,
        0,
        7,
        8,
        9,
        0,
        10,
        11,
        12,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0
        };
        gm_clrdm_mgr = new GMS_CLRDM_MGR();
        gm_clrdm_mgr_p = null;
        AMS_AMB_HEADER[] array497 = new AMS_AMB_HEADER[2];
        gm_clrdm_amb = array497;
        gm_clrdm_tex = New<AOS_TEXTURE>(2);
    }
}
