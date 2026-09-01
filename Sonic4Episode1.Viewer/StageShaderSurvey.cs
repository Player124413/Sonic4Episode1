using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sonic4Episode1.Viewer;

// Which nnstdshader permutation every block model in a stage selects, and whether it was built.
// Device free: nnSetupShaderProfile and nnGetStdShaderName need the parsed material and the vertex
// format, nothing else, so this runs without a window.
internal static class StageShaderSurvey
{
    public static int Run(string stage, string root)
    {
        if (!StageTable.TryResolve(stage, out int stageId) || !StageTable.HasBlockMap(stageId))
        {
            Console.Error.WriteLine($"no stage with a block map called {stage}");
            return 1;
        }

        var map = new StageMap(root, stageId);
        map.Load();

        if (map.ModelFormat != StageModelFormat.NnObject)
        {
            Console.Error.WriteLine($"{map.Name} ships {map.ModelFormat} models");
            return 1;
        }

        var built = NnShaderCache.BuiltPermutations();
        var keys = new Dictionary<string, int>(StringComparer.Ordinal);
        var fields = new Dictionary<string, string>(StringComparer.Ordinal);
        var texTypes = new Dictionary<uint, int>();
        int meshes = 0;

        for (int i = 0; i < map.ModelCount; i++)
        {
            var obj = NnArchive.ReadObject(AppMain.amBindGet(map.ModelArchive, i, out _));
            if (obj == null)
                continue;

            // StageScene tags the WATER models with the user sampler, which changes their key
            uint drawFlagEx =
                (map.ModelArchive.user0[i] & AppMain.GMD_MAP_MODEL_USER_FLAG0_WATER) != 0
                    ? AppMain.NND_DRAWOBJ_MATCTRL_USERSAMPLER2D1
                    : 0U;

            for (int s = 0; s < obj.nSubobj; s++)
            {
                var subobj = obj.pSubobjList[s];
                for (int m = 0; m < subobj.nMeshset; m++)
                {
                    var meshset = subobj.pMeshsetList[m];
                    if (!(obj.pVtxListPtrList[meshset.iVtxList].pVtxList is AppMain.NNS_VTXLIST_DX_DESC desc))
                        continue;

                    var matPtr = obj.pMatPtrList[meshset.iMaterial];
                    var profile = new AppMain.NNS_STDSHADER_PROFILE();
                    if (AppMain.nnSetupShaderProfile(
                            profile, matPtr, DrawFlag, drawFlagEx, subobj.fType, desc.Format) < 0)
                        continue;

                    meshes++;
                    var key = AppMain.nnGetStdShaderName(profile).ToString();
                    keys.TryGetValue(key, out int n);
                    keys[key] = n + 1;

                    if (!fields.ContainsKey(key))
                    {
                        var set = new List<string>();
                        for (int f = 0; f < AppMain.NNS_STDSHADER_PROFILE.FieldCount; f++)
                        {
                            if (profile[f] != 0)
                                set.Add($"{f}={profile[f]}");
                        }
                        fields[key] = string.Join(" ", set);
                    }

                    if (matPtr.pMaterial is AppMain.NNS_MATERIAL_STDSHADER_DESC mat)
                    {
                        texTypes.TryGetValue(mat.fTexType, out int t);
                        texTypes[mat.fTexType] = t + 1;
                    }
                }
            }
        }

        Console.WriteLine($"{map.Name}: {map.ModelCount} model(s), {meshes} meshset(s),"
            + $" {keys.Count} distinct permutation(s)");

        foreach (var pair in keys.OrderByDescending(p => p.Value))
        {
            bool have = built.Contains(pair.Key);
            Console.WriteLine($"  {pair.Key}  x{pair.Value,-6} {(have ? "built" : "*** NOT BUILT ***")}");
            Console.WriteLine($"      profile: {fields[pair.Key]}");
        }

        Console.WriteLine("fTexType:");
        foreach (var pair in texTypes.OrderByDescending(p => p.Value))
            Console.WriteLine($"  0x{pair.Key:X8}  x{pair.Value}");

        Console.WriteLine("models tagged WATER, and the textures they name:");
        for (int i = 0; i < map.ModelCount; i++)
        {
            if ((map.ModelArchive.user0[i] & AppMain.GMD_MAP_MODEL_USER_FLAG0_WATER) == 0)
                continue;

            var chunk = AppMain.amBindGet(map.ModelArchive, i, out _);
            var names = TextureNames(chunk);
            Console.WriteLine($"  member {i,4}: {string.Join(", ", names)}");
        }

        return 0;
    }

    // the same flag StageScene builds its block models with
    private const uint DrawFlag = AppMain.NND_DRAWOBJ_DOUBLESIDE | AppMain.NND_DRAWOBJ_FRAGPARALIGHT1;

    private static List<string> TextureNames(AppMain.AmbChunk chunk)
    {
        var names = new List<string>();
        var list = NnArchive.ReadTexFileList(chunk);

        for (int i = 0; list != null && i < list.nTex; i++)
            names.Add(Path.GetFileName(list.pTexFileList[i].Filename));

        return names;
    }
}
