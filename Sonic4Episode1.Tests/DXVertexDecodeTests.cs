using System;
using System.IO;
using System.Linq;

namespace Sonic4Episode1.Tests;

public class DxVertexDecodeTests
{
    private static string Fixture(string name)
    {
        return Path.Combine(Path.GetDirectoryName(typeof(DxVertexDecodeTests).Assembly.Location), "Fixtures", name);
    }

    private static AppMain.NNS_OBJECT ReadLittleEndian(string name)
    {
        using var stream = File.OpenRead(Fixture(name));
        using var reader = new BinaryReader(stream);

        AppMain.amObjectSetup(out var obj, out _, reader);
        return obj;
    }

    private static AppMain.NNS_OBJECT ReadRingBigEndian()
    {
        var header = new AppMain.AMS_AMB_HEADER();
        header.data = File.ReadAllBytes(Fixture("RING_MDL_HD.AMB"));

        using (var stream = new MemoryStream(header.data))
            AppMain.readAmbHeader(stream, ref header);

        using var member = new MemoryStream(header.data, header.offsets[0], header.lengths[0]);
        using var reader = new EndianBinaryReader(member, bigEndian: true);

        AppMain.amObjectSetup(out var obj, out _, reader);
        return obj;
    }

    [Fact]
    public void DecodesPlanePositionsAndNormals()
    {
        var obj = ReadLittleEndian("NN_sample_Plane01.zno");
        var desc = (AppMain.NNS_VTXLIST_DX_DESC)obj.pVtxListPtrList[0].pVtxList;

        var arrays = DXVertexArrays.Decode(desc, bigEndian: false);

        Assert.Equal(25, arrays.Positions.Length);
        Assert.Equal(25, arrays.Normals.Length);
        Assert.Null(arrays.TexCoords);

        // a plane: every normal points the same way, and it is a unit vector
        foreach (var normal in arrays.Normals)
        {
            Assert.Equal(1f, normal.Length(), 3);
            Assert.Equal(arrays.Normals[0], normal);
        }

        // 25 vertices on a flat 5x5 grid, so one axis has no extent
        Assert.True(Extent(arrays.Positions.Select(p => p.X)) < 0.001f
                 || Extent(arrays.Positions.Select(p => p.Y)) < 0.001f
                 || Extent(arrays.Positions.Select(p => p.Z)) < 0.001f);
    }

    [Fact]
    public void DecodesBigEndianRing()
    {
        var obj = ReadRingBigEndian();
        var desc = (AppMain.NNS_VTXLIST_DX_DESC)obj.pVtxListPtrList[0].pVtxList;

        var arrays = DXVertexArrays.Decode(desc, bigEndian: true);

        Assert.Equal(533, arrays.Positions.Length);
        Assert.Equal(533, arrays.Normals.Length);
        Assert.Equal(533, arrays.TexCoords.Length);

        // byte-swapped floats come out as denormals or absurd magnitudes
        foreach (var normal in arrays.Normals)
            Assert.Equal(1f, normal.Length(), 2);

        foreach (var position in arrays.Positions)
        {
            Assert.True(float.IsFinite(position.X) && float.IsFinite(position.Y) && float.IsFinite(position.Z));
            Assert.True(position.Length() < 1000f, $"implausible position {position}");
        }

        foreach (var uv in arrays.TexCoords)
            Assert.InRange(uv.X, -8f, 8f);

        // a ring is a torus, so it has real extent on two axes and is centred near the origin
        Assert.True(Extent(arrays.Positions.Select(p => p.X)) > 1f);
        Assert.True(Extent(arrays.Positions.Select(p => p.Y)) > 1f);
    }

    [Fact]
    public void FlattensRingStripIntoTriangles()
    {
        var obj = ReadRingBigEndian();
        var desc = (AppMain.NNS_VTXLIST_DX_DESC)obj.pVtxListPtrList[0].pVtxList;
        var strip = (AppMain.NNS_PRIMTYPE_DX_STRIPLIST)obj.pPrimListPtrList[0].pPrimList;

        var indices = DXStripList.ToTriangleList(strip);

        Assert.Equal(0, indices.Length % 3);
        Assert.NotEmpty(indices);
        Assert.True(indices.Length / 3 <= strip.nIndex - 2, "a strip cannot yield more triangles than it has indices");
        Assert.All(indices, i => Assert.InRange(i, 0, (int)desc.nVertex - 1));

        for (int i = 0; i < indices.Length; i += 3)
            Assert.True(indices[i] != indices[i + 1] && indices[i + 1] != indices[i + 2] && indices[i] != indices[i + 2]);
    }

    private static float Extent(System.Collections.Generic.IEnumerable<float> values)
    {
        return values.Max() - values.Min();
    }
}
