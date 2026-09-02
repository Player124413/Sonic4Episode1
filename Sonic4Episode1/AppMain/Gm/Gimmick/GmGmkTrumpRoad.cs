internal partial class AppMain
{
    // The giant card and the card road it spawns. PC gmGmkTrumpRoadInit 0x4FA1E0 (event 295, ours
    // 380) and gmGmkTrumpRoadRouteInit 0x4F9F70 (event 378, ours 393). Behaviour, work layout and
    // provenance are in docs/z2a2-trumproad-re.md; the player half is GmPlySeqTrumpRoad.cs.
    //
    // The card takes three homing attacks, then spawns the road at its own position and goes
    // dormant. The road plays ROUTE01-04 and carries the player along node 11 of its own model,
    // then crossfades to the matching GOAL and deletes itself.

    // GMK_TRUMPROAD_MDL.AMB: member 0 is CARDS_ROAD_MODEL, member 1 is CARD_MASSIVE
    private const int GMD_GMK_TRUMPROAD_MODEL_ROAD = 0;
    private const int GMD_GMK_TRUMPROAD_MODEL_CARD = 1;

    private const int GMD_GMK_TRUMPROAD_CARD_HP = 3;

    // def_flag, from the high half of PC's write at +456. Bobbin's 0xFFFE was the wrong neighbour
    private const ushort GMD_GMK_TRUMPROAD_CARD_DEF_FLAG = 0xFFFD;

    // the node the rider is glued to. boss 1 registers the same one
    private const int GMD_GMK_TRUMPROAD_RIDE_NODE = 11;

    private const int GMD_GMK_TRUMPROAD_SNM_REG_MAX = 12;

    // MTN members: 0/1 are the card's two hit reactions, then ROUTE01/GOAL01 .. ROUTE04/GOAL04.
    // gm_gmk_trumproad_route_mtn_idx on PC, 0x581828
    internal static readonly int[] gm_gmk_trumproad_route_mtn_idx = { 2, 4, 6, 8 };

    private const int GMD_GMK_TRUMPROAD_ROUTE_MAX = 4;

    // each hit knocks the hittable face this far in +x
    private const short GMD_GMK_TRUMPROAD_HIT_SHIFT = 16;

    public static void GmGmkTrumpRoadBuild()
    {
        g_gm_gmk_trumproad_obj_3d_list = GmGameDBuildRegBuildModel(
            readAMBFile(GmGameDatGetGimmickData(GMD_DWORK_NO_GMK_TRUMPROAD_MDL)),
            readAMBFile(GmGameDatGetGimmickData(GMD_DWORK_NO_GMK_TRUMPROAD_TEX)),
            0U);
    }

    public static void GmGmkTrumpRoadFlush()
    {
        AMS_AMB_HEADER header = readAMBFile(GmGameDatGetGimmickData(GMD_DWORK_NO_GMK_TRUMPROAD_MDL));
        GmGameDBuildRegFlushModel(g_gm_gmk_trumproad_obj_3d_list, header.file_num);
        g_gm_gmk_trumproad_obj_3d_list = null;
    }

    private static GMS_GMK_TRUMPROAD_WORK gmGmkTrumpRoadLoadObj(
      GMS_EVE_RECORD_EVENT eve_rec,
      int pos_x,
      int pos_y,
      int model_index,
      string name)
    {
        GMS_GMK_TRUMPROAD_WORK work = (GMS_GMK_TRUMPROAD_WORK)GMM_ENEMY_CREATE_WORK(
            eve_rec, pos_x, pos_y, () => new GMS_GMK_TRUMPROAD_WORK(), name);

        GMS_ENEMY_3D_WORK gmk_work = (GMS_ENEMY_3D_WORK)work;
        OBS_OBJECT_WORK obj_work = gmk_work.ene_com.obj_work;

        gmk_work.ene_com.rect_work[0].flag &= 4294967291U;
        gmk_work.ene_com.rect_work[1].flag &= 4294967291U;
        gmk_work.ene_com.rect_work[2].flag &= 4294967291U;

        ObjObjectCopyAction3dNNModel(obj_work, g_gm_gmk_trumproad_obj_3d_list[model_index], gmk_work.obj_3d);
        ObjObjectAction3dNNMotionLoad(obj_work, 0, false, ObjDataGet(GMD_DWORK_NO_GMK_TRUMPROAD_MTN), null, 0, null);

        obj_work.flag |= OBD_OBJECT_NORELEASE_3D;
        obj_work.dir.y = 16384;
        obj_work.pos.z = -393216;
        obj_work.move_flag |= OBD_MOVE_NOMOVE | OBD_MOVE_NOCOL;
        obj_work.disp_flag |= OBD_DISP_NODIRFLIP;
        obj_work.disp_flag &= ~(OBD_DISP_REPEAT | OBD_DISP_END);
        obj_work.ppMove = null;

        return work;
    }

    private static OBS_OBJECT_WORK GmGmkTrumpRoadInit(
      GMS_EVE_RECORD_EVENT eve_rec,
      int pos_x,
      int pos_y,
      byte type)
    {
        GMS_GMK_TRUMPROAD_WORK work = gmGmkTrumpRoadLoadObj(
            eve_rec, pos_x, pos_y, GMD_GMK_TRUMPROAD_MODEL_CARD, "GMK_TRUMPROAD_CARD");

        OBS_OBJECT_WORK obj_work = (OBS_OBJECT_WORK)work;
        work.hp = GMD_GMK_TRUMPROAD_CARD_HP;
        work.route = MTM_MATH_CLIP(eve_rec.left, 0, GMD_GMK_TRUMPROAD_ROUTE_MAX - 1);

        gmGmkTrumpRoadCardSetRect(work);
        obj_work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkTrumpRoadCardState);

        return obj_work;
    }

    // Transcribed field by field from PC 0x4FA1E0 after mapping OBS_RECT_WORK's real layout:
    // OBS_RECT carries a trailing VecFx32 pos, making it 24 bytes and OBS_RECT_WORK 64, which is
    // exactly the spacing of the card's three blocks at +408/+472/+536. Every one of PC's writes
    // then lands on a named field, including the two that had been unreadable offsets
    private static void gmGmkTrumpRoadCardSetRect(GMS_GMK_TRUMPROAD_WORK work)
    {
        GMS_ENEMY_COM_WORK ene_com = ((GMS_ENEMY_3D_WORK)work).ene_com;

        OBS_RECT_WORK pRec = ene_com.rect_work[0];
        ObjRectWorkZSet(pRec, -16, -32, -16, 16, -8, 16);

        // PC masks before it sets. Our factory rects do not have these bits, so the mask is inert
        // today, but a rect arriving with them set would collide differently than PC's does
        pRec.flag = pRec.flag & 0xFFFEFCFBU | 4U | 0x400U;

        pRec.ppHit = null;
        pRec.ppDef = new OBS_RECT_WORK_Delegate1(gmGmkTrumpRoadCardHitCb);

        // +452 and +456 in the binary: hit_power/def_power then hit_flag/def_flag. The card takes
        // no attack flags of its own and accepts everything except bit 1
        ObjRectAtkSet(pRec, 0, 0);
        ObjRectDefSet(pRec, GMD_GMK_TRUMPROAD_CARD_DEF_FLAG, 0);

        // Rect 2 is never enabled, but gmPlayerGetHomingTarget aims at the midpoint of its top and
        // bottom. Left at zero the player homes in on the card's origin while the hittable box
        // above it goes untouched. -24/-8 puts the aim point at -16, inside rect 0. PC writes only
        // these four, leaving back and front at their defaults
        OBS_RECT aim = ene_com.rect_work[2].rect;
        aim.left = 0;
        aim.top = -24;
        aim.right = 0;
        aim.bottom = -8;
    }

    // only a homing attack in flight counts. a plain jump or roll bounces off without scoring a hit
    private static void gmGmkTrumpRoadCardHitCb(OBS_RECT_WORK card_rect, OBS_RECT_WORK other_rect)
    {
        OBS_OBJECT_WORK other = other_rect.parent_obj;
        if (other.obj_type != 1)
            return;

        GMS_PLAYER_WORK ply_work = (GMS_PLAYER_WORK)other;
        if (ply_work.seq_state != GME_PLY_SEQ_STATE_HOMING)
            return;

        ((GMS_GMK_TRUMPROAD_WORK)card_rect.parent_obj).hit_latch = true;
    }

    private static void gmGmkTrumpRoadCardState(OBS_OBJECT_WORK obj_work)
    {
        GMS_GMK_TRUMPROAD_WORK work = (GMS_GMK_TRUMPROAD_WORK)obj_work;
        if (!work.hit_latch)
            return;

        work.hit_latch = false;
        --work.hp;

        if (work.hp > 0)
        {
            gmGmkTrumpRoadCardRecoil(work);
            return;
        }

        gmGmkTrumpRoadCardSpawnRoad(work);
    }

    private static void gmGmkTrumpRoadCardRecoil(GMS_GMK_TRUMPROAD_WORK work)
    {
        OBS_OBJECT_WORK obj_work = (OBS_OBJECT_WORK)work;

        // MTN member 0 on the first hit, 1 on the second
        ObjDrawObjectActionSet3DNN(obj_work, GMD_GMK_TRUMPROAD_CARD_HP - 1 - work.hp, 0);
        gmGmkTrumpRoadShiftHitFace(work);
        gmGmkTrumpRoadReboundPlayer();
    }

    // PC gmPlayerSetHomingRebound 0x4EB610, the branch this family reaches: a player still in the
    // homing state is bounced into the recovery state, which is what stops it and throws it back
    private static void gmGmkTrumpRoadReboundPlayer()
    {
        GMS_PLAYER_WORK ply_work = g_gm_main_system.ply_work[0];
        if (ply_work == null || ply_work.seq_state != GME_PLY_SEQ_STATE_HOMING)
            return;

        GmPlySeqChangeSequence(ply_work, GME_PLY_SEQ_STATE_HOMING_REF);
    }

    private static void gmGmkTrumpRoadShiftHitFace(GMS_GMK_TRUMPROAD_WORK work)
    {
        OBS_RECT_WORK pRec = ((GMS_ENEMY_3D_WORK)work).ene_com.rect_work[0];
        pRec.rect.left += GMD_GMK_TRUMPROAD_HIT_SHIFT;
        pRec.rect.right += GMD_GMK_TRUMPROAD_HIT_SHIFT;
    }

    private static void gmGmkTrumpRoadCardSpawnRoad(GMS_GMK_TRUMPROAD_WORK work)
    {
        OBS_OBJECT_WORK obj_work = (OBS_OBJECT_WORK)work;

        OBS_OBJECT_WORK road_obj = GmEventMgrLocalEventBirth(
            GMD_EVENT_ID_NOSET_TRUMP_ROAD_ROUTE,
            obj_work.pos.x,
            obj_work.pos.y,
            0,
            (sbyte)work.route,
            0, 0, 0, 0);

        if (road_obj != null)
            ((GMS_GMK_TRUMPROAD_WORK)road_obj).card_obj = obj_work;

        // no more hits; the card stays where it is holding its final pose
        ((GMS_ENEMY_3D_WORK)work).ene_com.rect_work[0].flag &= 4294967291U;
        GmSoundPlaySE("Cards3");
    }

    private static OBS_OBJECT_WORK GmGmkTrumpRoadRouteInit(
      GMS_EVE_RECORD_EVENT eve_rec,
      int pos_x,
      int pos_y,
      byte type)
    {
        GMS_GMK_TRUMPROAD_WORK work = gmGmkTrumpRoadLoadObj(
            eve_rec, pos_x, pos_y, GMD_GMK_TRUMPROAD_MODEL_ROAD, "GMK_TRUMPROAD_ROUTE");

        OBS_OBJECT_WORK obj_work = (OBS_OBJECT_WORK)work;
        work.route = MTM_MATH_CLIP(eve_rec.left, 0, GMD_GMK_TRUMPROAD_ROUTE_MAX - 1);

        // PC 0x4F9F70 does `flag |= 0x210` here, and it is what makes the ride possible at all. The
        // road's origin stays at the card while the rider travels several hundred units away, so
        // without NOCLIP the object system releases it the moment that origin leaves the view
        // margin (view_out_ofst = size_tbl + 192 = 256), sets OBD_OBJECT_TASKCLEAR, and GmPlayer
        // then nulls gmk_obj because the gimmick it was riding no longer exists
        obj_work.flag |= OBD_OBJECT_NOCLIP | OBD_OBJECT_PARENT_NODIE;

        // Empirical, and not understood. Without this the road stops drawing once its origin has
        // scrolled a certain distance away, even though the object survives (flag NOCLIP above
        // keeps it off ppViewCheck) and its draw function keeps being called. PC does not set it.
        // Nothing in this tree reads disp_flag bit 28 either, and the one comment about the flag,
        // at TitleOpVariant.cs:20, calls it iPhone-only - so the mechanism is something other than
        // the name suggests, most likely the bit reaching the am layer as a drawflag, where bit 28
        // does have meaning. Removing it brings the disappearance straight back
        obj_work.disp_flag |= OBD_DISP_NOCLIP;

        GmBsCmnInitBossMotionCBSystem(obj_work, work.bmcb_mgr);
        GmBsCmnCreateSNMWork(work.snm_work, obj_work.obj_3d._object, GMD_GMK_TRUMPROAD_SNM_REG_MAX);
        GmBsCmnAppendBossMotionCallback(work.bmcb_mgr, work.snm_work.bmcb_link);
        work.snm_reg_id = GmBsCmnRegisterSNMNode(work.snm_work, GMD_GMK_TRUMPROAD_RIDE_NODE);
        work.snm_ready = true;

        ObjDrawObjectActionSet3DNN(obj_work, gm_gmk_trumproad_route_mtn_idx[work.route], 0);
        obj_work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkTrumpRoadRouteState);
        obj_work.ppOut = new MPP_VOID_OBS_OBJECT_WORK(gmGmkTrumpRoadRouteDraw);

        work.se_handle = GsSoundAllocSeHandle();
        GmSoundPlaySE("Cards4", work.se_handle);

        GMS_PLAYER_WORK ply_work = g_gm_main_system.ply_work[0];
        if (ply_work != null)
            GmPlySeqInitTrumpRoad(ply_work, obj_work);

        return obj_work;
    }

    // PC gmGmkTrumpRoadRouteDraw 0x4F9CF0: tag the model for the card-road pass, push that pass's
    // projection and view, then draw normally. The camera command has to be pushed every frame,
    // because the display list is rebuilt every frame
    private static void gmGmkTrumpRoadRouteDraw(OBS_OBJECT_WORK obj_work)
    {
        obj_work.obj_3d.command_state = OBD_DRAW_CMD_STATE_TRUMPROAD;
        ObjDraw3DNNSetCameraEx(
            GME_CAMERA_NO_TRUMPROAD,
            NNE_PROJECTION_TYPE_PERSPECTIVE,
            OBD_DRAW_CMD_STATE_TRUMPROAD);

        ObjDrawActionSummary(obj_work);
    }

    // gmk_obj is shared with every other gimmick that grabs the player, so this checks rather than
    // casting blind
    internal static GMS_GMK_TRUMPROAD_WORK gmGmkTrumpRoadWorkOf(OBS_OBJECT_WORK obj_work)
    {
        if (obj_work == null)
            return null;

        return ((GMS_ENEMY_3D_WORK)obj_work).holder as GMS_GMK_TRUMPROAD_WORK;
    }

    // returns false once the ROUTE motion has finished, which is what ends the ride
    private static bool gmGmkTrumpRoadRidePlayer(GMS_PLAYER_WORK ply_work)
    {
        OBS_OBJECT_WORK road_obj = ply_work.gmk_obj;
        if (road_obj == null || (road_obj.disp_flag & OBD_DISP_END) != 0)
            return false;

        GMS_GMK_TRUMPROAD_WORK road = gmGmkTrumpRoadWorkOf(road_obj);
        if (road == null || !road.snm_ready)
            return false;

        ply_work.obj_work.disp_flag |= OBD_DISP_NODIRFLIP;

        // Position from the node with y negated, and the node's basis normalised into
        // obj_3d.user_obj_mtx_r. That matrix survives to the draw only because gmPlayerDispFunc
        // skips its usual rebuild while the ride flag is set
        GmBsCmnUpdateObject3DNNStuckWithNode(ply_work.obj_work, road.snm_work, road.snm_reg_id, 1);

        return true;
    }

    private static void gmGmkTrumpRoadRouteState(OBS_OBJECT_WORK obj_work)
    {
        if ((obj_work.disp_flag & OBD_DISP_END) == 0)
            return;

        GMS_GMK_TRUMPROAD_WORK work = (GMS_GMK_TRUMPROAD_WORK)obj_work;

        gmGmkTrumpRoadStopRideSe(work);

        // GOAL0x is always the member after its ROUTE. PC crossfades the two on the obj3d's second
        // channel; this hard cuts, which is a visible difference at the end of the ride
        // TODO(port): blend ROUTE's final pose into GOAL rather than cutting
        ObjDrawObjectActionSet3DNN(obj_work, gm_gmk_trumproad_route_mtn_idx[work.route] + 1, 0);
        obj_work.disp_flag &= ~OBD_DISP_END;
        obj_work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkTrumpRoadGoalState);
    }

    private static void gmGmkTrumpRoadGoalState(OBS_OBJECT_WORK obj_work)
    {
        if ((obj_work.disp_flag & OBD_DISP_END) == 0)
            return;

        GMS_GMK_TRUMPROAD_WORK work = (GMS_GMK_TRUMPROAD_WORK)obj_work;
        if (work.snm_ready)
        {
            work.snm_ready = false;
            GmBsCmnDeleteSNMWork(work.snm_work);
        }

        // PC's destructor stops the handle too, not just the route end, so a road that dies without
        // finishing its ROUTE does not leave the cue running
        gmGmkTrumpRoadStopRideSe(work);

        obj_work.flag |= OBD_OBJECT_TASKCLEAR;
    }

    private static void gmGmkTrumpRoadStopRideSe(GMS_GMK_TRUMPROAD_WORK work)
    {
        if (work.se_handle == null)
            return;

        GmSoundStopSE(work.se_handle);
        GsSoundFreeSeHandle(work.se_handle);
        work.se_handle = null;
    }
}
