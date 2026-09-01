using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sonic4Episode1.Viewer;

// Everything drawn for one stage, built once. Owns the GPU resources and nothing else: no window,
// no input, no camera. StageGame hands it a view and a projection; the WPF shell will do the same.
internal sealed class StageScene : IDisposable
{
    public const float BlockSize = StagePlacement.BlockSize;

    public StageMap Map { get; }
    public List<StageBlock> Blocks { get; } = new List<StageBlock>();
    public BoundingBox Bounds { get; private set; }
    public string Log { get; private set; } = "";

    // by block id. console indexes members straight, so index 0 is the blank member and stays null
    private LoadedObject[] _models = Array.Empty<LoadedObject>();
    private Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();
    private readonly bool[] _layerVisible;

    // the same blocks as Blocks, split by layer: each one draws through its own parallax camera
    private readonly List<StageBlock>[] _layerBlocks;

    // user0 & WATER, by block id. Those are the waterfalls, and they refract what is behind them,
    // so they draw in a second pass with the first pass bound as a texture.
    private bool[] _isWater = Array.Empty<bool>();

    public bool HasWater { get; private set; }

    public bool IsWater(StageBlock block) =>
        block.Mp.id < _isWater.Length && _isWater[block.Mp.id];

    // layer indices ordered by PosZ, furthest first
    private readonly int[] _byDepth;

    // decorations, by member index in DECO_ZONEx.AMB members 0 and 4. Both indexed straight, unlike
    // the mobile TVX arm which indexes id - 1.
    private LoadedObject[] _decoModels = Array.Empty<LoadedObject>();
    private LoadedObject[] _fallModels = Array.Empty<LoadedObject>();
    private Dictionary<string, Texture2D> _decoTextures = new Dictionary<string, Texture2D>();
    private Dictionary<string, Texture2D> _fallTextures = new Dictionary<string, Texture2D>();

    public List<StageDecoration> Decorations { get; } = new List<StageDecoration>();

    public bool ShowDecorations { get; set; } = true;

    public StageScene(StageMap map)
    {
        Map = map;
        _layerVisible = new bool[map.Layers.Count];
        _layerBlocks = new List<StageBlock>[map.Layers.Count];

        for (int i = 0; i < _layerVisible.Length; i++)
        {
            _layerVisible[i] = i < 2 || i - 2 < StageTable.AddLayerCount(map.StageId);
            _layerBlocks[i] = new List<StageBlock>();
        }

        _byDepth = Enumerable.Range(0, map.Layers.Count)
            .OrderBy(i => map.Layers[i].PosZ)
            .ToArray();
    }

    public int LayerCount => Map.Layers.Count;

    public bool IsLayerVisible(int index) => _layerVisible[index];

    public void SetLayerVisible(int index, bool visible)
    {
        _layerVisible[index] = visible;
        Rebuild();
    }

    public void Build(GraphicsDevice device)
    {
        if (Map.ModelFormat != StageModelFormat.NnObject)
            throw new NotSupportedException($"{Map.Name} ships {Map.ModelFormat} models, which the stage scene cannot build yet");

        var log = new List<string>();
        var min = new Vector3(float.MaxValue);
        var max = new Vector3(float.MinValue);

        _textures = DXModelLoader.LoadTextures(device, Map.TextureArchive, log);
        _models = new LoadedObject[Map.ModelCount];

        _isWater = new bool[Map.ModelCount];

        int built = 0;
        int water = 0;
        for (int i = 0; i < Map.ModelCount; i++)
        {
            var chunk = AppMain.amBindGet(Map.ModelArchive, i, out _);
            if (chunk.length < 32)
                continue;

            // gmMapBuildDataLoop reads the tag off the archive entry, not off the model
            _isWater[i] = (Map.ModelArchive.user0[i] & AppMain.GMD_MAP_MODEL_USER_FLAG0_WATER) != 0;
            uint drawFlagEx = _isWater[i] ? AppMain.NND_DRAWOBJ_MATCTRL_USERSAMPLER2D1 : 0U;

            // 154 models times their meshsets buries everything else, so only the first two
            // report in full and the rest go to a list nobody reads
            var modelLog = built < 2 ? log : _discard;

            try
            {
                _models[i] = DXModelLoader.LoadObject(
                    device, chunk, _textures, modelLog, ref min, ref max, MapDrawFlag, drawFlagEx);
                if (_models[i] != null)
                {
                    built++;
                    if (_isWater[i])
                        water++;
                }
            }
            catch (Exception ex)
            {
                log.Add($"block model {i} failed: {ex.Message}");
            }

            _discard.Clear();
        }

        log.Add($"{built} of {Map.ModelCount} block model(s) built, {water} tagged WATER,"
            + $" {_textures.Count} texture(s) decoded of {Map.TextureArchive.file_num} member(s),"
            + $" model bounds {min} .. {max}");

        BuildDeco(device, log);
        Log = string.Join(Environment.NewLine, log);

        BuildMarkers(device);
        Rebuild();
    }

    // GmDecoBuildData's console arm: members 0 and 1 are the decorations, 4 and 5 the waterfalls,
    // and the falls carry the user sampler in drawFlagEx rather than in drawFlag
    private void BuildDeco(GraphicsDevice device, List<string> log)
    {
        var deco = Map.Deco;
        if (deco == null)
            return;

        if (deco.ModelFormat != StageModelFormat.NnObject)
        {
            log.Add($"decorations ship {deco.ModelFormat} models, not drawn");
            return;
        }

        _decoTextures = DXModelLoader.LoadTextures(device, deco.TextureArchive, _discard);
        _decoModels = BuildDecoSet(device, deco.ModelArchive, _decoTextures, 0, log, "decoration");

        _fallTextures = DXModelLoader.LoadTextures(device, deco.FallTextureArchive, _discard);
        _fallModels = BuildDecoSet(
            device, deco.FallModelArchive, _fallTextures,
            AppMain.NND_DRAWOBJ_MATCTRL_USERSAMPLER2D1, log, "fall");

        _discard.Clear();
    }

    private LoadedObject[] BuildDecoSet(
        GraphicsDevice device,
        AppMain.AMS_AMB_HEADER archive,
        Dictionary<string, Texture2D> textures,
        uint drawFlagEx,
        List<string> log,
        string label)
    {
        if (archive == null)
            return Array.Empty<LoadedObject>();

        var models = new LoadedObject[archive.file_num];
        var min = new Vector3(float.MaxValue);
        var max = new Vector3(float.MinValue);
        int built = 0;

        for (int i = 0; i < archive.file_num; i++)
        {
            var chunk = AppMain.amBindGet(archive, i, out _);
            if (chunk.length < 32)
                continue;

            try
            {
                models[i] = DXModelLoader.LoadObject(
                    device, chunk, textures, _discard, ref min, ref max, DecoDrawFlag, drawFlagEx);
                if (models[i] != null)
                    built++;
            }
            catch (Exception ex)
            {
                log.Add($"{label} model {i} failed: {ex.Message}");
            }

            _discard.Clear();
        }

        log.Add($"{built} of {archive.file_num} {label} model(s) built,"
            + $" {textures.Count} texture(s) decoded");

        return models;
    }

    private void Rebuild()
    {
        Blocks.Clear();
        foreach (var list in _layerBlocks)
            list.Clear();

        var min = new Vector3(float.MaxValue);
        var max = new Vector3(float.MinValue);

        for (int i = 0; i < Map.Layers.Count; i++)
        {
            var layer = Map.Layers[i];
            if (!layer.Present || !_layerVisible[i])
                continue;

            foreach (var block in StagePlacement.Place(layer, i))
            {
                // gmMapDrawMapRange: a .mp referencing a model the archive does not have is a skip
                if (block.Mp.id >= _models.Length || _models[block.Mp.id] == null)
                    continue;

                block.World = StagePlacement.BlockMatrix(
                    block.Mp,
                    _models[block.Mp.id].Object.pNodeList[0].Translation,
                    block.CellX,
                    block.CellY,
                    layer.PosZ);

                Blocks.Add(block);
                _layerBlocks[i].Add(block);
                HasWater |= IsWater(block);

                min = Vector3.Min(min, new Vector3(block.Centre - new Vector2(BlockSize * 0.5f), layer.PosZ));
                max = Vector3.Max(max, new Vector3(block.Centre + new Vector2(BlockSize * 0.5f), layer.PosZ));
            }
        }

        Bounds = Blocks.Count == 0 ? new BoundingBox() : new BoundingBox(min, max);

        RebuildDecorations();
    }

    // one entry per .dc record that resolves to a model. gmEveMgrCreateEventBlkDecorate skips an id
    // with no dispatch entry, and the glare and effect ids have no model of their own.
    private void RebuildDecorations()
    {
        Decorations.Clear();

        foreach (var marker in Markers)
        {
            if (marker.Kind != StageMarkerKind.Decoration)
                continue;

            var kind = StageDeco.KindOf(marker.Id);
            var set = kind == StageDecoKind.Model ? _decoModels
                : kind == StageDecoKind.Fall ? _fallModels
                : null;

            if (set == null)
                continue;

            int index = AppMain.g_gm_deco_model_index[marker.Id][0];
            if (index < 0 || index >= set.Length || set[index] == null)
                continue;

            var position = StageDeco.WorldPosition(marker.Position, marker.Id);

            Decorations.Add(new StageDecoration
            {
                Id = marker.Id,
                ModelIndex = index,
                Kind = kind,
                Position = position,
                World = Matrix.CreateScale(StageDeco.Scale(marker.Id))
                    * Matrix.CreateTranslation(position),
                Centre = new Vector2(position.X, position.Y),
            });
        }
    }

    public LoadedObject ModelFor(StageBlock block)
    {
        return _models[block.Mp.id];
    }

    // gmMapBuildDataLoop sets NND_DRAWOBJ_DOUBLESIDE on every block model, and a lit permutation
    // needs a fragment light count the map's own draw flag never carries
    private const uint MapDrawFlag = AppMain.NND_DRAWOBJ_DOUBLESIDE | AppMain.NND_DRAWOBJ_FRAGPARALIGHT1;

    // GmDecoBuildData passes 0, and unlike the blocks nothing adds DOUBLESIDE afterwards. The
    // fragment light is what nnBindStdShader forces on at draw time.
    private const uint DecoDrawFlag = AppMain.NND_DRAWOBJ_FRAGPARALIGHT1;

    // nnPutMaterialCoreStdShader wants one of these; iPrevMaterial -1 makes it re-put every time,
    // since nothing here tracks what the last mesh set
    private static readonly AppMain.NNS_DRAWCALLBACK_VAL DrawCallback =
        new AppMain.NNS_DRAWCALLBACK_VAL { DrawFlag = MapDrawFlag, iPrevMaterial = -1 };

    private static readonly RasterizerState WireframeState =
        new RasterizerState { FillMode = FillMode.WireFrame, CullMode = CullMode.None };

    public bool Wireframe { get; set; }

    public bool NoAlphaTest { get; set; }

    public bool NoBlend { get; set; }

    public bool ShowMarkers { get; set; } = true;

    // set for one frame to have the draw explain itself; see StageGame's F1
    public System.Text.StringBuilder Report;

    public List<StageMarker> Markers { get; private set; } = new List<StageMarker>();

    private BasicEffect _markerEffect;
    private VertexPositionColor[] _markerLines = Array.Empty<VertexPositionColor>();

    private static readonly Color EventColour = new Color(1f, 0.25f, 0.85f);
    private static readonly Color EventRectColour = new Color(0.5f, 0.12f, 0.42f);
    private static readonly Color DecorationColour = new Color(0.3f, 0.85f, 1f);
    private static readonly Color RingColour = new Color(1f, 0.85f, 0.15f);

    private void BuildMarkers(GraphicsDevice device)
    {
        Markers = StageMarkers.Build(Map);

        _markerEffect = new BasicEffect(device) { VertexColorEnabled = true, LightingEnabled = false };

        // markers sit just in front of layer A so they read over the map rather than through it
        float z = AppMain.GMD_MAP_A_POS_Z + 8f;
        var lines = new List<VertexPositionColor>();

        foreach (var marker in Markers)
        {
            switch (marker.Kind)
            {
                case StageMarkerKind.Ring:
                    Diamond(lines, marker.Position, 6f, z, RingColour);
                    break;
                case StageMarkerKind.Decoration:
                    Box(lines, marker.Position - new Vector2(5f), marker.Position + new Vector2(5f), z, DecorationColour);
                    break;
                default:
                    Cross(lines, marker.Position, 8f, z, EventColour);
                    if (marker.HasRect)
                        Box(lines, marker.RectMin, marker.RectMax, z, EventRectColour);
                    break;
            }
        }

        _markerLines = lines.ToArray();
    }

    private static void Line(List<VertexPositionColor> lines, Vector2 a, Vector2 b, float z, Color colour)
    {
        lines.Add(new VertexPositionColor(new Vector3(a, z), colour));
        lines.Add(new VertexPositionColor(new Vector3(b, z), colour));
    }

    private static void Box(List<VertexPositionColor> lines, Vector2 min, Vector2 max, float z, Color colour)
    {
        Line(lines, new Vector2(min.X, min.Y), new Vector2(max.X, min.Y), z, colour);
        Line(lines, new Vector2(max.X, min.Y), new Vector2(max.X, max.Y), z, colour);
        Line(lines, new Vector2(max.X, max.Y), new Vector2(min.X, max.Y), z, colour);
        Line(lines, new Vector2(min.X, max.Y), new Vector2(min.X, min.Y), z, colour);
    }

    private static void Cross(List<VertexPositionColor> lines, Vector2 at, float r, float z, Color colour)
    {
        Line(lines, at - new Vector2(r, 0f), at + new Vector2(r, 0f), z, colour);
        Line(lines, at - new Vector2(0f, r), at + new Vector2(0f, r), z, colour);
    }

    private static void Diamond(List<VertexPositionColor> lines, Vector2 at, float r, float z, Color colour)
    {
        Line(lines, at + new Vector2(-r, 0f), at + new Vector2(0f, r), z, colour);
        Line(lines, at + new Vector2(0f, r), at + new Vector2(r, 0f), z, colour);
        Line(lines, at + new Vector2(r, 0f), at + new Vector2(0f, -r), z, colour);
        Line(lines, at + new Vector2(0f, -r), at + new Vector2(-r, 0f), z, colour);
    }

    // the main camera, since .ev, .dc and .rg all sit on the main map rather than on a layer
    private void DrawMarkers(GraphicsDevice device, Matrix view, Matrix projection)
    {
        if (!ShowMarkers || _markerLines.Length == 0)
            return;

        device.BlendState = BlendState.Opaque;
        device.DepthStencilState = DepthStencilState.None;
        device.RasterizerState = RasterizerState.CullNone;

        _markerEffect.View = view;
        _markerEffect.Projection = projection;
        _markerEffect.World = Matrix.Identity;

        foreach (var pass in _markerEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            device.DrawUserPrimitives(PrimitiveType.LineList, _markerLines, 0, _markerLines.Length / 2);
        }
    }

    // the map plane sits at z 0 and the layers spread either side of it, so the eye clears the lot
    public const float CameraHeight = 4000f;

    // A block model is not confined to its own cell: the .md overhang table carries offsets up to
    // seven cells, and GMD_MAP_DRAW_DF_SIZE is 8 because the game's own draw window allows for it.
    // Culling on the cell centre with a one block margin drops anything that leans further than
    // that, which shows up as scenery popping off at the screen edge.
    public const float CullMargin = AppMain.GMD_MAP_DRAW_DF_SIZE * BlockSize;

    // gmMapMain draws each parallax layer through its own camera, so the eye moves per layer and
    // nothing else does
    public Vector2 EyeFor(int layerIndex, Vector2 mainEye)
    {
        var layer = Map.Layers[layerIndex];
        return layer.Parallax
            ? StageParallax.CameraFor(mainEye, layer.MapSize, Map.MainMapSize, layer.ScrollScale)
            : mainEye;
    }

    // one draw per block, which is what the console build does; there is no batch to open or flush
    public int Draw(
        GraphicsDevice device,
        NnShaderCache shaders,
        Vector2 mainEye,
        Vector2 halfExtent,
        Matrix projection,
        Vector3 lightDirection,
        Texture2D behind = null)
    {
        int drawn = 0;
        _meshesDrawn = 0;
        _skippedMaterial = 0;
        _skippedShader = 0;
        _missingPermutation = null;

        // Furthest first. Layer order is not index order: A is at +128 and B at -128, but N is the
        // near layer at +256, so walking the indices backwards puts N down before the two it sits
        // in front of and it punches depth holes through them.
        foreach (int i in _byDepth)
        {
            if (_layerBlocks[i].Count == 0)
                continue;

            var eye = EyeFor(i, mainEye);
            var view = Matrix.CreateLookAt(new Vector3(eye, CameraHeight), new Vector3(eye, 0f), Vector3.Up);
            var lightView = Vector3.TransformNormal(lightDirection, view);
            var visible = new RectangleF(
                eye.X - halfExtent.X, eye.X + halfExtent.X,
                eye.Y - halfExtent.Y, eye.Y + halfExtent.Y);

            int before = drawn;
            _clipMin = new Vector2(float.MaxValue);
            _clipMax = new Vector2(float.MinValue);

            drawn += DrawLayer(
                device, shaders, _layerBlocks[i], view, projection, lightView, visible, behind,
                Map.Layers[i].PosZ);

            if (Report != null)
            {
                Report.AppendLine($"  layer {i} {Map.Layers[i].Name,-8}"
                    + $" eye {eye.X,10:0.0},{eye.Y,9:0.0}"
                    + $"  {drawn - before} of {_layerBlocks[i].Count} drawn"
                    + (drawn > before
                        ? $"  clip x {_clipMin.X,7:0.00}..{_clipMax.X,-7:0.00} y {_clipMin.Y,7:0.00}..{_clipMax.Y,-7:0.00}"
                        : ""));
            }
        }

        var mainView = Matrix.CreateLookAt(
            new Vector3(mainEye, CameraHeight), new Vector3(mainEye, 0f), Vector3.Up);

        int decorations = DrawDecorations(
            device, shaders, mainView, halfExtent, mainEye, projection, lightDirection, behind);

        DrawMarkers(device, mainView, projection);

        if (Report != null)
            Report.AppendLine($"  {decorations} of {Decorations.Count} decoration(s) drawn");

        if (Report != null)
        {
            Report.AppendLine($"  {drawn} block(s), {_meshesDrawn} mesh(es) issued;"
                + $" skipped {_skippedMaterial} on material, {_skippedShader} on missing shader"
                + (_missingPermutation != null ? $" (first: {_missingPermutation})" : ""));
            Report.AppendLine($"  scissor {device.ScissorRectangle}, viewport {device.Viewport.Bounds},"
                + $" blend {device.BlendState?.Name}, depth {device.DepthStencilState?.Name}");
        }

        return drawn;
    }

    private readonly List<string> _discard = new List<string>();

    private Vector2 _clipMin;
    private Vector2 _clipMax;

    private int _meshesDrawn;
    private int _skippedMaterial;
    private int _skippedShader;
    private string _missingPermutation;

    private int DrawLayer(
        GraphicsDevice device,
        NnShaderCache shaders,
        List<StageBlock> blocks,
        Matrix view,
        Matrix projection,
        Vector3 lightView,
        RectangleF visible,
        Texture2D behind,
        float layerZ)
    {
        int drawn = 0;

        foreach (var block in blocks)
        {
            if (!visible.Contains(block.Centre, CullMargin))
                continue;

            // a waterfall samples what is behind it, so it can only draw once that exists
            bool water = IsWater(block);
            if (water && behind == null)
                continue;

            var model = _models[block.Mp.id];
            drawn++;

            DrawModel(
                device, shaders, model, block.World * view, projection, lightView, behind, water,
                new Vector3(block.Centre, layerZ), view);
        }

        return drawn;
    }

    // Decorations sit on the main map, not on a layer, so they draw through the main camera the
    // way .ev, .dc and .rg placement already assumes.
    private int DrawDecorations(
        GraphicsDevice device,
        NnShaderCache shaders,
        Matrix view,
        Vector2 halfExtent,
        Vector2 mainEye,
        Matrix projection,
        Vector3 lightDirection,
        Texture2D behind)
    {
        if (!ShowDecorations || Decorations.Count == 0)
            return 0;

        var lightView = Vector3.TransformNormal(lightDirection, view);
        var visible = new RectangleF(
            mainEye.X - halfExtent.X, mainEye.X + halfExtent.X,
            mainEye.Y - halfExtent.Y, mainEye.Y + halfExtent.Y);

        int drawn = 0;

        foreach (var deco in Decorations)
        {
            if (!visible.Contains(deco.Centre, CullMarginFor(deco.Id)))
                continue;

            // the falls refract, so like a water block they can only draw once there is something
            // behind them to sample. Zone 1 has no water blocks, so nothing produces that yet.
            bool water = deco.Kind == StageDecoKind.Fall;
            if (water && behind == null)
                continue;

            var model = (water ? _fallModels : _decoModels)[deco.ModelIndex];
            drawn++;

            DrawModel(
                device, shaders, model, deco.World * view, projection, lightView, behind, water,
                deco.Position, view);
        }

        return drawn;
    }

    // gmDecoLoadObj's own view_out_ofst, so a decoration leaves the screen when the game says it does
    private static float CullMarginFor(int id)
    {
        return AppMain.g_gm_decorate_size_tbl[id] + 16 + 32 + 16;
    }

    // One model at one place, which is all the console arm ever issues. Blocks and decorations
    // differ only in how their world matrix is arrived at, so they share this.
    private void DrawModel(
        GraphicsDevice device,
        NnShaderCache shaders,
        LoadedObject model,
        Matrix modelView,
        Matrix projection,
        Vector3 lightView,
        Texture2D behind,
        bool water,
        Vector3 reportPoint,
        Matrix view)
    {
        foreach (var mesh in model.Meshes)
        {
            if (mesh.Material == null || model.IsHidden(mesh.NodeIndex))
            {
                _skippedMaterial++;
                continue;
            }

            var uber = shaders.Get(mesh.Permutation);
            if (uber == null)
            {
                _skippedShader++;
                _missingPermutation ??= mesh.Permutation;
                continue;
            }

            _meshesDrawn++;

            if (Report != null)
            {
                // the block's own centre, not the mesh's local origin: with node 0 far from the
                // model origin those are nowhere near each other
                var clip = Vector4.Transform(new Vector4(reportPoint, 1f), view * projection);

                _clipMin = Vector2.Min(_clipMin, new Vector2(clip.X / clip.W, clip.Y / clip.W));
                _clipMax = Vector2.Max(_clipMax, new Vector2(clip.X / clip.W, clip.Y / clip.W));

                if (_meshesDrawn == 1)
                {
                    Report.AppendLine($"  first mesh: {mesh.Permutation}");
                    Report.AppendLine($"    {mesh.VertexCount} vtx, {mesh.TriangleCount} tri,"
                        + $" tfm {mesh.VtxTfmMode}, texture {(mesh.Texture == null ? "NULL" : "ok")},"
                        + $" {mesh.SamplerTextures.Count(t => t == null)} null sampler(s)");
                }
            }

            if (mesh.VtxTfmMode == 0)
            {
                uber.SetTransforms(model.ModelView(mesh.MatrixIndex, modelView), projection);
            }
            else
            {
                uber.SetTransforms(modelView, projection);
                uber.SetBoneMatrices(model.GatherBones(mesh, modelView));
            }

            uber.SetParallelLight(0, lightView, Vector3.One, Vector3.One);

            // pushed past the map so nothing fogs, but the uniforms still have to be real
            uber.SetFog(Vector3.Zero, 100000f, 200000f);

            uber.SetMaterial(mesh.Material.pColor, new Vector3(0.35f, 0.35f, 0.4f));
            uber.SetTextureMatrices(mesh.Material);
            uber.SetMapBlend(mesh.Material);

            // the material's own blend, depth, cull and alpha test, through the same code the
            // game uses. Without this every block draws opaque with the clip switched off.
            DrawCallback.pMaterial = mesh.MaterialPtr;
            AppMain.nnPutMaterialCoreStdShader(DrawCallback);
            AppMain.mppApplyStdShaderRenderState(device);

            // two switches that split "the clip discarded it" from "it blended to nothing",
            // which is all that is left once the pipeline state is known to be identical
            if (NoAlphaTest)
                uber.SetAlphaTestAlways();
            else
                AppMain.mppSetStdShaderAlphaTest(uber);

            if (NoBlend)
                device.BlendState = BlendState.Opaque;

            if (Wireframe)
                device.RasterizerState = WireframeState;

            // read after the state is applied, not before: this is what the draw actually uses
            if (Report != null && _meshesDrawn == 1)
            {
                Report.AppendLine($"    blend {device.BlendState?.ColorSourceBlend}"
                    + $"/{device.BlendState?.ColorDestinationBlend}"
                    + $" write {device.BlendState?.ColorWriteChannels}");
                Report.AppendLine($"    depth {device.DepthStencilState?.DepthBufferEnable}"
                    + $"/{device.DepthStencilState?.DepthBufferFunction}"
                    + $" write {device.DepthStencilState?.DepthBufferWriteEnable}"
                    + $"  cull {device.RasterizerState?.CullMode}"
                    + $"  scissorTest {device.RasterizerState?.ScissorTestEnable}"
                    + $"  scissor {device.ScissorRectangle}");
                Report.AppendLine($"    alphaTest {AppMain.mppStdShaderAlphaTestState()}"
                    + $"  u_AlphaTest {uber.AlphaTestValue()}"
                    + $"  diffuse alpha {mesh.Material.pColor?.Diffuse.a}");
            }

            for (int slot = 0; slot < mesh.SamplerTextures.Length; ++slot)
            {
                device.Textures[slot] = mesh.SamplerTextures[slot];
                if (mesh.SamplerStates[slot] != null)
                    device.SamplerStates[slot] = mesh.SamplerStates[slot];
            }

            // sampler_def.h puts the user sampler at SAMPLER_TAIL, straight after the
            // material's own maps, and the fall shader wants the projection to find itself
            // on screen with. gmMapDrawFallShaderPrioPreMidMapUserFunc clamps and filters it.
            if (water)
            {
                int slot = AppMain.mppStdShaderSamplerCount(mesh.Material.fTexType);
                device.Textures[slot] = behind;
                device.SamplerStates[slot] = SamplerState.LinearClamp;
                uber.SetUserUniforms(NnStdShaderEffect.ProjectionUserUniforms(projection));
            }

            device.SetVertexBuffer(mesh.Vertices);
            device.Indices = mesh.Indices;

            foreach (var pass in uber.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList, 0, 0, mesh.VertexCount, 0, mesh.TriangleCount);
            }
        }
    }

    // topmost block under a point on screen. The screen point is given relative to the main camera,
    // so a parallax layer has to be asked in its own space rather than in the main one.
    public StageBlock PickAt(Vector2 mainEye, Vector2 offset)
    {
        StageBlock best = null;

        for (int i = 0; i < Map.Layers.Count; i++)
        {
            var world = EyeFor(i, mainEye) + offset;

            foreach (var block in _layerBlocks[i])
            {
                if (Math.Abs(block.Centre.X - world.X) > BlockSize * 0.5f)
                    continue;
                if (Math.Abs(block.Centre.Y - world.Y) > BlockSize * 0.5f)
                    continue;

                if (best == null || Map.Layers[i].PosZ > Map.Layers[best.LayerIndex].PosZ)
                    best = block;

                break;
            }
        }

        return best;
    }

    public void Dispose()
    {
        foreach (var texture in _textures.Values)
            texture?.Dispose();
        foreach (var texture in _decoTextures.Values)
            texture?.Dispose();
        foreach (var texture in _fallTextures.Values)
            texture?.Dispose();

        _textures.Clear();
        _decoTextures.Clear();
        _fallTextures.Clear();
        _models = Array.Empty<LoadedObject>();
        _decoModels = Array.Empty<LoadedObject>();
        _fallModels = Array.Empty<LoadedObject>();
        Blocks.Clear();
        Decorations.Clear();
    }
}
