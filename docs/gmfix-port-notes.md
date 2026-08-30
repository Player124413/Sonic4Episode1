# The HUD

Working notes from porting `GmFix`. It owns two of `CPIT_MAIN`'s members outright and fourteen more
through `SPST`, and it is the most heavily `#ifdef`'d screen in the archive: 28 conditional blocks
against `GmStartDemo`'s one.

`docs/per-member-variants.md` has the mechanism; this is what `G_FIX` specifically needed.

## SPST belongs here, not to GmSplStage

`gmFix.cpp:39` includes `SPST.HMA`, and `gm_fix_tex_amb_idx_tbl` is
`{G_FIX.AMB, SPST.AMB, SPST_<lang>.AMB}`. `SPST` is the special stage's ring and time overlay, drawn
by `gmFixTimerSSPartInit`, which is its only consumer in either tree. `gmSplStage.cpp` references no
archive ids at all; its `CPIT_MAIN.HMB` include is vestigial.

`SPST` has seven actions and every `SPST_<lang>` exactly one (`tex_time`), which is why the
decompile's `gm_fix_timer_ss_act_id_tbl` was eleven identical rows.

## The id divergence is two indices

`file/common/ace/G_FIX.HMA` has 45 ids, `file/iPhone/ace/G_FIX.HMA` has 64:

- **0-28 identical**
- **29 and 30 transposed.** common `{29 TA_NUM_C_1000, 30 TA_NUM_C_A}`, iPhone the reverse
- **31-44 identical**
- **iPhone appends 45-63**: `ICON_SUPER01`, `START`, `CROSS`, `JUMP`, then the `DA_*` set

`gm_fix_timer_timeattack_act_id_tbl` is `{27, 28, 29, 30, 31, 32, 33, 34}` and is the only table
that straddles the pair. Subparts are positional - slot *k* gets the action at table position *k* -
so the console arm is the same list with those two entries swapped, putting the same sprite in the
same slot. This is the one thing here that would draw a correct-looking wrong sprite: the time
attack counter's thousands digit and the apostrophe between minutes and seconds trading places.

## The plan is a mobile-only concept

`gmFixGetPlan()` picks one of three HUD layouts:

| plan | when | what it changes |
| --- | --- | --- |
| `TILT` (0) | default | ids 22-26 and 36-44, all shared |
| `FLICK` (1) | `INPUT_IS_FLICK` set, on the truck stage or a special stage | the `DA_*` ids 49-62 |
| `VIRTUAL_PAD` (2) | `INPUT_CLASSIC` set, anywhere else | same, plus the d-pad at 47 |

`DA_*` is a second copy of the challenge and score digits, repositioned to clear the on-screen
controls.

**`gmFixGetPlan`, `GME_FIX_PLAN` and every plan-indexed table are `#if _IPHONE`.** The console build
has no plan at all: `gm_fix_score_stage22_act_id_tbl` and `gm_fix_challenge_act_id_tbl` are declared
as flat one-dimensional arrays there, not `[GME_FIX_PLAN_MAX][...]`. Only the virtual pad reaches
the plan, and that part does not exist on console either.

Getting this wrong is what crashed the first attempt. The reasoning was that both non-zero plans
need a mobile control mode, so console would land on plan 0 by itself. It does not: `INPUT_CLASSIC`
selects plan 2, and `StgSlct.dmStgSlctSetInputFlag()` sets `INPUT_CLASSIC` *precisely when the stage
select ran on console assets*. `gmFixChallengePartInit` asked for action 49 against a 45-action
archive, `AoActCreate` returned null, and it fell over in `ObjDrawAction2DAMA` on the first HUD
draw, three frames from the cause.

Forcing plan 0 on console would have stopped the crash and still been wrong, because the flat
console tables are not the mobile plan-0 rows:

| table | console | mobile plan 0 |
| --- | --- | --- |
| `gm_fix_challenge_act_id_tbl` | `{22, 23, 24, 25, 26}` | identical |
| `gm_fix_score_stage22_act_id_tbl` | `{ICON_SCORE(35), 36..44}` | `{-1, 36..44}` |

Console draws the score icon on act 2-2 time attack; mobile draws nothing there. Borrowing plan 0
would have lost the icon silently.

One SEGA quirk left alone: on the truck stage and the special stages, `classic` can never produce
plan 2, so the mobile d-pad never appears there.

## The rest of the ifdefs

Four more sites change behaviour, all carried on `GMS_FIX_VARIANT`:

| C++ | console | mobile |
| --- | --- | --- |
| `GmFixSetDisp` (`:1019`) | sets `HIDE` and nothing else | delegates to `GmFixSetDispEx` |
| `GmFixSetDispEx` (`:1037`) | does not exist | exists |
| score narrowscreen offset (`:1873`) | live: `if (!TimeAttack && !wide_screen)` shift left | absent |
| timer time-attack `pos.x += -98` (`:2065`) | not applied | applied |

`GmFixSetDispEx` stays public because `GmEnding.cs` calls it; on console the four pad flags it
writes are inert. The narrowscreen offset was dropped in the first pass on the grounds that the
decompile had reduced it to `num = 0; pos.x += num;` - that is just the mobile arm, and the console
one is live in the reference source.

What stays unconditional, because the console arm only loses an unused name: the
`HIDE_VIRTUAL_PAD_PART_*` flags, `GME_FIX_PLAN`, `GME_FIX_VIRTUAL_PAD_SUBPART`,
`GMS_FIX_PART_VIRTUAL_PAD`, the fifth `GME_FIX_PART_TYPE` slot, and the fifth entry of each
part-init table. On console the pad part returns immediately, so slot 4 stays null and
`gmFixProcMain` skips it.

## The guard

`gmFixCreateSubpart` refuses an action the loaded archive does not have rather than letting
`AoActCreate` store a null `act` for the draw path to trip over later. It costs one
`ObjDataLoadAmbIndex` per subpart, which is an `amb.buf[index]` hit after the first. The crash this
replaces surfaced three frames and two call stacks away from its cause.

## Structural work

The decompile emulated the C++ "first member is the base struct" cast with a `readonly object
holder` on `GMS_FIX_PART_WORK`, a `part_work` back-reference on each of the five part types, and
eleven `explicit operator` overloads between them. Inheritance says the same thing; all of it went.

The four `gmFixVirtualPadOutClass*` type files were one C++ `template<u32 FLAG>` rendered as four
one-method classes differing only in the flag tested. They are now one function taking the mask.

`act_id_tblgmFixScorePartInit`, `act_id_tblgmFixTimerPartInit` and
`digit_num_listgmFixTimerPartUpdateDigitList` were C++ function-local statics hoisted to class
scope, and the first two were written at runtime. They are locals again.

## A quirk reproduced, not fixed

`gmFixTimerPartUpdateFlashAction` phase 0 does `scale_ratio = MTM_MATH_CLIP(fade_ratio, 0, 1)` where
phase 1 clips `scale_ratio` correctly, so the grow phase tracks the fade's 0.5/frame rather than
accumulating its own 0.25. Present in `gmClearDemo`'s sibling code too. Kept, because the C++ wins
and that is what shipped.
