using System.Linq;

namespace Sonic4Episode1.Tests;

// GmGameDatLoadInit walks seven tables into one fixed context array and never checks the bound, so
// a stage whose loadout outgrows it dies with an IndexOutOfRange on the frame the stage loads and
// says nothing about which table did it. That is not hypothetical: adding TRUMPSTAGE's eleven
// archives took PC-era Z2A2 past the mobile-era 96 on 2026-09-02.
public class GamedatLoadContextTests
{
    // the four per-stage tables are not all the same length, so the shortest is what can be walked
    private static readonly int STAGE_MAX = new[]
    {
        AppMain.gm_gamedat_tbl_map_info_tbl.Length,
        AppMain.gm_gamedat_tbl_effect_info_tbl.Length,
        AppMain.gm_gamedat_tbl_enemy_info_tbl.Length,
        AppMain.gm_gamedat_tbl_gimmick_info_tbl.Length,
    }.Min();

    private const int Z2A2 = 5;

    // GmMain always passes one player, and stage 16 skips the enemy table
    private const int NO_ENEMIES_STAGE = 16;

    private static int ContextsFor(int stage_id, bool pc_level)
    {
        return Breakdown(stage_id, pc_level).Sum(part => part.Value);
    }

    // in the order GmGameDatLoadInit walks them, so a failure names the table that grew
    private static (string Name, int Value)[] Breakdown(int stage_id, bool pc_level)
    {
        bool pc_card_stage = pc_level && stage_id == Z2A2;

        return new[]
        {
            ("common", AppMain.gm_gamedat_tbl_common_info_tbl[0].num),
            ("player", AppMain.gm_gamedat_tbl_player_info_tbl.Max(info => info.num)),
            // zone 2 splices in the two motion sidecars whenever its art is console-grade, which
            // is every tree that can reach these levels at all
            ("map", AppMain.gm_gamedat_tbl_map_info_tbl[stage_id].num + (stage_id >= 4 && stage_id <= 7 ? 2 : 0)),
            ("effect", AppMain.gm_gamedat_tbl_effect_info_tbl[stage_id].num),
            ("enemy", stage_id == NO_ENEMIES_STAGE
                ? 0
                : pc_card_stage
                    ? AppMain.gm_gamedat_tbl_enemy02_02_pc.Length
                    : AppMain.gm_gamedat_tbl_enemy_info_tbl[stage_id].num),
            ("gimmick common", AppMain.gm_gamedat_tbl_gimmick_common_info_tbl[0].num),
            ("gimmick", pc_card_stage
                ? AppMain.gm_gamedat_tbl_gimmick02_02_pc.Length
                : AppMain.gm_gamedat_tbl_gimmick_info_tbl[stage_id].num),
        };
    }

    private static string Describe(int stage_id, bool pc_level)
    {
        return string.Join(" + ", Breakdown(stage_id, pc_level).Select(p => $"{p.Name} {p.Value}"));
    }

    [Fact]
    public void EveryStageLoadoutFitsTheContextArray()
    {
        for (int stage_id = 0; stage_id < STAGE_MAX; ++stage_id)
        {
            int needed = ContextsFor(stage_id, false);

            Assert.True(needed <= AppMain.GMD_GAMEDAT_LOAD_CONTEXT_MAX,
                $"stage {stage_id} needs {needed} of {AppMain.GMD_GAMEDAT_LOAD_CONTEXT_MAX}: {Describe(stage_id, false)}");
        }
    }

    // Z2A2 on a PC-era level swaps in two wider tables, and it is the biggest loadout in the game
    // by a distance: three card families on top of a zone 2 act
    [Fact]
    public void ThePcCardStageFitsTheContextArray()
    {
        int needed = ContextsFor(Z2A2, true);

        Assert.True(needed <= AppMain.GMD_GAMEDAT_LOAD_CONTEXT_MAX,
            $"PC Z2A2 needs {needed} of {AppMain.GMD_GAMEDAT_LOAD_CONTEXT_MAX}: {Describe(Z2A2, true)}");
    }

    // the card stage is what sets the size, so say by how much rather than leaving the next person
    // to find out by crashing
    [Fact]
    public void ThePcCardStageIsTheStageThatSetsTheSize()
    {
        int pc_card_stage = ContextsFor(Z2A2, true);

        for (int stage_id = 0; stage_id < STAGE_MAX; ++stage_id)
            Assert.True(ContextsFor(stage_id, false) <= pc_card_stage,
                $"stage {stage_id} now outgrows PC Z2A2, so the headroom below is measured wrong");

        Assert.True(AppMain.GMD_GAMEDAT_LOAD_CONTEXT_MAX - pc_card_stage >= 8,
            $"only {AppMain.GMD_GAMEDAT_LOAD_CONTEXT_MAX - pc_card_stage} spare contexts left");
    }
}
