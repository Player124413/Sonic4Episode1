using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sonic4Episode1.Viewer;

// What a stage's DECO archive holds, and which nnstdshader permutation each decoration model
// selects. The block twin of this is StageShaderSurvey; both run without a graphics device.
//
// DECO_ZONEx.AMB is six nested archives. Members 0 and 1 are the decoration models and their
// textures, which console ships as NN objects and mobile as baked TVX. Members 4 and 5 are the
// waterfalls, NN on both. See docs/console-map-blocks.md for the block equivalent.
internal static class StageDecoSurvey
{
    public static int Run(string stage, string root)
    {
        if (!StageTable.TryResolve(stage, out int stageId))
        {
            Console.Error.WriteLine($"no stage called {stage}");
            return 1;
        }

        var relative = StageTable.DecoPath(stageId);
        if (relative == null)
        {
            Console.Error.WriteLine($"stage {stageId} loads no decorations");
            return 1;
        }

        var path = Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar));
        if (!File.Exists(path))
        {
            Console.Error.WriteLine($"{relative} is not in {root}");
            return 1;
        }

        var deco = NnArchive.Open(path, out _);
        Console.WriteLine($"stage {stageId} {StageTable.Name(stageId) ?? "(no block map)"},"
            + $" zone type {StageTable.ZoneType(stageId)}");
        Console.WriteLine($"{relative}: {deco.file_num} member(s)"
            + (deco.big_endian ? ", big endian" : ", little endian"));

        for (int i = 0; i < deco.file_num; i++)
        {
            var nested = NnArchive.ReadNested(AppMain.amBindGet(deco, i, out _));
            Console.WriteLine($"  {i} {MemberName(i),-12} {Path.GetFileName(deco.files[i].Replace('\\', '/')),-26}"
                + $" {deco.lengths[i],9} byte(s), {nested?.file_num.ToString() ?? "not an #AMB"} member(s)");
        }

        var built = NnShaderCache.BuiltPermutations();

        Survey(deco, AppMain.GMD_DECO_DATA_INDEX_AMB_MDL, "decorations", 0, built);

        // the falls are surveyed both ways because the two milestones want different answers:
        // without the sampler is what they draw as today, with it is the refraction route
        Survey(deco, AppMain.GMD_DECO_DATA_INDEX_AMB_MDL_RENDER, "waterfalls, no user sampler",
            0, built);
        Survey(deco, AppMain.GMD_DECO_DATA_INDEX_AMB_MDL_RENDER, "waterfalls, user sampler",
            AppMain.NND_DRAWOBJ_MATCTRL_USERSAMPLER2D1, built);

        return 0;
    }

    // GmDecoBuildData passes 0 as draw_flag for both sets, and gmDeco.cpp:4396 puts the user
    // sampler in draw_flag_ex on the falls, which is what decides whether the key asks for it
    private static void Survey(
        AppMain.AMS_AMB_HEADER deco, int member, string label, uint drawFlagEx, HashSet<string> built)
    {
        Console.WriteLine();
        var models = NnArchive.ReadNested(AppMain.amBindGet(deco, member, out _));

        if (models == null || models.file_num == 0)
        {
            Console.WriteLine($"{label} (member {member}): empty");
            return;
        }

        Console.WriteLine($"{label} (member {member}): {models.file_num} member(s),"
            + $" drawFlagEx 0x{drawFlagEx:X8}");

        int nnObjects = 0;

        for (int i = 0; i < models.file_num; i++)
        {
            var chunk = AppMain.amBindGet(models, i, out _);
            var name = Path.GetFileName(models.files[i].Replace('\\', '/'));
            var obj = NnArchive.ReadObject(chunk);

            if (obj == null)
            {
                Console.WriteLine($"  [{i,3}] {name,-26} not an NN object"
                    + $" ({Tag(chunk)}, {models.lengths[i]} bytes)");
                continue;
            }

            nnObjects++;
            var textures = TextureNames(chunk);
            Console.WriteLine($"  [{i,3}] {name,-26} {string.Join(", ", textures)}");

            var permutations = Permutations(obj, drawFlagEx);
            if (permutations.Count == 0)
            {
                Console.WriteLine("        no meshset carries a DX vertex list, so no key is selected");
                continue;
            }

            foreach (var line in permutations)
            {
                bool have = built.Contains(line.Key);
                Console.WriteLine($"        {line.Key}  x{line.Count,-4}"
                    + $" {(have ? "built" : "*** NOT BUILT ***")}"
                    + (line.UserProfile > 0 ? $"  user profile {line.UserProfile}" : ""));
            }
        }

        Console.WriteLine($"  {nnObjects} of {models.file_num} member(s) parsed as NN objects");
    }

    // one row per distinct permutation this model selects, since a model's meshsets usually share
    private static List<(string Key, int Count, int UserProfile)> Permutations(
        AppMain.NNS_OBJECT obj, uint drawFlagEx)
    {
        var keys = new Dictionary<string, (int Count, int UserProfile)>(StringComparer.Ordinal);

        for (int s = 0; s < obj.nSubobj; s++)
        {
            var subobj = obj.pSubobjList[s];
            for (int m = 0; m < subobj.nMeshset; m++)
            {
                var meshset = subobj.pMeshsetList[m];
                if (!(obj.pVtxListPtrList[meshset.iVtxList].pVtxList is AppMain.NNS_VTXLIST_DX_DESC desc))
                    continue;

                var profile = new AppMain.NNS_STDSHADER_PROFILE();
                if (AppMain.nnSetupShaderProfile(
                        profile, obj.pMatPtrList[meshset.iMaterial], DrawFlag, drawFlagEx,
                        subobj.fType, desc.Format) < 0)
                    continue;

                var key = AppMain.nnGetStdShaderName(profile).ToString();
                keys.TryGetValue(key, out var seen);
                keys[key] = (seen.Count + 1, profile.UserProfile);
            }
        }

        return keys.Select(p => (p.Key, p.Value.Count, p.Value.UserProfile)).ToList();
    }

    // GmDecoBuildData passes 0 as draw_flag, but the key is picked at draw time, where
    // nnBindStdShader forces the fragment light on. Survey what will actually be asked for.
    private const uint DrawFlag = AppMain.NND_DRAWOBJ_FRAGPARALIGHT1;

    private static List<string> TextureNames(AppMain.AmbChunk chunk)
    {
        var names = new List<string>();
        var list = NnArchive.ReadTexFileList(chunk);

        for (int i = 0; list != null && i < list.nTex; i++)
            names.Add(Path.GetFileName(list.pTexFileList[i].Filename));

        return names;
    }

    private static string MemberName(int index)
    {
        switch (index)
        {
            case AppMain.GMD_DECO_DATA_INDEX_AMB_MDL: return "MDL";
            case AppMain.GMD_DECO_DATA_INDEX_AMB_TEX: return "TEX";
            case AppMain.GMD_DECO_DATA_INDEX_AMB_MTN: return "MTN";
            case AppMain.GMD_DECO_DATA_INDEX_AMB_MAT: return "MAT";
            case AppMain.GMD_DECO_DATA_INDEX_AMB_MDL_RENDER: return "MDL_FALL";
            case AppMain.GMD_DECO_DATA_INDEX_AMB_TEX_RENDER: return "TEX_FALL";
            default: return "";
        }
    }

    private static string Tag(AppMain.AmbChunk chunk)
    {
        if (chunk == null || chunk.length < 4)
            return "empty";

        var text = new char[4];
        for (int i = 0; i < 4; i++)
        {
            byte b = chunk.array[chunk.offset + i];
            text[i] = b >= 0x20 && b < 0x7F ? (char)b : '.';
        }

        return new string(text);
    }
}
