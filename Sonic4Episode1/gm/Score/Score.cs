using static AppMain;

// the floating "+100" popup, gmScore.cpp. No platform fork of any kind, so this is the C++ with its
// constants put back: the decompile inlined every one of them.
internal static partial class GmScore
{
    public static void GmScoreCreateScore(int score, int pos_x, int pos_y, int scale, int vib_level)
    {
        var score_dig = new int[GMD_SCORE_FIG_MAX];

        if (score <= 0)
            return;

        var obj_work = OBM_OBJECT_TASK_DETAIL_INIT(
            GMD_TASK_PRIO_SCORE, GMD_TASK_GROUP_SCORE, 0, 0,
            () => new GMS_SCORE_DISP_WORK(), null);

        var score_work = (GMS_SCORE_DISP_WORK)obj_work;

        obj_work.pos.x = pos_x;
        obj_work.pos.y = pos_y + GMD_SCORE_DEF_OFST_Y * FX32_ONE;
        obj_work.pos.z = GMD_SCORE_POS_Z;
        obj_work.flag |= 18U;
        obj_work.move_flag |= 256U;
        obj_work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmScoreMainFunc);

        score_work.vib_level = vib_level;
        score_work.base_pos.Assign(obj_work.pos);
        score_work.scale = scale;
        score_work.rise_dist = GMD_SCORE_DEF_RIZE_DIST * FX32_ONE
                               + GMD_SCORE_RIZE_DIST_SCALE * (scale - FX32_ONE);
        score_work.rise_spd = score_work.rise_dist * 2 / GMD_SCORE_RIZE_TIME;
        score_work.rise_dec = -score_work.rise_spd / GMD_SCORE_RIZE_TIME;
        score_work.timer = GMD_SCORE_DISP_TIME * FX32_ONE;

        // the clamp lands after rise and timer are set, which is what the C++ does
        if (score > GMD_SCORE_MAX)
            score = GMD_SCORE_MAX;

        var remainder = score;
        var enable = false;
        var score_num = 0;

        // leading zeros come out as -1 and stop the draw loop rather than printing
        for (int i = 0; i < GMD_SCORE_FIG_MAX; ++i)
        {
            var digit = GMD_SCORE_FIG_MAX - 1 - i;

            score_dig[digit] = remainder / gm_score_digit_divisor_tbl[i];
            remainder -= score_dig[digit] * gm_score_digit_divisor_tbl[i];

            if (!enable)
            {
                if (score_dig[digit] == 0)
                {
                    score_dig[digit] = -1;
                }
                else
                {
                    enable = true;
                    ++score_num;
                }
            }
            else
            {
                ++score_num;
            }
        }

        var ofst_x = ((score_num * GMD_SCORE_DEF_SIZE_X + (score_num - 1) * GMD_SCORE_DEF_INT_X)
                      * FX32_ONE >> 1)
                     - GMD_SCORE_DEF_SIZE_X * FX32_ONE / 2;
        var add_ofst_x = -GMD_SCORE_DEF_OFST_X * FX32_ONE;

        for (int i = 0; i < GMD_SCORE_FIG_MAX && score_dig[i] != -1; ++i, ofst_x += add_ofst_x)
        {
            var efct = GmEfctCmnEsCreate(obj_work, GME_EFCT_CMN_IDX_SCORE_0 + score_dig[i]);

            score_work.efct_work[i] = efct;
            efct.efct_com.obj_work.scale.x =
                efct.efct_com.obj_work.scale.y =
                    efct.efct_com.obj_work.scale.z = scale;
            efct.obj_3des.command_state = OBD_DRAW_CMD_STATE_3DFIX;

            GmComEfctSetDispOffset(efct, ofst_x, 0, 0);
        }
    }

    private static void gmScoreMainFunc(OBS_OBJECT_WORK obj_work)
    {
        var score_work = (GMS_SCORE_DISP_WORK)obj_work;

        score_work.base_pos.y += score_work.rise_spd;
        score_work.rise_spd += score_work.rise_dec;
        if (score_work.rise_spd > 0)
            score_work.rise_spd = 0;

        obj_work.pos.Assign(score_work.base_pos);

        // the shake stops with the rise, not with the timer
        if (score_work.rise_spd != 0)
        {
            score_work.vib_timer = ObjTimeCountUp(score_work.vib_timer);

            var step = score_work.vib_timer >> FX32_SHIFT & GMD_SCORE_VIB_TBL_MASK;
            var vib_scale = gm_score_vib_scale_tbl[score_work.vib_level];

            obj_work.pos.x += FX_Mul(gm_score_vib_tbl[step][0], vib_scale);
            obj_work.pos.y += FX_Mul(gm_score_vib_tbl[step][1], vib_scale);
        }

        score_work.timer = ObjTimeCountDown(score_work.timer);
        if (score_work.timer > 0)
            return;

        for (int i = 0; i < GMD_SCORE_FIG_MAX; ++i)
        {
            if (score_work.efct_work[i] != null)
                score_work.efct_work[i].efct_com.obj_work.flag |= 8U;
        }

        obj_work.flag |= 4U;
    }
}
