using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Sonic4Episode1.Viewer;

internal sealed class ViewerGame : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private readonly string _modelPath;
    private readonly string _texturePath;
    private readonly string _motionPath;

    // -1 is the bind pose, which is also what the viewer shows when there is no motion archive
    private int _motionIndex = -1;
    private float _frame;
    private bool _playing = true;

    private BasicEffect _effect;
    private readonly Dictionary<string, NnStdShaderEffect> _shaders =
        new Dictionary<string, NnStdShaderEffect>(StringComparer.OrdinalIgnoreCase);
    private LoadedModel _model;
    private string _error;

    private float _yaw = 0.6f;
    private float _pitch = 0.4f;
    private float _distance;
    private bool _flipV;
    private bool _wireframe;
    private bool _spin = true;
    private bool _useUber = true;
    // confirmed by eye on the HD ring and Sonic: strip conversion emits CCW front faces, so
    // CullClockwiseFace is correct and CullCounterClockwiseFace is visibly inverted
    private CullMode _cullMode = CullMode.CullClockwiseFace;
    private KeyboardState _lastKeys;

    // world space, so the lighting stays put while the camera orbits
    private static readonly Vector3 LightDirection = Vector3.Normalize(new Vector3(0.4f, 0.7f, 1.0f));
    private static readonly Vector3 LightDiffuse = new Vector3(1.0f, 0.98f, 0.92f);
    private static readonly Vector3 LightSpecular = new Vector3(1.0f, 1.0f, 1.0f);
    private static readonly Vector3 SceneAmbient = new Vector3(0.35f, 0.35f, 0.4f);
    private static readonly Color Background = new Color(0.10f, 0.11f, 0.13f);

    public ViewerGame(string modelPath, string texturePath, string motionPath)
    {
        _modelPath = modelPath;
        _texturePath = texturePath;
        _motionPath = motionPath;

        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = 1280,
            PreferredBackBufferHeight = 720,
            PreferMultiSampling = true,
        };

        Window.Title = "Sonic 4 model viewer - " + Path.GetFileName(modelPath);
        Window.AllowUserResizing = true;
        IsMouseVisible = true;
    }

    protected override void LoadContent()
    {
        _effect = new BasicEffect(GraphicsDevice)
        {
            LightingEnabled = true,
            PreferPerPixelLighting = true,
            SpecularPower = 24f,
        };
        _effect.EnableDefaultLighting();

        Reload();
    }

    private NnStdShaderEffect Shader(string permutation)
    {
        if (permutation == null)
            return null;

        NnStdShaderEffect shader;
        if (_shaders.TryGetValue(permutation, out shader))
            return shader;

        shader = LoadUberShader(permutation);
        _shaders[permutation] = shader;
        return shader;
    }

    // read straight out of the repo rather than copied to the output, it's 284 files and a tool
    private static string _shaderDir;

    private NnStdShaderEffect LoadUberShader(string permutation)
    {
        _shaderDir ??= FindRepoDirectory("Shaders/FNA/nnstd");
        if (_shaderDir == null)
        {
            Console.Error.WriteLine("could not find Shaders/FNA/nnstd; run Shaders/nn/Build-Permutations.ps1");
            return null;
        }

        string path = Path.Combine(_shaderDir, permutation + ".fxb");
        if (!File.Exists(path))
        {
            Console.Error.WriteLine($"no shader built for key {permutation}");
            return null;
        }

        try
        {
            var effect = new NnStdShaderEffect(GraphicsDevice, File.ReadAllBytes(path));
            Console.WriteLine($"ubershader {permutation}: {effect.Parameters.Count} parameter(s)");
            return effect;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"ubershader {permutation} failed to load: " + ex);
            return null;
        }
    }

    private static string FindRepoDirectory(string relative)
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            string candidate = Path.Combine(dir.FullName, relative);
            if (Directory.Exists(candidate))
                return candidate;
            dir = dir.Parent;
        }
        return null;
    }

    private void Reload()
    {
        try
        {
            _model = DXModelLoader.Load(GraphicsDevice, _modelPath, _texturePath, _motionPath, _flipV);
            _error = null;
            _motionIndex = _model.Motions.Count > 0 ? 0 : -1;
            _frame = 0f;

            var size = _model.Bounds.Max - _model.Bounds.Min;
            _distance = Math.Max(size.Length() * 1.6f, 1f);

            Console.WriteLine(_model.Summary);
            Console.WriteLine($"total {_model.Objects.Sum(o => o.Meshes.Count)} mesh(es)"
                + $" across {_model.Objects.Count} object(s)");
            Console.WriteLine($"shader = {(_useUber ? "nnstdshader" : "BasicEffect")}");
        }
        catch (Exception ex)
        {
            _error = ex.ToString();
            _model = null;
            Console.Error.WriteLine(_error);
        }
    }

    protected override void Update(GameTime gameTime)
    {
        var keys = Keyboard.GetState();
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (keys.IsKeyDown(Keys.Escape))
            Exit();

        if (Pressed(keys, Keys.F))
        {
            _flipV = !_flipV;
            Console.WriteLine($"flipV = {_flipV}");
            Reload();
        }

        if (Pressed(keys, Keys.U))
        {
            _useUber = !_useUber;
            Console.WriteLine($"shader = {(_useUber ? "nnstdshader" : "BasicEffect")}");
        }

        if (Pressed(keys, Keys.C))
        {
            _cullMode = NextCullMode(_cullMode);
            Console.WriteLine($"cull = {_cullMode}");
        }

        if (Pressed(keys, Keys.W))
            _wireframe = !_wireframe;

        if (Pressed(keys, Keys.Space))
            _spin = !_spin;

        UpdateMotion(keys, dt);

        if (keys.IsKeyDown(Keys.Left))
            _yaw -= dt * 2f;
        if (keys.IsKeyDown(Keys.Right))
            _yaw += dt * 2f;
        if (keys.IsKeyDown(Keys.Up))
            _pitch = MathHelper.Clamp(_pitch + dt * 2f, -1.5f, 1.5f);
        if (keys.IsKeyDown(Keys.Down))
            _pitch = MathHelper.Clamp(_pitch - dt * 2f, -1.5f, 1.5f);
        if (keys.IsKeyDown(Keys.OemMinus) || keys.IsKeyDown(Keys.PageDown))
            _distance *= 1f + dt;
        if (keys.IsKeyDown(Keys.OemPlus) || keys.IsKeyDown(Keys.PageUp))
            _distance *= 1f - dt;

        if (_spin)
            _yaw += dt * 0.6f;

        _lastKeys = keys;
        base.Update(gameTime);
    }

    // motion plays at its own FrameRate, not the display's; fall back to 30 if it states none
    private void UpdateMotion(KeyboardState keys, float dt)
    {
        if (_model == null)
            return;

        if (Pressed(keys, Keys.M) && _model.Motions.Count > 0)
        {
            _motionIndex = _motionIndex + 1 >= _model.Motions.Count ? -1 : _motionIndex + 1;
            _frame = 0f;
            Console.WriteLine(DescribeMotion());
        }

        if (Pressed(keys, Keys.P))
        {
            _playing = !_playing;
            Console.WriteLine($"playback {(_playing ? "running" : "paused")}");
        }

        var current = _motionIndex >= 0 && _motionIndex < _model.Motions.Count
            ? _model.Motions[_motionIndex]
            : null;

        if (current != null)
        {
            float rate = current.FrameRate > 0f ? current.FrameRate : 30f;
            float step = Pressed(keys, Keys.OemCloseBrackets) ? 1f
                : Pressed(keys, Keys.OemOpenBrackets) ? -1f
                : _playing ? dt * rate : 0f;

            if (step != 0f)
            {
                float span = current.EndFrame - current.StartFrame;
                _frame += step;

                // nnCalcMotionFrame treats a motion as valid on [StartFrame, EndFrame); landing
                // exactly on EndFrame falls back to the bind pose for that one frame
                if (span > 0f)
                {
                    while (_frame >= current.EndFrame)
                        _frame -= span;
                    while (_frame < current.StartFrame)
                        _frame += span;
                }
                else
                {
                    _frame = current.StartFrame;
                }
            }
        }

        _model.SetPose(_motionIndex, _frame);
    }

    private string DescribeMotion()
    {
        return _model.Describe(_motionIndex);
    }

    private static CullMode NextCullMode(CullMode mode)
    {
        switch (mode)
        {
            case CullMode.None: return CullMode.CullClockwiseFace;
            case CullMode.CullClockwiseFace: return CullMode.CullCounterClockwiseFace;
            default: return CullMode.None;
        }
    }

    private bool Pressed(KeyboardState keys, Keys key)
    {
        return keys.IsKeyDown(key) && _lastKeys.IsKeyUp(key);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Background);

        if (_model == null || _model.Objects.Count == 0)
        {
            base.Draw(gameTime);
            return;
        }

        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        GraphicsDevice.BlendState = BlendState.Opaque;
        GraphicsDevice.RasterizerState = new RasterizerState
        {
            FillMode = _wireframe ? FillMode.WireFrame : FillMode.Solid,
            CullMode = _wireframe ? CullMode.None : _cullMode,
        };

        var centre = (_model.Bounds.Min + _model.Bounds.Max) * 0.5f;
        var eye = centre + new Vector3(
            (float)(Math.Cos(_pitch) * Math.Sin(_yaw)),
            (float)Math.Sin(_pitch),
            (float)(Math.Cos(_pitch) * Math.Cos(_yaw))) * _distance;

        float aspect = GraphicsDevice.Viewport.AspectRatio;
        Matrix world = Matrix.Identity;
        Matrix view = Matrix.CreateLookAt(eye, centre, Vector3.Up);
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect, _distance * 0.01f, _distance * 10f);

        if (_useUber)
            DrawWithUberShader(world, view, projection);
        else
            DrawWithBasicEffect(world, view, projection);

        base.Draw(gameTime);
    }

    private void DrawWithBasicEffect(Matrix world, Matrix view, Matrix projection)
    {
        GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        _effect.World = world;
        _effect.View = view;
        _effect.Projection = projection;

        foreach (var loaded in _model.Objects)
        foreach (var mesh in loaded.Meshes)
        {
            if (loaded.IsHidden(mesh.NodeIndex))
                continue;

            _effect.Texture = mesh.Texture;
            _effect.TextureEnabled = mesh.Texture != null;
            _effect.DiffuseColor = mesh.DiffuseColor;

            GraphicsDevice.SetVertexBuffer(mesh.Vertices);
            GraphicsDevice.Indices = mesh.Indices;

            foreach (var pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, mesh.VertexCount, 0, mesh.TriangleCount);
            }
        }
    }

    private static Matrix MatrixFor(LoadedObject loaded, int paletteIndex, Matrix modelView)
    {
        var palette = loaded.Palette;
        if (paletteIndex < 0 || paletteIndex >= palette.Length)
            return modelView;

        return palette[paletteIndex] * modelView;
    }

    // palette holds the pose in model space, camera folds in here rather than in the node walk
    private static Matrix[] GatherBones(LoadedObject loaded, LoadedMesh mesh, Matrix modelView)
    {
        int[] indices = mesh.BoneMatrixIndices;
        if (indices == null)
            return mesh.BoneMatrices;

        for (int i = 0; i < indices.Length; ++i)
            mesh.BoneMatrices[i] = MatrixFor(loaded, indices[i], modelView);

        return mesh.BoneMatrices;
    }

    private void DrawWithUberShader(Matrix world, Matrix view, Matrix projection)
    {
        Vector3 lightView = Vector3.TransformNormal(LightDirection, view);
        Matrix modelView = world * view;

        foreach (var loaded in _model.Objects)
        foreach (var mesh in loaded.Meshes)
        {
            // nnDrawObject skips a meshset whose node the hide motion switched off (NnDraw.cs:456)
            if (mesh.Material == null || loaded.IsHidden(mesh.NodeIndex))
                continue;

            var uber = Shader(mesh.Permutation);
            if (uber == null)
                continue;

            // rigid takes its model view straight from the palette, pliable carries it in the bones
            if (mesh.VtxTfmMode == 0)
            {
                uber.SetTransforms(MatrixFor(loaded, mesh.MatrixIndex, modelView), projection);
            }
            else
            {
                uber.SetTransforms(modelView, projection);
                uber.SetBoneMatrices(GatherBones(loaded, mesh, modelView));
            }

            uber.SetParallelLight(0, lightView, LightDiffuse, LightSpecular);

            // pushed past the model so the viewer sees no fog, but the uniforms still have to be real
            uber.SetFog(Background.ToVector3(), _distance * 8f, _distance * 10f);

            uber.SetMaterial(mesh.Material.pColor, SceneAmbient);
            uber.SetTextureMatrices(mesh.Material);
            uber.SetMapBlend(mesh.Material);

            for (int slot = 0; slot < mesh.SamplerTextures.Length; ++slot)
            {
                GraphicsDevice.Textures[slot] = mesh.SamplerTextures[slot];
                if (mesh.SamplerStates[slot] != null)
                    GraphicsDevice.SamplerStates[slot] = mesh.SamplerStates[slot];
            }

            GraphicsDevice.SetVertexBuffer(mesh.Vertices);
            GraphicsDevice.Indices = mesh.Indices;

            foreach (var pass in uber.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, mesh.VertexCount, 0, mesh.TriangleCount);
            }
        }
    }
}
