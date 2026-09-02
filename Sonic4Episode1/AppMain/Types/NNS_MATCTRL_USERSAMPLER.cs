using Microsoft.Xna.Framework.Graphics;

internal partial class AppMain
{
    // nnlmaterialdxg20.h:166. The NNS_MATRIX44 between texmap and state is dropped: the stock
    // shader declares no user-sampler matrix uniform and every call site passes the identity.
    internal sealed class NNS_MATCTRL_USERSAMPLER
    {
        public Texture texmap;
        public readonly uint[] state = new uint[NNE_SAMPLERSTATETYPE_MAX];

        // the bind path runs every frame and the state behind it almost never moves, so the XNA
        // state object is built when the state is written rather than when it is read
        public SamplerState sampler;
    }
}
