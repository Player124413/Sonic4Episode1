internal partial class AppMain
{
    private const string GMD_GMK_ANIMAL_MDL_FILENAME = "G_COM/GMK/GMK_ANIMAL_MDL.AMB";
    private const string GMD_GMK_ANIMAL_TEX_FILENAME = "G_COM/GMK/GMK_ANIMAL_TEX.AMB";
    private const string GMD_GMK_ANIMAL_MTN_FILENAME = "G_COM/GMK/GMK_ANIMAL_MTN.AMB";

    // the board arm's fixed billboard yaw, and the three the model arm turns to instead
    private const ushort GMD_GMK_ANIMAL_BOARD_DIR = 0xB000;
    private const ushort GMD_GMK_ANIMAL_DIR_RIGHT = unchecked((ushort)-GMD_GMK_ANIMAL_ANGLE);
    private const ushort GMD_GMK_ANIMAL_DIR_LEFT = 0x8000 + GMD_GMK_ANIMAL_ANGLE;
    private const ushort GMD_GMK_ANIMAL_DIR_FRONT = 0xC000;

    // console draws one skinned model per animal off GMK_ANIMAL_MTN; mobile has no motion at all
    // and swaps between four baked poses instead
    private static bool gm_gmk_animal_as_console;

    // indexed by GSE_GMK_ANM_TYPE (pig, bird, penguin, rabbit, chicken), into an archive ordered
    // pig, bird, cock, rabbit, pen
    internal static readonly int[] gm_gmk_animal_console_obj_id = { 0, 1, 4, 3, 2 };

    // footwork, rising, falling
    internal static readonly int[][] gm_gmk_animal_console_mtn_id =
    {
        new[] { 0, 1, 2 },
        new[] { 3, 4, 5 },
        new[] { 12, 13, 14 },
        new[] { 9, 10, 11 },
        new[] { 6, 7, 8 },
    };

    public static void GmGmkAnimalBuild()
    {
        gm_gmk_animal_as_console = amFsAllConsoleGrade(
            GMD_GMK_ANIMAL_MDL_FILENAME, GMD_GMK_ANIMAL_TEX_FILENAME, GMD_GMK_ANIMAL_MTN_FILENAME);

        gm_gmk_animal_obj_3d_list = GmGameDBuildRegBuildModel(GmGameDatGetGimmickData(870), GmGameDatGetGimmickData(871), 0U);
    }

    public static void GmGmkAnimalFlush()
    {
        AMS_AMB_HEADER amsAmbHeader = readAMBFile(GmGameDatGetGimmickData(870));
        GmGameDBuildRegFlushModel(gm_gmk_animal_obj_3d_list, amsAmbHeader.file_num);
    }

    public static OBS_OBJECT_WORK GmGmkAnimalInit(
      OBS_OBJECT_WORK parent_work,
      int ofs_x,
      int ofs_y,
      int ofs_z,
      byte type,
      byte vec,
      ushort timer)
    {
        OBS_OBJECT_WORK work = GMM_EFFECT_CREATE_WORK(() => new GMS_EFFECT_3DNN_WORK(), parent_work, 0, "GMK_ANIMAL");
        GMS_EFFECT_3DNN_WORK gmsEffect3DnnWork = (GMS_EFFECT_3DNN_WORK)work;
        work.view_out_ofst = 64;
        work.pos.x += ofs_x;
        work.pos.y += ofs_y;
        work.pos.z = ofs_z - 131072;
        type = type == 0 ? (byte)(mtMathRand() & 1U) : (byte)(type - 1 & 1);
        int index = g_gm_gamedat_zone_type_tbl[g_gs_main_sys_info.stage_id];
        work.user_work = g_gm_gmk_animal_type_id[index][type];
        work.user_flag = vec;
        work.user_timer = timer;
        gmGmkAnimalObjSet(work, gmsEffect3DnnWork.obj_3d);
        work.move_flag |= 16128U;
        work.move_flag &= 4294967167U;
        work.flag |= 512U;
        work.flag |= 2U;
        work.flag &= 4294967279U;
        work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkAnimalWait);
        return work;
    }

    private static void gmGmkAnimalObjSet(
      OBS_OBJECT_WORK obj_work,
      OBS_ACTION3D_NN_WORK dest_obj_3d)
    {
        if (gm_gmk_animal_as_console)
        {
            ObjObjectCopyAction3dNNModel(obj_work, gm_gmk_animal_obj_3d_list[gm_gmk_animal_console_obj_id[(int)obj_work.user_work]], dest_obj_3d);
            ObjObjectAction3dNNMotionLoad(obj_work, 0, true, ObjDataGet(872), null, 0, null);
            ObjDrawObjectSetToon(obj_work);
            ObjDrawObjectActionSet(obj_work, gm_gmk_animal_console_mtn_id[(int)obj_work.user_work][0]);
        }
        else
        {
            ObjObjectCopyAction3dNNModel(obj_work, gm_gmk_animal_obj_3d_list[(int)g_gm_gmk_animal_obj_id[(int)obj_work.user_work][0]], dest_obj_3d);
        }

        ObjObjectFieldRectSet(obj_work, -2, -8, 2, 0);
        obj_work.disp_flag |= 4259840U;
        if (gm_gmk_animal_as_console)
            obj_work.disp_flag |= 4U;
    }

    // console animates one model instead of swapping between poses, so the rising and falling
    // motions cross-fade every frame rather than only on landing
    private static void gmGmkAnimalUpdateMotion(OBS_OBJECT_WORK obj_work)
    {
        int[] mtn_id = gm_gmk_animal_console_mtn_id[(int)obj_work.user_work];
        int id = obj_work.spd.y < 0 ? mtn_id[1] : mtn_id[2];
        if (obj_work.obj_3d.act_id[0] == id)
            return;

        ObjDrawObjectActionSet3DNNBlend(obj_work, id);
        obj_work.disp_flag |= 4U;
    }

    private static void gmGmkAnimalWait(OBS_OBJECT_WORK obj_work)
    {
        if (obj_work.user_timer != 0)
        {
            --obj_work.user_timer;
        }
        else
        {
            if (!gm_gmk_animal_as_console)
            {
                ObjObjectAction3dNNModelReleaseCopy(obj_work);
                ObjObjectCopyAction3dNNModel(obj_work, gm_gmk_animal_obj_3d_list[(int)g_gm_gmk_animal_obj_id[(int)obj_work.user_work][1]], obj_work.obj_3d);
            }
            obj_work.move_flag &= 4294952703U;
            obj_work.move_flag |= 1680U;
            obj_work.spd.y = g_gm_gmk_animal_speed_param[(int)obj_work.user_work].jump;
            obj_work.spd_fall = g_gm_gmk_animal_speed_param[(int)obj_work.user_work].gravity;
            obj_work.pos.z = 131072;
            obj_work.ppFunc = new MPP_VOID_OBS_OBJECT_WORK(gmGmkAnimalJump);
        }
    }

    private static void gmGmkAnimalJump(OBS_OBJECT_WORK obj_work)
    {
        if (gm_gmk_animal_as_console)
            gmGmkAnimalUpdateMotion(obj_work);

        if (((int)obj_work.move_flag & 1) == 0)
            return;
        if (obj_work.user_flag != 0U)
        {
            obj_work.spd.x = g_gm_gmk_animal_speed_param[(int)obj_work.user_work].spd_x;
            if (gm_gmk_animal_as_console)
            {
                obj_work.dir.y = GMD_GMK_ANIMAL_DIR_RIGHT;
            }
            else
            {
                ObjObjectAction3dNNModelReleaseCopy(obj_work);
                ObjObjectCopyAction3dNNModel(obj_work, gm_gmk_animal_obj_3d_list[(int)g_gm_gmk_animal_obj_id[(int)obj_work.user_work][2]], obj_work.obj_3d);
                obj_work.dir.y = GMD_GMK_ANIMAL_BOARD_DIR;
            }
        }
        else
        {
            obj_work.spd.x = -g_gm_gmk_animal_speed_param[(int)obj_work.user_work].spd_x;
            if (gm_gmk_animal_as_console)
            {
                obj_work.dir.y = GMD_GMK_ANIMAL_DIR_LEFT;
            }
            else
            {
                ObjObjectAction3dNNModelReleaseCopy(obj_work);
                ObjObjectCopyAction3dNNModel(obj_work, gm_gmk_animal_obj_3d_list[(int)g_gm_gmk_animal_obj_id[(int)obj_work.user_work][3]], obj_work.obj_3d);
                obj_work.dir.y = GMD_GMK_ANIMAL_BOARD_DIR;
            }
        }
        obj_work.spd.y = g_gm_gmk_animal_speed_param[(int)obj_work.user_work].jump;
        obj_work.move_flag |= 16U;
        obj_work.disp_flag &= 4290772735U;
    }

    private static void gmGmkEndingAnimalMove(OBS_OBJECT_WORK obj_work)
    {
        if (gm_gmk_animal_as_console)
            gmGmkAnimalUpdateMotion(obj_work);

        if (((int)obj_work.move_flag & 1) == 0)
            return;
        if ((((GMS_ENEMY_COM_WORK)obj_work).eve_rec.flag & 24) == 0 || GmEndingAnimalForwardChk())
        {
            obj_work.spd.x = 0;
            if (gm_gmk_animal_as_console)
            {
                obj_work.dir.y = GMD_GMK_ANIMAL_DIR_FRONT;
            }
            else
            {
                ObjObjectAction3dNNModelReleaseCopy(obj_work);
                ObjObjectCopyAction3dNNModel(obj_work, gm_gmk_animal_obj_3d_list[(int)g_gm_gmk_animal_obj_id[(int)obj_work.user_work][1]], obj_work.obj_3d);
                obj_work.dir.y = GMD_GMK_ANIMAL_BOARD_DIR;
            }
        }
        else
        {
            obj_work.user_flag = (uint)((int)obj_work.user_flag + 1 & 3);
            if (((int)obj_work.user_flag & 2) != 0)
            {
                obj_work.spd.x = g_gm_gmk_animal_speed_param[(int)obj_work.user_work].spd_x;
                if (gm_gmk_animal_as_console)
                {
                    obj_work.dir.y = GMD_GMK_ANIMAL_DIR_RIGHT;
                }
                else
                {
                    ObjObjectAction3dNNModelReleaseCopy(obj_work);
                    ObjObjectCopyAction3dNNModel(obj_work, gm_gmk_animal_obj_3d_list[(int)g_gm_gmk_animal_obj_id[(int)obj_work.user_work][2]], obj_work.obj_3d);
                    obj_work.dir.y = GMD_GMK_ANIMAL_BOARD_DIR;
                }
            }
            else
            {
                obj_work.spd.x = -g_gm_gmk_animal_speed_param[(int)obj_work.user_work].spd_x;
                if (gm_gmk_animal_as_console)
                {
                    obj_work.dir.y = GMD_GMK_ANIMAL_DIR_LEFT;
                }
                else
                {
                    ObjObjectAction3dNNModelReleaseCopy(obj_work);
                    ObjObjectCopyAction3dNNModel(obj_work, gm_gmk_animal_obj_3d_list[(int)g_gm_gmk_animal_obj_id[(int)obj_work.user_work][3]], obj_work.obj_3d);
                    obj_work.dir.y = GMD_GMK_ANIMAL_BOARD_DIR;
                }
            }
        }
        obj_work.spd.y = g_gm_gmk_animal_speed_param[(int)obj_work.user_work].jump;
        obj_work.move_flag |= 16U;
        obj_work.disp_flag &= 4290772735U;
    }

}