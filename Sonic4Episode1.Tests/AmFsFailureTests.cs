using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sonic4Episode1.Tests;

// A read that fails must not reach the frame loop. amFs services only the head of its queue, so a
// failed request parks there the way the C++ leaves AMD_FS_STAT_CANTOPEN, and waits to be retried
[Collection(FsVariantCollection.Name)]
public class AmFsFailureTests
{
    private sealed class ScriptedSource : IContentSource
    {
        public Exception Fail;
        public byte[] Bytes;
        public int Reads;

        public bool CanOpenOnDemand => false;

        public byte[] Read(string path)
        {
            throw new NotSupportedException("the test source is async only");
        }

        public Task<byte[]> ReadAsync(string path)
        {
            Reads++;
            return Fail != null
                ? Task.FromException<byte[]>(Fail)
                : Task.FromResult(Bytes);
        }
    }

    private static byte[] Archive()
    {
        var entries = new List<AppMain.AmbEntry>
        {
            new AppMain.AmbEntry("A.BIN", new byte[] { 1, 2, 3, 4 }),
        };

        return AppMain.writeAmbArchive(entries, false);
    }

    [Fact]
    public void AFailedReadParksTheRequestInsteadOfThrowing()
    {
        var previous = ContentSource.Current;
        try
        {
            var source = new ScriptedSource { Fail = new HttpRequestException("boom") };
            ContentSource.Current = source;

            var fs = AppMain.amFsReadBackground("TEST/PARK.AMB");
            AppMain.amFsExecuteBackgroundRead();

            Assert.False(AppMain.amFsIsComplete(fs));
            Assert.Same(fs, AppMain.amFsFailedRequest());

            AppMain.amFsClearRequest(fs);
        }
        finally
        {
            ContentSource.Current = previous;
        }
    }

    // without this a parked request reissues every frame, turning one dead file into a request storm
    [Fact]
    public void AParkedRequestDoesNotReissueOnTheNextPump()
    {
        var previous = ContentSource.Current;
        try
        {
            var source = new ScriptedSource { Fail = new HttpRequestException("boom") };
            ContentSource.Current = source;

            var fs = AppMain.amFsReadBackground("TEST/STICKY.AMB");
            AppMain.amFsExecuteBackgroundRead();

            var after = source.Reads;
            AppMain.amFsExecuteBackgroundRead();
            AppMain.amFsExecuteBackgroundRead();

            Assert.Equal(after, source.Reads);

            AppMain.amFsClearRequest(fs);
        }
        finally
        {
            ContentSource.Current = previous;
        }
    }

    [Fact]
    public void RetryingAParkedRequestReadsItAgainAndCompletes()
    {
        var previous = ContentSource.Current;
        try
        {
            var source = new ScriptedSource { Fail = new HttpRequestException("boom") };
            ContentSource.Current = source;

            var fs = AppMain.amFsReadBackground("TEST/RECOVER.AMB");
            AppMain.amFsExecuteBackgroundRead();
            Assert.NotNull(AppMain.amFsFailedRequest());

            source.Fail = null;
            source.Bytes = Archive();
            AppMain.amFsRetryRequest(fs);

            AppMain.amFsExecuteBackgroundRead();

            Assert.Null(AppMain.amFsFailedRequest());
            Assert.True(AppMain.amFsIsComplete(fs));
        }
        finally
        {
            ContentSource.Current = previous;
        }
    }
}
