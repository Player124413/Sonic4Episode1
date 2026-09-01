#if BGM_STREAM
using System.IO;
using Microsoft.Xna.Framework;

internal partial class AppMain
{
    internal static class CriAuBgmSource
    {
        public const string Intro = "Intro";
        public const string Loop = "Loop";

        public static bool Prepare(string track)
        {
            return true;
        }

        public static void Clear()
        {
        }

        public static Stream Open(string track, string part)
        {
            return TitleContainer.OpenStream("Content\\SOUND\\" + track + "\\" + part + ".ogg");
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
