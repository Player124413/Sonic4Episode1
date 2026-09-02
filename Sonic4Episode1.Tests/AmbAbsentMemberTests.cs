using System.IO;

namespace Sonic4Episode1.Tests;

// An AMB can carry absent members, written as offset 0 and size 0. Console leaves those null through
// amBindConv - its amConvert macro only rebases a non-zero offset - so amBindGet returns null and
// every `if (data)` guard in the game does what it looks like it does. Our port built a zero-length
// chunk pointing at the archive header instead, which reads as "present but empty" to callers that
// have no reason to check.
//
// It stayed invisible while every archive we loaded was dense, and surfaced the moment zone 2's map
// motion sidecars went in: ZONE2_M_M has 4 live members of 268, and handing the other 264 to the NN
// motion parser is an EndOfStreamException on stage load.
public class AmbAbsentMemberTests
{
    private const int HEADER = 0x20;
    private const int ENTRY = 0x10;

    // Member 0 absent the way a source archive writes it, offset 0; member 2 absent the way our
    // content build repacks it, offset pointing at the running data position with a zero length.
    // The second shape is the one that actually shipped and the one an offset test misses
    private static byte[] ArchiveWithAbsentMembers()
    {
        const int count = 3;
        int payload_at = HEADER + count * ENTRY;
        var bytes = new byte[payload_at + 4];

        using (var stream = new MemoryStream(bytes))
        using (var w = new BinaryWriter(stream))
        {
            w.Write(AppMain.AMB_FILE_ID);
            w.Write(AppMain.AMB_HEADER_SIZE);
            w.Write(0x00040000);
            w.Write(0);
            w.Write(count);
            w.Write(HEADER);
            w.Write(0);
            w.Write(0);

            // offset, size, name offset, reserved
            w.Write(0); w.Write(0); w.Write(-1); w.Write(0);
            w.Write(payload_at); w.Write(4); w.Write(-1); w.Write(0);
            w.Write(payload_at + 4); w.Write(0); w.Write(-1); w.Write(0);

            w.Write(0x21524E4E);
        }

        return bytes;
    }

    private static AppMain.AMS_AMB_HEADER Read(byte[] bytes)
    {
        var header = new AppMain.AMS_AMB_HEADER { data = bytes };

        using var stream = new MemoryStream(bytes);
        AppMain.readAmbHeader(stream, ref header);

        return header;
    }

    [Fact]
    public void AnAbsentMemberComesBackAsNull()
    {
        var amb = Read(ArchiveWithAbsentMembers());

        Assert.Null(AppMain.amBindGet(amb, 0, out string _));
        Assert.Null(AppMain.amBindGet(amb, 2, out string _));
    }

    // the repacked shape: a real offset and no bytes. Testing the offset alone lets this one
    // through, which is how ZONE2_M_M's 264 absent members reached the NN motion parser
    [Fact]
    public void AnAbsentMemberWithARealOffsetIsStillAbsent()
    {
        var amb = Read(ArchiveWithAbsentMembers());

        Assert.NotEqual(0, amb.offsets[2]);
        Assert.Equal(0, amb.lengths[2]);
        Assert.Null(AppMain.amBindGet(amb, 2, out string _));
    }

    [Fact]
    public void APresentMemberIsUnaffected()
    {
        var amb = Read(ArchiveWithAbsentMembers());
        var chunk = AppMain.amBindGet(amb, 1, out string _);

        Assert.NotNull(chunk);
        Assert.Equal(4, chunk.length);
    }

    // the object overload wraps a nested archive, and a null member is not one
    [Fact]
    public void TheWrappingOverloadPassesTheNullThrough()
    {
        var amb = Read(ArchiveWithAbsentMembers());

        Assert.Null(AppMain.amBindGet(amb, 0));
        Assert.NotNull(AppMain.amBindGet(amb, 1));
    }
}
