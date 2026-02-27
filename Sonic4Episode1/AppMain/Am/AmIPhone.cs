using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using mpp;

internal partial class AppMain
{
    public void amIPhoneInitNN(PresentationParameters parameters) =>
        amIPhoneInitNN(parameters.BackBufferWidth, parameters.BackBufferHeight);

    public void amIPhoneInitNN(int bbWidth, int bbHeight)
    {
        OpenGL.init(m_game, m_graphicsDevice);
        OpenGL.glViewport(0, 0, bbWidth, bbHeight);

        float width = bbWidth;
        float height = bbHeight;
        amRenderInit();
        NNS_MATRIX mtx = GlobalPool<NNS_MATRIX>.Alloc();
        NNS_VECTOR nnsVector = new NNS_VECTOR(0.0f, 0.0f, -1f);
        NNS_RGBA nnsRgba = new NNS_RGBA(1f, 1f, 1f, 1f);
        NNS_CONFIG_GL config;
        config.WindowWidth = (int)width;
        config.WindowHeight = (int)height;
        this.nnConfigureSystemGL(config);

        var fov = 45f + NNM_RADtoDEG((float)AppMain.nnArcTan2(height, width) - NNM_DEGtoRAD(45f));

        nnMakePerspectiveMatrix(mtx, NNM_DEGtoA32(fov), height / (float)width, 1f, 10000f);
        nnSetProjection(mtx, 0);

        _am_draw_video.draw_aspect = height / (float)width;
        _am_draw_video.draw_width = bbWidth;
        _am_draw_video.draw_height = bbHeight;
        _am_draw_video.disp_width = bbWidth;
        _am_draw_video.disp_height = bbHeight;

        var aspect = width / height;
        g_ao_act_wide_w = _am_draw_video.width_2d = 720.0f * aspect;
        g_ao_act_wide_h = _am_draw_video.height_2d = 720.0f;

        float vw = _am_draw_video.draw_aspect * _am_draw_video.height_2d;
        _am_draw_video.scale_x_2d = _am_draw_video.draw_width / vw;
        _am_draw_video.scale_y_2d = _am_draw_video.draw_height
                                        / _am_draw_video.height_2d;
        _am_draw_video.base_x_2d = (vw - _am_draw_video.width_2d) * 0.5f
                                        * _am_draw_video.scale_x_2d;
        _am_draw_video.base_y_2d = 0.0f;

        _am_draw_video.wide_screen = true;
        _am_draw_video.refresh_rate = 60f;

        //for (int i = 0; i < gm_player_push_jump_key_rect2.Length; i++)
        //{
        //    gm_player_push_jump_key_rect[i] = new ushort[gm_player_push_jump_key_rect2[i].Length];
        //    for (int j = 0; j < gm_player_push_jump_key_rect2[i].Length; j++)
        //    {
        //        gm_player_push_jump_key_rect[i][j] = (ushort)gm_player_push_jump_key_rect2[i][j];
        //        if (j % 2 == 0)
        //            gm_player_push_jump_key_rect[i][j] -= (ushort)(((AppMain.AMD_SCREEN_2D_WIDTH - 1280.0f) / 2.0f) * 0.375f);
        //    }
        //}


        amRenderInit();
        GlobalPool<NNS_MATRIX>.Release(mtx);
    }

    public static void amIPhoneExitNN()
    {
    }

    public static void amIPhoneSetTextureAttribute(AMS_PARAM_LOAD_TEXTURE param)
    {
    }

    public static bool IsGLExtensionSupported(string extension)
    {
        return true;
    }

    public static void amIPhoneAccelerate(ref Vector3 accel)
    {
        NNS_VECTOR core = _am_iphone_accel_data.core;
        NNS_VECTOR sensor = _am_iphone_accel_data.sensor;
        core.x = accel.X;
        core.y = accel.Y;
        core.z = accel.Z;
        sensor.x = -core.y;
        sensor.y = core.x;
        sensor.z = core.z;
        _am_iphone_accel_data.rot_x = nnArcTan2(-sensor.z, -sensor.y);
        _am_iphone_accel_data.rot_z = nnArcTan2(sensor.x, -sensor.y);
    }

    private static void amIPhoneRequestTouch(AMS_IPHONE_TP_DATA DispData, int TouchIndex)
    {
        DispData?.Assign(_am_iphone_tp_ctrl_data[TouchIndex].tpdata);
    }

    public static void setBackKeyRequest(bool val)
    {
        _am_is_back_key_pressed = val;
    }

    public static bool isBackKeyPressed()
    {
        return _am_is_back_key_pressed;
    }

    public static void amKeyGetData()
    {
        _am_is_back_key_pressed = back_key_is_pressed;
        back_key_is_pressed = false;
    }

    static bool wasPressed = false;
    public static void onTouchEvents()
    {
        // PATCH
        var state = new List<TouchLocation>();
        if (TouchPanel.GetCapabilities().IsConnected)
        {
            state.AddRange(TouchPanel.GetState());
        }

        var mouseState = Mouse.GetState();
        if (wasPressed)
            state.Add(new TouchLocation(0, mouseState.LeftButton == ButtonState.Released ?
                (wasPressed ? TouchLocationState.Released : TouchLocationState.Moved) : TouchLocationState.Pressed,
                new Vector2(mouseState.X, mouseState.Y), TouchLocationState.Invalid, new Vector2()));
        wasPressed = mouseState.LeftButton == ButtonState.Pressed;

        for (int i = 0; i < 4; i++)
        {
            touchMarked[i] = false;
        }
        for (int j = 0; j < 4; j++)
        {
            int id = 0;
            TouchLocationState touchLocationState;
            if (j >= state.Count)
            {
                touchLocationState = TouchLocationState.Invalid;
                posVector.X = -1f;
                posVector.Y = -1f;
                int num = 0;
                while (num < 4 && touchMarked[num])
                {
                    num++;
                }
                int num2 = num;
                touchMarked[num2] = true;
            }
            else
            {
                TouchLocation touchLocation = state[j];
                float x = touchLocation.Position.X;
                float y = touchLocation.Position.Y;
                screen2real(ref x, ref y);
                posVector.X = x;
                posVector.Y = y;
                touchLocationState = touchLocation.State;
                id = touchLocation.Id;
                int num2 = amFindTouchIndex(id);
                touchMarked[num2] = true;
            }

            switch (touchLocationState)
            {
                case TouchLocationState.Invalid:
                    amIPhoneTouchCanceled(j);
                    break;
                case TouchLocationState.Released:
                    amIPhoneTouchEnded(j);
                    break;
                case TouchLocationState.Pressed:
                    amIPhoneTouchBegan(ref posVector, j, id);
                    break;
                case TouchLocationState.Moved:
                    amIPhoneTouchMoved(ref posVector, j, id);
                    break;
            }
        }
    }

    private static void screen2real(ref float X, ref float Y)
    {
#if WINDOWSPHONE7_5
        var bounds = new Rectangle(0,0,800,480);
#else
        var bounds = m_game.Window.ClientBounds;
#endif

        // nominal 480, 288
        X /= bounds.Width;
        X *= 480;

        Y /= bounds.Height;
        Y *= 288;
        Y += 16;
    }

    private static int amFindTouchIndex(int id)
    {
        for (int index = 0; index < 4; ++index)
        {
            if (_am_iphone_tp_ctrl_data[index].tpdata.id == id)
                return index;
        }
        for (int index = 0; index < 4; ++index)
        {
            if (!touchMarked[index])
                return index;
        }
        return 0;
    }

    private static void amIPhoneTouchBegan(ref Vector2 touch, int i, int id)
    {
        AMS_IPHONE_TP_DATA tpdata = _am_iphone_tp_ctrl_data[i].tpdata;
        tpdata.x = (ushort)touch.X;
        tpdata.y = (ushort)touch.Y;
        tpdata.id = id;
        tpdata.touch = 1;
        tpdata.validity = 1;
    }

    private static void amIPhoneTouchMoved(ref Vector2 touch, int i, int id)
    {
        AMS_IPHONE_TP_DATA tpdata = _am_iphone_tp_ctrl_data[i].tpdata;
        tpdata.x = (ushort)touch.X;
        tpdata.y = (ushort)touch.Y;
        tpdata.id = id;
        tpdata.touch = 1;
        tpdata.validity = 1;
    }

    private static void amIPhoneTouchCanceled()
    {
        for (int index = 0; index < 4; ++index)
        {
            _am_iphone_tp_ctrl_data[index].tpdata.touch = 0;
            _am_iphone_tp_ctrl_data[index].tpdata.validity = 0;
        }
    }

    private static void amIPhoneTouchCanceled(int i)
    {
        _am_iphone_tp_ctrl_data[i].tpdata.touch = 0;
        _am_iphone_tp_ctrl_data[i].tpdata.validity = 0;
    }

    private static void amIPhoneTouchEnded(int i)
    {
        _am_iphone_tp_ctrl_data[i].tpdata.touch = 0;
    }
}