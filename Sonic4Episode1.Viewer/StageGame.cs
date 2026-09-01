using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Sonic4Episode1.Viewer;

// Window, camera and input for a StageScene. The scene itself knows about none of this, which is
// what lets the editor shell drive the same one later.
internal sealed class StageGame : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private readonly StageMap _map;

    private StageScene _scene;
    private StageMapFarScene _mapFar;
    private bool _showMapFar = true;
    private NnShaderCache _shaders;
    private RenderTarget2D _sceneTarget;
    private SpriteBatch _sprites;

    // the camera looks down -Z at the map plane, so its position is a point and a half width
    private Vector2 _eye;
    private float _span = 1200f;
    private bool _wireframe;
    private bool _reportNextFrame;
    private bool _reportedBlank;
    private int _lastDrawn = -1;
    private string _reportSummary;
    private KeyboardState _lastKeys;
    private MouseState _lastMouse;

    private static readonly Vector3 LightDirection = Vector3.Normalize(new Vector3(0.4f, 0.7f, 1.0f));
    private static readonly Color Background = new Color(0.10f, 0.11f, 0.13f);

    public StageGame(StageMap map)
    {
        _map = map;

        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = 1600,
            PreferredBackBufferHeight = 900,
            PreferMultiSampling = true,
        };

        Window.Title = $"Sonic 4 stage viewer - {map.Name}";
        Window.AllowUserResizing = true;
        IsMouseVisible = true;
    }

    protected override void LoadContent()
    {
        _shaders = new NnShaderCache(GraphicsDevice);

        // start the file fresh each run, then everything worth reading goes to it
        try
        {
            File.Delete(Path.Combine(AppContext.BaseDirectory, "stage-report.txt"));
        }
        catch (Exception)
        {
        }

        try
        {
            _scene = new StageScene(_map);
            _scene.Build(GraphicsDevice);
            WriteReport(_scene.Log);
            WriteReport($"{_scene.Blocks.Count} block placement(s), {_map.Layers.Count} layer(s)");

            var centre = (_scene.Bounds.Min + _scene.Bounds.Max) * 0.5f;
            _eye = new Vector2(centre.X, centre.Y);
            _span = Math.Max(_scene.Bounds.Max.X - _scene.Bounds.Min.X, 1f) * 0.25f;
        }
        catch (Exception ex)
        {
            _scene = null;
            WriteReport(ex.ToString());
        }

        try
        {
            if (_map.MapFar != null && _map.MapFar.Layout.Drawn)
            {
                _mapFar = new StageMapFarScene(_map);
                _mapFar.Build(GraphicsDevice);
                WriteReport(_mapFar.Log);
            }
        }
        catch (Exception ex)
        {
            _mapFar = null;
            WriteReport(ex.ToString());
        }
    }

    protected override void Update(GameTime gameTime)
    {
        var keys = Keyboard.GetState();
        var mouse = Mouse.GetState();
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (keys.IsKeyDown(Keys.Escape))
            Exit();

        float pan = _span * dt * 1.5f;
        if (keys.IsKeyDown(Keys.Left) || keys.IsKeyDown(Keys.A)) _eye.X -= pan;
        if (keys.IsKeyDown(Keys.Right) || keys.IsKeyDown(Keys.D)) _eye.X += pan;
        if (keys.IsKeyDown(Keys.Up) || keys.IsKeyDown(Keys.W)) _eye.Y += pan;
        if (keys.IsKeyDown(Keys.Down) || keys.IsKeyDown(Keys.S)) _eye.Y -= pan;

        if (keys.IsKeyDown(Keys.OemMinus) || keys.IsKeyDown(Keys.PageDown)) Zoom(1f + dt, null);
        if (keys.IsKeyDown(Keys.OemPlus) || keys.IsKeyDown(Keys.PageUp)) Zoom(1f - dt, null);

        if (mouse.ScrollWheelValue != _lastMouse.ScrollWheelValue)
        {
            float steps = (mouse.ScrollWheelValue - _lastMouse.ScrollWheelValue) / 120f;
            Zoom(MathF.Pow(0.85f, steps), mouse);
        }

        Drag(mouse);

        if (Pressed(keys, Keys.G))
            _wireframe = !_wireframe;

        if (Pressed(keys, Keys.F1))
            _reportNextFrame = true;

        if (Pressed(keys, Keys.T) && _scene != null)
        {
            _scene.NoAlphaTest = !_scene.NoAlphaTest;
            _reportSummary = $"alpha test {(_scene.NoAlphaTest ? "BYPASSED" : "on")}";
        }

        if (Pressed(keys, Keys.Y) && _scene != null)
        {
            _scene.NoBlend = !_scene.NoBlend;
            _reportSummary = $"blend {(_scene.NoBlend ? "FORCED OPAQUE" : "from material")}";
        }

        if (Pressed(keys, Keys.O) && _scene != null)
        {
            _scene.ShowMarkers = !_scene.ShowMarkers;
            _reportSummary = $"markers {(_scene.ShowMarkers ? "on" : "off")}, {_scene.Markers.Count} of them";
        }

        if (Pressed(keys, Keys.K) && _scene != null)
        {
            _scene.ShowDecorations = !_scene.ShowDecorations;
            _reportSummary = $"decorations {(_scene.ShowDecorations ? "on" : "off")},"
                + $" {_scene.Decorations.Count} placed";
        }

        for (int i = 0; i < 9 && _scene != null && i < _scene.LayerCount; i++)
        {
            if (Pressed(keys, Keys.D1 + i))
            {
                _scene.SetLayerVisible(i, !_scene.IsLayerVisible(i));
                _reportSummary = $"{_map.Layers[i].Name} {(_scene.IsLayerVisible(i) ? "on" : "off")}"
                    + $", {_scene.Blocks.Count} placement(s)";
            }
        }

        if (Pressed(keys, Keys.B) && _mapFar != null)
        {
            _showMapFar = !_showMapFar;
            _reportSummary = $"background {(_showMapFar ? "on" : "off")}";
        }

        if (_scene != null && mouse.LeftButton == ButtonState.Pressed && _lastMouse.LeftButton == ButtonState.Released)
            Pick(mouse);

        // the scroll tables are driven off the main camera, and the sky turns on its own
        _mapFar?.Update(dt * 60f, _eye);

        _lastKeys = keys;
        _lastMouse = mouse;
        base.Update(gameTime);
    }

    // holding the right or middle button drags the map under the cursor, so a drag moves the world
    // by exactly what the cursor moved
    private void Drag(MouseState mouse)
    {
        bool held = mouse.RightButton == ButtonState.Pressed || mouse.MiddleButton == ButtonState.Pressed;
        bool wasHeld = _lastMouse.RightButton == ButtonState.Pressed
            || _lastMouse.MiddleButton == ButtonState.Pressed;

        if (!held || !wasHeld)
            return;

        var half = HalfExtent();
        _eye.X -= (mouse.X - _lastMouse.X) * 2f * half.X / GraphicsDevice.Viewport.Width;
        _eye.Y += (mouse.Y - _lastMouse.Y) * 2f * half.Y / GraphicsDevice.Viewport.Height;
    }

    // scaling about the cursor rather than the middle keeps whatever you are pointing at put, which
    // is what makes wheel zoom usable on a map this long
    private void Zoom(float factor, MouseState? about)
    {
        var before = about.HasValue ? WorldAt(about.Value) : _eye;

        _span = MathHelper.Clamp(_span * factor, 64f, 40000f);

        if (about.HasValue)
            _eye += before - WorldAt(about.Value);
    }

    // the cursor can be off the window when a wheel event arrives, and an unclamped u or v would
    // throw the eye clean off the map
    private Vector2 WorldAt(MouseState mouse)
    {
        var half = HalfExtent();
        float u = MathHelper.Clamp(mouse.X / (float)GraphicsDevice.Viewport.Width, 0f, 1f);
        float v = MathHelper.Clamp(mouse.Y / (float)GraphicsDevice.Viewport.Height, 0f, 1f);

        return _eye + new Vector2(
            MathHelper.Lerp(-half.X, half.X, u),
            MathHelper.Lerp(half.Y, -half.Y, v));
    }

    private void Pick(MouseState mouse)
    {
        var half = HalfExtent();
        float u = mouse.X / (float)GraphicsDevice.Viewport.Width;
        float v = mouse.Y / (float)GraphicsDevice.Viewport.Height;

        var offset = new Vector2(
            MathHelper.Lerp(-half.X, half.X, u),
            MathHelper.Lerp(half.Y, -half.Y, v));

        var picked = _scene.PickAt(_eye, offset);

        _reportSummary = picked == null
            ? "no block there"
            : $"{_map.Layers[picked.LayerIndex].Name} cell {picked.CellX},{picked.CellY}"
            + $" id {picked.Mp.id} rot {picked.Mp.rot} flip {picked.Mp.flip_h}{picked.Mp.flip_v}";

        WriteReport(_reportSummary);
    }

    private Vector2 HalfExtent()
    {
        return new Vector2(_span, _span / GraphicsDevice.Viewport.AspectRatio);
    }

    private void EnsureSceneTarget()
    {
        int w = GraphicsDevice.PresentationParameters.BackBufferWidth;
        int h = GraphicsDevice.PresentationParameters.BackBufferHeight;

        if (_sceneTarget != null && _sceneTarget.Width == w && _sceneTarget.Height == h)
            return;

        _sceneTarget?.Dispose();
        _sceneTarget = new RenderTarget2D(
            GraphicsDevice, w, h, false, SurfaceFormat.Color, DepthFormat.Depth24);
    }

    private void Blit(Texture2D texture)
    {
        _sprites ??= new SpriteBatch(GraphicsDevice);
        _sprites.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null);
        _sprites.Draw(texture, GraphicsDevice.Viewport.Bounds, Color.White);
        _sprites.End();
    }

    private bool Pressed(KeyboardState keys, Keys key)
    {
        return keys.IsKeyDown(key) && _lastKeys.IsKeyUp(key);
    }

    // launched without a console there is nowhere for Console.WriteLine to go, so the report goes
    // to a file beside the exe and its path onto the title bar
    private static readonly string ReportPath =
        Path.Combine(AppContext.BaseDirectory, "stage-report.txt");

    private void WriteReport(string text)
    {
        try
        {
            File.AppendAllText(ReportPath, text + Environment.NewLine);
        }
        catch (Exception)
        {
        }

        Console.WriteLine(text);
    }

    protected override void Draw(GameTime gameTime)
    {
        // gmMapFarClearColor: the background owns the clear, and zone 1's is the one colour the two
        // arms visibly disagree on
        var clear = _mapFar != null && _showMapFar ? _mapFar.ClearColour : Background;
        GraphicsDevice.Clear(clear);

        if (_scene == null)
        {
            base.Draw(gameTime);
            return;
        }

        // no blend, depth or cull set here: every mesh carries its own and StageScene applies it
        _scene.Wireframe = _wireframe;

        var half = HalfExtent();

        // the frame after one that drew nothing explains itself without being asked, since that is
        // the case worth explaining and it is awkward to catch with a keypress
        if (_reportNextFrame || (_lastDrawn == 0 && _scene.Blocks.Count > 0 && !_reportedBlank))
        {
            _reportedBlank |= _lastDrawn == 0;
            _reportNextFrame = false;
            _scene.Report = new System.Text.StringBuilder();
            _scene.Report.AppendLine($"eye {_eye.X:0.0},{_eye.Y:0.0}  span {_span:0.0}"
                + $"  half {half.X:0.0} x {half.Y:0.0}  map {_map.MainMapSize.X} x {_map.MainMapSize.Y}");
        }

        // orthographic: the map is flat, and parallax moves each layer's camera rather than relying
        // on perspective, which is what gmMapMain does
        var projection = Matrix.CreateOrthographic(
            half.X * 2f, half.Y * 2f, 1f, StageScene.CameraHeight * 2f);

        void DrawMapFar()
        {
            if (_mapFar == null || !_showMapFar)
                return;

            var viewport = GraphicsDevice.Viewport;
            _mapFar.Draw(GraphicsDevice, _shaders, LightDirection, viewport.AspectRatio);

            // map far leaves depth behind it; the block layers own the depth buffer from here
            GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Black, 1f, 0);
        }

        int drawn;
        if (_scene.HasWater)
        {
            // gmMapFallShaderSettingPrioPreMidMapUserFunc copies the render target before the
            // waterfalls draw, so they can sample what is behind them. Two passes here: everything
            // but the waterfalls into a target, that target back onto the screen, then the
            // waterfalls over it with it bound.
            EnsureSceneTarget();

            GraphicsDevice.SetRenderTarget(_sceneTarget);
            GraphicsDevice.Clear(clear);
            DrawMapFar();
            _scene.Draw(GraphicsDevice, _shaders, _eye, half, projection, LightDirection);

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(clear);
            Blit(_sceneTarget);

            drawn = _scene.Draw(GraphicsDevice, _shaders, _eye, half, projection, LightDirection, _sceneTarget);
        }
        else
        {
            DrawMapFar();
            drawn = _scene.Draw(GraphicsDevice, _shaders, _eye, half, projection, LightDirection);
        }

        _lastDrawn = drawn;

        if (_scene.Report != null)
        {
            WriteReport(_scene.Report.ToString());
            _scene.Report = null;
            _reportSummary = $"report written to {ReportPath}";
        }

        Window.Title = $"Sonic 4 stage viewer - {_map.Name} - {drawn} of {_scene.Blocks.Count} block(s)"
            + (_reportSummary != null ? "  |  " + _reportSummary : "");

        base.Draw(gameTime);
    }
}
