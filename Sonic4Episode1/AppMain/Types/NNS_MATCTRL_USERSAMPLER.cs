using Microsoft.Xna.Framework.Graphics;

internal partial class AppMain
{
    // nnlmaterialdxg20.h:166. The NNS_MATRIX44 between texmap and state is dropped: the stock
    // shader declares no user-sampler matrix uniform and every call site passes the identity.
    internal sealed class NNS_MATCTRL_USERSAMPLER
    {
        public Texture texmap;
        public readonly uint[] state = new uint[NNE_SAMPLERSTATETYPE_MAX];
    }
}
