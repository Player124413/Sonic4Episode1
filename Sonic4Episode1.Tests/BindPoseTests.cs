using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;

namespace Sonic4Episode1.Tests;

// a degenerate palette entry reaches the shader as a singular model view, NaN-ing the normal matrix
public class BindPoseTests
{
    private static string Fixture(string name)
    {
        return Path.Combine(Path.GetDirectoryName(typeof(BindPoseTests).Assembly.Location), "Fixtures", name);
    }

    private static AppMain.NNS_OBJECT ReadRing()
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

    private static Matrix[] BuildBindPose(AppMain.NNS_OBJECT obj)
    {
        var palette = new NNS_MATRIX[obj.nNode];
        for (int i = 0; i < palette.Length; ++i)
            palette[i] = NNS_MATRIX.CreateIdentity();

        AppMain.nnCalcMatrixPalette(palette, null, obj, null, new AppMain.NNS_MATRIXSTACK(), 0U);

        var converted = new Matrix[palette.Length];
        for (int i = 0; i < palette.Length; ++i)
            converted[i] = (Matrix)palette[i];

        return converted;
    }

    // regression: nnSinCos reads a table only AppInit used to fill, giving every rotation a zero matrix
    [Fact]
    public void RotationWorksWithoutBootingTheGame()
    {
        var mtx = NNS_MATRIX.CreateIdentity();
        AppMain.nnmRotateMatrixFast(mtx, 16384, 0, 1);

        Assert.NotEqual(0f, ((Matrix)mtx).Determinant());
    }

    [Fact]
    public void EveryNodeCarriesAnInvertibleInitialMatrix()
    {
        var obj = ReadRing();

        for (int i = 0; i < obj.nNode; ++i)
        {
            var node = obj.pNodeList[i];
            if (node.iMatrix == -1)
                continue;

            Matrix inv = (Matrix)node.InvInitMtx;
            Assert.True(
                inv.Determinant() != 0f,
                $"node {i} (iMatrix {node.iMatrix}) has a singular InvInitMtx, so the palette entry "
                + $"it writes would be zero: {inv}");
        }
    }

    [Fact]
    public void BindPoseIsUsableForEveryMeshset()
    {
        var obj = ReadRing();
        var palette = BuildBindPose(obj);

        var checked_ = new List<int>();
        for (int s = 0; s < obj.nSubobj; ++s)
        {
            var subobj = obj.pSubobjList[s];
            for (int m = 0; m < subobj.nMeshset; ++m)
            {
                int index = subobj.pMeshsetList[m].iMatrix;
                if (index == -1)
                    continue;

                Assert.InRange(index, 0, palette.Length - 1);
                Assert.True(
                    palette[index].Determinant() != 0f,
                    $"subobj {s} meshset {m} takes palette entry {index}, which is singular: {palette[index]}");

                checked_.Add(index);
            }
        }

        Assert.NotEmpty(checked_);
    }
}
