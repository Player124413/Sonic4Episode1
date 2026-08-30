# Camera view extent: why screen width comes off disp_height

Working note. Confirmed 2026-08-30 from a runtime probe, not from reading alone.

## The rotation

The scene is rendered rotated 90 degrees and unrotated on the way out:

- `ObjCameraGetTargetRollCamera` (`AppMain/Obj/ObjCamera.cs`) sets
  `troll_camera.Roll = obj_camera.roll + 16384`. 16384 in Angle32 is +90 deg, applied in the view
  matrix.
- `OpenGL.PROJECTION_CORRECTION` (`mpp/OpenGL.cs:147`) is
  `CreateRotationZ(1.570796f) * CreateScale(1f, 1.12f, 0.5f) * CreateTranslation(0,0,0.5f)`,
  post-multiplied onto every projection. The rotation is -90 deg in clip space.

Net orientation is identity, but the projection matrix sits between the two rotations, so its X and
Y are swapped relative to the screen. This is a leftover of the mobile framebuffer being portrait
native.

## What that means for the ortho box

`ObjCamera`/`ObjDraw` build the box as

```
dx = disp_height * scale * 0.5 * (disp_height / disp_width)
dy = dx / aspect                      // aspect = AMD_SCREEN_ASPECT = height/width
```

which at 1280x720, scale 0.5469176375, aspect 0.5625 gives `dx*2 = 221.5`, `dy*2 = 393.78`.
Measured, not derived: the probe printed `ortho=221.50163x393.78067`.

Because of the swap, `dy` is the **screen width** extent and `dx` is the **screen height** extent:

| | world units across | px per world unit |
| --- | --- | --- |
| screen width (1280 px) | `disp_height * scale` = 393.78 | 3.25 |
| screen height (720 px) | `disp_height * scale * aspect` = 221.5 | 3.25 |

Square pixels, so the framing is undistorted. Cross-check: the console arm of the C++ shows
383.6 x 215.8 world units (`GMD_CAMERA_SCALE` 0.2997259375 at 1280x720). The port is within 2.6% of
that, which is why `GMD_CAMERA_SCALE = 0.5469176375` was tuned to the value it has. Dropping in the
console's own 0.2997259375 zooms *in* to 215.8 x 121.4.

## The consequence that caused bugs

`GMD_OBJ_LCD_X/Y` were `scale * GSD_DISP_WIDTH` / `scale * GSD_DISP_HEIGHT` = 700 x 393, on the
assumption of an unrotated projection. The real view is 393.78 x 221.5, so the object system
believed the screen was 1.78x too big in both axes. Everything keyed off it inherited that:

- `clip_lcd_size` (= `lcd_size` from `ObjInit`) is the event spawn/despawn rect. Records only spawn
  in a 32-unit band just outside it, so a record could sit permanently inside an oversized rect and
  never spawn. This is the Zone 3 Act 3 drain tank: probe showed `id=217 pos=(17312,312)` with
  `on=True off=False`, rect `(16816,102)-(17516,495)`.
- `map_fcol` camera-limit boxes authored for a ~215-tall view are shorter than a 393-tall view, so
  the clamp range in `GmCameraFunc` inverts and pins the camera to the top.
- `GmBoss4` sets `map_fcol.right = scroll_pt + (AMD_SCREEN_2D_WIDTH/2) * GMD_CAMERA_SCALE` = +350
  where the true half-width is 196.9. `gmBoss4BodyStateUpdate2ndWaitBoss` waits for
  `body.pos.x >= map_fcol.right - 10`; the body closes at `GMD_BOSS4_SCROLL_SPD_BOSS` (5/frame)
  while the edge runs at `GMD_BOSS4_SCROLL_SPD_MAX` (8/frame), so it bleeds 3/frame from a starting
  margin of 661 and runs out 19 frames before `GMD_BOSS4_BODY_SONIC_CTRL_TIME` (240) expires, then
  pins at `right - 50` forever, 40 short of the gate. At the correct half-width the starting margin
  is ~814 against the 720 needed.

## Fix applied

`GMD_OBJ_LCD_X/Y` now describe the real view:

```
GMD_OBJ_LCD_X = GMD_CAMERA_SCALE * GSD_DISP_HEIGHT                    // 393
GMD_OBJ_LCD_Y = GMD_CAMERA_SCALE * GSD_DISP_HEIGHT * AMD_SCREEN_ASPECT // 221
```

and every site that had its own screen-half-extent expression now uses `OBD_LCD_X/2` /
`OBD_LCD_Y/2`: `GmCameraPosSet`, the display-limit check in `GmCameraFunc`, `GmGmkCamScrLimit`
(both the release check and `gmGmkCamScrLimitSetting`), `GmBoss4`'s arena bounds, and
`GmMapGetAddMapCameraPos`.

Note the C++ forms (`GSD_DISP_WIDTH/2 * scale`, `AMD_SCREEN_2D_WIDTH/2 * GMD_CAMERA_SCALE`) are all
equal to `OBD_LCD_X/2` *in the C++*, because there `OBD_LCD_X == scale * GSD_DISP_WIDTH`. Using
`OBD_LCD_X/2` keeps one definition and stays faithful; it only diverges here because of the
rotation.

## Still open

- `AMD_SCREEN_ASPECT` is `height/width`, but `nnMakePerspectiveMatrix` does `M00 = cot(fovy/2)/aspect`,
  the `width/height` convention. Only matters for the perspective path; gameplay is ortho
  (`GmCameraInit` sets `glb_camera_type = NNE_PROJECTION_TYPE_ORTHO`).
- The `CreateScale(1f, 1.12f, 0.5f)` in `PROJECTION_CORRECTION` is unexplained. 1.12 is not
  obviously derived from anything. If it landed on the screen-vertical axis the real view height
  would be 221.5/1.12 = 197.8, but play testing with `GMD_OBJ_LCD_Y = 221` came out right, so it
  does not appear to affect the world extent the gameplay logic needs. Left alone.
- On a non-16:9 window the view height scales with aspect while the width stays fixed, which is
  backwards from what widescreen should do. Out of scope for now.
