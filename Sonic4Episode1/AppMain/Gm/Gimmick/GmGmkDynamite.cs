using System;

internal partial class AppMain
{
    private static OBS_ACTION3D_NN_WORK[] gm_gmk_dynamite_obj_3d_list;

    private const int GMD_GMK_LL2_CHAIN_MAX = 32;
    private const int GMD_GMK_LL2_ROW_MAX = 32;

    private static readonly OBS_OBJECT_WORK[,] gm_gmk_ll2_grid_tbl =
        new OBS_OBJECT_WORK[GMD_GMK_LL2_CHAIN_MAX, GMD_GMK_LL2_ROW_MAX];
    private static readonly int[] gm_gmk_ll2_chain_lit_tbl = new int[GMD_GMK_LL2_CHAIN_MAX];
    private static readonly int[] gm_gmk_ll2_chain_blast_row_tbl = new int[GMD_GMK_LL2_CHAIN_MAX];

    private const uint GMD_GMK_FUSE_HEAD = 1U;
    private const uint GMD_GMK_FUSE_DIAG = 2U;
    private const uint GMD_GMK_FUSE_DYNAMITE = 4U;
    private const uint GMD_GMK_FUSE_BOMB_WALL = 8U;
    private const uint GMD_GMK_FUSE_LIT = 16U;
    private const uint GMD_GMK_FUSE_RUBBLE_ODD = 128U;

    private const int GMD_GMK_DYNAMITE_MDL_BOMBWALL = 25;
    private const int GMD_GMK_DYNAMITE_MDL_BOMBWALL_R = 26;

    private const int GMD_GMK_FUSE_HEAD_Z = 655360;
    private const int GMD_GMK_FUSE_LINE_Z = 720896;
    private const int GMD_GMK_BOMBWALL_Z = -65536;

    private const int GMD_GMK_DYNAMITE_FUSE_TIME = 4;
    private const int GMD_GMK_DYNAMITE_LIGHT_TIME = 38;
    private const int GMD_GMK_DYNAMITE_BLAST_ROW_TIME = 10;
    private const int GMD_GMK_DYNAMITE_BLAST_RECT_TIME = 30;
    private const int GMD_GMK_DYNAMITE_EFF_OFST_X = 98304;
    private const int GMD_GMK_DYNAMITE_EFF_OFST_Z = 262144;

    private const int GMD_GMK_BOMBWALL_BREAK_WAIT = 10;
    private const int GMD_GMK_BOMBWALL_RUBBLE_TIME = 32;
    private const int GMD_GMK_BOMBWALL_EFF_OFST_Z = 655360;
    private const ushort GMD_GMK_BOMBWALL_COL_W = 128;
    private const ushort GMD_GMK_BOMBWALL_COL_H = 128;
    private const short GMD_GMK_BOMBWALL_COL_OFST_X = -64;
    private const short GMD_GMK_BOMBWALL_COL_OFST_Y = -128;

    private const short GMD_GMK_FUSE_RECT_BACK = -500;
    private const short GMD_GMK_FUSE_RECT_FRONT = 500;

    private const float GMD_GMK_FUSE_FLAME_LIGHT_SCALE = 0.3f;
    private const float GMD_GMK_DYNAMITE_LIGHT_SCALE = 6.0f;
    private const int GMD_GMK_FUSE_LIGHT_OFST_Y = 49152;

    internal static readonly int[] gm_gmk_fuse_line_mdl_tbl =
    {
        22,  9,  4, 16,
        17, 20, 10, 14,
         6, 18, 23, 11,
         8, 12, 19, 21
    };

    internal static readonly int[] gm_gmk_fuse_head_mdl_tbl = { 7, 13, 5, 15 };
    internal static readonly int[] gm_gmk_dynamite_mdl_tbl = { 2, 0, 3, 1 };

    internal static readonly int[] gm_gmk_fuse_line_mat_tbl =
    {
         1,  9,  4, 16,
        17,  3, 10, 12,
         5, 18,  0, 11,
         8, 14, 19,  2
    };

    internal static readonly int[] gm_gmk_fuse_head_mat_tbl = { 7, 15, 6, 13 };

    private static readonly int[] gm_gmk_fuse_edge_ofst_x_tbl = { 0, 16, 32, 16 };
    private static readonly int[] gm_gmk_fuse_edge_ofst_y_tbl = { -16, -32, -16, 0 };
    private static readonly int[] gm_gmk_fuse_center_ofst_x_tbl = { 16, 16, 16, 16 };
    private static readonly int[] gm_gmk_fuse_center_ofst_y_tbl = { -16, -16, -16, -16 };

    private static readonly int[] gm_gmk_fuse_diag_in_x_tbl = { 0, 0, 32, 32 };
    private static readonly int[] gm_gmk_fuse_diag_in_y_tbl = { -16, -16, -16, -16 };
    private static readonly int[] gm_gmk_fuse_diag_mid_x_tbl = { 20, 20, 20, 20 };
    private static readonly int[] gm_gmk_fuse_diag_mid_y_tbl = { -16, -16, -16, -16 };
    private static readonly int[] gm_gmk_fuse_diag_out_x_tbl = { 40, 40, -8, -8 };
    private static readonly int[] gm_gmk_fuse_diag_out_y_tbl = { -32, 0, -32, 0 };

    private static readonly int[] gm_gmk_bombwall_eff_top_tbl =
    {
        GME_EFCT_Z03_IDX_PC_WALL_T_A, GME_EFCT_Z03_IDX_PC_WALL_T_B, GME_EFCT_Z03_IDX_PC_WALL_T_C
    };

    private static readonly int[] gm_gmk_bombwall_eff_btm_tbl =
    {
        GME_EFCT_Z03_IDX_PC_WALL_B_A, GME_EFCT_Z03_IDX_PC_WALL_B_B, GME_EFCT_Z03_IDX_PC_WALL_B_C
    };

    private static readonly short[][] gm_gmk_bombwall_rubble_rect_tbl =
    {
        new short[] { -80, -128, 80, -64 },
        new short[] { -80, -112, 80, -56 },
        new short[] { -82,  -96, 82, -32 },
        new short[] { -84,  -56, 84,   8 },
        new short[] { -86,    0, 86,  64 },
        new short[] { -88,   64, 88, 128 },
        new short[] { -90,  128, 90, 192 },
        new short[] { -92,  192, 92, 255 },
        new short[] { -80,  -32, 80,  32 },
        new short[] { -80,    0, 80,  56 },
        new short[] { -82,   32, 82,  96 },
        new short[] { -84,   72, 84, 136 },
        new short[] { -86,  128, 86, 192 },
        new short[] { -88,  192, 88, 256 },
        new short[] { -90,  256, 90, 320 },
        new short[] { -92,  320, 92, 383 }
    };

    private static void GmGmkDynamiteBuild()
    {
        gm_gmk_dynamite_obj_3d_list = GmGameDBuildRegBuildModel(
            GmGameDatGetGimmickData(GMD_DWORK_NO_GMK_DYNAMITE_MDL),
            GmGameDatGetGimmickData(GMD_DWORK_NO_GMK_DYNAMITE_TEX), 0U);
        GmGmkDynamiteReBuild();
    }

    private static void GmGmkDynamiteFlush()
    {
        AMS_AMB_HEADER gimmickData = GmGameDatGetGimmickData(GMD_DWORK_NO_GMK_DYNAMITE_MDL);
        GmGameDBuildRegFlushModel(gm_gmk_dynamite_obj_3d_list, gimmickData.file_num);
    }

    // neither the lit flag nor the blast row is ever cleared once set, so a chain stays awake and a
    // blown wall stays blown for the rest of the load. this is the only thing that resets them
    private static void GmGmkDynamiteReBuild()
    {
        Array.Clear(gm_gmk_ll2_grid_tbl, 0, gm_gmk_ll2_grid_tbl.Length);
        Array.Clear(gm_gmk_ll2_chain_lit_tbl, 0, gm_gmk_ll2_chain_lit_tbl.Length);
        Array.Clear(gm_gmk_ll2_chain_blast_row_tbl, 0, gm_gmk_ll2_chain_blast_row_tbl.Length);
    }

    private static OBS_OBJECT_WORK gmGmkFuseGridGet(int chain, int row)
    {
        if (row < 0 || row >= GMD_GMK_LL2_ROW_MAX)
            return null;
        return gm_gmk_ll2_grid_tbl[chain, row];
    }

    private static int gmGmkFuseClipChainField(sbyte field, int max)
    {
        if (field < 0)
            return 0;
        return field > max ? max : field;
    }

    private static int gmGmkFuseFirstSetBit(int nibble)
    {
        for (int bit = 0; bit < 4; ++bit)
        {
            if ((nibble & (1 << bit)) != 0)
                return bit;
        }
        return 4;
    }

    // ev 305/306 and their far-margin twins 318/319. One maker, three shapes: the touch-lit head at
    // row 0, the dynamite bundle at the end of a chain, and the line pieces in between
    private static OBS_OBJECT_WORK GmGmkFuseLineInit(
      GMS_EVE_RECORD_EVENT eve_rec,
      int pos_x,
      int pos_y,
      byte type)
    {
        UNREFERENCED_PARAMETER(type);
        OBS_OBJECT_WORK work = GMM_ENEMY_CREATE_WORK(eve_rec, pos_x, pos_y, () => new GMS_GMK_DYNAMITE_WORK(), "GMK_FUSE_LINE");
        GMS_ENEMY_3D_WORK enemy = (GMS_ENEMY_3D_WORK)work;
        GMS_GMK_DYNAMITE_WORK fuse = (GMS_GMK_DYNAMITE_WORK)work;

        if (eve_rec.top == 0)
            fuse.fuse_flag |= GMD_GMK_FUSE_HEAD;
        if (eve_rec.id == GMD_EVENT_ID_GMK_DYNAMITE || eve_rec.id == GMD_EVENT_ID_GMK_DYNAMITE_FAR)
            fuse.fuse_flag |= GMD_GMK_FUSE_DYNAMITE;

        int entry_nibble = eve_rec.flag & 0xF;
        int exit_dir = gmGmkFuseFirstSetBit((eve_rec.flag & 0xFF) >> 4);
        bool shaped = (fuse.fuse_flag & (GMD_GMK_FUSE_HEAD | GMD_GMK_FUSE_DYNAMITE)) == 0U;

        // a head and a dynamite both carry a zero entry nibble, so the bit scan would hand them 4.
        // PC takes the raw nibble for those two and only scans it for a line piece
        int entry_dir = shaped ? gmGmkFuseFirstSetBit(entry_nibble) : entry_nibble;
        if (shaped && entry_dir == exit_dir)
            fuse.fuse_flag |= GMD_GMK_FUSE_DIAG;

        AMS_AMB_HEADER mat_amb = GmGameDatGetGimmickData(GMD_DWORK_NO_GMK_DYNAMITE_MAT);
        if ((fuse.fuse_flag & GMD_GMK_FUSE_DYNAMITE) != 0U)
        {
            ObjObjectCopyAction3dNNModel(work, gm_gmk_dynamite_obj_3d_list[gm_gmk_dynamite_mdl_tbl[exit_dir]], enemy.obj_3d);
        }
        else if ((fuse.fuse_flag & GMD_GMK_FUSE_HEAD) != 0U)
        {
            ObjObjectCopyAction3dNNModel(work, gm_gmk_dynamite_obj_3d_list[gm_gmk_fuse_head_mdl_tbl[exit_dir]], enemy.obj_3d);
            ObjObjectAction3dNNMaterialMotionLoad(work, 0, null, null, gm_gmk_fuse_head_mat_tbl[exit_dir], mat_amb);
            ObjDrawObjectActionSet3DNNMaterial(work, 0);
            gmGmkFuseLineSetHeadRect(enemy.ene_com, work);
            work.disp_flag |= OBD_DISP_STOP;
        }
        else
        {
            ObjObjectCopyAction3dNNModel(work, gm_gmk_dynamite_obj_3d_list[gm_gmk_fuse_line_mdl_tbl[exit_dir + 4 * entry_dir]], enemy.obj_3d);
            ObjObjectAction3dNNMaterialMotionLoad(work, 0, null, null, gm_gmk_fuse_line_mat_tbl[exit_dir + 4 * entry_dir], mat_amb);
            ObjDrawObjectActionSet3DNNMaterial(work, 0);
            work.disp_flag |= OBD_DISP_STOP;
        }

        work.move_flag |= OBD_MOVE_NOMOVE | OBD_MOVE_NOCOL;
        work.disp_flag |= OBD_DISP_NODIRFLIP;
        work.pos.z = GMD_GMK_FUSE_HEAD_Z;

        fuse.entry_dir = entry_dir;
        fuse.exit_dir = exit_dir;
        fuse.chain = gmGmkFuseClipChainField(eve_rec.left, GMD_GMK_LL2_CHAIN_MAX - 1);
        fuse.row = gmGmkFuseClipChainField(eve_rec.top, GMD_GMK_LL2_ROW_MAX - 1);
        fuse.param = eve_rec.width;
        gm_gmk_ll2_grid_tbl[fuse.chain, fuse.row] = work;

        if ((fuse.fuse_flag & GMD_GMK_FUSE_HEAD) != 0U)
        {
            work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkFuseLineStateHeadWaitTouch);
        }
        else if ((fuse.fuse_flag & GMD_GMK_FUSE_DYNAMITE) != 0U)
        {
            work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkDynamiteStateWaitFire);
        }
        else
        {
            work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkFuseLineStateWaitFire);
            work.pos.z = GMD_GMK_FUSE_LINE_Z;
        }

        if (fuse.param != 0 && (fuse.fuse_flag & GMD_GMK_FUSE_DYNAMITE) == 0U)
            work.obj_3d.mat_speed = fuse.param / 10f;

        mtTaskChangeTcbDestructor(work.tcb, new GSF_TASK_PROCEDURE(gmGmkFuseLineDest));
        return work;
    }

    private static void gmGmkFuseLineSetHeadRect(GMS_ENEMY_COM_WORK enemy, OBS_OBJECT_WORK work)
    {
        OBS_RECT_WORK pRec = enemy.rect_work[2];
        ObjRectGroupSet(pRec, 1, 1);
        ObjRectAtkSet(pRec, 0, 0);
        ObjRectDefSet(pRec, 65534, 0);
        ObjRectWorkZSet(pRec, 8, -24, -16, 24, -8, 16);
        pRec.ppDef = new OBS_RECT_WORK_Delegate1(gmGmkLl2TouchDefFunc);
        pRec.ppHit = null;
        pRec.parent_obj = work;
        pRec.flag |= OBD_RECT_OUT;
        work.user_flag = 0U;
    }

    private static void gmGmkFuseLineKeepAlive(OBS_OBJECT_WORK obj_work, int chain)
    {
        if (gm_gmk_ll2_grid_tbl[chain, 0] != null || gm_gmk_ll2_chain_lit_tbl[chain] != 0)
            obj_work.flag |= OBD_OBJECT_NOCLIP;
        else
            obj_work.flag &= ~OBD_OBJECT_NOCLIP;
    }

    private static void gmGmkFuseLineStopSe(GMS_GMK_DYNAMITE_WORK fuse)
    {
        if (fuse.h_snd_fuse == null)
            return;
        GmSoundStopSE(fuse.h_snd_fuse);
        GsSoundFreeSeHandle(fuse.h_snd_fuse);
        fuse.h_snd_fuse = null;
    }

    private static void gmGmkFuseLineDest(MTS_TASK_TCB tcb)
    {
        GMS_GMK_DYNAMITE_WORK fuse = (GMS_GMK_DYNAMITE_WORK)mtTaskGetTcbWork(tcb);
        gm_gmk_ll2_grid_tbl[fuse.chain, fuse.row] = null;
        gmGmkFuseLineStopSe(fuse);
        GmEnemyDefaultExit(tcb);
    }

    private static void gmGmkFuseLineStateHeadWaitTouch(OBS_OBJECT_WORK obj_work)
    {
        if ((obj_work.user_flag & GMD_GMK_LL2_TOUCHED) == 0U)
            return;
        obj_work.user_flag &= ~GMD_GMK_LL2_TOUCHED;

        GMS_GMK_DYNAMITE_WORK fuse = (GMS_GMK_DYNAMITE_WORK)obj_work;
        fuse.fuse_flag |= GMD_GMK_FUSE_LIT;
        obj_work.disp_flag &= ~OBD_DISP_STOP;
        // a burnt piece must not come back when the camera does, and this is the flag
        // GmEnemyDefaultExit reads to decide whether the event can respawn
        ((GMS_ENEMY_COM_WORK)obj_work).enemy_flag |= GMD_ENEMY_FLAG_DIE;
        gm_gmk_ll2_chain_lit_tbl[fuse.chain] = 1;
        obj_work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkFuseLineStateBurn);

        GMS_EFFECT_3DES_WORK flame = GmEfctZoneEsCreate(obj_work, GSD_MAIN_ZONE_TYPE_3, GME_EFCT_Z03_IDX_PC_FUSE_01);
        if (flame != null)
        {
            OBS_OBJECT_WORK flame_work = flame.efct_com.obj_work;
            flame_work.flag |= OBD_OBJECT_PARENT_NODIE;
            flame_work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkFuseFlameEffectState);
            flame_work.ppOutSub = new MPP_VOID_OBS_OBJECT_WORK(gmGmkFuseFlameLightDraw);
        }

        if (fuse.h_snd_fuse == null)
        {
            fuse.h_snd_fuse = GsSoundAllocSeHandle();
            GmSoundPlaySE("Fuse", fuse.h_snd_fuse);
        }

        GMS_EFFECT_3DES_WORK spark = GmEfctZoneEsCreate(obj_work, GSD_MAIN_ZONE_TYPE_3, GME_EFCT_Z03_IDX_PC_FUSE_02);
        if (spark == null)
            return;
        spark.efct_com.obj_work.flag |= OBD_OBJECT_PARENT_NODIE;
        spark.efct_com.obj_work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkFuseFlameEffectState);
    }

    private static void gmGmkFuseLineStateWaitFire(OBS_OBJECT_WORK obj_work)
    {
        GMS_GMK_DYNAMITE_WORK fuse = (GMS_GMK_DYNAMITE_WORK)obj_work;
        gmGmkFuseLineKeepAlive(obj_work, fuse.chain);

        if ((fuse.fuse_flag & GMD_GMK_FUSE_LIT) == 0U)
            return;
        obj_work.disp_flag &= ~OBD_DISP_STOP;
        ((GMS_ENEMY_COM_WORK)obj_work).enemy_flag |= GMD_ENEMY_FLAG_DIE;
        obj_work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkFuseLineStateBurn);
    }

    private static void gmGmkFuseLineStateBurn(OBS_OBJECT_WORK obj_work)
    {
        if ((obj_work.disp_flag & OBD_DISP_MAT_END) == 0U)
            return;

        GMS_GMK_DYNAMITE_WORK fuse = (GMS_GMK_DYNAMITE_WORK)obj_work;
        OBS_OBJECT_WORK next = gmGmkFuseGridGet(fuse.chain, fuse.row + 1);
        if (next != null)
        {
            GMS_GMK_DYNAMITE_WORK next_fuse = (GMS_GMK_DYNAMITE_WORK)next;
            fuse.fuse_flag &= ~GMD_GMK_FUSE_LIT;
            next_fuse.fuse_flag |= GMD_GMK_FUSE_LIT;
            if ((next_fuse.fuse_flag & GMD_GMK_FUSE_DYNAMITE) == 0U)
            {
                if (fuse.h_snd_fuse != null)
                {
                    next_fuse.h_snd_fuse = fuse.h_snd_fuse;
                    fuse.h_snd_fuse = null;
                    obj_work.flag |= OBD_OBJECT_TASKCLEAR_REQUEST;
                    return;
                }
            }
            else
            {
                gmGmkFuseLineStopSe(fuse);
            }
        }
        else
        {
            gmGmkFuseLineStopSe(fuse);
        }

        obj_work.flag |= OBD_OBJECT_TASKCLEAR_REQUEST;
    }

    private static void gmGmkDynamiteStateWaitFire(OBS_OBJECT_WORK obj_work)
    {
        GMS_GMK_DYNAMITE_WORK fuse = (GMS_GMK_DYNAMITE_WORK)obj_work;
        gmGmkFuseLineKeepAlive(obj_work, fuse.chain);

        if ((fuse.fuse_flag & GMD_GMK_FUSE_LIT) == 0U)
            return;

        ((GMS_ENEMY_COM_WORK)obj_work).enemy_flag |= GMD_ENEMY_FLAG_DIE;
        obj_work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkDynamiteStateFuse);
        obj_work.ppOutSub = new MPP_VOID_OBS_OBJECT_WORK(gmGmkDynamiteBlastLightDraw);
        obj_work.user_timer = GMD_GMK_DYNAMITE_LIGHT_TIME;
        fuse.fuse_timer = GMD_GMK_DYNAMITE_FUSE_TIME;

        GMS_EFFECT_3DES_WORK flash = GmEfctZoneEsCreate(obj_work, GSD_MAIN_ZONE_TYPE_3, GME_EFCT_Z03_IDX_PC_BOMB_F);
        if (flash == null)
            return;
        OBS_OBJECT_WORK flash_work = flash.efct_com.obj_work;
        flash_work.pos.x = obj_work.pos.x + GMD_GMK_DYNAMITE_EFF_OFST_X;
        flash_work.pos.y = obj_work.pos.y;
        flash_work.pos.z += GMD_GMK_DYNAMITE_EFF_OFST_Z;
        flash_work.flag |= OBD_OBJECT_PARENT_NODIE;
    }

    private static void gmGmkDynamiteStateFuse(OBS_OBJECT_WORK obj_work)
    {
        GMS_GMK_DYNAMITE_WORK fuse = (GMS_GMK_DYNAMITE_WORK)obj_work;
        if (fuse.param != 0)
        {
            --fuse.param;
            if (fuse.param == 0)
            {
                OBS_OBJECT_WORK next = gmGmkFuseGridGet(fuse.chain, fuse.row + 1);
                if (next != null)
                    ((GMS_GMK_DYNAMITE_WORK)next).fuse_flag |= GMD_GMK_FUSE_LIT;
            }
        }

        if (fuse.fuse_timer == 0)
            return;
        --fuse.fuse_timer;
        if (fuse.fuse_timer != 0)
            return;

        obj_work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkDynamiteStateBlast);
        fuse.blast_rect_timer = GMD_GMK_DYNAMITE_BLAST_RECT_TIME;
        fuse.blast_row_timer = GMD_GMK_DYNAMITE_BLAST_ROW_TIME;

        OBS_RECT_WORK pRec = ((GMS_ENEMY_COM_WORK)obj_work).rect_work[1];
        if ((fuse.exit_dir & 1) == 0)
            ObjRectWorkZSet(pRec, -32, -80, GMD_GMK_FUSE_RECT_BACK, 32, 16, GMD_GMK_FUSE_RECT_FRONT);
        else
            ObjRectWorkZSet(pRec, -32, -16, GMD_GMK_FUSE_RECT_BACK, 32, 80, GMD_GMK_FUSE_RECT_FRONT);
        pRec.rect.pos.x = 0;
        pRec.rect.pos.y = 0;
        pRec.rect.pos.z = 0;
        pRec.flag |= OBD_RECT_OUT;

        // TODO(port): PC births EFF_BOMB_WALL_B or _T here, common effect bank 12 or 13 by exit
        // parity. Both are PC-only members of the game-wide EFF_CMN.AMB, so BOMB_BIG stands in
        // until that archive moves to the pc tier. docs/z3a2-dynamite-re.md has what that takes
        GMS_EFFECT_3DES_WORK blast = GmEfctCmnEsCreate(obj_work, GME_EFCT_CMN_IDX.BOMB_BIG);
        if (blast != null)
        {
            OBS_OBJECT_WORK blast_work = blast.efct_com.obj_work;
            blast_work.pos.x = obj_work.pos.x + GMD_GMK_DYNAMITE_EFF_OFST_X;
            blast_work.pos.y = obj_work.pos.y;
            blast_work.flag |= OBD_OBJECT_PARENT_NODIE;
        }

        GmSoundPlaySE("Dynamite");
    }

    private static void gmGmkDynamiteStateBlast(OBS_OBJECT_WORK obj_work)
    {
        GMS_GMK_DYNAMITE_WORK fuse = (GMS_GMK_DYNAMITE_WORK)obj_work;
        if (fuse.param != 0)
        {
            --fuse.param;
            if (fuse.param == 0)
            {
                OBS_OBJECT_WORK next = gmGmkFuseGridGet(fuse.chain, fuse.row + 1);
                if (next != null)
                    ((GMS_GMK_DYNAMITE_WORK)next).fuse_flag |= GMD_GMK_FUSE_LIT;
            }
        }

        if (fuse.blast_row_timer != 0)
        {
            --fuse.blast_row_timer;
            if (fuse.blast_row_timer == 0)
            {
                gm_gmk_ll2_chain_blast_row_tbl[fuse.chain] = fuse.row;
                obj_work.disp_flag |= OBD_DISP_NODISP;
            }
        }

        if (fuse.blast_rect_timer != 0)
        {
            --fuse.blast_rect_timer;
            if (fuse.blast_rect_timer == 0)
                ((GMS_ENEMY_COM_WORK)obj_work).rect_work[1].flag &= ~OBD_RECT_ENABLE;
        }

        if (fuse.param == 0 && fuse.blast_row_timer == 0 && fuse.blast_rect_timer == 0)
            obj_work.flag |= OBD_OBJECT_TASKCLEAR_REQUEST;
    }

    private static void gmGmkDynamiteBlastLightDraw(OBS_OBJECT_WORK obj_work)
    {
        float ramp = 0.0f;
        if (obj_work.user_timer != 0)
        {
            int step = obj_work.user_timer;
            ramp = (step <= 24 ? step : 48 - step) / 24f;
            if (ObjObjectPauseCheck(0U) == 0U)
                --obj_work.user_timer;
        }

        float scale = ramp * GMD_GMK_DYNAMITE_LIGHT_SCALE;
        GmDarkLightSpriteDraw(
            FXM_FX32_TO_FLOAT(obj_work.pos.x),
            -FXM_FX32_TO_FLOAT(obj_work.pos.y - GMD_GMK_FUSE_LIGHT_OFST_Y),
            -96f, OBD_DRAW_CMD_STATE_LIGHTMAP, scale, scale, byte.MaxValue);
    }

    // the flame sprite and the burning texture run off one parameter, the material motion's own
    // frame, so the two stay in step whatever burn speed the piece was given
    private static void gmGmkFuseFlameEffectState(OBS_OBJECT_WORK obj_work)
    {
        OBS_OBJECT_WORK piece = obj_work.parent_obj;
        if (piece == null)
        {
            gmGmkFuseFlameEffectEnd(obj_work);
            return;
        }

        GMS_GMK_DYNAMITE_WORK fuse = (GMS_GMK_DYNAMITE_WORK)piece;
        if ((fuse.fuse_flag & GMD_GMK_FUSE_LIT) == 0U)
        {
            OBS_OBJECT_WORK next = gmGmkFuseGridGet(fuse.chain, fuse.row + 1);
            if (next == null)
            {
                gmGmkFuseFlameEffectEnd(obj_work);
                return;
            }

            GMS_GMK_DYNAMITE_WORK next_fuse = (GMS_GMK_DYNAMITE_WORK)next;
            if ((next_fuse.fuse_flag & GMD_GMK_FUSE_LIT) == 0U
                || (next_fuse.fuse_flag & GMD_GMK_FUSE_DYNAMITE) != 0U)
            {
                gmGmkFuseFlameEffectEnd(obj_work);
                return;
            }

            obj_work.parent_obj = next;
            piece = next;
            fuse = next_fuse;
        }

        OBS_ACTION3D_NN_WORK obj_3d = piece.obj_3d;
        if (obj_3d == null || obj_3d.motion == null)
            return;

        float start_frame = amMotionMaterialGetStartFrame(obj_3d.motion, obj_3d.mat_act_id);
        float span = amMotionMaterialGetEndFrame(obj_3d.motion, obj_3d.mat_act_id) - start_frame;
        float per = obj_3d.mat_frame / span;

        float from_x;
        float from_y;
        float to_x;
        float to_y;
        if ((fuse.fuse_flag & GMD_GMK_FUSE_DIAG) != 0U)
        {
            if (per >= 0.5f)
            {
                from_x = gm_gmk_fuse_diag_mid_x_tbl[fuse.entry_dir];
                from_y = gm_gmk_fuse_diag_mid_y_tbl[fuse.entry_dir];
                to_x = gm_gmk_fuse_diag_out_x_tbl[fuse.exit_dir];
                to_y = gm_gmk_fuse_diag_out_y_tbl[fuse.exit_dir];
                per = per + per - 1f;
            }
            else
            {
                from_x = gm_gmk_fuse_diag_in_x_tbl[fuse.entry_dir];
                from_y = gm_gmk_fuse_diag_in_y_tbl[fuse.entry_dir];
                to_x = gm_gmk_fuse_diag_mid_x_tbl[fuse.exit_dir];
                to_y = gm_gmk_fuse_diag_mid_y_tbl[fuse.exit_dir];
                per = per + per;
            }
        }
        else
        {
            if ((fuse.fuse_flag & GMD_GMK_FUSE_HEAD) != 0U)
            {
                from_x = gm_gmk_fuse_center_ofst_x_tbl[fuse.entry_dir];
                from_y = gm_gmk_fuse_center_ofst_y_tbl[fuse.entry_dir];
            }
            else
            {
                from_x = gm_gmk_fuse_edge_ofst_x_tbl[fuse.entry_dir];
                from_y = gm_gmk_fuse_edge_ofst_y_tbl[fuse.entry_dir];
            }
            to_x = gm_gmk_fuse_edge_ofst_x_tbl[fuse.exit_dir];
            to_y = gm_gmk_fuse_edge_ofst_y_tbl[fuse.exit_dir];
        }

        obj_work.pos.x = piece.pos.x + (int)((from_x + (to_x - from_x) * per) * 4096f);
        obj_work.pos.y = piece.pos.y + (int)((from_y + (to_y - from_y) * per) * 4096f);
    }

    private static void gmGmkFuseFlameEffectEnd(OBS_OBJECT_WORK obj_work)
    {
        if (obj_work.obj_3des != null && obj_work.obj_3des.ecb != null)
            amEffectKill(obj_work.obj_3des.ecb);
        obj_work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(GmEffectDefaultMainFuncDeleteAtEnd);
    }

    private static void gmGmkFuseFlameLightDraw(OBS_OBJECT_WORK obj_work)
    {
        OBS_OBJECT_WORK piece = obj_work.parent_obj;
        if (piece == null)
            return;
        if ((((GMS_GMK_DYNAMITE_WORK)piece).fuse_flag & (GMD_GMK_FUSE_DYNAMITE | GMD_GMK_FUSE_BOMB_WALL)) != 0U)
            return;

        GmDarkLightSpriteDraw(
            FXM_FX32_TO_FLOAT(obj_work.pos.x),
            -FXM_FX32_TO_FLOAT(obj_work.pos.y - GMD_GMK_FUSE_LIGHT_OFST_Y),
            -96f, OBD_DRAW_CMD_STATE_LIGHTMAP,
            GMD_GMK_FUSE_FLAME_LIGHT_SCALE, GMD_GMK_FUSE_FLAME_LIGHT_SCALE, byte.MaxValue);
    }

    // ev 307 and 320. The wall is solid because of the platform record it carries, not because of
    // anything in the map, so breaking it is a model swap and a null
    private static OBS_OBJECT_WORK GmGmkBombWallInit(
      GMS_EVE_RECORD_EVENT eve_rec,
      int pos_x,
      int pos_y,
      byte type)
    {
        UNREFERENCED_PARAMETER(type);
        OBS_OBJECT_WORK work = GMM_ENEMY_CREATE_WORK(eve_rec, pos_x, pos_y, () => new GMS_GMK_DYNAMITE_WORK(), "GMK_BOMB_WALL");
        GMS_ENEMY_3D_WORK enemy = (GMS_ENEMY_3D_WORK)work;
        GMS_GMK_DYNAMITE_WORK wall = (GMS_GMK_DYNAMITE_WORK)work;

        if (eve_rec.byte_param[1] != 0)
        {
            if ((eve_rec.flag & 1) != 0)
                work.disp_flag |= OBD_DISP_NODISP;
            else
                ObjObjectCopyAction3dNNModel(work, gm_gmk_dynamite_obj_3d_list[GMD_GMK_DYNAMITE_MDL_BOMBWALL_R], enemy.obj_3d);
        }
        else
        {
            ObjObjectCopyAction3dNNModel(work, gm_gmk_dynamite_obj_3d_list[GMD_GMK_DYNAMITE_MDL_BOMBWALL], enemy.obj_3d);
            enemy.ene_com.col_work.obj_col.obj = work;
            enemy.ene_com.col_work.obj_col.width = GMD_GMK_BOMBWALL_COL_W;
            enemy.ene_com.col_work.obj_col.height = GMD_GMK_BOMBWALL_COL_H;
            enemy.ene_com.col_work.obj_col.ofst_x = GMD_GMK_BOMBWALL_COL_OFST_X;
            enemy.ene_com.col_work.obj_col.ofst_y = GMD_GMK_BOMBWALL_COL_OFST_Y;
            work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkBombWallStateWaitBlast);
        }

        work.move_flag |= OBD_MOVE_NOMOVE | OBD_MOVE_NOCOL;
        work.disp_flag |= OBD_DISP_NODIRFLIP;
        work.pos.z = GMD_GMK_BOMBWALL_Z;

        wall.fuse_flag |= GMD_GMK_FUSE_BOMB_WALL;
        wall.chain = gmGmkFuseClipChainField(eve_rec.left, GMD_GMK_LL2_CHAIN_MAX - 1);
        wall.row = gmGmkFuseClipChainField(eve_rec.top, GMD_GMK_LL2_ROW_MAX - 1);
        return work;
    }

    private static void gmGmkBombWallStateWaitBlast(OBS_OBJECT_WORK obj_work)
    {
        GMS_GMK_DYNAMITE_WORK wall = (GMS_GMK_DYNAMITE_WORK)obj_work;
        gmGmkFuseLineKeepAlive(obj_work, wall.chain);

        if (gm_gmk_ll2_chain_blast_row_tbl[wall.chain] != wall.row)
            return;

        GMS_ENEMY_3D_WORK enemy = (GMS_ENEMY_3D_WORK)obj_work;
        GMS_EVE_RECORD_EVENT eve_rec = enemy.ene_com.eve_rec;
        eve_rec.byte_param[1] = 1;
        enemy.ene_com.col_work.obj_col.obj = null;

        if ((eve_rec.flag & 1) != 0)
            obj_work.disp_flag |= OBD_DISP_NODISP;
        else
            ObjObjectCopyAction3dNNModel(obj_work, gm_gmk_dynamite_obj_3d_list[GMD_GMK_DYNAMITE_MDL_BOMBWALL_R], enemy.obj_3d);

        obj_work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkBombWallStateBreakWait);
        obj_work.user_timer = GMD_GMK_BOMBWALL_BREAK_WAIT;

        OBS_RECT_WORK pRec = enemy.ene_com.rect_work[1];
        ObjRectWorkZSet(pRec, -80, -144, GMD_GMK_FUSE_RECT_BACK, 80, 16, GMD_GMK_FUSE_RECT_FRONT);
        pRec.rect.pos.x = 0;
        pRec.rect.pos.y = 0;
        pRec.rect.pos.z = 0;
        pRec.flag |= OBD_RECT_OUT;

        OBS_OBJECT_WORK dynamite = gm_gmk_ll2_grid_tbl[wall.chain, wall.row];
        if (dynamite == null)
            return;

        int[] eff_tbl;
        if ((((GMS_GMK_DYNAMITE_WORK)dynamite).exit_dir & 1) != 0)
        {
            wall.fuse_flag |= GMD_GMK_FUSE_RUBBLE_ODD;
            eff_tbl = gm_gmk_bombwall_eff_btm_tbl;
        }
        else
        {
            eff_tbl = gm_gmk_bombwall_eff_top_tbl;
        }

        for (int index = 0; index < eff_tbl.Length; ++index)
        {
            GMS_EFFECT_3DES_WORK crumble = GmEfctZoneEsCreate(obj_work, GSD_MAIN_ZONE_TYPE_3, eff_tbl[index]);
            if (crumble == null)
                continue;
            OBS_OBJECT_WORK crumble_work = crumble.efct_com.obj_work;
            crumble_work.pos.x = obj_work.pos.x;
            crumble_work.pos.y = obj_work.pos.y;
            crumble_work.pos.z = obj_work.pos.z + GMD_GMK_BOMBWALL_EFF_OFST_Z;
            crumble_work.flag |= OBD_OBJECT_PARENT_NODIE;
        }
    }

    private static void gmGmkBombWallStateBreakWait(OBS_OBJECT_WORK obj_work)
    {
        if (obj_work.user_timer == 0)
            return;
        --obj_work.user_timer;
        if (obj_work.user_timer == 0)
            obj_work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkBombWallStateRubbleFall);
    }

    private static void gmGmkBombWallStateRubbleFall(OBS_OBJECT_WORK obj_work)
    {
        OBS_RECT_WORK pRec = ((GMS_ENEMY_COM_WORK)obj_work).rect_work[1];
        if (obj_work.user_timer >= GMD_GMK_BOMBWALL_RUBBLE_TIME)
        {
            obj_work.ppFunc = null;
            pRec.flag &= ~OBD_RECT_ENABLE;
            return;
        }

        if ((obj_work.user_timer & 3) == 0)
        {
            GMS_GMK_DYNAMITE_WORK wall = (GMS_GMK_DYNAMITE_WORK)obj_work;
            int step = (obj_work.user_timer >> 2) & 7;
            if ((wall.fuse_flag & GMD_GMK_FUSE_RUBBLE_ODD) != 0U)
                step += 8;
            short[] rect = gm_gmk_bombwall_rubble_rect_tbl[step];
            ObjRectWorkZSet(pRec, rect[0], rect[1], GMD_GMK_FUSE_RECT_BACK, rect[2], rect[3], GMD_GMK_FUSE_RECT_FRONT);
        }
        ++obj_work.user_timer;
    }
}
