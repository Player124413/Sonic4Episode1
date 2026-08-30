using System.IO;

namespace Sonic4Episode1.Tests;

public class DxObjectParseTests
{
    private static string Fixture(string name)
    {
        return Path.Combine(Path.GetDirectoryName(typeof(DxObjectParseTests).Assembly.Location), "Fixtures", name);
    }

    private static void ReadFixture(string name, out AppMain.NNS_OBJECT obj, out AppMain.NNS_TEXFILELIST texfilelist)
    {
        using var stream = File.OpenRead(Fixture(name));
        using var reader = new BinaryReader(stream);

        AppMain.amObjectSetup(out obj, out texfilelist, reader);
    }

    [Fact]
    public void ParsesUntexturedPlane()
    {
        ReadFixture("NN_sample_Plane01.zno", out var obj, out var texfilelist);

        Assert.NotNull(obj);
        Assert.Null(texfilelist);
        Assert.Equal(1, obj.nVtxList);
        Assert.Equal(1, obj.nPrimList);

        var vtx = Assert.IsType<AppMain.NNS_VTXLIST_DX_DESC>(obj.pVtxListPtrList[0].pVtxList);
        Assert.Equal(AppMain.NND_VTXMEMBER_DX_POS_F32 | AppMain.NND_VTXMEMBER_DX_NRM_F32, vtx.Format);
        Assert.Equal(0x12u, vtx.VertexFVF);
        Assert.Equal(24u, vtx.Stride);
        Assert.Equal(25u, vtx.nVertex);
        Assert.Equal(0, vtx.nBoneMtx);
        Assert.Equal((int)(vtx.Stride * vtx.nVertex), vtx.pList.Data.Length);

        var prim = Assert.IsType<AppMain.NNS_PRIMTYPE_DX_STRIPLIST>(obj.pPrimListPtrList[0].pPrimList);
        Assert.Equal(0x4810u, prim.Format);
        Assert.Equal(46u, prim.nIndex);
        Assert.Equal(1u, prim.nPrimitive);
        Assert.Equal(46u, prim.pLengthList[0]);
        Assert.Equal(46, prim.pList.Length);
    }

    [Fact]
    public void ParsesTexturedBox()
    {
        ReadFixture("cube_sample_Box01.zno", out var obj, out var texfilelist);

        Assert.NotNull(obj);
        Assert.NotNull(texfilelist);
        Assert.True(texfilelist.nTex > 0);

        var vtx = Assert.IsType<AppMain.NNS_VTXLIST_DX_DESC>(obj.pVtxListPtrList[0].pVtxList);
        Assert.NotEqual(0u, vtx.Format & AppMain.NND_VTXMEMBER_DX_POS_F32);
        Assert.Equal((int)(vtx.Stride * vtx.nVertex), vtx.pList.Data.Length);
    }

    // Format and VertexFVF are independent encodings of the same vertex layout, so a stride they
    // both agree on is strong evidence the descriptor was read at the right offset.
    [Theory]
    [InlineData("NN_sample_Plane01.zno")]
    [InlineData("cube_sample_Box01.zno")]
    public void FormatAndFvfAgreeOnStride(string name)
    {
        ReadFixture(name, out var obj, out _);

        for (int i = 0; i < obj.nVtxList; ++i)
        {
            var vtx = Assert.IsType<AppMain.NNS_VTXLIST_DX_DESC>(obj.pVtxListPtrList[i].pVtxList);
            Assert.Equal(StrideFromFormat(vtx.Format), (int)vtx.Stride);
            Assert.Equal(StrideFromFvf(vtx.VertexFVF), (int)vtx.Stride);
        }
    }

    private static int StrideFromFormat(uint format)
    {
        int stride = 0;

        if ((format & AppMain.NND_VTXMEMBER_DX_POS_F32) != 0u)
            stride += 12;
        if ((format & AppMain.NND_VTXMEMBER_DX_NRM_F32) != 0u)
            stride += 12;
        if ((format & AppMain.NND_VTXMEMBER_DX_COL_U32) != 0u)
            stride += 4;
        if ((format & AppMain.NND_VTXMEMBER_DX_COL2_U32) != 0u)
            stride += 4;
        if ((format & AppMain.NND_VTXMEMBER_DX_BLENDFMT_I8) != 0u)
            stride += 4;

        int bones = (int)((format & AppMain.NND_VTXMEMBER_DX_BLEND_MASK) >> AppMain.NND_VTXMEMBER_DX_BLEND_SHIFT);
        if (bones == 7)
            stride += 12;

        int texCount = (int)((format & AppMain.NND_VTXMEMBER_DX_TEXNUM_MASK) >> AppMain.NND_VTXMEMBER_DX_TEXNUM_SHIFT);
        stride += texCount * 8;

        return stride;
    }

    private static int StrideFromFvf(uint fvf)
    {
        const uint D3DFVF_POSITION_MASK = 0x400E;
        const uint D3DFVF_XYZ = 0x002;
        const uint D3DFVF_XYZB3 = 0x00A;
        const uint D3DFVF_NORMAL = 0x010;
        const uint D3DFVF_DIFFUSE = 0x040;
        const uint D3DFVF_SPECULAR = 0x080;
        const uint D3DFVF_TEXCOUNT_MASK = 0xF00;
        const int D3DFVF_TEXCOUNT_SHIFT = 8;
        const uint D3DFVF_LASTBETA_UBYTE4 = 0x1000;

        int stride = 0;

        switch (fvf & D3DFVF_POSITION_MASK)
        {
            case D3DFVF_XYZ:
                stride += 12;
                break;
            case D3DFVF_XYZB3:
                stride += 12 + 12;
                break;
            default:
                Assert.Fail($"unhandled FVF position type 0x{fvf & D3DFVF_POSITION_MASK:X}");
                break;
        }

        if ((fvf & D3DFVF_LASTBETA_UBYTE4) != 0u)
            stride += 4 - 4;

        if ((fvf & D3DFVF_NORMAL) != 0u)
            stride += 12;
        if ((fvf & D3DFVF_DIFFUSE) != 0u)
            stride += 4;
        if ((fvf & D3DFVF_SPECULAR) != 0u)
            stride += 4;

        stride += (int)((fvf & D3DFVF_TEXCOUNT_MASK) >> D3DFVF_TEXCOUNT_SHIFT) * 8;

        return stride;
    }
}
