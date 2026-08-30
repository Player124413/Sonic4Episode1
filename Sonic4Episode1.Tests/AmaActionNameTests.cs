using System;
using System.IO;
using System.Linq;

namespace Sonic4Episode1.Tests;

public class AmaActionNameTests
{
    private static AppMain.A2S_AMA_HEADER Read(string fixture)
    {
        var path = Path.Combine(
            Path.GetDirectoryName(typeof(AmaActionNameTests).Assembly.Location), "Fixtures", fixture);

        using var stream = File.OpenRead(path);
        using var reader = new BinaryReader(stream);
        return AppMain.readAMAFile(reader);
    }

    [Fact]
    public void ReadsMobileActionNames()
    {
        var ama = Read("D_TITLE_US_MOBILE.AMA");

        Assert.Equal(10u, ama.act_num);
        Assert.Equal(
            new[]
            {
                "tex_hazime", "tex_tuzuki", "tex_option", "tex_kanzenban", "tex_win_msg1",
                "tex_win_msg2", "tex_start", "tex_modoru", "tex_game", "tex_e",
            },
            ama.act_names);
    }

    [Fact]
    public void ReadsConsoleActionNames()
    {
        var ama = Read("D_TITLE_US_PC.AMA");

        Assert.Equal(18u, ama.act_num);
        Assert.Equal(
            new[]
            {
                "tex_hazime", "tex_hazime2", "tex_tuzuki", "tex_tuzuki2", "tex_option",
                "tex_option2", "tex_ranking", "tex_ranking2", "tex_zisseki", "tex_zisseki2",
                "tex_library", "tex_library2", "tex_kanzenban",
            },
            ama.act_names.Take(13));
    }

    [Theory]
    [InlineData("tex_hazime", 0, 0)]
    [InlineData("tex_tuzuki", 1, 2)]
    [InlineData("tex_option", 2, 4)]
    [InlineData("tex_kanzenban", 3, 12)]
    public void SameNameResolvesToDifferentIndexPerVariant(string name, int mobile, int console)
    {
        Assert.Equal(mobile, Read("D_TITLE_US_MOBILE.AMA").FindAct(name));
        Assert.Equal(console, Read("D_TITLE_US_PC.AMA").FindAct(name));
    }

    [Fact]
    public void ReportsAbsenceRatherThanGuessing()
    {
        // -1 here, not 0: DmTitle.cs currently maps this miss to 0, a real action, not a sentinel
        Assert.Equal(-1, Read("D_TITLE_US_MOBILE.AMA").FindAct("tex_library"));
        Assert.True(Read("D_TITLE_US_PC.AMA").FindAct("tex_library") > 0);
    }

    [Fact]
    public void HasNoNodeNamesInEitherVariant()
    {
        foreach (var fixture in new[] { "D_TITLE_US_MOBILE.AMA", "D_TITLE_US_PC.AMA" })
        {
            var ama = Read(fixture);
            Assert.Equal(0u, ama.node_num);
            Assert.Null(ama.node_names);
            Assert.Equal(-1, ama.FindNode("anything"));
        }
    }

    // D_LOAD.AMA out of DEMO/LOADING/D_LOADING.AMB in the iPad HD bundle and the xbla beta. Same
    // file, same length, opposite byte order, so they pin the detection against each other.
    //
    // These live in this class rather than their own because readAMAFile's node and act caches are
    // process-wide statics, cleared at the end of each parse. Sequential parses are fine; two test
    // classes parsing at once are not, and xunit parallelises across classes.
    private const string LittleEndian = "D_LOAD_MOBILE.AMA";
    private const string BigEndian = "D_LOAD_XBLA.AMA";

    private static readonly string[] LoadActNames =
    {
        "bg_white", "bg_bottom", "obi", "run", "stop", "period1", "period2", "period3", "text_load",
    };

    private static AppMain.A2S_AMA_HEADER ReadDetecting(string fixture, bool claimedBigEndian)
    {
        var path = Path.Combine(
            Path.GetDirectoryName(typeof(AmaActionNameTests).Assembly.Location), "Fixtures", fixture);

        var bytes = File.ReadAllBytes(path);
        return AppMain.readAMAFile(bytes, 0, bytes.Length, claimedBigEndian);
    }

    // the container is asked last, so a member that outlived its archive's byte order still reads.
    // a per-member console variant splices an xbla .AMA into a little-endian archive
    [Theory]
    [InlineData(LittleEndian, false)]
    [InlineData(LittleEndian, true)]
    [InlineData(BigEndian, false)]
    [InlineData(BigEndian, true)]
    public void ByteOrderComesFromTheFileNotTheContainer(string fixture, bool claimedBigEndian)
    {
        var ama = ReadDetecting(fixture, claimedBigEndian);

        Assert.Equal(9u, ama.act_num);
        Assert.Equal(LoadActNames, ama.act_names);
    }

    [Fact]
    public void TheTwoByteOrdersOfOneFileAgree()
    {
        var little = ReadDetecting(LittleEndian, claimedBigEndian: false);
        var big = ReadDetecting(BigEndian, claimedBigEndian: false);

        Assert.Equal(little.act_num, big.act_num);
        Assert.Equal(little.node_num, big.node_num);
        Assert.Equal(little.act_tbl_offset, big.act_tbl_offset);
        Assert.Equal(little.act_name_tbl_offset, big.act_name_tbl_offset);
        Assert.Equal(little.act_names, big.act_names);
    }

    // reading a big-endian header the wrong way round gives act_num 0x09000000, which sized act_tbl
    // straight to roughly a gigabyte. see docs/amb-archives.md
    [Fact]
    public void DoesNotSizeTablesFromAMisreadHeader()
    {
        Assert.Equal(9, ReadDetecting(BigEndian, claimedBigEndian: false).act_tbl.Length);
    }
}
