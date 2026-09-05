using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sonic4Episode1.Tests;

// packflat turns a directory the runtime used to open a file at a time into one archive. The shader
// permutations are the tight case: 30 character names against a 32 byte slot
public class FlatArchiveTests
{
    // what Build-Permutations.ps1 emits, an NNS_STDSHADER_NAME key plus the extension
    private const string PermutationName = "0000000000000RDMJC0000MHC4.fxb";

    private static AppMain.AMS_AMB_HEADER Read(byte[] bytes)
    {
        var header = new AppMain.AMS_AMB_HEADER();
        header.data = bytes;

        using var stream = new MemoryStream(bytes);
        AppMain.readAmbHeader(stream, ref header);

        return header;
    }

    private static List<AppMain.AmbEntry> Permutations(int count)
    {
        var entries = new List<AppMain.AmbEntry>(count);
        for (int i = 0; i < count; i++)
        {
            var name = i.ToString("X8") + PermutationName.Substring(8);
            var data = new byte[64 + i];
            for (int j = 0; j < data.Length; j++)
                data[j] = (byte)(i + j);

            entries.Add(new AppMain.AmbEntry(name, data));
        }

        return entries;
    }

    [Fact]
    public void PermutationNamesSurviveTheThirtyTwoByteSlot()
    {
        Assert.Equal(AppMain.AMB_NAME_SIZE - 2, PermutationName.Length);

        var entries = Permutations(64);
        var archive = Read(AppMain.writeAmbArchive(entries, false));

        Assert.Equal(entries.Count, archive.file_num);
        for (int i = 0; i < entries.Count; i++)
            Assert.Equal(entries[i].Name, archive.files[i]);
    }

    [Fact]
    public void EveryMemberIsFoundByNameWithItsBytesIntact()
    {
        var entries = Permutations(64);
        var archive = Read(AppMain.writeAmbArchive(entries, false));

        foreach (var entry in entries)
        {
            var chunk = AppMain.amBindSearch(archive, entry.Name);
            Assert.NotNull(chunk);

            var found = chunk.array.AsSpan(chunk.offset, chunk.length);
            Assert.True(found.SequenceEqual(entry.Data), $"'{entry.Name}' came back different");
        }
    }

    // silent truncation would alias two permutations onto one member, which would show up as the
    // wrong shader rather than as a failure
    [Fact]
    public void RefusesANameThatWouldNotFit()
    {
        var tooLong = new string('x', AppMain.AMB_NAME_SIZE) + ".fxb";
        var entries = new List<AppMain.AmbEntry> { new AppMain.AmbEntry(tooLong, new byte[4]) };

        Assert.Throws<InvalidDataException>(() => AppMain.writeAmbArchive(entries, false));
    }

    [Fact]
    public void PacksDeterministically()
    {
        var first = AppMain.writeAmbArchive(Permutations(32), false);
        var second = AppMain.writeAmbArchive(Permutations(32), false);

        Assert.True(first.AsSpan().SequenceEqual(second), "two packs of the same input differ");
    }
}
