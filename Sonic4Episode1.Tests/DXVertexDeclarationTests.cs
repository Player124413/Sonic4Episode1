using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace Sonic4Episode1.Tests;

// D3D11's CreateInputLayout rejects a declaration missing anything the vertex shader declares, so
// DXVertex has to cover every permutation in the retail set. GL only leaves the attribute disabled,
// which is why this went unnoticed. See docs/nn-stdshader-ubershader.md.
public class DXVertexDeclarationTests
{
    private static readonly string KeyFile = Path.Combine(
        Path.GetDirectoryName(typeof(DXVertexDeclarationTests).Assembly.Location),
        "Fixtures", "nnstdshader-keys.txt");

    // nnstdshader.vsh:687-720, at the NND_PROF_VTX_SPECULAR and NND_PROF_CALC_VTX_BINORMAL that
    // Shaders/nn/Build-Permutations.ps1 compiles with
    private static IEnumerable<(VertexElementUsage, int)> DeclaredInputs(AppMain.NNS_STDSHADER_PROFILE p)
    {
        yield return (VertexElementUsage.Position, 0);

        if (p.VtxTfmMode != 0)
            yield return (VertexElementUsage.BlendWeight, 0);
        if (p.VtxTfmMode == 2)
            yield return (VertexElementUsage.BlendIndices, 0);
        if (p.bVtxNrm != 0)
            yield return (VertexElementUsage.Normal, 0);
        if (p.bVtxCol != 0)
            yield return (VertexElementUsage.Color, 0);

        int texCoords = 0;
        for (int i = 0; i < 8; ++i)
            texCoords = Math.Max(texCoords, p.GetTexCoord(i) + 1);
        for (int i = 0; i < texCoords; ++i)
            yield return (VertexElementUsage.TextureCoordinate, i);

        if (p.NormalMapType != 0)
        {
            yield return (VertexElementUsage.Tangent, 0);
            yield return (VertexElementUsage.Binormal, 0);
        }
    }

    [Fact]
    public void DeclarationCoversEveryRetailPermutation()
    {
        var provided = DXVertex.Declaration.GetVertexElements()
            .Select(e => (e.VertexElementUsage, e.UsageIndex))
            .ToHashSet();

        var keys = File.ReadAllLines(KeyFile).Select(l => l.Trim()).Where(l => l.Length > 0).ToArray();
        Assert.NotEmpty(keys);

        var missing = new SortedSet<string>();
        foreach (var key in keys)
        {
            var profile = new AppMain.NNS_STDSHADER_PROFILE();
            AppMain.nnGetStdShaderProfile(profile, AppMain.mppStdShaderNameParse(key));

            foreach (var input in DeclaredInputs(profile))
                if (!provided.Contains(input))
                    missing.Add($"{input.Item1}{input.Item2}");
        }

        Assert.Empty(missing);
    }

    // offsets have to match the struct layout or the buffer is read at the wrong stride
    [Fact]
    public void ElementOffsetsAreContiguousAndTheStrideCoversThem()
    {
        var elements = DXVertex.Declaration.GetVertexElements()
            .OrderBy(e => e.Offset)
            .ToArray();

        Assert.Equal(0, elements[0].Offset);

        for (int i = 1; i < elements.Length; i++)
            Assert.True(elements[i].Offset > elements[i - 1].Offset, "offsets must ascend");

        Assert.True(DXVertex.Declaration.VertexStride > elements[^1].Offset,
            "stride must cover the last element");
    }
}
