// Android replacements for the platform pieces the Windows Phone 7 original got from
// the Silverlight/XNA framework: the localized strings and the GamerServices message box.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Android.App;
using Microsoft.Xna.Framework;

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
    /// <summary>
    /// The icons GamerServices supported, kept for source compatibility with the original game.
    /// </summary>
    public enum MessageBoxIcon : uint
    {
        None = 0,
        Alert = 1,
        Warning = 2,
        Error = 3
    }

    /// <summary>
    /// Xbox LIVE guide replacement. Only the message box the game uses for "resume game?"
    /// and the leaderboard notices is implemented, it is shown as a native Android dialog.
    /// </summary>
    public static class Guide
    {
        public static bool IsVisible { get; private set; }
        public static bool IsScreenSaverEnabled { get; set; }

        public static IAsyncResult BeginShowMessageBox(
            string title,
            string text,
            IEnumerable<string> buttons,
            int focusButton,
            MessageBoxIcon icon,
            AsyncCallback callback,
            object state)
        {
            return BeginShowMessageBox(PlayerIndex.One, title, text, buttons, focusButton, icon, callback, state);
        }

        public static IAsyncResult BeginShowMessageBox(
            PlayerIndex player,
            string title,
            string text,
            IEnumerable<string> buttons,
            int focusButton,
            MessageBoxIcon icon,
            AsyncCallback callback,
            object state)
        {
            var captions = new List<string>();
            if (buttons != null)
                captions.AddRange(buttons);

            var completion = new TaskCompletionSource<int?>(TaskCreationOptions.RunContinuationsAsynchronously);
            var result = completion.Task;

            // The game keeps rendering while the box is up, so the box is shown asynchronously.
            // On dismiss the game's callback (which usually clears g_ao_sys_global.is_show_ui)
            // has to run, exactly like the blocking WP7/desktop implementations did.
            var finished = result.ContinueWith(t =>
            {
                IsVisible = false;
                callback?.Invoke(t);
            });

            IsVisible = true;
            Show(captions, title, text, completion);

            return finished;
        }

        private static void Show(List<string> captions, string title, string text, TaskCompletionSource<int?> completion)
        {
            var activity = global::Microsoft.Xna.Framework.Game.Activity ?? Application.Context as Activity;

            if (activity == null)
            {
                IsVisible = false;
                completion.TrySetResult(null);
                return;
            }

            // AlertDialog has to be created on the UI thread, the game may be ticking on its own.
            activity.RunOnUiThread(() =>
            {
                try
                {
                    var builder = new AlertDialog.Builder(activity);

                    if (!string.IsNullOrEmpty(title) && title.Trim().Length > 0)
                        builder.SetTitle(title);

                    builder.SetMessage(text);
                    builder.SetCancelable(false);

                    if (captions.Count > 0)
                        builder.SetPositiveButton(captions[0], (sender, args) => completion.TrySetResult(0));
                    if (captions.Count > 1)
                        builder.SetNegativeButton(captions[1], (sender, args) => completion.TrySetResult(1));
                    if (captions.Count > 2)
                        builder.SetNeutralButton(captions[2], (sender, args) => completion.TrySetResult(2));

                    var dialog = builder.Create();
                    dialog.SetCanceledOnTouchOutside(false);
                    dialog.Show();
                }
                catch (Exception)
                {
                    IsVisible = false;
                    completion.TrySetResult(null);
                }
            });
        }

        public static int? EndShowMessageBox(IAsyncResult result)
        {
            var task = result as Task<int?>;
            if (task == null)
                return null;

            try
            {
                return task.GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
