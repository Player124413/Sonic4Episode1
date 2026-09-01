// Decompiled with JetBrains decompiler
// Type: Sonic4Ep1
// Assembly: Sonic4 ep I, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 093CE2FC-33E2-4332-B0FE-1EA1E44D3AE7
// Assembly location: C:\Users\wamwo\Documents\GitHub\Sonic4Ep1-WP7-Decompilation\XAP\Sonic4 ep I.dll


using System;
using System.Diagnostics;
using gs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using mpp;
using GameFramework;
using System.Linq;


#if WINDOWSPHONE7_5 || WASM || IOS || ANDROID
#if WINDOWSPHONE7_5
using System.Windows;
#endif
using Microsoft.Devices.Sensors;
#endif

public class Sonic4Ep1 : Game
{
    internal static bool cheat = false;
    private static bool inputDataRead = true;
    //internal SpriteFont[] fnts = new SpriteFont[3];
    //private WeakReference wr = new WeakReference(new object());
    protected bool storeSystemVolume = true;
    internal static Sonic4Ep1 pInstance;
    private GraphicsDeviceManager graphics;
    //private SpriteFont fntKootenay;
    //private int GCCount;
    //private double _lastUpdateMilliseconds;
    internal RasterizerState scissorState;
    private AppMain appMain;
    protected float deviceMusicVolume;
    private Vector3 accel;
    //private long timestamp;

#if SHOW_FPS
    internal SpriteBatch spriteBatch;
    private BenchmarkObject _benchmark;
#endif

    private string storePath;

#if WINDOWSPHONE7_5 || WASM || IOS || ANDROID
    private Accelerometer _accelerometer;
#endif

    public Sonic4Ep1()
    {
        pInstance = this;

#if WINDOWSPHONE7_5
        // hacky exception handler
        Application.Current.UnhandledException += (object sender, ApplicationUnhandledExceptionEventArgs e) =>
        {
            var result = MessageBox.Show(e.ExceptionObject.ToString(), "An error occurred", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.Cancel)
                e.Handled = true;
        };
#endif

#if WINDOWSPHONE7_5
        AppMain.storePath = storePath = ""; // unused, works via IsolatedStorage
#elif WINDOWS_UAP
        AppMain.storePath = storePath = Windows.Storage.ApplicationData.Current.RoamingFolder.Path;
#elif ANDROID
        AppMain.storePath = storePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
#elif IOS
        AppMain.storePath = storePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
#else
        AppMain.storePath = storePath = System.IO.Directory.GetCurrentDirectory();
#endif

        this.graphics = new GraphicsDeviceManager(this);
#if WINDOWSPHONE7_5
        this.graphics.PreferredBackBufferWidth = 533;
        this.graphics.PreferredBackBufferHeight = 320;
#elif IOS
        var displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
        GetFixedWidthBackBuffer(displayMode.Width, displayMode.Height, out var preferredWidth, out var preferredHeight);
        this.graphics.PreferredBackBufferWidth = preferredWidth;
        this.graphics.PreferredBackBufferHeight = preferredHeight;

        System.Diagnostics.Debug.WriteLine(displayMode);
#elif WINDOWS_UAP
        this.graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        this.graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
#else
        this.graphics.PreferredBackBufferWidth = 1920;
        this.graphics.PreferredBackBufferHeight = 1080;
#endif
#if WINDOWS_UAP || IOS || ANDROID || WINDOWSPHONE7_5
        this.graphics.IsFullScreen = true;
        this.graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
#else
        this.graphics.IsFullScreen = false;
#endif
        this.graphics.SynchronizeWithVerticalRetrace = true;
        this.graphics.PreparingDeviceSettings += (object sender, PreparingDeviceSettingsEventArgs e) =>
        {
            e.GraphicsDeviceInformation.PresentationParameters.PresentationInterval = PresentInterval.One;
#if !IOS && !ANDROID
            e.GraphicsDeviceInformation.PresentationParameters.MultiSampleCount = 4;
#endif

#if IOS
            /* FNA subscribes to ClientSizeChanged itself and re-pins the back buffer to the window,
             * which beats PreferredBackBuffer. This event is raised at the end of
             * INTERNAL_CreateGraphicsDeviceInformation, after that has happened and on both the
             * CreateDevice and ApplyChanges paths, so it is the only place the fixed width sticks.
             */
            var pp = e.GraphicsDeviceInformation.PresentationParameters;
            GetFixedWidthBackBuffer(pp.BackBufferWidth, pp.BackBufferHeight, out var bbWidth, out var bbHeight);
            pp.BackBufferWidth = bbWidth;
            pp.BackBufferHeight = bbHeight;
#endif

#if ANDROID
            var largestSupported = e.GraphicsDeviceInformation.Adapter.SupportedDisplayModes
                .OrderByDescending(m => m.Width * m.Height)
                .FirstOrDefault(m => m.Format == SurfaceFormat.Color);

            if (largestSupported != null)
            {
                this.graphics.PreferredBackBufferWidth = largestSupported.Width;
                this.graphics.PreferredBackBufferHeight = largestSupported.Height;
            }
#endif
        };

#if ANDROID
        this.Content = new AndroidContentManager(this.Services);
#endif
        this.Content.RootDirectory = "Content";

        this.IsMouseVisible = true;
        this.IsFixedTimeStep = true;
        this.TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 60);

        this.Activated += new EventHandler<EventArgs>(OnActivated);
        this.Deactivated += new EventHandler<EventArgs>(OnDeactivated);

        this.Window.Title = "SONIC THE HEDGEHOG 4: Episode I";
        this.Window.AllowUserResizing = true;
        this.Window.ClientSizeChanged += OnClientSizeChanged;
    }

#if IOS
    // TODO(wam): revisit when ultrawide gets a real pass
    private const int FixedBackBufferWidth = 1920;

    // the surface arrives portrait when the orientation lock has not settled yet, and we only ever
    // run landscape, so take the long side as the width rather than trusting which way round it is
    private static void GetFixedWidthBackBuffer(int surfaceWidth, int surfaceHeight, out int width, out int height)
    {
        var longSide = Math.Max(surfaceWidth, surfaceHeight);
        var shortSide = Math.Min(surfaceWidth, surfaceHeight);

        width = FixedBackBufferWidth;
        height = longSide > 0
            ? (int)Math.Round(FixedBackBufferWidth * (double)shortSide / longSide)
            : 1080;
    }
#endif

    private bool resizedFlag = false;
    private int newWidth = 0, newHeight = 0;
    private void OnClientSizeChanged(object sender, EventArgs e)
    {
        resizedFlag = true;
    }

    protected override void Initialize()
    {
        this.scissorState = new RasterizerState() { ScissorTestEnable = true, CullMode = CullMode.None };
        Guide.IsScreenSaverEnabled = false;
        //LiveFeature.GAME = this;
        //LiveFeature.getInstance();

#if WINDOWSPHONE7_5 || WASM || IOS || ANDROID
        try
        {
            if (_accelerometer == null)
                _accelerometer = new Accelerometer();
            _accelerometer.TimeBetweenUpdates = TimeSpan.FromSeconds(1.0 / 30.0);
            _accelerometer.CurrentValueChanged += (object sender, SensorReadingEventArgs<AccelerometerReading> args) =>
                accel = new Vector3(args.SensorReading.Acceleration.X, args.SensorReading.Acceleration.Y, args.SensorReading.Acceleration.Z);
            _accelerometer.Start();
        }
        catch (AccelerometerFailedException)
        {
            _accelerometer = null;
        }
#endif
        base.Initialize();
    }

    protected override void LoadContent()
    {
        //this.fntKootenay = this.Content.Load<SpriteFont>("Kootenay");
        //this.fnts[0] = this.Content.Load<SpriteFont>("small");
        //this.fnts[2] = this.Content.Load<SpriteFont>("large");

#if SHOW_FPS
        this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
        var font = this.Content.Load<SpriteFont>("medium");
        _benchmark = new BenchmarkObject(this, font, Vector2.Zero, Color.Red);
        _benchmark.IsEnabled = true;
#endif

        var config = XmlStorage.Load() ?? new Sonic4Save();
        var backup = gs.backup.SSave.CreateInstance();
        backup.SetSave(config);

        XmlStorage.Save(config, false, false);

        this.appMain = new AppMain(this, this.graphics, this.GraphicsDevice);
        this.appMain.AppInit(storePath);
    }

    protected override void OnDeactivated(object sender, EventArgs args)
    {
        AppMain.isForeground = false;
        if (SaveState.saveLater)
            SaveState._saveFile(SaveState.save);

#if WINDOWSPHONE7_5
        if (Guide.IsVisible)
        {
            storeSystemVolume = false;
            return;
        }

        storeSystemVolume = true;
        try
        {
            if (!AppMain.g_ao_sys_global.is_playing_device_bgm_music)
                MediaPlayer.Pause();
            MediaPlayer.Volume = deviceMusicVolume;
        }
        catch (Exception) { }
#endif
    }

    protected override void OnActivated(object sender, EventArgs args)
    {
        AppMain.isForeground = true;

        if (storeSystemVolume)
            deviceMusicVolume = Microsoft.Xna.Framework.Media.MediaPlayer.Volume;

        if (((int)(AppMain.g_gm_main_system?.game_flag ?? 0) & 64) == 0)
            AppMain.g_pause_flag = true;
    }

    protected override void UnloadContent()
    {
    }

    protected override void Update(GameTime gameTime)
    {
        if (resizedFlag)
        {
            graphics.PreferredBackBufferWidth = graphics.GraphicsDevice.Viewport.Width;
            graphics.PreferredBackBufferHeight = graphics.GraphicsDevice.Viewport.Height;
            graphics.ApplyChanges();

            if (appMain != null)
            {
                appMain.amIPhoneInitNN(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            }

            resizedFlag = false;
        }

        AppMain.lastGameTime = gameTime;
        if (!AppMain.g_ao_sys_global.is_show_ui && inputDataRead)
        {
            //inputDataRead = false;
            if (/*!LiveFeature.getInstance().InputOverride() && !Upsell.inputUpsellScreen() &&*/ AppMain.isForeground)
            {
                AppMain.onTouchEvents();
                AppMain.amIPhoneAccelerate(ref this.accel);
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    AppMain.back_key_is_pressed = true;
            }
        }

#if SHOW_FPS
        _benchmark?.Update(gameTime);
#endif

#if SPLIT_LOOP
        inputDataRead = true;
        OpenGL.drawPrimitives_Count = 0;
        OpenGL.drawVertexBuffer_Count = 0;
        this.appMain.AppMainUpdate();
#endif

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
#if SPLIT_LOOP
        this.appMain.AppMainDraw();
#else
        this.appMain.AppMainLoop();
#endif
#if SHOW_FPS
        this.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
        _benchmark.Draw(gameTime, this.spriteBatch);
        this.spriteBatch.End();
#endif

        // these are disabled but we might as well keep them around
        // LiveFeature.getInstance().ShowOverride();
        // Upsell.drawUpsellScreen();
        base.Draw(gameTime);
    }
}