using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sonic4Episode1.Tests;

// RING_MDL_HD.AMB and RING_TEX_HD.AMB are big-endian Xbox 360 fixtures, unreferenced by any code
public class BigEndianAmbTests
{
    private static string Fixture(string name)
    {
        return Path.Combine(Path.GetDirectoryName(typeof(BigEndianAmbTests).Assembly.Location), "Fixtures", name);
    }

    private static AppMain.AMS_AMB_HEADER ReadArchive(string name)
    {
        var header = new AppMain.AMS_AMB_HEADER();
        header.data = File.ReadAllBytes(Fixture(name));

        using var stream = new MemoryStream(header.data);
        AppMain.readAmbHeader(stream, ref header);

        return header;
    }

    // nothing inside an .AMA says which byte order it is, so the container has to carry it through
    // to readAMAFile. xbla menu archives are unreadable without this.
    [Fact]
    public void RecordsEndiannessOnTheHeader()
    {
        Assert.True(ReadArchive("RING_MDL_HD.AMB").big_endian);
        Assert.False(ReadArchive("GMK_SPRING_MTN.AMB").big_endian);
    }

    // SEGA declares the byte order in flag[0], which is why it is a byte array: amBindConv's swap
    // list covers every scalar in the header except that one
    [Fact]
    public void KeepsTheDeclaredEndianFlag()
    {
        var big = ReadArchive("RING_MDL_HD.AMB");
        Assert.Equal(AppMain.AMD_AMB_FLAG0_ENDIAN_BIG, (byte)big.flag[0] & AppMain.AMD_AMB_FLAG0_ENDIAN_BIG);

        var little = ReadArchive("RING_TEX_HD.AMB");
        Assert.Equal(4, little.flag.Length);
    }

    // header size and flag[0] are independent statements of the same fact, so a disagreement means
    // the header is misparsed rather than merely mislabelled
    [Fact]
    public void RefusesAHeaderWhoseFlagDisagreesWithItsSize()
    {
        var bytes = File.ReadAllBytes(Fixture("RING_MDL_HD.AMB"));
        bytes[0x0C] = 0;

        var header = new AppMain.AMS_AMB_HEADER { data = bytes };
        using var stream = new MemoryStream(bytes);

        Assert.Throws<InvalidDataException>(() => AppMain.readAmbHeader(stream, ref header));
    }

    // amBindConv stamps file_id[0] with '!' once it has converted in place, and asserts only on
    // file_id[1..3] == "AMB"
    [Fact]
    public void AcceptsAConvertedMark()
    {
        var bytes = File.ReadAllBytes(Fixture("RING_MDL_HD.AMB"));
        bytes[0] = (byte)'!';

        var header = new AppMain.AMS_AMB_HEADER { data = bytes };
        using var stream = new MemoryStream(bytes);
        AppMain.readAmbHeader(stream, ref header);

        Assert.True(header.is_real_amb);
        Assert.True(header.big_endian);
        Assert.Equal(1, header.file_num);
    }

    [Fact]
    public void DetectsBigEndianArchive()
    {
        var header = ReadArchive("RING_MDL_HD.AMB");

        Assert.True(header.is_real_amb);
        Assert.Equal(1, header.file_num);
        Assert.Equal(".\\RING.ENO", header.files[0]);
        Assert.Equal(0x40, header.offsets[0]);
        Assert.True(header.lengths[0] > 0);
        Assert.True(header.offsets[0] + header.lengths[0] <= header.data.Length);
    }

    [Fact]
    public void ReadsBigEndianTextureArchive()
    {
        var header = ReadArchive("RING_TEX_HD.AMB");

        Assert.True(header.is_real_amb);
        Assert.Equal(3, header.file_num);

        for (int i = 0; i < header.file_num; ++i)
        {
            Assert.True(header.offsets[i] + header.lengths[i] <= header.data.Length);
            Assert.Equal("DDS ", System.Text.Encoding.ASCII.GetString(header.data, header.offsets[i], 4));
        }
    }

    [Fact]
    public void ParsesBigEndianObjectChunk()
    {
        var header = ReadArchive("RING_MDL_HD.AMB");

        using var stream = new MemoryStream(header.data, header.offsets[0], header.lengths[0]);
        using var reader = new EndianBinaryReader(stream, bigEndian: true);

        AppMain.amObjectSetup(out var obj, out var texfilelist, reader);

        Assert.NotNull(obj);
        Assert.NotNull(texfilelist);
        Assert.True(obj.nVtxList > 0);
        Assert.True(obj.nPrimList > 0);

        var vtx = Assert.IsType<AppMain.NNS_VTXLIST_DX_DESC>(obj.pVtxListPtrList[0].pVtxList);
        Assert.NotEqual(0u, vtx.Format & AppMain.NND_VTXMEMBER_DX_POS_F32);
        Assert.Equal((int)(vtx.Stride * vtx.nVertex), vtx.pList.Data.Length);

        var prim = Assert.IsType<AppMain.NNS_PRIMTYPE_DX_STRIPLIST>(obj.pPrimListPtrList[0].pPrimList);
        Assert.True(prim.nIndex > 0);
        Assert.Equal((int)prim.nIndex, prim.pList.Length);
    }

    // every value in pLogic is a D3D9 enum, so a wrong offset shows up as an out-of-range value here
    [Fact]
    public void RingMaterialsCarryReadableStdShaderLogic()
    {
        var header = ReadArchive("RING_MDL_HD.AMB");

        using var stream = new MemoryStream(header.data, header.offsets[0], header.lengths[0]);
        using var reader = new EndianBinaryReader(stream, bigEndian: true);

        AppMain.amObjectSetup(out var obj, out _, reader);

        int withLogic = 0;
        for (int i = 0; i < obj.nMaterial; ++i)
        {
            var material = Assert.IsType<AppMain.NNS_MATERIAL_STDSHADER_DESC>(obj.pMatPtrList[i].pMaterial);
            if (material.pLogic == null)
                continue;

            var logic = material.pLogic;

            // only the five flags nnfmaterialdxg20.h:293-297 defines; a shifted read shows up here
            Assert.InRange(logic.fFlag, 0u, 0x1Fu);

            // LogicOp is never read on DXG20, so a correct offset always finds the exporter's default
            Assert.Equal(0x1505, logic.LogicOp);
            Assert.Equal(0u, logic.BlendColor);

            if ((logic.fFlag & 1) != 0)
            {
                Assert.InRange(logic.SrcFactorRGB, 1, 19);
                Assert.InRange(logic.DstFactorRGB, 1, 19);
            }

            if ((logic.fFlag & 8) != 0)
            {
                Assert.InRange(logic.AlphaFunc, 1, 8);
                Assert.InRange(logic.AlphaRef, 0u, 255u);
            }

            if ((logic.fFlag & 0x10) != 0)
                Assert.InRange(logic.DepthFunc, 1, 8);

            ++withLogic;
        }

        Assert.True(withLogic > 0, "no material carried a logic block");
    }

    // every material has to name a permutation Build-Permutations.ps1 actually produced
    [Fact]
    public void RingMaterialsSelectBuiltShaderPermutations()
    {
        var header = ReadArchive("RING_MDL_HD.AMB");

        using var stream = new MemoryStream(header.data, header.offsets[0], header.lengths[0]);
        using var reader = new EndianBinaryReader(stream, bigEndian: true);

        AppMain.amObjectSetup(out var obj, out _, reader);

        var built = new HashSet<string>(File.ReadAllLines(Fixture("nnstdshader-keys.txt"))
            .Select(line => line.Trim())
            .Where(line => line.Length > 0));

        Assert.NotEmpty(built);

        int checkedMeshsets = 0;
        for (int i = 0; i < obj.nSubobj; ++i)
        {
            var subobj = obj.pSubobjList[i];
            for (int j = 0; j < subobj.nMeshset; ++j)
            {
                var meshset = subobj.pMeshsetList[j];
                var vtx = Assert.IsType<AppMain.NNS_VTXLIST_DX_DESC>(
                    obj.pVtxListPtrList[meshset.iVtxList].pVtxList);

                var profile = new AppMain.NNS_STDSHADER_PROFILE();
                int result = AppMain.nnSetupShaderProfile(profile, obj.pMatPtrList[meshset.iMaterial],
                    AppMain.NND_DRAWOBJ_FRAGPARALIGHT1, 0U, subobj.fType, vtx.Format);

                Assert.Equal(0, result);
                Assert.Contains(AppMain.nnGetStdShaderName(profile).ToString(), built);
                ++checkedMeshsets;
            }
        }

        Assert.True(checkedMeshsets > 0);
    }
}
