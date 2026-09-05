using System;
using System.IO;

namespace Sonic4Episode1.Tests;

// asserted against the built Content.Build tree rather than Sources, since the pipeline repacks every archive
//
// a motion member ending in .amb is itself an archive: ObjDataLoadAmbIndex recurses into it and
// amMotionSetup treats its members as actions (standby/lean-right/lean-left here)
public class DecoZone1ArchiveTests
{
    private const string Deco = "G_ZONE1/DECO/DECO_ZONE1";

    private static AppMain.AMS_AMB_HEADER Open(string tier)
    {
        var here = Path.GetDirectoryName(typeof(DecoZone1ArchiveTests).Assembly.Location);
        var root = Path.GetFullPath(Path.Combine(here, "..", "..", "..", "..", "Content.Build", "desktop-hd-dxt"));
        var path = Path.Combine(root, (Deco + tier + ".AMB").Replace('/', Path.DirectorySeparatorChar));

        if (!File.Exists(path))
            throw Xunit.Sdk.SkipException.ForSkip($"{path} is not present; build the desktop-hd-dxt profile");

        var header = new AppMain.AMS_AMB_HEADER();
        header.data = File.ReadAllBytes(path);

        using var stream = new MemoryStream(header.data);
        AppMain.readAmbHeader(stream, ref header);

        return header;
    }

    private static AppMain.AMS_AMB_HEADER Member(AppMain.AMS_AMB_HEADER header, int index)
    {
        return (AppMain.AMS_AMB_HEADER)AppMain.amBindGet(header, index);
    }

    private static string Name(AppMain.AMS_AMB_HEADER header, int index)
    {
        return Path.GetFileName(header.files[index].Replace('\\', '/'));
    }

    [Fact]
    public void ThePcModelArchivePutsTheSunflowerLast()
    {
        var mdl = Member(Open(".pc"), AppMain.GMD_DECO_DATA_INDEX_AMB_MDL);

        Assert.Equal(14, mdl.file_num);
        Assert.Equal("DECO_Z1_WALLA.ZNO", Name(mdl, 0));
        Assert.Equal("SUNFLOWER.ZNO", Name(mdl, 13));
    }

    [Fact]
    public void TheConsoleModelArchiveStillLeadsWithHimawari()
    {
        var mdl = Member(Open(".console"), AppMain.GMD_DECO_DATA_INDEX_AMB_MDL);

        Assert.Equal(14, mdl.file_num);
        Assert.Equal("DECO_Z1_HIMAWARI.ENO", Name(mdl, 0));
    }

    // member 1 of PC's MTN is the nested archive the sunflower's node motion index points at, and
    // member 0 is the log's, which is the swap gm_deco_z1_motion_pc encodes
    [Fact]
    public void ThePcSunflowerMotionIsANestedArchiveOfThreeActions()
    {
        var mtn = Member(Open(".pc"), AppMain.GMD_DECO_DATA_INDEX_AMB_MTN);

        Assert.Equal(2, mtn.file_num);
        Assert.Equal("DECO_Z1_WOODA.ZNM", Name(mtn, 0));
        Assert.EndsWith(".AMB", Name(mtn, 1), StringComparison.OrdinalIgnoreCase);

        var sunflower = Member(mtn, 1);

        Assert.Equal(3, sunflower.file_num);
        Assert.Equal("SUN_STANDBY.ZNM", Name(sunflower, 0));
        Assert.Equal("SUN_R_INCLINATION.ZNM", Name(sunflower, 1));
        Assert.Equal("SUN_L_INCLINATION.ZNM", Name(sunflower, 2));
    }

    // the order is the action id the touch callback picks: 0 standby, 1 leaning right, 2 leaning
    // left. Reordering the archive silently swaps which way the flower leans
    [Fact]
    public void EveryNestedSunflowerMemberParsesAsAMotion()
    {
        var mtn = Member(Open(".pc"), AppMain.GMD_DECO_DATA_INDEX_AMB_MTN);
        var sunflower = Member(mtn, 1);

        AppMain.amPreLoadAmbItems(sunflower);

        for (int i = 0; i < sunflower.file_num; ++i)
            Assert.IsType<AppMain.NNS_MOTION>(sunflower.buf[i]);

        var motions = new AppMain.NNS_MOTION[sunflower.file_num];
        int count = AppMain.amMotionSetup(new AppMain.ArrayPointer<AppMain.NNS_MOTION>(motions), sunflower);

        Assert.Equal(3, count);
        Assert.All(motions, motion => Assert.NotNull(motion));
    }

    [Fact]
    public void TheConsoleMotionArchiveIsTheFlatPair()
    {
        var mtn = Member(Open(".console"), AppMain.GMD_DECO_DATA_INDEX_AMB_MTN);

        Assert.Equal(2, mtn.file_num);
        Assert.Equal("DECO_Z1_HIMAWARI.ENM", Name(mtn, 0));
        Assert.Equal("DECO_Z1_WOODA.ENM", Name(mtn, 1));
    }
}
