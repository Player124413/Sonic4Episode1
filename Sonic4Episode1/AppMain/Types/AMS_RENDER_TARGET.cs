using Microsoft.Xna.Framework.Graphics;

internal partial class AppMain
{
    internal class AMS_RENDER_TARGET
    {
        public uint flag;
        public int surface_num;
        public int width;
        public int height;
        public float aspect;
        public RenderTarget2D target;
        public uint tex_name;
    }
}
