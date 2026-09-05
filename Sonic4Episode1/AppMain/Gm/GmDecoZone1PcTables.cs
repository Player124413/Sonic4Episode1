internal partial class AppMain
{
    // PC's DECO_ZONE1 is not the same archive as XBLA's. It drops DECO_Z1_HIMAWARI from MDL member
    // 0 and appends SUNFLOWER at 13, which shifts every other zone 1 model down by one, and it
    // replaces the sunflower's single node motion with a nested archive of three actions. So a
    // handful of the id-indexed tables fork on the deco archive's tier.
    //
    // Values measured from the retail PC binary (g_gm_deco_model_index 0x57B2A0, disp_flag
    // 0x57B8E0, node motion 0x57BC00, user_flag 0x57CEC0, rect_size 0x57EC70) and diffed against
    // the cctor literals.
    //
    // These are read through accessors rather than written into the shared tables. Writing them
    // would make a process-wide static depend on which stage loaded last, which is both a test
    // ordering hazard and the kind of thing that survives a stage transition and then reads wrong.
    //
    // Every row's slot 1 is -1 on both builds, so only slot 0 forks and the pair tables carry one
    // column each.

    // ids that index DECO_ZONE1's MDL. 17 and 111 are the sunflower, which moves to the end;
    // everything else shifts down one because HIMAWARI left member 0. Ids 14-16, 20, 27, 40-53,
    // 54, 109 and 110 are absent on purpose: for the glare and fall ids this slot is not a model
    // index at all, and DECO_ZONE1_MDL_FALL's member order is identical between the two builds.
    internal static readonly int[] gm_deco_z1_model_ids =
    {
        17, 18, 19, 21, 22, 23, 24, 25, 26, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39,
        111, 112, 113, 114,
    };

    internal static readonly int[] gm_deco_z1_model_base =
    {
         0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13,  1,  2,  9, 10, 11, 12, 13,
          0,   6,   7,   8,
    };

    internal static readonly int[] gm_deco_z1_model_pc =
    {
        13,  0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12,  0,  1,  8,  9, 10, 11, 12,
         13,   5,   6,   7,
    };

    // MTN member. XBLA is HIMAWARI then WOODA, PC is WOODA then the nested SUNFLOWER archive, so
    // the sunflower and the log swap. PC's member 1 is itself an AMB of three motions, in the order
    // SUN_STANDBY, SUN_R_INCLINATION, SUN_L_INCLINATION, which the loader reads as three actions.
    internal static readonly int[] gm_deco_z1_motion_ids = { 17, 26, 111, 114 };
    internal static readonly int[] gm_deco_z1_motion_base = { 0, 1, 0, 1 };
    internal static readonly int[] gm_deco_z1_motion_pc = { 1, 0, 1, 0 };

    // members of that nested archive, which the loader hands to amMotionSetup as action ids
    internal const int GMD_DECO_SUNFLOWER_ACT_STANDBY = 0;
    internal const int GMD_DECO_SUNFLOWER_ACT_INCLINE_R = 1;
    internal const int GMD_DECO_SUNFLOWER_ACT_INCLINE_L = 2;

    // bit 4 becomes bit 2. The sunflower's main func restores bit 2 from this table when it lands
    // back on standby, so the two have to agree.
    internal static readonly int[] gm_deco_z1_disp_ids = { 17, 111 };
    internal static readonly uint[] gm_deco_z1_disp_base = { 0x00400010, 0x00400010 };
    internal static readonly uint[] gm_deco_z1_disp_pc = { 0x00400004, 0x00400004 };

    // bit 4 becomes bit 10. Mobile's bit 4 is read by gmDecoRectFuncChangeDecreaseMotionSpeed as
    // "keep the sign"; PC's 0x400 selects the blended action change instead. Bit 0, which the
    // ending ids carry, is untouched.
    internal static readonly int[] gm_deco_z1_user_flag_ids = { 17, 111 };
    internal static readonly uint[] gm_deco_z1_user_flag_base = { 0x10, 0x11 };
    internal static readonly uint[] gm_deco_z1_user_flag_pc = { 0x400, 0x401 };

    // every height halves, because PC also moved the rect's anchor from centred on the origin to
    // sitting entirely above it. gmDecoTouchRect does the anchoring; these are the sizes it works
    // from and the two changes only make sense together.
    internal static readonly int[] gm_deco_z1_rect_ids = { 17, 26, 111, 114 };
    internal static readonly short[][] gm_deco_z1_rect_base =
    {
        new short[] { 16, 128 }, new short[] { 32, 32 },
        new short[] { 16, 128 }, new short[] { 32, 32 },
    };

    internal static readonly short[][] gm_deco_z1_rect_pc =
    {
        new short[] { 16, 64 }, new short[] { 32, 16 },
        new short[] { 16, 64 }, new short[] { 32, 16 },
    };

    private static int gmDecoZone1Row(int[] ids, int id)
    {
        for (int index = 0; index < ids.Length; ++index)
        {
            if (ids[index] == id)
                return index;
        }

        return -1;
    }

    // The five accessors every read of these tables goes through. Off the PC tier, or for an id
    // with no override, they are the table read they replaced.

    internal static int gmDecoModelIndex(int id, int slot)
    {
        int row = gm_deco_as_pc && slot == 0 ? gmDecoZone1Row(gm_deco_z1_model_ids, id) : -1;
        return row < 0 ? g_gm_deco_model_index[id][slot] : gm_deco_z1_model_pc[row];
    }

    internal static int gmDecoNodeMotionIndex(int id, int slot)
    {
        int row = gm_deco_as_pc && slot == 0 ? gmDecoZone1Row(gm_deco_z1_motion_ids, id) : -1;
        return row < 0 ? g_gm_deco_node_motion_index[id][slot] : gm_deco_z1_motion_pc[row];
    }

    internal static uint gmDecoDispFlag(int id)
    {
        int row = gm_deco_as_pc ? gmDecoZone1Row(gm_deco_z1_disp_ids, id) : -1;
        return row < 0 ? g_gm_deco_disp_flag[id] : gm_deco_z1_disp_pc[row];
    }

    internal static uint gmDecoUserFlag(int id)
    {
        int row = gm_deco_as_pc ? gmDecoZone1Row(gm_deco_z1_user_flag_ids, id) : -1;
        return row < 0 ? g_gm_deco_user_flag[id] : gm_deco_z1_user_flag_pc[row];
    }

    internal static short gmDecoRectSize(int id, int slot)
    {
        int row = gm_deco_as_pc ? gmDecoZone1Row(gm_deco_z1_rect_ids, id) : -1;
        return row < 0 ? g_gm_deco_rect_size[id][slot] : gm_deco_z1_rect_pc[row][slot];
    }

    // PC 0x4E0810 replaces gmDecoRectFuncChangeDecreaseMotionSpeed on the sunflower. Only these
    // two ids fork; the log at 26 and 114 keeps gmDecoRectFuncChangeMotionCount on both builds.
    internal static OBS_RECT_WORK_Delegate1 gmDecoFuncRect(int id)
    {
        if (gm_deco_as_pc && (id == GMD_DECORATE_ID_SUNFLOWER || id == GMD_DECORATE_ID_SUNFLOWER_ENDING))
            return gmDecoRectFuncSunflowerLean;

        return g_gm_deco_func_rect[id];
    }

    // PC 0x4E1270. Mobile centres the touch rect on the origin; PC hangs it above, and turns the
    // whole thing a quarter about Z for the ids carrying user_flag 0x800. That anchor change is why
    // every PC rect height is half its mobile counterpart: same top edge, no half below.
    // PC 0x4E1270 hangs the touch rect above the origin where mobile centres it on the origin,
    // which is why every PC rect height is half its mobile counterpart: same top edge, nothing
    // below. The anchor and the halved size are a pair, so this forks on the id having a measured
    // PC size rather than on the tier alone. An id still carrying mobile's size would otherwise get
    // PC's anchor against the wrong height and land in neither place.
    internal static void gmDecoTouchRectBounds(
      int id,
      out short left,
      out short top,
      out short right,
      out short bottom)
    {
        short width = gmDecoRectSize(id, 0);
        short height = gmDecoRectSize(id, 1);

        left = (short)(-width / 2);
        right = (short)(width / 2);

        if (gm_deco_as_pc && gmDecoZone1Row(gm_deco_z1_rect_ids, id) >= 0)
        {
            top = (short)-height;
            bottom = 0;
        }
        else
        {
            top = (short)(-height / 2);
            bottom = (short)(height / 2);
        }
    }

    private static void gmDecoSetTouchRect(GMS_DECO_WORK deco_work, int id)
    {
        gmDecoTouchRectBounds(id, out short left, out short top, out short right, out short bottom);
        gmDecoSetRect(deco_work, left, top, -500, right, bottom, 500, gmDecoFuncRect(id));
    }

    // PC 0x4E0810. Lean the way the player went and cross fade into it at a rate their speed sets,
    // so a fast Sonic snaps the flower over and a slow one nudges it. Mobile's arm instead scrubbed
    // the single motion's frame, which is why it had nothing to lean into.
    //
    // The hard cut arm is dead for both ids, which carry user_flag 0x400, and is transcribed
    // because it is what the oracle does rather than because anything reaches it.
    private static void gmDecoRectFuncSunflowerLean(
      OBS_RECT_WORK own_rect_work,
      OBS_RECT_WORK target_rect_work)
    {
        OBS_OBJECT_WORK self = own_rect_work.parent_obj;
        OBS_OBJECT_WORK toucher = target_rect_work.parent_obj;

        float speed = (float)(((int)toucher.move_flag & 32768) == 0
            ? FX_FX32_TO_F32(toucher.spd_m)
            : FX_FX32_TO_F32(toucher.spd.x));

        float blend = MTM_MATH_ABS(speed * 0.04f);
        if (blend < 0.02f)
            return;

        int act = ((int)toucher.disp_flag & 1) != 0
            ? GMD_DECO_SUNFLOWER_ACT_INCLINE_R
            : GMD_DECO_SUNFLOWER_ACT_INCLINE_L;

        if (((int)self.user_flag & 0x400) != 0)
        {
            self.obj_3d.blend_spd = blend;
            ObjDrawObjectActionSet3DNNBlend(self, act);
        }
        else
        {
            ObjDrawObjectActionSet3DNN(self, act, 0);
        }

        self.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmDecoMainFuncSunflowerReturn);
    }

    // PC 0x4DFE90. Runs while the lean plays and takes the flower home. The blend back starts ten
    // frames before the lean ends rather than at the end, so the two cross fade rather than cut.
    private static void gmDecoMainFuncSunflowerReturn(OBS_OBJECT_WORK obj_work)
    {
        OBS_ACTION3D_NN_WORK obj_3d = obj_work.obj_3d;
        float endFrame = amMotionGetEndFrame(obj_3d.motion, obj_3d.act_id[0]);

        if (((int)obj_work.user_flag & 0x400) != 0 && endFrame - 10.0 <= obj_3d.frame[0])
        {
            obj_3d.blend_spd = 0.03f;
            ObjDrawObjectActionSet3DNNBlend(obj_work, GMD_DECO_SUNFLOWER_ACT_STANDBY);
        }
        else
        {
            // disp_flag bit 3 is the motion-end latch, so this is "the lean has not finished yet"
            if (((int)obj_work.disp_flag & 8) == 0)
                return;

            ObjDrawObjectActionSet3DNN(obj_work, GMD_DECO_SUNFLOWER_ACT_STANDBY, 0);
        }

        obj_3d.speed[0] = 1f;

        int id = ((GMS_DECO_WORK)obj_work).event_record.id;
        if (((int)gmDecoDispFlag(id) & 4) != 0)
            obj_work.disp_flag |= 4U;

        obj_work.ppFunc = null;
    }
}
