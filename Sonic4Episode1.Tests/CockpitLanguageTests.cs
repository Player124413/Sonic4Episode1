using System.Linq;
using System.Reflection;

namespace Sonic4Episode1.Tests;

// FI/PT/RU/CN/HK are WP7 additions. Their CPIT_MAIN members sit at 58-97, which only the 98-member
// wp7-orig archive has; the iOS and xbla copies stop at 57. ObjDataLoadAmbIndex reads amb.buf[index]
// unguarded, so an unfallen-back row is a crash at stage build, not a missing sprite.
public class CockpitLanguageTests
{
    private const int Mobile = 58;
    private const int Wp7 = 98;

    private const int JP = 0, US = 1, GE = 4, SP = 5, FI = 6, PT = 7, RU = 8, CN = 9, HK = 10;

    private static int[] Table(string name)
    {
        var field = typeof(AppMain).GetField(name, BindingFlags.NonPublic | BindingFlags.Static)
            ?? typeof(AppMain).GetField(name, BindingFlags.Public | BindingFlags.Static);

        Assert.True(field != null, $"AppMain.{name} not found");

        return field.GetValue(null) switch
        {
            int[] flat => flat,
            int[][] jagged => jagged.Select(row => row[row.Length - 1]).ToArray(),
            _ => null,
        };
    }

    // every table that indexes CPIT_MAIN by language, and the slot that can run past the end
    public static TheoryData<string> CockpitTables => new TheoryData<string>
    {
        "gm_fix_tex_amb_idx_tbl",
        "gm_fix_ama_amb_idx_tbl",
        "gm_over_tex_amb_idx_tbl",
        "gm_over_ama_amb_idx_tbl",
        "g_gm_clear_demo_data_ama_id",
        "g_gm_clear_demo_data_amb_id",
    };

    [Theory]
    [MemberData(nameof(CockpitTables))]
    public void TheFirstSixLanguagesFitEveryBuild(string name)
    {
        var members = Table(name);
        Assert.NotNull(members);
        Assert.Equal(11, members.Length);

        for (int lang = JP; lang <= SP; lang++)
            Assert.True(members[lang] < Mobile, $"{name}[{lang}] is {members[lang]}");
    }

    [Theory]
    [MemberData(nameof(CockpitTables))]
    public void TheWp7FiveDoNot(string name)
    {
        var members = Table(name);

        foreach (var lang in new[] { FI, PT, RU, CN, HK })
            Assert.True(members[lang] >= Mobile, $"{name}[{lang}] is {members[lang]}");
    }

    [Theory]
    [MemberData(nameof(CockpitTables))]
    public void TheWp7FiveFallBackOnAMobileArchive(string name)
    {
        var members = Table(name);

        foreach (var lang in new[] { FI, PT, RU, CN, HK })
            Assert.Equal(US, AppMain.GmGameDatCockpitLanguageFor(lang, members[lang], Mobile));
    }

    [Theory]
    [MemberData(nameof(CockpitTables))]
    public void NothingFallsBackOnTheWp7Archive(string name)
    {
        var members = Table(name);

        for (int lang = 0; lang < members.Length; lang++)
            Assert.Equal(lang, AppMain.GmGameDatCockpitLanguageFor(lang, members[lang], Wp7));
    }

    [Fact]
    public void TheBoundaryIsTheMemberCountItself()
    {
        Assert.Equal(US, AppMain.GmGameDatCockpitLanguageFor(RU, Mobile, Mobile));
        Assert.Equal(RU, AppMain.GmGameDatCockpitLanguageFor(RU, Mobile - 1, Mobile));
    }

    // no archive loaded yet is not the same as a short one, and guessing US would be wrong
    [Fact]
    public void AnUnloadedArchiveChangesNothing()
    {
        Assert.Equal(RU, AppMain.GmGameDatCockpitLanguageFor(RU, 93, 0));
    }
}
