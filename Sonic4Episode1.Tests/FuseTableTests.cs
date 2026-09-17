namespace Sonic4Episode1.Tests;

// GMK_DYNAMITE_MDL and GMK_DYNAMITE_MAT are ordered differently, and the two selector tables have
// to name the same piece at every one of the twenty entries. That agreement is what pinned both
// archive orderings in the first place, so it is also the check that catches a transcription slip
public class FuseTableTests
{
    private static readonly string[] ModelMembers =
    {
        "Z3_DYNAMITE_OL", "Z3_DYNAMITE_OR", "Z3_DYNAMITE_UL", "Z3_DYNAMITE_UR",
        "Z3_LINE_BW_L", "Z3_LINE_BW_LE", "Z3_LINE_BW_R", "Z3_LINE_BW_RE",
        "Z3_LINE_LC_A", "Z3_LINE_LC_B", "Z3_LINE_LC_C", "Z3_LINE_LC_D",
        "Z3_LINE_LW_U", "Z3_LINE_LW_UE", "Z3_LINE_LW_O", "Z3_LINE_LW_OE",
        "Z3_LINE_RC_A", "Z3_LINE_RC_B", "Z3_LINE_RC_C", "Z3_LINE_RC_D",
        "Z3_LINE_J_OL", "Z3_LINE_J_OR", "Z3_LINE_J_UL", "Z3_LINE_J_UR",
        "Z3_LINE_BW_L_X", "GMK_BOMBWALL", "GMK_BOMBWALL_R",
    };

    private static readonly string[] MaterialMembers =
    {
        "J_UR", "J_UL", "J_OR", "J_OL",
        "BW_L", "BW_R", "BW_LE", "BW_RE",
        "LC_A", "LC_B", "LC_C", "LC_D",
        "LW_O", "LW_OE", "LW_U", "LW_UE",
        "RC_A", "RC_B", "RC_C", "RC_D",
        "BW_L_X",
    };

    private static string LinePiece(int model_index)
    {
        return ModelMembers[model_index].Substring("Z3_LINE_".Length);
    }

    [Fact]
    public void TheLineSelectorsNameTheSamePieces()
    {
        for (int i = 0; i < AppMain.gm_gmk_fuse_line_mdl_tbl.Length; ++i)
        {
            Assert.Equal(
                MaterialMembers[AppMain.gm_gmk_fuse_line_mat_tbl[i]],
                LinePiece(AppMain.gm_gmk_fuse_line_mdl_tbl[i]));
        }
    }

    [Fact]
    public void TheHeadSelectorsNameTheSamePieces()
    {
        for (int i = 0; i < AppMain.gm_gmk_fuse_head_mdl_tbl.Length; ++i)
        {
            Assert.Equal(
                MaterialMembers[AppMain.gm_gmk_fuse_head_mat_tbl[i]],
                LinePiece(AppMain.gm_gmk_fuse_head_mdl_tbl[i]));
        }
    }

    // a head piece is the half length variant of the edge it exits by, and the four of them are
    // the only pieces whose name ends in E
    [Fact]
    public void EveryHeadPieceIsAnEndVariant()
    {
        Assert.All(AppMain.gm_gmk_fuse_head_mdl_tbl,
            index => Assert.EndsWith("E", ModelMembers[index]));
    }

    [Fact]
    public void TheDynamiteSelectorOnlyNamesDynamite()
    {
        Assert.All(AppMain.gm_gmk_dynamite_mdl_tbl,
            index => Assert.StartsWith("Z3_DYNAMITE_", ModelMembers[index]));
    }

    // the diagonals are the four entries where the fuse leaves by the edge it came in, and the
    // maker tags exactly those with GMD_GMK_FUSE_DIAG off the same test
    [Fact]
    public void EveryDiagonalSlotHoldsAJumpPiece()
    {
        for (int dir = 0; dir < 4; ++dir)
        {
            Assert.StartsWith("J_", LinePiece(AppMain.gm_gmk_fuse_line_mdl_tbl[dir + 4 * dir]));
        }
    }
}
