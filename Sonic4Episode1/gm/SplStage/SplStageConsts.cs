internal static partial class GmSplStage
{
    public const float GMD_SPL_STAGE_FADEIN_TIME = 30f;

    public const ushort GMD_SPL_STAGE_ROLL_SPD_INIT = 0x0100;
    public const ushort GMD_SPL_STAGE_ROLL_SPD_ACL = 0x0038;
    public const int GMD_SPL_STAGE_ROLL_FADEWAIT = 90;

    // the base light matches the main game; the second one is the special stage's own
    public const float GMD_SPL_STG_BASE_LIGHT_POS_X = -1f;
    public const float GMD_SPL_STG_BASE_LIGHT_POS_Y = -1f;
    public const float GMD_SPL_STG_BASE_LIGHT_POS_Z = -1f;
    public const float GMD_SPL_STG_OPT1_LIGHT_POS_X = -0.4f;
    public const float GMD_SPL_STG_OPT1_LIGHT_POS_Y = -0.4f;
    public const float GMD_SPL_STG_OPT1_LIGHT_POS_Z = -1f;

    public const int GMD_SPL_STG_SW_MAX = 16;

    public const uint GMD_SPL_STAGE_BOUNCE_HIT = 1;
    public const uint GMD_SPL_STAGE_NUDGE_HIT = 2;
    public const uint GMD_SPL_STAGE_RESULT = 4;

    public const int GMD_SS_RINGGATE_MAX = 9;

    // no gate configured at this slot, and the scan stops rather than skipping past it
    public const ushort GMD_SS_RINGGATE_NONE = 255;
}
