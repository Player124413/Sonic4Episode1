global using TitleContainer = AndroidTitleContainer;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Xamarin.Google.Android.Play.Core.AssetPacks;

internal static class DirectoryRoot
{
    public static string Root { get; set; }
}

internal static class AndroidTitleContainer
{
    public static Stream OpenStream(string assetName)
    {
        assetName = assetName.Replace('\\', '/');
        return File.OpenRead(Path.Combine(DirectoryRoot.Root, assetName));
    }
}

internal partial class AndroidContentManager(IServiceProvider serviceProvider) : ContentManager(serviceProvider)
{
    protected override Stream OpenStream(string assetName)
    {
        assetName = assetName.Replace('\\', '/');

        return File.OpenRead(Path.Combine(DirectoryRoot.Root, "Content", assetName) + ".xnb");
    }

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_filePath")]
    public static extern ref string GetFilePath(Song instance);

    public override T Load<T>(string assetName)
    {
        T t = base.Load<T>(assetName);
        if (t is Song song)
        {
            var filePath = GetFilePath(song);
            if (!filePath.StartsWith("file://"))
                GetFilePath(song) = "file://" + Path.Combine(DirectoryRoot.Root, filePath);
        }

        return t;
    }
}

namespace Sonic4ep1
{
    public static class Strings
    {
        public const string ID_STAGE3 = @"LOST LABYRINTH ZONE {0}";
        public const string ID_STAGE2 = @"CASINO STREET ZONE {0}";
        public const string ID_STAGE1 = @"SPLASH HILL ZONE {0}";
        public const string ID_STAGE4 = @"MAD GEAR ZONE {0}";
        public const string ID_SSTAGE = @"SPECIAL STAGE {0}";
        public const string ID_LOADING = @"Loading...";
        public const string ID_LB_DATE = @"Date";
        public const string ID_LB_GTAG = @"Gamertag";
        public const string ID_LB_RANK = @"Rank";
        public const string ID_YES = @"Yes";
        public const string ID_ACT = @"ACT {0}";
        public const string ID_LB_BUY = @"Please purchase the game in order to view the leaderboards";
        public const string ID_RESUME_TEXT = @"Do you want to resume from your most recent Star Post?";
        public const string ID_UPDATE_CAPTION = @"Title Update Available";
        public const string ID_BOSS = @"BOSS";
        public const string ID_LEADERBOARDS = @"Leaderboards";
        public const string ID_BESTSCORE = @"Best Score";
        public const string ID_A6_NAME = @"Contender";
        public const string ID_A6_DESC = @"Upload your recorded clear times for all Time Attack stages.";
        public const string ID_A5_NAME = @"All Stages Cleared!";
        public const string ID_A5_DESC = @"Defeat the final boss and view the ending.";
        public const string ID_A2_NAME = @"The First Chaos Emerald";
        public const string ID_A2_DESC = @"Acquire a Chaos Emerald.";
        public const string ID_A7_NAME = @"Ring Collector";
        public const string ID_A7_DESC = @"Collect all the Rings in 'Special Stage 1' and Clear the stage.";
        public const string ID_A4_NAME = @"Golden Flash";
        public const string ID_A4_DESC = @"Clear all Acts as Super Sonic.";
        public const string ID_A1_NAME = @"The Story Begins";
        public const string ID_A1_DESC = @"Clear Splash Hill Zone Act 1.";
        public const string ID_MUSIC_INTERRUPT_CAPTON = @"Interrupt Music";
        public const string ID_MUSIC_INTERRUPT_TEXT = @"Do you want to interrupt your music and use game background music?";
        public const string ID_SUPERSONIC = @"Super Sonic";
        public const string ID_A3_NAME = @"Enemy Hunter";
        public const string ID_A3_DESC = @"Defeat 1,000 enemies.";
        public const string ID_BESTTIME = @"Best Time";
        public const string ID_A9_DESC = @"Acquire all seven Chaos Emeralds.";
        public const string ID_A9_NAME = @"Super Sonic Genesis";
        public const string ID_A8_DESC = @"Build up 99 or more extra lives.";
        public const string ID_A8_NAME = @"Immortal";
        public const string ID_FINALZONE = @"E.G.G. STATION ZONE BOSS";
        public const string ID_OK = @"OK";
        public const string ID_NO = @"No";
        public const string ID_LB_UPDATE = @"Please update the game to view the Leaderboard";
        public const string ID_LB_UNABLE = @"You must be signed in to Xbox LIVE to access this feature.";
        public const string ID_RESUME_CAPTION = @"Resume game?";
        public const string ID_SONIC = @"Sonic";
        public const string ID_ACHIEVEMENTS = @"Achievements";
        public const string ID_A14_NAME = @"Casino Street Zone";
        public const string ID_A14_DESC = @"Finish all 3 Acts and boss round of Casino Street Zone";
        public const string ID_LB_OFFLINE = @"Please sign in to view the Leaderboard";
        public const string ID_A13_NAME = @"Splash Hill Zone";
        public const string ID_A13_DESC = @"Finish all 3 Acts and boss round of Splash Hill Zone";
        public const string ID_A16_NAME = @"Mad Gear Zone";
        public const string ID_A16_DESC = @"Finish all 3 Acts and boss round of Mad Gear Zone";
        public const string ID_A15_NAME = @"Lost Labyrinth Zone";
        public const string ID_A15_DESC = @"Finish all 3 Acts and boss round of Lost Labrinth Zone";
        public const string ID_A10_NAME = @"Speed's My Game";
        public const string ID_A10_DESC = @"Clear Splash Hill Zone Act 1 in less than a minute.";
        public const string ID_A12_NAME = @"Centurion";
        public const string ID_A12_DESC = @"Get 120 rings in Mad Gear Zone Act 1.";
        public const string ID_A11_NAME = @"Untouchable";
        public const string ID_A11_DESC = @"Clear the E.G.G. Station Zone without taking any damage.";
        public const string ID_LB_NORECORDS = @"No Records";
        public const string ID_UPDATE_TEXT = @"An update is available! This update is required to connect to Xbox LIVE. Update now?";

        public static CultureInfo Culture { get; internal set; }
    }
}


namespace Microsoft.Xna.Framework.GamerServices
{
    public enum MessageBoxIcon : uint
    {
        Alert = 1,
        Warning = 2,
        Error = 3,
        None = 0
    }

    public static class Guide
    {
        public static bool IsVisible { get; private set; } = false;
        public static bool IsScreenSaverEnabled { get; set; }

        public static IAsyncResult BeginShowMessageBox(string title, string text, IEnumerable<string> buttons, int focusButton, MessageBoxIcon icon, AsyncCallback callback, object state)
        {
            return BeginShowMessageBox(PlayerIndex.One, title, text, buttons, focusButton, icon, callback, state);
        }

        public static IAsyncResult BeginShowMessageBox(PlayerIndex player, string title, string text, IEnumerable<string> buttons, int focusButton, MessageBoxIcon icon, AsyncCallback callback, object state)
        {
            var tcs = new TaskCompletionSource<int?>();
            var task = tcs.Task.ContinueWith(t =>
            {
                IsVisible = false;
                callback?.Invoke(t);
            });

            var btns = buttons.ToArray();
            void Handler(object sender, DialogClickEventArgs dialog)
            {
                tcs.SetResult(dialog.Which);
            }

            var builder = new AlertDialog.Builder(Sonic4Ep1.pInstance.Services.GetService<View>().Context)
                .SetTitle(title)
                .SetMessage(text);

            System.Diagnostics.Debug.Assert(btns.Length == 2);
            for (int i = 0; i < btns.Length; i++)
            {
                if (i == focusButton)
                    builder.SetPositiveButton(btns[i], Handler);
                else
                    builder.SetNeutralButton(btns[i], Handler);
            }

            var dialog = builder.Show();

            return task;
        }

        public static int? EndShowMessageBox(IAsyncResult result)
        {
            return ((Task<int?>)result).Result;
        }
    }
}