using System;

internal partial class AppMain
{
    // nnfmaterialdxg20.h aliases every NND_*_STDSHADER_* constant straight onto that platform's D3D
    // macro, so a material exports whatever the building platform's header said. Xbox 360's render
    // state enums are not D3D9's, and a big-endian DXG20 file is Xenon by construction.
    private static ushort mppXenonValue(ushort value, ushort[] table, string name)
    {
        if (value >= table.Length || table[value] == 0)
            throw new NotSupportedException(
                $"Xenon {name} {value} has no measured D3D9 value; see docs/xenon-render-state.md");

        return table[value];
    }

    // ZERO, ONE, then a two-value gap before SRCCOLOR. 4 and 5 are the values the corpus never uses
    // and no sample pins, not values known to be absent.
    private static readonly ushort[] mpp_xenon_blend =
    {
        1, 2, 0, 0, 3, 0, 5, 6,
    };

    private static readonly ushort[] mpp_xenon_blendop =
    {
        1,
    };

    // dense NEVER..ALWAYS on both platforms, so the whole range shifts by one
    private static readonly ushort[] mpp_xenon_cmpfunc =
    {
        1, 2, 3, 4, 5, 6, 7, 8,
    };

    // WRAP, MIRROR, CLAMP. BORDER and MIRRORONCE are unused by the corpus.
    private static readonly ushort[] mpp_xenon_textureaddress =
    {
        1, 2, 3,
    };

    public static ushort mppXenonBlend(ushort value) => mppXenonValue(value, mpp_xenon_blend, "D3DBLEND");

    public static ushort mppXenonBlendOp(ushort value) => mppXenonValue(value, mpp_xenon_blendop, "D3DBLENDOP");

    public static ushort mppXenonCmpFunc(ushort value) => mppXenonValue(value, mpp_xenon_cmpfunc, "D3DCMPFUNC");

    public static ushort mppXenonTextureAddress(ushort value) =>
        mppXenonValue(value, mpp_xenon_textureaddress, "D3DTEXTUREADDRESS");
}
