using System.IO;

namespace Sonic4Episode1.Tests;

// The pipeline half of this is checked by `content check`, which asserts the sparse sibling's
// members line up with the base indices the manifest names. This is the runtime half: the manifest
// parse, the screen-level gate, and the splice that puts a sibling member in front of a base one.
[Collection(FsVariantCollection.Name)]
public class MemberVariantTests
{
    private const string Cpit = "G_COM/CPIT/CPIT_MAIN.AMB";

    private static void Load(params string[] lines)
    {
        AppMain.amFsLoadVariants(new StringReader(string.Join("\n", lines)));
    }

    [Fact]
    public void MemberLinesDoNotResolveTheWholeArchive()
    {
        Load(Cpit + "#44 console", Cpit + "#45 console");

        Assert.Null(AppMain.amFsResolvedVariant(Cpit));
        Assert.Equal(Cpit, AppMain.amFsResolveAs(Cpit, AppMain.AMD_FS_VARIANT_CONSOLE));
    }

    [Fact]
    public void ArchiveLinesStillResolveWholesale()
    {
        Load("DEMO/LOADING/D_LOADING.AMB console");

        Assert.Equal(
            "DEMO/LOADING/D_LOADING.console.AMB",
            AppMain.amFsResolveAs("DEMO/LOADING/D_LOADING.AMB", AppMain.AMD_FS_VARIANT_CONSOLE));
    }

    // the splice pairs the sibling's members with these positionally, so the order is load-bearing
    [Fact]
    public void DeclaredIndicesComeBackAscending()
    {
        Load(Cpit + "#57 console", Cpit + "#44 console", Cpit + "#50 console");

        Assert.Equal(
            new[] { 44, 50, 57 },
            AppMain.amFsMemberVariantIndices(Cpit, AppMain.AMD_FS_VARIANT_CONSOLE));
    }

    [Fact]
    public void IndicesAreScopedToTheirVariant()
    {
        Load(Cpit + "#44 console");

        Assert.Null(AppMain.amFsMemberVariantIndices(Cpit, "beta"));
        Assert.Null(AppMain.amFsMemberVariantIndices(Cpit, AppMain.AMD_FS_VARIANT_BASE));
        Assert.Null(AppMain.amFsMemberVariantIndices("G_COM/CPIT/OTHER.AMB", AppMain.AMD_FS_VARIANT_CONSOLE));
    }

    [Fact]
    public void TheGateNeedsEveryIndexTheScreenNames()
    {
        Load(Cpit + "#44 console", Cpit + "#45 console");

        Assert.True(AppMain.amFsAllMemberVariant(AppMain.AMD_FS_VARIANT_CONSOLE, Cpit, 44, 45));
        Assert.False(AppMain.amFsAllMemberVariant(AppMain.AMD_FS_VARIANT_CONSOLE, Cpit, 44, 46));
        Assert.False(AppMain.amFsAllMemberVariant(AppMain.AMD_FS_VARIANT_CONSOLE, "G_COM/CPIT/OTHER.AMB", 44));
    }

    private static AppMain.AMS_AMB_HEADER Archive(
        bool bigEndian, params (string Name, byte[] Data)[] members)
    {
        var entries = new System.Collections.Generic.List<AppMain.AmbEntry>();
        foreach (var member in members)
            entries.Add(new AppMain.AmbEntry(member.Name, member.Data));

        var bytes = AppMain.writeAmbArchive(entries, bigEndian);
        var header = new AppMain.AMS_AMB_HEADER { data = bytes };

        using var stream = new MemoryStream(bytes);
        AppMain.readAmbHeader(stream, ref header);
        header.data = bytes;
        header.buf = new object[header.file_num];

        return header;
    }

    private static AppMain.AMS_AMB_HEADER Archive(params (string Name, byte[] Data)[] members)
    {
        return Archive(bigEndian: false, members);
    }

    // splices `sibling` over `header` the way phase 1 of the background read does
    private static void Splice(
        AppMain.AMS_AMB_HEADER header, AppMain.AMS_AMB_HEADER sibling, params int[] indices)
    {
        header.member_variant_amb = sibling;
        header.member_variant_index = new int[header.file_num];
        for (int i = 0; i < header.file_num; i++)
            header.member_variant_index[i] = -1;

        for (int i = 0; i < indices.Length; i++)
            header.member_variant_index[indices[i]] = i;
    }

    private static byte[] Bytes(AppMain.AmbChunk chunk)
    {
        var copy = new byte[chunk.length];
        System.Buffer.BlockCopy(chunk.array, chunk.offset, copy, 0, chunk.length);
        return copy;
    }

    [Fact]
    public void AnOverriddenMemberWinsAndItsNeighboursDoNot()
    {
        var header = Archive(
            ("A.AMA", new byte[] { 1, 1, 1, 1 }),
            ("B.AMA", new byte[] { 2, 2, 2, 2 }),
            ("C.AMA", new byte[] { 3, 3, 3, 3 }));

        Splice(header, Archive(("B.AMA", new byte[] { 9, 9 })), 1);

        Assert.Equal(new byte[] { 1, 1, 1, 1 }, Bytes(AppMain.amBindChunk(header, 0)));
        Assert.Equal(new byte[] { 9, 9 }, Bytes(AppMain.amBindChunk(header, 1)));
        Assert.Equal(new byte[] { 3, 3, 3, 3 }, Bytes(AppMain.amBindChunk(header, 2)));
    }

    // the whole reason the chunk is parented to the sibling rather than copied out of it: an .AMA
    // has no size field of its own, so a reader that asked the base would read xbla bytes as
    // little-endian
    [Fact]
    public void AnOverriddenChunkKeepsTheSiblingsByteOrder()
    {
        var header = Archive(bigEndian: false, ("A.AMA", new byte[] { 1, 1, 1, 1 }));
        var sibling = Archive(bigEndian: true, ("A.AMA", new byte[] { 2, 2, 2, 2 }));

        Assert.False(header.big_endian);
        Assert.True(sibling.big_endian);

        Splice(header, sibling, 0);

        Assert.True(AppMain.amBindChunk(header, 0).amb.big_endian);
        Assert.True(AppMain.amBindChunk(header, 0).amb.is_real_amb);
    }

    [Fact]
    public void SearchByNameSeesTheOverrideToo()
    {
        var header = Archive(("A.AMA", new byte[] { 1, 1 }), ("B.AMA", new byte[] { 2, 2 }));

        Splice(header, Archive(("B.AMA", new byte[] { 8, 8, 8 })), 1);

        Assert.Equal(new byte[] { 8, 8, 8 }, Bytes(AppMain.amBindSearch(header, "B.AMA")));
    }
}
