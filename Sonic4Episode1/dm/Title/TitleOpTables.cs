using static AppMain;

internal static partial class DmTitleOp
{
    public const int DMD_TITLEOP_DEBUG_TEST = 0;
    public const int DMD_TITLEOP_DEBUG_TEST_FOG = 0;
    public const int DMD_TITLEOP_DEBUG_TEST_CAMERA = 0;
    public const int DMD_TITLEOP_DEBUG_TEST_EXIT = 0;
    public const int DMD_TITLEOP_DEBUG_TEST_ROCK_EDIT = 0;
    public const int DMD_TITLEOP_TASK_PRIO_DATA_LOAD = 4096;
    public const int DMD_TITLEOP_TASK_GROUP_DATA_LOAD = 0;
    public const int DMD_TITLEOP_TASK_PRIO_DATA_BUILD = 4096;
    public const int DMD_TITLEOP_TASK_GROUP_DATA_BUILD = 0;
    public const int DMD_TITLEOP_TASK_PRIO_DATA_FLUSH = 4096;
    public const int DMD_TITLEOP_TASK_GROUP_DATA_FLUSH = 0;
    public const int DMD_TITLEOP_TASK_PRIO_DATA_RELEASE = 4096;
    public const int DMD_TITLEOP_TASK_GROUP_DATA_RELEASE = 0;
    public const int DMD_TITLEOP_TASK_PRIO_MGR = 12288;
    public const int DMD_TITLEOP_TASK_GROUP_MGR = 0;
    public const int DMD_TITLEOP_TASK_PRIO_CAMERA = 12288;
    public const int DMD_TITLEOP_TASK_GROUP_CAMERA = 0;
    public const int DMD_TITLEOP_TASK_PRIO_FAR_SKY_T = 24576;
    public const int DMD_TITLEOP_TASK_GROUP_FAR_SKY_T = 0;
    public const int DMD_TITLEOP_TASK_PRIO_FAR_SKY_B = 16384;
    public const int DMD_TITLEOP_TASK_GROUP_FAR_SKY_B = 0;
    public const int DMD_TITLEOP_TASK_PRIO_FAR_ROCK_T = 24832;
    public const int DMD_TITLEOP_TASK_GROUP_FAR_ROCK_T = 0;
    public const int DMD_TITLEOP_TASK_PRIO_FAR_ROCK_B = 16640;
    public const int DMD_TITLEOP_TASK_GROUP_FAR_ROCK_B = 0;
    public const int DMD_TITLEOP_TASK_PRIO_FAR_SEA = 20480;
    public const int DMD_TITLEOP_TASK_GROUP_FAR_SEA = 0;
    public const int DMD_TITLEOP_TASK_PAUSELEVEL_DEF = 0;
    public const int DMD_TITLEOP_OBJ_OBJPAUSELEVEL_DEF = 0;
    public const int DMD_TITLEOP_OBJTYPE_MAPFAR = 1;
    public const int DMD_TITLEOP_CAMERA_ID = 0;
    public const int DMD_TITLEOP_DRAW_CMD_STATE_PRE_MAPFAR = 0;
    public const int DMD_TITLEOP_DRAW_CMD_STATE_MAPFAR_B = 1;
    public const int DMD_TITLEOP_DRAW_CMD_STATE_MAPFAR_SEA = 2;
    public const int DMD_TITLEOP_DRAW_CMD_STATE_MAPFAR_T = 3;
    public const int DMD_TITLEOP_DRAW_CMD_STATE_POST_MAPFAR = 4;
    public const int DMD_TITLEOP_DRAW_CMD_STATE_3DNN = 5;
    public const int DMD_TITLEOP_ROCK_TYPE_NUM = 3;
    public const int DMD_TITLEOP_ROCK_SETTING_NUM = 6;
    public const int DMD_TITLEOP_ROCK_DISP_CLIP_DIST = 1179648;
    public const int DMD_TITLEOP_MAP_SCRL_SPD = -128;
    public const int DMD_TITLEOP_MAP_SCRL_LOOP_DIST = 3145728;
    public const int DMD_TITLEOP_ACT_LOGO_START_FRANE = 30;
    public const int DMD_TITLEOP_ACT_RIGHT_START_FRANE = 75;
    public const int DMD_TITLEOP_ACT_ALL_SET_END_FRAME = 105;
    public const int DMD_TITLEOP_ACT_FINGER_WAVE_S_FRAME = 60;
    public const int DMD_TITLEOP_ACT_FINGER_WAVE_E_FRAME = 84;
    public const int DMD_TITLEOP_DATA_FAR_AMB = 0;
    public const int DMD_TITLEOP_DATA_COM_AMA_SET_AMB = 1;
    public const int DMD_TITLEOP_DATA_MAX = 2;
    public const int DMD_TITLEOP_MAPFAR_DATA_MDL = 0;
    public const int DMD_TITLEOP_MAPFAR_DATA_TEX = 1;
    public const int DMD_TITLEOP_MAPFAR_DATA_MAT = 2;
    public const int DMD_TITLEOP_MAPFAR_DATA_MDL_WATER = 3;
    public const int DMD_TITLEOP_MAPFAR_DATA_MAX = 4;
    public const int DMD_TITLEOP_AMA_DATA_SET_AMA = 0;
    public const int DMD_TITLEOP_AMA_DATA_SET_TEX_AMB = 1;
    public const int DMD_TITLEOP_AMA_DATA_SET_MAX = 2;
    public const int DMD_TITLEOP_OBJWORK_SKY_T = 0;
    public const int DMD_TITLEOP_OBJWORK_ROCK_A_T = 1;
    public const int DMD_TITLEOP_OBJWORK_ROCK_B_T = 2;
    public const int DMD_TITLEOP_OBJWORK_ROCK_C_T = 3;
    public const int DMD_TITLEOP_OBJWORK_SEW = 4;
    public const int DMD_TITLEOP_OBJWORK_MAX = 5;
    public const int DMD_TITLEOP_AOSTEX_COM = 0;
    public const int DMD_TITLEOP_AOSTEX_MAX = 1;
    public const int DMD_TITLEOP_AOS_ACT_LOGO_1 = 0;
    public const int DMD_TITLEOP_AOS_ACT_FINGER = 1;
    public const int DMD_TITLEOP_AOS_ACT_LOGO_2 = 2;
    public const int DMD_TITLEOP_AOS_ACT_LOGO_3 = 3;
    public const int DMD_TITLEOP_AOS_ACT_RIGHT = 4;
    public const int DMD_TITLEOP_AOS_ACT_FREE = 5;
    public const int DMD_TITLEOP_AOS_ACT_FREE_RED = 6;
    public const int DMD_TITLEOP_AOS_ACT_MAX = 7;
    public const uint DMD_TITLEOP_FLAG_ACT_DISP_LOGO_1 = 1;
    public const uint DMD_TITLEOP_FLAG_ACT_DISP_FINGER = 2;
    public const uint DMD_TITLEOP_FLAG_ACT_DISP_LOGO_2 = 4;
    public const uint DMD_TITLEOP_FLAG_ACT_DISP_LOGO_3 = 8;
    public const uint DMD_TITLEOP_FLAG_ACT_DISP_RIGHT = 16;
    public const uint DMD_TITLEOP_FLAG_ACT_DISP_FREE = 32;
    public const uint DMD_TITLEOP_FLAG_ACT_DISP_FREE_RED = 64;
    public const uint DMD_TITLEOP_FLAG_ACT_USER_NODISP_LOGO_1 = 256;
    public const uint DMD_TITLEOP_FLAG_ACT_USER_NODISP_FINGER = 512;
    public const uint DMD_TITLEOP_FLAG_ACT_USER_NODISP_LOGO_2 = 1024;
    public const uint DMD_TITLEOP_FLAG_ACT_USER_NODISP_LOGO_3 = 2048;
    public const uint DMD_TITLEOP_FLAG_ACT_USER_NODISP_RIGHT = 4096;
    public const uint DMD_TITLEOP_FLAG_SCRL = 268435456;
    public const uint DMD_TITLEOP_FLAG_EXIT = 536870912;
    public const uint DMD_TITLEOP_FLAG_ALL_ACT_END = 1073741824;
    public const uint DMD_TITLEOP_FLAG_2DACT_READY = 2147483648;
    private const int dm_titleop_com_file_num = 2;
    private const int IDB_MAPFAR_ZONE1_MAT_Z1_SEA_D_ZNV = 1;

    internal static Reference<MTS_TASK_TCB> dm_titleop_load_tcb;
    internal static MTS_TASK_TCB dm_titleop_build_tcb;
    internal static MTS_TASK_TCB dm_titleop_flush_tcb;
    internal static MTS_TASK_TCB dm_titleop_mgr_tcb;
    internal static bool dm_titleop_build_state;
    internal static NNS_RGBA_U8 dm_titleop_clear_color;
    internal static DMS_TITLEOP_ROCK_SETTING[][] dm_titleop_rock_setting;
    internal static int[] dm_titleop_rock_setting_num;
    internal static AMS_FS[] dm_titleop_data;
    internal static object[] dm_titleop_mapfar_data;
    internal static AOS_TEXTURE[] dm_titleop_aos_tex;
    internal static OBS_ACTION3D_NN_WORK[] dm_titleop_obj_3d_list;
    internal static OBS_ACTION3D_NN_WORK[] dm_titleop_water_obj_3d_list;
    internal static int dm_titleop_scrl_x_ofst;
    internal static OBF_DRAW_USER_DT_FUNC _dmTitleOpPreDrawDT;
    internal static OBF_DRAW_USER_DT_FUNC _dmTitleOpFallShaderPreRenderUserFunc;

    internal static DMS_LOGO_COM_LOAD_FILE_INFO[] dm_titleop_com_fileinfo_list = new DMS_LOGO_COM_LOAD_FILE_INFO[2]
    {
        new DMS_LOGO_COM_LOAD_FILE_INFO("G_ZONE1/MAPFAR/ZONE1_MAPFAR.AMB", new post_func_Delegate(DmTitleOp.dmTitleOpLoadPostFuncMapFar)),
        new DMS_LOGO_COM_LOAD_FILE_INFO("DEMO/TITLE/D_TITLEOP.AMB", new post_func_Delegate(DmTitleOp.dmTitleOpLoadPostFuncTitleLogo))
    };

    // kept as a static constructor rather than field initialisers: the decompiler's block is one
    // 47-line run and reflowing a rock table by hand during a move is how a table gets transposed
    static DmTitleOp()
    {
        dm_titleop_load_tcb = new Reference<MTS_TASK_TCB>(null);
        dm_titleop_build_tcb = null;
        dm_titleop_flush_tcb = null;
        dm_titleop_mgr_tcb = null;
        dm_titleop_build_state = false;
        dm_titleop_clear_color = new NNS_RGBA_U8(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
        dm_titleop_rock_setting = new DMS_TITLEOP_ROCK_SETTING[][]
        {
        new DMS_TITLEOP_ROCK_SETTING[]
        {
            new DMS_TITLEOP_ROCK_SETTING(247808U, 0U, 323584U, 8704U, 8704U, 8704U),
            new DMS_TITLEOP_ROCK_SETTING(4294731776U, 0U, 4294823936U, 4096U, 4096U, 4096U),
            new DMS_TITLEOP_ROCK_SETTING(1822720U, 0U, 4294475776U, 21120U, 21120U, 21120U),
            new DMS_TITLEOP_ROCK_SETTING(501760U, 0U, 4294520832U, 7344U, 7344U, 7344U),
            new DMS_TITLEOP_ROCK_SETTING(4294492160U, 0U, 4294500352U, 17712U, 17712U, 17712U),
            new DMS_TITLEOP_ROCK_SETTING(0U, 0U, 0U, 4096U, 4096U, 4096U)
        },
        new DMS_TITLEOP_ROCK_SETTING[]
        {
            new DMS_TITLEOP_ROCK_SETTING(4294768640U, 0U, 305152U, 4720U, 4720U, 4720U),
            new DMS_TITLEOP_ROCK_SETTING(497664U, 0U, 249856U, 11216U, 11216U, 11216U),
            new DMS_TITLEOP_ROCK_SETTING(393216U, 0U, 4294072320U, 9715U, 9715U, 9715U),
            new DMS_TITLEOP_ROCK_SETTING(4294234112U, 0U, 133120U, 8752U, 8752U, 8752U),
            new DMS_TITLEOP_ROCK_SETTING(266240U, 0U, 405504U, 4032U, 4032U, 4032U),
            new DMS_TITLEOP_ROCK_SETTING(0U, 0U, 0U, 4096U, 4096U, 4096U)
        },
        new DMS_TITLEOP_ROCK_SETTING[]
        {
            new DMS_TITLEOP_ROCK_SETTING(223232U, 0U, 98304U, 4512U, 4512U, 4512U),
            new DMS_TITLEOP_ROCK_SETTING(4293341184U, 0U, 4294273024U, 17504U, 17504U, 17504U),
            new DMS_TITLEOP_ROCK_SETTING(923648U, 0U, 4294821888U, 19168U, 19168U, 19168U),
            new DMS_TITLEOP_ROCK_SETTING(4293591040U, 0U, 354304U, 8384U, 8384U, 8384U),
            new DMS_TITLEOP_ROCK_SETTING(4294359040U, 0U, 102400U, 5424U, 5424U, 5424U),
            new DMS_TITLEOP_ROCK_SETTING(649216U, 0U, 151552U, 4304U, 4304U, 4304U)
        }
        };
        dm_titleop_rock_setting_num = new int[]
        {
        6,
        5,
        6
        };
        dm_titleop_data = new AMS_FS[2];
        dm_titleop_mapfar_data = new object[4];
        dm_titleop_aos_tex = null;
        _dmTitleOpPreDrawDT = new OBF_DRAW_USER_DT_FUNC(DmTitleOp.dmTitleOpPreDrawDT);
        _dmTitleOpFallShaderPreRenderUserFunc = new OBF_DRAW_USER_DT_FUNC(DmTitleOp.dmTitleOpFallShaderPreRenderUserFunc);
    }
}
