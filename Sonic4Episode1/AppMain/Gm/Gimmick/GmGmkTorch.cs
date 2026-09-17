internal partial class AppMain
{
    private static OBS_ACTION3D_NN_WORK[] gm_gmk_torch_obj_3d_list;

    private const int GMD_GMK_TORCH_NODE = 81;
    private const int GMD_GMK_TORCH_FLAME_NUM = 3;
    private const float GMD_GMK_TORCH_FAST_SPD = 15f;

    private static void GmGmkTorchBuild()
    {
        gm_gmk_torch_obj_3d_list = GmGameDBuildRegBuildModel(
            GmGameDatGetGimmickData(GMD_DWORK_NO_GMK_TORCH_MDL),
            GmGameDatGetGimmickData(GMD_DWORK_NO_GMK_TORCH_TEX), 0U);
    }

    private static void GmGmkTorchFlush()
    {
        AMS_AMB_HEADER gimmickData = GmGameDatGetGimmickData(GMD_DWORK_NO_GMK_TORCH_MDL);
        GmGameDBuildRegFlushModel(gm_gmk_torch_obj_3d_list, gimmickData.file_num);
    }

    // ev 296, the torch stand. Five of them, the first at the stage entrance. Carrying the torch is
    // one field on the player and nothing else: no flag, no sequence state, and nothing in the
    // stage checks it. The candles and fuses light on touch whether or not you have it
    private static OBS_OBJECT_WORK GmGmkTorchInit(
      GMS_EVE_RECORD_EVENT eve_rec,
      int pos_x,
      int pos_y,
      byte type)
    {
        GMS_PLAYER_WORK ply_work = g_gm_main_system.ply_work[GSD_MAIN_PLAYER_1P];
        if (ply_work == null || ply_work.torch_work != null)
            return null;

        OBS_OBJECT_WORK work = GMM_ENEMY_CREATE_WORK(eve_rec, pos_x, pos_y, () => new GMS_GMK_TORCH_WORK(), "GMK_TORCH");
        GMS_ENEMY_3D_WORK enemy = (GMS_ENEMY_3D_WORK)work;
        ObjObjectCopyAction3dNNModel(work, gm_gmk_torch_obj_3d_list[0], enemy.obj_3d);
        work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkTorchMain);
        mtTaskChangeTcbDestructor(work.tcb, new GSF_TASK_PROCEDURE(gmGmkTorchDest));

        GmGmkTorchGiveToPlayer(ply_work, work);
        return work;
    }

    internal static void GmGmkTorchGiveToPlayer(GMS_PLAYER_WORK ply_work, OBS_OBJECT_WORK work)
    {
        ply_work.torch_work = work;
        GmGmkTorchInstallHooks(ply_work);
    }

    // the player draws through four action works and the torch has to follow whichever is live, so
    // the hook goes on all of them. The player re-init calls this again, which is why the torch
    // survives a respawn
    internal static void GmGmkTorchInstallHooks(GMS_PLAYER_WORK ply_work)
    {
        if (ply_work == null || ply_work.torch_work == null)
            return;

        for (int index = 0; index < ply_work.obj_3d.Length; ++index)
        {
            OBS_ACTION3D_NN_WORK obj_3d = ply_work.obj_3d[index];
            if (obj_3d == null)
                continue;
            obj_3d.mplt_cb_func = new MPP_VOID_ARRAYNNSMATRIX_NNSOBJECT_OBJECT(gmGmkTorchMatrixCallback);
            obj_3d.mplt_cb_param = ply_work;
        }
    }

    private static void gmGmkTorchDest(MTS_TASK_TCB tcb)
    {
        GMS_GMK_TORCH_WORK torch = (GMS_GMK_TORCH_WORK)mtTaskGetTcbWork(tcb);
        GMS_PLAYER_WORK ply_work = g_gm_main_system.ply_work[GSD_MAIN_PLAYER_1P];
        if (ply_work != null && ply_work.torch_work == (OBS_OBJECT_WORK)torch)
            ply_work.torch_work = null;
        GmEnemyDefaultExit(tcb);
    }

    private static void gmGmkTorchMain(OBS_OBJECT_WORK obj_work)
    {
        GMS_PLAYER_WORK ply_work = g_gm_main_system.ply_work[GSD_MAIN_PLAYER_1P];
        if (ply_work == null)
            return;

        if (gmGmkTorchIsHidden(ply_work))
            obj_work.disp_flag |= OBD_DISP_NODISP;
        else
            obj_work.disp_flag &= ~OBD_DISP_NODISP;

        gmGmkTorchUpdateFlames((GMS_GMK_TORCH_WORK)obj_work, ply_work);
    }

    // the long idle waits and every spin state draw Sonic without a visible hand to hold it
    private static bool gmGmkTorchIsHidden(GMS_PLAYER_WORK ply_work)
    {
        if ((ply_work.obj_work.disp_flag & OBD_DISP_NODISP) != 0U)
            return true;

        int act_state = ply_work.act_state;
        return act_state >= 4 && act_state <= 7
            || act_state >= 26 && act_state <= 31
            || act_state == 39;
    }

    private static void gmGmkTorchUpdateFlames(GMS_GMK_TORCH_WORK torch, GMS_PLAYER_WORK ply_work)
    {
        int want;
        if (gmGmkTorchIsBalled(ply_work))
            want = GME_EFCT_Z03_IDX_PC_FIRE_06;
        else if (gmGmkTorchIsFast(ply_work))
            want = GME_EFCT_Z03_IDX_PC_FIRE_04;
        else
            want = GME_EFCT_Z03_IDX_PC_FIRE_01;

        if (torch.flame_idx == want)
            return;

        gmGmkTorchReleaseFlames(torch);
        torch.flame_idx = want;

        // the idle flame is a three link chain, the other two are single
        int num = want == GME_EFCT_Z03_IDX_PC_FIRE_01 ? GMD_GMK_TORCH_FLAME_NUM : 1;
        for (int index = 0; index < num; ++index)
            torch.flame[index] = GmEfctZoneEsCreate((OBS_OBJECT_WORK)torch, GSD_MAIN_ZONE_TYPE_3, want + index);
    }

    private static void gmGmkTorchReleaseFlames(GMS_GMK_TORCH_WORK torch)
    {
        for (int index = 0; index < torch.flame.Length; ++index)
        {
            if (torch.flame[index] == null)
                continue;
            torch.flame[index].efct_com.obj_work.disp_flag |= OBD_DISP_NODISP;
            torch.flame[index].efct_com.obj_work.move_flag |= 8U;
            torch.flame[index] = null;
        }
    }

    private static bool gmGmkTorchIsBalled(GMS_PLAYER_WORK ply_work)
    {
        int act_state = ply_work.act_state;
        return act_state >= 26 && act_state <= 31 || act_state == 39;
    }

    private static bool gmGmkTorchIsFast(GMS_PLAYER_WORK ply_work)
    {
        int spd = ((int)ply_work.obj_work.move_flag & 0x8000) != 0
            ? ply_work.obj_work.spd.x
            : ply_work.obj_work.spd_m;
        return MTM_MATH_ABS(FX_FX32_TO_F32(spd) * 4f) > GMD_GMK_TORCH_FAST_SPD;
    }

    // runs after the player's matrix palette is built, which is the only point node 81 is known.
    // The palette is in view space, so it comes back through the inverted world-view first
    private static void gmGmkTorchMatrixCallback(
      NNS_MATRIX[] mtx_plt,
      NNS_OBJECT _object,
      object mplt_cb_param)
    {
        GMS_PLAYER_WORK ply_work = mplt_cb_param as GMS_PLAYER_WORK;
        if (ply_work == null || ply_work.torch_work == null || _object == null)
            return;

        int node = _object.nNode < GMD_GMK_TORCH_NODE ? 0 : GMD_GMK_TORCH_NODE;
        if (node >= _object.nNode)
            return;
        int iMatrix = _object.pNodeList[node].iMatrix;
        if (iMatrix == -1 || iMatrix >= mtx_plt.Length)
            return;

        NNS_MATRIX inv_view = GlobalPool<NNS_MATRIX>.Alloc();
        NNS_MATRIX node_mtx = GlobalPool<NNS_MATRIX>.Alloc();
        nnInvertMatrix(inv_view, amDrawGetWorldViewMatrix());
        nnMultiplyMatrix(node_mtx, inv_view, mtx_plt[iMatrix]);

        OBS_OBJECT_WORK torch_work = ply_work.torch_work;
        GMS_ENEMY_3D_WORK torch_3d = (GMS_ENEMY_3D_WORK)torch_work;

        AkMathNormalizeMtx(torch_3d.obj_3d.user_obj_mtx, node_mtx);
        nnRotateYMatrix(torch_3d.obj_3d.user_obj_mtx, torch_3d.obj_3d.user_obj_mtx, unchecked((ushort)-16384));
        torch_work.disp_flag |= 8388608U;
        VEC_Set(ref torch_work.pos, 0, 0, 0);

        gmGmkTorchPlaceFlames((GMS_GMK_TORCH_WORK)torch_work, node_mtx);

        GlobalPool<NNS_MATRIX>.Release(node_mtx);
        GlobalPool<NNS_MATRIX>.Release(inv_view);
    }

    // the flames sit three units out along the torch and stack one unit apart up the chain
    private static void gmGmkTorchPlaceFlames(GMS_GMK_TORCH_WORK torch, NNS_MATRIX node_mtx)
    {
        NNS_MATRIX flame_mtx = GlobalPool<NNS_MATRIX>.Alloc();
        AkMathNormalizeMtx(flame_mtx, node_mtx);
        nnTranslateMatrix(flame_mtx, flame_mtx, 0.0f, 0.0f, 3f);
        nnRotateYMatrix(flame_mtx, flame_mtx, unchecked((ushort)-16384));

        int x = FX_F32_TO_FX32(flame_mtx.M03);
        int y = -FX_F32_TO_FX32(flame_mtx.M13);
        int z = FX_F32_TO_FX32(flame_mtx.M23);

        for (int index = 0; index < torch.flame.Length; ++index)
        {
            if (torch.flame[index] == null)
                continue;
            OBS_OBJECT_WORK flame_work = torch.flame[index].efct_com.obj_work;
            VEC_Set(ref flame_work.pos, x, y - (index << 12), z);
        }

        GlobalPool<NNS_MATRIX>.Release(flame_mtx);
    }
}
