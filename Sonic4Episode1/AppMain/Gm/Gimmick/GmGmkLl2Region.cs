internal partial class AppMain
{
    internal const int GMD_GMK_LL2_LIGHT_THRESHOLD = 16;

    private const uint GMD_GMK_LL2_REGION_LIT = 1U;
    private const uint GMD_GMK_LL2_REGION_ALWAYS = 2U;
    private const int GMD_GMK_LL2_REGION_RAMP_MAX = 32;
    private const int GMD_GMK_LL2_REGION_UNIT = 176;

    // ev 312, an invisible patch of light in a dark room. 20 of the 32 in ZONE32 are pinned on
    // with rect_x 0xFF; the rest fade in and out with their region's candle
    private static OBS_OBJECT_WORK GmGmkLl2RegionInit(
      GMS_EVE_RECORD_EVENT eve_rec,
      int pos_x,
      int pos_y,
      byte type)
    {
        OBS_OBJECT_WORK work = GMM_ENEMY_CREATE_WORK(eve_rec, pos_x, pos_y, () => new GMS_GMK_LL2_REGION_WORK(), "GMK_LL2_REGION");
        GMS_GMK_LL2_REGION_WORK region_work = (GMS_GMK_LL2_REGION_WORK)work;
        region_work.region = MTM_MATH_CLIP((sbyte)eve_rec.left, 0, 64);
        region_work.size_x = eve_rec.width << 6;
        region_work.size_y = eve_rec.height << 6;
        region_work.state = (byte)eve_rec.left == byte.MaxValue ? GMD_GMK_LL2_REGION_ALWAYS : 0U;
        region_work.ramp = 0;
        work.disp_flag |= OBD_DISP_NODISP;
        work.flag |= 16U;
        work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkLl2RegionState);
        work.ppOut = new MPP_VOID_OBS_OBJECT_WORK(gmGmkLl2RegionLightDraw);
        return work;
    }

    private static void gmGmkLl2RegionState(OBS_OBJECT_WORK obj_work)
    {
        GMS_GMK_LL2_REGION_WORK region_work = (GMS_GMK_LL2_REGION_WORK)obj_work;
        GMS_PLAYER_WORK ply_work = g_gm_main_system.ply_work[GSD_MAIN_PLAYER_1P];

        // a shipped cleanup for the back half of the stage, not a general rule
        if (ply_work != null && obj_work.pos.x <= 22000 << 12 && ply_work.obj_work.pos.x >= 23000 << 12)
        {
            obj_work.move_flag |= 8U;
            return;
        }

        if ((region_work.state & GMD_GMK_LL2_REGION_ALWAYS) != 0U)
        {
            region_work.state |= GMD_GMK_LL2_REGION_LIT;
            region_work.ramp = GMD_GMK_LL2_REGION_RAMP_MAX;
            return;
        }

        if (gm_gmk_switch_state[region_work.region].counter >= GMD_GMK_LL2_LIGHT_THRESHOLD)
            region_work.state |= GMD_GMK_LL2_REGION_LIT;
        else
            region_work.state &= ~GMD_GMK_LL2_REGION_LIT;

        if ((region_work.state & GMD_GMK_LL2_REGION_LIT) != 0U)
        {
            if (region_work.ramp < GMD_GMK_LL2_REGION_RAMP_MAX)
                ++region_work.ramp;
        }
        else if (region_work.ramp > 0)
            --region_work.ramp;
    }

    private static void gmGmkLl2RegionLightDraw(OBS_OBJECT_WORK obj_work)
    {
        GMS_GMK_LL2_REGION_WORK region_work = (GMS_GMK_LL2_REGION_WORK)obj_work;
        if ((region_work.state & GMD_GMK_LL2_REGION_LIT) == 0U && region_work.ramp == 0)
            return;

        if (!gmGmkLl2RegionIsNearPlayer(obj_work, region_work))
            return;

        float x = (obj_work.pos.x >> 12) + region_work.size_x * 0.5f;
        float y = -((obj_work.pos.y >> 12) + region_work.size_y * 0.5f);
        GmDarkLightSpriteDraw(
            x, y, 256f, OBD_DRAW_CMD_STATE_LIGHTMAP,
            region_work.size_x / (float)GMD_GMK_LL2_REGION_UNIT,
            region_work.size_y / (float)GMD_GMK_LL2_REGION_UNIT,
            (byte)(255 * region_work.ramp >> 5));
    }

    // the player stands in for the camera here, with half a screen of margin in x and a whole one
    // in y. Dying bypasses the test, which is how the light survives the death camera
    private static bool gmGmkLl2RegionIsNearPlayer(OBS_OBJECT_WORK obj_work, GMS_GMK_LL2_REGION_WORK region_work)
    {
        GMS_PLAYER_WORK ply_work = g_gm_main_system.ply_work[GSD_MAIN_PLAYER_1P];
        if (ply_work == null)
            return false;
        if ((ply_work.player_flag & GMD_PLF_DIE) != 0U)
            return true;

        int margin_x = (GMD_OBJ_LCD_X / 2 + 64) << 12;
        int margin_y = (GMD_OBJ_LCD_Y + 64) << 12;
        int ply_x = ply_work.obj_work.pos.x;
        int ply_y = ply_work.obj_work.pos.y;

        return ply_x + margin_x >= obj_work.pos.x
            && ply_x - margin_x <= obj_work.pos.x + (region_work.size_x << 12)
            && ply_y + margin_y >= obj_work.pos.y
            && ply_y - margin_y <= obj_work.pos.y + (region_work.size_y << 12);
    }
}
