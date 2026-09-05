using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sonic4Episode1.Viewer;

// The background, drawn through its own perspective camera before the block layers. StageScene is
// the equivalent for the map itself; the two share nothing but the shader cache, because map far is
// a separate subsystem in the game too.
internal sealed class StageMapFarScene : IDisposable
{
    private readonly StageMap _map;
    private readonly StageMapFarObject[] _objects;
    private readonly List<Matrix>[] _instances;
    private LoadedObject[] _models;

    // nnInitMaterialMotionObject clones the animated materials into a shadow object; the draw then
    // reads pMatPtrList off the clone rather than the model. Null where an object has no mat motion.
    private AppMain.NNS_OBJECT[] _materialObjects;
    private AppMain.NNS_MOTION[] _materialMotions;
    private AppMain.NNS_MOTION[] _nodeMotions;
    private float[] _frames;
    private Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();

    private readonly StageMapFarScroll _scrollX;
    private readonly StageMapFarScroll _scrollY;
    private readonly Vector2 _mapSize;

    private float _degSky;
    private float _degSky2;

    // the swizzled camera position, which is what g_map_far_data.pos holds and what every placement
    // function gates on
    private Vector3 _cameraPos;

    public Color ClearColour { get; }
    public string Log { get; private set; } = "";
    public int Drawn { get; private set; }

    public StageMapFarScene(StageMap map)
    {
        _map = map;

        var far = map.MapFar;
        _objects = StageMapFarTable.Objects(map.ZoneType, far.Arm);
        _instances = new List<Matrix>[_objects.Length];
        for (int i = 0; i < _objects.Length; i++)
            _instances[i] = new List<Matrix>();

        bool boss = StageMapFarTable.IsBossStage(map.StageId);
        _scrollX = StageMapFarTable.ScrollX(map.ZoneType, far.Arm, boss);
        _scrollY = StageMapFarTable.ScrollY(map.ZoneType, far.Arm, boss);

        // gmMapFarGetMapsetMpA is layer A's MP, not the ATTR_A grid StageMap measures parallax with
        var layerA = map.Layers[0];
        _mapSize = layerA.Present
            ? new Vector2(layerA.Width * StagePlacement.BlockSize, layerA.Height * StagePlacement.BlockSize)
            : map.MainMapSize;

        ClearColour = StageMapFarTable.ClearColour(map.ZoneType, far.Arm);
    }

    public bool Any => _objects.Length > 0;

    public void Build(GraphicsDevice device)
    {
        var far = _map.MapFar;
        var log = new List<string>();
        var min = new Vector3(float.MaxValue);
        var max = new Vector3(float.MinValue);

        _textures = DXModelLoader.LoadTextures(device, far.Textures, log);
        _models = new LoadedObject[_objects.Length];

        int built = 0;
        for (int i = 0; i < _objects.Length; i++)
        {
            var entry = _objects[i];
            var archive = entry.Source == StageMapFarSource.RenderModels ? far.RenderModels : far.Models;

            if (archive == null || entry.Model >= archive.file_num)
            {
                log.Add($"object {i} wants {entry.Source} {entry.Model}, which is not there");
                continue;
            }

            // the console sea is the one map far model built with the user sampler set
            uint drawFlagEx = entry.Draw == StageMapFarDraw.Sea && far.Arm == StageMapFarArm.Console
                ? AppMain.NND_DRAWOBJ_MATCTRL_USERSAMPLER2D1
                : 0U;

            try
            {
                _models[i] = DXModelLoader.LoadObject(
                    device, AppMain.amBindGet(archive, entry.Model, out _), _textures,
                    built < 1 ? log : _discard, ref min, ref max, DrawFlag, drawFlagEx);

                if (_models[i] != null)
                    built++;
            }
            catch (Exception ex)
            {
                log.Add($"object {i} failed: {ex.Message}");
            }

            _discard.Clear();
        }

        BuildMotions(log);

        log.Add($"{built} of {_objects.Length} background object(s) built,"
            + $" {_textures.Count} texture(s) decoded of {far.Textures?.file_num ?? 0} member(s),"
            + $" bounds {min} .. {max}");
        Log = string.Join(Environment.NewLine, log);
    }

    private void BuildMotions(List<string> log)
    {
        var far = _map.MapFar;

        _materialObjects = new AppMain.NNS_OBJECT[_objects.Length];
        _materialMotions = new AppMain.NNS_MOTION[_objects.Length];
        _nodeMotions = new AppMain.NNS_MOTION[_objects.Length];
        _frames = new float[_objects.Length];

        for (int i = 0; i < _objects.Length; i++)
        {
            if (_models[i] == null)
                continue;

            _nodeMotions[i] = ReadMotion(far.Motions, _objects[i].Motion, i, "node", log);
            _materialMotions[i] = ReadMotion(far.MaterialMotions, _objects[i].MaterialMotion, i, "material", log);

            if (_materialMotions[i] == null)
                continue;

            // only the materials with submotions get cloned, and only the parts that animate
            var clone = new AppMain.NNS_OBJECT();
            AppMain.nnInitMaterialMotionObject(clone, _models[i].Object, _materialMotions[i]);
            _materialObjects[i] = clone;
        }
    }

    private static AppMain.NNS_MOTION ReadMotion(
        AppMain.AMS_AMB_HEADER archive, int member, int objectIndex, string kind, List<string> log)
    {
        if (archive == null || member < 0 || member >= archive.file_num)
            return null;

        var chunk = AppMain.amBindGet(archive, member, out _);
        if (chunk?.array == null)
            return null;

        foreach (var (motion, error) in NnArchive.ReadMotions(chunk.array, chunk.offset, chunk.length))
        {
            if (error != null)
            {
                log.Add($"object {objectIndex} {kind} motion {member}: {error}");
                continue;
            }

            if (motion != null)
                return motion;
        }

        log.Add($"object {objectIndex} {kind} motion {member}: no motion chunk in the member");
        return null;
    }

    private readonly List<string> _discard = new List<string>();

    // map far models are lit, and a lit permutation needs a fragment light count the game's own
    // draw flag never carries
    private const uint DrawFlag = AppMain.NND_DRAWOBJ_FRAGPARALIGHT1;

    private static readonly AppMain.NNS_DRAWCALLBACK_VAL DrawCallback =
        new AppMain.NNS_DRAWCALLBACK_VAL { DrawFlag = DrawFlag, iPrevMaterial = -1 };

    public void Update(float frames, Vector2 mainEye)
    {
        _cameraPos = StageMapFarCamera.Position(mainEye, _mapSize, _scrollX, _scrollY);

        Spin(frames);
        Animate(frames);

        for (int i = 0; i < _objects.Length; i++)
        {
            _instances[i].Clear();
            Place(_objects[i], _instances[i], i);
        }
    }

    private void Animate(float frames)
    {
        if (_materialMotions == null)
            return;

        for (int i = 0; i < _objects.Length; i++)
        {
            var material = _materialMotions[i];
            var node = _nodeMotions[i];
            if (material == null && node == null)
                continue;

            // gmMapFarZone1Init slows the sea's material to a fifth; everything else runs at one
            // frame per frame
            float speed = _objects[i].Draw == StageMapFarDraw.Sea ? 0.2f : 1f;
            _frames[i] = Wrap(_frames[i] + frames * speed, material ?? node);

            if (material != null)
                AppMain.nnCalcMaterialMotion(_materialObjects[i], _models[i].Object, material, _frames[i]);

            if (node != null)
                _models[i].SetPose(node, _frames[i]);
        }
    }

    // every map far motion loops, so the accumulator wraps rather than running away and losing
    // precision; nnCalcMotionFrame would otherwise clamp it flat at the end
    private static float Wrap(float frame, AppMain.NNS_MOTION motion)
    {
        float span = motion.EndFrame - motion.StartFrame;
        if (span <= 0f)
            return motion.StartFrame;

        while (frame >= motion.EndFrame)
            frame -= span;

        return frame;
    }

    private void Spin(float frames)
    {
        if (_map.ZoneType == 0)
        {
            _degSky += frames * 0.005f;
            _degSky2 += frames * 0.01f;
        }
        else if (_map.ZoneType == 4)
        {
            _degSky += frames * 0.01f;
            _degSky2 += frames * 0.02f;
        }

        if (_degSky > 360f)
            _degSky = 0f;
        if (_degSky2 > 360f)
            _degSky2 = 0f;
    }

    public void Draw(
        GraphicsDevice device, NnShaderCache shaders, Vector3 lightDirection, float aspect)
    {
        Drawn = 0;

        if (_models == null)
            return;

        var view = Matrix.CreateLookAt(
            _cameraPos, StageMapFarCamera.Target(_cameraPos, _map.ZoneType), Vector3.Up);

        var projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(StageMapFarCamera.FovDegrees), aspect,
            StageMapFarCamera.NearPlane, StageMapFarCamera.FarPlane);

        var lightView = Vector3.TransformNormal(lightDirection, view);
        var fog = StageMapFarFog.For(_map.ZoneType, _map.StageId, _map.MapFar.Arm);

        // PRE_MAPFAR then MAPFAR: on console zone 1 that is the mirrored reflection, then the sea
        // over it, then the real sky and rocks
        foreach (uint bucket in Buckets)
        {
            for (int i = 0; i < _objects.Length; i++)
            {
                if (_objects[i].Bucket != bucket || _models[i] == null)
                    continue;

                foreach (var world in _instances[i])
                    DrawModel(device, shaders, _models[i], _materialObjects?[i],
                        world * view, projection, lightView, fog);
            }
        }
    }

    private static readonly uint[] Buckets =
    {
        AppMain.OBD_DRAW_CMD_STATE_PRE_MAPFAR,
        AppMain.OBD_DRAW_CMD_STATE_MAPFAR,
    };

    private void DrawModel(
        GraphicsDevice device, NnShaderCache shaders, LoadedObject model,
        AppMain.NNS_OBJECT materialObject,
        Matrix modelView, Matrix projection, Vector3 lightView, StageMapFarFog fog)
    {
        foreach (var mesh in model.Meshes)
        {
            if (mesh.Material == null || model.IsHidden(mesh.NodeIndex))
                continue;

            var uber = shaders.Get(mesh.Permutation);
            if (uber == null)
                continue;

            // a playing material motion writes into the clone, so read the material back out of it
            var matPtr = mesh.MaterialPtr;
            var material = mesh.Material;

            if (materialObject != null && mesh.MaterialIndex < materialObject.nMaterial)
            {
                matPtr = materialObject.pMatPtrList[mesh.MaterialIndex];
                if (matPtr.pMaterial is AppMain.NNS_MATERIAL_STDSHADER_DESC animated)
                    material = animated;
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
            uber.SetFog(fog.Colour, fog.Near, fog.Far);
            uber.SetMaterial(material.pColor, new Vector3(0.35f, 0.35f, 0.4f));
            uber.SetTextureMatrices(material);
            uber.SetMapBlend(material);

            DrawCallback.pMaterial = matPtr;
            AppMain.nnPutMaterialCoreStdShader(DrawCallback);
            AppMain.mppApplyStdShaderRenderState(device);
            AppMain.mppSetStdShaderAlphaTest(uber);

            for (int slot = 0; slot < mesh.SamplerTextures.Length; ++slot)
            {
                device.Textures[slot] = mesh.SamplerTextures[slot];
                if (mesh.SamplerStates[slot] != null)
                    device.SamplerStates[slot] = mesh.SamplerStates[slot];
            }

            // iTexIdx is an animated channel, so a playing material can flip which texture a slot
            // binds; the baked ones above are the base pose
            if (materialObject != null)
                RebindAnimatedTextures(device, model, mesh, material);

            device.SetVertexBuffer(mesh.Vertices);
            device.Indices = mesh.Indices;

            foreach (var pass in uber.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList, 0, 0, mesh.VertexCount, 0, mesh.TriangleCount);
            }

            Drawn++;
        }
    }

    private static void RebindAnimatedTextures(
        GraphicsDevice device, LoadedObject model, LoadedMesh mesh,
        AppMain.NNS_MATERIAL_STDSHADER_DESC material)
    {
        if (material.pTexDesc == null || model.TexturesByFileIndex.Length == 0)
            return;

        foreach (var desc in material.pTexDesc)
        {
            uint mapBit = desc.fType & AppMain.NND_TEXTURETYPE_STDSHADER_MAP_MASK;
            int slot = AppMain.mppStdShaderSamplerSlot(material.fTexType, mapBit);

            if (slot < 0 || slot >= mesh.SamplerTextures.Length)
                continue;

            if (desc.iTexIdx >= 0 && desc.iTexIdx < model.TexturesByFileIndex.Length)
                device.Textures[slot] = model.TexturesByFileIndex[desc.iTexIdx];
        }
    }

    public void Dispose()
    {
        foreach (var texture in _textures.Values)
            texture?.Dispose();

        _textures.Clear();
        _models = null;
    }

    // ---- placement, gmMapFarSceneObjFuncDraw* ----------------------------------------------

    private static readonly Matrix DirFlipMatrix = Matrix.CreateRotationY(MathHelper.PiOver2);

    private void Place(StageMapFarObject entry, List<Matrix> into, int index)
    {
        int first = into.Count;

        switch (entry.Draw)
        {
            case StageMapFarDraw.Rotate: PlaceRotate(into, index); break;
            case StageMapFarDraw.RockA: PlaceRockA(into); break;
            case StageMapFarDraw.RockB: PlaceRockB(into); break;
            case StageMapFarDraw.RockC: PlaceRockC(into); break;
            case StageMapFarDraw.Sea: PlaceSea(into); break;
            case StageMapFarDraw.Wheel: PlaceWheelOrLight(into, light: false); break;
            case StageMapFarDraw.SearchLight: PlaceWheelOrLight(into, light: true); break;
            case StageMapFarDraw.Bg: PlaceBg(into); break;
            case StageMapFarDraw.FlatBg: PlaceFlatBg(into); break;
        }

        if (!StageMapFarTable.DirFlip(entry.Draw, _map.ZoneType))
            return;

        for (int i = first; i < into.Count; i++)
            into[i] = DirFlipMatrix * into[i];
    }

    private static Matrix Place(Vector3 at, float scale = 1f) =>
        Matrix.CreateScale(scale) * Matrix.CreateTranslation(at);

    private void PlaceRotate(List<Matrix> into, int index)
    {
        int stage = _map.StageId;

        if (_map.ZoneType == 1)
        {
            if (!DrawCheckYakei())
                return;

            into.Add(Matrix.CreateRotationY(MathHelper.ToRadians(_degSky))
                * Matrix.CreateScale(1.4f)
                * Matrix.CreateTranslation(_cameraPos.X, -30f, _cameraPos.Z));
            return;
        }

        if (_map.ZoneType == 0)
        {
            if (stage == 3)
                return;

            if (stage == 0
                && ((_cameraPos.Y <= 10f && Between(_cameraPos.Z, -191.8f, -182f))
                    || (_cameraPos.Y <= 7.85f && Between(_cameraPos.Z, -304f, -269f))
                    || (_cameraPos.Y <= 9.97f && Between(_cameraPos.Z, -358f, -337f))
                    || (_cameraPos.Y >= 7f && Between(_cameraPos.Z, -359f, -347f))
                    || Between(_cameraPos.Z, -503f, -495f)))
                return;

            if (stage == 1
                && ((_cameraPos.Y <= 6.47f && Between(_cameraPos.Z, -156f, -135.4f))
                    || (_cameraPos.Y <= 3.05f && Between(_cameraPos.Z, -190f, -130.4f))
                    || _cameraPos.Z <= -419f))
                return;

            into.Add(Matrix.CreateRotationY(MathHelper.ToRadians(_degSky))
                * Matrix.CreateTranslation(_cameraPos.X, -10f, _cameraPos.Z));
            return;
        }

        // TODO(port): the final zone splits earth, cloud and film on user_work and gives each its
        // own rate plus a 23.4 degree tilt; this is the shared fallback until that is transcribed
        into.Add(Matrix.CreateRotationY(MathHelper.ToRadians(_degSky)));
    }

    private void PlaceRockA(List<Matrix> into)
    {
        if (!DrawCheckRock())
            return;

        const float y = -10f;
        const float x = -100f;

        if (_map.StageId == 2)
        {
            if (Between(_cameraPos.Z, -295f, -18f))
                into.Add(Place(new Vector3(x - 40f, y, -90f)));
            if (Between(_cameraPos.Z, -366f, -112f))
                into.Add(Place(new Vector3(x + 100f, y, -240f), 2f));
            if (Between(_cameraPos.Z, -559f, -238f))
                into.Add(Place(new Vector3(x + 30f, y, -400f)));
            return;
        }

        if (_map.StageId == 3)
        {
            into.Add(Place(new Vector3(x - 30f, y, -100f), 3f));
            return;
        }

        if (Between(_cameraPos.Z, -260f, -10f))
            into.Add(Place(new Vector3(x + 100f, y, -135f), 2f));
        if (Between(_cameraPos.Z, -500f, -84f))
            into.Add(Place(new Vector3(x - 40f, y, -290f)));
        if (Between(_cameraPos.Z, -475f, -182f))
            into.Add(Place(new Vector3(x + 80f, y, -330f), 2f));
    }

    private void PlaceRockB(List<Matrix> into)
    {
        if (!DrawCheckRock())
            return;

        const float y = -10f;
        const float x = -100f;

        if (_map.StageId == 2)
        {
            if (Between(_cameraPos.Z, -256f, -18f))
                into.Add(Place(new Vector3(x + 60f, y, -100f), 2.5f));
            if (Between(_cameraPos.Z, -406f, -112f))
                into.Add(Place(new Vector3(x + 20f, y, -280f)));
            if (Between(_cameraPos.Z, -526f, -275f))
                into.Add(Place(new Vector3(x + 100f, y, -400f), 2f));
            return;
        }

        if (_map.StageId == 3)
        {
            into.Add(Place(new Vector3(x, y, 75f), 5f));
            into.Add(Place(new Vector3(x - 130f, y, -180f), 3f));
            return;
        }

        if (Between(_cameraPos.Z, -317f, -6f))
            into.Add(Place(new Vector3(x + 40f, y, -160f)));
        if (Between(_cameraPos.Z, -397f, -100f))
            into.Add(Place(new Vector3(x + 70f, y, -250f), 2f));
        if (Between(_cameraPos.Z, -574f, -314f))
            into.Add(Place(new Vector3(x + 100f, y, -440f), 2.5f));
    }

    private void PlaceRockC(List<Matrix> into)
    {
        if (!DrawCheckRock())
            return;

        const float y = -10f;
        const float x = -100f;

        if (_map.StageId == 2)
        {
            if (Between(_cameraPos.Z, -387f, -18f))
                into.Add(Place(new Vector3(x + 20f, y, -180f), 2f));
            if (Between(_cameraPos.Z, -582f, -167f))
                into.Add(Place(new Vector3(x + 80f, y, -360f), 4f));
            return;
        }

        if (_map.StageId == 3)
        {
            into.Add(Place(new Vector3(x + 160f, y, 10f), 2f));
            return;
        }

        if (Between(_cameraPos.Z, -526f, -6f))
            into.Add(Place(new Vector3(x + 30f, y, -250f), 5f));
        if (Between(_cameraPos.Z, -577f, -202f))
            into.Add(Place(new Vector3(x + 40f, y, -380f), 2f));
    }

    private void PlaceSea(List<Matrix> into)
    {
        if (_map.StageId == 3)
            return;

        into.Add(Matrix.CreateTranslation(40f, -10f, _cameraPos.Z));
    }

    private void PlaceWheelOrLight(List<Matrix> into, bool light)
    {
        int stage = _map.StageId;
        if (stage == 7)
            return;

        if (!light)
        {
            if (stage == 5)
                return;
            if ((stage == 4 || stage == 6) && _cameraPos.Z >= -119.4f)
                return;
            if (stage == 4 && !DrawCheckYakei())
                return;
        }
        else if (!DrawCheckYakei())
        {
            return;
        }

        into.Add(Matrix.CreateTranslation(50f, -30f, -135f));
    }

    private void PlaceBg(List<Matrix> into)
    {
        int stage = _map.StageId;

        switch (_map.ZoneType)
        {
            case 1:
                if (!DrawCheckYakei())
                    return;
                into.Add(stage == 7
                    ? Matrix.CreateTranslation(50f, -20f, 0f)
                    : Matrix.CreateTranslation(50f, -30f, -135f));
                return;

            case 2:
                if (stage == 8 && Between(_cameraPos.Z, -333f, -312.5f))
                    return;
                into.Add(Matrix.CreateTranslation(50f, -30f, -160f));
                return;

            case 4:
                into.Add(Matrix.CreateRotationY(MathHelper.ToRadians(90f))
                    * Matrix.CreateTranslation(_cameraPos.X, -10f, _cameraPos.Z));
                return;

            // TODO(port): the special stage places its kaleidoscope at OBD_LCD_X/Y, which read
            // g_obj and so need AppInit. Out of scope for M1b.
            case 5:
                return;
        }
    }

    private void PlaceFlatBg(List<Matrix> into)
    {
        int stage = _map.StageId;

        if (stage <= 2)
            return;

        if (stage >= 4 && stage <= 7 && !DrawCheckYakei())
            return;

        if (stage == 3)
            into.Add(Matrix.CreateTranslation(250f, -97f, -20f));
        else if (stage >= 4 && stage <= 7)
            into.Add(Matrix.CreateTranslation(250f, -70f, -50f));
        else if (stage >= 8 && stage <= 11)
            into.Add(Matrix.CreateTranslation(10f, -30f, -50f));
        else if (stage >= 16 && stage <= 20)
            into.Add(Matrix.CreateTranslation(250f, -70f, -50f));
    }

    // gmMapFarDrawCheckRock and gmMapFarDrawCheckYakei, console arm. TODO(port): the console arm
    // also draws one extra rock per model at the zone 1 boss, which PlaceRock* does not.
    private bool DrawCheckRock()
    {
        switch (_map.StageId)
        {
            case 0:
                if (_cameraPos.Y <= 5.2f) return false;
                if (_cameraPos.Y <= 5.4f && Between(_cameraPos.Z, -110f, -62f)) return false;
                if (_cameraPos.Y <= 13f && Between(_cameraPos.Z, -182f, -179f)) return false;
                if (_cameraPos.Y <= 8.85f && Between(_cameraPos.Z, -308f, -267f)) return false;
                if (_cameraPos.Y <= 9.97f && Between(_cameraPos.Z, -358f, -337f)) return false;
                if (_cameraPos.Y >= 7f && Between(_cameraPos.Z, -359f, -347f)) return false;
                if (Between(_cameraPos.Z, -503f, -495f)) return false;
                return true;

            case 1:
                if (_cameraPos.Y <= 4.62f) return false;
                if (_cameraPos.Y <= 5.85f && _cameraPos.Z >= -50f) return false;
                if (_cameraPos.Y <= 6.47f && Between(_cameraPos.Z, -156f, -135.4f)) return false;
                if (_cameraPos.Y <= 4.5f && Between(_cameraPos.Z, -170f, -135.4f)) return false;
                if (_cameraPos.Z <= -419f) return false;
                return true;

            default:
                return true;
        }
    }

    private bool DrawCheckYakei()
    {
        switch (_map.StageId)
        {
            case 4:
                if (_cameraPos.Y <= 10f && Between(_cameraPos.Z, -50.8f, -48f)) return false;
                if (_cameraPos.Y <= 33f && Between(_cameraPos.Z, -186f, -170f)) return false;
                if (_cameraPos.Y >= 32f && Between(_cameraPos.Z, -138f, -123f)) return false;
                if (_cameraPos.Y >= 19f && _cameraPos.Y <= 27f
                    && Between(_cameraPos.Z, -224f, -208f)) return false;
                return true;

            case 6:
                if (_cameraPos.Y <= 11.95f) return false;
                if (_cameraPos.Y <= 24f && Between(_cameraPos.Z, -229f, -197f)) return false;
                if (_cameraPos.Y <= 24f && _cameraPos.Y >= 20.38f && _cameraPos.Z >= -116.5f) return false;
                if (_cameraPos.Y >= 35.55f && _cameraPos.Y <= 40.76f && _cameraPos.Z >= -95f) return false;
                if (_cameraPos.Y >= 35.55f && _cameraPos.Z >= -81f) return false;
                return true;

            default:
                return true;
        }
    }

    private static bool Between(float value, float low, float high) => value >= low && value <= high;
}
