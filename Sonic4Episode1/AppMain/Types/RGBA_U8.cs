using Microsoft.Xna.Framework;

internal partial class AppMain
{
    internal struct RGBA_U8
    {
        public byte r;
        public byte g;
        public byte b;
        public byte a;

        public static explicit operator Color(RGBA_U8 c)
        {
            return new Color(c.r, c.g, c.b, (int)c.a);
        }
    }
}
