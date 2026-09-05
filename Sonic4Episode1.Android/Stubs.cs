
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Hardware;
using Android.Views;
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

        // callers want the index into `buttons`, and EndShowMessageBox casts the IAsyncResult back
        // to Task<int?>, so the task handed to both the callback and the caller has to be the
        // typed one. Don't return the ContinueWith, that's a plain Task and the cast throws
        public static IAsyncResult BeginShowMessageBox(PlayerIndex player, string title, string text, IEnumerable<string> buttons, int focusButton, MessageBoxIcon icon, AsyncCallback callback, object state)
        {
            var tcs = new TaskCompletionSource<int?>();
            _ = tcs.Task.ContinueWith(t =>
            {
                IsVisible = false;
                callback?.Invoke(t);
            });

            var labels = buttons?.ToArray() ?? Array.Empty<string>();
            var activity = Sonic4Episode1.Android.Sonic4Activity.Current;
            if (activity == null)
            {
                tcs.TrySetResult(null);
                return tcs.Task;
            }

            IsVisible = true;

            // the game loop is not the UI thread, and AlertDialog.Show throws off it
            activity.RunOnUiThread(() => ShowMessageBox(activity, title, text, labels, icon, tcs));

            return tcs.Task;
        }

        private static void ShowMessageBox(Activity activity, string title, string text,
            string[] labels, MessageBoxIcon icon, TaskCompletionSource<int?> tcs)
        {
            var builder = new AlertDialog.Builder(activity)
                .SetTitle(title)
                .SetMessage(text)
                .SetCancelable(false)
                .SetIcon(IconResource(icon));

            // AlertDialog has three button slots and reports which slot was hit, not which entry of
            // ours it was, so each handler closes over its own index. More than three cannot be
            // expressed as buttons at all and becomes a list, where Which is the index already
            if (labels.Length > 3)
            {
                builder.SetItems(labels, (sender, e) => tcs.TrySetResult(e.Which));
            }
            else
            {
                for (int i = 0; i < labels.Length; i++)
                {
                    int index = i;
                    void OnClick(object sender, DialogClickEventArgs e) => tcs.TrySetResult(index);

                    switch (i)
                    {
                        case 0: builder.SetPositiveButton(labels[i], OnClick); break;
                        case 1: builder.SetNegativeButton(labels[i], OnClick); break;
                        default: builder.SetNeutralButton(labels[i], OnClick); break;
                    }
                }
            }

            // a button result lands first and wins the race, so this only fires when the dialog
            // went away without one. Without it the task never completes and EndShowMessageBox,
            // which blocks on .Result, hangs the caller forever
            builder.SetOnDismissListener(new DismissListener(() => tcs.TrySetResult(null)));

            builder.Show();
        }

        private static int IconResource(MessageBoxIcon icon)
        {
            switch (icon)
            {
                case MessageBoxIcon.Alert:
                case MessageBoxIcon.Warning:
                case MessageBoxIcon.Error:
                    return global::Android.Resource.Drawable.IcDialogAlert;
                default:
                    return 0;
            }
        }

        private sealed class DismissListener(Action onDismiss)
            : Java.Lang.Object, IDialogInterfaceOnDismissListener
        {
            public void OnDismiss(IDialogInterface dialog) => onDismiss();
        }

        public static int? EndShowMessageBox(IAsyncResult result)
        {
            return ((Task<int?>)result).Result;
        }
    }
}

namespace Microsoft.Devices.Sensors
{
    // amIPhoneAccelerate was written against the iPhone convention, which is g and points the
    // opposite way to Android's. SensorManager reports m/s^2 in the natural device frame, and does
    // not rotate with the display, so the frame itself already matches
    public partial class Accelerometer : Java.Lang.Object, ISensorEventListener
    {
        private readonly SensorManager _manager;
        private readonly Sensor _sensor;

        public Accelerometer()
        {
            _manager = (SensorManager)Sonic4Episode1.Android.Sonic4Activity.Current
                ?.GetSystemService(Context.SensorService);
            _sensor = _manager?.GetDefaultSensor(SensorType.Accelerometer);

            if (_sensor == null)
                throw new AccelerometerFailedException();
        }

        public TimeSpan TimeBetweenUpdates { get; set; } = TimeSpan.FromSeconds(1.0 / 30.0);
        public event EventHandler<SensorReadingEventArgs<AccelerometerReading>> CurrentValueChanged;

        public void Start()
        {
            // the SensorDelay overload also takes a raw microsecond period, which is the only way
            // to honour TimeBetweenUpdates rather than snapping to one of the four named rates
            var microseconds = (int)(TimeBetweenUpdates.TotalMilliseconds * 1000);
            _manager.RegisterListener(this, _sensor, (SensorDelay)microseconds);
        }

        public void Stop()
        {
            _manager.UnregisterListener(this, _sensor);
        }

        public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
        {
        }

        public void OnSensorChanged(SensorEvent e)
        {
            if (e.Values == null || e.Values.Count < 3)
                return;

            const float scale = -1f / 9.80665f;
            var acceleration = new Vector3(e.Values[0] * scale, e.Values[1] * scale, e.Values[2] * scale);

            CurrentValueChanged?.Invoke(this,
                new SensorReadingEventArgs<AccelerometerReading>(
                    new AccelerometerReading { Acceleration = acceleration }));
        }
    }

    public struct AccelerometerReading
    {
        public Vector3 Acceleration;
    }

    public class SensorReadingEventArgs<T> : EventArgs
    {
        public readonly T SensorReading;

        internal SensorReadingEventArgs(T reading)
        {
            SensorReading = reading;
        }
    }

    public class AccelerometerFailedException : Exception
    {
    }
}

