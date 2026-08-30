# The pause menu, and its two archives

Working notes. Written while porting the console pause menu, because the two builds do not share a
screen here and nothing in the repo recorded which ids belong to which.

`docs/asset-lineage.md` has the provenance rules. This is what `G_PAUSE` specifically needed.

## Two implementations, one API

`gmPauseMenu.cpp` is 3223 lines wrapped entirely in `#if !_IPHONE`: a D-pad cursor menu with a
confirmation dialog. `gmPauseMenu_i.cpp` is 2064 lines under `#if _IPHONE`, a C++ class `CMain`,
touch-driven, no cursor. Only the second one was ever decompiled, as
`AppMain/Types/CMain_PauseMenu.cs`.

Both export the same eleven functions (`GmPauseMenuLoadStart` through `GmPauseMenuGetResult`), so
`AppMain/Gm/GmPauseMenu.cs` can route between them on one build-time answer.

## The archives are different screens, not different art

Read out of the shipped `.AMA` files, not the generated headers. The leak has no `_XBOX` header set
at all, only `file/Wii/ace` and `file/iPhone/ace`, and the Wii ids are wrong for console (below).

**`G_PAUSE.AMA`** - the plates and the dimmer.

| mobile (10) | xbla (6) |
| --- | --- |
| 0 `back` | 0 `title_base` |
| 1-3 `btn01_l/c/r` | 1 `btn_base` |
| 4-6 `btn03_l/c/r` | 2 `btn_base_s` |
| 7-9 `btn04_l/c/r` | 3 `btn_base_d` |
| | 4 `btn_base_d_s` |
| | 5 `back` |

`back` is the only name in common, and it is index 0 on one side and 5 on the other. Mobile's three
`btnNN_l/c/r` triples are a nine-slice button stretched per label; console has one plate plus a
selected variant plus a disabled pair.

**`G_PAUSE_L.AMA`** - the localised text.

| mobile (10) | xbla (12) |
| --- | --- |
| 0 `tex_retry` | 0 `title_pause` |
| 1 `tex_world` | 1 `btn_back` |
| 2 `tex_main` | 2 `dlg_title_attension` |
| 3 `tex_modoru` | 3 `dlg_title_line` |
| 4 `msg_retry` | 4 `dlg_sel_yes` |
| 5 `msg_world` | 5 `dlg_sel_yes_s` |
| 6 `msg_retry2` | 6 `dlg_sel_no` |
| 7 `msg_main` | 7 `dlg_sel_no_s` |
| 8 `tex_hai` | 8 `msg_retry` |
| 9 `tex_iie` | 9 `msg_world` |
| | 10 `msg_retry2` |
| | 11 `msg_mainmenu` |

Four names in common (`msg_retry`, `msg_world`, `msg_retry2`, and `msg_main`/`msg_mainmenu` if you
count the rename), none at the same index.

**`G_PAUSE_P.AMA`** and **`G_PAUSE_P_L.AMA`** are console-only, no mobile counterpart:

- `G_PAUSE_P.AMA`: one action, `mark_back`.
- `G_PAUSE_P_L.AMA`: sixteen, an eight-pair set. `btn_retry` 0, `btn_option` 2, `btn_rank` 4,
  `btn_trophy` 6, `btn_world` 8, `btn_retry_d` 10, `btn_mmenu` 12, `btn_resume` 14, each followed by
  its `_s` selected twin. `gmPmDrawSelect` relies on that pairing: it draws `act_id_tbl[i]` normally
  and `act_id_tbl[i] + 1` for the highlighted row.

`G_PAUSE_WIN.AMB` is also console-only. It carries no `.AMA`; it is the 32px corner texture
`AoWinSysDrawState` stretches into the confirm dialog's 9-slice frame.

## The Wii headers do not stand in for the missing XBOX ones

`file/Wii/ace/g_pause_p_l.hma` looks like the right header and is not:

| action | Wii | xbla |
| --- | --- | --- |
| `btn_retry` | 0 | 0 |
| `btn_world` | **2** | **8** |
| `btn_retry_d` | **4** | **10** |
| `btn_mmenu` | **6** | **12** |
| `btn_option` | absent | 2 |
| `btn_rank` | absent | 4 |
| `btn_trophy` | absent | 6 |
| `btn_resume` | absent | 14 |

Which follows from the enum: `ACT_PL_BTN_OPTION` is `#if !_WII` and the rank/trophy/resume block is
`#if _PC || _XBOX`. Same story for `g_pause_p.hma`, which has `mark_back_o` and `mark_back_x` where
xbla has one `mark_back` - the O/X split is the PS3 button-glyph swap, and `g_gm_pm_act_id_tbl`
maps both console entries onto the single id.

## Byte order has nothing to hold it

`G_PAUSE.AMA` and `G_PAUSE_L.AMA` are the only loose `.AMA` files in the whole game; everything else
reaches an `.AMA` through an archive. `readAMAFile(string)` therefore has no container to ask, and
passes `bigEndian: false`. The xbla files are big-endian, so the header scoring in `amAmaIsBigEndian`
is the only thing keeping them readable. `AmaActionNameTests` pins that against the shipped files.

The content pipeline did not emit loose `.AMA` at all until this work - `ContentBuild.Archives`
globbed `*.AMB` - so on a clean tree the mobile pause menu threw before any of this. It worked only
because `CopyToOutputDirectory=PreserveNewest` never deletes, and stale copies were still sitting in
`Sonic4Episode1.FNA/bin` alongside `G_PAUSE_KO.AMB` and `G_PAUSE_TW.AMB`, which exist in no source
this repo knows about.

## Six languages, so the fallback is the whole screen

xbla ships JP/US/FR/IT/GE/SP. The WP7 lineage adds FI/PT/RU/CN/HK, and the console arm's
`g_gm_pm_file_lng_amb_name_tbl`, `g_gm_pm_file_plng_amb_name_tbl` and `g_gm_pmenu_bb_width_tbl` are
all six wide. So `amFsAllVariant` drops the whole screen back to the touch menu in those five
locales, the same rule `DmStgSlct` uses and for the same reason. Here it swaps implementations
rather than art, which is only safe because the eleven-function API is identical.

## What opens it, and the sound it does not make

`gmMain.cpp:3252-3254`:

```c
#if !_IPHONE
if ((!AoPadIsConnected() || AoSysIsShowPlatformUI() || (AoPadStand() & KEY_START)) &&
    GmPauseCheckExecutable()) {
#else
if (((GmMainKeyCheckPauseKeyPush() != -1) || gmMainIsSuspendedPause()) && GmPauseCheckExecutable()) {
#endif
```

So console pauses on **START**, on the pad going away, or on the platform UI appearing. `KEY_SELECT`
appears once, at `:3301`, and only under `MTD_DEBUG` - Select is the debug pause.

The C# carried the mobile arm plus two WP7 additions, `isBackKeyPressed()` and `!isForeground`. On
the desktop head `Sonic4Ep1.cs:255` feeds the hardware back key from the pad's **Back** button, so
Select was the only thing that paused until START was added. Back still pauses, and still cancels in
every menu that checks it; dropping that mapping is a separate call.

The confirm window opens silently. `gmPmProcRetryIn`, `gmPmProcBackIn` and `gmPmProcMainMenuIn`
play no SE, and the console SE table (`:2745-2752`) is only `Cursol`, `Ok`, `Cancel`, `Pause`. The
mobile table does list `"Window"` (`gmPauseMenu_i.cpp:1483`) but nothing ever calls `playSe(2)`. The
cue itself is real and every other dialog in the game uses it: `DmOption`, `DmSave`, `DmTitle`,
`DmStaffRoll`. So this is a gap SEGA left in the pause menu specifically, on both branches, not
something the port lost.

## TODO(wam): the retry prompt wording

`gmPauseMenu.cpp:1334-1338` picks `MSG_RETRY2` when the stage is **not** a special stage.
`CMain_PauseMenu.cs:383` picks its slot 6 when it **is** one. Most likely the two archives just
assign opposite wording to the two slots, but that needs looking at rather than reasoning about, and
getting it wrong reads wrong in exactly one of the two cases.
