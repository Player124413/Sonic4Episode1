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
        private Sonic4Ep1 _game;
        private View _view;
        private IAssetPackManager _assetPackManager;
        private AssetPackStateUpdateListenerWrapper _listener;

        private ProgressDialog _dialog;

        private void Run()
        {
            _dialog?.Hide();

            var packLocation = Path.Combine(_assetPackManager.GetPackLocation("content").Path(), "assets");
            DirectoryRoot.Root = packLocation;
            _game.Run();
        }

        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var controller = WindowCompat.GetInsetsController(Window, Window.DecorView);
            controller.SystemBarsBehavior = WindowInsetsControllerCompat.BehaviorShowTransientBarsBySwipe;
            controller.Hide(WindowInsetsCompat.Type.NavigationBars());

            _game = new Sonic4Ep1();
            _view = _game.Services.GetService<View>();
            SetContentView(_view);

            _listener = new AssetPackStateUpdateListenerWrapper();
            _listener.StateUpdate += Listener_StateUpdate;

            _assetPackManager = AssetPackManagerFactory.GetInstance(_view.Context);
            _assetPackManager.RegisterListener(_listener.Listener);

            var assetPackPath = _assetPackManager.GetPackLocation("content");
            var assetsFolderPath = assetPackPath?.AssetsPath() ?? null;
            if (assetsFolderPath is null)
            {
                var result = await _assetPackManager.Fetch(new string[] { "content" })
                     .AsAsync<AssetPackStates>();
            }
            else
            {
                Run();
            }
        }

        protected override void OnResume()
        {
            _assetPackManager.RegisterListener(_listener.Listener);
            base.OnResume();
        }

        protected override void OnPause()
        {
            _assetPackManager.UnregisterListener(_listener.Listener);
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
                    double percent = 100.0 * downloaded / totalSize;
                    _dialog.Progress = (int)percent;
                    Log.Info("Listener_StateUpdate", $"Downloading {percent}");
                    break;
                case AssetPackStatus.Failed:
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

    }
}
