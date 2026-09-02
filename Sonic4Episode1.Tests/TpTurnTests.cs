namespace Sonic4Episode1.Tests;

// The flip cards are almost entirely table driven, and every one of those tables fails quietly: a
// frame segment reads its model from one table, its offset from a second and its two materials from
// a third, all indexed [variant][slot], so a row that drifts by one slot draws the right models in
// the wrong places rather than crashing. That is not hypothetical - the first hand transcription of
// variants 4 and 6 was off by one slot, and it was the offset table disagreeing that caught it.
// See docs/z2a2-tp-turn-re.md.
public class TpTurnTests
{
    private static readonly int[] AllVariants = { 0, 1, 2, 3, 4, 5, 6 };

    [Fact]
    public void EveryTableCoversEveryVariantAndSlot()
    {
        Assert.Equal(AppMain.GMD_GMK_TP_TURN_VARIANT_MAX, AppMain.gm_gmk_tp_turn_member_tbl.Length);
        Assert.Equal(AppMain.GMD_GMK_TP_TURN_VARIANT_MAX, AppMain.gm_gmk_tp_turn_mat_tbl.Length);
        Assert.Equal(AppMain.GMD_GMK_TP_TURN_VARIANT_MAX, AppMain.gm_gmk_tp_turn_mat_hit_tbl.Length);
        Assert.Equal(AppMain.GMD_GMK_TP_TURN_VARIANT_MAX, AppMain.gm_gmk_tp_turn_ofst_tbl.Length);
        Assert.Equal(AppMain.GMD_GMK_TP_TURN_VARIANT_MAX, AppMain.gm_gmk_tp_turn_mat_slot_tbl.Length);

        Assert.Equal(AppMain.GMD_GMK_TP_TURN_FRAME_MAX, AppMain.gm_gmk_tp_turn_draw_flag_tbl.Length);

        foreach (int variant in AllVariants)
        {
            Assert.Equal(AppMain.GMD_GMK_TP_TURN_FRAME_MAX, AppMain.gm_gmk_tp_turn_member_tbl[variant].Length);
            Assert.Equal(AppMain.GMD_GMK_TP_TURN_FRAME_MAX, AppMain.gm_gmk_tp_turn_mat_tbl[variant].Length);
            Assert.Equal(AppMain.GMD_GMK_TP_TURN_FRAME_MAX, AppMain.gm_gmk_tp_turn_mat_hit_tbl[variant].Length);
            Assert.Equal(AppMain.GMD_GMK_TP_TURN_FRAME_MAX, AppMain.gm_gmk_tp_turn_ofst_tbl[variant].Length);
        }
    }

    // The three per-slot tables have to agree on which slots are live. This is the check that
    // caught variants 4 and 6 sitting a slot off the offsets they belong to
    [Fact]
    public void TheLiveSlotsAgreeAcrossEveryTable()
    {
        foreach (int variant in AllVariants)
        {
            int[] members = AppMain.gm_gmk_tp_turn_member_tbl[variant];
            int[] mats = AppMain.gm_gmk_tp_turn_mat_tbl[variant];
            int[] hits = AppMain.gm_gmk_tp_turn_mat_hit_tbl[variant];
            VecFx32[] offsets = AppMain.gm_gmk_tp_turn_ofst_tbl[variant];

            for (int slot = 0; slot < AppMain.GMD_GMK_TP_TURN_FRAME_MAX; ++slot)
            {
                bool live = members[slot] >= 0;
                string where = $"variant {variant} slot {slot}";

                Assert.True(live == (mats[slot] >= 0), $"{where}: material disagrees with model");
                Assert.True(live == (hits[slot] >= 0), $"{where}: lit material disagrees with model");

                // every live segment sits at a non-zero z, so a zero offset means the row moved
                bool placed = offsets[slot].x != 0 || offsets[slot].y != 0 || offsets[slot].z != 0;
                Assert.True(live == placed, $"{where}: offset disagrees with model");
            }
        }
    }

    // Both MAT archives interleave base and lit, so the lit material is always the base plus one.
    // PC ships the derived table rather than deriving it, and a hand edit to one and not the other
    // would light the wrong segment
    [Fact]
    public void TheLitMaterialIsAlwaysTheOneAfterTheBase()
    {
        foreach (int variant in AllVariants)
        {
            for (int slot = 0; slot < AppMain.GMD_GMK_TP_TURN_FRAME_MAX; ++slot)
            {
                int mat = AppMain.gm_gmk_tp_turn_mat_tbl[variant][slot];
                if (mat < 0)
                    continue;

                Assert.Equal(mat + 1, AppMain.gm_gmk_tp_turn_mat_hit_tbl[variant][slot]);
            }
        }
    }

    // GMK_TP_TURN_MDL ships 12 members: 0-2 are the pict faces and 3-11 are FRAME_A..I. A frame
    // slot pointing at 0-2 would draw a playing card face as a piece of neon border
    [Fact]
    public void EveryFrameModelIsAFrameMember()
    {
        foreach (int variant in AllVariants)
        {
            foreach (int member in AppMain.gm_gmk_tp_turn_member_tbl[variant])
            {
                if (member < 0)
                    continue;

                Assert.InRange(member, 3, 11);
            }
        }
    }

    // The two MAT archives are different sizes - TP_TURN_MAT has 12 members and TP_TURN_8_MAT has
    // 6 - so a variant reading the wrong one would run off the end of the smaller
    [Theory]
    [InlineData(0, 12)]
    [InlineData(1, 12)]
    [InlineData(2, 12)]
    [InlineData(3, 6)]
    [InlineData(4, 6)]
    [InlineData(5, 12)]
    [InlineData(6, 12)]
    public void EveryMaterialFitsTheArchiveItsVariantLoads(int variant, int archive_members)
    {
        int expected_slot = archive_members == 6
            ? AppMain.GMD_DWORK_NO_GMK_TP_TURN_8_MAT
            : AppMain.GMD_DWORK_NO_GMK_TP_TURN_MAT;

        Assert.Equal(expected_slot, AppMain.gm_gmk_tp_turn_mat_slot_tbl[variant]);

        for (int slot = 0; slot < AppMain.GMD_GMK_TP_TURN_FRAME_MAX; ++slot)
        {
            int mat = AppMain.gm_gmk_tp_turn_mat_tbl[variant][slot];
            if (mat < 0)
                continue;

            Assert.InRange(mat, 0, archive_members - 1);
            Assert.InRange(AppMain.gm_gmk_tp_turn_mat_hit_tbl[variant][slot], 0, archive_members - 1);
        }
    }

    // Odd slots are the mirrored halves of the even ones, which is how a frame is built from half
    // as many models as it draws. Losing the alternation collapses each pair on top of itself.
    // DIR2DFLIP has to be on every slot too, for a different reason: it suppresses the quarter
    // turn about Y that the draw otherwise applies, which would leave every segment edge on
    [Fact]
    public void OnlyTheOddSlotsMirror()
    {
        for (int slot = 0; slot < AppMain.GMD_GMK_TP_TURN_FRAME_MAX; ++slot)
        {
            uint flags = AppMain.gm_gmk_tp_turn_draw_flag_tbl[slot];

            Assert.True((flags & AppMain.OBD_DISP_DIR2DFLIP) != 0, $"slot {slot} lost DIR2DFLIP");
            Assert.Equal(slot % 2 == 1, (flags & AppMain.OBD_DISP_HFLIP) != 0);
        }
    }

    // Variants 3 and 4 are the same three models on opposite slots, so the pair is one picture card
    // frame and its mirror image rather than two different frames
    [Fact]
    public void TheTwoPictureCardVariantsAreEachOthersMirror()
    {
        for (int slot = 0; slot < AppMain.GMD_GMK_TP_TURN_FRAME_MAX; ++slot)
        {
            int even = slot - slot % 2;
            Assert.Equal(AppMain.gm_gmk_tp_turn_member_tbl[3][even],
                AppMain.gm_gmk_tp_turn_member_tbl[4][even + 1]);
        }
    }

    // Faces are 0 Eggman, 1 Ring, 2 Sonic, and the kind doubles as the MDL member the face is drawn
    // from. A 3 in a layout would index past the pict models into FRAME_A
    [Fact]
    public void NoBoardLayoutDealsAFaceThatDoesNotExist()
    {
        Assert.Equal(AppMain.GMD_GMK_TP_TURN_BOARD_LAYOUT_MAX, AppMain.gm_gmk_tp_turn_board_layout_tbl.Length);

        foreach (byte[] layout in AppMain.gm_gmk_tp_turn_board_layout_tbl)
        {
            Assert.Equal(AppMain.GMD_GMK_TP_TURN_PICT_PER_LINE, layout.Length);

            foreach (byte row in layout)
            {
                for (int col = 0; col < AppMain.GMD_GMK_TP_TURN_PICT_PER_LINE; ++col)
                    Assert.InRange((row >> 2 * col) & 3, 0, AppMain.GMD_GMK_TP_TURN_KIND_MAX - 1);
            }
        }
    }

    // Panels reach their maker through the local event birth, which can only carry one byte, so
    // the whole of a panel's identity is packed into it. Getting the layout wrong scrambles which
    // face lands where without failing anywhere
    [Theory]
    [InlineData(0, 0, 0, false)]
    [InlineData(3, 3, 2, true)]
    [InlineData(2, 1, 1, false)]
    [InlineData(1, 2, 2, true)]
    public void ThePackedPanelByteRoundTrips(int row, int col, int kind, bool revealed)
    {
        byte packed = (byte)((row << 6) | (col << 4) | (kind << 1) | (revealed ? 1 : 0));

        Assert.Equal(row, packed >> 6);
        Assert.Equal(col, (packed & 0x3F) >> 4);
        Assert.Equal(kind, (packed & 0x0F) >> 1);
        Assert.Equal(revealed, (packed & 1) != 0);
    }

    // the four archives are appended past the card road's, so the gimmick array has to have grown
    // with them. GmGameDatGetGimmickData indexes it by slot - GMK_START
    [Fact]
    public void TheGamedatSlotsResolveInsideTheWidenedArray()
    {
        int[] slots =
        {
            AppMain.GMD_DWORK_NO_GMK_TP_TURN_MDL,
            AppMain.GMD_DWORK_NO_GMK_TP_TURN_TEX,
            AppMain.GMD_DWORK_NO_GMK_TP_TURN_MAT,
            AppMain.GMD_DWORK_NO_GMK_TP_TURN_8_MAT,
        };

        foreach (int slot in slots)
        {
            Assert.True(slot >= AppMain.GMD_DWORK_NO_GMK_START, $"slot {slot} below GMK_START");
            Assert.True(slot < AppMain.GMD_DWORK_NO_GMK_END, $"slot {slot} past GMK_END");
        }

        Assert.Equal(AppMain.GMD_DWORK_NO_GMK_END, AppMain.GMD_DWORK_NO_DEC_START);
        Assert.True(AppMain.GMD_DWORK_NO_MAX >= AppMain.GMD_DWORK_NO_DEC_END);
    }

    // Not covered here, because they need a started game rather than a static table: the deck
    // randomiser, the match window and the reward path. All three are verified by playing.
}
