using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace Sonic4Episode1.Viewer;

internal sealed class LoadedMesh
{
    public VertexBuffer Vertices;
    public IndexBuffer Indices;
    public int VertexCount;
    public int TriangleCount;
    public Texture2D Texture;
    public Vector3 DiffuseColor = Vector3.One;

    // material as read, plus its maps resolved into sampler_def.h's sampler slots for this permutation
    public AppMain.NNS_MATERIAL_STDSHADER_DESC Material;

    // the pointer it hangs off, which is what nnPutMaterialCoreStdShader wants
    public AppMain.NNS_MATERIALPTR MaterialPtr;

    // meshset.iMaterial. Material motion animates a clone of pMatPtrList, so anything that plays one
    // has to look the material up by index each frame rather than use the two fields above.
    public int MaterialIndex;
    public Texture2D[] SamplerTextures = Array.Empty<Texture2D>();
    public SamplerState[] SamplerStates = Array.Empty<SamplerState>();
    public string Permutation;

    // NNS_STDSHADER_PROFILE.VtxTfmMode: 0 rigid, 1 four bone matrices, 2 four indexed into sixteen
    public int VtxTfmMode;

    // rigid meshsets take their model view straight from this palette entry
    public int MatrixIndex;

    // maps this mesh's BLENDINDICES onto palette entries; null for a rigid mesh
    public int[] BoneMatrixIndices;

    // this mesh's slice of the palette, gathered through BoneMatrixIndices each frame
    public Matrix[] BoneMatrices = Array.Empty<Matrix>();

    // the node this meshset hangs off, which node hide motion can switch off
    public int NodeIndex;
}

// one NNS_OBJECT and everything drawn from it; an archive can hold several
internal sealed class LoadedObject
{
    public AppMain.NNS_OBJECT Object;
    public readonly List<LoadedMesh> Meshes = new List<LoadedMesh>();

    // every texture the object's NNS_TEXFILELIST names, resolved once and indexed the way
    // NNS_MATERIAL_STDSHADER_TEXMAP_DESC.iTexIdx does. Material motion animates that index, so a
    // playing material has to be able to swap textures without re-resolving by name each frame.
    public Texture2D[] TexturesByFileIndex = Array.Empty<Texture2D>();

    // pose in model space, base matrix left out so the camera can move without rebuilding it
    public Matrix[] Palette = Array.Empty<Matrix>();

    private NNS_MATRIX[] _pose;
    private uint[] _nodeStatus;
    private AppMain.NNS_MATRIXSTACK _stack;

    public LoadedObject(AppMain.NNS_OBJECT obj)
    {
        Object = obj;
        _pose = new NNS_MATRIX[obj.nNode];
        for (int i = 0; i < _pose.Length; ++i)
            _pose[i] = NNS_MATRIX.CreateIdentity();

        _nodeStatus = new uint[obj.nNode];
        _stack = new AppMain.NNS_MATRIXSTACK();
        Palette = new Matrix[obj.nNode];
    }

    public bool IsHidden(int nodeIndex)
    {
        return nodeIndex >= 0 && nodeIndex < _nodeStatus.Length
            && (_nodeStatus[nodeIndex] & AppMain.NND_NODESTATUS_HIDE) != 0;
    }

    public Matrix ModelView(int paletteIndex, Matrix modelView)
    {
        return paletteIndex < 0 || paletteIndex >= Palette.Length
            ? modelView
            : Palette[paletteIndex] * modelView;
    }

    // palette holds the pose in model space, camera folds in here rather than in the node walk
    public Matrix[] GatherBones(LoadedMesh mesh, Matrix modelView)
    {
        int[] indices = mesh.BoneMatrixIndices;
        if (indices == null)
            return mesh.BoneMatrices;

        for (int i = 0; i < indices.Length; ++i)
            mesh.BoneMatrices[i] = ModelView(indices[i], modelView);

        return mesh.BoneMatrices;
    }

    // a null motion means the bind pose
    public void SetPose(AppMain.NNS_MOTION motion, float frame)
    {
        // reset first, a stale pose left over from a node the walk doesn't claim is worse than none
        var identity = NNS_MATRIX.CreateIdentity();
        for (int i = 0; i < _pose.Length; ++i)
            _pose[i].Assign(identity);

        AppMain.nnSetUpNodeStatusList(_nodeStatus, _nodeStatus.Length, AppMain.NND_NODESTATUS_NONE);

        if (motion != null)
        {
            AppMain.nnCalcMatrixPaletteMotion(
                _pose, _nodeStatus, Object, motion, frame, null, _stack,
                AppMain.NND_SETNODESTATUS_CLIP_HIDE);
            AppMain.nnCalcNodeHideMotion(new AppMain.ArrayPointer<uint>(_nodeStatus), motion, frame);
        }
        else
        {
            AppMain.nnCalcMatrixPalette(_pose, null, Object, null, _stack, 0U);
        }

        for (int i = 0; i < _pose.Length; ++i)
            Palette[i] = (Matrix)_pose[i];
    }
}

internal sealed class LoadedModel
{
    public readonly List<LoadedObject> Objects = new List<LoadedObject>();
    public readonly List<AppMain.NNS_MOTION> Motions = new List<AppMain.NNS_MOTION>();
    public BoundingBox Bounds;
    public string Summary = "";

    // which object each motion was authored against, by index into Objects
    private readonly List<int> _motionOwner = new List<int>();

    public void AddMotion(AppMain.NNS_MOTION motion)
    {
        Motions.Add(motion);
        _motionOwner.Add(OwnerOf(motion));
    }

    // picks the smallest object with enough nodes for every id the motion references, since a
    // motion for a small object can fit inside a bigger one with no index out of range to catch it
    private int OwnerOf(AppMain.NNS_MOTION motion)
    {
        int highest = -1;
        for (int i = 0; i < motion.nSubmotion && motion.pSubmotion != null; ++i)
            highest = Math.Max(highest, motion.pSubmotion[i].Id);

        int best = -1;
        int bestNodes = int.MaxValue;
        for (int i = 0; i < Objects.Count; ++i)
        {
            int nodes = Objects[i].Object.nNode;
            if (highest < nodes && nodes < bestNodes)
            {
                best = i;
                bestNodes = nodes;
            }
        }

        return best < 0 ? 0 : best;
    }

    public string Describe(int motionIndex)
    {
        if (motionIndex < 0 || motionIndex >= Motions.Count)
            return "motion: bind pose";

        var motion = Motions[motionIndex];
        return $"motion {motionIndex} of {Motions.Count} on object {_motionOwner[motionIndex]}:"
            + $" frames {motion.StartFrame}..{motion.EndFrame} at {motion.FrameRate}fps,"
            + $" {motion.nSubmotion} submotion(s)";
    }

    // motion poses the object it belongs to; every other object holds its bind pose
    public void SetPose(int motionIndex, float frame)
    {
        int owner = motionIndex >= 0 && motionIndex < Motions.Count ? _motionOwner[motionIndex] : -1;

        for (int i = 0; i < Objects.Count; ++i)
            Objects[i].SetPose(i == owner ? Motions[motionIndex] : null, frame);
    }
}

internal static class DXModelLoader
{
    public static LoadedModel Load(
        GraphicsDevice device, string modelPath, string texturePath, string motionPath, bool flipV)
    {
        var log = new List<string>();
        var model = new LoadedModel();
        var min = new Vector3(float.MaxValue);
        var max = new Vector3(float.MinValue);

        foreach (var (bytes, memberName) in ReadArchiveMembers(modelPath, log))
        {
            bool bigEndian = DetectBigEndian(bytes);

            using var stream = new MemoryStream(bytes);
            using BinaryReader reader = bigEndian
                ? new EndianBinaryReader(stream, bigEndian: true)
                : new BinaryReader(stream);

            AppMain.amObjectSetup(out var obj, out var texfilelist, reader);
            if (obj == null)
                continue;

            log.Add($"object {model.Objects.Count} from '{memberName}' ({(bigEndian ? "big" : "little")}-endian):"
                + $" {obj.nNode} node(s), {obj.nMaterial} material(s), {obj.nVtxList} vertex list(s),"
                + $" {obj.nSubobj} subobject(s)");

            var textures = LoadTextures(device, texturePath, log);
            for (int i = 0; texfilelist != null && i < texfilelist.nTex; ++i)
                log.Add($"  texfilelist[{i}] = {texfilelist.pTexFileList[i].Filename}");

            var loaded = new LoadedObject(obj);
            ReportNodes(obj, log);
            BuildMeshes(device, loaded, obj, texfilelist, textures, bigEndian, flipV, log, ref min, ref max);
            model.Objects.Add(loaded);
        }

        if (model.Objects.Count == 0)
            throw new InvalidDataException($"no object chunk in {modelPath}");

        model.Bounds = new BoundingBox(min, max);
        log.Add($"bounds {min} .. {max}");

        LoadMotions(model, motionPath, log);
        model.SetPose(-1, 0f);

        model.Summary = string.Join(Environment.NewLine, log);
        return model;
    }

    private static void LoadMotions(LoadedModel model, string motionPath, List<string> log)
    {
        if (motionPath == null || !File.Exists(motionPath))
            return;

        foreach (var (index, motion, error) in NnArchive.ReadMotions(motionPath))
        {
            if (error != null)
            {
                log.Add($"  motion {index} failed: {error}");
                continue;
            }

            // only node motion moves the palette, and the viewer has nothing to show for the rest
            if ((motion.fType & AppMain.NND_MOTIONTYPE_NODE) == 0)
                continue;

            model.AddMotion(motion);
        }

        log.Add($"{model.Motions.Count} node motion(s) from {Path.GetFileName(motionPath)}");
    }

    private static bool DetectBigEndian(byte[] data)
    {
        uint little = BitConverter.ToUInt32(data, 0);
        if (little == AppMain.NND_CHUNK_HEADER_GLES11 || little == AppMain.NND_CHUNK_HEADER_DXG20)
            return false;

        uint big = BinaryPrimitives.ReverseEndianness(little);
        if (big == AppMain.NND_CHUNK_HEADER_XE)
            return true;

        throw new InvalidDataException($"unrecognised NN file header 0x{little:X8}");
    }

    private static IEnumerable<(byte[] Bytes, string Name)> ReadArchiveMembers(string path, List<string> log)
    {
        byte[] raw = File.ReadAllBytes(path);

        // a bare .zno/.eno is the chunk file itself, with no archive around it
        if (!path.EndsWith(".amb", StringComparison.OrdinalIgnoreCase))
        {
            yield return (raw, Path.GetFileName(path));
            yield break;
        }

        var header = new AppMain.AMS_AMB_HEADER();
        header.data = raw;
        using (var stream = new MemoryStream(raw))
            AppMain.readAmbHeader(stream, ref header);

        log.Add($"archive {Path.GetFileName(path)}: {header.file_num} member(s), real_amb={header.is_real_amb}");

        for (int i = 0; i < header.file_num; ++i)
        {
            if (header.lengths[i] < 32)
                continue;

            var member = new byte[header.lengths[i]];
            Array.Copy(raw, header.offsets[i], member, 0, header.lengths[i]);
            yield return (member, header.files[i] ?? i.ToString());
        }
    }

    private static Dictionary<string, Texture2D> LoadTextures(
        GraphicsDevice device, string texturePath, List<string> log)
    {
        if (texturePath == null || !File.Exists(texturePath))
        {
            log.Add("no texture archive");
            return new Dictionary<string, Texture2D>(StringComparer.OrdinalIgnoreCase);
        }

        byte[] raw = File.ReadAllBytes(texturePath);
        var header = new AppMain.AMS_AMB_HEADER();
        header.data = raw;
        using (var stream = new MemoryStream(raw))
            AppMain.readAmbHeader(stream, ref header);

        return LoadTextures(device, header, log);
    }

    // the members a stage's ZONEx_T.AMB opens with are a .TXB name bundle, not a texture; it fails
    // the DDS read like any other member and is skipped
    public static Dictionary<string, Texture2D> LoadTextures(
        GraphicsDevice device, AppMain.AMS_AMB_HEADER header, List<string> log)
    {
        var byName = new Dictionary<string, Texture2D>(StringComparer.OrdinalIgnoreCase);

        log.Add($"texture archive: {header.file_num} member(s)");

        for (int i = 0; i < header.file_num; ++i)
        {
            string name = Path.GetFileName(header.files[i] ?? i.ToString());
            try
            {
                NormaliseDdsFlags(header.data, header.offsets[i]);
                using var member = new MemoryStream(header.data, header.offsets[i], header.lengths[i]);
                var texture = Texture2D.DDSFromStreamEXT(device, member);
                byName[name] = texture;
                log.Add($"  [{i}] {name} {texture.Width}x{texture.Height} {texture.Format}");
            }
            catch (Exception ex)
            {
                log.Add($"  [{i}] {name} FAILED: {ex.Message}");
            }
        }

        return byName;
    }

    // one archive member, already located, built against textures the caller owns. The stage loader
    // uses this rather than Load: 200-odd block models share one ZONEx_T.AMB between them.
    public static LoadedObject LoadObject(
        GraphicsDevice device,
        AppMain.AmbChunk chunk,
        Dictionary<string, Texture2D> textures,
        List<string> log,
        ref Vector3 min,
        ref Vector3 max,
        uint drawFlag = ViewerDrawFlag,
        uint drawFlagEx = 0)
    {
        var bytes = new byte[chunk.length];
        Array.Copy(chunk.array, chunk.offset, bytes, 0, chunk.length);

        bool bigEndian = DetectBigEndian(bytes);

        using var stream = new MemoryStream(bytes);
        using BinaryReader reader = bigEndian
            ? new EndianBinaryReader(stream, bigEndian: true)
            : new BinaryReader(stream);

        AppMain.amObjectSetup(out var obj, out var texfilelist, reader);
        if (obj == null)
            return null;

        var loaded = new LoadedObject(obj);
        loaded.TexturesByFileIndex = ResolveEveryTexture(texfilelist, textures);
        BuildMeshes(device, loaded, obj, texfilelist, textures, bigEndian, false, log, ref min, ref max, drawFlag, drawFlagEx);
        loaded.SetPose(null, 0f);
        return loaded;
    }

    private static Texture2D[] ResolveEveryTexture(
        AppMain.NNS_TEXFILELIST texfilelist, Dictionary<string, Texture2D> textures)
    {
        if (texfilelist == null)
            return Array.Empty<Texture2D>();

        var discard = new List<string>();
        var byIndex = new Texture2D[texfilelist.nTex];

        for (int i = 0; i < texfilelist.nTex; i++)
            byIndex[i] = ResolveTexture(texfilelist, textures, i, discard);

        return byIndex;
    }

    private const uint ViewerDrawFlag = AppMain.NND_DRAWOBJ_FRAGPARALIGHT1;

    // same route the game takes: build the profile, hash it
    private static string ShaderKeyFor(
        AppMain.NNS_OBJECT obj,
        AppMain.NNS_MESHSET meshset,
        uint subobjFType,
        AppMain.NNS_VTXLIST_DX_DESC desc,
        LoadedMesh mesh,
        List<string> log,
        uint drawFlag = ViewerDrawFlag,
        uint drawFlagEx = 0)
    {
        var matPtr = obj.pMatPtrList[meshset.iMaterial];
        var profile = new AppMain.NNS_STDSHADER_PROFILE();

        // the user sampler bits live in drawFlagEx, a separate word from the NND_DRAWOBJ_* one
        int result = AppMain.nnSetupShaderProfile(
            profile, matPtr, drawFlag, drawFlagEx, subobjFType, desc.Format);

        if (result < 0)
        {
            log.Add("  material is not a stdshader desc, no shader key");
            return null;
        }

        mesh.VtxTfmMode = profile.VtxTfmMode;
        string key = AppMain.nnGetStdShaderName(profile).ToString();
        log.Add($"  shader key {key} VtxTfmMode {profile.VtxTfmMode}");
        return key;
    }

    private const uint DDS_MAGIC = 0x20534444;
    private const uint DDSD_PITCH = 0x8;
    private const uint DDSD_LINEARSIZE = 0x80000;

    // FNA throws on a header with both DDSD_PITCH and DDSD_LINEARSIZE set; clear the unused one
    private static void NormaliseDdsFlags(byte[] data, int offset)
    {
        if (offset + 12 > data.Length || BitConverter.ToUInt32(data, offset) != DDS_MAGIC)
            return;

        uint flags = BitConverter.ToUInt32(data, offset + 8);
        if ((flags & (DDSD_PITCH | DDSD_LINEARSIZE)) != (DDSD_PITCH | DDSD_LINEARSIZE))
            return;

        BitConverter.GetBytes(flags & ~DDSD_PITCH).CopyTo(data, offset + 8);
    }

    private static void BuildMeshes(
        GraphicsDevice device,
        LoadedObject loaded,
        AppMain.NNS_OBJECT obj,
        AppMain.NNS_TEXFILELIST texfilelist,
        Dictionary<string, Texture2D> textures,
        bool bigEndian,
        bool flipV,
        List<string> log,
        ref Vector3 min,
        ref Vector3 max,
        uint drawFlag = ViewerDrawFlag,
        uint drawFlagEx = 0)
    {
        for (int s = 0; s < obj.nSubobj; ++s)
        {
            var subobj = obj.pSubobjList[s];
            for (int m = 0; m < subobj.nMeshset; ++m)
            {
                var meshset = subobj.pMeshsetList[m];
                var mesh = BuildMesh(device, obj, texfilelist, meshset, subobj.fType, textures, bigEndian, flipV, log, ref min, ref max, drawFlag, drawFlagEx);
                if (mesh != null)
                    loaded.Meshes.Add(mesh);
            }
        }
    }

    private static void ReportNodes(AppMain.NNS_OBJECT obj, List<string> log)
    {
        int claimed = 0;
        for (int i = 0; i < obj.nNode; ++i)
        {
            if (obj.pNodeList[i].iMatrix != -1)
                ++claimed;
        }

        log.Add($"{obj.nNode} node(s), {obj.nMtxPal} matrix palette entries, {claimed} claimed by a node");
    }

    private static LoadedMesh BuildMesh(
        GraphicsDevice device,
        AppMain.NNS_OBJECT obj,
        AppMain.NNS_TEXFILELIST texfilelist,
        AppMain.NNS_MESHSET meshset,
        uint subobjFType,
        Dictionary<string, Texture2D> textures,
        bool bigEndian,
        bool flipV,
        List<string> log,
        ref Vector3 min,
        ref Vector3 max,
        uint drawFlag = ViewerDrawFlag,
        uint drawFlagEx = 0)
    {
        if (!(obj.pVtxListPtrList[meshset.iVtxList].pVtxList is AppMain.NNS_VTXLIST_DX_DESC desc))
        {
            log.Add($"meshset skipped: vertex list {meshset.iVtxList} is not DXG20");
            return null;
        }

        if (!(obj.pPrimListPtrList[meshset.iPrimList].pPrimList is AppMain.NNS_PRIMTYPE_DX_STRIPLIST strip))
        {
            log.Add($"meshset skipped: primitive list {meshset.iPrimList} is not a DXG20 strip");
            return null;
        }

        static Vector2 ReadUv(Vector2[] set, int i, bool flipV)
        {
            if (set == null)
                return Vector2.Zero;
            var uv = set[i];
            if (flipV)
                uv.Y = 1f - uv.Y;
            return uv;
        }

        var arrays = DXVertexArrays.Decode(desc, bigEndian);
        var indices = DXStripList.ToTriangleList(strip);
        if (indices.Length == 0)
            return null;

        var vertices = new DXVertex[desc.nVertex];
        for (int i = 0; i < vertices.Length; ++i)
        {
            var position = arrays.Positions[i];
            var uv = ReadUv(arrays.TexCoords, i, flipV);
            var uv2 = ReadUv(arrays.TexCoords2, i, flipV);
            var uv3 = ReadUv(arrays.TexCoords3, i, flipV);

            var weights = arrays.BlendWeights != null ? arrays.BlendWeights[i] : Vector4.Zero;
            var bones = arrays.BlendIndices != null ? arrays.BlendIndices[i] : null;

            vertices[i] = new DXVertex
            {
                Position = position,
                BlendWeights = new Vector3(weights.X, weights.Y, weights.Z),
                BlendIndices = bones == null
                    ? new Byte4(0f, 0f, 0f, 0f)
                    : new Byte4(bones[0], bones[1], bones[2], bones[3]),
                Normal = arrays.Normals != null ? arrays.Normals[i] : Vector3.Up,
                TexCoord = uv,
                TexCoord2 = uv2,
                TexCoord3 = uv3,
                Color = arrays.Colors != null ? arrays.Colors[i] : Color.White,
            };

            min = Vector3.Min(min, position);
            max = Vector3.Max(max, position);
        }

        var mesh = new LoadedMesh();
        mesh.Vertices = new VertexBuffer(device, DXVertex.Declaration, vertices.Length, BufferUsage.WriteOnly);
        mesh.Vertices.SetData(vertices);
        mesh.Indices = new IndexBuffer(device, IndexElementSize.SixteenBits, indices.Length, BufferUsage.WriteOnly);
        mesh.Indices.SetData(indices);
        mesh.VertexCount = vertices.Length;
        mesh.TriangleCount = indices.Length / 3;
        mesh.MatrixIndex = meshset.iMatrix;
        mesh.NodeIndex = meshset.iNode;

        if (desc.pBoneMtxList != null && desc.nBoneMtx > 0)
        {
            mesh.BoneMatrixIndices = new int[desc.nBoneMtx];
            for (int i = 0; i < desc.nBoneMtx; ++i)
                mesh.BoneMatrixIndices[i] = (int)desc.pBoneMtxList[i];

            mesh.BoneMatrices = new Matrix[desc.nBoneMtx];
        }

        ApplyMaterial(obj, texfilelist, meshset, textures, mesh, log);
        mesh.Permutation = ShaderKeyFor(obj, meshset, subobjFType, desc, mesh, log, drawFlag, drawFlagEx);

        if (mesh.VtxTfmMode != 0)
            log.Add($"  {desc.nBoneMtx} bone matrix indices [{string.Join(",", mesh.BoneMatrixIndices ?? Array.Empty<int>())}]");

        log.Add($"meshset: {desc.nVertex} vertices, {mesh.TriangleCount} triangles, texture={(mesh.Texture == null ? "none" : mesh.Texture.Width + "x" + mesh.Texture.Height)}");
        return mesh;
    }

    private static void ApplyMaterial(
        AppMain.NNS_OBJECT obj,
        AppMain.NNS_TEXFILELIST texfilelist,
        AppMain.NNS_MESHSET meshset,
        Dictionary<string, Texture2D> textures,
        LoadedMesh mesh,
        List<string> log)
    {
        if (meshset.iMaterial < 0 || meshset.iMaterial >= obj.nMaterial)
            return;

        if (!(obj.pMatPtrList[meshset.iMaterial].pMaterial is AppMain.NNS_MATERIAL_STDSHADER_DESC material))
            return;

        if (material.pColor != null)
        {
            mesh.DiffuseColor = new Vector3(
                material.pColor.Diffuse.r,
                material.pColor.Diffuse.g,
                material.pColor.Diffuse.b);
        }

        mesh.Material = material;
        mesh.MaterialPtr = obj.pMatPtrList[meshset.iMaterial];
        mesh.MaterialIndex = meshset.iMaterial;

        if (material.pColor != null)
        {
            var c = material.pColor;
            log.Add($"  material fTexType=0x{material.fTexType:X8}"
                + $" diffuse=({c.Diffuse.r},{c.Diffuse.g},{c.Diffuse.b},{c.Diffuse.a})"
                + $" ambient=({c.Ambient.r},{c.Ambient.g},{c.Ambient.b})"
                + $" specular=({c.Specular.r},{c.Specular.g},{c.Specular.b})"
                + $" emission=({c.Emission.r},{c.Emission.g},{c.Emission.b})"
                + $" shininess={c.Shininess}");
        }

        if (material.pTexDesc == null || material.pTexDesc.Length == 0)
            return;

        mesh.Texture = ResolveTexture(texfilelist, textures, material.pTexDesc[0].iTexIdx, log);
        BuildSamplerSlots(material, texfilelist, textures, mesh, log);
    }

    // puts each map in the register sampler_def.h would have given it
    private static void BuildSamplerSlots(
        AppMain.NNS_MATERIAL_STDSHADER_DESC material,
        AppMain.NNS_TEXFILELIST texfilelist,
        Dictionary<string, Texture2D> textures,
        LoadedMesh mesh,
        List<string> log)
    {
        int slots = AppMain.mppStdShaderSamplerCount(material.fTexType);
        if (slots == 0)
            return;

        mesh.SamplerTextures = new Texture2D[slots];
        mesh.SamplerStates = new SamplerState[slots];

        foreach (var desc in material.pTexDesc)
        {
            uint mapBit = desc.fType & AppMain.NND_TEXTURETYPE_STDSHADER_MAP_MASK;
            int slot = AppMain.mppStdShaderSamplerSlot(material.fTexType, mapBit);
            if (slot < 0 || slot >= slots)
            {
                log.Add($"texmap fType 0x{desc.fType:X8} has no sampler slot in fTexType 0x{material.fTexType:X8}");
                continue;
            }

            mesh.SamplerTextures[slot] = ResolveTexture(texfilelist, textures, desc.iTexIdx, log);
            mesh.SamplerStates[slot] = new SamplerState
            {
                Filter = TextureFilter.Linear,
                AddressU = NnStdShaderEffect.AddressModeOf(desc.WrapS),
                AddressV = NnStdShaderEffect.AddressModeOf(desc.WrapT),
            };

            log.Add($"  sampler s{slot} <- map 0x{mapBit:X} tex {desc.iTexIdx} texcoord {desc.TexCoord} blend {desc.Blend}");
        }
    }

    // match archive members by name against the texture chunk, fall back to position if unnamed
    private static Texture2D ResolveTexture(
        AppMain.NNS_TEXFILELIST texfilelist,
        Dictionary<string, Texture2D> textures,
        int texIndex,
        List<string> log)
    {
        if (texfilelist == null || texIndex < 0 || texIndex >= texfilelist.nTex)
            return null;

        string wanted = Path.GetFileNameWithoutExtension(texfilelist.pTexFileList[texIndex].Filename);
        foreach (var candidate in textures)
        {
            if (Path.GetFileNameWithoutExtension(candidate.Key).Equals(wanted, StringComparison.OrdinalIgnoreCase))
                return candidate.Value;
        }

        // no positional fallback: the dictionary is in hash order, so "member texIndex" picked an
        // essentially arbitrary texture and quietly bound the wrong one. Better to draw untextured
        // and say so than to draw something plausible and wrong.
        log.Add($"texture '{wanted}' is not in the texture archive");
        return null;
    }
}
