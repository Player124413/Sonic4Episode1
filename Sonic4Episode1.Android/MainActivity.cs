using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Microsoft.Xna.Framework;

namespace Sonic4Episode1Android
{
    /// <summary>
    /// Android entry point of the Sonic 4: Episode I port.
    /// </summary>
    [Activity(
        Label = "@string/app_name",
        MainLauncher = true,
        Icon = "@drawable/ic_launcher",
        AlwaysRetainTaskState = true,
        LaunchMode = LaunchMode.SingleInstance,
        ScreenOrientation = ScreenOrientation.SensorLandscape,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden |
                               ConfigChanges.ScreenSize | ConfigChanges.SmallestScreenSize | ConfigChanges.UiMode,
        Theme = "@android:style/Theme.NoTitleBar.Fullscreen")]
    public class MainActivity : AndroidGameActivity
    {
        // View.SYSTEM_UI_FLAG_* constants. They are spelled out as numbers so the build does not
        // depend on the naming (or existence) of the corresponding bindings enum members.
        private const int FlagLayoutStable = 0x00000100;
        private const int FlagLayoutHideNavigation = 0x00000200;
        private const int FlagLayoutFullscreen = 0x00000400;
        private const int FlagHideNavigation = 0x00000002;
        private const int FlagFullscreen = 0x00000004;
        private const int FlagImmersiveSticky = 0x00001000;

        private Sonic4Ep1 game;
        private View gameView;
        private bool gameStarted;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            EnableFullscreen();

            if (ContentInstalled())
                StartGame();
            else
                ShowContentMissingDialog();
        }

        /// <summary>
        /// Hides the status and navigation bars and keeps the screen switched on, so the game
        /// gets the whole display - exactly what GraphicsDeviceManager.ResetClientBounds expects.
        /// </summary>
        private void EnableFullscreen()
        {
            Window.AddFlags(WindowManagerFlags.KeepScreenOn);
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
            ApplyImmersiveMode();
        }

        private void ApplyImmersiveMode()
        {
            var decorView = Window?.DecorView;
            if (decorView == null)
                return;

            var flags = FlagLayoutStable | FlagLayoutHideNavigation | FlagLayoutFullscreen |
                        FlagHideNavigation | FlagFullscreen | FlagImmersiveSticky;

            decorView.SystemUiVisibility = (StatusBarVisibility)flags;
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);

            if (hasFocus)
                ApplyImmersiveMode();
        }

        private void StartGame()
        {
            if (gameStarted)
                return;

            gameStarted = true;

            game = new Sonic4Ep1();
            gameView = game.Services.GetService(typeof(View)) as View;

            SetContentView(gameView);

            game.Run();
        }

        /// <summary>
        /// Checks whether the original game files were dropped into "Assets/Content".
        /// </summary>
        /// <remarks>
        /// The assets are read through MonoGame's TitleContainer, which resolves paths against
        /// the Android asset manager, so "Content/DEMO/TITLE/D_TITLE.AMB" has to live in
        /// Sonic4Episode1.Android/Assets/Content/DEMO/TITLE/D_TITLE.AMB.
        /// </remarks>
        private bool ContentInstalled()
        {
            string[] folders = { "Content", "content" };

            foreach (var folder in folders)
            {
                try
                {
                    var entries = Assets?.List(folder);
                    if (entries != null && entries.Length > 0)
                        return true;
                }
                catch (Exception)
                {
                    // Folder does not exist - try the next one.
                }
            }

            return false;
        }

        private void ShowContentMissingDialog()
        {
            var dialog = new AlertDialog.Builder(this)
                .SetTitle("Game content not found")
                .SetMessage("Sonic 4: Episode I needs the original game files, they are not part of this port.\n\n" +
                            "Copy the \"Content\" folder of the game (or the extracted \"Content.asar\") into:\n\n" +
                            "Sonic4Episode1.Android/Assets/Content/\n\n" +
                            "…and build the app again. See README.md for details.")
                .SetPositiveButton("Start anyway", (sender, args) => StartGame())
                .SetNegativeButton("Quit", (sender, args) => Finish())
                .Create();

            dialog.SetCancelable(false);
            dialog.Show();
        }
    }
}
