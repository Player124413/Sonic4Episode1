using System.IO;

namespace Sonic4Episode1.Tests;

public class StdShaderSamplerTests
{
    private static string Fixture(string name)
    {
        return Path.Combine(Path.GetDirectoryName(typeof(StdShaderSamplerTests).Assembly.Location), "Fixtures", name);
    }

    private static AppMain.NNS_MATERIAL_STDSHADER_DESC RingMaterial()
    {
        var header = new AppMain.AMS_AMB_HEADER();
        header.data = File.ReadAllBytes(Fixture("RING_MDL_HD.AMB"));

        using (var stream = new MemoryStream(header.data))
            AppMain.readAmbHeader(stream, ref header);

        using var member = new MemoryStream(header.data, header.offsets[0], header.lengths[0]);
        using var reader = new EndianBinaryReader(member, bigEndian: true);

        AppMain.amObjectSetup(out var obj, out _, reader);
        return (AppMain.NNS_MATERIAL_STDSHADER_DESC)obj.pMatPtrList[0].pMaterial;
    }

    // registers fxc actually assigned compiling this material: s_texBase s0, s_texDecal s1, s_texSpecular s2
    [Fact]
    public void RingMapsLandOnTheRegistersSamplerDefAllocates()
    {
        var material = RingMaterial();

        Assert.Equal(3, AppMain.mppStdShaderSamplerCount(material.fTexType));

        Assert.Equal(0, AppMain.mppStdShaderSamplerSlot(
            material.fTexType, AppMain.NND_TEXTURETYPE_STDSHADER_BASE_MAP));
        Assert.Equal(1, AppMain.mppStdShaderSamplerSlot(
            material.fTexType, AppMain.NND_TEXTURETYPE_STDSHADER_DECAL_MAP));
        Assert.Equal(2, AppMain.mppStdShaderSamplerSlot(
            material.fTexType, AppMain.NND_TEXTURETYPE_STDSHADER_SPECULAR_MAP));

        // a map the material does not carry has no register at all
        Assert.Equal(-1, AppMain.mppStdShaderSamplerSlot(
            material.fTexType, AppMain.NND_TEXTURETYPE_STDSHADER_NORMAL_MAP));
    }

    [Fact]
    public void RingReflectionIsSphereMapped()
    {
        var material = RingMaterial();

        var decal = AppMain.mppStdShaderTexMap(material, AppMain.NND_TEXTURETYPE_STDSHADER_DECAL_MAP);
        Assert.NotNull(decal);

        // this is what selects CalcSphereMapTexCoord in the vertex half instead of a vertex UV
        Assert.Equal(AppMain.NNE_TEXCOORDTYPE_STDSHADER_SPHERE_MAP, decal.TexCoord);

        // the other two read vertex UV channel 0, which is why the permutation is MAX_TEXCOORD=1
        var baseMap = AppMain.mppStdShaderTexMap(material, AppMain.NND_TEXTURETYPE_STDSHADER_BASE_MAP);
        var specular = AppMain.mppStdShaderTexMap(material, AppMain.NND_TEXTURETYPE_STDSHADER_SPECULAR_MAP);
        Assert.Equal(AppMain.NNE_TEXCOORDTYPE_STDSHADER_TEXCOORD0, baseMap.TexCoord);
        Assert.Equal(AppMain.NNE_TEXCOORDTYPE_STDSHADER_TEXCOORD0, specular.TexCoord);
    }

    // worth pinning: a sphere map goes through u_TextureMatrix as a float4 and only agrees with a
    // UV map's row 3 offset while the matrix is identity
    [Fact]
    public void RingDeclaresNoUvTransform()
    {
        var material = RingMaterial();

        foreach (var desc in material.pTexDesc)
        {
            Assert.NotEqual(0u, desc.fType & AppMain.NND_TEXTURETYPE_STDSHADER_NO_UV_TRANSFORM);
            Assert.InRange(desc.Blend, 0f, 1f);
        }
    }
}
