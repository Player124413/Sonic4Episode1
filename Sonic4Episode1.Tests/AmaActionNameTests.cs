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
}
