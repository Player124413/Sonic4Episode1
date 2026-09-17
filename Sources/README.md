# Sources

Provenance roots. `content-sources.toml` points here and nowhere else, so no absolute path to a game
install exists anywhere in the repo.

Copy each one in as a plain directory with the name in the first column. Copies, not junctions or
symlinks: nothing the pipeline does should be able to write back into an install.

Nothing here is committed except this file and `.gitignore`.

| directory | what goes in it | size | needed by |
| --- | --- | --- | --- |
| `wp7-orig` | the shipped WP7 XAP's `Content/`, the pristine original of this repo's lineage | 120 MB | `mobile-sd`, and as an overlay on both HD assemblies |
| `ios-hd` | the retail iPad HD bundle's `hog.app`. The clean one, **not** the copy contaminated with 2011 SD archives, see `docs/content-catalog.md` | 571 MB | both HD assemblies |
| `pc` | the Steam install root | 521 MB | `console-stages-pc` |
| `xbla` | the Feb 2010 Xbox 360 prototype, the `.../58410A07/000D0000/data` directory | 373 MB | `console-stages`, `console-menus` |
| `ios-hd-raw` | the contaminated iPad HD bundle, kept as a texture oracle | 555 MB | nothing; `diff` and `texcheck` only |
| `ios-sd-raw` | the retail iPhone SD bundle, same | 199 MB | nothing; `diff` and `texcheck` only |
| `sound` | the audio tree, frozen. Not from any shipped build; see below | 127 MB | every profile |

`ios-hd-raw` and `ios-sd-raw` are referenced by no profile. Leave them out unless you want to compare
against them; `s4content profiles` will say MISSING and nothing will break.

## sound

The one source that is not a copy of somebody's install. It holds the SFX (145 `.wav` sources beside
their built `.xnb`), the music as 45 per-track directories of `.ogg` plus a 123-byte Song `.xnb`, the
`.inf` tables, and 21 `.CSB` originals.

It cannot come from `wp7-orig`: that ships a flat 120 SFX and 37 `.wma` tracks, a different and
smaller set. The copy here was taken from a known-good FNA build output.

TODO(wam): this is frozen rather than derived. The intent is to rebuild it from the `.CSB` with
SonicAudioTools, at which point this directory becomes an output and stops being a source.

The recipe for adding a single cue exists now, in [sound-from-csb.md](../docs/sound-from-csb.md);
extraction needed two fixes to the vendored `CsbEditor` before it would read the bank at all.

TODO(wam): where each of these came from originally is not written down here on purpose. Add the
acquisition notes if you want them somewhere.
