using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Microsoft.Xna.Framework;

namespace Sonic4Episode1Android
{
    /// <summary>
    /// Android entry point of the Sonic 4: Episode I port.
    ///
    /// Everything the activity does is traced through <see cref="Diagnostics"/> and every
    /// surprise is turned into an on screen report instead of an immediate process death,
    /// because on a phone there usually is no adb / logcat to look at.
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

        private static bool handlersInstalled;

        private Sonic4Ep1 game;
        private View gameView;
        private bool gameStarted;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            InstallGlobalHandlers();
            Diagnostics.Step("OnCreate: activity " + GetType().Name + " created");

            try
            {
                base.OnCreate(savedInstanceState);
                Diagnostics.Step("OnCreate: base.OnCreate completed");
            }
            catch (Exception ex)
            {
                Diagnostics.Fail("base.OnCreate (AndroidGameActivity) failed", ex);
                ShowFailureScreen("MonoGame's AndroidGameActivity.OnCreate");
                return;
            }

            try
            {
                EnableFullscreen();
                Diagnostics.Step("OnCreate: immersive fullscreen enabled");
            }
            catch (Exception ex)
            {
                Diagnostics.Fail("EnableFullscreen failed", ex);
            }

            bool hasContent = false;
            try
            {
                hasContent = ContentInstalled();
            }
            catch (Exception ex)
            {
                Diagnostics.Fail("ContentInstalled failed", ex);
            }

            Diagnostics.Step("OnCreate: game content installed = " + hasContent);

            if (hasContent)
                StartGame();
            else
                ShowContentMissingDialog();
        }

        /// <summary>
        /// Makes sure a crash leaves a readable report behind (and on screen) instead of
        /// closing the app without a word.
        /// </summary>
        private static void InstallGlobalHandlers()
        {
            if (handlersInstalled)
                return;

            handlersInstalled = true;

            try
            {
                Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser += (sender, args) =>
                {
                    try
                    {
                        Diagnostics.Fail("unhandled exception in managed code", args.Exception);

                        var activity = Microsoft.Xna.Framework.Game.Activity;
                        if (activity != null)
                        {
                            activity.RunOnUiThread(() => ShowFailureDialog(activity, args.Exception));
                            // Keep the process alive so the report stays on screen. The game
                            // loop is usually broken at this point, but the user can read and
                            // share the error instead of looking at a vanished app.
                            args.Handled = true;
                        }
                    }
                    catch (Exception)
                    {
                        // Nothing else we can do - let the runtime kill the process.
                    }
                };

                AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
                    Diagnostics.Fail("AppDomain unhandled exception", args.ExceptionObject as Exception);

                var previous = Java.Lang.Thread.DefaultUncaughtExceptionHandler;
                Java.Lang.Thread.DefaultUncaughtExceptionHandler = new JavaCrashHandler(previous);

                Diagnostics.Step("global exception handlers installed");
            }
            catch (Exception ex)
            {
                Diagnostics.Fail("installing global exception handlers", ex);
            }
        }

        /// <summary>Hides the status and navigation bars and keeps the screen switched on, so the game
        /// gets the whole display - exactly what GraphicsDeviceManager.ResetClientBounds expects.</summary>
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

            try
            {
                Diagnostics.Step("StartGame: constructing Sonic4Ep1");

                game = new Sonic4Ep1();
                Diagnostics.Step("StartGame: Sonic4Ep1 constructed");

                gameView = game.Services.GetService(typeof(View)) as View;
                Diagnostics.Step("StartGame: view = " + (gameView == null ? "null" : gameView.GetType().FullName));

                SetContentView(gameView);
                Diagnostics.Step("StartGame: content view set, calling Game.Run()");

                game.Run();

                Diagnostics.Step("StartGame: Game.Run() returned");
            }
            catch (Exception ex)
            {
                Diagnostics.Fail("StartGame failed", ex);
                ShowFailureScreen("starting the MonoGame game loop");
            }
        }

        /// <summary>Checks whether the original game files were dropped into "Assets/Content".</summary>
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
            try
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
                Diagnostics.Step("OnCreate: content dialog shown");
            }
            catch (Exception ex)
            {
                Diagnostics.Fail("showing the content dialog failed", ex);
                ShowFailureScreen("the \"content missing\" dialog");
            }
        }

        /// <summary>Replaces the screen with the startup report so the failure can be read and shared.</summary>
        private void ShowFailureScreen(string context)
        {
            try
            {
                var layout = new LinearLayout(this)
                {
                    Orientation = Orientation.Vertical
                };
                layout.SetBackgroundColor(Color.Black);
                layout.SetPadding(24, 24, 24, 24);

                var title = new TextView(this)
                {
                    Text = "Sonic 4: Episode I could not start"
                };
                title.SetTextColor(Color.Rgb(255, 216, 61));
                title.SetTextSize(Android.Util.ComplexUnitType.Sp, 16f);
                layout.AddView(title);

                var reason = new TextView(this)
                {
                    Text = context
                };
                reason.SetTextColor(Color.White);
                reason.SetTextSize(Android.Util.ComplexUnitType.Sp, 12f);
                layout.AddView(reason);

                var buttons = new LinearLayout(this) { Orientation = Orientation.Horizontal };
                buttons.AddView(MakeButton("Share report", (sender, args) => ShareReport()));
                buttons.AddView(MakeButton("Quit", (sender, args) => Finish()));
                layout.AddView(buttons);

                var scroll = new ScrollView(this);
                var text = new TextView(this)
                {
                    Text = Diagnostics.BuildReport()
                };
                text.SetTextColor(Color.LightGray);
                text.SetTextSize(Android.Util.ComplexUnitType.Sp, 9f);
                text.SetTextIsSelectable(true);
                scroll.AddView(text);
                layout.AddView(scroll, new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, 0, 1f));

                SetContentView(layout);
            }
            catch (Exception ex)
            {
                // If even this fails, at least logcat knows about it.
                Diagnostics.Fail("ShowFailureScreen failed", ex);

                try
                {
                    new AlertDialog.Builder(this)
                        .SetTitle("Sonic 4: Episode I could not start")
                        .SetMessage(context + "\n\n" + Diagnostics.LastFailure)
                        .SetPositiveButton("OK", (sender, args) => { })
                        .Create()
                        .Show();
                }
                catch (Exception)
                {
                    // Give up.
                }
            }
        }

        private static void ShowFailureDialog(Activity activity, Exception exception)
        {
            try
            {
                new AlertDialog.Builder(activity)
                    .SetTitle("Sonic 4: Episode I crashed")
                    .SetMessage(exception == null ? "(no exception)" : exception.ToString())
                    .SetPositiveButton("OK", (sender, args) => { })
                    .Create()
                    .Show();
            }
            catch (Exception)
            {
                // Nothing to do.
            }
        }

        private Button MakeButton(string caption, EventHandler onClick)
        {
            var button = new Button(this) { Text = caption };
            button.Click += onClick;
            return button;
        }

        private void ShareReport()
        {
            try
            {
                var intent = new Intent(Intent.ActionSend);
                intent.SetType("text/plain");
                intent.PutExtra(Intent.ExtraSubject, "Sonic 4 Episode I - Android startup report");
                intent.PutExtra(Intent.ExtraText, Diagnostics.BuildReport());
                StartActivity(Intent.CreateChooser(intent, "Share startup report"));
            }
            catch (Exception ex)
            {
                Diagnostics.Fail("ShareReport failed", ex);
            }
        }
    }

    /// <summary>
    /// Logs uncaught exceptions that reach the Java side (the MonoGame game loop runs on a
    /// Java thread) and then lets the default handler do its job.
    /// </summary>
    internal class JavaCrashHandler : Java.Lang.Object, Java.Lang.Thread.IUncaughtExceptionHandler
    {
        private readonly Java.Lang.Thread.IUncaughtExceptionHandler previous;

        public JavaCrashHandler(Java.Lang.Thread.IUncaughtExceptionHandler previous)
        {
            this.previous = previous;
        }

        public void UncaughtException(Java.Lang.Thread thread, Java.Lang.Throwable exception)
        {
            try
            {
                Diagnostics.Fail("uncaught exception on thread " + (thread == null ? "(unknown)" : thread.Name),
                    new Exception(exception == null ? "(no throwable)" : exception.ToString()));
            }
            catch (Exception)
            {
                // Never let diagnostics break the crash reporting path.
            }

            if (previous != null)
            {
                try
                {
                    previous.UncaughtException(thread, exception);
                    return;
                }
                catch (Exception)
                {
                    // Fall through to the default handling below.
                }
            }

            // No useful handler to chain to: behave like the Android default and end the
            // process instead of leaving it in an undefined state.
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        }
    }
}
