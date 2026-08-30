using System;
using System.Collections.Generic;
using er;
using er.web;
using gs.backup;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

using static AppMain;

internal static partial class DmTitle
{
    // the archive path is the same either way, amFsResolve decides which build's data lands there.
    // these tables describe that data, so they stay paired. picked once and not asked about again.
    internal sealed class DMS_TITLE_VARIANT
    {
        public uint[] act_id_tbl;
        public int[] act_slot_tbl;
        public float[,] win_act_pos_tbl;
        public DMS_TITLE_MAIN_WORK._proc_ proc_win_input;

        public int act_num;

        // The two ACT enums share member names but not values. Shared code names actions with the
        // console enum, which is the superset for everything shared code touches, and this maps
        // that name onto whichever index the running variant uses. -1 for an action the variant
        // does not have, which throws on use rather than quietly indexing the wrong sprite.
        public int[] act_index;

        public float window_size_w;
        public float disp_scale_text;
        public float crsr_dflt_pos_x;

        public float[] crsr_trial_pos_y_tbl;

        public int slct_menu_num_first;
        public int slct_menu_num_played;
        public int slct_menu_num_trial;

        // indexed by DME_TITLE_MENU_TYPE, so by the full menu, not by cursor position
        public DME_TITLE_MENU_ACTION[] menu_actions;

        public float mmenu_win_size_w;
        public float mmenu_win_size_h;

        // the second main-menu window size exists only on mobile; DMD_TITLE_MMENU_WIN_SIZE_{W,H}_2
        // are declared inside the _IPHONE arm and have no console counterpart
        public float? mmenu_win_size_w2;
        public float? mmenu_win_size_h2;

        public bool is_console;

        // what every archive this screen loads gets pinned to. the global am_fs_variant cannot be
        // trusted here: it stays "console" even when this screen has fallen back to mobile
        public string fs_variant;
    }

    private static DMS_TITLE_VARIANT dm_title_variant;

    // shared draw code names actions with the console enum and goes through these
    private static int dmTitleActIdx(DME_TITLE_ACT_REG key)
    {
        return dm_title_variant.act_index[(int)key];
    }

    // The cursor walks the items a build actually shows, which is fewer than the menu has whenever
    // TUDUKI is hidden, so cursor position is not menu slot. dmTitle.cpp:1266-1279 reconciles them
    // by adding one past the first item. Both dmTitleSetNextEvent and the decide effect need this
    // and used to disagree: the decide effect shifted and dmTitleSetNextEvent did not.
    private static int dmTitleCrsrToMenuSlot(DMS_TITLE_MAIN_WORK main_work)
    {
        if (main_work.cur_slct_menu > 0 && (GsTrialIsTrial() || main_work.is_init_play))
            return main_work.cur_slct_menu + 1;

        return main_work.cur_slct_menu;
    }

    private static DME_TITLE_MENU_ACTION dmTitleMenuAction(DMS_TITLE_MAIN_WORK main_work)
    {
        return dm_title_variant.menu_actions[dmTitleCrsrToMenuSlot(main_work)];
    }

    private static int[] dmTitleBuildActIndex(System.Type target)
    {
        var keys = (int)DME_TITLE_ACT_REG.ACT_NUM + 1;
        var index = new int[keys];

        for (int key = 0; key < keys; key++)
        {
            var name = System.Enum.GetName(typeof(DME_TITLE_ACT_REG), key);

            index[key] = name != null && System.Enum.IsDefined(target, name)
                ? (int)System.Enum.Parse(target, name)
                : -1;
        }

        return index;
    }

    // (slot, end) runs, each covering up to but not including `end`
    private static int[] BuildSlots(params (int Slot, int End)[] runs)
    {
        var slots = new int[runs[runs.Length - 1].End];
        var start = 0;
        foreach (var run in runs)
        {
            for (int i = start; i < run.End; i++)
                slots[i] = run.Slot;

            start = run.End;
        }

        return slots;
    }
    internal static DMS_TITLE_VARIANT TitleVariantForTest(bool iphone)
    {
        return iphone ? dm_title_variant_iphone : dm_title_variant_pc;
    }
    // five of the eleven languages have no console archive, so this is not always true on desktop
    private static bool dmTitleIsConsoleDataComplete()
    {
        var lng = GsEnvGetLanguage();

        return amFsAllVariant(
            AMD_FS_VARIANT_CONSOLE,
            DMD_TITLE_DATA_FILENAME,
            dm_title_file_lng_amb_name_tbl[lng],
            dm_title_menu_cmn_lng_amb_name_tbl[lng],
            dm_title_menu_cmn_amb_name_tbl[0],
            dm_title_menu_cmn_amb_name_tbl[1],
            dm_title_menu_cmn_amb_name_tbl[2]);
    }
}
