using System;
using System.IO;

namespace Sonic4Episode1.Tests;

// regression: amMotionSetup used to match only the GLES11 chunk ids, so a PC/Xbox 360 archive
// parsed to nothing and left the caller's array full of nulls
public class MotionSetupTests
{
    private static string Fixture(string name)
    {
        return Path.Combine(Path.GetDirectoryName(typeof(MotionSetupTests).Assembly.Location), "Fixtures", name);
    }

    private static AppMain.AMS_AMB_HEADER ReadArchive(string name)
    {
        var header = new AppMain.AMS_AMB_HEADER();
        header.data = File.ReadAllBytes(Fixture(name));

        using var stream = new MemoryStream(header.data);
        AppMain.readAmbHeader(stream, ref header);

        return header;
    }

    [Fact]
    public void PreloadsAndCollectsMotionsFromAPcArchive()
    {
        var header = ReadArchive("GMK_SPRING_MTN.AMB");
        AppMain.amPreLoadAmbItems(header);

        for (int i = 0; i < header.file_num; ++i)
            Assert.IsType<AppMain.NNS_MOTION>(header.buf[i]);

        var motions = new AppMain.NNS_MOTION[header.file_num];
        int count = AppMain.amMotionSetup(new AppMain.ArrayPointer<AppMain.NNS_MOTION>(motions), header);

        Assert.Equal(header.file_num, count);
        Assert.All(motions, motion => Assert.NotNull(motion));
    }

    [Fact]
    public void MotionBufferSizesToTheArchiveNotTheCompileTimeCount()
    {
        var header = ReadArchive("GMK_SPRING_MTN.AMB");
        AppMain.amPreLoadAmbItems(header);

        int count = AppMain.mppCountArchiveMotions(header);
        Assert.Equal(header.file_num, count);

        // amMotionCreate only reads nNode off the object, so any parsed object serves here
        var model = ReadArchive("RING_MDL_HD.AMB");
        using var chunk = new MemoryStream(model.data, model.offsets[0], model.lengths[0]);
        using var reader = new EndianBinaryReader(chunk, bigEndian: true);
        AppMain.amObjectSetup(out var obj, out _, reader);

        const int undersized = 2;
        var motion = AppMain.amMotionCreate(obj, Math.Max(undersized, count), 4, 0);
        AppMain.amMotionRegistFile(motion, 0, header);

        Assert.Equal(count, motion.mtnfile[0].motion_num);
        Assert.True(motion.motion_num >= count);
    }

    [Fact]
    public void ReadsEveryMotionOutOfAPcArchive()
    {
        var header = ReadArchive("GMK_SPRING_MTN.AMB");
        Assert.True(header.file_num > 0);

        var motions = new AppMain.NNS_MOTION[header.file_num];
        var cursor = new AppMain.ArrayPointer<AppMain.NNS_MOTION>(motions);

        int total = 0;
        for (int i = 0; i < header.file_num; ++i)
        {
            var chunk = new AppMain.AmbChunk(header.data, header.offsets[i], header.lengths[i], header);
            int read = AppMain.amMotionSetup(cursor, chunk);
            cursor += read;
            total += read;
        }

        Assert.Equal(header.file_num, total);
        for (int i = 0; i < total; ++i)
        {
            Assert.NotNull(motions[i]);
            Assert.True(motions[i].nSubmotion > 0);
            Assert.True(motions[i].EndFrame >= motions[i].StartFrame);
        }
    }
}
