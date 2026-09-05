using System;
using System.IO;

namespace Sonic4Episode1.Tests;

public class XenonRenderStateTests
{
    private static string Fixture(string name)
    {
        return Path.Combine(Path.GetDirectoryName(typeof(XenonRenderStateTests).Assembly.Location), "Fixtures", name);
    }

    [Theory]
    [InlineData(0, 1)]  // ZERO
    [InlineData(1, 2)]  // ONE
    [InlineData(4, 3)]  // SRCCOLOR
    [InlineData(6, 5)]  // SRCALPHA
    [InlineData(7, 6)]  // INVSRCALPHA
    public void BlendFactorsTranslate(int xenon, int d3d9)
    {
        Assert.Equal(d3d9, AppMain.mppXenonBlend((ushort)xenon));
    }

    [Fact]
    public void BlendOpTranslates()
    {
        Assert.Equal(1, AppMain.mppXenonBlendOp(0));
    }

    // dense NEVER..ALWAYS on both platforms, so the whole range is the same shift
    [Theory]
    [InlineData(0, 1)]
    [InlineData(3, 4)]
    [InlineData(4, 5)]
    [InlineData(6, 7)]
    [InlineData(7, 8)]
    public void CompareFunctionsTranslate(int xenon, int d3d9)
    {
        Assert.Equal(d3d9, AppMain.mppXenonCmpFunc((ushort)xenon));
    }

    [Theory]
    [InlineData(0, 1)]  // WRAP
    [InlineData(1, 2)]  // MIRROR
    [InlineData(2, 3)]  // CLAMP
    public void TextureAddressModesTranslate(int xenon, int d3d9)
    {
        Assert.Equal(d3d9, AppMain.mppXenonTextureAddress((ushort)xenon));
    }

    // the corpus pins five of seventeen D3DBLEND values and one of five D3DBLENDOP values. The rest
    // are unknown rather than absent, so they throw instead of translating to something plausible.
    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(8)]
    public void AnUnmeasuredBlendFactorThrows(int xenon)
    {
        Assert.Throws<NotSupportedException>(() => AppMain.mppXenonBlend((ushort)xenon));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(4)]
    public void AnUnmeasuredBlendOpThrows(int xenon)
    {
        Assert.Throws<NotSupportedException>(() => AppMain.mppXenonBlendOp((ushort)xenon));
    }

    [Theory]
    [InlineData(8)]
    [InlineData(255)]
    public void AnUnmeasuredCompareFunctionThrows(int xenon)
    {
        Assert.Throws<NotSupportedException>(() => AppMain.mppXenonCmpFunc((ushort)xenon));
    }

    [Fact]
    public void TheStagedXenonRingReadsAsItsSteamTwin()
    {
        var logic = RingLogic();

        Assert.Equal(0x19u, logic.fFlag);
        Assert.Equal(5, logic.SrcFactorRGB);   // SRCALPHA
        Assert.Equal(6, logic.DstFactorRGB);   // INVSRCALPHA
        Assert.Equal(5, logic.SrcFactorA);
        Assert.Equal(6, logic.DstFactorA);
        Assert.Equal(1, logic.BlendOp);        // ADD
        Assert.Equal(5, logic.AlphaFunc);      // GREATER
        Assert.Equal(4, logic.DepthFunc);      // LESSEQUAL

        // the two anchors, identical on both platforms and untouched by the translation
        Assert.Equal(0x1505, logic.LogicOp);
        Assert.Equal(0u, logic.AlphaRef);
        Assert.Equal(0u, logic.BlendColor);
    }

    private static AppMain.NNS_MATERIAL_STDSHADER_LOGIC RingLogic()
    {
        var header = new AppMain.AMS_AMB_HEADER();
        header.data = File.ReadAllBytes(Fixture("RING_MDL_HD.AMB"));

        using (var stream = new MemoryStream(header.data))
            AppMain.readAmbHeader(stream, ref header);

        using var member = new MemoryStream(header.data, header.offsets[0], header.lengths[0]);
        using var reader = new EndianBinaryReader(member, bigEndian: true);

        AppMain.amObjectSetup(out var obj, out _, reader);

        var material = Assert.IsType<AppMain.NNS_MATERIAL_STDSHADER_DESC>(obj.pMatPtrList[0].pMaterial);
        Assert.NotNull(material.pLogic);

        return material.pLogic;
    }

    // WrapS and WrapT go the same way, and nothing downstream should still be looking at raw Xenon
    [Fact]
    public void TheStagedXenonRingHasD3D9WrapModes()
    {
        var header = new AppMain.AMS_AMB_HEADER();
        header.data = File.ReadAllBytes(Fixture("RING_MDL_HD.AMB"));

        using (var stream = new MemoryStream(header.data))
            AppMain.readAmbHeader(stream, ref header);

        using var member = new MemoryStream(header.data, header.offsets[0], header.lengths[0]);
        using var reader = new EndianBinaryReader(member, bigEndian: true);

        AppMain.amObjectSetup(out var obj, out _, reader);
        var material = (AppMain.NNS_MATERIAL_STDSHADER_DESC)obj.pMatPtrList[0].pMaterial;

        foreach (var tex in material.pTexDesc)
        {
            Assert.InRange(tex.WrapS, 1, 3);
            Assert.InRange(tex.WrapT, 1, 3);
        }
    }
}
