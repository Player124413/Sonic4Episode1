internal partial class AppMain
{
    private static OBS_ACTION3D_NN_WORK[] gm_gmk_railend_obj_3d_list;

    private const int GMD_GMK_RAILEND_HEAD_Z = 4096;
    private const int GMD_GMK_RAILEND_PRESS_DIST = 233472;
    private const int GMD_GMK_RAILEND_PRESS_SNAP = 12288;
    private const int GMD_GMK_RAILEND_HEAD_MAX = 20480;
    private const int GMD_GMK_RAILEND_RELEASE_TIME = 8;
    private const int GMD_GMK_RAILEND_HIT_VIB = 65536;
    private const ushort GMD_GMK_RAILEND_RIDE_W = 52;
    private const ushort GMD_GMK_RAILEND_RIDE_H = 32;
    private const short GMD_GMK_RAILEND_RIDE_OFST_X = -20;
    private const short GMD_GMK_RAILEND_RIDE_OFST_Y = -31;
    private const ushort GMD_GMK_RAILEND_PRESS_H = 8;
    private const short GMD_GMK_RAILEND_PRESS_OFST_Y = -8;

    private static void GmGmkRailEndBuild()
    {
        gm_gmk_railend_obj_3d_list = GmGameDBuildRegBuildModel(
            GmGameDatGetGimmickData(GMD_DWORK_NO_GMK_RAILEND_MDL),
            GmGameDatGetGimmickData(GMD_DWORK_NO_GMK_RAILEND_TEX), 0U);
    }

    private static void GmGmkRailEndFlush()
    {
        AMS_AMB_HEADER gimmickData = GmGameDatGetGimmickData(GMD_DWORK_NO_GMK_RAILEND_MDL);
        GmGameDBuildRegFlushModel(gm_gmk_railend_obj_3d_list, gimmickData.file_num);
    }

    // ev 298, the buffer at the end of a run of rails. Two models: A is the fixed post and B is the
    // head the cart shoves along x. The cart's own nose rect is what throws the player out, this
    // half only sinks its platform and springs the head
    private static OBS_OBJECT_WORK GmGmkRailEndInit(
      GMS_EVE_RECORD_EVENT eve_rec,
      int pos_x,
      int pos_y,
      byte type)
    {
        UNREFERENCED_PARAMETER(type);
        OBS_OBJECT_WORK work = GMM_ENEMY_CREATE_WORK(eve_rec, pos_x, pos_y, () => new GMS_GMK_RAILEND_WORK(), "GMK_RAILEND");
        GMS_ENEMY_3D_WORK enemy = (GMS_ENEMY_3D_WORK)work;
        GMS_GMK_RAILEND_WORK railend = (GMS_GMK_RAILEND_WORK)work;

        ObjObjectCopyAction3dNNModel(work, gm_gmk_railend_obj_3d_list[0], enemy.obj_3d);
        ObjCopyAction3dNNModel(gm_gmk_railend_obj_3d_list[1], railend.obj_3d_b);

        railend.head_ofst.x = 0;
        railend.head_ofst.y = 0;
        railend.head_ofst.z = GMD_GMK_RAILEND_HEAD_Z;
        work.pos.z = -131072;
        work.ppOut = new MPP_VOID_OBS_OBJECT_WORK(gmGmkRailEndDispFunc);

        gmGmkRailEndSetHitRect(enemy.ene_com, -40, -48, 0, 0);

        enemy.ene_com.col_work.obj_col.obj = work;
        enemy.ene_com.col_work.obj_col.width = GMD_GMK_RAILEND_RIDE_W;
        enemy.ene_com.col_work.obj_col.height = GMD_GMK_RAILEND_RIDE_H;
        enemy.ene_com.col_work.obj_col.ofst_x = GMD_GMK_RAILEND_RIDE_OFST_X;
        // the init writes -31 and the release restores -32. not a typo, PC is a unit out with itself
        enemy.ene_com.col_work.obj_col.ofst_y = GMD_GMK_RAILEND_RIDE_OFST_Y;

        work.disp_flag |= OBD_DISP_NODIRFLIP;
        enemy.ene_com.enemy_flag |= GMD_ENEMY_FLAG_DIE;
        work.move_flag = work.move_flag & ~(OBD_MOVE_NOMOVE | OBD_MOVE_NOCOL | OBD_MOVE_FALL)
            | (OBD_MOVE_NOMOVE | OBD_MOVE_NOCOL);
        return work;
    }

    // ev 299, the same trigger with no model and a rect straight out of the .EV. Eighteen of them
    // stack into columns that catch a runaway cart where the rails stop
    private static OBS_OBJECT_WORK GmGmkRailEndAreaInit(
      GMS_EVE_RECORD_EVENT eve_rec,
      int pos_x,
      int pos_y,
      byte type)
    {
        UNREFERENCED_PARAMETER(type);
        OBS_OBJECT_WORK work = GMM_ENEMY_CREATE_WORK(eve_rec, pos_x, pos_y, () => new GMS_ENEMY_COM_WORK(), "GMK_RAILEND_AREA");
        GMS_ENEMY_COM_WORK enemy = (GMS_ENEMY_COM_WORK)work;

        gmGmkRailEndSetHitRect(enemy, eve_rec.left, eve_rec.top,
            (short)(eve_rec.left + eve_rec.width), (short)(eve_rec.top + eve_rec.height));

        work.disp_flag |= OBD_DISP_NODIRFLIP | OBD_DISP_NODISP;
        work.move_flag = work.move_flag & ~(OBD_MOVE_NOMOVE | OBD_MOVE_NOCOL | OBD_MOVE_FALL)
            | (OBD_MOVE_NOMOVE | OBD_MOVE_NOCOL);
        return work;
    }

    // hit_flag 8 against the cart nose's def_flag 0xFFF7, which blocks every other bit, so this
    // pair is the only thing either rect answers to
    private static void gmGmkRailEndSetHitRect(
      GMS_ENEMY_COM_WORK enemy,
      short left,
      short top,
      short right,
      short bottom)
    {
        OBS_RECT_WORK pRec = enemy.rect_work[1];
        ObjRectAtkSet(pRec, 8, 1);
        ObjRectDefSet(pRec, 65535, 3);
        ObjRectWorkSet(pRec, left, top, right, bottom);
        pRec.ppHit = new OBS_RECT_WORK_Delegate1(gmGmkRailEndHitFunc);
        pRec.ppDef = null;
        enemy.rect_work[0].flag &= ~4U;
        enemy.rect_work[2].flag &= ~4U;
    }

    private static void gmGmkRailEndHitFunc(
      OBS_RECT_WORK mine_rect,
      OBS_RECT_WORK match_rect)
    {
        OBS_OBJECT_WORK match_obj = match_rect.parent_obj;
        OBS_OBJECT_WORK mine_obj = mine_rect.parent_obj;
        if (match_obj == null || mine_obj == null || match_obj.obj_type != 3)
            return;

        GMS_ENEMY_COM_WORK match_enemy = (GMS_ENEMY_COM_WORK)match_obj;
        GMS_ENEMY_COM_WORK mine_enemy = (GMS_ENEMY_COM_WORK)mine_obj;
        GMS_PLAYER_WORK ply_work = g_gm_main_system.ply_work[GSD_MAIN_PLAYER_1P];

        if (match_enemy.eve_rec == null || match_enemy.eve_rec.id != 222)
        {
            ObjRectFuncNoHit(mine_rect, match_rect);
            return;
        }

        bool grounded = ply_work != null && (ply_work.obj_work.move_flag & OBD_MOVE_UNDER) != 0U;
        if (mine_enemy.eve_rec != null && mine_enemy.eve_rec.id == GMD_EVENT_ID_GMK_RAIL_END)
        {
            if (!grounded)
            {
                ObjRectFuncNoHit(mine_rect, match_rect);
                return;
            }

            GMS_GMK_RAILEND_WORK railend = (GMS_GMK_RAILEND_WORK)mine_obj;
            railend.truck = match_obj;
            mine_obj.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkRailEndPressState);
            mine_obj.user_timer = GMD_GMK_RAILEND_RELEASE_TIME;
            mine_enemy.col_work.obj_col.height = GMD_GMK_RAILEND_PRESS_H;
            mine_enemy.col_work.obj_col.ofst_y = GMD_GMK_RAILEND_PRESS_OFST_Y;
            return;
        }

        // an airborne player against an area marker falls through everything, which is PC's shape
        if (grounded)
            ObjRectFuncNoHit(mine_rect, match_rect);
    }

    private static void gmGmkRailEndPressState(OBS_OBJECT_WORK obj_work)
    {
        GMS_GMK_RAILEND_WORK railend = (GMS_GMK_RAILEND_WORK)obj_work;
        GMS_ENEMY_COM_WORK enemy = (GMS_ENEMY_COM_WORK)obj_work;
        if (railend.truck == null)
            return;

        // PC re-reads the latch it just cleared here, which would fault. bail instead
        if ((railend.truck.flag & (OBD_OBJECT_TASKCLEAR | OBD_OBJECT_TASKCLEAR_REQUEST)) != 0U)
        {
            railend.truck = null;
            obj_work.ppFunc = obj_work.user_timer > 0
                ? new MPP_VOID_OBS_OBJECT_WORK(gmGmkRailEndReleaseState)
                : null;
            return;
        }

        int gap = obj_work.pos.x - railend.truck.pos.x;
        if (gap < GMD_GMK_RAILEND_PRESS_DIST)
        {
            railend.head_ofst.x = GMD_GMK_RAILEND_PRESS_DIST - gap;
            if (railend.head_ofst.x >= GMD_GMK_RAILEND_PRESS_SNAP)
            {
                railend.head_ofst.x = GMD_GMK_RAILEND_HEAD_MAX;
                railend.truck = null;
                obj_work.ppFunc = obj_work.user_timer > 0
                    ? new MPP_VOID_OBS_OBJECT_WORK(gmGmkRailEndReleaseState)
                    : null;
                GmEfctZoneEsCreate(obj_work, GSD_MAIN_ZONE_TYPE_3, GME_EFCT_Z03_IDX_PC_TRUCK_SMOKE);
                GmCameraVibrationSet(GMD_GMK_RAILEND_HIT_VIB, GMD_GMK_RAILEND_HIT_VIB, 0);
            }
        }

        if (obj_work.user_timer <= 0)
            return;
        if (--obj_work.user_timer > 0)
            return;
        enemy.col_work.obj_col.height = GMD_GMK_RAILEND_RIDE_H;
        enemy.col_work.obj_col.ofst_y = -32;
    }

    private static void gmGmkRailEndReleaseState(OBS_OBJECT_WORK obj_work)
    {
        GMS_ENEMY_COM_WORK enemy = (GMS_ENEMY_COM_WORK)obj_work;
        if (--obj_work.user_timer > 0)
            return;
        enemy.col_work.obj_col.height = GMD_GMK_RAILEND_RIDE_H;
        enemy.col_work.obj_col.ofst_y = -32;
        obj_work.ppFunc = null;
    }

    private static void gmGmkRailEndDispFunc(OBS_OBJECT_WORK obj_work)
    {
        GMS_GMK_RAILEND_WORK railend = (GMS_GMK_RAILEND_WORK)obj_work;
        VecU16 dir = obj_work.dir;
        uint head_disp_flag = obj_work.disp_flag;

        ObjDrawAction3DNN(obj_work.obj_3d, new VecFx32?(obj_work.pos), new VecU16?(dir),
            obj_work.scale, ref obj_work.disp_flag);

        VecFx32 head_pos;
        head_pos.x = obj_work.pos.x + railend.head_ofst.x;
        head_pos.y = obj_work.pos.y + railend.head_ofst.y;
        head_pos.z = obj_work.pos.z + railend.head_ofst.z;
        ObjDrawAction3DNN(railend.obj_3d_b, new VecFx32?(head_pos), new VecU16?(dir),
            obj_work.scale, ref head_disp_flag);
    }
}
