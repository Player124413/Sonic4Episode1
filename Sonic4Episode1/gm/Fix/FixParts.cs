using static AppMain;

using static GmFix.GME_FIX_PART_TYPE;
using static GmFix.GME_FIX_PLAN;
using static GmFix.GME_FIX_TEX_IDX;
using static GmFix.GME_FIX_VIRTUAL_PAD_SUBPART;

internal static partial class GmFix
{
    // ---- ring counter

    private static void gmFixRingCountPartInit(GMS_FIX_MGR_WORK mgr_work)
    {
        var part = mgr_work.part_ringcount;

        gmFixRegisterPart(mgr_work, part, GME_FIX_PART_TYPE_RINGCOUNT);
        part.proc_update = gmFixRingCountPartProcUpdateMain;
        part.proc_disp = gmFixRingCountPartProcDispMain;

        for (int i = 0; i < part.sub_parts.Length; i++)
        {
            var subpart = gmFixCreateSubpart(
                "FIX_RING", gm_fix_ringcount_act_id_tbl[i], gmFixSubpartOutFunc);

            if (i != 0)
                ((OBS_OBJECT_WORK)subpart).pos.y += FX_F32_TO_FX32(5f);

            part.sub_parts[i] = subpart;
        }
    }

    private static void gmFixRingCountPartProcUpdateMain(GMS_FIX_PART_WORK part_work)
    {
        var part = (GMS_FIX_PART_RINGCOUNT)part_work;

        gmFixRingCountPartUpdateDigitList(part);

        if (gmFixGetRingNum() != 0)
        {
            part.flag &= ~GMD_FIX_PART_FLAG_BLINK;
            return;
        }

        if ((part.flag & GMD_FIX_PART_FLAG_BLINK) != 0)
            return;

        part.flag |= GMD_FIX_PART_FLAG_BLINK;
        gmFixInitBlink(
            part,
            GMD_FIX_PART_RINGCOUNT_DIGIT_BLINK_ON_TIME,
            GMD_FIX_PART_RINGCOUNT_DIGIT_BLINK_OFF_TIME);
    }

    private static void gmFixRingCountPartProcDispMain(GMS_FIX_PART_WORK part_work)
    {
        var part = (GMS_FIX_PART_RINGCOUNT)part_work;

        gmFixUpdateBlinkFlag(part);
        gmFixRingCountPartSetDispDigits(part, (part.flag & GMD_FIX_PART_FLAG_BLINK_OFF) == 0);
        gmFixRingCountPartUpdateActionDigitsType(part);
    }

    private static void gmFixUpdateBlinkFlag(GMS_FIX_PART_WORK part_work)
    {
        if ((part_work.flag & GMD_FIX_PART_FLAG_BLINK) != 0 && !gmFixUpdateBlink(part_work))
            part_work.flag |= GMD_FIX_PART_FLAG_BLINK_OFF;
        else
            part_work.flag &= ~GMD_FIX_PART_FLAG_BLINK_OFF;
    }

    private static void gmFixRingCountPartUpdateDigitList(GMS_FIX_PART_RINGCOUNT part)
    {
        int ring_num = gmFixGetRingNum();
        AkUtilNumValueToDigits(ring_num, part.digit_list, GMD_FIX_PART_RINGCOUNT_DIGIT_NUM);

        if (ring_num != 0)
            return;

        for (int i = 0; i < GMD_FIX_PART_RINGCOUNT_DIGIT_NUM; i++)
            part.digit_list[i] = GMD_FIX_DIGIT_BLANK;
    }

    private static void gmFixRingCountPartUpdateActionDigitsType(GMS_FIX_PART_RINGCOUNT part)
    {
        for (int i = 0; i < GMD_FIX_PART_RINGCOUNT_DIGIT_NUM; i++)
        {
            gmFixSetFrameStatic(
                (OBS_OBJECT_WORK)part.sub_parts[gm_fix_part_ring_count_digit_subpart_idx_tbl[i]],
                gm_fix_part_ring_count_digit_type_frame_tbl[part.digit_list[i]]);
        }
    }

    private static void gmFixRingCountPartSetDispDigits(GMS_FIX_PART_RINGCOUNT part, bool enable)
    {
        gmFixSetSubpartsDisp(part.sub_parts, enable);
    }

    // subpart 0 is the icon and always shows; only the digits blink
    private static void gmFixSetSubpartsDisp(GMS_COCKPIT_2D_WORK[] sub_parts, bool enable)
    {
        for (int i = 1; i < sub_parts.Length; i++)
        {
            var obj_work = (OBS_OBJECT_WORK)sub_parts[i];

            if (enable)
                obj_work.disp_flag &= ~OBD_DISP_NODISP;
            else
                obj_work.disp_flag |= OBD_DISP_NODISP;
        }
    }

    // ---- score

    private static void gmFixScorePartInit(GMS_FIX_MGR_WORK mgr_work)
    {
        var part = mgr_work.part_score;

        gmFixRegisterPart(mgr_work, part, GME_FIX_PART_TYPE_SCORE);
        part.proc_update = gmFixScorePartProcUpdateMain;
        part.proc_disp = gmFixScorePartProcDispMain;

        var stage22 = gmFixIsStage22();
        var act_id_tbl = stage22 ? gmFixScoreStage22ActIdTbl() : gm_fix_score_act_id_tbl;

        // console-only: on a 4:3 screen the act 2-2 score shifts left to clear the timer
        var pos_ofst_x = 0;
        if (stage22
            && gm_fix_variant.score_narrowscreen_offset
            && !gmFixIsTimeAttack()
            && !_am_draw_video.wide_screen)
        {
            pos_ofst_x = GMD_FIX_PART_SCORE_S22_TA_NARROWSCREEN_ADJUST_OFST_X;
        }

        for (int i = 0; i < part.sub_parts.Length; i++)
        {
            // mobile leaves subpart 0 out on act 2-2; console puts the score icon there
            if (act_id_tbl[i] < 0)
            {
                part.sub_parts[i] = null;
                continue;
            }

            var subpart = gmFixCreateSubpart("FIX_SCORE", act_id_tbl[i], gmFixSubpartOutFunc);
            var obj_work = (OBS_OBJECT_WORK)subpart;

            obj_work.pos.x += pos_ofst_x;

            if (i != 0)
                obj_work.pos.y += FX_F32_TO_FX32(2f);

            part.sub_parts[i] = subpart;
        }
    }

    private static int[] gmFixScoreStage22ActIdTbl()
    {
        return gm_fix_variant.score_stage22_act_id_tbl
            ?? gm_fix_score_stage22_act_id_tbl[(int)gmFixGetPlan()];
    }

    private static void gmFixScorePartProcUpdateMain(GMS_FIX_PART_WORK part_work)
    {
        var part = (GMS_FIX_PART_SCORE)part_work;
        AkUtilNumValueToDigits((int)gmFixGetScore(), part.digit_list, GMD_FIX_PART_SCORE_DIGIT_NUM);
    }

    private static void gmFixScorePartProcDispMain(GMS_FIX_PART_WORK part_work)
    {
        var part = (GMS_FIX_PART_SCORE)part_work;

        for (int i = 0; i < GMD_FIX_PART_SCORE_DIGIT_NUM; i++)
        {
            gmFixSetFrameStatic(
                (OBS_OBJECT_WORK)part.sub_parts[gm_fix_part_score_digit_subpart_idx_tbl[i]],
                gm_fix_part_common_digit_type_frame_tbl[part.digit_list[i]]);
        }
    }

    // ---- timer

    private static void gmFixTimerPartInit(GMS_FIX_MGR_WORK mgr_work)
    {
        var part = mgr_work.part_timer;

        gmFixRegisterPart(mgr_work, part, GME_FIX_PART_TYPE_TIMER);
        part.proc_update = gmFixTimerPartProcUpdateMain;
        part.proc_disp = gmFixTimerPartProcDispMain;

        var time_attack = gmFixIsTimeAttack();
        var act_id_tbl = time_attack
            ? gm_fix_variant.timer_timeattack_act_id_tbl
            : gm_fix_timer_act_id_tbl;

        if (time_attack)
            part.timer_flag |= GMD_FIX_PART_TIMER_FLAG_RED_BY_TEX_FRAME;

        for (int i = 0; i < part.sub_parts.Length; i++)
        {
            var subpart = gmFixCreateSubpart("FIX_TIMER", act_id_tbl[i], gmFixSubpartOutFunc);
            var obj_work = (OBS_OBJECT_WORK)subpart;

            // mobile-only: console leaves the time attack counter where the normal one sits
            if (time_attack && gm_fix_variant.timer_timeattack_x_offset)
                obj_work.pos.x += FX_F32_TO_FX32(-98f);

            if (i != 0)
                obj_work.pos.y += FX_F32_TO_FX32(5f);

            part.sub_parts[i] = subpart;
        }
    }

    // the special stage draws its timer out of SPST instead, with the "TIME" label per language
    private static void gmFixTimerSSPartInit(GMS_FIX_MGR_WORK mgr_work)
    {
        var part = mgr_work.part_timer;

        gmFixRegisterPart(mgr_work, part, GME_FIX_PART_TYPE_TIMER);
        part.proc_update = gmFixTimerPartProcUpdateMain;
        part.proc_disp = gmFixTimerPartProcDispMain;
        part.timer_flag |= GMD_FIX_PART_TIMER_FLAG_RED_BY_TEX_FRAME;

        for (int i = 0; i < part.sub_parts.Length; i++)
        {
            var slot = i == 0
                ? (int)GME_FIX_TEX_IDX_SSFIX_LANG
                : (int)GME_FIX_TEX_IDX_SSFIX_CMN;

            part.sub_parts[i] = gmFixCreateSubpart(
                "FIX_TIMER_SS", gm_fix_timer_ss_act_id_tbl[i], gmFixSubpartOutFunc, slot, slot);
        }
    }

    private static void gmFixTimerPartProcUpdateMain(GMS_FIX_PART_WORK part_work)
    {
        var part = (GMS_FIX_PART_TIMER)part_work;

        gmFixTimerPartUpdateDigitList(part);

        if (gmFixProcessRequest(part.parent_mgr, GMD_FIX_MGR_REQUEST_FLAG_TIMER_FLASH_ACT))
            gmFixTimerPartInitFlashAction(part);
        else
            gmFixTimerPartUpdateFlashAction(part);

        if (!gmFixTimerPartIsTimeRunningOut(part.parent_mgr))
        {
            part.flag &= ~GMD_FIX_PART_FLAG_BLINK;
            return;
        }

        ushort min = 0;
        ushort sec = 0;
        ushort msec = 0;
        AkUtilFrame60ToTime(gmFixGetGameTime(), ref min, ref sec, ref msec);

        if ((part.flag & GMD_FIX_PART_FLAG_BLINK) == 0)
        {
            part.flag |= GMD_FIX_PART_FLAG_BLINK;
            gmFixInitBlink(
                part,
                GMD_FIX_PART_TIMER_DIGIT_BLINK_ON_TIME,
                GMD_FIX_PART_TIMER_DIGIT_BLINK_OFF_TIME);

            part.cur_sec = (ushort)(sec - 1);
        }

        if (part.cur_sec == sec)
            return;

        part.cur_sec = sec;
        GmSoundPlaySE("Countdown");
    }

    private static void gmFixTimerPartProcDispMain(GMS_FIX_PART_WORK part_work)
    {
        var part = (GMS_FIX_PART_TIMER)part_work;

        gmFixTimerPartSetDigitsRed(part, (part.flag & GMD_FIX_PART_FLAG_BLINK) != 0);
        gmFixUpdateBlinkFlag(part);
        gmFixSetSubpartsDisp(part.sub_parts, (part.flag & GMD_FIX_PART_FLAG_BLINK_OFF) == 0);
        gmFixTimerPartUpdateActionDigitsType(part);
    }

    private static void gmFixTimerPartUpdateDigitList(GMS_FIX_PART_TIMER part)
    {
        ushort min = 0;
        ushort sec = 0;
        ushort msec = 0;
        AkUtilFrame60ToTime(gmFixGetGameTime(), ref min, ref sec, ref msec);

        var value = new[] { (int)msec, sec, min };
        var offset = 0;

        for (int i = 0; i < gm_fix_timer_digit_num_tbl.Length; i++)
        {
            AkUtilNumValueToDigits(
                value[i],
                new ArrayPointer<int>(part.digit_list, offset),
                gm_fix_timer_digit_num_tbl[i]);

            offset += gm_fix_timer_digit_num_tbl[i];
        }
    }

    private static void gmFixTimerPartUpdateActionDigitsType(GMS_FIX_PART_TIMER part)
    {
        for (int i = 0; i < GMD_FIX_PART_TIMER_ALL_DIGIT_NUM; i++)
        {
            gmFixSetFrameStatic(
                (OBS_OBJECT_WORK)part.sub_parts[gm_fix_part_timer_digit_subpart_idx_tbl[i]],
                gm_fix_part_common_digit_type_frame_tbl[part.digit_list[i]] + part.digit_frame_ofst);
        }

        for (int i = 0; i < GMD_FIX_PART_TIMER_DECO_CHAR_NUM; i++)
        {
            gmFixSetFrameStatic(
                (OBS_OBJECT_WORK)part.sub_parts[gm_fix_part_timer_deco_char_subpart_idx_tbl[i]],
                part.deco_char_frame_ofst);
        }
    }

    // the special stage's digits are their own red texture frames; everywhere else they tint
    private static void gmFixTimerPartSetDigitsRed(GMS_FIX_PART_TIMER part, bool enable)
    {
        if ((part.timer_flag & GMD_FIX_PART_TIMER_FLAG_RED_BY_TEX_FRAME) != 0)
        {
            part.digit_frame_ofst = enable ? GMD_FIX_PART_TIMER_DIGIT_RED_FRAME_OFST : 0.0f;
            part.deco_char_frame_ofst = enable ? GMD_FIX_PART_TIMER_DECO_CHAR_RED_FRAME_OFST : 0.0f;
            return;
        }

        for (int i = 1; i < part.sub_parts.Length; i++)
        {
            ((OBS_OBJECT_WORK)part.sub_parts[i]).obj_2d.color.c =
                enable ? 0xFF0000FFU : uint.MaxValue;
        }
    }

    private static bool gmFixTimerPartIsTimeRunningOut(GMS_FIX_MGR_WORK mgr_work)
    {
        ushort min = 0;
        ushort sec = 0;
        AkUtilFrame60ToTime(gmFixGetGameTime(), ref min, ref sec);

        // the special stage counts down, the main game counts up to a ten minute limit
        return (mgr_work.flag & GMD_FIX_MGR_FLAG_TIMER_COUNTDOWN) != 0
            ? min <= 0 && sec < GMD_FIX_PART_TIMER_DIGIT_BLINK_REMAIN_SEC
            : min >= 9 && sec + GMD_FIX_PART_TIMER_DIGIT_BLINK_REMAIN_SEC >= 60U;
    }

    private static void gmFixTimerPartInitFlashAction(GMS_FIX_PART_TIMER part)
    {
        part.timer_flag |= GMD_FIX_PART_TIMER_FLAG_FLASH_ACT_ACTIVE;
        part.fade_ratio = 0.0f;
        part.scale_ratio = 0.0f;
        part.flash_act_phase = 0U;
    }

    private static void gmFixTimerPartUpdateFlashAction(GMS_FIX_PART_TIMER part)
    {
        if ((part.timer_flag & GMD_FIX_PART_TIMER_FLAG_FLASH_ACT_ACTIVE) == 0)
            return;

        var finished = false;

        switch (part.flash_act_phase)
        {
            case 0:
                part.fade_ratio += 0.5f;
                part.scale_ratio += 0.25f;
                part.fade_ratio = MTM_MATH_CLIP(part.fade_ratio, 0.0f, 1f);

                // SEGA clips fade_ratio into scale_ratio here where phase 1 clips its own, so the
                // grow phase tracks the fade's 0.5/frame rather than its own 0.25.
                // see docs/gmfix-port-notes.md
                part.scale_ratio = MTM_MATH_CLIP(part.fade_ratio, 0.0f, 1f);

                if (part.scale_ratio >= 1.0f)
                    part.flash_act_phase++;
                break;

            case 1:
                part.fade_ratio -= 0.02f;
                part.scale_ratio -= 0.05f;
                part.fade_ratio = MTM_MATH_CLIP(part.fade_ratio, 0.0f, 1f);
                part.scale_ratio = MTM_MATH_CLIP(part.scale_ratio, 0.0f, 1f);

                if (part.fade_ratio <= 0.0f)
                    part.flash_act_phase++;
                break;

            default:
                finished = true;
                break;
        }

        if (finished)
        {
            part.fade_ratio = 0.0f;
            part.scale_ratio = 0.0f;
            part.flash_act_phase = 0U;

            for (int i = 1; i < part.sub_parts.Length; i++)
            {
                var obj_work = (OBS_OBJECT_WORK)part.sub_parts[i];
                obj_work.scale.x = obj_work.scale.y = GMD_FIX_PART_TIMER_FLASH_ACT_SCALE_DEF;
                obj_work.obj_2d.fade.a = obj_work.obj_2d.fade.b =
                    obj_work.obj_2d.fade.g = obj_work.obj_2d.fade.r = 0;
            }

            part.timer_flag &= ~GMD_FIX_PART_TIMER_FLAG_FLASH_ACT_ACTIVE;
            return;
        }

        var scale = GMD_FIX_PART_TIMER_FLASH_ACT_SCALE_DEF + (int)(part.scale_ratio * 2048.0f);
        var alpha = (byte)MTM_MATH_CLIP(byte.MaxValue * part.fade_ratio, 0.0f, byte.MaxValue);

        for (int i = 1; i < part.sub_parts.Length; i++)
        {
            var obj_work = (OBS_OBJECT_WORK)part.sub_parts[i];
            obj_work.scale.x = obj_work.scale.y = scale;
            obj_work.obj_2d.fade.b = obj_work.obj_2d.fade.g =
                obj_work.obj_2d.fade.r = byte.MaxValue;
            obj_work.obj_2d.fade.a = alpha;
        }
    }

    // ---- challenge (the life counter)

    private static void gmFixChallengePartInit(GMS_FIX_MGR_WORK mgr_work)
    {
        var part = mgr_work.part_challenge;

        gmFixRegisterPart(mgr_work, part, GME_FIX_PART_TYPE_CHALLENGE);
        part.proc_update = gmFixChallengePartProcUpdateMain;
        part.proc_disp = gmFixChallengePartProcDispMain;

        var act_id_tbl = gm_fix_variant.challenge_act_id_tbl
            ?? gm_fix_challenge_act_id_tbl[(int)gmFixGetPlan()];

        for (int i = 0; i < part.sub_parts.Length; i++)
            part.sub_parts[i] = gmFixCreateSubpart("FIX_CHALLENGE", act_id_tbl[i], gmFixSubpartOutFunc);
    }

    private static void gmFixChallengePartProcUpdateMain(GMS_FIX_PART_WORK part_work)
    {
        var part = (GMS_FIX_PART_CHALLENGE)part_work;
        var ply_work = g_gm_main_system.ply_work[0];

        var frame = ply_work != null && (ply_work.player_flag & GMD_PLF_SUPER_SONIC) != 0
            ? 1f
            : 0.0f;

        gmFixSetFrameStatic((OBS_OBJECT_WORK)part.sub_parts[0], frame);

        AkUtilNumValueToDigits(
            (int)gmFixGetChallengeNum(), part.digit_list, GMD_FIX_PART_CHALLENGE_DIGIT_NUM);
    }

    private static void gmFixChallengePartProcDispMain(GMS_FIX_PART_WORK part_work)
    {
        var part = (GMS_FIX_PART_CHALLENGE)part_work;

        for (int i = 0; i < GMD_FIX_PART_CHALLENGE_DIGIT_NUM; i++)
        {
            gmFixSetFrameStatic(
                (OBS_OBJECT_WORK)part.sub_parts[gm_fix_part_challenge_digit_subpart_idx_tbl[i]],
                gm_fix_part_common_digit_type_frame_tbl[part.digit_list[i]]);
        }
    }

    // ---- virtual pad, the on-screen controls. mobile only: every action it names is past the end
    // of the console G_FIX

    private static void gmFixVirtualPadPartInit(GMS_FIX_MGR_WORK mgr_work)
    {
        if (!gm_fix_variant.has_virtual_pad)
            return;

        if (!CPadVirtualPad.CreateInstance().IsValid())
            return;

        var out_func = new[]
        {
            gmFixVirtualPadOutFunc(GMD_FIX_MGR_FLAG_HIDE_VIRTUAL_PAD_PART_SUPER_SONIC),
            gmFixVirtualPadOutFunc(GMD_FIX_MGR_FLAG_HIDE_VIRTUAL_PAD_PART_PAUSE),
            gmFixVirtualPadOutFunc(GMD_FIX_MGR_FLAG_HIDE_VIRTUAL_PAD_PART_ACTION),
            gmFixVirtualPadOutFunc(GMD_FIX_MGR_FLAG_HIDE_VIRTUAL_PAD_PART_MOVE_PAD),
        };

        var part = mgr_work.part_virtual_pad;

        gmFixRegisterPart(mgr_work, part, GME_FIX_PART_TYPE_VIRTUAL_PAD);
        part.proc_update = gmFixVirtualPadPartProcUpdateMain;
        part.proc_disp = gmFixVirtualPadPartProcDispMain;

        var act_id_tbl = gm_fix_virtual_pad_act_id_tbl[(int)gmFixGetPlan()];

        for (int i = 0; i < part.sub_parts.Length; i++)
        {
            if (act_id_tbl[i] < 0)
            {
                part.sub_parts[i] = null;
                continue;
            }

            var subpart = gmFixCreateSubpart("FIX_VIRTUAL_PAD", act_id_tbl[i], out_func[i]);
            var obj_work = (OBS_OBJECT_WORK)subpart;

            obj_work.disp_flag &= ~OBD_DISP_NODISP;

            // the pause button moves clear of the special stage's and time attack's wider HUD
            if (i == (int)GME_FIX_VIRTUAL_PAD_SUBPART_PAUSE)
            {
                if (gmFixIsSpecialStage())
                    obj_work.pos.x += FX_F32_TO_FX32(400f);
                else if (gmFixIsTimeAttack())
                    obj_work.pos.x += FX_F32_TO_FX32(200f);
            }

            part.sub_parts[i] = subpart;
        }

        // IT and SP have their own pause glyphs further along the strip
        var lang = GsEnvGetLanguage();
        var italian_or_spanish = lang == GSD_LANGUAGE_IT || lang == GSD_LANGUAGE_SP;

        part.pause_icon_frame[0] = italian_or_spanish ? 2f : 0.0f;
        part.pause_icon_frame[1] = italian_or_spanish ? 3f : 1f;

        part.flag &= ~GMD_FIX_PART_FLAG_NODISP;
    }

    private static void gmFixVirtualPadPartProcUpdateMain(GMS_FIX_PART_WORK part_work)
    {
        var part = (GMS_FIX_PART_VIRTUAL_PAD)part_work;

        var ssonic = (OBS_OBJECT_WORK)part.sub_parts[(int)GME_FIX_VIRTUAL_PAD_SUBPART_SSONIC];
        if (GmPlayerIsTransformSuperSonic(g_gm_main_system.ply_work[0]))
            ssonic.disp_flag &= ~OBD_DISP_NODISP;
        else
            ssonic.disp_flag |= OBD_DISP_NODISP;

        var pause = (OBS_OBJECT_WORK)part.sub_parts[(int)GME_FIX_VIRTUAL_PAD_SUBPART_PAUSE];
        pause.disp_flag &= ~OBD_DISP_NODISP;
        gmFixSetFrameStatic(
            pause, part.pause_icon_frame[gmFixVirtualPadPartIsOnPauseIcon() ? 1 : 0]);

        var plan = gmFixGetPlan();

        if (plan == GME_FIX_PLAN_VIRTUAL_PAD)
        {
            gmFixSetFrameStatic(
                (OBS_OBJECT_WORK)part.sub_parts[(int)GME_FIX_VIRTUAL_PAD_SUBPART_MOVE],
                gmFixVirtualPadPartGetMovePadFrame());
        }

        if (plan == GME_FIX_PLAN_FLICK || plan == GME_FIX_PLAN_VIRTUAL_PAD)
        {
            var action = (OBS_OBJECT_WORK)part.sub_parts[(int)GME_FIX_VIRTUAL_PAD_SUBPART_ACTION];
            gmFixSetFrameStatic(
                action, GmPlayerKeyCheckJumpKeyOn(g_gm_main_system.ply_work[0]) ? 1f : 0.0f);
        }
    }

    private static void gmFixVirtualPadPartProcDispMain(GMS_FIX_PART_WORK part_work)
    {
    }

    private static bool gmFixVirtualPadPartIsOnPauseIcon()
    {
        var pausable = GmPauseCheckExecutable()
            || (g_gm_main_system.game_flag & (GMD_GAME_FLAG_PAUSE_DEMO | GMD_GAME_FLAG_PAUSE_IS_DECIDED)) != 0;

        return pausable && GmMainKeyCheckPauseKeyOn() >= 0;
    }

    private static float gmFixVirtualPadPartGetMovePadFrame()
    {
        var key = CPadVirtualPad.CreateInstance().GetValue();

        foreach (var entry in c_key_to_frame_table)
        {
            if ((entry.key & key) != 0)
                return entry.frame;
        }

        return 0.0f;
    }

    // ---- which parts each mode builds. these must stay below the init functions they name

    internal delegate void GMF_FIX_PART_INIT_FUNC(GMS_FIX_MGR_WORK mgr_work);

    private static readonly GMF_FIX_PART_INIT_FUNC[] gm_fix_part_init_func_tbl =
    [
        gmFixRingCountPartInit,
        gmFixScorePartInit,
        gmFixTimerPartInit,
        gmFixChallengePartInit,
        gmFixVirtualPadPartInit,
    ];

    // time attack has no score and no life counter
    private static readonly GMF_FIX_PART_INIT_FUNC[] gm_fix_ta_part_init_func_tbl =
    [
        gmFixRingCountPartInit,
        null,
        gmFixTimerPartInit,
        null,
        gmFixVirtualPadPartInit,
    ];

    // except on act 2-2, which scores the pinball table
    private static readonly GMF_FIX_PART_INIT_FUNC[] gm_fix_ta_s22_part_init_func_tbl =
    [
        gmFixRingCountPartInit,
        gmFixScorePartInit,
        gmFixTimerPartInit,
        null,
        gmFixVirtualPadPartInit,
    ];

    private static readonly GMF_FIX_PART_INIT_FUNC[] gm_fix_ss_part_init_func_tbl =
    [
        null,
        null,
        gmFixTimerSSPartInit,
        null,
        gmFixVirtualPadPartInit,
    ];
}
