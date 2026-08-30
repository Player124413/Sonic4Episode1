using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sonic4Episode1.Tests;

// the round trip invariant is member level, not byte level: legacy archives are upgraded to #AMB
// on the way out, so offsets never come back identical
public class AmbRoundTripTests
{
    private static string Fixture(string name)
    {
        return Path.Combine(Path.GetDirectoryName(typeof(AmbRoundTripTests).Assembly.Location), "Fixtures", name);
    }

    private static AppMain.AMS_AMB_HEADER Read(byte[] bytes)
    {
        var header = new AppMain.AMS_AMB_HEADER();
        header.data = bytes;

        using var stream = new MemoryStream(bytes);
        AppMain.readAmbHeader(stream, ref header);

        return header;
    }

    private static AppMain.AMS_AMB_HEADER ReadFixture(string name)
    {
        return Read(File.ReadAllBytes(Fixture(name)));
    }

    private static void AssertSameMembers(AppMain.AMS_AMB_HEADER before, AppMain.AMS_AMB_HEADER after)
    {
        Assert.Equal(before.file_num, after.file_num);

        for (int i = 0; i < before.file_num; i++)
        {
            Assert.Equal(before.files[i], after.files[i]);
            Assert.Equal(before.lengths[i], after.lengths[i]);
            Assert.Equal(before.types[i], after.types[i]);
            Assert.Equal(before.user0[i], after.user0[i]);
            Assert.Equal(before.user1[i], after.user1[i]);

            var lhs = before.data.AsSpan(before.offsets[i], before.lengths[i]);
            var rhs = after.data.AsSpan(after.offsets[i], after.lengths[i]);
            Assert.True(lhs.SequenceEqual(rhs), $"member {i} '{before.files[i]}' differs");
        }
    }

    private static AppMain.AMS_AMB_HEADER RoundTrip(AppMain.AMS_AMB_HEADER source, bool bigEndian)
    {
        var entries = AppMain.readAmbEntries(source);
        return Read(AppMain.writeAmbArchive(entries, bigEndian));
    }

    [Fact]
    public void RoundTripsLittleEndianArchive()
    {
        var before = ReadFixture("GMK_SPRING_MTN.AMB");
        AssertSameMembers(before, RoundTrip(before, bigEndian: false));
    }

    [Fact]
    public void RoundTripsBigEndianArchive()
    {
        var before = ReadFixture("RING_TEX_HD.AMB");
        Assert.True(before.file_num == 3);

        var after = RoundTrip(before, bigEndian: true);
        AssertSameMembers(before, after);

        Assert.True(after.is_real_amb);
    }

    [Fact]
    public void UpgradesLegacyArchiveToAmb()
    {
        var before = ReadFixture("GMK_SPRING_MTN.AMB");
        var packed = AppMain.writeAmbArchive(AppMain.readAmbEntries(before), bigEndian: false);

        Assert.Equal("#AMB", System.Text.Encoding.ASCII.GetString(packed, 0, 4));
        AssertSameMembers(before, Read(packed));
    }

    [Fact]
    public void PreservesTypeAndUserData()
    {
        var entries = new List<AppMain.AmbEntry>
        {
            new AppMain.AmbEntry("FIRST.INO", new byte[] { 1, 2, 3 }) { Type = 11, User0 = 1 },
            new AppMain.AmbEntry("SECOND.DDS", new byte[64]) { Type = 0, User0 = 2, User1 = 7 },
        };

        var after = Read(AppMain.writeAmbArchive(entries, bigEndian: false));

        Assert.Equal(new[] { "FIRST.INO", "SECOND.DDS" }, after.files);
        Assert.Equal(new[] { 11, 0 }, after.types);
        Assert.Equal(new short[] { 1, 2 }, after.user0);
        Assert.Equal(new short[] { 0, 7 }, after.user1);
    }

    [Fact]
    public void AlignsMembersToSixteenBytes()
    {
        var entries = new List<AppMain.AmbEntry>
        {
            new AppMain.AmbEntry("A", new byte[3]),
            new AppMain.AmbEntry("B", new byte[17]),
            new AppMain.AmbEntry("C", new byte[1]),
        };

        var after = Read(AppMain.writeAmbArchive(entries, bigEndian: false));

        foreach (var offset in after.offsets)
            Assert.Equal(0, offset % AppMain.AMB_ALIGN);
    }

    // content trees are gitignored and machine-local, skip rather than fail when absent
    private static string ContentTree(string name)
    {
        var here = Path.GetDirectoryName(typeof(AmbRoundTripTests).Assembly.Location);
        var root = Path.GetFullPath(Path.Combine(here, "..", "..", "..", "..", "Content", "bin", name, "Content"));
        return Directory.Exists(root) ? root : null;
    }

    [Theory]
    [InlineData("DesktopGL")]
    [InlineData("DesktopGLHD")]
    public void RoundTripsEveryArchiveInTheTree(string tree)
    {
        var root = ContentTree(tree);
        if (root == null)
            throw Xunit.Sdk.SkipException.ForSkip($"Content/bin/{tree} is not present");

        var failures = new List<string>();
        var total = 0;

        foreach (var path in Directory.EnumerateFiles(root, "*.AMB", SearchOption.AllDirectories))
        {
            total++;
            try
            {
                var before = Read(File.ReadAllBytes(path));
                AssertSameMembers(before, RoundTrip(before, IsBigEndian(before)));
            }
            catch (Exception ex)
            {
                failures.Add($"{Path.GetRelativePath(root, path)}: {ex.Message}");
            }
        }

        Assert.True(total > 0, $"no archives found under {root}");
        Assert.True(failures.Count == 0, string.Join("\n", failures.Take(10)));
    }

    private static bool IsBigEndian(AppMain.AMS_AMB_HEADER header)
    {
        return header.is_real_amb
            && header.data.Length >= 8
            && BitConverter.ToInt32(header.data, 4) != AppMain.AMB_HEADER_SIZE;
    }

    // see docs/amb-archives.md for ZONE1_M_M.AMB, which has this exact shape
    [Fact]
    public void ReadsAnEmptyLegacyArchive()
    {
        var empty = new byte[] { 0, 0, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0 };
        var header = Read(empty);

        Assert.False(header.is_real_amb);
        Assert.Equal(0, header.file_num);
        Assert.Empty(header.files);
    }

    [Fact]
    public void WritesAnEmptyArchive()
    {
        var packed = AppMain.writeAmbArchive(new List<AppMain.AmbEntry>(), bigEndian: false);

        Assert.Equal("#AMB", System.Text.Encoding.ASCII.GetString(packed, 0, 4));
        Assert.Equal(0, Read(packed).file_num);
    }

    [Fact]
    public void RefusesAnOverlongName()
    {
        var entries = new List<AppMain.AmbEntry>
        {
            new AppMain.AmbEntry(new string('X', AppMain.AMB_NAME_SIZE), new byte[4]),
        };

        Assert.Throws<InvalidDataException>(() => AppMain.writeAmbArchive(entries, bigEndian: false));
    }
}
