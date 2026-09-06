using System;

internal partial class AppMain
{
    // decoration ids 184-199, which only the PC-era levels place. PC's zone 2 act 2 puts 725 of
    // them across the card stage; no mobile or xbla map references any id above 183, so widening
    // the tables is inert for those and needs no variant gate.
    //
    // appended here rather than edited into the table literals in AppMain.cctor.cs: several of
    // those tables are assigned from decompiler-aliased locals with reused array locals, so
    // hand-editing them in place risks silently transposing values. DecoPcTableTests pins every
    // value.
    //
    // values are transcribed from the retail PC binary, whose tables are 200 long where ours are
    // 184
    internal const int GMD_DECORATE_ID_MAX_PC = 200;

    private const int GMD_DECORATE_ID_PC_START = 184;

    // named after the model each one draws, from DECO_ZONE2_MDL's own string table; the _TURN
    // suffix is the 0x800 user_flag, the quarter turn about Z that separates the two otherwise
    // identical groups
    private const int GMD_DECORATE_ID_Z2_POLE_STAND = 184;
    private const int GMD_DECORATE_ID_Z2_POLE_TOP = 185;
    private const int GMD_DECORATE_ID_Z2_DICE_A = 186;
    private const int GMD_DECORATE_ID_Z2_DICE_B = 187;
    private const int GMD_DECORATE_ID_Z2_DICE_C = 188;
    private const int GMD_DECORATE_ID_Z2_TRUMP_C = 189;
    private const int GMD_DECORATE_ID_Z2_TRUMP_D = 190;
    private const int GMD_DECORATE_ID_Z2_TRUMP_H = 191;
    private const int GMD_DECORATE_ID_Z2_TRUMP_S = 192;
    private const int GMD_DECORATE_ID_Z2_DICE_A_TURN = 193;
    private const int GMD_DECORATE_ID_Z2_DICE_B_TURN = 194;
    private const int GMD_DECORATE_ID_Z2_DICE_C_TURN = 195;
    private const int GMD_DECORATE_ID_Z2_TRUMP_C_TURN = 196;
    private const int GMD_DECORATE_ID_Z2_TRUMP_D_TURN = 197;
    private const int GMD_DECORATE_ID_Z2_TRUMP_H_TURN = 198;
    private const int GMD_DECORATE_ID_Z2_TRUMP_S_TURN = 199;

    // 184/185 static scenery, 186-188 and 193-195 dice, 189-192 and 196-199 trump cards. The set
    // repeats because the second group is the same models with a different touch response
    internal static void gmDecoExtendTablesForPCLevels()
    {
        const int from = GMD_DECORATE_ID_PC_START;
        const int to = GMD_DECORATE_ID_MAX_PC;
        const int count = to - from;

        // model member in DECO_ZONE2.AMB's MDL, 11 members on PC against mobile's 2
        int[] model = { 5, 6, 2, 3, 4, 7, 8, 9, 10, 2, 3, 4, 7, 8, 9, 10 };

        // node motion member, the three dice tumbles. -1 is none
        int[] motion = { -1, -1, 0, 1, 2, -1, -1, -1, -1, 0, 1, 2, -1, -1, -1, -1 };

        // bit 22 on every one, which is what skips the automatic 90 degree turn about Y in
        // ObjDrawAction3DNN. Bit 4 additionally on the dice
        uint[] disp = {
            0x00400000, 0x00400000, 0x00400010, 0x00400010, 0x00400010, 0x00400000, 0x00400000,
            0x00400000, 0x00400000, 0x00400010, 0x00400010, 0x00400010, 0x00400000, 0x00400000,
            0x00400000, 0x00400000,
        };

        // scales the player's speed into the spin counter. 0 on the static pair
        uint[] user_work = { 0, 0, 15, 15, 15, 20, 20, 20, 20, 15, 15, 15, 20, 20, 20, 20 };

        // 0x800 makes gmDecoLoadObj set dir.z to 0x4000, a deliberate quarter turn, on the second
        // group only. Not the same thing as the accidental turn bit 22 above suppresses
        uint[] user_flag = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x800, 0x800, 0x800, 0x800, 0x800, 0x800, 0x800 };

        Array.Resize(ref g_gm_deco_model_index, to);
        Array.Resize(ref g_gm_deco_node_motion_index, to);
        Array.Resize(ref g_gm_deco_mat_motion_index, to);
        Array.Resize(ref g_gm_deco_disp_flag, to);
        Array.Resize(ref g_gm_deco_user_work, to);
        Array.Resize(ref g_gm_deco_user_timer, to);
        Array.Resize(ref g_gm_deco_user_flag, to);
        Array.Resize(ref g_gm_deco_func_main, to);
        Array.Resize(ref g_gm_deco_func_move, to);
        Array.Resize(ref g_gm_deco_func_rect, to);
        Array.Resize(ref g_gm_deco_func_out, to);
        Array.Resize(ref g_gm_deco_func_dest, to);
        Array.Resize(ref g_gm_deco_pos, to);
        Array.Resize(ref g_gm_deco_rot_z, to);
        Array.Resize(ref g_gm_deco_rect_size, to);
        Array.Resize(ref g_gm_deco_command_state, to);
        Array.Resize(ref g_gm_decorate_tbl, to);
        Array.Resize(ref g_gm_decorate_size_tbl, to);

        for (int i = 0; i < count; ++i)
        {
            int id = from + i;
            bool dice = motion[i] >= 0;
            bool statik = i < 2;
            bool second_group = i >= 9;

            g_gm_deco_model_index[id] = new[] { model[i], -1 };
            g_gm_deco_node_motion_index[id] = new[] { motion[i], -1 };
            g_gm_deco_mat_motion_index[id] = new[] { -1, -1 };
            g_gm_deco_disp_flag[id] = disp[i];
            g_gm_deco_user_work[id] = user_work[i];
            g_gm_deco_user_timer[id] = 0;
            g_gm_deco_user_flag[id] = user_flag[i];
            g_gm_deco_pos[id] = new[] { 0, 0, -393216 };
            g_gm_deco_rot_z[id] = 0;
            g_gm_deco_rect_size[id] = statik ? new short[] { 0, 0 } : new short[] { 16, 32 };
            g_gm_deco_command_state[id] = 0;

            // every one of these draws and dies the ordinary way
            g_gm_deco_func_out[id] = new MPP_VOID_OBS_OBJECT_WORK(gmDecoDraw);
            g_gm_deco_func_dest[id] = new GSF_TASK_PROCEDURE(gmDecoTcbDest);
            g_gm_deco_func_move[id] = null;

            g_gm_deco_func_main[id] = statik ? null
                : dice ? new MPP_VOID_OBS_OBJECT_WORK(gmDecoMainFuncDiceRoll)
                : new MPP_VOID_OBS_OBJECT_WORK(gmDecoMainFuncCardSpin);

            g_gm_deco_func_rect[id] = statik ? null
                : second_group ? new OBS_RECT_WORK_Delegate1(gmDecoRectFuncSpinOnLand)
                : new OBS_RECT_WORK_Delegate1(gmDecoRectFuncSpinOnRun);

            // PC's clip half-size is 64 for all sixteen
            g_gm_decorate_size_tbl[id] = 64;

            g_gm_decorate_tbl[id] = statik ? new Del__2(GmDecoInitModel)
                : dice ? new Del__2(GmDecoInitModelMotionTouch)
                : new Del__2(GmDecoInitModelTouch);
        }
    }

    // model plus a touch rect and no node motion; the leak's dispatch set has no constructor for
    // this combination because no mobile-era decoration needed it
    private static OBS_OBJECT_WORK GmDecoInitModelTouch(
      GMS_EVE_RECORD_DECORATE dec_rec,
      int x,
      int y,
      byte type)
    {
        return gm_deco_as_model
            ? gmDecoInitModelTouch(dec_rec, x, y, type)?.obj_work
            : gmDecoInitModel(dec_rec, x, y, type)?.obj_work;
    }

    private static GMS_DECO_WORK gmDecoInitModelTouch(
      GMS_EVE_RECORD_DECORATE dec_rec,
      int x,
      int y,
      byte type)
    {
        GMS_DECO_WORK deco_work = gmDecoInitModel(dec_rec, x, y, type);
        if (deco_work == null)
            return null;

        ((OBS_OBJECT_WORK)deco_work).flag &= 4294967293U;

        int id = dec_rec.id;
        short width = gmDecoRectSize(id, 0);
        short height = gmDecoRectSize(id, 1);

        gmDecoSetRect(deco_work,
            (short)(-width / 2), (short)(-height / 2), -500,
            (short)(width / 2), (short)(height / 2), 500,
            g_gm_deco_func_rect[id]);

        return deco_work;
    }

    // the dice twin of the card spin: the same decaying impulse, but it scrubs the node motion's
    // frame rather than turning the object, so the die tumbles through its animation and coasts to
    // a stop. wrapped against the motion's own bounds; the overflow branch really does subtract
    // the end frame rather than the span - reproduced as written
    private static void gmDecoMainFuncDiceRoll(OBS_OBJECT_WORK obj_work)
    {
        int spin = obj_work.user_timer;
        if (spin == 0)
            return;

        OBS_ACTION3D_NN_WORK obj_3d = obj_work.obj_3d;
        if (obj_3d == null || obj_3d.motion == null || obj_3d.motion.mtnbuf[0] == null)
            return;

        float frame = obj_3d.frame[0] + spin / 10.0f;
        float start = amMotionGetStartFrame(obj_3d.motion, obj_3d.act_id[0]);
        float end = amMotionGetEndFrame(obj_3d.motion, obj_3d.act_id[0]);

        if (end >= frame)
        {
            if (start > frame)
                frame = end - (start - frame);
        }
        else
            frame = frame - end;

        obj_3d.frame[0] = frame;

        if (spin > 0)
            obj_work.user_timer = spin - 1 < 0 ? 0 : spin - 1;
        else
            obj_work.user_timer = spin + 1 > 0 ? 0 : spin + 1;
    }

    // user_timer carries a signed spin impulse the touch callbacks set from the player's speed;
    // it decays to zero and turns the card about Z on the way
    private static void gmDecoMainFuncCardSpin(OBS_OBJECT_WORK obj_work)
    {
        int spin = obj_work.user_timer;
        if (spin == 0)
            return;

        obj_work.dir.y += (ushort)(int)(spin / 5.0f * 182.04443359375f);

        if (spin > 0)
            obj_work.user_timer = spin - 1 < 0 ? 0 : spin - 1;
        else
            obj_work.user_timer = spin + 1 > 0 ? 0 : spin + 1;
    }

    // both turn the toucher's speed into the spin impulse, scaled by the decoration's user_work,
    // and ring once past a threshold. they differ only in which component of the player's motion
    // they read: the first group is driven by running past, the second by landing on it
    private static void gmDecoRectFuncSpinOnRun(OBS_RECT_WORK deco_rect, OBS_RECT_WORK other_rect)
    {
        OBS_OBJECT_WORK deco = deco_rect.parent_obj;
        OBS_OBJECT_WORK other = other_rect.parent_obj;

        // 0x8000 is OBD_MOVE_NOSPDM, "spd_m is not in use", so the fallback is the vector speed
        int speed = (other.move_flag & OBD_MOVE_NOSPDM) != 0 ? other.spd.x : other.spd_m;

        gmDecoApplySpin(deco, speed);
    }

    private static void gmDecoRectFuncSpinOnLand(OBS_RECT_WORK deco_rect, OBS_RECT_WORK other_rect)
    {
        OBS_OBJECT_WORK deco = deco_rect.parent_obj;
        OBS_OBJECT_WORK other = other_rect.parent_obj;

        int speed = 0;
        if ((other.move_flag & OBD_MOVE_NOSPDM) != 0)
            speed = other.spd.y;
        else if (other.dir.z != 0)
            speed = other.spd_m;

        gmDecoApplySpin(deco, speed);
    }

    private static void gmDecoApplySpin(OBS_OBJECT_WORK deco, int speed)
    {
        // 0.000244140625 is 1/4096: the speed is 20.12 fixed and the impulse is a plain integer
        int spin = (int)(speed * 0.000244140625f * deco.user_work);
        deco.user_timer = spin;

        // an id-186-199 gate here would be redundant: these two callbacks are only ever installed
        // on those ids
        if (MTM_MATH_ABS(spin) > 60)
            GmSoundPlaySE("Cards5");
    }
}
