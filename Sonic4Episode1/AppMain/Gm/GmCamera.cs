using System;

internal partial class AppMain
{
    private static void GmCameraInit()
    {
        NNS_VECTOR cam_pos = new NNS_VECTOR(177f, -1580.0f, 50f);
        ObjCameraInit(GME_CAMERA_NO_MAIN, cam_pos, GMD_TASK_GROUP_OBJSYS, GMD_TASK_PAUSE_LEVEL_CAMERA, GMD_TASK_PRIO_CAMERA);
        ObjCamera3dInit(GME_CAMERA_NO_MAIN);
        g_obj.glb_camera_id = GME_CAMERA_NO_MAIN;
        g_obj.glb_camera_type = NNE_PROJECTION_TYPE_ORTHO;  // 正射影
        GmCameraDelayReset();
        GmCameraAllowReset();
        ObjCameraSetUserFunc(GME_CAMERA_NO_MAIN, GmCameraFunc);	// ユーザー処理
        ObjCameraSetUserFunc(0, new OBJF_CAMERA_USER_FUNC(GmCameraFunc));
        OBS_CAMERA obsCamera1 = ObjCameraGet(0);
        obsCamera1.scale = GMD_CAMERA_SCALE;
        obsCamera1.ofst.z = 1000f;
        gm_camera_vibration.x = 0.0f;
        gm_camera_vibration.y = 0.0f;
        gm_camera_vibration.z = 0.0f;
        gm_camera_work.flag = 0;
        gm_camera_work.timer = 0;
        gm_camera_work.offset = 0;
        gm_camera_work.scale_now = 1f;
        gm_camera_work.scale_target = 1f;
        gm_camera_work.scale_spd = 0.1f;
        int num = g_gm_gamedat_zone_type_tbl[g_gs_main_sys_info.stage_id];
        switch (num)
        {
            case 0:
            case 1:
            case 2:
            case 4:
            case 5:
                ObjCameraInit(1, new NNS_VECTOR(0.0f, 0.0f, 60f), 4, 0, 8192);
                ObjCamera3dInit(1);
                ObjCameraSetUserFunc(1, new OBJF_CAMERA_USER_FUNC(gmCameraFuncMapFar));
                OBS_CAMERA obsCamera2 = ObjCameraGet(1);
                obsCamera2.command_state = 1U;
                obsCamera2.fovy = 2 != num ? NNM_DEGtoA32(40f) : NNM_DEGtoA32(40f);
                obsCamera2.znear = 0.1f;
                obsCamera2.zfar = 32768f;
                break;
        }
        NNS_VECTOR pos2 = new NNS_VECTOR(177f, -15800f, 50f);
        int[] numArray1 = new int[4] { 2, 3, 4, 5 };
        int[] numArray2 = new int[4] { 0, 4, 6, 8 };
        uint[] numArray3 = new uint[4] { 7U, 0U, 0U, 0U };
        for (int index = 0; index < 4; ++index)
        {
            if (g_gm_gamedat_map_set_add[numArray2[index]] != null)
            {
                ObjCameraInit(numArray1[index], pos2, 4, 0, 8192);
                ObjCamera3dInit(numArray1[index]);
                ObjCameraSetUserFunc(numArray1[index], new OBJF_CAMERA_USER_FUNC(gmCameraFuncAddMap));
                OBS_CAMERA obsCamera3 = ObjCameraGet(numArray1[index]);
                obsCamera3.scale = GMD_CAMERA_SCALE;
                obsCamera3.ofst.z = 1000f;
                obsCamera3.command_state = numArray3[index];
            }
        }
        ObjCameraInit(6, new NNS_VECTOR(cam_pos.x, cam_pos.y, cam_pos.z), 4, 0, 8192);
        ObjCamera3dInit(6);
        ObjCameraSetUserFunc(6, new OBJF_CAMERA_USER_FUNC(gmCameraFuncWater));
        OBS_CAMERA obsCamera4 = ObjCameraGet(6);
        obsCamera4.command_state = 9U;
        obsCamera4.scale = GMD_CAMERA_SCALE;
        obsCamera4.ofst.z = 1000f;

        // Camera 7, values read out of PC's camera init at 0x4C7780. It follows the gameplay
        // camera and differs only in projection: an 11.8 degree lens against camera 0's ortho,
        // which is what gives the card road its depth without the stage visibly warping. PC builds
        // it for every stage and so does this; index 7 is the last slot the camera array has
        ObjCameraInit(GME_CAMERA_NO_TRUMPROAD, new NNS_VECTOR(cam_pos.x, cam_pos.y, cam_pos.z), 4, 0, 8192);
        ObjCamera3dInit(GME_CAMERA_NO_TRUMPROAD);
        ObjCameraSetUserFunc(GME_CAMERA_NO_TRUMPROAD, new OBJF_CAMERA_USER_FUNC(gmCameraFuncTrumpRoad));
        OBS_CAMERA obsCamera5 = ObjCameraGet(GME_CAMERA_NO_TRUMPROAD);
        obsCamera5.command_state = OBD_DRAW_CMD_STATE_TRUMPROAD;
        obsCamera5.scale = GMD_CAMERA_SCALE;
        obsCamera5.ofst.z = 1000f;
        obsCamera5.fovy = GMD_CAMERA_TRUMPROAD_FOVY;
        obsCamera5.znear = 512f;
        obsCamera5.zfar = 65535f;
    }

    // 2144 in binary angle, 11.8 degrees
    private const ushort GMD_CAMERA_TRUMPROAD_FOVY = 2144;

    internal const int GME_CAMERA_NO_TRUMPROAD = 7;

    private static void gmCameraFuncTrumpRoad(OBS_CAMERA obj_camera)
    {
        OBS_CAMERA obsCamera = ObjCameraGet(GME_CAMERA_NO_MAIN);
        nnCopyVector(obj_camera.pos, obsCamera.pos);
        nnCopyVector(obj_camera.target_pos, obsCamera.target_pos);
        nnCopyVector(obj_camera.disp_pos, obsCamera.disp_pos);
    }

    private static void GmCameraExit()
    {
    }

    public static void GmCameraPosSet(int pos_x, int pos_y, int pos_z)
    {
        OBS_CAMERA obsCamera = ObjCameraGet(0);
        float lcd_width = (float)(OBD_LCD_X / 2);
        float lcd_height = (float)(OBD_LCD_Y / 2);
        float bottom = g_gm_main_system.map_fcol.bottom - (float)(OBD_LCD_Y / 2);
        obsCamera.pos.x = FXM_FX32_TO_FLOAT(pos_x);
        if (obsCamera.pos.x < (double)lcd_width)
            obsCamera.pos.x = lcd_width;
        obsCamera.pos.y = -FXM_FX32_TO_FLOAT(pos_y);
        if (obsCamera.pos.y > -(double)lcd_height)
            obsCamera.pos.y = -lcd_height;
        if (obsCamera.pos.y > (double)bottom)
            obsCamera.pos.y = bottom;
        obsCamera.pos.z = FXM_FX32_TO_FLOAT(pos_z) + 50f;
        obsCamera.disp_pos.x = obsCamera.pos.x;
        obsCamera.disp_pos.y = obsCamera.pos.y;
        obsCamera.disp_pos.z = obsCamera.pos.z;
    }

    private static void GmCameraVibrationSet(int vib_x, int vib_y, int vib_z)
    {
        gm_camera_vibration.x = FXM_FX32_TO_FLOAT(vib_x);
        gm_camera_vibration.y = FXM_FX32_TO_FLOAT(vib_y);
        gm_camera_vibration.z = FXM_FX32_TO_FLOAT(vib_z);
    }

    private static void GmCameraAllowSet(float alw_x, float alw_y, float alw_z)
    {
        gm_camera_option_allow_pos.x = alw_x;
        gm_camera_option_allow_pos.y = alw_y;
        gm_camera_option_allow_pos.z = alw_z;
        ObjCameraAllowSet(0, gm_camera_option_allow_pos);
    }

    private static void GmCameraAllowReset()
    {
        if (!GSM_MAIN_STAGE_IS_SPSTAGE())
            ObjCameraAllowSet(0, gm_camera_common_allow_pos);
        else
            ObjCameraAllowSet(0, gm_camera_splstg_allow_pos);
    }

    private static void GmCameraDelaySet(float dly_x, float dly_y, float dly_z)
    {
        NNS_VECTOR ofst = GlobalPool<NNS_VECTOR>.Alloc();
        ofst.x = dly_x;
        ofst.y = dly_y;
        ofst.z = dly_z;
        ObjCameraPlaySet(0, ofst);
        GlobalPool<NNS_VECTOR>.Release(ofst);
    }

    private static void GmCameraDelayReset()
    {
        NNS_VECTOR ofst1 = new NNS_VECTOR(50f, 50f, 0.0f);
        NNS_VECTOR ofst2 = new NNS_VECTOR(0.0f, 0.0f, 0.0f);
        if (!GSM_MAIN_STAGE_IS_SPSTAGE())
            ObjCameraPlaySet(0, ofst1);
        else
            ObjCameraPlaySet(0, ofst2);
    }

    public static void GmCameraScaleSet(float scale_target, float scale_spd)
    {
        gm_camera_work.scale_target = scale_target;
        gm_camera_work.scale_spd = scale_spd;
    }

    public static void GmCameraSetClipCamera(OBS_CAMERA obj_camera)
    {
        float left = (float)(g_gm_main_system.map_fcol.left + OBD_OBJ_CLIP_LCD_X / 2);
        float right = (float)(g_gm_main_system.map_fcol.right - OBD_OBJ_CLIP_LCD_X / 2);
        float top = (float)(g_gm_main_system.map_fcol.top + OBD_OBJ_CLIP_LCD_Y / 2);
        float bottom = (float)(g_gm_main_system.map_fcol.bottom - OBD_OBJ_CLIP_LCD_Y / 2);

        float camera_x, camera_y;

        if (obj_camera.disp_pos.x <= left)
        {
            camera_x = left;
        }
        else if (obj_camera.disp_pos.x >= right)
        {
            camera_x = right;
        }
        else
        {
            camera_x = obj_camera.disp_pos.x;
        }
        if (-obj_camera.disp_pos.y <= top)
        {
            camera_y = top;
        }
        else if (-obj_camera.disp_pos.y >= bottom)
        {
            camera_y = bottom;
        }
        else
        {
            camera_y = -obj_camera.disp_pos.y;
        }

        ObjObjectClipCameraSet(FXM_FLOAT_TO_FX32(camera_x - (float)(OBD_OBJ_CLIP_LCD_X / 2)),
                            FXM_FLOAT_TO_FX32(camera_y - (float)(OBD_OBJ_CLIP_LCD_Y / 2)));
    }

    public static void GmCameraFunc(OBS_CAMERA obj_camera)
    {
        SNNS_VECTOR pos;
        GMS_PLAYER_WORK ply_work = g_gm_main_system.ply_work[GSD_MAIN_PLAYER_1P];

        pos.x = FXM_FX32_TO_FLOAT(ply_work.obj_work.pos.x);
        pos.z = FXM_FX32_TO_FLOAT(ply_work.obj_work.pos.z);
        if (!GSM_MAIN_STAGE_IS_SPSTAGE())
        {
            // 通常はカメラＹを少しオフセット
            pos.y = FXM_FX32_TO_FLOAT(-ply_work.obj_work.pos.y + GMD_SCR_PLY_Y_OFFS);
        }
        else
        {
            // スペステはオフセット量を少し抑える（スペステ終了時画面回転時の中心ずれ違和感を抑えるため）
            pos.y = FXM_FX32_TO_FLOAT(-ply_work.obj_work.pos.y + GMD_SCR_PLY_Y_OFFS / 3);
        }

        // 目的値を記録
        if ((ply_work.player_flag & GMD_PLF_DIE) == GMD_PLF_DIE &&
                    (ply_work.player_flag & GMD_PLF_TATK_RETRY) == 0)
        {
            // プレイヤー死亡時はカメラ不動(リトライ演出中を除く)
            pos.x = obj_camera.work.x;
            pos.y = obj_camera.work.y;
            pos.z = obj_camera.work.z;
        }
        else
        {
            // 通常時は更新
            obj_camera.work.x = pos.x;
            obj_camera.work.y = pos.y;
            obj_camera.work.z = pos.z;
        }

        obj_camera.prev_pos.x = obj_camera.pos.x;
        obj_camera.prev_pos.y = obj_camera.pos.y;
        obj_camera.prev_pos.z = obj_camera.pos.z;

        if ((obj_camera.flag & OBD_CAMERA_FIX) == OBD_CAMERA_FIX)
        {
            if (obj_camera.target_obj != null)
            {
                obj_camera.pos.x = pos.x;
                obj_camera.pos.y = pos.y;
                obj_camera.pos.z = pos.z;
            }

            return;
        }

        // スケール変更チェック＆実行
        gmCameraScaleChange(obj_camera);


        if ((obj_camera.flag & OBD_CAMERA_SHIFT) == OBD_CAMERA_SHIFT)
        {
            // シフトで各軸移動
            obj_camera.pos.x = ObjShiftSetF(obj_camera.pos.x, pos.x, obj_camera.shift, obj_camera.spd_max.x, obj_camera.spd_add.x);
            obj_camera.pos.y = ObjShiftSetF(obj_camera.pos.y, pos.x, obj_camera.shift, obj_camera.spd_max.y, obj_camera.spd_add.y);
            obj_camera.pos.z = ObjShiftSetF(obj_camera.pos.z, pos.x, obj_camera.shift, obj_camera.spd_max.z, obj_camera.spd_add.z);
        }
        else
        {
            // スクロール開始範囲セット
            obj_camera.allow.x = obj_camera.allow_limit.x;
            obj_camera.allow.z = obj_camera.allow_limit.z;
            if ((ply_work.obj_work.move_flag & OBD_MOVE_JUMP) == OBD_MOVE_JUMP)
            {   // 接地で判定する場合は、OBD_MOVE_UNDER
                // 空中
                //			obj_camera.allow.y = MTM_MATH_MIN(obj_camera.allow.y + GMD_SCR_ALLOW_SPD, obj_camera.allow_limit.y);
                obj_camera.allow.y = obj_camera.allow_limit.y;
            }
            else
            {
                //　地上
                obj_camera.allow.y = MTM_MATH_MAX((uint)(obj_camera.allow.y - GMD_SCR_ALLOW_SPD), 0);
            }

            // スクロール開始範囲チェック(スクロールトリガ)
            // X
            if (pos.x < (obj_camera.pos.x - obj_camera.allow.x))
            {
                pos.x += obj_camera.allow.x;
            }
            else if (pos.x > (obj_camera.pos.x + obj_camera.allow.x))
            {
                pos.x -= obj_camera.allow.x;
            }
            else
            {
                pos.x = obj_camera.pos.x;
            }
            // Y
            if (pos.y < (obj_camera.pos.y - obj_camera.allow.y))
            {
                pos.y += obj_camera.allow.y;
            }
            else if (pos.y > (obj_camera.pos.y + obj_camera.allow.y))
            {
                pos.y -= obj_camera.allow.y;
            }
            else
            {
                pos.y = obj_camera.pos.y;
            }
            // 移動速度設定、加減速
            if (pos.x != obj_camera.pos.x)
            {
                obj_camera.spd.x = ObjSpdUpSetF(obj_camera.spd.x, obj_camera.spd_add.x, obj_camera.spd_max.x);
            }
            else
            {
                obj_camera.spd.x = ObjSpdDownSetF(obj_camera.spd.x, obj_camera.spd_add.x);
            }
            if (pos.y != obj_camera.pos.y)
            {
                obj_camera.spd.y = ObjSpdUpSetF(obj_camera.spd.y, obj_camera.spd_add.y, obj_camera.spd_max.y);
            }
            else
            {
                obj_camera.spd.y = ObjSpdDownSetF(obj_camera.spd.y, obj_camera.spd_add.y);
            }
            if (pos.z != obj_camera.pos.z)
            {
                obj_camera.spd.z = ObjSpdUpSetF(obj_camera.spd.z, obj_camera.spd_add.z, obj_camera.spd_max.z);
            }
            else
            {
                obj_camera.spd.z = ObjSpdDownSetF(obj_camera.spd.z, obj_camera.spd_add.z);
            }
            // 各軸移動
            if (pos.x > obj_camera.pos.x)
            {
                obj_camera.pos.x += obj_camera.spd.x;
                if (obj_camera.pos.x > pos.x)
                    obj_camera.pos.x = pos.x;
            }
            else
            {
                obj_camera.pos.x -= obj_camera.spd.x;
                if (obj_camera.pos.x < pos.x)
                    obj_camera.pos.x = pos.x;
            }
            if (pos.y > obj_camera.pos.y)
            {
                obj_camera.pos.y += obj_camera.spd.y;
                if (obj_camera.pos.y > pos.y)
                    obj_camera.pos.y = pos.y;
            }
            else
            {
                obj_camera.pos.y -= obj_camera.spd.y;
                if (obj_camera.pos.y < pos.y)
                    obj_camera.pos.y = pos.y;
            }
            if (pos.z > obj_camera.pos.z)
            {
                obj_camera.pos.z += obj_camera.spd.z;
                if (obj_camera.pos.z > pos.z)
                    obj_camera.pos.z = pos.z;
            }
            else
            {
                obj_camera.pos.z -= obj_camera.spd.z;
                if (obj_camera.pos.z < pos.z)
                    obj_camera.pos.z = pos.z;
            }
        }
#if false
    // 離れオーバーチェック
    if ( MTM_MATH_ABS(pos.x - obj_camera.pos.x) > obj_camera.play_ofst_max.x ){
        if ( pos.x > obj_camera.pos.x ){
            obj_camera.pos.x = pos.x - obj_camera.play_ofst_max.x;
        }else{
            obj_camera.pos.x = pos.x + obj_camera.play_ofst_max.x;
        }

    }
    if ( MTM_MATH_ABS(pos.y - obj_camera.pos.y) > obj_camera.play_ofst_max.y ){
        if ( pos.y > obj_camera.pos.y ){
            obj_camera.pos.y = pos.y - obj_camera.play_ofst_max.y;
        }else{
            obj_camera.pos.y = pos.y + obj_camera.play_ofst_max.y;
        }

    }
#endif
        if (MTM_MATH_ABS(pos.z - obj_camera.pos.z) > obj_camera.play_ofst_max.z)
        {
            if (pos.z > obj_camera.pos.z)
            {
                obj_camera.pos.z = pos.z - obj_camera.play_ofst_max.z;
            }
            else
            {
                obj_camera.pos.z = pos.z + obj_camera.play_ofst_max.z;
            }
        }
        // 範囲オーバーチェック
        //	objCameraPosLimitCheck(obj_camera, &obj_camera.pos );

        // 振動チェック
        obj_camera.disp_ofst.x = gm_camera_vibration.x / 16;
        obj_camera.disp_ofst.y = gm_camera_vibration.y / 16;
        obj_camera.disp_ofst.z = gm_camera_vibration.z / 16;
        gm_camera_vibration.x = gmCameraVibCheck(gm_camera_vibration.x);
        gm_camera_vibration.y = gmCameraVibCheck(gm_camera_vibration.y);
        gm_camera_vibration.z = gmCameraVibCheck(gm_camera_vibration.z);

        obj_camera.pos.z = 50.0f;

#if false
        // コンソール版
        obj_camera.disp_pos.x = obj_camera.pos.x + obj_camera.ofst.x;
        obj_camera.disp_pos.y = obj_camera.pos.y + obj_camera.ofst.y;
        obj_camera.disp_pos.z = obj_camera.pos.z + obj_camera.ofst.z;
#else
	// iPhone版：The iPhone version corrects the offset amount for the camera offset when magnifying the screen.
	{
		SNNS_VECTOR	ofst;
		ofst.x = obj_camera.ofst.x;
		ofst.y = obj_camera.ofst.y;
		ofst.z = obj_camera.ofst.z;
		if (obj_camera.scale < GMD_CAMERA_SCALE) {
			// スケール値によりオフセット量を調整
			float rate = obj_camera.scale - GMD_CAMERA_UP_SCALE_MAX;
			rate /= GMD_CAMERA_SCALE - GMD_CAMERA_UP_SCALE_MAX;
			ofst.x *= rate;
			ofst.y *= rate;
		}
		obj_camera.disp_pos.x = obj_camera.pos.x + ofst.x;
		obj_camera.disp_pos.y = obj_camera.pos.y + ofst.y;
		obj_camera.disp_pos.z = obj_camera.pos.z + ofst.z;
	}
#endif
        gmCameraLookupCheck(obj_camera);                                // 見上げ/見下ろし

        // 表示限界リミットチェック
        {
            float left = g_gm_main_system.map_fcol.left + (float)(OBD_LCD_X / 2);
            float right = g_gm_main_system.map_fcol.right - (float)(OBD_LCD_X / 2);
            float top = g_gm_main_system.map_fcol.top + (float)(OBD_LCD_Y / 2);
            float bottom = g_gm_main_system.map_fcol.bottom - (float)(OBD_LCD_Y / 2);
            // scale変更時の左右端チェック
#if true
            // 左端チェック
            if (obj_camera.disp_pos.x < left)
            {
                obj_camera.disp_pos.x = left;

                if (obj_camera.disp_pos.x > right)
                {
                    obj_camera.disp_pos.x = (right + left) / 2;
                }
            }
            // 右端チェック
            else
            if (obj_camera.disp_pos.x > right)
            {
                obj_camera.disp_pos.x = right;

                if (obj_camera.disp_pos.x < left)
                {
                    obj_camera.disp_pos.x = (right + left) / 2;
                }
            }
#else
            // 左端チェック
            if (obj_camera.disp_pos.x < left)
            {
                obj_camera.disp_pos.x = left;
            }
            // 右端チェック
            if (obj_camera.disp_pos.x > right)
            {
                obj_camera.disp_pos.x = right;
            }
#endif
            // 上端チェック
            if (obj_camera.disp_pos.y > -top)
            {
                obj_camera.disp_pos.y = -top;
            }
            // 下端チェック
            if (obj_camera.disp_pos.y < -bottom)
            {
                obj_camera.disp_pos.y = -bottom;
            }
        }

        obj_camera.target_pos.Assign(obj_camera.disp_pos);
        obj_camera.target_pos.z -= 50.0f;

        // スペステ回転時は何も回転補正しない
        if (GSM_MAIN_STAGE_IS_SPSTAGE())
        {
        }
        else
#if true
    // ループカメラテスト開始その１
    if ((obj_camera.flag & OBD_CAMERA_ROT_EX) == OBD_CAMERA_ROT_EX && (ply_work.obj_work.move_flag & OBD_MOVE_UNDER) == OBD_MOVE_UNDER)
        {
            // ループカメラ用回転制御
            // 接地時のみ
            short ply_roll;
            short cam_roll;
            short add_roll;
            ply_roll = (short)(0 - (short)ply_work.obj_work.dir.z);
            cam_roll = (short)obj_camera.roll;
            if (((ushort)ply_roll > 0x4000)
                && ((ushort)ply_roll < 0xc000))
            {
                add_roll = (short)((ushort)((uint)((ushort)(ply_roll) + (ushort)(cam_roll)) / 2));
            }
            else
            {
                add_roll = (short)((ply_roll + cam_roll) / 2);
            }
            obj_camera.roll = (int)add_roll;
            obj_camera.flag &= ~OBD_CAMERA_ROT_EX;
        }
#if false  // Camera rotation control by tilting operation: From here                                                             
	else
// ループカメラテスト終了その１

	if (!(g_gs_main_sys_info.game_flag & GSD_MAINSYS_GAME_FLAG_INPUT_CLASSIC)) {
		// 傾斜操作による画面回転
		Angle32	roll;
#if false  // ほぼコントローラ角度直値                  
		roll = (0-AoPadRotZ())/4;
		roll = (roll + obj_camera.roll) /2;
#elif false    // コントローラＺ回転角度履歴からの平均値                       
		u16		i;
		obj_camera.roll_ptr = ++obj_camera.roll_ptr & 0x0f;
		obj_camera.roll_hist[obj_camera.roll_ptr] = ply_work.key_rot_z/4;

		roll = 0;
		for (i = 0; i < 16;i++) {
			roll += obj_camera.roll_hist[i];
		}
		roll /= 16;
#elif false    // コントローラＺ回転角度累乗値履歴からの平均値                          
		u16		i;
		float	mul;
		obj_camera.roll_ptr = ++obj_camera.roll_ptr & 0x0f;

		roll = ply_work.key_rot_z/4;
		mul = float(roll);
		mul = MTM_MATH_ABS(mul / (GMD_SCR_ROLL_MAX/2));
		roll = fx32(roll * mul);
		obj_camera.roll_hist[obj_camera.roll_ptr] = roll;

		roll = 0;
		for (i = 0; i < 16;i++) {
			roll += obj_camera.roll_hist[i];
		}
		roll /= 16;
#elif true // コントローラＺ回転角度累乗値履歴からの平均値(コントローラ傾斜リミットをプレイヤー傾斜にあわせる)                                                      
		u16		i;
		float	fMul,fRol;
		obj_camera.roll_ptr = ++obj_camera.roll_ptr & 0x0f;

		roll = MTM_MATH_CLIP(ply_work.key_rot_z, -GMD_PL_DEF_ROLL_MAX, GMD_PL_DEF_ROLL_MAX);
		fRol = (float)(roll * GMD_SCR_ROLL_MAX / GMD_PL_DEF_ROLL_MAX);
		fMul = fRol;
		fMul = MTM_MATH_ABS(fMul / GMD_SCR_ROLL_MAX);
		roll = fx32(fRol * fMul);
		obj_camera.roll_hist[obj_camera.roll_ptr] = roll;

		roll = 0;
		for (i = 0; i < 16;i++) {
			roll += obj_camera.roll_hist[i];
		}
		roll /= 16;
#else // プレイヤー速度から回転角を求める                      
		u16		i;
//		fx32	spd = ply_work.obj_work.spd_m;
//		roll = FXM_FX32_TO_FLOAT(spd);
	//	roll = FXM_FX32_TO_FLOAT(ply_work.obj_work.spd_m);
		roll = Angle32(ply_work.obj_work.spd_m) / 12;

		obj_camera.roll_ptr = ++obj_camera.roll_ptr & 0x07;
		obj_camera.roll_hist[obj_camera.roll_ptr] = roll;

		roll = 0;
		for (i = 0; i < 8;i++) {
			roll += obj_camera.roll_hist[i];
		}
		roll /= 8;
#endif    // コントロールタイプ              
		obj_camera.roll = MTM_MATH_CLIP(roll, -GMD_SCR_ROLL_MAX, GMD_SCR_ROLL_MAX);
	}
#endif    // 傾斜操作によるカメラ回転制御：ここまで                        
        // ループカメラテスト開始その２
        else
        {
            // 徐々に角度を戻す
            obj_camera.roll = obj_camera.roll * 9 / 10;
            obj_camera.flag &= ~OBD_CAMERA_ROT_EX;
        }
        // ループカメラテスト終了その２
#endif

        // オブジェクトカメラ設定
        ObjObjectCameraSet(FXM_FLOAT_TO_FX32(obj_camera.disp_pos.x - (float)(OBD_LCD_X / 2)),
                            FXM_FLOAT_TO_FX32(-obj_camera.disp_pos.y - (float)(OBD_LCD_Y / 2)),
                            FXM_FLOAT_TO_FX32(obj_camera.disp_pos.x - (float)(OBD_LCD_X / 2)),
                            FXM_FLOAT_TO_FX32(-obj_camera.disp_pos.y - (float)(OBD_LCD_Y / 2)));

        // クリッピングカメラ設定
        GmCameraSetClipCamera(obj_camera);
    }

    private static void gmCameraFuncMapFar(OBS_CAMERA obj_camera)
    {
        OBS_CAMERA obsCamera = ObjCameraGet(0);
        obj_camera.pos.Assign(GmMapFarGetCameraPos(obsCamera.disp_pos));
        obj_camera.target_pos.Assign(GmMapFarGetCameraTarget(obj_camera.pos));
        obj_camera.disp_pos.Assign(obj_camera.pos);
        if (GSM_MAIN_STAGE_IS_SPSTAGE())
            return;
        obj_camera.roll = obsCamera.roll;
    }

    private static void gmCameraFuncAddMap(OBS_CAMERA obj_camera)
    {
        OBS_CAMERA obsCamera = ObjCameraGet(0);
        obj_camera.prev_pos.Assign(obj_camera.pos);
        obj_camera.prev_disp_pos.Assign(obj_camera.disp_pos);
        obj_camera.disp_pos.Assign(obsCamera.disp_pos);
        obj_camera.pos.Assign(obsCamera.pos);
        obj_camera.ofst.Assign(obsCamera.ofst);
        obj_camera.disp_ofst.Assign(obsCamera.disp_ofst);
        obj_camera.target_ofst.Assign(obsCamera.target_ofst);
        obj_camera.play_ofst_max.Assign(obsCamera.play_ofst_max);
        obj_camera.camup_obj = obsCamera.camup_obj;
        obj_camera.camup_pos.Assign(obsCamera.camup_pos);
        obj_camera.roll = obsCamera.roll;
        obj_camera.up_vec.Assign(obsCamera.up_vec);
        obj_camera.scale = obsCamera.scale;
        obj_camera.left = obsCamera.left;
        obj_camera.right = obsCamera.right;
        obj_camera.bottom = obsCamera.bottom;
        obj_camera.top = obsCamera.top;
        obj_camera.znear = obsCamera.znear;
        obj_camera.zfar = obsCamera.zfar;
        obj_camera.aspect = obsCamera.aspect;
        GmMapGetAddMapCameraPos(obsCamera.disp_pos, obsCamera.target_pos, obj_camera.disp_pos, obj_camera.target_pos, obj_camera.camera_id);
    }

    private static void gmCameraFuncWater(OBS_CAMERA obj_camera)
    {
        OBS_CAMERA obsCamera = ObjCameraGet(0);
        nnCopyVector(obj_camera.pos, obsCamera.pos);
        nnCopyVector(obj_camera.target_pos, obsCamera.target_pos);
        nnCopyVector(obj_camera.disp_pos, obsCamera.disp_pos);
    }

    private static void GmCameraTruckFunc(OBS_CAMERA obj_camera)
    {
        NNS_VECTOR nnsVector1 = GlobalPool<NNS_VECTOR>.Alloc();
        GMS_PLAYER_WORK gmsPlayerWork = g_gm_main_system.ply_work[0];
        if ((gmsPlayerWork.player_flag & GMD_PLF_DIE) != 0 && (gmsPlayerWork.player_flag & GMD_PLF_TATK_RETRY) == 0)
        {
            GlobalPool<NNS_VECTOR>.Release(nnsVector1);
        }
        else
        {
            nnsVector1.x = FXM_FX32_TO_FLOAT(gmsPlayerWork.obj_work.pos.x);
            nnsVector1.y = FXM_FX32_TO_FLOAT(-gmsPlayerWork.obj_work.pos.y + 24576);
            nnsVector1.z = FXM_FX32_TO_FLOAT(gmsPlayerWork.obj_work.pos.z);
            obj_camera.work.x = nnsVector1.x;
            obj_camera.work.y = nnsVector1.y;
            obj_camera.work.z = nnsVector1.z;
            obj_camera.prev_pos.x = obj_camera.pos.x;
            obj_camera.prev_pos.y = obj_camera.pos.y;
            obj_camera.prev_pos.z = obj_camera.pos.z;
            if (((int)obj_camera.flag & 1) != 0)
            {
                if (obj_camera.target_obj != null)
                {
                    obj_camera.pos.x = nnsVector1.x;
                    obj_camera.pos.y = nnsVector1.y;
                    obj_camera.pos.z = nnsVector1.z;
                }
                GlobalPool<NNS_VECTOR>.Release(nnsVector1);
            }
            else
            {
                if (((int)obj_camera.flag & 2) != 0)
                {
                    obj_camera.pos.x = ObjShiftSetF(obj_camera.pos.x, nnsVector1.x, obj_camera.shift, obj_camera.spd_max.x, obj_camera.spd_add.x);
                    obj_camera.pos.y = ObjShiftSetF(obj_camera.pos.y, nnsVector1.x, obj_camera.shift, obj_camera.spd_max.y, obj_camera.spd_add.y);
                    obj_camera.pos.z = ObjShiftSetF(obj_camera.pos.z, nnsVector1.x, obj_camera.shift, obj_camera.spd_max.z, obj_camera.spd_add.z);
                }
                else
                {
                    obj_camera.allow.x = obj_camera.allow_limit.x;
                    obj_camera.allow.z = obj_camera.allow_limit.z;
                    obj_camera.allow.y = ((int)gmsPlayerWork.obj_work.move_flag & 16) == 0 ? Math.Max(obj_camera.allow.y - 4f, 0.0f) : obj_camera.allow_limit.y;
                    if (nnsVector1.x < obj_camera.pos.x - (double)obj_camera.allow.x)
                        nnsVector1.x += obj_camera.allow.x;
                    else if (nnsVector1.x > obj_camera.pos.x + (double)obj_camera.allow.x)
                        nnsVector1.x -= obj_camera.allow.x;
                    else
                        nnsVector1.x = obj_camera.pos.x;
                    if (nnsVector1.y < obj_camera.pos.y - (double)obj_camera.allow.y)
                        nnsVector1.y += obj_camera.allow.y;
                    else if (nnsVector1.y > obj_camera.pos.y + (double)obj_camera.allow.y)
                        nnsVector1.y -= obj_camera.allow.y;
                    else
                        nnsVector1.y = obj_camera.pos.y;
                    obj_camera.spd.x = nnsVector1.x == (double)obj_camera.pos.x ? ObjSpdDownSetF(obj_camera.spd.x, obj_camera.spd_add.x) : ObjSpdUpSetF(obj_camera.spd.x, obj_camera.spd_add.x, obj_camera.spd_max.x);
                    obj_camera.spd.y = nnsVector1.y == (double)obj_camera.pos.y ? ObjSpdDownSetF(obj_camera.spd.y, obj_camera.spd_add.y) : ObjSpdUpSetF(obj_camera.spd.y, obj_camera.spd_add.y, obj_camera.spd_max.y);
                    obj_camera.spd.z = nnsVector1.z == (double)obj_camera.pos.z ? ObjSpdDownSetF(obj_camera.spd.z, obj_camera.spd_add.z) : ObjSpdUpSetF(obj_camera.spd.z, obj_camera.spd_add.z, obj_camera.spd_max.z);
                    if (nnsVector1.x > (double)obj_camera.pos.x)
                    {
                        obj_camera.pos.x += obj_camera.spd.x;
                        if (obj_camera.pos.x > (double)nnsVector1.x)
                            obj_camera.pos.x = nnsVector1.x;
                    }
                    else
                    {
                        obj_camera.pos.x -= obj_camera.spd.x;
                        if (obj_camera.pos.x < (double)nnsVector1.x)
                            obj_camera.pos.x = nnsVector1.x;
                    }
                    if (nnsVector1.y > (double)obj_camera.pos.y)
                    {
                        obj_camera.pos.y += obj_camera.spd.y;
                        if (obj_camera.pos.y > (double)nnsVector1.y)
                            obj_camera.pos.y = nnsVector1.y;
                    }
                    else
                    {
                        obj_camera.pos.y -= obj_camera.spd.y;
                        if (obj_camera.pos.y < (double)nnsVector1.y)
                            obj_camera.pos.y = nnsVector1.y;
                    }
                    if (nnsVector1.z > (double)obj_camera.pos.z)
                    {
                        obj_camera.pos.z += obj_camera.spd.z;
                        if (obj_camera.pos.z > (double)nnsVector1.z)
                            obj_camera.pos.z = nnsVector1.z;
                    }
                    else
                    {
                        obj_camera.pos.z -= obj_camera.spd.z;
                        if (obj_camera.pos.z < (double)nnsVector1.z)
                            obj_camera.pos.z = nnsVector1.z;
                    }
                }
                if (MTM_MATH_ABS(nnsVector1.z - obj_camera.pos.z) > (double)obj_camera.play_ofst_max.z)
                    obj_camera.pos.z = nnsVector1.z <= (double)obj_camera.pos.z ? nnsVector1.z + obj_camera.play_ofst_max.z : nnsVector1.z - obj_camera.play_ofst_max.z;
                obj_camera.disp_ofst.x = gm_camera_vibration.x / 16f;
                obj_camera.disp_ofst.y = gm_camera_vibration.y / 16f;
                obj_camera.disp_ofst.z = gm_camera_vibration.z / 16f;
                gm_camera_vibration.x = gmCameraVibCheck(gm_camera_vibration.x);
                gm_camera_vibration.y = gmCameraVibCheck(gm_camera_vibration.y);
                gm_camera_vibration.z = gmCameraVibCheck(gm_camera_vibration.z);
                obj_camera.pos.z = 50f;
                if (true)
                {
                    obj_camera.disp_pos.x = obj_camera.pos.x + obj_camera.ofst.x;
                    obj_camera.disp_pos.y = obj_camera.pos.y + obj_camera.ofst.y;
                    obj_camera.disp_pos.z = obj_camera.pos.z + obj_camera.ofst.z;
                }
                else
                {
                    NNS_VECTOR nnsVector2 = GlobalPool<NNS_VECTOR>.Alloc();
                    nnsVector2.x = obj_camera.ofst.x;
                    nnsVector2.y = obj_camera.ofst.y;
                    nnsVector2.z = obj_camera.ofst.z;
                    if ((double)obj_camera.scale < GMD_CAMERA_SCALE)
                    {
                        float num = (obj_camera.scale - 0.3371917f) / 0.3371917f;
                        nnsVector2.x *= num;
                        nnsVector2.y *= num;
                    }
                    obj_camera.disp_pos.x = obj_camera.pos.x + nnsVector2.x;
                    obj_camera.disp_pos.y = obj_camera.pos.y + nnsVector2.y;
                    obj_camera.disp_pos.z = obj_camera.pos.z + nnsVector2.z;
                    GlobalPool<NNS_VECTOR>.Release(nnsVector2);
                }
                gmCameraLookupCheck(obj_camera);
                float num1 = g_gm_main_system.map_fcol.left + GSD_DISP_WIDTH / 2 * obj_camera.scale;
                float num2 = g_gm_main_system.map_fcol.right - GSD_DISP_WIDTH / 2 * obj_camera.scale;
                float num3 = g_gm_main_system.map_fcol.top + GSD_DISP_HEIGHT / 2 * obj_camera.scale;
                float num4 = g_gm_main_system.map_fcol.bottom - GSD_DISP_HEIGHT / 2 * obj_camera.scale;
                if (obj_camera.disp_pos.x < (double)num1)
                    obj_camera.disp_pos.x = num1;
                if (obj_camera.disp_pos.x > (double)num2)
                    obj_camera.disp_pos.x = num2;
                if (obj_camera.disp_pos.y > -(double)num3)
                    obj_camera.disp_pos.y = -num3;
                if (obj_camera.disp_pos.y < -(double)num4)
                    obj_camera.disp_pos.y = -num4;
                obj_camera.target_pos.Assign(obj_camera.disp_pos);
                obj_camera.target_pos.z -= 50f;
                int num5 = 4608;
                if (((int)gmsPlayerWork.gmk_flag2 & 32) != 0)
                    num5 = 672;
                int a1 = -gmsPlayerWork.ply_pseudofall_dir - obj_camera.roll;
                if (a1 > 32768)
                    a1 -= 65536;
                else if (a1 < short.MinValue)
                    a1 += 65536;
                int a2;
                if (MTM_MATH_ABS(a1) < 128)
                {
                    a2 = a1;
                }
                else
                {
                    a2 = a1 >> 4;
                    if (MTM_MATH_ABS(a2) > 3328)
                        a2 += a2 - 3328;
                    if (MTM_MATH_ABS(a2) > num5)
                        a2 = a2 < 0 ? -num5 : num5;
                    else if (MTM_MATH_ABS(a2) < 128)
                        a2 = a2 < 0 ? sbyte.MinValue : 128;
                }
                obj_camera.roll += a2;
                if (obj_camera.roll > 65536)
                {
                    obj_camera.roll -= 65536;
                    gmsPlayerWork.ply_pseudofall_dir += 65536;
                }
                else if (obj_camera.roll < -65536)
                {
                    obj_camera.roll += 65536;
                    gmsPlayerWork.ply_pseudofall_dir -= 65536;
                }
                ObjObjectCameraSet(FXM_FLOAT_TO_FX32(obj_camera.disp_pos.x - OBD_LCD_X / 2), FXM_FLOAT_TO_FX32(-obj_camera.disp_pos.y - OBD_LCD_Y / 2), FXM_FLOAT_TO_FX32(obj_camera.disp_pos.x - OBD_LCD_X / 2), FXM_FLOAT_TO_FX32(-obj_camera.disp_pos.y - OBD_LCD_Y / 2));
                GmCameraSetClipCamera(obj_camera);
                GlobalPool<NNS_VECTOR>.Release(nnsVector1);
            }
        }
    }

    private static float gmCameraVibCheck(float vib)
    {
        if (vib != 0.0)
        {
            if (vib > 0.0)
            {
                vib = (float)(0.0 - (vib - 1.0));
                if (vib > 0.0)
                    vib = 0.0f;
            }
            else
            {
                vib = (float)(0.0 - (vib + 1.0));
                if (vib < 0.0)
                    vib = 0.0f;
            }
        }
        return vib;
    }

    private static void gmCameraLookupCheck(OBS_CAMERA obj_camera)
    {
        GMS_PLAYER_WORK gmsPlayerWork = g_gm_main_system.ply_work[0];
        int num1 = 0;
        if (GSM_MAIN_STAGE_IS_SPSTAGE())
            return;
        if (gmsPlayerWork.seq_state == 4 && (gmsPlayerWork.key_on & 1) != 0)
            num1 = 1;
        if (gmsPlayerWork.seq_state == 7 && (gmsPlayerWork.key_on & 2) != 0)
            num1 = 2;
        if ((gm_camera_work.flag & num1) != 0 && gm_camera_work.timer > 90)
        {
            if ((gm_camera_work.flag & 1) != 0)
                gm_camera_work.offset += 2;
            else
                gm_camera_work.offset -= 2;
            int num2 = gmsPlayerWork.camera_ofst_y >> 12;
            gm_camera_work.offset = MTM_MATH_CLIP(gm_camera_work.offset, -96 - num2, 80 - num2);
        }
        else
        {
            if ((gm_camera_work.flag & num1) != 0)
                ++gm_camera_work.timer;
            else if (num1 != 0)
            {
                gm_camera_work.flag = num1;
                gm_camera_work.timer = 1;
            }
            else
            {
                gm_camera_work.flag = 0;
                gm_camera_work.timer = 0;
            }
            if (gm_camera_work.offset > 0)
            {
                gm_camera_work.offset -= 2;
                if (gm_camera_work.offset < 0)
                    gm_camera_work.offset = 0;
            }
            else if (gm_camera_work.offset < 0)
            {
                gm_camera_work.offset += 2;
                if (gm_camera_work.offset > 0)
                    gm_camera_work.offset = 0;
            }
        }
        switch ((ushort)(obj_camera.roll + 8192 >> 14))
        {
            case 0:
                obj_camera.disp_pos.y += gm_camera_work.offset;
                break;
            case 1:
                obj_camera.disp_pos.x -= gm_camera_work.offset;
                break;
            case 2:
                obj_camera.disp_pos.y -= gm_camera_work.offset;
                break;
            case 3:
                obj_camera.disp_pos.x += gm_camera_work.offset;
                break;
        }
    }

    private static void gmCameraScaleChange(OBS_CAMERA obj_camera)
    {
        if (gm_camera_work.scale_now == (double)gm_camera_work.scale_target)
            return;
        if (gm_camera_work.scale_now < (double)gm_camera_work.scale_target)
        {
            gm_camera_work.scale_now += gm_camera_work.scale_spd;
            if (gm_camera_work.scale_now > (double)gm_camera_work.scale_target)
                gm_camera_work.scale_now = gm_camera_work.scale_target;
        }
        else if (gm_camera_work.scale_now > (double)gm_camera_work.scale_target)
        {
            gm_camera_work.scale_now -= gm_camera_work.scale_spd;
            if (gm_camera_work.scale_now < (double)gm_camera_work.scale_target)
                gm_camera_work.scale_now = gm_camera_work.scale_target;
        }
        obj_camera.scale = GMD_CAMERA_SCALE / gm_camera_work.scale_now;
    }

}