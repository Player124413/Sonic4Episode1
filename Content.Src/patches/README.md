# patches

Authored replacements for individual archive members. Unlike everything else under `Content.Src`,
these are committed: a hand-made fix cannot be reproduced from a provenance root.

A file here does one of two things, decided by whether the member it names already exists:

- **replace** an existing member's bytes, which then goes through the ordinary encode path and obeys
  the profile's `textures` set like anything else;
- **append** a member the archive never had, emitted **verbatim** and exempt from that set, because
  the author picked the format for a reason.

Neither removes or reorders anything. Appends land after every existing member, so no inlined archive
index moves, and `check` reports the count rather than failing on it.

A stem can be both: zone 1 carries a `Z1_FALL_TEST.INO` and wants a `Z1_FALL_TEST.DDS` beside it.

## Layout

The tree mirrors what `unpack` writes, so the edit loop is unpack, edit, build:

```
patches/<archive path>/<nested archive>/<member>.png
```

The extension is ignored when matching, so a `.png` patch stands in for a `.pvr` or `.dds` member and
the member is renamed to match its new payload on the way out. After that a patch goes through the
ordinary encode path, so it obeys the profile's `textures` set like any other member.

## Working on one

```
s4content unpack "<provenance root>/DEMO/STGSLCT/D_STGSLCT.AMB" scratch --decode
```

`--decode` is the part that matters: without it members come out in their shipped format, which for
the iOS bundle means PVR that nothing can open. With it, textures are written as PNG and the member
is renamed to match. Edit the one you want, then copy it under `patches/` at its tree-relative path:

```
patches/DEMO/STGSLCT/D_STGSLCT.AMB/D_STGSLCT.AMB/D_STGSLCT_TAB09.PNG
```

Then `s4content build desktop-hd`. The build reports `patched members` and `check` still verifies the
member list is unmoved.

Note the textures are stored upside down, because the game's UVs put the origin at the bottom left.
Edit them in that orientation; the pipeline does not flip.

## What is here

**Replacements**

`DEMO/STGSLCT/.../D_STGSLCT_TAB09` - the zone panel frame. The area *outside* the rounded rectangle
shipped as opaque black in the iPad set, which put a black slab behind the stage select panels. The
inside is meant to be black, so keying is not available; the fix is authored alpha on the outside
only, which leaves the alpha binary and lets it encode to BC1.

**Appends: the textures the mobile builds dropped**

Every mobile-lineage archive is missing exactly what the shader paths consumed, and no provenance
root has a mobile copy, because the console ones are stored the other way up. These three are the
PC/360 originals with the vertical flip already applied, taken from the tree they were first
hand-injected into. PC and 360 payloads are byte-identical, so either build is a valid source.

| member | archive | what it is |
| --- | --- | --- |
| `WATER_A.DDS` | `G_ZONE3/GMK/GMK_WATER_SF_MDL.AMB` | 256x256 uncompressed BGRA offset map for the zone 3 gameplay water |
| `WATERCOLOR.DDS` | same | the depth ramp |
| `Z1_FALL_TEST.DDS` | `G_ZONE1/DECO/DECO_ZONE1.AMB` | the zone 1 waterfall offset map |

The offset maps **must stay uncompressed**. Their RGB and alpha carry two decorrelated channels,
which is why the shaders take x from `.w` and y from `.z`; block compressing that destroys it. That
is why appends bypass the encode path.

Note also that adding the file is necessary but not always sufficient - the zone 1 fall material
declares one texture map where PC declares two, so the offset map is loaded explicitly by
`gmDecoLoadFallBaseTexture`. See `docs/amb-archives.md`.
