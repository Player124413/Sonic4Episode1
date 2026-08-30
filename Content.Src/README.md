# Content.Src

Assets that are not reproducible from a provenance root. Everything else resolves straight from a
root declared in `content-sources.toml`, so this directory holds the delta and nothing more.

Only `_archive.toml` descriptors and this file are committed; the assets beside them are not.

## staged/

Twenty archives that `Content/bin/DesktopGL/Content` carried over the shipped WP7 XAP. They were
living in a build output, which is the only copy some of them had.

| what | count | where it really comes from |
| --- | --- | --- |
| `DEMO/{CMN,TITLE}/PC/*.AMB` | 18 | `pc`, the same archives with their DDS converted to PNG |
| `G_COM/RING/RING_{MDL,TEX}_HD.AMB` | 2 | `xbla`, big-endian, renamed with an `_HD` suffix |

Both are derivable in principle: the rings are a copy and a rename, and the console screens are a
`pc` archive at a `PC/` subpath with its textures transcoded. Neither is derivable *yet*, because
nothing in the pipeline converts DDS to PNG, so they are preserved as data until milestone 3 makes
the transcode real. At that point this directory should shrink to nothing.
