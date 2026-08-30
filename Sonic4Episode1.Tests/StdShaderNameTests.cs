using System.IO;

namespace Sonic4Episode1.Tests;

// The key algorithm was recovered from nnGetStdShaderName in libnndxg20.lib. See
// docs/nn-stdshader-ubershader.md for the table and how it was read.
public class StdShaderNameTests
{
    private static string[] RetailKeys()
    {
        string path = Path.Combine(
            Path.GetDirectoryName(typeof(StdShaderNameTests).Assembly.Location),
            "Fixtures", "nnstdshader-keys.txt");

        return File.ReadAllLines(path);
    }

    // every distinct shader name in the retail NNSTDSHADER/SHADER.AMB
    [Fact]
    public void RoundTripsEveryRetailShaderName()
    {
        var keys = RetailKeys();
        Assert.Equal(285, keys.Length);

        var profile = new AppMain.NNS_STDSHADER_PROFILE();
        foreach (string key in keys)
        {
            var name = AppMain.mppStdShaderNameParse(key);
            AppMain.nnGetStdShaderProfile(profile, name);
            Assert.Equal(key, AppMain.nnGetStdShaderName(profile).ToString());
        }
    }

    // the packer only ever produces 80 bits, nothing above that may be set
    [Fact]
    public void RetailNamesFitTheEightyBitsThePackerProduces()
    {
        foreach (string key in RetailKeys())
        {
            var name = AppMain.mppStdShaderNameParse(key);
            Assert.Equal(0ul, name.high >> 16);
        }
    }

    // Every field decodes inside the range nnlshaderdxg20.h declares for it
    [Fact]
    public void RetailProfilesStayInRange()
    {
        var profile = new AppMain.NNS_STDSHADER_PROFILE();
        foreach (string key in RetailKeys())
        {
            AppMain.nnGetStdShaderProfile(profile, AppMain.mppStdShaderNameParse(key));

            Assert.InRange(profile.VtxTfmMode, 0, 2);
            Assert.InRange(profile.NormalMapType, 0, 2);
            Assert.InRange(profile.nDecalMap, 0, 3);
            Assert.InRange(profile.nShadowMap, 0, 2);
            Assert.InRange(profile.nFragParallelLight, 0, 3);
            Assert.InRange(profile.nFragPointLight, 0, 3);
            Assert.InRange(profile.UserProfile, 0, 63);
            Assert.InRange(profile.UserProfileDrawobj, 0, 255);

            for (int i = 0; i < 8; ++i)
                Assert.InRange(profile.GetTexCoord(i), -3, 3);
        }
    }

    // nothing here consults the decoded key, so agreeing with it is a real result, not a round trip
    [Fact]
    public void RingMaterialProducesTheRetailShaderName()
    {
        var header = new AppMain.AMS_AMB_HEADER();
        header.data = File.ReadAllBytes(Path.Combine(
            Path.GetDirectoryName(typeof(StdShaderNameTests).Assembly.Location),
            "Fixtures", "RING_MDL_HD.AMB"));

        using (var stream = new MemoryStream(header.data))
            AppMain.readAmbHeader(stream, ref header);

        using var member = new MemoryStream(header.data, header.offsets[0], header.lengths[0]);
        using var reader = new EndianBinaryReader(member, bigEndian: true);
        AppMain.amObjectSetup(out var obj, out _, reader);

        var meshset = obj.pSubobjList[0].pMeshsetList[0];
        var vtxList = (AppMain.NNS_VTXLIST_DX_DESC)obj.pVtxListPtrList[meshset.iVtxList].pVtxList;

        var profile = new AppMain.NNS_STDSHADER_PROFILE();
        int result = AppMain.nnSetupShaderProfile(
            profile,
            obj.pMatPtrList[meshset.iMaterial],
            AppMain.NND_DRAWOBJ_FRAGPARALIGHT1,
            0,
            obj.pSubobjList[0].fType,
            vtxList.Format);

        Assert.Equal(0, result);
        Assert.Equal("0000000000000RDMQC0000M1C4", AppMain.nnGetStdShaderName(profile).ToString());
    }

    // confirms the algorithm against a profile read independently out of the model, not just self consistency
    [Fact]
    public void RingShaderNameDecodesToTheRingsMaterial()
    {
        var profile = new AppMain.NNS_STDSHADER_PROFILE();
        AppMain.nnGetStdShaderProfile(profile, AppMain.mppStdShaderNameParse("0000000000000RDMQC0000M1C4"));

        Assert.Equal(1, profile.bBaseMap);
        Assert.Equal(1, profile.nDecalMap);
        Assert.Equal(1, profile.bSpecularMap);

        Assert.Equal(AppMain.NNE_TEXCOORDTYPE_STDSHADER_TEXCOORD0, profile.GetTexCoord(0));
        Assert.Equal(AppMain.NNE_TEXCOORDTYPE_STDSHADER_SPHERE_MAP, profile.GetTexCoord(1));
        Assert.Equal(AppMain.NNE_TEXCOORDTYPE_STDSHADER_TEXCOORD0, profile.GetTexCoord(2));

        Assert.Equal(1, profile.bVtxNrm);
        Assert.Equal(1, profile.bLighting);
        Assert.Equal(1, profile.bSpecular);
        Assert.Equal(1, profile.nFragParallelLight);
        Assert.Equal(0, profile.VtxTfmMode);

        Assert.Equal(0, profile.bShininessMap);
        Assert.Equal(0, profile.bDualParaboloidMap);
        Assert.Equal(0, profile.nShadowMap);
    }
}
