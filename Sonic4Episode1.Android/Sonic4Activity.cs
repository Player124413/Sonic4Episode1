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

        protected override void Main()
        {
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;

            SDL_SetMainReady();

            SDL_SetHint(SDL_HINT_ORIENTATIONS, "LandscapeLeft LandscapeRight");
            SDL_SetHint(SDL_HINT_MOUSE_TOUCH_EVENTS, "0");
            SDL_SetHint(SDL_HINT_TOUCH_MOUSE_EVENTS, "0");
            SDL_SetHint(SDL_HINT_PEN_TOUCH_EVENTS, "0");

            var home = Path.Combine(FilesDir.AbsolutePath, Profile);
            var root = Path.Combine(home, "content");

            TitleContainer.SetRoot_EXT(home);
            TitleContainer.IsLowerCase_EXT = true;

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

            using var game = new Sonic4Ep1();
            game.Run();
        }

        private void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            Log.Warn(nameof(Sonic4Activity), $"first chance exception: {e.Exception}");
        }

        private string SetLocation()
        {
            if (ContentHost.Length != 0)
                return $"{ContentHost.TrimEnd('/')}/sets/{Profile}";

            return Path.Combine(GetExternalFilesDir(null).AbsolutePath, "sets", Profile);
        }

        private void ShowProgress(double fraction)
        {
            // 95% sure Progress<T> does SynchronizationContext.Post, but just to be sure
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
