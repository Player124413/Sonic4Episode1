using System;
using System.IO;
using Android.App;
using Android.Content.PM;
using Android.Util;
using Microsoft.Xna.Framework;
using Org.Libsdl.App;
using static SDL3.SDL;

namespace Sonic4Episode1.Android
{
    [Activity(
        Label = "@string/app_name",
        MainLauncher = true,
        Icon = "@drawable/icon",
        AlwaysRetainTaskState = true,
        LaunchMode = LaunchMode.SingleInstance,
        ScreenOrientation = ScreenOrientation.UserLandscape,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize
    )]
    public class Sonic4Activity : SDLActivity
    {
        private ProgressDialog _dialog;
        private const string Profile = "mobile-hd-astc";
        private const string ContentHost = "https://wamwoowam.co.uk/sonic4-new/";
        internal static Sonic4Activity Current { get; private set; }
        protected override string[] GetLibraries() => ["SDL3"];

        protected override void OnCreate(global::Android.OS.Bundle savedInstanceState)
        {
            Current = this;
            base.OnCreate(savedInstanceState);
        }

        // SDLThread. The default body dlopens libmain.so and hands SDL_main to SDL_RunApp, which is
        // SDL_SetMainReady plus a call. Same thing, minus the dlopen.
        //
        // It has to block for as long as the game is up. SDLMain calls nativeCleanupMainThread and
        // finishes the Activity the moment this returns, so an async body ends the app at its first
        // await
        protected override void Main()
        {
            SDL_SetMainReady();

            SDL_SetHint(SDL_HINT_ORIENTATIONS, "LandscapeLeft LandscapeRight");
            SDL_SetHint(SDL_HINT_MOUSE_TOUCH_EVENTS, "0");
            SDL_SetHint(SDL_HINT_TOUCH_MOUSE_EVENTS, "0");
            SDL_SetHint(SDL_HINT_PEN_TOUCH_EVENTS, "0");

            // one tree per profile, so switching between them is a relaunch rather than a
            // reinstall. The title location is what holds the tree, not the tree: the boot readers
            // write the "Content\\" prefix themselves, and OpenStream lowercases the whole name,
            // the directory included
            var home = Path.Combine(FilesDir.AbsolutePath, Profile);
            var root = Path.Combine(home, "content");

            TitleContainer.SetRoot_EXT(home);

            try
            {
                ContentSource.Current = BundleContentSource
                    .InstallAsync(root, SetLocation(), new Progress<double>(ShowProgress))
                    .GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Log.Error(nameof(Sonic4Activity), $"content install failed: {ex}");
                RunOnUiThread(() => ContentUnavailable(ex.Message));
                return;
            }

            RunOnUiThread(Dismiss);

            // Content.RootDirectory stays the relative "Content" the ctor sets. An absolute one
            // gets lowercased whole by OpenStream, application id included, and there is no
            // /data/user/0/org.wamwoowam.sonic4episode1
            using var game = new Sonic4Ep1();
            game.Run();
        }

        private string SetLocation()
        {
            if (ContentHost.Length != 0)
                return $"{ContentHost.TrimEnd('/')}/sets/{Profile}";

            return Path.Combine(GetExternalFilesDir(null).AbsolutePath, "sets", Profile);
        }

        private void ShowProgress(double fraction)
        {
            RunOnUiThread(() =>
            {
                _dialog ??= Show();
                _dialog.Progress = (int)(fraction * 100);
            });

            ProgressDialog Show()
            {
                // TODO(wam): what the player reads while the game downloads itself
                var dialog = new ProgressDialog(this);
                dialog.SetTitle("Downloading content...");
                dialog.SetCancelable(false);
                dialog.Max = 100;
                dialog.Show();
                return dialog;
            }
        }

        private void Dismiss()
        {
            _dialog?.Dismiss();
            _dialog = null;
        }

        private void ContentUnavailable(string detail)
        {
            Dismiss();

            // TODO(wam): the message the player actually sees when content cannot be downloaded
            new AlertDialog.Builder(this)
                .SetTitle("Content unavailable")
                .SetMessage(detail)
                .SetCancelable(false)
                .Show();
        }
    }
}
