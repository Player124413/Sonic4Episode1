using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using mpp;

internal partial class AppMain
{
    // widest the game will render before pillarboxing. 2560x1080 is 2.370 and 3440x1440 is 2.389,
    // so both stay native and only 32:9 gets bars
    public static float mppAspectCap = 2.40f;

    // the part of the back buffer the game draws into. everything aspect-dependent comes off this
    // rather than the back buffer, so the cap propagates by construction
    public static Rectangle mppViewport;

    public static Rectangle mppCalcViewport(int bbWidth, int bbHeight)
    {
        if (bbWidth <= 0 || bbHeight <= 0)
            return new Rectangle(0, 0, bbWidth, bbHeight);

        if (bbWidth <= bbHeight * mppAspectCap)
            return new Rectangle(0, 0, bbWidth, bbHeight);

        int width = (int)(bbHeight * mppAspectCap);
        return new Rectangle((bbWidth - width) / 2, 0, width, bbHeight);
    }

    public static bool mppIsPillarboxed => mppViewport.Width > 0
        && mppViewport != new Rectangle(0, 0,
            m_game.GraphicsDevice.PresentationParameters.BackBufferWidth,
            m_game.GraphicsDevice.PresentationParameters.BackBufferHeight);

    public void amIPhoneInitNN(PresentationParameters parameters) =>
        amIPhoneInitNN(parameters.BackBufferWidth, parameters.BackBufferHeight);

    public void amIPhoneInitNN(int bbWidth, int bbHeight)
    {
        OpenGL.init(m_game, m_graphicsDevice);

        mppViewport = mppCalcViewport(bbWidth, bbHeight);
        OpenGL.glViewport(mppViewport.X, mppViewport.Y, mppViewport.Width, mppViewport.Height);

        float width = mppViewport.Width;
        float height = mppViewport.Height;
        amRenderInit(mppViewport.Width, mppViewport.Height);
        amRenderCreateWork(mppViewport.Width, mppViewport.Height);
        NNS_MATRIX mtx = GlobalPool<NNS_MATRIX>.Alloc();
        NNS_VECTOR nnsVector = new NNS_VECTOR(0.0f, 0.0f, -1f);
        NNS_RGBA nnsRgba = new NNS_RGBA(1f, 1f, 1f, 1f);
        NNS_CONFIG_GL config;
        config.WindowWidth = (int)width;
        config.WindowHeight = (int)height;
        this.nnConfigureSystemGL(config);

        var aspect = width / height;

        nnMakePerspectiveMatrix(mtx, NNM_DEGtoA32(45f), aspect, 1f, 10000f);
        nnSetProjection(mtx, 0);

        _am_draw_video.draw_aspect = aspect;
        _am_draw_video.draw_width = mppViewport.Width;
        _am_draw_video.draw_height = mppViewport.Height;
        _am_draw_video.disp_width = mppViewport.Width;
        _am_draw_video.disp_height = mppViewport.Height;

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

        GlobalPool<NNS_MATRIX>.Release(mtx);

        mppDiagSurface("initNN");
    }

    // TODO(wam): remove, iOS blank screen diagnostics
    public static void mppDiagSurface(string where)
    {
        var pp = m_game.GraphicsDevice.PresentationParameters;
        var vp = m_game.GraphicsDevice.Viewport;
        var bounds = m_game.Window.ClientBounds;
        var caps = Microsoft.Xna.Framework.Input.Touch.TouchPanel.GetCapabilities();

        Console.WriteLine($"[s4diag] {where} bb={pp.BackBufferWidth}x{pp.BackBufferHeight}"
            + $" vp={vp.X},{vp.Y},{vp.Width}x{vp.Height} client={bounds.Width}x{bounds.Height}"
            + $" 2d={_am_draw_video.width_2d}x{_am_draw_video.height_2d}"
            + $" scale={_am_draw_video.scale_x_2d},{_am_draw_video.scale_y_2d}"
            + $" base={_am_draw_video.base_x_2d},{_am_draw_video.base_y_2d}"
            + $" draw={_am_draw_video.draw_width}x{_am_draw_video.draw_height}"
            + $" touch={caps.IsConnected}/{caps.MaximumTouchCount}"
            + $" {mppDiagOrtho(pp.BackBufferWidth, pp.BackBufferHeight)}"
            // FNA writes this back to 0 when SDL_WINDOW_HIGH_PIXEL_DENSITY did not survive window
            // creation, so it says whether high DPI was refused rather than never asked for
            + $" highdpi={Environment.GetEnvironmentVariable("FNA_GRAPHICS_ENABLE_HIGHDPI")}"
            + $" mode={GraphicsAdapter.DefaultAdapter.CurrentDisplayMode}");
    }

    // the gameplay view in world units and the resulting pixel density per axis. the two densities
    // must agree, and ortho height must not move when the window aspect does
    private static string mppDiagOrtho(int bbWidth, int bbHeight)
    {
        if (mppViewport.Width <= 0 || mppViewport.Height <= 0 || g_obj.disp_height <= 0f)
            return "ortho=n/a";

        ObjCameraGetOrthoExtent(GMD_CAMERA_SCALE, AMD_SCREEN_ASPECT, out float dx, out float dy);
        float world_w = dx * 2f;
        float world_h = dy * 2f;

        return $"vp={mppViewport.X},{mppViewport.Y},{mppViewport.Width}x{mppViewport.Height}"
            + $" cap={mppAspectCap} bars={mppIsPillarboxed}"
            + $" ortho={world_w:F3}x{world_h:F3}"
            + $" px/unit={mppViewport.Width / world_w:F4},{mppViewport.Height / world_h:F4}"
            + $" lcd={GMD_OBJ_LCD_X}x{GMD_OBJ_LCD_Y}";
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
    // TODO(wam): remove, iOS blank screen diagnostics
    private static int _mppDiagLastTouchCount = -1;
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
        var originX = 0;
        var originY = 0;
        var width = 800;
        var height = 480;
#else
        // TouchPanel scales a finger by DisplayWidth/Height, which FNA sets from the back buffer,
        // so that is the space these coordinates are already in. ClientBounds is points rather than
        // pixels once high DPI is on, and dividing by it loses the display scale
        var originX = mppViewport.X;
        var originY = mppViewport.Y;
        var width = mppViewport.Width;
        var height = mppViewport.Height;
#endif

        if (width <= 0 || height <= 0 || g_ao_act_wide_w <= 0f)
            return;

        // Every hit rect in the game is written in a 480x320 space, and they were authored against
        // a 1080-wide 2D box, which is 3:2 at 720 tall: the iPhone. The art they sit on is
        // CORW_CENTER, so it moves with the window while a fixed screen fraction does not. Undo the
        // centring and re-express in the authoring frame, and the rects hold at any aspect.
        float wide_x = (X - originX) / width * g_ao_act_wide_w;
        float base_x = wide_x - (g_ao_act_wide_w - g_ao_act_base_w) * 0.5f;

        X = (base_x + (AOD_ACT_AUTHORED_WIDE_W - g_ao_act_base_w) * 0.5f)
                * AOD_ACT_SCREEN_WIDTH / AOD_ACT_AUTHORED_WIDE_W;
        Y = (Y - originY) / height * AOD_ACT_SCREEN_HEIGHT;
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