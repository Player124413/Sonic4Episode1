using System;

namespace Sonic4Episode1.Tests;

// PC's DECO_ZONE1 drops HIMAWARI from MDL member 0 and appends SUNFLOWER at 13, forking the
// id-indexed deco tables on archive tier
//
// gm_deco_as_pc is process wide, so this shares the variant collection.
[Collection(FsVariantCollection.Name)]
public class DecoZone1PcTableTests
{
    private const int Sunflower = 17;
    private const int SunflowerEnding = 111;
    private const int WoodA = 26;
    private const int WoodAEnding = 114;

    private static void AsPc(Action body)
    {
        var was = AppMain.gm_deco_as_pc;
        AppMain.gm_deco_as_pc = true;

        try
        {
            body();
        }
        finally
        {
            AppMain.gm_deco_as_pc = was;
        }
    }

    // the half of each pair that is not measured from PC has to keep matching the static
    // constructor, or the diff the PC rows were derived against has moved under them
    [Fact]
    public void TheBaseRowsStillMatchTheStaticConstructor()
    {
        var ids = AppMain.gm_deco_z1_model_ids;
        for (int i = 0; i < ids.Length; ++i)
            Assert.Equal(AppMain.gm_deco_z1_model_base[i], AppMain.g_gm_deco_model_index[ids[i]][0]);

        ids = AppMain.gm_deco_z1_motion_ids;
        for (int i = 0; i < ids.Length; ++i)
            Assert.Equal(AppMain.gm_deco_z1_motion_base[i], AppMain.g_gm_deco_node_motion_index[ids[i]][0]);

        ids = AppMain.gm_deco_z1_disp_ids;
        for (int i = 0; i < ids.Length; ++i)
            Assert.Equal(AppMain.gm_deco_z1_disp_base[i], AppMain.g_gm_deco_disp_flag[ids[i]]);

        ids = AppMain.gm_deco_z1_user_flag_ids;
        for (int i = 0; i < ids.Length; ++i)
            Assert.Equal(AppMain.gm_deco_z1_user_flag_base[i], AppMain.g_gm_deco_user_flag[ids[i]]);

        ids = AppMain.gm_deco_z1_rect_ids;
        for (int i = 0; i < ids.Length; ++i)
        {
            Assert.Equal(AppMain.gm_deco_z1_rect_base[i][0], AppMain.g_gm_deco_rect_size[ids[i]][0]);
            Assert.Equal(AppMain.gm_deco_z1_rect_base[i][1], AppMain.g_gm_deco_rect_size[ids[i]][1]);
        }
    }

    [Fact]
    public void EveryRowTableIsTheSameLength()
    {
        Assert.Equal(AppMain.gm_deco_z1_model_ids.Length, AppMain.gm_deco_z1_model_base.Length);
        Assert.Equal(AppMain.gm_deco_z1_model_ids.Length, AppMain.gm_deco_z1_model_pc.Length);
        Assert.Equal(AppMain.gm_deco_z1_motion_ids.Length, AppMain.gm_deco_z1_motion_base.Length);
        Assert.Equal(AppMain.gm_deco_z1_motion_ids.Length, AppMain.gm_deco_z1_motion_pc.Length);
        Assert.Equal(AppMain.gm_deco_z1_disp_ids.Length, AppMain.gm_deco_z1_disp_base.Length);
        Assert.Equal(AppMain.gm_deco_z1_disp_ids.Length, AppMain.gm_deco_z1_disp_pc.Length);
        Assert.Equal(AppMain.gm_deco_z1_user_flag_ids.Length, AppMain.gm_deco_z1_user_flag_base.Length);
        Assert.Equal(AppMain.gm_deco_z1_user_flag_ids.Length, AppMain.gm_deco_z1_user_flag_pc.Length);
        Assert.Equal(AppMain.gm_deco_z1_rect_ids.Length, AppMain.gm_deco_z1_rect_base.Length);
        Assert.Equal(AppMain.gm_deco_z1_rect_ids.Length, AppMain.gm_deco_z1_rect_pc.Length);
    }

    [Fact]
    public void TheSunflowerMovesToTheEndOfTheModelArchive()
    {
        AsPc(() =>
        {
            Assert.Equal(13, AppMain.gmDecoModelIndex(Sunflower, 0));
            Assert.Equal(13, AppMain.gmDecoModelIndex(SunflowerEnding, 0));
        });
    }

    // HIMAWARI leaving member 0 is the whole reason this is 25 rows and not 2
    [Fact]
    public void EveryOtherZone1ModelShiftsDownOne()
    {
        var ids = AppMain.gm_deco_z1_model_ids;

        AsPc(() =>
        {
            for (int i = 0; i < ids.Length; ++i)
            {
                if (ids[i] == Sunflower || ids[i] == SunflowerEnding)
                    continue;

                Assert.Equal(AppMain.gm_deco_z1_model_base[i] - 1, AppMain.gmDecoModelIndex(ids[i], 0));
            }
        });
    }

    [Fact]
    public void TheSunflowerAndTheLogSwapMotionMembers()
    {
        AsPc(() =>
        {
            Assert.Equal(1, AppMain.gmDecoNodeMotionIndex(Sunflower, 0));
            Assert.Equal(1, AppMain.gmDecoNodeMotionIndex(SunflowerEnding, 0));
            Assert.Equal(0, AppMain.gmDecoNodeMotionIndex(WoodA, 0));
            Assert.Equal(0, AppMain.gmDecoNodeMotionIndex(WoodAEnding, 0));
        });
    }

    // bit 4 becomes bit 2 in disp_flag and bit 10 in user_flag; bit 22 and the ending bit 0 stay
    [Fact]
    public void TheSunflowerFlagsMoveOffBitFour()
    {
        AsPc(() =>
        {
            Assert.Equal(0x00400004u, AppMain.gmDecoDispFlag(Sunflower));
            Assert.Equal(0x00400004u, AppMain.gmDecoDispFlag(SunflowerEnding));
            Assert.Equal(0x400u, AppMain.gmDecoUserFlag(Sunflower));
            Assert.Equal(0x401u, AppMain.gmDecoUserFlag(SunflowerEnding));
        });
    }

    // the width is unchanged and the height halves, because PC also anchors the rect above the
    // origin instead of centring it on it
    [Fact]
    public void EveryTouchRectHeightHalvesAndNoWidthMoves()
    {
        var ids = AppMain.gm_deco_z1_rect_ids;

        AsPc(() =>
        {
            for (int i = 0; i < ids.Length; ++i)
            {
                Assert.Equal(AppMain.gm_deco_z1_rect_base[i][0], AppMain.gmDecoRectSize(ids[i], 0));
                Assert.Equal(AppMain.gm_deco_z1_rect_base[i][1] / 2, AppMain.gmDecoRectSize(ids[i], 1));
            }
        });
    }

    // slot 1 is -1 on both builds for every overridden row, which is why the pair tables carry one
    // column. If PC ever needs a submodel here the accessor has to grow a second column first
    [Fact]
    public void SlotOneNeverForks()
    {
        AsPc(() =>
        {
            foreach (var id in AppMain.gm_deco_z1_model_ids)
                Assert.Equal(AppMain.g_gm_deco_model_index[id][1], AppMain.gmDecoModelIndex(id, 1));

            foreach (var id in AppMain.gm_deco_z1_motion_ids)
                Assert.Equal(AppMain.g_gm_deco_node_motion_index[id][1], AppMain.gmDecoNodeMotionIndex(id, 1));
        });
    }

    // the falls and the glare quads read this slot as an effect index rather than a model member,
    // and DECO_ZONE1_MDL_FALL's member order is identical between the builds, so they must not move
    [Fact]
    public void TheFallAndGlareIdsAreUntouchedOnThePcTier()
    {
        int[] untouched = { 14, 15, 16, 20, 27, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 109, 110 };

        AsPc(() =>
        {
            foreach (var id in untouched)
            {
                Assert.Equal(AppMain.g_gm_deco_model_index[id][0], AppMain.gmDecoModelIndex(id, 0));
                Assert.Equal(AppMain.g_gm_deco_disp_flag[id], AppMain.gmDecoDispFlag(id));
                Assert.Equal(AppMain.g_gm_deco_user_flag[id], AppMain.gmDecoUserFlag(id));
            }
        });
    }

    // zone 2's PC-only ids come from GmDecoPcTables and have no zone 1 override, so the deco tier
    // must not disturb them
    [Fact]
    public void ThePcOnlyCardIdsAreUntouched()
    {
        AsPc(() =>
        {
            for (int id = 184; id < AppMain.GMD_DECORATE_ID_MAX_PC; ++id)
            {
                Assert.Equal(AppMain.g_gm_deco_model_index[id][0], AppMain.gmDecoModelIndex(id, 0));
                Assert.Equal(AppMain.g_gm_deco_node_motion_index[id][0], AppMain.gmDecoNodeMotionIndex(id, 0));
                Assert.Equal(AppMain.g_gm_deco_rect_size[id][0], AppMain.gmDecoRectSize(id, 0));
                Assert.Equal(AppMain.g_gm_deco_rect_size[id][1], AppMain.gmDecoRectSize(id, 1));
            }
        });
    }

    // PC anchors the rect above the origin, but the touch constructor is shared with ids outside
    // zone 1, so the fork keys off having a measured PC size, not off the tier itself
    [Fact]
    public void OnlyTheIdsWithAMeasuredPcSizeGetThePcAnchor()
    {
        AsPc(() =>
        {
            AppMain.gmDecoTouchRectBounds(Sunflower, out var left, out var top, out var right, out var bottom);
            Assert.Equal(-8, left);
            Assert.Equal(8, right);
            Assert.Equal(-64, top);
            Assert.Equal(0, bottom);

            // 170 and 171 share GmDecoInitModelMotionMaterialTouch with nothing in zone 1, and
            // their size is still mobile's, so they must keep the centred form
            foreach (var id in new[] { 170, 171 })
            {
                AppMain.gmDecoTouchRectBounds(id, out var l, out var t, out var r, out var b);
                Assert.Equal(-AppMain.g_gm_deco_rect_size[id][1] / 2, t);
                Assert.Equal(AppMain.g_gm_deco_rect_size[id][1] / 2, b);
                Assert.Equal(-AppMain.g_gm_deco_rect_size[id][0] / 2, l);
                Assert.Equal(AppMain.g_gm_deco_rect_size[id][0] / 2, r);
            }
        });
    }

    // same top edge as mobile's, and nothing below the origin, which is what halving the height
    // buys. Pinned for all four rather than just the sunflower because the log shares the change
    [Fact]
    public void ThePcAnchorKeepsMobilesTopEdge()
    {
        var ids = AppMain.gm_deco_z1_rect_ids;

        AsPc(() =>
        {
            for (int i = 0; i < ids.Length; ++i)
            {
                AppMain.gmDecoTouchRectBounds(ids[i], out _, out var top, out _, out var bottom);

                Assert.Equal(-(AppMain.gm_deco_z1_rect_base[i][1] / 2), top);
                Assert.Equal(0, bottom);
            }
        });
    }

    [Fact]
    public void OffThePcTierEveryRectIsStillCentred()
    {
        var was = AppMain.gm_deco_as_pc;
        AppMain.gm_deco_as_pc = false;

        try
        {
            for (int id = 0; id < AppMain.GMD_DECORATE_ID_MAX_PC; ++id)
            {
                AppMain.gmDecoTouchRectBounds(id, out var left, out var top, out var right, out var bottom);

                Assert.Equal(-AppMain.g_gm_deco_rect_size[id][0] / 2, left);
                Assert.Equal(-AppMain.g_gm_deco_rect_size[id][1] / 2, top);
                Assert.Equal(AppMain.g_gm_deco_rect_size[id][0] / 2, right);
                Assert.Equal(AppMain.g_gm_deco_rect_size[id][1] / 2, bottom);
            }
        }
        finally
        {
            AppMain.gm_deco_as_pc = was;
        }
    }

    // only the sunflower's callback forks. The log keeps gmDecoRectFuncChangeMotionCount on both
    // builds, and nothing outside zone 1 moves at all
    [Fact]
    public void OnlyTheSunflowerSwapsItsTouchCallback()
    {
        AsPc(() =>
        {
            Assert.NotSame(AppMain.g_gm_deco_func_rect[Sunflower], AppMain.gmDecoFuncRect(Sunflower));
            Assert.NotSame(AppMain.g_gm_deco_func_rect[SunflowerEnding], AppMain.gmDecoFuncRect(SunflowerEnding));

            for (int id = 0; id < AppMain.GMD_DECORATE_ID_MAX_PC; ++id)
            {
                if (id == Sunflower || id == SunflowerEnding)
                    continue;

                Assert.Same(AppMain.g_gm_deco_func_rect[id], AppMain.gmDecoFuncRect(id));
            }
        });
    }

    [Fact]
    public void OffThePcTierNoCallbackForks()
    {
        var was = AppMain.gm_deco_as_pc;
        AppMain.gm_deco_as_pc = false;

        try
        {
            for (int id = 0; id < AppMain.GMD_DECORATE_ID_MAX_PC; ++id)
                Assert.Same(AppMain.g_gm_deco_func_rect[id], AppMain.gmDecoFuncRect(id));
        }
        finally
        {
            AppMain.gm_deco_as_pc = was;
        }
    }

    // off the tier every accessor has to be exactly the table read it replaced, for every id
    [Fact]
    public void OffThePcTierTheAccessorsAreTheTableRead()
    {
        var was = AppMain.gm_deco_as_pc;
        AppMain.gm_deco_as_pc = false;

        try
        {
            for (int id = 0; id < AppMain.GMD_DECORATE_ID_MAX_PC; ++id)
            {
                Assert.Equal(AppMain.g_gm_deco_model_index[id][0], AppMain.gmDecoModelIndex(id, 0));
                Assert.Equal(AppMain.g_gm_deco_model_index[id][1], AppMain.gmDecoModelIndex(id, 1));
                Assert.Equal(AppMain.g_gm_deco_node_motion_index[id][0], AppMain.gmDecoNodeMotionIndex(id, 0));
                Assert.Equal(AppMain.g_gm_deco_node_motion_index[id][1], AppMain.gmDecoNodeMotionIndex(id, 1));
                Assert.Equal(AppMain.g_gm_deco_disp_flag[id], AppMain.gmDecoDispFlag(id));
                Assert.Equal(AppMain.g_gm_deco_user_flag[id], AppMain.gmDecoUserFlag(id));
                Assert.Equal(AppMain.g_gm_deco_rect_size[id][0], AppMain.gmDecoRectSize(id, 0));
                Assert.Equal(AppMain.g_gm_deco_rect_size[id][1], AppMain.gmDecoRectSize(id, 1));
            }
        }
        finally
        {
            AppMain.gm_deco_as_pc = was;
        }
    }
}
