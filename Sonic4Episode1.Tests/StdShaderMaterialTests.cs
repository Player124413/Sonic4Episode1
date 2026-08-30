using System.IO;

namespace Sonic4Episode1.Tests;

public class StdShaderMaterialTests
{
    private static string Fixture(string name)
    {
        return Path.Combine(Path.GetDirectoryName(typeof(StdShaderMaterialTests).Assembly.Location), "Fixtures", name);
    }

    [Fact]
    public void ReadsRingMaterialAndResolvesItsTexture()
    {
        var header = new AppMain.AMS_AMB_HEADER();
        header.data = File.ReadAllBytes(Fixture("RING_MDL_HD.AMB"));

        using (var stream = new MemoryStream(header.data))
            AppMain.readAmbHeader(stream, ref header);

        using var member = new MemoryStream(header.data, header.offsets[0], header.lengths[0]);
        using var reader = new EndianBinaryReader(member, bigEndian: true);

        AppMain.amObjectSetup(out var obj, out var texfilelist, reader);

        Assert.Equal(1, obj.nMaterial);
        var material = Assert.IsType<AppMain.NNS_MATERIAL_STDSHADER_DESC>(obj.pMatPtrList[0].pMaterial);

        // NND_MATTYPE_STDSHADERDESC lives in the top nibble on DXG20, unlike GLES11's low bitflags
        Assert.Equal(AppMain.NND_MATTYPE_STDSHADERDESC,
            obj.pMatPtrList[0].fType & AppMain.NND_MATTYPE_MATTYPE_MASK);

        Assert.True(material.nTex > 0);
        Assert.NotNull(material.pTexDesc);
        Assert.Equal(material.nTex, material.pTexDesc.Length);

        foreach (var tex in material.pTexDesc)
            Assert.InRange(tex.iTexIdx, 0, texfilelist.nTex - 1);

        // the ring is a metal material: diffuse, specular and a sky reflection map
        var named = new System.Collections.Generic.List<string>();
        for (int i = 0; i < texfilelist.nTex; ++i)
            named.Add(Path.GetFileName(texfilelist.pTexFileList[i].Filename));

        Assert.Contains("CMN_METAL_MS_RING_DIF.DDS", named);
        Assert.Contains("CMN_METAL_MS_RING_SPC.DDS", named);
        Assert.Contains("CMN_METAL_MS_RINGSKY_REF.DDS", named);

        // the viewer matches a material to a texture archive member the same way
        Assert.Contains(Path.GetFileName(texfilelist.pTexFileList[material.pTexDesc[0].iTexIdx].Filename), named);

        Assert.NotNull(material.pColor);
        foreach (var channel in new[]
                 {
                     material.pColor.Diffuse.r, material.pColor.Diffuse.g, material.pColor.Diffuse.b,
                     material.pColor.Diffuse.a, material.pColor.Ambient.r, material.pColor.Ambient.a,
                 })
        {
            Assert.InRange(channel, 0f, 1f);
        }
    }
}
