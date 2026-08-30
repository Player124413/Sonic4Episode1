internal partial class AppMain
{
    public const string GMD_GMK_SPL_RING_MDL_FILENAME = "G_COM/GMK/GMK_SS_RING_MDL.AMB";
    public const string GMD_GMK_SPL_RING_TEX_FILENAME = "G_COM/GMK/GMK_SS_RING_TEX.AMB";

    private const int GMD_GMK_SPLRING_ROT_SPD = 0x0300;
    private const int GMD_GMK_SPLRING_VANISH_ROT_SPD = 0x2000;

    // console spins the model on Y, mobile holds it still and runs a material motion instead
    private static bool gm_gmk_splring_as_model;

    private static OBS_OBJECT_WORK GmGmkSplRingInit(
      GMS_EVE_RECORD_EVENT eve_rec,
      int pos_x,
      int pos_y,
      byte type)
    {
        OBS_OBJECT_WORK work = GMM_ENEMY_CREATE_WORK(eve_rec, pos_x, pos_y, () => new GMS_ENEMY_3D_WORK(), "GMK_SPL_RING");
        GMS_ENEMY_3D_WORK gmsEnemy3DWork = (GMS_ENEMY_3D_WORK)work;
        gmsEnemy3DWork.ene_com.enemy_flag |= 65536U;
        ObjObjectCopyAction3dNNModel(work, gm_gmk_splring_obj_3d_list[IDB_GMK_SS_RING_MDL_SS_RING_ZNO], gmsEnemy3DWork.obj_3d);
        if (!gm_gmk_splring_as_model)
        {
            ObjAction3dNNMaterialMotionLoad(gmsEnemy3DWork.obj_3d, 0, null, null, IDB_GMK_SS_RING_MAT_GMK_SS_RING_INV, (AMS_AMB_HEADER)ObjDataGet(GMD_DWORK_NO_GMK_SPL_RING_MAT).pData);
            ObjDrawAction3dActionSet3DNNMaterial(gmsEnemy3DWork.obj_3d, IDB_GMK_SS_RING_MAT_GMK_SS_RING_INV);
        }
        work.pos.z = -131072;
        work.move_flag |= 8448U;
        work.disp_flag |= 4194336U;
        work.flag |= 18U;
        gmsEnemy3DWork.ene_com.col_work.obj_col.flag |= 134217728U;
        OBS_RECT_WORK pRec = gmsEnemy3DWork.ene_com.rect_work[2];
        pRec.ppDef = new OBS_RECT_WORK_Delegate1(gmGmkSplRingDefFunc);
        ObjRectWorkSet(pRec, -4, -4, 4, 4);
        work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkSplRingWait);
        return work;
    }

    public static void GmGmkSplRingBuild()
    {
        gm_gmk_splring_as_model = amFsAllVariant(AMD_FS_VARIANT_CONSOLE,
            GMD_GMK_SPL_RING_MDL_FILENAME, GMD_GMK_SPL_RING_TEX_FILENAME);

        gm_gmk_splring_obj_3d_list = GmGameDBuildRegBuildModel(
            GmGameDatGetGimmickData(GMD_DWORK_NO_GMK_SPL_RING_MODEL),
            GmGameDatGetGimmickData(GMD_DWORK_NO_GMK_SPL_RING_TEX), 0U);
    }

    public static void GmGmkSplRingFlush()
    {
        AMS_AMB_HEADER amsAmbHeader = readAMBFile(GmGameDatGetGimmickData(GMD_DWORK_NO_GMK_SPL_RING_MODEL));
        GmGameDBuildRegFlushModel(gm_gmk_splring_obj_3d_list, amsAmbHeader.file_num);
    }

    private static OBS_OBJECT_WORK GmGmkSplRingMake(int pos_x, int pos_y)
    {
        return GmEventMgrLocalEventBirth(304, pos_x, pos_y, 0, 0, 0, 0, 0, 0);
    }

    private static void gmGmkSplRingWait(OBS_OBJECT_WORK obj_work)
    {
        GMS_PLAYER_WORK gmsPlayerWork = g_gm_main_system.ply_work[0];
        if (gmsPlayerWork == null || g_gs_main_sys_info.game_mode != 0 || (GsTrialIsTrial() || gmsPlayerWork.obj_work.pos.x < obj_work.pos.x - FXM_FLOAT_TO_FX32(AMD_SCREEN_2D_WIDTH)) || (GsMainSysIsSpecialStageClearedAct(g_gs_main_sys_info.stage_id) || GsMainSysIsStageClear(27)))
            return;
        if (g_gm_main_system.ply_work[0].ring_num < 50)
        {
            obj_work.disp_flag |= 32U;
            obj_work.flag |= 2U;
        }
        else
        {
            obj_work.disp_flag &= 4294967263U;
            obj_work.flag &= 4294967293U;
        }
        if (gm_gmk_splring_as_model)
        {
            obj_work.dir.y -= GMD_GMK_SPLRING_ROT_SPD;
            return;
        }

        uint p_disp_flag = 4;
        ObjDrawAction3DNNMaterialUpdate(obj_work.obj_3d, ref p_disp_flag);
    }

    private static void gmGmkSplRingVanishReady(OBS_OBJECT_WORK obj_work)
    {
        if (gm_gmk_splring_as_model)
        {
            obj_work.dir.y -= GMD_GMK_SPLRING_VANISH_ROT_SPD;
        }
        else
        {
            obj_work.obj_3d.mat_frame = 1f;
            uint p_disp_flag = 16;
            ObjDrawAction3DNNMaterialUpdate(obj_work.obj_3d, ref p_disp_flag);
        }

        if ((obj_work.dir.y & short.MaxValue) != 0)
            return;
        obj_work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkSplRingVanish);
        GmEffect3DESSetDispOffset(GmEfctCmnEsCreate(obj_work, 78), 0.0f, 0.0f, 50f);
        GmEfctCmnEsCreate(obj_work, 79);
    }

    private static void gmGmkSplRingVanish(OBS_OBJECT_WORK obj_work)
    {
        GMS_PLAYER_WORK gmsPlayerWork = g_gm_main_system.ply_work[0];
        obj_work.dir.y -= 4096;
        if ((obj_work.dir.y & short.MaxValue) != 0)
            return;
        obj_work.ppFunc = null;
        obj_work.disp_flag |= 32U;
        gmsPlayerWork.obj_work.disp_flag |= 32U;
        gmsPlayerWork.obj_work.move_flag |= 8192U;
    }

    private static void gmGmkSplRingDefFunc(
      OBS_RECT_WORK mine_rect,
      OBS_RECT_WORK match_rect)
    {
        GMS_ENEMY_COM_WORK parentObj1 = (GMS_ENEMY_COM_WORK)mine_rect.parent_obj;
        GMS_PLAYER_WORK parentObj2 = (GMS_PLAYER_WORK)match_rect.parent_obj;
        if (parentObj1 == null || parentObj2 == null || parentObj2.obj_work.obj_type != 1)
            return;
        if (((int)parentObj2.player_flag & 262144) != 0)
            GmPlayerSetEndTruckRide(parentObj2);
        GmPlySeqInitSplIn(parentObj2, parentObj1.obj_work.pos);
        parentObj2.gmk_flag2 |= 6U;
        ((OBS_OBJECT_WORK)parentObj1).ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkSplRingVanishReady);
        parentObj1.obj_work.dir.y &= 57344;
        parentObj1.obj_work.flag |= 2U;
        GMM_PAD_VIB_SMALL();
        GmSoundPlaySE("Special1");
    }

}