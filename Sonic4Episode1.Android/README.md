# Sonic 4: Episode I — Android port

This is the Android front end of the decompilation. It compiles the whole shared game
(`../Sonic4Episode1/Sonic4Episode1.projitems`) against **MonoGame 3.8.4 for Android** and adds the
platform glue the game needs on a phone: a writable save directory, an Android replacement for the
Xbox LIVE `Guide` message box, the back button, immutable landscape fullscreen and asset loading.

| | |
|---|---|
| Target framework | `net8.0-android` (Android 6.0 / API 23 and newer) |
| Framework | `MonoGame.Framework.Android` 3.8.4 (NuGet) |
| Output | `.apk` (`dotnet build`/`publish`) |
| Application id | `io.github.player124413.sonic4ep1` |

## 1. Requirements

* **.NET SDK 8** (`dotnet --version` → 8.x)
* The Android workload: `dotnet workload install android`
* **JDK 17** (set `JAVA_HOME`, .NET for Android does not support JDK 21 yet)
* The Android SDK + build tools — installed automatically by the SDK, or by running

  ```bash
  dotnet build -t:InstallAndroidDependencies -f net8.0-android \
      -p:AcceptAndroidSDKLicenses=True \
      -p:AndroidSdkDirectory="$ANDROID_SDK_ROOT" \
      -p:JavaSdkDirectory="$JAVA_HOME"
  ```

## 2. Game content (required!)

None of the original Sonic 4: Episode I assets may be redistributed with the source, so a fresh
checkout **does not contain any content**. The app detects this at startup and tells you instead of
showing a black screen.

1. Take the `Content` folder of the game — the Windows Phone 7 `.xap` (`Sonic4 ep I.xap`,
   it is a zip file) or an existing desktop/`FNA` build of this decompilation.
2. Copy it to

   ```
   Sonic4Episode1.Android/Assets/Content/
   ```

   so that the first entries look like this (the game asks for uppercase paths, Android's asset
   manager is case sensitive — keep the casing of the original):

   ```
   Assets/Content/DEMO/TITLE/D_TITLE.AMB
   Assets/Content/DEMO/OPTION/D_OPTION.AMB
   Assets/Content/G_COM/MENU/G_PAUSE.AMB
   Assets/Content/SOUND/...
   Assets/Content/Kootenay.xnb, small.xnb, medium.xnb, large.xnb
   ```

   `Assets/` and everything inside `Content/` are ignored by git (`.gitignore`), the files stay local.

### Background music

The Windows Phone original played music through the CRI middleware, this port plays it through
MonoGame's `MediaPlayer` (`Content.Load<Song>("SOUND\\<track>\\Intro")` / `\\Loop`). If your content
folder has no `Song` `.xnb` files, the game still starts but stays silent. Build them with the
MonoGame content pipeline (`dotnet tool install -g dotnet-mgcb`) or copy the `SOUND` folder of the
desktop (`Sonic4Episode1.FNA`) build.

MonoGame resolves a song to an asset path relative to the asset root, so the audio file the pipeline
produces has to sit next to its `.xnb`:

```
Assets/Content/SOUND/<track>/Intro.xnb
Assets/Content/SOUND/<track>/Intro.ogg     <- resolved as Content/SOUND/<track>/Intro.ogg
Assets/Content/SOUND/<track>/Loop.xnb
Assets/Content/SOUND/<track>/Loop.ogg
```

`Song.Android` hands the file to Android's `MediaPlayer` through `AssetManager.openFd()`, which only
works for assets that are **not** compressed inside the APK. That is why the project sets:

```xml
<AndroidStoreUncompressedFileExtensions>.ogg;.mp3;.m4a;.wav;.adx</AndroidStoreUncompressedFileExtensions>
```

The intro/loop hand off is handled by the game itself (`GsSound.MediaPlayer_MediaStateChanged`,
the `#if !FNA` branch the desktop port also uses), so looping tracks work as soon as the `Loop`
assets are there.

## 3. Build & install

```bash
# debug build + install on a connected device/emulator
dotnet build Sonic4Episode1.Android/Sonic4Episode1.Android.csproj -t:Install -f net8.0-android

# release apk (unsigned/debug-signed, directly installable)
dotnet publish Sonic4Episode1.Android/Sonic4Episode1.Android.csproj -c Release -f net8.0-android
# -> Sonic4Episode1.Android/bin/Release/net8.0-android/publish/*.apk
```

For a store release add your own signing information:

```bash
dotnet publish Sonic4Episode1.Android/Sonic4Episode1.Android.csproj -c Release -f net8.0-android \
    -p:AndroidKeyStore=true -p:AndroidSigningKeyStore=my.keystore \
    -p:AndroidSigningKeyAlias=mykey -p:AndroidSigningKeyPass=… -p:AndroidSigningStorePass=…
```

There is also a GitHub Actions workflow (`.github/workflows/android.yml`) that builds an APK on every
push and uploads it as a build artifact — that is the easiest way to get a binary without a local
Android setup.

## 4. Controls

The game keeps the on-screen controls of the Windows Phone version (the virtual pad drawn in the
stage, touch triggered menus) and uses them as long as the device has a touch screen. In addition:

* **Hardware gamepad / physical keys** — `MonoGameController` maps `A/B/X/Y`, `L/R`, `Start`, the
  D-pad and the sticks; `KeyboardController` maps arrows, `A/S/D/Q/W/E` and `Enter`/`Esc`.
* **Android back button** — pauses the game / leaves a menu (MonoGame reports it as
  `GamePad.GetState(PlayerIndex.One).Buttons.Back`).

## 5. Known limitations

* Achievements, leaderboards, trial mode and profile sign-in came from Xbox LIVE on Windows Phone.
  `XBOXLive` is a no-op stub (like in the desktop port), `Guide.BeginShowMessageBox` is implemented
  with a native Android dialog (`PlatformStubs.cs`).
* The title screen "SEGA" button opens the URL in the system browser.
* The screen aspect is kept at 5:3 (the ratio the game's layout was written for, 533×320) and
  letterboxed with black bars on wider phones: `GraphicsDeviceManager.ResetClientBounds` does the
  letterboxing, `AppMain.amIPhoneAndroidSyncViewport()` keeps the GL viewport in sync when the
  window resizes (navigation bar hiding, rotation, split screen). Change
  `graphics.PreferredBackBufferWidth/Height` in `../Sonic4Episode1/Sonic4Ep1.cs` if you want a
  different ratio — but menus and touch targets assume 5:3.
* Emulators without a hardware touch screen feature (`PackageManager.FEATURE_TOUCHSCREEN`) fall back
  to keyboard/gamepad input only — enable touch input in the emulator settings.
