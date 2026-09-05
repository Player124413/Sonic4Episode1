using System.Threading.Tasks;
using Microsoft.Xna.Framework;

// where the game gets content bytes. The web head installs one that fetches; everything else reads
// the content tree off disk
public interface IContentSource
{
    // false when a read has to be awaited. Callers that cannot await, because they are handing
    // bytes to something synchronous, have to prefetch through a gate rather than open on demand
    bool CanOpenOnDemand { get; }

    // path is variant-resolved and relative to the content root, no "Content" on the front
    byte[] Read(string path);

    Task<byte[]> ReadAsync(string path);
}

public static class ContentSource
{
    private static IContentSource m_current;

    // lazy rather than a field initialiser: tools reach content without running AppMain's cctor
    public static IContentSource Current
    {
        get => m_current ??= new TitleContentSource();
        set => m_current = value;
    }
}

internal sealed class TitleContentSource : IContentSource
{
    public bool CanOpenOnDemand => true;

    public byte[] Read(string path)
    {
        return AppMain.amFsReadAll(TitleContainer.OpenStream("Content\\" + path));
    }

    public Task<byte[]> ReadAsync(string path)
    {
#if ASYNC_TARGETING_PACK
        return Task.Factory.StartNew(() => Read(path));
#else
        return Task.Run(() => Read(path));
#endif
    }
}
