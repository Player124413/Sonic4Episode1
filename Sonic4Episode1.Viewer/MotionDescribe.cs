using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sonic4Episode1.Viewer;

// prints what one motion is made of, for when it looks wrong and its neighbours look fine
internal static class MotionDescribe
{
    public static int Run(string archivePath, int first, int last)
    {
        if (!File.Exists(archivePath))
        {
            Console.Error.WriteLine($"no such archive: {archivePath}");
            return 1;
        }

        foreach (var (index, motion, error) in NnArchive.ReadMotions(archivePath))
        {
            if (index < first || index > last)
                continue;

            if (error != null)
            {
                Console.WriteLine($"motion {index}: FAILED {error}");
                continue;
            }

            Describe(index, motion);
        }

        return 0;
    }

    public static int RunModel(string archivePath)
    {
        if (!File.Exists(archivePath))
        {
            Console.Error.WriteLine($"no such archive: {archivePath}");
            return 1;
        }

        var objects = NnArchive.ReadObjects(archivePath);
        Console.WriteLine($"{objects.Count} object(s) in {Path.GetFileName(archivePath)}");

        for (int i = 0; i < objects.Count; ++i)
        {
            var obj = objects[i];
            Console.WriteLine($"  object {i}: {obj.nNode} node(s), {obj.nMtxPal} palette entries,"
                + $" {obj.nMaterial} material(s), {obj.nSubobj} subobject(s), {obj.nVtxList} vertex list(s)");

            DescribeSkinning(obj);
        }

        return 0;
    }

    // Whether a mesh is skinned is not a property of the model, it is what nnSetupShaderProfile
    // decides from the subobject type and the vertex format. A mesh that comes out rigid ignores
    // node motion entirely and draws in its bind pose, which looks like "the motion is not playing".
    private static void DescribeSkinning(AppMain.NNS_OBJECT obj)
    {
        for (int s = 0; s < obj.nSubobj; s++)
        {
            var subobj = obj.pSubobjList[s];

            for (int m = 0; m < subobj.nMeshset; m++)
            {
                var meshset = subobj.pMeshsetList[m];
                if (!(obj.pVtxListPtrList[meshset.iVtxList].pVtxList is AppMain.NNS_VTXLIST_DX_DESC desc))
                {
                    Console.WriteLine($"    subobj {s} mesh {m}: not a DX vertex list");
                    continue;
                }

                bool rigidType = (subobj.fType & AppMain.NND_SUBOBJTYPE_RIGID) != 0;
                uint blend = desc.Format & AppMain.NND_VTXMEMBER_DX_BLEND_MASK;
                bool rigidBlend = blend != AppMain.NND_VTXMEMBER_DX_BLEND4FIXED;
                bool rigid = rigidType || rigidBlend;

                int mode = rigid ? 0 : ((desc.Format & AppMain.NND_VTXMEMBER_DX_BLENDFMT_I8) != 0 ? 2 : 1);

                // shader skinning reads the bones out of pBoneMtxList; without it
                // nnSetShaderBoneMatrix leaves the count at zero and the mesh draws in bind pose
                var bones = desc.pBoneMtxList == null
                    ? "pBoneMtxList NULL, would draw in bind pose"
                    : $"nBoneMtx {desc.nBoneMtx}, list {desc.pBoneMtxList.Length}";

                Console.WriteLine($"    subobj {s} mesh {m}: VtxTfmMode {mode}"
                    + $" ({(rigid ? "rigid, ignores node motion" : "skinned")})"
                    + $"  subobjType 0x{subobj.fType:X}"
                    + $"  vtxFormat 0x{desc.Format:X8} blend 0x{blend:X}"
                    + (rigidType ? "  SUBOBJTYPE_RIGID" : "")
                    + (rigidBlend ? "  blend is not BLEND4FIXED" : ""));
                Console.WriteLine($"        {bones}");

                // the permutation the mesh actually asks for, at the draw-time flags
                var profile = new AppMain.NNS_STDSHADER_PROFILE();
                if (AppMain.nnSetupShaderProfile(
                        profile, obj.pMatPtrList[meshset.iMaterial],
                        AppMain.NND_DRAWOBJ_FRAGPARALIGHT1, 0, subobj.fType, desc.Format) < 0)
                {
                    Console.WriteLine("        no profile");
                    continue;
                }

                var key = AppMain.nnGetStdShaderName(profile).ToString();
                Console.WriteLine($"        key {key} profile.VtxTfmMode={profile.VtxTfmMode}"
                    + $"  {(NnShaderCache.BuiltPermutations().Contains(key) ? "built" : "*** NOT BUILT ***")}");

                // what the skinning actually reads per vertex. Weights that decode to nothing, or
                // indices that decode reversed, both leave a model that draws but never deforms.
                var arrays = DXVertexArrays.Decode(desc, desc.mppBigEndian);
                Console.WriteLine($"        bigEndian={desc.mppBigEndian} nVertex={desc.nVertex}"
                    + $" stride={desc.Stride}");

                if (arrays.TexCoords != null)
                {
                    float u0 = float.MaxValue, u1 = float.MinValue;
                    float v0 = float.MaxValue, v1 = float.MinValue;
                    foreach (var uv in arrays.TexCoords)
                    {
                        u0 = Math.Min(u0, uv.X); u1 = Math.Max(u1, uv.X);
                        v0 = Math.Min(v0, uv.Y); v1 = Math.Max(v1, uv.Y);
                    }
                    Console.WriteLine($"          uv u {u0:0.000}..{u1:0.000}  v {v0:0.000}..{v1:0.000}");
                }

                for (int v = 0; v < Math.Min(3, desc.nVertex); v++)
                {
                    var w = arrays.BlendWeights != null ? arrays.BlendWeights[v] : Microsoft.Xna.Framework.Vector4.Zero;
                    var idx = arrays.BlendIndices != null
                        ? string.Join(",", arrays.BlendIndices[v])
                        : "none";
                    Console.WriteLine($"          v{v} wt=({w.X:0.000},{w.Y:0.000},{w.Z:0.000},{w.W:0.000})"
                        + $" idx=[{idx}]");
                }
            }
        }
    }

    private static void Describe(int index, AppMain.NNS_MOTION motion)
    {
        uint category = motion.fType & AppMain.NND_MOTIONTYPE_CATEGORY_MASK;
        uint repeat = motion.fType & AppMain.NND_MOTIONTYPE_REPEAT_MASK;

        Console.WriteLine(
            $"motion {index}: fType 0x{motion.fType:X} category 0x{category:X} repeat {RepeatName(repeat)}"
            + $" frames {motion.StartFrame}..{motion.EndFrame} at {motion.FrameRate}fps"
            + $" {motion.nSubmotion} submotion(s)");

        if (motion.pSubmotion == null)
            return;

        var shapes = new Dictionary<string, int>();
        int lowestNode = int.MaxValue;
        int highestNode = -1;
        var repeats = new Dictionary<string, int>();

        for (int i = 0; i < motion.nSubmotion; ++i)
        {
            var sub = motion.pSubmotion[i];
            lowestNode = Math.Min(lowestNode, sub.Id);
            highestNode = Math.Max(highestNode, sub.Id);

            string keys = sub.pKeyList == null
                ? "null"
                : sub.pKeyList.GetType().GetElementType()?.Name?.Replace("NNS_MOTION_KEY_", "") ?? "?";

            string shape = $"fType 0x{sub.fType:X} ip 0x{sub.fIPType & AppMain.NND_SMOTIPTYPE_IP_MASK:X}"
                + $" KeySize {sub.KeySize} {keys}";
            shapes[shape] = shapes.TryGetValue(shape, out int n) ? n + 1 : 1;

            string sr = RepeatName(sub.fIPType & AppMain.NND_SMOTIPTYPE_REPEAT_MASK);
            repeats[sr] = repeats.TryGetValue(sr, out int m) ? m + 1 : 1;

            // an empty key list evaluates to something finite and wrong, not a failure
            if (sub.nKeyFrame <= 0)
                Console.WriteLine($"    submotion {i} node {sub.Id} has {sub.nKeyFrame} keys");
        }

        Console.WriteLine($"  nodes {lowestNode}..{highestNode}");
        Console.WriteLine($"  submotion repeat: {string.Join(", ", repeats.Select(r => $"{r.Key} x{r.Value}"))}");
        foreach (var shape in shapes.OrderByDescending(s => s.Value))
            Console.WriteLine($"  {shape.Value,5}x {shape.Key}");
    }

    private static string RepeatName(uint repeat)
    {
        if (repeat == 0) return "none";

        var parts = new List<string>();
        if ((repeat & AppMain.NND_MOTIONTYPE_NOREPEAT) != 0) parts.Add("NOREPEAT");
        if ((repeat & AppMain.NND_MOTIONTYPE_CONSTREPEAT) != 0) parts.Add("CONSTREPEAT");
        if ((repeat & AppMain.NND_MOTIONTYPE_REPEAT) != 0) parts.Add("REPEAT");
        if ((repeat & AppMain.NND_SMOTIPTYPE_MIRROR) != 0) parts.Add("MIRROR");
        if ((repeat & AppMain.NND_SMOTIPTYPE_OFFSET) != 0) parts.Add("OFFSET");

        return parts.Count == 0 ? $"0x{repeat:X}" : string.Join("|", parts);
    }
}
