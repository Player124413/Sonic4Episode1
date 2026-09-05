using System;
using System.IO;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

// the host does the fetching, unpacking and caching. The boot tree is laid out into MEMFS for the
// synchronous readers; everything else is a path in, bytes out.
internal sealed partial class BundleContentSource : IContentSource
{
    public bool CanOpenOnDemand => false;

    public static Task InstallAsync(string profile)
    {
        return ContentInstall(profile);
    }

    public byte[] Read(string path)
    {
        using var stream = TitleContainer.OpenStream("Content\\" + path);
        var bytes = new byte[stream.Length];
        var read = 0;
        while (read < bytes.Length)
        {
            var got = stream.Read(bytes, read, bytes.Length - read);
            if (got <= 0)
                throw new EndOfStreamException(
                    $"'{path}' ended {bytes.Length - read} bytes short of {bytes.Length}");

            read += got;
        }

        return bytes;
    }

    // the generator has no Task<byte[]>, so the length comes back first and the bytes are copied
    // into a managed buffer through a view. The view is only valid during the synchronous call
    public async Task<byte[]> ReadAsync(string path)
    {
        var length = await ContentReadBegin(path);
        var bytes = new byte[length];
        ContentReadTake(path, bytes);
        return bytes;
    }

    [JSImport("contentInstall", "main.js")]
    private static partial Task ContentInstall(string profile);

    [JSImport("contentReadBegin", "main.js")]
    private static partial Task<int> ContentReadBegin(string path);

    [JSImport("contentReadTake", "main.js")]
    private static partial void ContentReadTake(
        string path, [JSMarshalAs<JSType.MemoryView>] Span<byte> into);
}
