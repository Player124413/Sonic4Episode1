#if BGM_STREAM
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

internal partial class AppMain
{
    internal static class CriAuBgmSource
    {
        public const string Intro = "Intro";
        public const string Loop = "Loop";

        // only populated when the source cannot be opened on demand. CriAuBgmVoice.Load hands the
        // stream straight to NVorbis and turns a failure into STATUS_ERROR, so on the web the bytes
        // have to be in hand before a cue starts rather than fetched when it does
        private static readonly Dictionary<string, byte[]> m_fetched =
            new Dictionary<string, byte[]>();

        private static readonly Dictionary<string, Task<byte[]>> m_fetching =
            new Dictionary<string, Task<byte[]>>();

        private static string Path(string track, string part)
        {
            return "SOUND/" + track + "/" + part + ".ogg";
        }

        // false while a part is still coming; GsSoundPrepareBGMForLevel is polled at the level load
        // barrier, so returning false costs a frame rather than a stall
        private static bool Ready(string path)
        {
            if (m_fetched.ContainsKey(path))
                return true;

            if (!m_fetching.TryGetValue(path, out var pending))
            {
                pending = ContentSource.Current.ReadAsync(path);
                m_fetching[path] = pending;
            }

            if (!pending.IsCompleted)
                return false;

            m_fetching.Remove(path);

            // a track with no Loop half is normal, and so is one the content build never emitted.
            // Record the miss so Open throws the same way an absent file would
            m_fetched[path] = pending.Status == TaskStatus.RanToCompletion ? pending.Result : null;
            return true;
        }

        public static bool Prepare(string track, bool loop)
        {
            if (ContentSource.Current.CanOpenOnDemand)
                return true;

            var ready = Ready(Path(track, Intro));
            if (loop && !Ready(Path(track, Loop)))
                ready = false;

            return ready;
        }

        public static void Clear()
        {
            m_fetched.Clear();
            m_fetching.Clear();
        }

        public static Stream Open(string track, string part)
        {
            var path = Path(track, part);
            if (ContentSource.Current.CanOpenOnDemand)
                return TitleContainer.OpenStream("Content\\" + path);

            if (!m_fetched.TryGetValue(path, out var bytes) || bytes == null)
                throw new FileNotFoundException($"'{path}' was not prepared", path);

            return new MemoryStream(bytes, false);
        }

        public static bool Exists(string track, string part)
        {
            try
            {
                using (Open(track, part))
                    return true;
            }
            catch (IOException)
            {
                return false;
            }
        }
    }
}
#endif
