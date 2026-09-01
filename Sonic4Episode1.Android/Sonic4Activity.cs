using System;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Extensions;
using Android.OS;
using Android.Util;
using Android.Views;
using AndroidX.Core.View;
using Microsoft.Xna.Framework;
using Xamarin.Google.Android.Play.Core.AssetPacks;
using Xamarin.Google.Android.Play.Core.AssetPacks.Model;

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
    public class Sonic4Activity : AndroidGameActivity
    {
        // the Guide shim needs an Activity to get onto the UI thread with, and the game loop only
        // ever knows about Game
        internal static Sonic4Activity Current { get; private set; }

        private Sonic4Ep1 _game;
        private View _view;
        private IAssetPackManager _assetPackManager;
        private AssetPackStateUpdateListenerWrapper _listener;

        private ProgressDialog _dialog;
        private bool _listening;
        private bool _running;

        // Completed can arrive more than once across a resume, and Game.Run does not survive being
        // called twice
        private void Run()
        {
            if (_running)
                return;

            _running = true;

            _dialog?.Dismiss();
            _dialog = null;

            DirectoryRoot.Root = _assetPackManager.GetPackLocation("content").AssetsPath();
            _game.Run();
        }

        private void StartListening()
        {
            if (_listening)
                return;

            _listening = true;
            _assetPackManager.RegisterListener(_listener.Listener);
        }

        private void StopListening()
        {
            if (!_listening)
                return;

            _listening = false;
            _assetPackManager.UnregisterListener(_listener.Listener);
        }

        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Current = this;

            var controller = WindowCompat.GetInsetsController(Window, Window.DecorView);
            controller.SystemBarsBehavior = WindowInsetsControllerCompat.BehaviorShowTransientBarsBySwipe;
            controller.Hide(WindowInsetsCompat.Type.NavigationBars());

            _game = new Sonic4Ep1();
            _view = _game.Services.GetService<View>();
            SetContentView(_view);

            _listener = new AssetPackStateUpdateListenerWrapper();
            _listener.StateUpdate += Listener_StateUpdate;

            _assetPackManager = AssetPackManagerFactory.GetInstance(_view.Context);
            StartListening();

            var assetsFolderPath = _assetPackManager.GetPackLocation("content")?.AssetsPath();
            if (assetsFolderPath is not null)
            {
                Run();
                return;
            }

            try
            {
                await _assetPackManager.Fetch(["content"]).AsAsync<AssetPackStates>();
            }
            catch (Exception ex)
            {
                ContentUnavailable(ex.Message);
            }
        }

        protected override void OnResume()
        {
            StartListening();
            base.OnResume();
        }

        protected override void OnPause()
        {
            StopListening();
            base.OnPause();
        }

        private void Listener_StateUpdate(object? sender, AssetPackStateUpdateListenerWrapper.AssetPackStateEventArgs e)
        {
            var status = e.State.Status();
            switch (status)
            {
                case AssetPackStatus.Downloading:
                    if (_dialog == null)
                    {
                        _dialog = new ProgressDialog(_view.Context);
                        _dialog.SetTitle("Downloading content...");
                        _dialog.SetCancelable(false);
                        _dialog.Max = 100;
                        _dialog.Show();
                    }

                    long downloaded = e.State.BytesDownloaded();
                    long totalSize = e.State.TotalBytesToDownload();
                    double percent = totalSize > 0 ? 100.0 * downloaded / totalSize : 0;
                    _dialog.Progress = (int)percent;
                    Log.Info("Listener_StateUpdate", $"Downloading {percent}");
                    break;
                case AssetPackStatus.Failed:
                    ContentUnavailable($"asset pack error {e.State.ErrorCode()}");
                    break;
                case AssetPackStatus.Completed:
                    {
                        Run();
                        break;
                    }
                case AssetPackStatus.WaitingForWifi:
                case AssetPackStatus.RequiresUserConfirmation:
                    _assetPackManager.ShowConfirmationDialog(this);
                    break;
            }
        }

        // without this a failed fetch leaves the progress dialog up forever with nothing said
        private void ContentUnavailable(string detail)
        {
            Log.Error(nameof(Sonic4Activity), $"content pack unavailable: {detail}");

            _dialog?.Dismiss();
            _dialog = null;

            // TODO(wam): the message the player actually sees when content cannot be downloaded
            new AlertDialog.Builder(_view.Context)
                .SetTitle("Content unavailable")
                .SetMessage(detail)
                .SetCancelable(false)
                .Show();
        }
    }
}
