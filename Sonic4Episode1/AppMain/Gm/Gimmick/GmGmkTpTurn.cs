internal partial class AppMain
{
    // The flip cards: three makers over one card behaviour, plus the frame the card draws. The
    // panels are GmGmkTpTurnPict.cs and the match logic is GmGmkTpTurnMatch.cs. Behaviour, work
    // layout and provenance are in docs/z2a2-tp-turn-re.md.
    //
    // PC gmGmkTpTurnInitBoard 0x4F8AD0 (event 302, ours 381), gmGmkTpTurnInitRow 0x4F89F0 (303,
    // ours 382) and gmGmkTpTurnInitColumn 0x4F8A60 (304, ours 383). The card itself never collides
    // and has no state beyond its highlight timer; running into one of its panels is what does
    // everything.

    private const int GMD_GMK_TP_TURN_VARIANT_BOARD = 0;
    private const int GMD_GMK_TP_TURN_VARIANT_ROW = 1;
    private const int GMD_GMK_TP_TURN_VARIANT_COLUMN = 2;

    // ev.flag bits 0 and 1 pick a frame variant within a maker
    private const ushort GMD_GMK_TP_TURN_EV_FLAG_ALT1 = 1;
    private const ushort GMD_GMK_TP_TURN_EV_FLAG_ALT2 = 2;

    // Slots 0-7 are reserved for cards the designer grouped through ev.left; the auto allocator
    // hands out 8 upwards and wraps back to 8, never to 0
    private const int GMD_GMK_TP_TURN_CARD_IDX_MAX = 64;
    private const int GMD_GMK_TP_TURN_CARD_IDX_AUTO_START = 8;

    // frames a matched card stays lit for. Variants 4 and 6 never tick theirs down
    internal const int GMD_GMK_TP_TURN_HIT_FRAMES = 120;

    internal static int[] gm_gmk_tp_turn_card_timer;
    private static int gm_gmk_tp_turn_card_next_idx;

    public static void GmGmkTpTurnBuild()
    {
        g_gm_gmk_tp_turn_obj_3d_list = GmGameDBuildRegBuildModel(
            readAMBFile(GmGameDatGetGimmickData(GMD_DWORK_NO_GMK_TP_TURN_MDL)),
            readAMBFile(GmGameDatGetGimmickData(GMD_DWORK_NO_GMK_TP_TURN_TEX)),
            0U);

        gmGmkTpTurnCreateManager();
    }

    public static void GmGmkTpTurnFlush()
    {
        gmGmkTpTurnDestroyManager();

        AMS_AMB_HEADER header = readAMBFile(GmGameDatGetGimmickData(GMD_DWORK_NO_GMK_TP_TURN_MDL));
        GmGameDBuildRegFlushModel(g_gm_gmk_tp_turn_obj_3d_list, header.file_num);
        g_gm_gmk_tp_turn_obj_3d_list = null;
    }

    private static GMS_GMK_TP_TURN_WORK gmGmkTpTurnLoadObj(
      GMS_EVE_RECORD_EVENT eve_rec,
      int pos_x,
      int pos_y,
      string name)
    {
        GMS_GMK_TP_TURN_WORK work = (GMS_GMK_TP_TURN_WORK)GMM_ENEMY_CREATE_WORK(
            eve_rec, pos_x, pos_y, () => new GMS_GMK_TP_TURN_WORK(), name);

        GMS_ENEMY_3D_WORK gmk_work = (GMS_ENEMY_3D_WORK)work;
        OBS_OBJECT_WORK obj_work = gmk_work.ene_com.obj_work;

        // the card is scenery. PC clears bit 2 of blocks 0 and 1 and leaves block 2 at its
        // factory default, which is already inactive
        gmk_work.ene_com.rect_work[0].flag &= ~4U;
        gmk_work.ene_com.rect_work[1].flag &= ~4U;

        obj_work.move_flag |= OBD_MOVE_NOMOVE | OBD_MOVE_NOCOL;
        obj_work.ppMove = null;

        return work;
    }

    private static OBS_OBJECT_WORK GmGmkTpTurnInitBoard(
      GMS_EVE_RECORD_EVENT eve_rec,
      int pos_x,
      int pos_y,
      byte type)
    {
        GMS_GMK_TP_TURN_WORK work = gmGmkTpTurnLoadObj(eve_rec, pos_x, pos_y, "GMK_TP_TURN_BOARD");
        gmGmkTpTurnSetupCard(work, GMD_GMK_TP_TURN_VARIANT_BOARD);
        return (OBS_OBJECT_WORK)work;
    }

    private static OBS_OBJECT_WORK GmGmkTpTurnInitRow(
      GMS_EVE_RECORD_EVENT eve_rec,
      int pos_x,
      int pos_y,
      byte type)
    {
        GMS_GMK_TP_TURN_WORK work = gmGmkTpTurnLoadObj(eve_rec, pos_x, pos_y, "GMK_TP_TURN_ROW");
        gmGmkTpTurnSetupCard(work, gmGmkTpTurnVariantOf(eve_rec, GMD_GMK_TP_TURN_VARIANT_ROW, 3, 4));
        return (OBS_OBJECT_WORK)work;
    }

    private static OBS_OBJECT_WORK GmGmkTpTurnInitColumn(
      GMS_EVE_RECORD_EVENT eve_rec,
      int pos_x,
      int pos_y,
      byte type)
    {
        GMS_GMK_TP_TURN_WORK work = gmGmkTpTurnLoadObj(eve_rec, pos_x, pos_y, "GMK_TP_TURN_COLUMN");
        gmGmkTpTurnSetupCard(work, gmGmkTpTurnVariantOf(eve_rec, GMD_GMK_TP_TURN_VARIANT_COLUMN, 5, 6));
        return (OBS_OBJECT_WORK)work;
    }

    // bit 0 wins over bit 1 when a record sets both, which is PC's ordering rather than a bit mask
    private static int gmGmkTpTurnVariantOf(
      GMS_EVE_RECORD_EVENT eve_rec,
      int base_variant,
      int alt1,
      int alt2)
    {
        if ((eve_rec.flag & GMD_GMK_TP_TURN_EV_FLAG_ALT1) != 0)
            return alt1;

        if ((eve_rec.flag & GMD_GMK_TP_TURN_EV_FLAG_ALT2) != 0)
            return alt2;

        return base_variant;
    }

    private static void gmGmkTpTurnSetupCard(GMS_GMK_TP_TURN_WORK work, int variant)
    {
        OBS_OBJECT_WORK obj_work = (OBS_OBJECT_WORK)work;

        work.card_idx = gmGmkTpTurnAllocCardIdx(work, variant);
        gm_gmk_tp_turn_card_timer[work.card_idx] = 0;

        obj_work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkTpTurnCardState);

        gmGmkTpTurnBindFrameModels(work, variant);
        gmGmkTpTurnSpawnPicts(work);
    }

    // The picture-card variants take a designer-chosen slot from ev.left so a set of them lights
    // as one; everything else auto-allocates above the reserved eight
    private static int gmGmkTpTurnAllocCardIdx(GMS_GMK_TP_TURN_WORK work, int variant)
    {
        if (variant >= 3 && variant <= 6)
        {
            int grouped = ((GMS_ENEMY_3D_WORK)work).ene_com.eve_rec.left;
            return grouped >= 0 && grouped < GMD_GMK_TP_TURN_CARD_IDX_AUTO_START ? grouped : 0;
        }

        int idx = gm_gmk_tp_turn_card_next_idx;
        gm_gmk_tp_turn_card_next_idx = idx + 1;

        if (gm_gmk_tp_turn_card_next_idx >= GMD_GMK_TP_TURN_CARD_IDX_MAX)
            gm_gmk_tp_turn_card_next_idx = GMD_GMK_TP_TURN_CARD_IDX_AUTO_START;

        return idx;
    }

    // PC gmGmkTpTurnBindFrameModels 0x4F7F30. Each live slot gets its own copy of the shared model
    // work plus the variant's material archive, and the base material selected into it
    private static void gmGmkTpTurnBindFrameModels(GMS_GMK_TP_TURN_WORK work, int variant)
    {
        OBS_OBJECT_WORK obj_work = (OBS_OBJECT_WORK)work;
        work.variant = variant;

        for (int slot = 0; slot < GMD_GMK_TP_TURN_FRAME_MAX; ++slot)
        {
            int member = gm_gmk_tp_turn_member_tbl[variant][slot];
            if (member < 0)
                continue;

            OBS_ACTION3D_NN_WORK obj_3d = work.frame_obj_3d[slot];
            ObjCopyAction3dNNModel(g_gm_gmk_tp_turn_obj_3d_list[member], obj_3d);

            // A mirrored segment comes out of the vertex shader wound the other way round, so the
            // face that used to point at the camera is now a back face and the default cull throws
            // the whole quad away. Confirmed off the GPU: the two columns are the same quad at the
            // same depth, one winding counter clockwise and the other clockwise, with the normal's
            // handedness flipped with it. Culling the front instead puts the mirrored half back
            if ((gm_gmk_tp_turn_draw_flag_tbl[slot] & OBD_DISP_HFLIP) != 0)
                obj_3d.drawflag = obj_3d.drawflag & ~NND_DRAWOBJ_CULLING_MASK | NND_DRAWOBJ_BACKSIDE;

            int mat = gm_gmk_tp_turn_mat_tbl[variant][slot];
            if (mat < 0)
                continue;

            gmGmkTpTurnSetSlotMaterial(work, slot, mat);
            gm_gmk_tp_turn_card_timer[work.card_idx] = 0;
        }

        obj_work.ppOut = new MPP_VOID_OBS_OBJECT_WORK(gmGmkTpTurnCardDraw);
    }

    // PC registers the whole MAT archive once and picks a member with the material action id, so
    // its tables index the archive directly. This tree loads a single member into registration
    // slot 0 instead, the shape every other gimmick and the decorations use, which leaves the
    // action id always 0 and makes the archive member the thing that gets reloaded. So the
    // base-to-lit swap costs a load here where on PC it was only a select
    private static void gmGmkTpTurnSetSlotMaterial(GMS_GMK_TP_TURN_WORK work, int slot, int member)
    {
        OBS_ACTION3D_NN_WORK obj_3d = work.frame_obj_3d[slot];
        AMS_AMB_HEADER mat_archive = readAMBFile(
            GmGameDatGetGimmickData(gm_gmk_tp_turn_mat_slot_tbl[work.variant]));

        ObjAction3dNNMaterialMotionLoad(obj_3d, 0, null, null, member, mat_archive, 64, 16);

        if (obj_3d.motion != null)
            ObjDrawAction3dActionSet3DNNMaterial(obj_3d, 0);

        gmGmkTpTurnSeedBindPose(obj_3d);

        work.frame_mat_idx[slot] = member;
        ((OBS_OBJECT_WORK)work).disp_flag |= OBD_DISP_REPEAT;
    }

    // Attaching a material motion moves the draw onto ObjDrawAction3DNN's motion branch, which
    // submits motion.data[] as a per-node TRS list. amMotionCreate only allocates that array and a
    // node motion is what fills it, so an eighteen node frame segment with a material and no node
    // motion collapses to nothing. TP_TURN ships no MTN at all, so seed the list from the model's
    // own bind pose and the segment stands still in the right shape with its materials animating
    private static void gmGmkTpTurnSeedBindPose(OBS_ACTION3D_NN_WORK obj_3d)
    {
        AMS_MOTION motion = obj_3d.motion;
        if (motion == null || motion._object == null || motion.data == null)
            return;

        for (int node = 0; node < motion.node_num && node < motion.data.Length; ++node)
        {
            NNS_NODE src = motion._object.pNodeList[node];
            NNS_TRS trs = motion.data[node];

            trs.Translation.x = src.Translation.x;
            trs.Translation.y = src.Translation.y;
            trs.Translation.z = src.Translation.z;

            trs.Scaling.x = src.Scaling.x;
            trs.Scaling.y = src.Scaling.y;
            trs.Scaling.z = src.Scaling.z;

            nnMakeRotateXYZQuaternion(out trs.Rotation, src.Rotation.x, src.Rotation.y, src.Rotation.z);
        }
    }

    // PC gmGmkTpTurnCardState 0x4F74C0. Two visual states, base and lit, off a timer only a match
    // ever adds to
    private static void gmGmkTpTurnCardState(OBS_OBJECT_WORK obj_work)
    {
        GMS_GMK_TP_TURN_WORK work = (GMS_GMK_TP_TURN_WORK)obj_work;

        // a card whose panel is still spinning must not be culled out from under it
        if (work.child_flipping)
            obj_work.flag |= OBD_OBJECT_NOCLIP;
        else
            obj_work.flag &= ~OBD_OBJECT_NOCLIP;

        work.child_flipping = false;

        // a card outliving the stage flush has nothing left to read
        if (gm_gmk_tp_turn_card_timer == null)
            return;

        if (work.card_idx < 0 || work.card_idx >= GMD_GMK_TP_TURN_CARD_IDX_MAX)
            return;

        // variants 4 and 6 stay lit for the rest of the life once they have been matched
        if (work.variant != 4 && work.variant != 6 && gm_gmk_tp_turn_card_timer[work.card_idx] > 0)
            --gm_gmk_tp_turn_card_timer[work.card_idx];

        int[] materials = gm_gmk_tp_turn_card_timer[work.card_idx] > 0
            ? gm_gmk_tp_turn_mat_hit_tbl[work.variant]
            : gm_gmk_tp_turn_mat_tbl[work.variant];

        for (int slot = 0; slot < GMD_GMK_TP_TURN_FRAME_MAX; ++slot)
        {
            int mat = materials[slot];
            if (mat < 0 || work.frame_mat_idx[slot] == mat)
                continue;

            gmGmkTpTurnSetSlotMaterial(work, slot, mat);
        }
    }

    // PC gmGmkTpTurnCardDraw 0x4F7DB0. The frame is ten separate draws at fixed offsets, the odd
    // slots mirrored in x off the same models as the even ones
    private static void gmGmkTpTurnCardDraw(OBS_OBJECT_WORK obj_work)
    {
        GMS_GMK_TP_TURN_WORK work = (GMS_GMK_TP_TURN_WORK)obj_work;
        int[] members = gm_gmk_tp_turn_member_tbl[work.variant];
        VecFx32[] offsets = gm_gmk_tp_turn_ofst_tbl[work.variant];

        for (int slot = 0; slot < GMD_GMK_TP_TURN_FRAME_MAX; ++slot)
        {
            if (members[slot] < 0)
                continue;

            OBS_ACTION3D_NN_WORK obj_3d = work.frame_obj_3d[slot];
            if (!ObjAction3dNNModelLoadCheck(obj_3d))
                continue;

            VecFx32 pos = new VecFx32(
                obj_work.pos.x + offsets[slot].x,
                obj_work.pos.y + offsets[slot].y,
                obj_work.pos.z + offsets[slot].z);

            VecFx32 scale = obj_work.scale;
            uint disp_flag = gm_gmk_tp_turn_draw_flag_tbl[slot];

            // Mirror the odd slots the way PC does, by clearing the flag's own HFLIP and negating
            // scale.x. Leaving HFLIP set instead looks equivalent and is not: the engine turns it
            // into a half turn about Y, which points the segment away from the camera, so it comes
            // out unlit even once the winding is accounted for. A mirror flips x and leaves the
            // normal facing front. Either way the winding reverses, which is what the cull flip in
            // gmGmkTpTurnBindFrameModels is for
            if ((disp_flag & OBD_DISP_HFLIP) != 0)
            {
                disp_flag &= ~OBD_DISP_HFLIP;
                scale.x = -scale.x;
            }

            if ((obj_work.disp_flag & OBD_DISP_NODISP) != 0)
                disp_flag |= OBD_DISP_NODISP;

            // the neon runs off the global clock rather than per card, so every frame in the stage
            // pulses together. mmobject is only set once a material motion has really registered
            if (obj_3d.motion != null && obj_3d.motion.mmobject != null)
            {
                float start = amMotionMaterialGetStartFrame(obj_3d.motion, obj_3d.mat_act_id);
                float span = amMotionMaterialGetEndFrame(obj_3d.motion, obj_3d.mat_act_id) - start;
                if (span > 0f)
                    obj_3d.mat_frame = g_gm_main_system.sync_time % span;
            }

            ObjDrawAction3DNN(obj_3d, new VecFx32?(pos), new VecU16?(obj_work.dir), scale, ref disp_flag);
        }
    }
}
