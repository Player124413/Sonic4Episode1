internal partial class AppMain
{
    // The seven frame variants of the flip cards, read out of the retail PC binary. Every table is
    // indexed [variant][slot] over ten frame segment slots, and the seven blocks sit contiguously
    // at 0x581150/0x581268/0x581380/0x581498/0x5814C0, which is what pins their sizes. Provenance
    // and the variant selection rules are in docs/z2a2-tp-turn-re.md.
    //
    // Variants: 0 board (ev 302), 1/3/4 horizontal (303), 2/5/6 vertical (304).

    internal const int GMD_GMK_TP_TURN_FRAME_MAX = 10;
    internal const int GMD_GMK_TP_TURN_VARIANT_MAX = 7;

    // MDL members: 0-2 are the pict faces (EGG/RING/SONIC, so the member is the kind), 3-11 are
    // FRAME_A..I. PC 0x581150
    internal static readonly int[][] gm_gmk_tp_turn_member_tbl =
    {
        new[] {  3,  3,  4,  4,  4,  4,  4,  4,  5,  5 },   // A,A  B x6  C,C
        new[] {  3,  3,  4,  4,  5,  5, -1, -1, -1, -1 },   // A,A  B,B  C,C
        new[] {  6,  6,  7,  7,  7,  7,  7,  7,  8,  8 },   // D,D  E x6  F,F
        new[] {  9, -1, 10, -1, 11, -1, -1, -1, -1, -1 },   // G,H,I
        new[] { -1,  9, -1, 10, -1, 11, -1, -1, -1, -1 },   // G,H,I on the odd slots, so mirrored
        new[] {  6,  6,  7,  7,  7,  7,  7,  7, -1, -1 },   // D,D  E x6, no bottom cap
        new[] { -1, -1,  7,  7,  7,  7,  7,  7,  8,  8 },   // E x6  F,F, no top cap
    };

    // Which MAT archive each variant draws its materials from. PC 0x57A95C holds its own gamedat
    // ordinals (1069/1070); ours are allocated at the tail of the GMK block instead
    internal static readonly int[] gm_gmk_tp_turn_mat_slot_tbl =
    {
        GMD_DWORK_NO_GMK_TP_TURN_MAT,
        GMD_DWORK_NO_GMK_TP_TURN_MAT,
        GMD_DWORK_NO_GMK_TP_TURN_MAT,
        GMD_DWORK_NO_GMK_TP_TURN_8_MAT,
        GMD_DWORK_NO_GMK_TP_TURN_8_MAT,
        GMD_DWORK_NO_GMK_TP_TURN_MAT,
        GMD_DWORK_NO_GMK_TP_TURN_MAT,
    };

    // Both MAT archives interleave base and lit: TP_TURN_MAT is A,A_HIT,B,B_HIT..F_HIT and
    // TP_TURN_8_MAT is G,G_HIT,H,H_HIT,I,I_HIT, so the lit index is always the base plus one.
    // PC still ships both tables rather than deriving one, and TpTurnTableTests pins that they
    // agree. PC 0x581268
    internal static readonly int[][] gm_gmk_tp_turn_mat_tbl =
    {
        new[] {  0,  0,  2,  2,  2,  2,  2,  2,  4,  4 },
        new[] {  0,  0,  2,  2,  4,  4, -1, -1, -1, -1 },
        new[] {  6,  6,  8,  8,  8,  8,  8,  8, 10, 10 },
        new[] {  0, -1,  2, -1,  4, -1, -1, -1, -1, -1 },
        new[] { -1,  0, -1,  2, -1,  4, -1, -1, -1, -1 },
        new[] {  6,  6,  8,  8,  8,  8,  8,  8, -1, -1 },
        new[] { -1, -1,  8,  8,  8,  8,  8,  8, 10, 10 },
    };

    // PC 0x581380
    internal static readonly int[][] gm_gmk_tp_turn_mat_hit_tbl =
    {
        new[] {  1,  1,  3,  3,  3,  3,  3,  3,  5,  5 },
        new[] {  1,  1,  3,  3,  5,  5, -1, -1, -1, -1 },
        new[] {  7,  7,  9,  9,  9,  9,  9,  9, 11, 11 },
        new[] {  1, -1,  3, -1,  5, -1, -1, -1, -1, -1 },
        new[] { -1,  1, -1,  3, -1,  5, -1, -1, -1, -1 },
        new[] {  7,  7,  9,  9,  9,  9,  9,  9, -1, -1 },
        new[] { -1, -1,  9,  9,  9,  9,  9,  9, 11, 11 },
    };

    // One row for all seven variants, not per variant. Every entry carries OBD_DISP_DIR2DFLIP,
    // and the odd slots add OBD_DISP_HFLIP, which is how the right hand column of a frame is drawn
    // from the same models as the left. PC 0x581498
    internal static readonly uint[] gm_gmk_tp_turn_draw_flag_tbl =
    {
        0x04000004U, 0x04000005U, 0x04000004U, 0x04000005U, 0x04000004U,
        0x04000005U, 0x04000004U, 0x04000005U, 0x04000004U, 0x04000005U,
    };

    // Segment offsets from the card origin. The left hand columns carry a +0.3 fudge that the
    // mirrored right hand ones do not, which is why they are not clean multiples of 4096; z is
    // +64 everywhere except variant 6's middle row. PC 0x5814C0
    internal static readonly VecFx32[][] gm_gmk_tp_turn_ofst_tbl =
    {
        new[]
        {
            new VecFx32(-260915, -160 * 4096, 64 * 4096),
            new VecFx32(  64 * 4096, -160 * 4096, 64 * 4096),
            new VecFx32(-457523,  -64 * 4096, 64 * 4096),
            new VecFx32( 112 * 4096,  -64 * 4096, 64 * 4096),
            new VecFx32(-457523,    0,          64 * 4096),
            new VecFx32( 112 * 4096,    0,          64 * 4096),
            new VecFx32(-457523,   64 * 4096, 64 * 4096),
            new VecFx32( 112 * 4096,   64 * 4096, 64 * 4096),
            new VecFx32(-260915,  128 * 4096, 64 * 4096),
            new VecFx32(  64 * 4096,  128 * 4096, 64 * 4096),
        },
        new[]
        {
            new VecFx32(-260915,  -96 * 4096, 64 * 4096),
            new VecFx32(  64 * 4096,  -96 * 4096, 64 * 4096),
            new VecFx32(-457523,    0,          64 * 4096),
            new VecFx32( 112 * 4096,    0,          64 * 4096),
            new VecFx32(-260915,   64 * 4096, 64 * 4096),
            new VecFx32(  64 * 4096,   64 * 4096, 64 * 4096),
            new VecFx32(0, 0, 0),
            new VecFx32(0, 0, 0),
            new VecFx32(0, 0, 0),
            new VecFx32(0, 0, 0),
        },
        new[]
        {
            new VecFx32(-260915, -160 * 4096, 64 * 4096),
            new VecFx32(  64 * 4096, -160 * 4096, 64 * 4096),
            new VecFx32(-260915,  -64 * 4096, 64 * 4096),
            new VecFx32(  64 * 4096,  -64 * 4096, 64 * 4096),
            new VecFx32(-260915,    0,          64 * 4096),
            new VecFx32(  64 * 4096,    0,          64 * 4096),
            new VecFx32(-260915,   64 * 4096, 64 * 4096),
            new VecFx32(  64 * 4096,   64 * 4096, 64 * 4096),
            new VecFx32(-260915,  128 * 4096, 64 * 4096),
            new VecFx32(  64 * 4096,  128 * 4096, 64 * 4096),
        },
        new[]
        {
            new VecFx32(  33997,  -96 * 4096, 64 * 4096),
            new VecFx32(0, 0, 0),
            new VecFx32(-490291,    0,          64 * 4096),
            new VecFx32(0, 0, 0),
            new VecFx32(  33997,   64 * 4096, 64 * 4096),
            new VecFx32(0, 0, 0),
            new VecFx32(0, 0, 0),
            new VecFx32(0, 0, 0),
            new VecFx32(0, 0, 0),
            new VecFx32(0, 0, 0),
        },
        new[]
        {
            new VecFx32(0, 0, 0),
            new VecFx32(  -8 * 4096,  -96 * 4096, 64 * 4096),
            new VecFx32(0, 0, 0),
            new VecFx32( 120 * 4096,    0,          64 * 4096),
            new VecFx32(0, 0, 0),
            new VecFx32(  -8 * 4096,   64 * 4096, 64 * 4096),
            new VecFx32(0, 0, 0),
            new VecFx32(0, 0, 0),
            new VecFx32(0, 0, 0),
            new VecFx32(0, 0, 0),
        },
        new[]
        {
            new VecFx32(-260915, -164 * 4096, 64 * 4096),
            new VecFx32(  64 * 4096, -164 * 4096, 64 * 4096),
            new VecFx32(-260915,  -68 * 4096, 64 * 4096),
            new VecFx32(  64 * 4096,  -68 * 4096, 64 * 4096),
            new VecFx32(-260915,   -4 * 4096, 64 * 4096),
            new VecFx32(  64 * 4096,   -4 * 4096, 64 * 4096),
            new VecFx32(-260915,   60 * 4096, 64 * 4096),
            new VecFx32(  64 * 4096,   60 * 4096, 64 * 4096),
            new VecFx32(0, 0, 0),
            new VecFx32(0, 0, 0),
        },
        new[]
        {
            new VecFx32(0, 0, 0),
            new VecFx32(0, 0, 0),
            new VecFx32(-260915,  -68 * 4096, 64 * 4096),
            new VecFx32(  64 * 4096,  -68 * 4096, 64 * 4096),
            new VecFx32(-260915,   -4 * 4096, 256 * 4096),
            new VecFx32(  64 * 4096,   -4 * 4096, 256 * 4096),
            new VecFx32(-260915,   60 * 4096, 64 * 4096),
            new VecFx32(  64 * 4096,   60 * 4096, 64 * 4096),
            new VecFx32(-260915,  124 * 4096, 64 * 4096),
            new VecFx32(  64 * 4096,  124 * 4096, 64 * 4096),
        },
    };

    // Eight canned 4x4 boards, one byte per row, four 2-bit kinds per byte low to high = left to
    // right. Layout 2 is sixteen Eggman. PC 0x581808
    internal static readonly byte[][] gm_gmk_tp_turn_board_layout_tbl =
    {
        new byte[] { 0x01, 0x25, 0x19, 0x42 },
        new byte[] { 0x54, 0x08, 0x20, 0x95 },
        new byte[] { 0x00, 0x00, 0x00, 0x00 },
        new byte[] { 0x15, 0x40, 0x6A, 0x40 },
        new byte[] { 0x20, 0x61, 0x61, 0x41 },
        new byte[] { 0x41, 0x14, 0x14, 0x41 },
        new byte[] { 0x14, 0x41, 0x41, 0x14 },
        new byte[] { 0x00, 0x10, 0x04, 0x01 },
    };

    internal const int GMD_GMK_TP_TURN_BOARD_LAYOUT_MAX = 8;
}
