# Stage select: the two action id spaces

Measured 2026-08-30 while planning the `DmStgSlct` re-port. The point of the exercise was to find out
whether the generated `.HMA` headers in the leaked dump can be trusted to describe the archives we
actually hold, before hand-writing two id tables off them.

They can. All four stage select archives match their headers exactly, name for name, id for id.

## What was compared

| header | archive |
| --- | --- |
| `file/common/ace/D_STGSLCT.HMA` (89) | XBLA beta `DEMO/STGSLCT/D_STGSLCT.AMB` |
| `file/common/ace/D_STGSLCT_JP.HMA` (20) | XBLA beta `DEMO/STGSLCT/D_STGSLCT_US.AMB` |
| `file/iPhone/ace/D_STGSLCT.HMA` (80) | this repo's `Content/DEMO/STGSLCT/D_STGSLCT.AMB` |
| `file/iPhone/ace/D_STGSLCT_JP.HMA` (13) | this repo's `Content/DEMO/STGSLCT/D_STGSLCT_US.AMB` |

Method: `s4content unpack` the archive, then read the `.AMA` header directly. The layout is 4 bytes of
`#AMA` magic, then seven 32-bit fields: version, node_num, act_num, node_tbl_offset, act_tbl_offset,
node_name_tbl_offset, act_name_tbl_offset. At `act_name_tbl_offset` sit `act_num` 32-bit offsets, each
pointing at a NUL-terminated name. XBLA is big-endian, the repo's mobile set little-endian.

The four `D_CMN_*` archives this screen also loads were checked the same way against
`file/iPhone/ace`, and the repo's copies match there too: `D_CMN_WIN` 8, `D_CMN_BTN` 15, `D_CMN_OBI` 5,
`D_CMN_BG` 3.

That the console headers match a Feb 2010 beta is worth noting, because the console branch forked from
the trunk around May 2010. For this screen the headers had not moved between those dates.

## The shape of the difference

Console 89 actions, mobile 80. Ten console-only names, one mobile-only, and everything else is an
order-preserving renumber.

Console only: `tab_normal_r`, `back_tab`, `act_cursor1/2/3`, `zone_cursor1/2/3`, `tex_sonic`,
`tex_eme`. Mobile only: `tab_time_move`.

Mobile keeps `act_cursor_up` and `act_cursor_down` but has none of the six cursor sprites, which is
consistent with selecting by touch rather than moving a cursor.

The language archives are 20 console against 13 mobile, with seven console-only names: `tex_time`,
`tex_score`, `tex_menu`, `tex_mess`, `tex_new`, `tex_sctm01b`, `tex_sctm02b`.

## Two things that decide how the port is written

**The id spaces are disjoint from index 1 onward.** In the main archive only `tab_normal_l` at id 0
lands on the same index in both. The language archives share nothing at all: mobile's index 0
(`tex_zname`) is console's index 2. So the two variants need two complete id tables, the way DmTitle
ended up, not one table with a correction applied.

**`act1_num_a3` and `act1_num_a4` are transposed.** Console declares a4 at 38 and a3 at 39; mobile has
a3 at 33 and a4 at 34. Every other name is order-preserving with a delta between -1 and -9, which
makes a per-range offset look workable. It is not, and this pair is the only thing that would catch
it out.

## Loose end

`readAMAFile` throws `OverflowException` at `AmAma.cs:275`, allocating `A2S_SUB_TRS[trs_key_num + 1]`
from a garbage count, when handed the stage select `.AMA` extracted to a standalone file by
`s4content unpack`. It does this for the mobile archive as well as the console one, and the game loads
that same mobile data through `readAMAFile(AmbChunk)` every time the screen opens without complaint.
So the fault is most likely in treating an unpacked member as a standalone file rather than in the
parser, but that has not been demonstrated. The title `.AMA` fixtures in `Sonic4Episode1.Tests` come
out of the same tool and parse fine, so whatever it is, it is not simply "unpacked members do not
parse".

Not chased down, because none of the findings above needed the full parse. Worth knowing before
anyone writes a test that calls `readAMAFile` on a stage select archive: it will consume the machine.
