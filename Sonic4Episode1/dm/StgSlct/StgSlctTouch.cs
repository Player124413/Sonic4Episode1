using System;

using er;

using static AppMain;

using static StgSlct.DME_STGSLCT_ACT;

internal static partial class StgSlct
{
    internal sealed class StgSlctTouch
    {
        public readonly CTrgAoAction[] trg_zone = New<CTrgAoAction>(6);
        public readonly CTrgAoAction[] trg_act = New<CTrgAoAction>(7);
        public readonly CTrgAoAction[] trg_act_tab = New<CTrgAoAction>(6);
        public readonly CTrgAoAction[] trg_act_lr = New<CTrgAoAction>(2);
        public readonly CTrgFlick trg_act_move = new CTrgFlick();
        public readonly CTrgAoAction[] trg_mode = New<CTrgAoAction>(2);
        public readonly CTrgAoAction trg_cancel = new CTrgAoAction();
        public readonly CTrgAoAction[] trg_answer = New<CTrgAoAction>(2);
        public readonly float[] act_tab_state_move_base_pos = new float[2];

        public bool is_disp_cover;
        public bool win_is_disp_cover;

        // cleared each frame in StgSlctInput
        public bool move_left;
        public bool move_right;

        public void Build(DMS_STGSLCT_MAIN_WORK main_work)
        {
            var act = main_work.act;

            AoActUpdate(act[(int)ACT_TAB_STATE_MOVE], 0.0f);
            act_tab_state_move_base_pos[AMD_X] = act[(int)ACT_TAB_STATE_MOVE].sprite.center_x;
            act_tab_state_move_base_pos[AMD_Y] = act[(int)ACT_TAB_STATE_MOVE].sprite.center_y;

            foreach (var trg in trg_zone)
                trg.Create(act[(int)ACT_TAB_ZONE_TAB]);

            foreach (var trg in trg_act)
                trg.Create(act[(int)ACT_TAB_TABLE2]);

            for (int i = 0; i < trg_act_tab.Length; i++)
                trg_act_tab[i].Create(act[(int)ACT_ICON_DOWN_1 + i]);

            trg_act_lr[0].Create(act[(int)ACT_ICON_L_ARROW]);
            trg_act_lr[1].Create(act[(int)ACT_ICON_R_ARROW]);

            trg_act_move.Create(50, 90, AOD_ACT_SCREEN_WIDTH - 100, 258);
            trg_act_move.SetMoveThreshold(4);

            trg_mode[0].Create(act[(int)ACT_TAB_STATE_C]);
            trg_mode[1].Create(act[(int)ACT_TAB_STATE_C2]);

            trg_cancel.Create(act[(int)ACT_BACK_BTN01_R]);

            trg_answer[0].Create(act[(int)ACT_NO_BTN_C]);
            trg_answer[1].Create(act[(int)ACT_YES_BTN_C]);
        }

        public void Release()
        {
            foreach (var trg in trg_zone)
                trg.Release();

            foreach (var trg in trg_act)
                trg.Release();

            foreach (var trg in trg_act_tab)
                trg.Release();

            foreach (var trg in trg_act_lr)
                trg.Release();

            trg_act_move.Release();

            foreach (var trg in trg_mode)
                trg.Release();

            trg_cancel.Release();

            foreach (var trg in trg_answer)
                trg.Release();
        }

        public void ResetAnswers(DMS_STGSLCT_MAIN_WORK main_work)
        {
            for (int i = (int)ACT_YES_BTN_L; i <= (int)ACT_NO_BTN_R; i++)
                AoActSetFrame(main_work.act[i], 0.0f);

            foreach (var trg in trg_answer)
                trg.ResetState();
        }

        // back button lights up under the finger; frame 2 latches until an explicit reset clears it
        public void UpdateBackButtonZoneSelect(DMS_STGSLCT_MAIN_WORK main_work)
        {
            var state = trg_cancel.GetState(0U);
            var frame = main_work.act[(int)ACT_BACK_BTN01_L].frame;

            if (state[CTrgState.EState.Up] && state[CTrgState.EState.Prev])
                frame = 2.0f;
            else if (state[CTrgState.EState.On])
                frame = 1.0f;
            else if (frame < 2.0f)
                frame = 0.0f;

            DrawBackButton(main_work, frame);
        }

        public void UpdateBackButtonStageSelect(DMS_STGSLCT_MAIN_WORK main_work)
        {
            var state = trg_cancel.GetState(0U);
            var frame = main_work.act[(int)ACT_BACK_BTN01_L].frame;

            var dragging = trg_act_move.GetState(0U)[CTrgState.EState.DragAndDrop];

            if (!dragging && state[CTrgState.EState.Lock])
            {
                if (state[CTrgState.EState.Up] && state[CTrgState.EState.Prev])
                    frame = 2.0f;
                else if (state[CTrgState.EState.On])
                    frame = 1.0f;
                else
                    frame = 0.0f;
            }
            else if (frame < 2.0f)
            {
                frame = 0.0f;
            }

            DrawBackButton(main_work, frame);
        }

        public void UpdateModeToggle(DMS_STGSLCT_MAIN_WORK main_work)
        {
            var mode0 = trg_mode[0].GetState(0U);
            var mode1 = trg_mode[1].GetState(0U);
            var dragging = trg_act_move.GetState(0U)[CTrgState.EState.DragAndDrop];

            float frame;

            if (!dragging && (mode0[CTrgState.EState.Lock] || mode1[CTrgState.EState.Lock]))
            {
                if (mode0[CTrgState.EState.Up] && mode0[CTrgState.EState.Prev]
                    || mode1[CTrgState.EState.Up] && mode1[CTrgState.EState.Prev])
                {
                    frame = 2.0f;
                }
                else if (mode0[CTrgState.EState.On] || mode1[CTrgState.EState.On])
                {
                    frame = 1.0f;
                }
                else
                {
                    frame = 0.0f;
                }
            }
            else if (main_work.act[(int)ACT_TAB_STATE_L].frame > 2.0f)
            {
                frame = -1.0f;
            }
            else
            {
                frame = 0.0f;
            }

            if (frame < 0.0f)
                return;

            if (frame == 2.0f)
            {
                main_work.cur_game_mode ^= (uint)DME_STGSLCT_PLAY_MODE.DME_STGSLCT_PLAY_MODE_TIME_ATK;
                DmSoundPlaySE("Cursol");
            }

            for (int i = (int)ACT_TAB_STATE_L; i <= (int)ACT_TAB_STATE_R2; i++)
                AoActSetFrame(main_work.act[i], frame);

            for (int i = (int)ACT_TEX_BIG_TIME; i <= (int)ACT_TEX_BIG_SCORE; i++)
                AoActSetFrame(main_work.act[i], frame);
        }

        public bool PickAct(DMS_STGSLCT_MAIN_WORK main_work, int src)
        {
            var act_num = (int)dm_stgslct_zone_act_num_tbl[main_work.cur_zone][1];

            if (act_num > 3)
                act_num = 3;

            for (int i = main_work.focus_disp_no; i < main_work.focus_disp_no + act_num; i++)
            {
                var act = trg_act[i].GetState(0U);

                if (act[CTrgState.EState.Down])
                {
                    main_work.cur_stage = src + i;
                    is_disp_cover = true;
                }

                if (act[CTrgState.EState.Out])
                    is_disp_cover = false;

                if (act[CTrgState.EState.Click])
                {
                    main_work.cur_stage = src + i;
                    is_disp_cover = true;
                    main_work.flag |= DMD_STGSLCT_FLAG_DECIDE;
                    return true;
                }
            }

            return false;
        }

        public bool PickZoneTab(DMS_STGSLCT_MAIN_WORK main_work)
        {
            var is_open = main_work.is_final_open;
            var moved = false;

            for (int i = 0; i < trg_act_tab.Length; i++)
            {
                var zone_index = i;

                if ((is_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN) != 0
                    && (is_open & DMD_STGSLCT_ZONE_FINAL_OPEN) == 0)
                {
                    if (i == (int)DME_STGSLCT_ZONE_TYPE.DME_STGSLCT_ZONE_TYPE_FINAL)
                        zone_index = (int)DME_STGSLCT_ZONE_TYPE.DME_STGSLCT_ZONE_TYPE_SPE;
                    else if (i == (int)DME_STGSLCT_ZONE_TYPE.DME_STGSLCT_ZONE_TYPE_SPE)
                        continue;
                }

                if (!trg_act_tab[i].GetState(0U)[CTrgState.EState.Down])
                    continue;

                if (main_work.cur_zone == zone_index)
                    continue;

                main_work.chng_zone = main_work.cur_zone;
                main_work.cur_zone = (uint)zone_index;

                if (main_work.chng_zone < main_work.cur_zone)
                    move_right = true;
                else
                    move_left = true;

                moved = true;
            }

            return moved;
        }

        public bool PickZoneArrow(DMS_STGSLCT_MAIN_WORK main_work)
        {
            var dragging = trg_act_move.GetState(0U)[CTrgState.EState.DragAndDrop];
            var moved = false;

            for (int i = 0; i < trg_act_lr.Length; i++)
            {
                var lr = trg_act_lr[i].GetState(0U);

                if (dragging || !lr[CTrgState.EState.Repeat] || !lr[CTrgState.EState.Lock])
                    continue;

                main_work.chng_zone = main_work.cur_zone;
                main_work.cur_zone = (uint)dmStgSlctGetRevisedZoneNo(
                    (int)main_work.cur_zone,
                    i == 0 ? -1 : 1,
                    main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN,
                    main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN);

                if (i == 0)
                    move_left = true;
                else
                    move_right = true;

                moved = true;
            }

            return moved;
        }

        public bool ApplyDrag(DMS_STGSLCT_MAIN_WORK main_work, int src, int dst)
        {
            var state = trg_act_move.GetState(0U);
            var spd = state.GetDragSpeed();

            if (Math.Abs(spd.x) >= 3.0f && Math.Abs(spd.y) < Math.Abs(spd.x))
            {
                // move_left here still advances the zone by +1; that's SEGA's naming, not a bug
                if (spd.x >= 0.0f)
                    move_left = true;
                else
                    move_right = true;

                main_work.chng_zone = main_work.cur_zone;
                main_work.cur_zone = (uint)dmStgSlctGetRevisedZoneNo(
                    (int)main_work.cur_zone,
                    move_left ? 1 : -1,
                    main_work.is_final_open & DMD_STGSLCT_ZONE_FINAL_OPEN,
                    main_work.is_final_open & DMD_STGSLCT_ZONE_SPECIAL_OPEN);

                return false;
            }

            if (state[CTrgState.EState.Up])
            {
                if (state[CTrgState.EState.DragAndDrop])
                    SettleScroll(main_work, src, spd.y);

                return false;
            }

            if (!state[CTrgState.EState.Move])
                return false;

            var move = state.GetLastMove();

            if (move.second == 0)
                return false;

            for (int i = src; i < dst; i++)
                main_work.act_top_pos_y[i] += move.second * 2;

            return true;
        }

        private void SettleScroll(DMS_STGSLCT_MAIN_WORK main_work, int src, float drag_y)
        {
            var move_max = main_work.cur_zone switch
            {
                (uint)DME_STGSLCT_ZONE_TYPE.DME_STGSLCT_ZONE_TYPE_FINAL => 0,
                (uint)DME_STGSLCT_ZONE_TYPE.DME_STGSLCT_ZONE_TYPE_SPE => 4,
                _ => 1,
            };

            var pitch = dm_stgslct_variant.act_table_dist_y - 8.0f;

            var focus = (int)dm_stgslct_variant.act_disp_y_pos_tbl[0]
                - (int)main_work.act_top_pos_y[src]
                + (int)pitch / 2;

            focus -= (int)(drag_y * pitch * 0.25f);
            focus /= (int)pitch;

            main_work.prev_disp_no = main_work.focus_disp_no;
            main_work.focus_disp_no = (int)amClamp(focus, 0, move_max);
            main_work.flag |= DMD_STGSLCT_FLAG_ACT_CHNG_VRTCL;
        }

        // trg_answer[0] is NO and [1] is YES
        public void UpdateAnswerButtons(DMS_STGSLCT_MAIN_WORK main_work)
        {
            UpdateAnswerButton(main_work, trg_answer[1], ACT_YES_BTN_L, ACT_YES_BTN_R, 0,
                DMD_STGSLCT_FLAG_DECIDE);

            UpdateAnswerButton(main_work, trg_answer[0], ACT_NO_BTN_L, ACT_NO_BTN_R, 1,
                DMD_STGSLCT_FLAG_CANCEL);
        }

        private static void UpdateAnswerButton(
            DMS_STGSLCT_MAIN_WORK main_work,
            CTrgAoAction answer,
            DME_STGSLCT_ACT first,
            DME_STGSLCT_ACT last,
            int win_cur_slct,
            int flag)
        {
            var state = answer.GetState(0U);
            float frame;

            if (state[CTrgState.EState.Up] && state[CTrgState.EState.Prev])
            {
                frame = 2.0f;
                main_work.win_cur_slct = win_cur_slct;
                main_work.flag |= (uint)flag;
            }
            else if (state[CTrgState.EState.On])
            {
                frame = 1.0f;
            }
            else if (main_work.act[(int)first].frame >= 2.0f)
            {
                frame = main_work.act[(int)first].frame;
            }
            else
            {
                frame = 0.0f;
            }

            for (int i = (int)first; i <= (int)last; i++)
            {
                AoActSetFrame(main_work.act[i], frame);
                AoActUpdate(main_work.act[i], 0.0f);
            }
        }

        private static void DrawBackButton(DMS_STGSLCT_MAIN_WORK main_work, float frame)
        {
            AoActSetTexture(AoTexGetTexList(main_work.cmn_tex[3]));

            for (int i = (int)ACT_BACK_BTN01_L; i <= (int)ACT_BACK_BTN01_R; i++)
            {
                AoActSetFrame(main_work.act[i], frame);
                AoActUpdate(main_work.act[i], 0.0f);
            }
        }
    }
}
