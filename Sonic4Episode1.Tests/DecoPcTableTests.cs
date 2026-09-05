namespace Sonic4Episode1.Tests;

public class DecoPcTableTests
{
    private const int Start = 184;
    private const int End = AppMain.GMD_DECORATE_ID_MAX_PC;

    // every table the dispatch indexes by id has to grow together, or the shortest one throws before the id bound is tested
    [Fact]
    public void EveryIdIndexedTableIsWidenedTogether()
    {
        Assert.Equal(End, AppMain.g_gm_decorate_tbl.Length);
        Assert.Equal(End, AppMain.g_gm_decorate_size_tbl.Length);
        Assert.Equal(End, AppMain.g_gm_deco_model_index.Length);
        Assert.Equal(End, AppMain.g_gm_deco_node_motion_index.Length);
        Assert.Equal(End, AppMain.g_gm_deco_mat_motion_index.Length);
        Assert.Equal(End, AppMain.g_gm_deco_disp_flag.Length);
        Assert.Equal(End, AppMain.g_gm_deco_user_work.Length);
        Assert.Equal(End, AppMain.g_gm_deco_user_timer.Length);
        Assert.Equal(End, AppMain.g_gm_deco_user_flag.Length);
        Assert.Equal(End, AppMain.g_gm_deco_func_main.Length);
        Assert.Equal(End, AppMain.g_gm_deco_func_move.Length);
        Assert.Equal(End, AppMain.g_gm_deco_func_rect.Length);
        Assert.Equal(End, AppMain.g_gm_deco_func_out.Length);
        Assert.Equal(End, AppMain.g_gm_deco_func_dest.Length);
        Assert.Equal(End, AppMain.g_gm_deco_pos.Length);
        Assert.Equal(End, AppMain.g_gm_deco_rot_z.Length);
        Assert.Equal(End, AppMain.g_gm_deco_rect_size.Length);
        Assert.Equal(End, AppMain.g_gm_deco_command_state.Length);
    }

    [Fact]
    public void TheMobileEntriesSurviveTheResize()
    {
        Assert.Equal(0, AppMain.g_gm_deco_model_index[17][0]);
        Assert.Equal(8, AppMain.g_gm_deco_model_index[26][0]);
        Assert.Equal(4194304u, AppMain.g_gm_deco_disp_flag[0]);
    }

    // DECO_ZONE2.AMB member order on PC: 11 members against mobile's 2. 184/185 are static scenery,
    // then dice and trump cards, and the set repeats from 193 with a different touch response
    [Theory]
    [InlineData(184, 5)]
    [InlineData(185, 6)]
    [InlineData(186, 2)]
    [InlineData(187, 3)]
    [InlineData(188, 4)]
    [InlineData(189, 7)]
    [InlineData(190, 8)]
    [InlineData(191, 9)]
    [InlineData(192, 10)]
    [InlineData(193, 2)]
    [InlineData(194, 3)]
    [InlineData(195, 4)]
    [InlineData(196, 7)]
    [InlineData(197, 8)]
    [InlineData(198, 9)]
    [InlineData(199, 10)]
    public void TheModelIndexTableMatchesThePcMemberOrder(int id, int member)
    {
        Assert.Equal(member, AppMain.g_gm_deco_model_index[id][0]);
    }

    // the three dice tumbles are the only node motions in the block
    [Theory]
    [InlineData(186, 0)]
    [InlineData(187, 1)]
    [InlineData(188, 2)]
    [InlineData(193, 0)]
    [InlineData(194, 1)]
    [InlineData(195, 2)]
    public void TheDiceCarryANodeMotion(int id, int motion)
    {
        Assert.Equal(motion, AppMain.g_gm_deco_node_motion_index[id][0]);
    }

    [Theory]
    [InlineData(184)]
    [InlineData(189)]
    [InlineData(192)]
    [InlineData(196)]
    [InlineData(199)]
    public void EverythingElseHasNoNodeMotion(int id)
    {
        Assert.Equal(-1, AppMain.g_gm_deco_node_motion_index[id][0]);
    }

    // bit 22 is what suppresses the automatic quarter turn about Y in ObjDrawAction3DNN
    [Fact]
    public void EveryPcIdSuppressesTheAutomaticYRotation()
    {
        for (int id = Start; id < End; ++id)
            Assert.True((AppMain.g_gm_deco_disp_flag[id] & 0x00400000u) != 0, $"id {id}");
    }

    // user_flag 0x800 is an authored quarter turn about Z, applied in gmDecoLoadObj. The second
    // group of card and dice ids sets it and the first does not, which is the whole difference
    // between the two otherwise identical halves of the block
    [Fact]
    public void OnlyTheSecondGroupTakesTheAuthoredQuarterTurn()
    {
        for (int id = Start; id < 193; ++id)
            Assert.Equal(0u, AppMain.g_gm_deco_user_flag[id] & 0x800u);

        for (int id = 193; id < End; ++id)
            Assert.Equal(0x800u, AppMain.g_gm_deco_user_flag[id] & 0x800u);
    }

    // the static pair has no touch box and no per-frame work; everything else has both
    [Fact]
    public void OnlyTheStaticPairIsInert()
    {
        for (int id = Start; id < End; ++id)
        {
            bool statik = id < 186;

            Assert.Equal(statik, AppMain.g_gm_deco_func_main[id] == null);
            Assert.Equal(statik, AppMain.g_gm_deco_func_rect[id] == null);
            Assert.Equal(statik ? 0 : 16, AppMain.g_gm_deco_rect_size[id][0]);
            Assert.Equal(statik ? 0 : 32, AppMain.g_gm_deco_rect_size[id][1]);
        }
    }

    // the spin impulse scales the toucher's speed by this; 0 on the static pair
    [Theory]
    [InlineData(184, 0u)]
    [InlineData(186, 15u)]
    [InlineData(189, 20u)]
    [InlineData(195, 15u)]
    [InlineData(199, 20u)]
    public void TheSpinScaleMatchesPc(int id, uint user_work)
    {
        Assert.Equal(user_work, AppMain.g_gm_deco_user_work[id]);
    }

    [Fact]
    public void EveryPcIdUsesTheSameClipSize()
    {
        for (int id = Start; id < End; ++id)
            Assert.Equal(64, AppMain.g_gm_decorate_size_tbl[id]);
    }

    [Fact]
    public void EveryPcIdHasAConstructor()
    {
        for (int id = Start; id < End; ++id)
            Assert.NotNull(AppMain.g_gm_decorate_tbl[id]);
    }
}
