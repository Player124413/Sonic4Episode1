using System.IO;

namespace Sonic4Episode1.Tests;

// 61 members in the shipped content are named ".\NAME.EXT". Path.GetFileName only splits on '\'
// when it is running on Windows, so the lookups in amTextureLoad and amBindSearchBaseName used to
// miss every one of them off Windows and fall through to the magenta placeholder.
public class AmbMemberNameTests
{
    [Theory]
    [InlineData(".\\CMN_METAL_MS_RING_DIF.DDS", "CMN_METAL_MS_RING_DIF.DDS")]
    [InlineData("./CMN_METAL_MS_RING_DIF.DDS", "CMN_METAL_MS_RING_DIF.DDS")]
    [InlineData("CMN_METAL_MS_RING_DIF.DDS", "CMN_METAL_MS_RING_DIF.DDS")]
    [InlineData("DIR\\SUB\\RING.ENO", "RING.ENO")]
    [InlineData("DIR/SUB/RING.ENO", "RING.ENO")]
    [InlineData("DIR/SUB\\RING.ENO", "RING.ENO")]
    [InlineData("RING", "RING")]
    [InlineData("", "")]
    public void MemberNameStripsEitherSeparator(string member, string expected)
    {
        Assert.Equal(expected, AppMain.mppAmbMemberName(member));
    }

    [Theory]
    [InlineData(".\\CMN_METAL_MS_RING_DIF.DDS", "CMN_METAL_MS_RING_DIF")]
    [InlineData("./CMN_METAL_MS_RING_DIF.DDS", "CMN_METAL_MS_RING_DIF")]
    [InlineData("CMN_METAL_MS_RING_DIF.DDS", "CMN_METAL_MS_RING_DIF")]
    [InlineData(".\\DECO_ZONE1_TEX.AMB", "DECO_ZONE1_TEX")]
    [InlineData("DIR\\SUB\\RING.ENO", "RING")]
    [InlineData("DIR/SUB/RING.ENO", "RING")]
    [InlineData("RING", "RING")]
    public void BaseNameStripsSeparatorAndExtension(string member, string expected)
    {
        Assert.Equal(expected, AppMain.mppAmbMemberBaseName(member));
    }

    [Fact]
    public void NullMemberStaysNull()
    {
        Assert.Null(AppMain.mppAmbMemberName(null));
        Assert.Null(AppMain.mppAmbMemberBaseName(null));
    }

    // the separator half is what Path gets wrong off Windows; everything else has to keep matching
    // it, or a name that works today starts resolving differently
    [Theory]
    [InlineData("CMN_METAL_MS_RING_DIF.DDS")]
    [InlineData("RING")]
    [InlineData("RING.ENO")]
    [InlineData("A.B.C")]
    [InlineData("")]
    public void SeparatorlessNamesAgreeWithPath(string member)
    {
        Assert.Equal(Path.GetFileName(member), AppMain.mppAmbMemberName(member));
        Assert.Equal(Path.GetFileNameWithoutExtension(member), AppMain.mppAmbMemberBaseName(member));
    }
}
