using static AppMain;

internal static partial class GmPauseMenu
{
    // the C++ compiles a different numbering per platform: mobile has no RANKING or TROPHY, so its
    // BACK is 2 where console's is 4. GmPause and GmMain used to read the raw number, which would
    // have made "back to world map" behave like retry the moment the console menu shipped.
    // RANKING and TROPHY have no screen to reach in this port, see docs/pause-action-ids.md
    public enum GME_PMENU_RESULT
    {
        GME_PMENU_RESULT_RETRY = 0,
        GME_PMENU_RESULT_OPTION,
        GME_PMENU_RESULT_RANKING,
        GME_PMENU_RESULT_TROPHY,
        GME_PMENU_RESULT_BACK,
        GME_PMENU_RESULT_MAINMENU,
        GME_PMENU_RESULT_CANCEL,

        GME_PMENU_RESULT_NUM,
        GME_PMENU_RESULT_NONE,
    }

    public enum GME_PMENU_STATE
    {
        GMD_PMENU_STATE_LOADING = 0,
        GMD_PMENU_STATE_LOADED,
        GMD_PMENU_STATE_BUILDING,
        GMD_PMENU_STATE_BUILDED,
        GMD_PMENU_STATE_EXECUTE,
        GMD_PMENU_STATE_FLUSHING,
        GMD_PMENU_STATE_RELEASED,
    }

    // the four section markers are how gmPmProcBegin and gmPmUtilSetTexture decide which of the
    // four archives an action index belongs to, so the ordering is load-bearing
    public enum GME_PMENU_ACT
    {
        ACT_TITLE_BASE = 0,

        ACT_BTN_BASE1,
        ACT_BTN_BASE1_S,
        ACT_BTN_BASE2,
        ACT_BTN_BASE2_S,
        ACT_BTN_BASE3,
        ACT_BTN_BASE3_S,
        ACT_BTN_BASE4,
        ACT_BTN_BASE4_S,
        ACT_BTN_BASE5,
        ACT_BTN_BASE5_S,
        ACT_BTN_BASE6,
        ACT_BTN_BASE6_S,

        ACT_BTN_BACK,

        ACT_L_TITLE,
        ACT_L_BTN_BACK,
        ACT_L_DLG_YES,
        ACT_L_DLG_YES_S,
        ACT_L_DLG_NO,
        ACT_L_DLG_NO_S,
        ACT_L_MSG_RETRY,
        ACT_L_MSG_WOLRD,
        ACT_L_MSG_MMENU,

        ACT_P_BACK_MARK_O,
        ACT_P_BACK_MARK_X,

        ACT_PL_BTN_RETRY,
        ACT_PL_BTN_RETRY_S,
        ACT_PL_BTN_OPTION,
        ACT_PL_BTN_OPTION_S,
        ACT_PL_BTN_RANK,
        ACT_PL_BTN_RANK_S,
        ACT_PL_BTN_TROPHY,
        ACT_PL_BTN_TROPHY_S,
        ACT_PL_BTN_RESUME,
        ACT_PL_BTN_RESUME_S,
        ACT_PL_BTN_WORLD,
        ACT_PL_BTN_WORLD_S,

        ACT_NUM,

        ACT_CMN_S = ACT_TITLE_BASE,
        ACT_LNG_S = ACT_L_TITLE,
        ACT_PLF_S = ACT_P_BACK_MARK_O,
        ACT_PLNG_S = ACT_PL_BTN_RETRY,
    }

    public enum GME_PMENU_SE
    {
        GMD_PMENU_SE_CURSOR = 0,
        GMD_PMENU_SE_DECIDE,
        GMD_PMENU_SE_CANCEL,
        GMD_PMENU_SE_START,
    }

    internal sealed class GMS_PMENU_GLOBAL
    {
        public GME_PMENU_STATE state = GME_PMENU_STATE.GMD_PMENU_STATE_RELEASED;
        public AMS_TCB tcb;
        public GME_PMENU_RESULT result = GME_PMENU_RESULT.GME_PMENU_RESULT_NONE;

        public readonly AOS_TEXTURE tex_cmn = new AOS_TEXTURE();
        public readonly AOS_TEXTURE tex_lng = new AOS_TEXTURE();
        public readonly AOS_TEXTURE tex_plf = new AOS_TEXTURE();
        public readonly AOS_TEXTURE tex_plng = new AOS_TEXTURE();
        public readonly AOS_TEXTURE tex_win = new AOS_TEXTURE();

        public A2S_AMA_HEADER file_cmn_ama;
        public AMS_AMB_HEADER file_cmn_amb;
        public A2S_AMA_HEADER file_lng_ama;
        public AMS_AMB_HEADER file_lng_amb;
        public A2S_AMA_HEADER file_plf_ama;
        public AMS_AMB_HEADER file_plf_amb;
        public A2S_AMA_HEADER file_plng_ama;
        public AMS_AMB_HEADER file_plng_amb;
        public AMS_AMB_HEADER file_win_amb;

        public AMS_FS fs;
        public GSS_SND_SE_HANDLE se_handle;
    }

    internal sealed class GMS_PMENU_WORK
    {
        public int select;
        public int select_sub;
        public GME_PMENU_RESULT result = GME_PMENU_RESULT.GME_PMENU_RESULT_NONE;
        public readonly AOS_ACTION[] act = new AOS_ACTION[(int)GME_PMENU_ACT.ACT_NUM];
    }

    internal delegate void GMS_PMENU_PROC(GMS_PMENU_WORK work);

    internal sealed class GMS_PMENU_TASK_WORK
    {
        public GMS_PMENU_WORK work;
        public uint proc_count;
        public GMS_PMENU_PROC proc;
    }
}
