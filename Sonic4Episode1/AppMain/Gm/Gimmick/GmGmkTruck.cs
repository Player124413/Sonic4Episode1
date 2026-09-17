internal partial class AppMain
{
    private const int GMD_GMK_TRUCK_ACT_ST = 3;
    private const int GMD_GMK_TRUCK_ACT_START = 4;
    private const int GMD_GMK_TRUCK_ROT_DIV_CONSOLE = 16384;
    private const int GMD_GMK_TRUCK_ROT_DIV_MOBILE = 65536;
    private const int GMD_GMK_TRUCK_GETOFF_TIME = 8;
    private const int GMD_GMK_TRUCK_GETOFF_BRAKE = 256;
    private const int GMD_GMK_TRUCK_ROCK_FRAMES = 16;
    private const int GMD_GMK_TRUCK_ROCK_STEP = 768;
    private const int GMD_GMK_TRUCK_GETOFF_JUMP = -32768;
    private const int GMD_GMK_TRUCK_GETOFF_JUMP_MAX = -61440;
    private const ushort GMD_GMK_TRUCK_RIDE_W = 64;
    private const ushort GMD_GMK_TRUCK_RIDE_H = 40;
    private const short GMD_GMK_TRUCK_RIDE_OFST_X = -32;
    private const short GMD_GMK_TRUCK_RIDE_OFST_Y = -21;

    public static void GmGmkTruckBuild()
    {
        gm_gmk_truck_obj_3d_list = GmGameDBuildRegBuildModel(GmGameDatGetGimmickData(928), GmGameDatGetGimmickData(929), 0U);
    }

    public static void GmGmkTruckFlush()
    {
        AMS_AMB_HEADER gimmickData = GmGameDatGetGimmickData(928);
        GmGameDBuildRegFlushModel(gm_gmk_truck_obj_3d_list, gimmickData.file_num);
    }

    public static OBS_OBJECT_WORK GmGmkTruckInit(
      GMS_EVE_RECORD_EVENT eve_rec,
      int pos_x,
      int pos_y,
      byte type)
    {
        OBS_OBJECT_WORK work = GMM_ENEMY_CREATE_WORK(eve_rec, pos_x, pos_y, () => new GMS_GMK_TRUCK_WORK(), "GMK_TRUCK");
        GMS_ENEMY_3D_WORK gmsEnemy3DWork = (GMS_ENEMY_3D_WORK)work;
        GMS_GMK_TRUCK_WORK truck_work = (GMS_GMK_TRUCK_WORK)work;
        mtTaskChangeTcbDestructor(work.tcb, new GSF_TASK_PROCEDURE(gmGmkTruckDest));
        ObjObjectCopyAction3dNNModel(work, gm_gmk_truck_obj_3d_list[0], gmsEnemy3DWork.obj_3d);
        ObjObjectAction3dNNMotionLoad(work, 0, true, ObjDataGet(930), null, 0, null);
        ObjDrawObjectActionSet(work, 0);
        ObjCopyAction3dNNModel(gm_gmk_truck_obj_3d_list[1], truck_work.obj_3d_tire);
        gmsEnemy3DWork.obj_3d.mtn_cb_func = new mtn_cb_func_delegate(gmGmkTruckMotionCallback);
        gmsEnemy3DWork.obj_3d.mtn_cb_param = truck_work;
        work.ppOut = new MPP_VOID_OBS_OBJECT_WORK(gmGmkTruckDispFunc);
        work.flag |= 1U;
        work.move_flag |= 128U;
        work.disp_flag |= 16777220U;
        work.disp_flag |= 16U;
        work.obj_3d.blend_spd = 0.125f;
        truck_work.trans_r.x = 0.0f;
        truck_work.trans_r.y = 0.0f;
        truck_work.trans_r.z = 4f / FXM_FX32_TO_FLOAT(g_obj.draw_scale.x);
        nnMakeUnitMatrix(gmsEnemy3DWork.obj_3d.user_obj_mtx_r);
        nnTranslateMatrix(gmsEnemy3DWork.obj_3d.user_obj_mtx_r, gmsEnemy3DWork.obj_3d.user_obj_mtx_r, truck_work.trans_r.x, truck_work.trans_r.y, truck_work.trans_r.z);
        ObjObjectFieldRectSet(work, (short)GMD_GMK_TRUCK_FIELD_RECT_LEFT, (short)GMD_GMK_TRUCK_FIELD_RECT_TOP, (short)GMD_GMK_TRUCK_FIELD_RECT_RIGHT, (short)GMD_GMK_TRUCK_FIELD_RECT_BOTTOM);
        gmsEnemy3DWork.ene_com.rect_work[0].flag &= 4294967291U;
        gmsEnemy3DWork.ene_com.rect_work[1].flag &= 4294967291U;
        OBS_RECT_WORK pRec = gmsEnemy3DWork.ene_com.rect_work[2];
        pRec.ppDef = new OBS_RECT_WORK_Delegate1(gmGmkTruckBodyDefFunc);
        pRec.ppHit = null;
        ObjRectAtkSet(pRec, 0, 0);
        if (gmStageIsPCLevel())
        {
            ObjRectWorkSet(pRec, -16, -128, 16, 0);
            gmGmkTruckSetRailEndRect(gmsEnemy3DWork);
            gmGmkTruckSetRidePlatform(gmsEnemy3DWork, work);
        }
        else
            ObjRectWorkSet(pRec, -64, -64, 64, 64);
        NNS_RGBA col = new NNS_RGBA(1f, 1f, 1f, 1f);
        NNS_VECTOR nnsVector = GlobalPool<NNS_VECTOR>.Alloc();
        nnsVector.x = -0.85f;
        nnsVector.y = -0.45f;
        nnsVector.z = -3.05f;
        nnNormalizeVector(nnsVector, nnsVector);
        ObjDrawSetParallelLight(NNE_LIGHT_1, ref col, 1f, nnsVector);
        gmsEnemy3DWork.obj_3d.use_light_flag &= 4294967294U;
        truck_work.obj_3d_tire.use_light_flag &= 4294967294U;
        gmsEnemy3DWork.obj_3d.use_light_flag |= 2U;
        truck_work.obj_3d_tire.use_light_flag |= 2U;
        gmGmkTruckCreateLightEfct(truck_work);
        gmsEnemy3DWork.ene_com.enemy_flag |= 65536U;
        // gmGmkTruck.cpp:408-412. Console names no such list in its draw order table, so leaving
        // this set is what makes the cart invisible on the PC map
        if (!gm_gmk_as_console)
            work.obj_3d.command_state = 16U;
        return work;
    }

    // the nose sensor, PC-only. hit_flag 0 and def_flag 0xFFF7 leave it deaf to everything except
    // a rail end, whose hit_flag is the one bit 0xFFF7 does not block
    private static void gmGmkTruckSetRailEndRect(GMS_ENEMY_3D_WORK enemy)
    {
        OBS_RECT_WORK pRec = enemy.ene_com.rect_work[0];
        ObjRectGroupSet(pRec, 0, 2);
        ObjRectAtkSet(pRec, 0, 0);
        ObjRectDefSet(pRec, 65527, 0);
        ObjRectWorkSet(pRec, 0, -24, 50, 24);
        pRec.ppDef = new OBS_RECT_WORK_Delegate1(gmGmkTruckRailEndDefFunc);
        pRec.ppHit = null;
    }

    private static void gmGmkTruckSetRidePlatform(GMS_ENEMY_3D_WORK enemy, OBS_OBJECT_WORK work)
    {
        enemy.ene_com.col_work.obj_col.obj = work;
        enemy.ene_com.col_work.obj_col.width = GMD_GMK_TRUCK_RIDE_W;
        enemy.ene_com.col_work.obj_col.height = GMD_GMK_TRUCK_RIDE_H;
        enemy.ene_com.col_work.obj_col.ofst_x = GMD_GMK_TRUCK_RIDE_OFST_X;
        enemy.ene_com.col_work.obj_col.ofst_y = GMD_GMK_TRUCK_RIDE_OFST_Y;
    }

    public static OBS_OBJECT_WORK GmGmkTruckGravityInit(
      GMS_EVE_RECORD_EVENT eve_rec,
      int pos_x,
      int pos_y,
      byte type)
    {
        OBS_OBJECT_WORK work = GMM_ENEMY_CREATE_WORK(eve_rec, pos_x, pos_y, () => new GMS_ENEMY_COM_WORK(), "GMK_T_GRAVITY");
        GMS_ENEMY_COM_WORK gmsEnemyComWork = (GMS_ENEMY_COM_WORK)work;
        work.move_flag |= 8448U;
        work.disp_flag |= 32U;
        OBS_RECT_WORK pRec = gmsEnemyComWork.rect_work[2];
        ObjRectGroupSet(pRec, 1, 1);
        ObjRectAtkSet(pRec, 0, 1);
        ObjRectDefSet(pRec, 65534, 0);
        if (239 <= eve_rec.id && eve_rec.id <= 246)
        {
            short[] numArray = 239 > eve_rec.id || eve_rec.id > 242 ? gm_gmk_t_gravity_rr_rect_tbl[eve_rec.id - 243] : gm_gmk_t_gravity_r_rect_tbl;
            ObjRectSet(pRec.rect, numArray[0], numArray[1], numArray[2], numArray[3]);
        }
        else
            ObjRectSet(pRec.rect, (short)(eve_rec.left << 1), (short)(eve_rec.top << 1), (short)(eve_rec.width + eve_rec.left << 1), (short)(eve_rec.height + eve_rec.top << 1));
        pRec.parent_obj = work;
        pRec.flag |= 192U;
        pRec.ppDef = 268 > eve_rec.id || eve_rec.id > 271 ? new OBS_RECT_WORK_Delegate1(gmGmkTGravityChangeDefFunc) : new OBS_RECT_WORK_Delegate1(gmGmkTGravityForceChangeDefFunc);
        gmsEnemyComWork.rect_work[1].flag &= 4294967291U;
        gmsEnemyComWork.rect_work[0].flag &= 4294967291U;
        return work;
    }

    public static OBS_OBJECT_WORK GmGmkTruckNoLandingInit(
      GMS_EVE_RECORD_EVENT eve_rec,
      int pos_x,
      int pos_y,
      byte type)
    {
        UNREFERENCED_PARAMETER(type);
        OBS_OBJECT_WORK work = GMM_ENEMY_CREATE_WORK(eve_rec, pos_x, pos_y, () => new GMS_ENEMY_COM_WORK(), "GMK_T_NOLANDING");
        GMS_ENEMY_COM_WORK gmsEnemyComWork = (GMS_ENEMY_COM_WORK)work;
        work.move_flag |= 8448U;
        work.disp_flag |= 32U;
        OBS_RECT_WORK pRec = gmsEnemyComWork.rect_work[2];
        ObjRectGroupSet(pRec, 1, 1);
        ObjRectAtkSet(pRec, 0, 1);
        ObjRectDefSet(pRec, 65534, 0);
        ObjRectSet(pRec.rect, (short)(eve_rec.left << 1), (short)(eve_rec.top << 1), (short)(eve_rec.width + eve_rec.left << 1), (short)(eve_rec.height + eve_rec.top << 1));
        pRec.ppDef = new OBS_RECT_WORK_Delegate1(gmGmkTNoLandingDefFunc);
        pRec.parent_obj = work;
        pRec.flag |= 192U;
        gmsEnemyComWork.rect_work[1].flag &= 4294967291U;
        gmsEnemyComWork.rect_work[0].flag &= 4294967291U;
        return work;
    }

    public static void gmGmkTruckDest(MTS_TASK_TCB tcb)
    {
        GMS_GMK_TRUCK_WORK tcbWork = (GMS_GMK_TRUCK_WORK)mtTaskGetTcbWork(tcb);
        if (tcbWork.h_snd_lorry != null)
        {
            GmSoundStopSE(tcbWork.h_snd_lorry);
            GsSoundFreeSeHandle(tcbWork.h_snd_lorry);
            tcbWork.h_snd_lorry = null;
        }
        GmEnemyDefaultExit(tcb);
    }

    public static void gmGmkTruckInitMain(
      OBS_OBJECT_WORK obj_work,
      GMS_PLAYER_WORK ply_work)
    {
        UNREFERENCED_PARAMETER(ply_work);
        GMS_GMK_TRUCK_WORK truck_work = (GMS_GMK_TRUCK_WORK)obj_work;
        GMS_ENEMY_3D_WORK enemy = (GMS_ENEMY_3D_WORK)obj_work;
        obj_work.move_flag |= 8448U;
        if (gmStageIsPCLevel())
        {
            // ObjObjectRectRegist drops every rect of an OBD_OBJECT_NOHIT object, so the leak's
            // flag |= NOHIT here (gmGmkTruck.cpp:628) would kill the nose sensor the moment you
            // board. PC does not set it, which is the whole reason the dismount can fire
            enemy.ene_com.rect_work[1].flag &= ~4U;
            enemy.ene_com.rect_work[2].flag &= ~4U;
            enemy.ene_com.col_work.obj_col.obj = null;
            obj_work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkTruckBoardMain);
        }
        else
        {
            obj_work.flag |= 2U;
            obj_work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkTruckMain);
        }
        gmGmkTruckCreateSparkEfct(truck_work, GME_EFCT_Z03_IDX_SPARK_S);
        truck_work.h_snd_lorry = GsSoundAllocSeHandle();
    }

    // PC gates the START motion on the player reaching act_state 78, its own boarding pose. That
    // motion is in no archive we ship, so ours runs the cart's half off the ride itself
    private static void gmGmkTruckBoardMain(OBS_OBJECT_WORK obj_work)
    {
        GMS_GMK_TRUCK_WORK truck_work = (GMS_GMK_TRUCK_WORK)obj_work;
        if (truck_work.target_player == null || obj_work.obj_3d == null || obj_work.obj_3d.motion == null)
        {
            obj_work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkTruckMain);
            gmGmkTruckMain(obj_work);
            return;
        }

        // PC's rider is stood in the boarding animation while this runs, so its cart has nothing to
        // follow. Ours is already riding, and a cart left behind for the 41 frames of START is worse
        // than no boarding motion at all
        gmGmkTruckFollowPlayer(obj_work, truck_work.target_player);

        if (obj_work.obj_3d.act_id[0] != GMD_GMK_TRUCK_ACT_START)
        {
            ObjDrawObjectActionSet3DNNBlend(obj_work, GMD_GMK_TRUCK_ACT_START);
            obj_work.disp_flag &= ~OBD_DISP_STOP;
            return;
        }

        if ((obj_work.disp_flag & OBD_DISP_END) == 0U)
            return;

        obj_work.disp_flag |= OBD_DISP_STOP;
        obj_work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkTruckMain);
        GmSoundPlaySEForce("Lorry", truck_work.h_snd_lorry, true);
    }

    private static void gmGmkTruckFollowPlayer(
      OBS_OBJECT_WORK obj_work,
      GMS_PLAYER_WORK ply_work)
    {
        if (((int)ply_work.player_flag & 1024) != 0)
            obj_work.pos.z = 983040;
        obj_work.prev_pos = obj_work.pos;
        obj_work.pos.x = ply_work.obj_work.pos.x;
        obj_work.pos.y = ply_work.obj_work.pos.y;
        obj_work.move.x = obj_work.pos.x - obj_work.prev_pos.x;
        obj_work.move.y = obj_work.pos.y - obj_work.prev_pos.y;
        obj_work.move.z = obj_work.pos.z - obj_work.prev_pos.z;
        obj_work.dir = ply_work.obj_work.dir;
        obj_work.dir.z += ply_work.obj_work.dir_fall;
        obj_work.vib_timer = ply_work.obj_work.vib_timer;
    }

    public static void gmGmkTruckMain(OBS_OBJECT_WORK obj_work)
    {
        GMS_GMK_TRUCK_WORK truck_work = (GMS_GMK_TRUCK_WORK)obj_work;
        GMS_ENEMY_COM_WORK gmsEnemyComWork = (GMS_ENEMY_COM_WORK)obj_work;
        if (truck_work.target_player == null)
        {
            gmGmkTruckInitDeathFall(obj_work, null);
            obj_work.ppFunc(obj_work);
        }
        else if (((int)truck_work.target_player.player_flag & 262144) == 0)
        {
            gmGmkTruckInitFree(obj_work, truck_work.target_player);
            obj_work.ppFunc(obj_work);
        }
        else if (((int)truck_work.target_player.gmk_flag2 & 64) != 0)
        {
            gmGmkTruckInitDeathFall(obj_work, truck_work.target_player);
            obj_work.ppFunc(obj_work);
        }
        else
        {
            GMS_PLAYER_WORK targetPlayer = truck_work.target_player;
            gmGmkTruckFollowPlayer(obj_work, targetPlayer);
            obj_work.disp_flag &= 4294967279U;
            truck_work.tire_dir_spd = ((int)targetPlayer.obj_work.move_flag & 1) == 0 ? ObjSpdDownSet(truck_work.tire_dir_spd, 128) : targetPlayer.obj_work.spd_m;
            truck_work.tire_dir += (ushort)FX_Div(truck_work.tire_dir_spd,
                gm_gmk_as_console ? GMD_GMK_TRUCK_ROT_DIV_CONSOLE : GMD_GMK_TRUCK_ROT_DIV_MOBILE);
            int id = -1;
            uint num = 0;
            if (0 <= targetPlayer.act_state && targetPlayer.act_state <= 7 || (targetPlayer.act_state == 69 || targetPlayer.act_state == 70) || (targetPlayer.act_state == 74 || targetPlayer.act_state == 76 || targetPlayer.act_state == 75))
            {
                id = 3;
                num = 4U;
            }
            else if (71 <= targetPlayer.act_state && targetPlayer.act_state <= 72)
            {
                id = 0;
                num = 4U;
            }
            else if (((int)targetPlayer.obj_work.move_flag & 1) == 0)
            {
                id = 1;
                num = 4U;
            }
            else if (((int)targetPlayer.obj_work.move_flag & 1) != 0 && ((int)targetPlayer.obj_work.move_flag & 4194304) == 0)
                id = 2;
            else if (obj_work.obj_3d.act_id[0] == 2 && ((int)obj_work.disp_flag & 8) != 0)
            {
                id = targetPlayer.obj_work.spd_m == 0 ? 3 : 0;
                num = 4U;
            }
            else if (11 <= targetPlayer.act_state && targetPlayer.act_state <= 16 && (obj_work.obj_3d.act_id[0] != 2 || ((int)obj_work.disp_flag & 8) != 0))
            {
                id = 3;
                num = 4U;
            }
            if (id != -1 && obj_work.obj_3d.act_id[0] != id)
            {
                ObjDrawObjectActionSet3DNNBlend(obj_work, id);
                obj_work.disp_flag |= num;
            }
            if (obj_work.obj_3d.act_id[0] != 3 && (11 > targetPlayer.act_state || targetPlayer.act_state > 16 || obj_work.obj_3d.act_id[0] != 2))
                obj_work.obj_3d.frame[0] = targetPlayer.obj_work.obj_3d.frame[0];
            truck_work.slope_f_y_dir = 0;
            truck_work.slope_f_z_dir = 0;
            truck_work.slope_z_dir = 0;
            float x;
            float y;
            float z;
            if (((int)targetPlayer.player_flag & 4) == 0)
            {
                x = 0.0f;
                y = 8f;
                z = -5f;
            }
            else
            {
                x = 0.0f;
                y = 8f;
                z = 5f;
            }
            nnMakeUnitMatrix(obj_work.obj_3d.user_obj_mtx_r);
            nnTranslateMatrix(obj_work.obj_3d.user_obj_mtx_r, obj_work.obj_3d.user_obj_mtx_r, truck_work.trans_r.x, truck_work.trans_r.y, truck_work.trans_r.z);
            if (((int)targetPlayer.gmk_flag & 262144) != 0 && targetPlayer.gmk_work3 != 0)
            {
                NNS_MATRIX nnsMatrix = GlobalPool<NNS_MATRIX>.Alloc();
                truck_work.slope_z_dir = (ushort)targetPlayer.gmk_work3;
                truck_work.slope_f_z_dir = (ushort)(MTM_MATH_ABS(targetPlayer.gmk_work3) >> 2);
                truck_work.slope_f_y_dir = (ushort)(targetPlayer.gmk_work3 >> 2);
                nnMakeUnitMatrix(nnsMatrix);
                nnTranslateMatrix(nnsMatrix, nnsMatrix, -x, -y, -z);
                nnRotateXMatrix(nnsMatrix, nnsMatrix, truck_work.slope_z_dir);
                nnRotateYMatrix(nnsMatrix, nnsMatrix, truck_work.slope_f_y_dir);
                nnRotateZMatrix(nnsMatrix, nnsMatrix, truck_work.slope_f_z_dir);
                nnTranslateMatrix(nnsMatrix, nnsMatrix, x, y, z);
                nnMultiplyMatrix(obj_work.obj_3d.user_obj_mtx_r, obj_work.obj_3d.user_obj_mtx_r, nnsMatrix);
                GlobalPool<NNS_MATRIX>.Release(nnsMatrix);
            }
            if (((int)targetPlayer.obj_work.move_flag & 1) != 0 && MTM_MATH_ABS(targetPlayer.obj_work.spd_m) >= GMD_GMK_TRUCK_SPARK_EFCT_SMALL_MIN_SPD && (truck_work.efct_f_spark == null || truck_work.efct_b_spark == null))
                gmGmkTruckCreateSparkEfct(truck_work, gmGmkTruckSparkEfctIdx(targetPlayer.obj_work.spd_m));
            if (truck_work.h_snd_lorry.au_player.sound == null || truck_work.h_snd_lorry.au_player.sound[0] == null)
            {
                truck_work.h_snd_lorry = GsSoundAllocSeHandle();
                truck_work.h_snd_lorry.au_player.SetAisac("Speed", 0.0f);
                GmSoundPlaySEForce("Lorry", truck_work.h_snd_lorry, true);
            }
            gmGmkTruckSetMoveSeParam(obj_work, truck_work.h_snd_lorry, targetPlayer, ((int)targetPlayer.player_flag & 16777216) != 0 ? 1 : 0);
        }
    }

    public static void gmGmkTruckInitFree(
      OBS_OBJECT_WORK obj_work,
      GMS_PLAYER_WORK ply_work)
    {
        ((GMS_GMK_TRUCK_WORK)obj_work).target_player = null;
        uint num1;
        uint num2;
        if (ply_work != null)
        {
            obj_work.spd = ply_work.obj_work.spd;
            obj_work.spd_m = ply_work.obj_work.spd_m;
            num1 = ply_work.obj_work.flag;
            num2 = ply_work.obj_work.move_flag;
        }
        else
        {
            obj_work.spd.x = 0;
            obj_work.spd.y = 0;
            obj_work.spd.z = 0;
            num1 = 0U;
            num2 = 0U;
        }
        obj_work.flag &= 4294967294U;
        obj_work.flag |= (uint)(2 | (int)num1 & 1);
        obj_work.flag &= 4294967279U;
        obj_work.move_flag |= 192U;
        obj_work.move_flag &= 4294401791U;
        if (((int)num2 & 16) != 0)
        {
            obj_work.move_flag |= 16U;
        }
        else
        {
            if (obj_work.spd.x > obj_work.spd_m)
                obj_work.spd_m = obj_work.spd.x;
            obj_work.spd.x = 0;
            if (obj_work.obj_3d.act_id[0] != 0)
            {
                ObjDrawObjectActionSet3DNNBlend(obj_work, 0);
                obj_work.disp_flag |= 4U;
            }
        }
        obj_work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkTruckFreeMain);
    }

    public static void gmGmkTruckFreeMain(OBS_OBJECT_WORK obj_work)
    {
        GMS_GMK_TRUCK_WORK truck_work = (GMS_GMK_TRUCK_WORK)obj_work;
        if (((int)obj_work.move_flag & 1) != 0 && ((int)obj_work.move_flag & 16) != 0)
        {
            if (obj_work.spd.x > obj_work.spd_m)
                obj_work.spd_m = obj_work.spd.x;
            obj_work.spd.x = 0;
            obj_work.move_flag &= 4294967279U;
            if (obj_work.obj_3d.act_id[0] != 0)
            {
                ObjDrawObjectActionSet3DNNBlend(obj_work, 0);
                obj_work.disp_flag |= 4U;
            }
            GmSoundPlaySE("Lorry4");
        }
        if (((int)obj_work.move_flag & 1) != 0)
            obj_work.spd_m = ObjSpdUpSet(obj_work.spd_m, 128, 40960);
        else
            obj_work.spd.x = ObjSpdUpSet(obj_work.spd.x, 128, 40960);
        truck_work.tire_dir_spd = ((int)obj_work.move_flag & 1) == 0 ? ObjSpdDownSet(truck_work.tire_dir_spd, 128) : obj_work.spd_m;
        truck_work.tire_dir += (ushort)FX_Div(truck_work.tire_dir_spd, 16384);
        if (((int)obj_work.move_flag & 1) != 0 && MTM_MATH_ABS(obj_work.spd_m) >= GMD_GMK_TRUCK_SPARK_EFCT_SMALL_MIN_SPD && (truck_work.efct_f_spark == null || truck_work.efct_b_spark == null))
            gmGmkTruckCreateSparkEfct(truck_work, gmGmkTruckSparkEfctIdx(obj_work.spd_m));
        gmGmkTruckSetMoveSeParam(obj_work, truck_work.h_snd_lorry, null, 1);
    }

    public static void gmGmkTruckInitDeathFall(
      OBS_OBJECT_WORK obj_work,
      GMS_PLAYER_WORK ply_work)
    {
        ((GMS_GMK_TRUCK_WORK)obj_work).target_player = null;
        if (ply_work != null)
        {
            obj_work.spd = ply_work.obj_work.spd;
            obj_work.spd_m = ply_work.obj_work.spd_m;
        }
        else
        {
            obj_work.spd.x = 0;
            obj_work.spd.y = 0;
            obj_work.spd.z = 0;
        }
        ObjObjectSpdDirFall(ref obj_work.spd.x, ref obj_work.spd.y, g_gm_main_system.pseudofall_dir);
        obj_work.spd.x = FX_Mul(obj_work.spd.x, 4608);
        obj_work.spd.y = FX_Mul(obj_work.spd.y, 4608);
        obj_work.spd_add.x = 0;
        obj_work.spd_add.y = obj_work.spd_fall;
        ObjObjectSpdDirFall(ref obj_work.spd_add.x, ref obj_work.spd_add.y, g_gm_main_system.pseudofall_dir);
        obj_work.flag |= 2U;
        obj_work.flag &= 4294967279U;
        obj_work.move_flag |= 272U;
        obj_work.move_flag &= 4294958975U;
        obj_work.pos.z = 983040;
        obj_work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkTruckDeathFallMain);
    }

    public static void gmGmkTruckDeathFallMain(OBS_OBJECT_WORK obj_work)
    {
        obj_work.dir.z += 1024;
    }

    public static void gmGmkTruckDispFunc(OBS_OBJECT_WORK obj_work)
    {
        GMS_GMK_TRUCK_WORK gmsGmkTruckWork = (GMS_GMK_TRUCK_WORK)obj_work;
        ObjDrawActionSummary(obj_work);
        VecU16 dir = obj_work.dir;
        uint p_disp_flag = obj_work.disp_flag | 16777216U;
        nnMakeRotateXYZMatrix(gmsGmkTruckWork.obj_3d_tire.user_obj_mtx_r, gmsGmkTruckWork.tire_dir, gmsGmkTruckWork.slope_f_y_dir, gmsGmkTruckWork.slope_f_z_dir);
        VecFx32 vecFx32;
        vecFx32.x = FXM_FLOAT_TO_FX32(gmsGmkTruckWork.tire_pos_f.M03);
        vecFx32.y = FXM_FLOAT_TO_FX32(-gmsGmkTruckWork.tire_pos_f.M13);
        vecFx32.z = FXM_FLOAT_TO_FX32(gmsGmkTruckWork.tire_pos_f.M23);
        ObjDrawAction3DNN(gmsGmkTruckWork.obj_3d_tire, new VecFx32?(vecFx32), new AppMain.VecU16?(dir), obj_work.scale, ref p_disp_flag);
        vecFx32.x = FXM_FLOAT_TO_FX32(gmsGmkTruckWork.tire_pos_b.M03);
        vecFx32.y = FXM_FLOAT_TO_FX32(-gmsGmkTruckWork.tire_pos_b.M13);
        vecFx32.z = FXM_FLOAT_TO_FX32(gmsGmkTruckWork.tire_pos_b.M23);
        ObjDrawAction3DNN(gmsGmkTruckWork.obj_3d_tire, new VecFx32?(vecFx32), new AppMain.VecU16?(dir), obj_work.scale, ref p_disp_flag);
    }

    public static void gmGmkTruckBodyDefFunc(
      OBS_RECT_WORK mine_rect,
      OBS_RECT_WORK match_rect)
    {
        GMS_ENEMY_COM_WORK parentObj1 = (GMS_ENEMY_COM_WORK)mine_rect.parent_obj;
        GMS_PLAYER_WORK parentObj2 = (GMS_PLAYER_WORK)match_rect.parent_obj;
        if (parentObj1 == null || parentObj2 == null || parentObj2.obj_work.obj_type != 1)
            return;
        GMS_GMK_TRUCK_WORK gmsGmkTruckWork = (GMS_GMK_TRUCK_WORK)parentObj1;
        parentObj1.obj_work.flag |= 16U;
        GmPlayerSetTruckRide(parentObj2, parentObj1.obj_work, parentObj1.obj_work.field_rect[0], parentObj1.obj_work.field_rect[1], parentObj1.obj_work.field_rect[2], parentObj1.obj_work.field_rect[3]);
        gmsGmkTruckWork.target_player = parentObj2;
        gmGmkTruckInitMain(parentObj1.obj_work, parentObj2);
    }

    // PC-only, the nose sensor against a rail end (298) or one of its blocker columns (299). The
    // rail end's own ppHit runs first in the same pair and sinks its platform
    private static void gmGmkTruckRailEndDefFunc(
      OBS_RECT_WORK mine_rect,
      OBS_RECT_WORK match_rect)
    {
        OBS_OBJECT_WORK mine_obj = mine_rect.parent_obj;
        OBS_OBJECT_WORK match_obj = match_rect.parent_obj;
        if (mine_obj == null || match_obj == null || match_obj.obj_type != 3)
            return;

        GMS_ENEMY_COM_WORK match_enemy = (GMS_ENEMY_COM_WORK)match_obj;
        if (match_enemy.eve_rec == null)
            return;

        int id = match_enemy.eve_rec.id;
        if (id != GMD_EVENT_ID_GMK_RAIL_END && id != GMD_EVENT_ID_GMK_RAIL_END_AREA)
            return;

        GMS_GMK_TRUCK_WORK truck_work = (GMS_GMK_TRUCK_WORK)mine_obj;
        GMS_PLAYER_WORK ply_work = truck_work.target_player;
        if (ply_work == null)
            return;

        GmPlayerSetEndTruckRide(ply_work);
        if (id == GMD_EVENT_ID_GMK_RAIL_END)
        {
            gmGmkTruckInitGetOff(mine_obj, false);
            GmPlySeqInitTruckGetOff(ply_work, GMD_GMK_TRUCK_GETOFF_JUMP);
            return;
        }

        gmGmkTruckInitGetOff(mine_obj, true);
        int spd_y = ply_work.obj_work.spd.y + GMD_GMK_TRUCK_GETOFF_JUMP;
        if (spd_y < GMD_GMK_TRUCK_GETOFF_JUMP_MAX)
            spd_y = GMD_GMK_TRUCK_GETOFF_JUMP_MAX;
        GmPlySeqInitTruckGetOff(ply_work, spd_y);
    }

    private static void gmGmkTruckInitGetOff(OBS_OBJECT_WORK obj_work, bool is_area)
    {
        GMS_GMK_TRUCK_WORK truck_work = (GMS_GMK_TRUCK_WORK)obj_work;
        gmGmkTruckInitFree(obj_work, truck_work.target_player);
        obj_work.flag = obj_work.flag & 4294967277U | 2U;
        if (is_area)
        {
            obj_work.move_flag = obj_work.move_flag & 4294966896U | 384U;
            if (obj_work.spd.y < 0)
                obj_work.spd.y >>= 1;
            ObjDrawObjectActionSet3DNNBlend(obj_work, GMD_GMK_TRUCK_ACT_ST);
        }
        else
        {
            obj_work.move_flag &= ~OBD_MOVE_NOCOL;
            ObjObjectFieldRectSet(obj_work, -32, -21, 32, 19);
        }

        obj_work.view_out_ofst = 64;
        obj_work.user_timer = GMD_GMK_TRUCK_GETOFF_TIME;
        if (truck_work.h_snd_lorry != null)
        {
            GmSoundStopSE(truck_work.h_snd_lorry);
            GsSoundFreeSeHandle(truck_work.h_snd_lorry);
            truck_work.h_snd_lorry = null;
        }

        GmSoundPlaySE("Lorry6");
        obj_work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkTruckGetOffMain);
    }

    private static void gmGmkTruckGetOffMain(OBS_OBJECT_WORK obj_work)
    {
        GMS_ENEMY_3D_WORK enemy = (GMS_ENEMY_3D_WORK)obj_work;
        GMS_GMK_TRUCK_WORK truck_work = (GMS_GMK_TRUCK_WORK)obj_work;
        if (obj_work.user_timer > 0 && --obj_work.user_timer <= 0)
            enemy.ene_com.col_work.obj_col.obj = obj_work;

        if ((obj_work.move_flag & OBD_MOVE_NOMOVE) != 0U)
            return;

        obj_work.spd.x = ObjSpdDownSet(obj_work.spd.x, GMD_GMK_TRUCK_GETOFF_BRAKE);
        obj_work.spd_m = ObjSpdDownSet(obj_work.spd_m, GMD_GMK_TRUCK_GETOFF_BRAKE);
        if ((obj_work.move_flag & OBD_MOVE_NOCOL) != 0U || obj_work.spd.x != 0 || obj_work.spd_m != 0)
            return;

        obj_work.move_flag |= OBD_MOVE_NOMOVE | OBD_MOVE_NOCOL;
        obj_work.spd.x = 0;
        obj_work.spd.y = 0;
        obj_work.spd.z = 0;
        obj_work.spd_add.x = 0;
        obj_work.spd_add.y = 0;
        obj_work.spd_add.z = 0;
        obj_work.spd_m = 0;
        ObjDrawObjectActionSet3DNNBlend(obj_work, GMD_GMK_TRUCK_ACT_ST);
        obj_work.disp_flag |= OBD_DISP_USERMTX_RIGHT;
        obj_work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkTruckGetOffEndMain);
        truck_work.rock_timer = 0;
    }

    // the empty cart settling: sixteen frames of rock about X, pivoted eight units above the origin
    private static void gmGmkTruckGetOffEndMain(OBS_OBJECT_WORK obj_work)
    {
        GMS_ENEMY_3D_WORK enemy = (GMS_ENEMY_3D_WORK)obj_work;
        GMS_GMK_TRUCK_WORK truck_work = (GMS_GMK_TRUCK_WORK)obj_work;
        if (obj_work.user_timer > 0 && --obj_work.user_timer <= 0)
            enemy.ene_com.col_work.obj_col.obj = obj_work;

        int t = ++truck_work.rock_timer;
        if (t > GMD_GMK_TRUCK_ROCK_FRAMES)
        {
            obj_work.disp_flag &= ~OBD_DISP_USERMTX_RIGHT;
            obj_work.ppFunc = null;
            enemy.ene_com.col_work.obj_col.obj = obj_work;
            return;
        }

        int dir_x = t >= GMD_GMK_TRUCK_ROCK_FRAMES / 2
            ? GMD_GMK_TRUCK_ROCK_STEP * (GMD_GMK_TRUCK_ROCK_FRAMES - t)
            : GMD_GMK_TRUCK_ROCK_STEP * t;
        if (dir_x < 0)
            dir_x = 0;

        NNS_MATRIX mtx = obj_work.obj_3d.user_obj_mtx_r;
        nnMakeUnitMatrix(mtx);
        nnTranslateMatrix(mtx, mtx, 0.0f, -8f, 0.0f);
        nnRotateXMatrix(mtx, mtx, (ushort)dir_x);
        nnTranslateMatrix(mtx, mtx, 0.0f, 8f, 0.0f);
    }

    public static void gmGmkTruckMotionCallback(
      AMS_MOTION motion,
      NNS_OBJECT _object,
      object param)
    {
        NNS_MATRIX nnsMatrix1 = GlobalPool<NNS_MATRIX>.Alloc();
        NNS_MATRIX nnsMatrix2 = GlobalPool<NNS_MATRIX>.Alloc();
        GMS_GMK_TRUCK_WORK gmsGmkTruckWork = (GMS_GMK_TRUCK_WORK)param;
        nnMakeUnitMatrix(nnsMatrix2);
        nnMultiplyMatrix(nnsMatrix2, nnsMatrix2, amMatrixGetCurrent());
        nnCalcNodeMatrixTRSList(nnsMatrix1, _object, GMD_GMK_TRUCK_NODE_ID_TIRE_POS_F, motion.data, nnsMatrix2);
        gmsGmkTruckWork.tire_pos_f.Assign(nnsMatrix1);
        nnCalcNodeMatrixTRSList(nnsMatrix1, _object, GMD_GMK_TRUCK_NODE_ID_TIRE_POS_B, motion.data, nnsMatrix2);
        gmsGmkTruckWork.tire_pos_b.Assign(nnsMatrix1);
        nnCalcNodeMatrixTRSList(nnsMatrix1, _object, GMD_GMK_TRUCK_NODE_ID_LIGHT_POS, motion.data, nnsMatrix2);
        gmsGmkTruckWork.light_pos.Assign(nnsMatrix1);
    }

    public static void gmGmkTruckSetMoveSeParam(
      OBS_OBJECT_WORK obj_work,
      GSS_SND_SE_HANDLE h_snd,
      GMS_PLAYER_WORK ply_work,
      int b_goal)
    {
        float val = 0.0f;
        if (h_snd == null)
            return;
        OBS_OBJECT_WORK obsObjectWork = ply_work == null ? obj_work : ply_work.obj_work;
        int num1 = MTM_MATH_ABS(obsObjectWork.spd_m);
        if (((int)obsObjectWork.move_flag & 1) != 0 && num1 >= GMD_GMK_TRUCK_SE_MIN_SPD)
        {
            if (num1 >= GMD_GMK_TRUCK_SE_MAX_SPD)
            {
                val = 1f;
            }
            else
            {
                val = FXM_FX32_TO_FLOAT(FX_Div(num1 - GMD_GMK_TRUCK_SE_MIN_SPD, GMD_GMK_TRUCK_SE_MAX_SPD - GMD_GMK_TRUCK_SE_MIN_SPD));
                if (val > 1.0)
                    val = 1f;
            }
        }
        h_snd.au_player.SetAisac("Speed", val);
        if (b_goal == 0)
            return;
        OBS_CAMERA obsCamera = ObjCameraGet(g_obj.glb_camera_id);
        float num2 = FXM_FX32_TO_FLOAT(obsObjectWork.pos.x) - obsCamera.disp_pos.x;
        float num3 = FXM_FX32_TO_FLOAT(obsObjectWork.pos.y) - -obsCamera.disp_pos.y;
        float num4;
        if (num2 < (double)GMD_GMK_TRUCK_SE_GOAL_MAX_DIST && num3 < (double)GMD_GMK_TRUCK_SE_GOAL_MAX_DIST)
        {
            float num5 = (float)(num2 * (double)num2 + num3 * (double)num3);
            if (num5 <= GMD_GMK_TRUCK_SE_GOAL_MIN_DIST * (double)GMD_GMK_TRUCK_SE_GOAL_MIN_DIST)
                num4 = 1f;
            else if (num5 <= GMD_GMK_TRUCK_SE_GOAL_MAX_DIST * (double)GMD_GMK_TRUCK_SE_GOAL_MAX_DIST)
            {
                num4 = (float)((GMD_GMK_TRUCK_SE_GOAL_MAX_DIST * (double)GMD_GMK_TRUCK_SE_GOAL_MAX_DIST - num5) / ((GMD_GMK_TRUCK_SE_GOAL_MAX_DIST - (double)GMD_GMK_TRUCK_SE_GOAL_MIN_DIST) * (GMD_GMK_TRUCK_SE_GOAL_MAX_DIST - (double)GMD_GMK_TRUCK_SE_GOAL_MIN_DIST)));
                if (num4 > 1.0)
                    num4 = 1f;
                else if (num4 < 0.0)
                    num4 = 0.0f;
            }
            else
                num4 = 0.0f;
        }
        else
            num4 = 0.0f;
        h_snd.snd_ctrl_param.volume = num4;
    }

    public static void gmGmkTGravityChangeDefFunc(
      OBS_RECT_WORK mine_rect,
      OBS_RECT_WORK match_rect)
    {
        GMS_ENEMY_COM_WORK parentObj1 = (GMS_ENEMY_COM_WORK)mine_rect.parent_obj;
        GMS_PLAYER_WORK parentObj2 = (GMS_PLAYER_WORK)match_rect.parent_obj;
        if (parentObj1 == null || parentObj2 == null || parentObj2.obj_work.obj_type != 1 || ((parentObj1.eve_rec.flag & GMD_GMK_T_GRAVITY_A) != 0 && ((int)parentObj2.obj_work.flag & 1) != 0 || (parentObj1.eve_rec.flag & GMD_GMK_T_GRAVITY_B) != 0 && ((int)parentObj2.obj_work.flag & 1) == 0))
            return;
        ushort num1;
        if (239 <= parentObj1.eve_rec.id && parentObj1.eve_rec.id <= 246)
        {
            int num2;
            int num3;
            int num4;
            if (239 <= parentObj1.eve_rec.id && parentObj1.eve_rec.id <= 242)
            {
                num2 = parentObj1.eve_rec.id - 239;
                num3 = (num2 & 2) == 0 ? parentObj1.obj_work.pos.x - 524288 : parentObj1.obj_work.pos.x + 524288;
                num4 = (num2 + 1 & 2) == 0 ? parentObj1.obj_work.pos.y + 524288 : parentObj1.obj_work.pos.y - 524288;
                parentObj2.gmk_flag2 |= 8U;
            }
            else
            {
                num2 = parentObj1.eve_rec.id - 243;
                num3 = (num2 & 2) == 0 ? parentObj1.obj_work.pos.x + 524288 : parentObj1.obj_work.pos.x - 524288;
                num4 = (num2 + 1 & 2) == 0 ? parentObj1.obj_work.pos.y - 524288 : parentObj1.obj_work.pos.y + 524288;
            }
            float num5 = FXM_FX32_TO_FLOAT(num3 - parentObj2.obj_work.pos.x);
            float num6 = FXM_FX32_TO_FLOAT(num4 - parentObj2.obj_work.pos.y);
            if (239 <= parentObj1.eve_rec.id && parentObj1.eve_rec.id <= 242)
            {
                if ((num2 & 2) != 0)
                {
                    if (num5 < 0.0)
                        return;
                }
                else if (num5 > 0.0)
                    return;
                if ((num2 + 1 & 2) != 0)
                {
                    if (num6 > 0.0)
                        return;
                }
                else if (num6 < 0.0)
                    return;
            }
            else
            {
                if ((num2 & 2) != 0)
                {
                    if (num5 > 0.0)
                        return;
                }
                else if (num5 < 0.0)
                    return;
                if ((num2 + 1 & 2) != 0)
                {
                    if (num6 < 0.0)
                        return;
                }
                else if (num6 > 0.0)
                    return;
            }
            num1 = (ushort)(65536U - (ushort)(nnArcTan2(-num6, num5) - 16384));
            if (239 <= parentObj1.eve_rec.id && parentObj1.eve_rec.id <= 242)
                num1 -= 32768;
        }
        else
            num1 = gm_gmk_t_gravity_flat_dir_tbl[parentObj1.eve_rec.id - 223];
        if (parentObj2.jump_pseudofall_eve_id_cur != parentObj1.eve_rec.id)
        {
            parentObj2.jump_pseudofall_eve_id_wait = parentObj1.eve_rec.id;
        }
        else
        {
            if ((parentObj1.eve_rec.flag & GMD_GMK_T_CLEAR_PSEUDOFALL_DIR_FIX) != 0)
                parentObj2.gmk_flag &= 4278190079U;
            if (((int)parentObj2.gmk_flag & 16777216) != 0)
                return;
            ObjObjectSpdDirFall(ref parentObj2.obj_work.spd.x, ref parentObj2.obj_work.spd.y, (ushort)-(num1 - parentObj2.jump_pseudofall_dir));
            parentObj2.jump_pseudofall_dir = num1;
            parentObj2.jump_pseudofall_eve_id_set = parentObj1.eve_rec.id;
        }
    }

    public static void gmGmkTGravityForceChangeDefFunc(
      OBS_RECT_WORK mine_rect,
      OBS_RECT_WORK match_rect)
    {
        GMS_ENEMY_COM_WORK parentObj1 = (GMS_ENEMY_COM_WORK)mine_rect.parent_obj;
        GMS_PLAYER_WORK parentObj2 = (GMS_PLAYER_WORK)match_rect.parent_obj;
        if (parentObj1 == null || parentObj2 == null || (parentObj2.obj_work.obj_type != 1 || ((int)parentObj2.obj_work.move_flag & 1) != 0) || ((parentObj1.eve_rec.flag & GMD_GMK_T_GRAVITY_A) != 0 && ((int)parentObj2.obj_work.flag & 1) != 0 || (parentObj1.eve_rec.flag & GMD_GMK_T_GRAVITY_B) != 0 && ((int)parentObj2.obj_work.flag & 1) == 0))
            return;
        int num1 = (parentObj1.eve_rec.id - 268) * 16384;
        ushort num2 = (ushort)((uint)num1 - parentObj2.jump_pseudofall_dir);
        parentObj2.jump_pseudofall_dir = (ushort)num1;
        int a = num1 - parentObj2.ply_pseudofall_dir;
        if ((ushort)MTM_MATH_ABS(a) > 32768)
        {
            if (a < 0)
                parentObj2.ply_pseudofall_dir += 65536 + a;
            else
                parentObj2.ply_pseudofall_dir += a - 65536;
        }
        else
            parentObj2.ply_pseudofall_dir = num1;
        ObjObjectSpdDirFall(ref parentObj2.obj_work.spd.x, ref parentObj2.obj_work.spd.y, (ushort)-num2);
        parentObj2.gmk_flag |= 16777216U;
    }

    public static void gmGmkTNoLandingDefFunc(
      OBS_RECT_WORK mine_rect,
      OBS_RECT_WORK match_rect)
    {
        GMS_ENEMY_COM_WORK parentObj1 = (GMS_ENEMY_COM_WORK)mine_rect.parent_obj;
        GMS_PLAYER_WORK parentObj2 = (GMS_PLAYER_WORK)match_rect.parent_obj;
        if (parentObj1 == null || parentObj2 == null || parentObj2.obj_work.obj_type != 1)
            return;
        parentObj2.obj_work.sys_flag |= (uint)(1 << parentObj1.eve_rec.id - 264);
    }

    public static void gmGmkTruckCreateLightEfct(GMS_GMK_TRUCK_WORK truck_work)
    {
        GMS_EFFECT_3DES_WORK gmsEffect3DesWork = GmEfctZoneEsCreate((OBS_OBJECT_WORK)truck_work, 2, 10);
        gmsEffect3DesWork.efct_com.obj_work.user_work_OBJECT = truck_work.light_pos;
        gmsEffect3DesWork.efct_com.obj_work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkTruckLightEfctMain);
        gmsEffect3DesWork.efct_com.obj_work.ppOut = new MPP_VOID_OBS_OBJECT_WORK(gmGmkTruckLightEfctDispFunc);
    }

    public static void gmGmkTruckLightEfctMain(OBS_OBJECT_WORK obj_work)
    {
        if (obj_work.parent_obj == null)
        {
            obj_work.flag |= 4U;
        }
        else
        {
            GMS_GMK_TRUCK_WORK parentObj = (GMS_GMK_TRUCK_WORK)obj_work.parent_obj;
            obj_work.dir.z = (ushort)(obj_work.parent_obj.dir.z + (uint)parentObj.slope_z_dir);
            if (((int)obj_work.disp_flag & 8) == 0)
                return;
            obj_work.flag |= 4U;
        }
    }

    public static void gmGmkTruckLightEfctDispFunc(OBS_OBJECT_WORK obj_work)
    {
        NNS_MATRIX userWorkObject = (NNS_MATRIX)obj_work.user_work_OBJECT;
        if (obj_work.parent_obj == null)
            return;
        obj_work.pos.x = FXM_FLOAT_TO_FX32(userWorkObject.M03);
        obj_work.pos.y = FXM_FLOAT_TO_FX32(-userWorkObject.M13);
        obj_work.pos.z = FXM_FLOAT_TO_FX32(userWorkObject.M23);
        ObjDrawActionSummary(obj_work);
    }

    private static int gmGmkTruckSparkEfctIdx(int spd_m)
    {
        return gm_gmk_as_console && MTM_MATH_ABS(spd_m) >= GMD_GMK_TRUCK_SPARK_EFCT_BIG_MIN_SPD
            ? GME_EFCT_Z03_IDX_SPARK
            : GME_EFCT_Z03_IDX_SPARK_S;
    }

    public static void gmGmkTruckCreateSparkEfct(GMS_GMK_TRUCK_WORK truck_work, int efct_type)
    {
        if (truck_work.efct_f_spark == null)
        {
            truck_work.efct_f_spark = GmEfctZoneEsCreate((OBS_OBJECT_WORK)truck_work, 2, efct_type);
            truck_work.efct_f_spark.efct_com.obj_work.flag |= 524304U;
            truck_work.efct_f_spark.efct_com.obj_work.user_work_OBJECT = truck_work.tire_pos_f;
            truck_work.efct_f_spark.efct_com.obj_work.user_timer = efct_type;
            truck_work.efct_f_spark.efct_com.obj_work.user_flag = 0U;
            truck_work.efct_f_spark.efct_com.obj_work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkTruckSparkEfctMain);
            truck_work.efct_f_spark.efct_com.obj_work.ppOut = new MPP_VOID_OBS_OBJECT_WORK(gmGmkTruckSparkEfctDispFunc);
        }
        if (truck_work.efct_b_spark != null)
            return;
        truck_work.efct_b_spark = GmEfctZoneEsCreate((OBS_OBJECT_WORK)truck_work, 2, efct_type);
        truck_work.efct_b_spark.efct_com.obj_work.flag |= 524304U;
        truck_work.efct_b_spark.efct_com.obj_work.user_work_OBJECT = truck_work.tire_pos_b;
        truck_work.efct_b_spark.efct_com.obj_work.user_timer = efct_type;
        truck_work.efct_b_spark.efct_com.obj_work.user_flag = 1U;
        truck_work.efct_b_spark.efct_com.obj_work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkTruckSparkEfctMain);
        truck_work.efct_b_spark.efct_com.obj_work.ppOut = new MPP_VOID_OBS_OBJECT_WORK(gmGmkTruckSparkEfctDispFunc);
    }

    public static void gmGmkTruckSparkEfctMain(OBS_OBJECT_WORK obj_work)
    {
        GMS_GMK_TRUCK_WORK parentObj = (GMS_GMK_TRUCK_WORK)obj_work.parent_obj;
        if (obj_work.parent_obj == null)
        {
            obj_work.flag |= 4U;
        }
        else
        {
            OBS_OBJECT_WORK obsObjectWork = (OBS_OBJECT_WORK)parentObj.target_player ?? (OBS_OBJECT_WORK)parentObj;
            uint dispFlag = obj_work.disp_flag;
            if (((int)obsObjectWork.move_flag & 1) == 0)
            {
                obj_work.disp_flag |= 32U;
            }
            else
            {
                obj_work.disp_flag &= 4294967263U;

                // gmGmkTruck.cpp:2003-2067. Console has two spark sizes and swaps between them at
                // BIG_MIN_SPD: the running one ends and creates the other. Mobile only ever has the
                // small one, so it ends on the low speed alone
                int spd = MTM_MATH_ABS(obsObjectWork.spd_m);
                bool is_big = gm_gmk_as_console && obj_work.user_timer == GME_EFCT_Z03_IDX_SPARK;
                bool over_big = gm_gmk_as_console && spd >= GMD_GMK_TRUCK_SPARK_EFCT_BIG_MIN_SPD;

                if (is_big ? !over_big : over_big || spd < GMD_GMK_TRUCK_SPARK_EFCT_SMALL_MIN_SPD)
                {
                    if (((int)dispFlag & 32) != 0)
                    {
                        obj_work.flag |= 8U;
                    }
                    else
                    {
                        ObjDrawKillAction3DES(obj_work);
                        obj_work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(GmEffectDefaultMainFuncDeleteAtEnd);
                    }
                    if (obj_work.user_flag == 0U)
                        parentObj.efct_f_spark = null;
                    else
                        parentObj.efct_b_spark = null;

                    if (is_big)
                        gmGmkTruckCreateSparkEfct(parentObj, GME_EFCT_Z03_IDX_SPARK_S);
                    else if (over_big)
                        gmGmkTruckCreateSparkEfct(parentObj, GME_EFCT_Z03_IDX_SPARK);
                }
                GmEffectDefaultMainFuncDeleteAtEnd(obj_work);
            }
        }
    }

    public static void gmGmkTruckSparkEfctDispFunc(OBS_OBJECT_WORK obj_work)
    {
        GMS_GMK_TRUCK_WORK parentObj = (GMS_GMK_TRUCK_WORK)obj_work.parent_obj;
        NNS_MATRIX userWorkObject = (NNS_MATRIX)obj_work.user_work_OBJECT;
        NNS_VECTOR nnsVector = GlobalPool<NNS_VECTOR>.Alloc();
        if (obj_work.parent_obj == null)
        {
            GlobalPool<NNS_VECTOR>.Release(nnsVector);
        }
        else
        {
            OBS_OBJECT_WORK obsObjectWork = (OBS_OBJECT_WORK)parentObj.target_player ?? (OBS_OBJECT_WORK)parentObj;
            VecFx32 vecFx32;
            vecFx32.x = FXM_FLOAT_TO_FX32(userWorkObject.M03);
            vecFx32.y = FXM_FLOAT_TO_FX32(-userWorkObject.M13);
            vecFx32.z = FXM_FLOAT_TO_FX32(userWorkObject.M23);
            ushort dir_z;
            ushort num;
            if (obsObjectWork.spd_m >= 0)
            {
                obj_work.disp_flag &= 4294967294U;
                dir_z = (ushort)(8192U - obj_work.parent_obj.dir.z);
                num = (ushort)((uint)-obj_work.parent_obj.dir.z - 2048U);
            }
            else
            {
                obj_work.disp_flag |= 1U;
                dir_z = (ushort)(8192U + obj_work.parent_obj.dir.z);
                num = (ushort)(obj_work.parent_obj.dir.z + 2048U);
            }
            obj_work.pos = vecFx32;
            nnsVector.x = nnSin(num) * GMD_GMK_TRUCK_EFCT_SPRAK_OFST_DIST;
            nnsVector.y = nnCos(num) * GMD_GMK_TRUCK_EFCT_SPRAK_OFST_DIST;
            nnsVector.z = 0.0f;
            GmComEfctSetDispOffsetF((GMS_EFFECT_3DES_WORK)obj_work, nnsVector.x, nnsVector.y, nnsVector.z);
            GmComEfctSetDispRotation((GMS_EFFECT_3DES_WORK)obj_work, 0, 0, dir_z);
            ObjDrawActionSummary(obj_work);
            GlobalPool<NNS_VECTOR>.Release(nnsVector);
        }
    }

}