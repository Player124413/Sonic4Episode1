#if !HAS_GAMERSERVICES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#if ANDROID
using Android.App;
using Android.Content;
#elif IOS
using CoreFoundation;
using UIKit;
#elif WASM
using System.Runtime.InteropServices.JavaScript;
#else
using System.Text;
#endif

namespace Microsoft.Xna.Framework.GamerServices
{
    public enum MessageBoxIcon : uint
    {
        None = 0,
        Alert = 1,
        Warning = 2,
        Error = 3
    }

    public class GuideAlreadyVisibleException : Exception
    {
        public GuideAlreadyVisibleException()
            : base("A message box is already showing.")
        {
        }
    }

    public static partial class Guide
    {
        private const int MaxButtons = 3;

        public static bool IsVisible { get; private set; }
        public static bool IsScreenSaverEnabled { get; set; }

        public static IAsyncResult BeginShowMessageBox(string title, string text, IEnumerable<string> buttons, int focusButton, MessageBoxIcon icon, AsyncCallback callback, object state)
        {
            return BeginShowMessageBox(PlayerIndex.One, title, text, buttons, focusButton, icon, callback, state);
        }

        public static IAsyncResult BeginShowMessageBox(PlayerIndex player, string title, string text, IEnumerable<string> buttons, int focusButton, MessageBoxIcon icon, AsyncCallback callback, object state)
        {
            if (title == null)
                throw new ArgumentNullException(nameof(title));
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            if (buttons == null)
                throw new ArgumentNullException(nameof(buttons));
            if (IsVisible)
                throw new GuideAlreadyVisibleException();

            var labels = buttons.ToArray();
            if (labels.Length < 1 || labels.Length > MaxButtons)
                throw new ArgumentException("A message box takes between one and " + MaxButtons + " buttons.", nameof(buttons));
            if (focusButton < 0 || focusButton >= labels.Length)
                throw new ArgumentOutOfRangeException(nameof(focusButton));

            var tcs = new TaskCompletionSource<int?>(state);
            IsVisible = true;

            // callers want the index into `buttons`, and EndShowMessageBox casts the IAsyncResult
            // back to Task<int?>, so the task handed to both the callback and the caller has to be
            // the typed one. Don't return the ContinueWith, that's a plain Task and the cast throws
            _ = tcs.Task.ContinueWith(t =>
            {
                IsVisible = false;
                callback?.Invoke(t);
            });

            ShowMessageBox(title, text, labels, focusButton, icon, tcs);
            return tcs.Task;
        }

        public static int? EndShowMessageBox(IAsyncResult result)
        {
            return ((Task<int?>)result).Result;
        }

#if ANDROID

        private static void ShowMessageBox(string title, string text, string[] labels,
            int focusButton, MessageBoxIcon icon, TaskCompletionSource<int?> tcs)
        {
            var activity = Sonic4Episode1.Android.Sonic4Activity.Current;
            if (activity == null)
            {
                tcs.TrySetResult(null);
                return;
            }

            // the game loop is not the UI thread, and AlertDialog.Show throws off it
            activity.RunOnUiThread(() => Show(activity, title, text, labels, icon, tcs));
        }

        private static void Show(Activity activity, string title, string text,
            string[] labels, MessageBoxIcon icon, TaskCompletionSource<int?> tcs)
        {
            var builder = new AlertDialog.Builder(activity)
                .SetTitle(title)
                .SetMessage(text)
                .SetCancelable(false)
                .SetIcon(IconResource(icon));

            // AlertDialog has three button slots and reports which slot was hit, not which entry of
            // ours it was, so each handler closes over its own index
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

#elif IOS

        private static void ShowMessageBox(string title, string text, string[] labels,
            int focusButton, MessageBoxIcon icon, TaskCompletionSource<int?> tcs)
        {
            // presenting has to happen on the main thread, and the game loop is already on it, so
            // this cannot run inline: it has to go back out to the run loop first
            DispatchQueue.MainQueue.DispatchAsync(() => Show(title, text, labels, focusButton, tcs));
        }

        private static void Show(string title, string text, string[] labels,
            int focusButton, TaskCompletionSource<int?> tcs)
        {
            var presenter = RootViewController();
            if (presenter == null)
            {
                tcs.TrySetResult(null);
                return;
            }

            var alert = UIAlertController.Create(title, text, UIAlertControllerStyle.Alert);

            for (int i = 0; i < labels.Length; i++)
            {
                int index = i;
                var style = i == focusButton
                    ? UIAlertActionStyle.Default
                    : UIAlertActionStyle.Cancel;

                alert.AddAction(UIAlertAction.Create(labels[i], style, _ => tcs.TrySetResult(index)));
            }

            presenter.PresentViewController(alert, true, null);
        }

        // SDL creates its own UIWindow and root controller, and that is what has to present
        private static UIViewController RootViewController()
        {
            var window = UIApplication.SharedApplication.ConnectedScenes
                .OfType<UIWindowScene>()
                .SelectMany(scene => scene.Windows)
                .FirstOrDefault(w => w.IsKeyWindow);

            var root = window?.RootViewController ?? UIApplication.SharedApplication.KeyWindow?.RootViewController;

            while (root?.PresentedViewController != null)
                root = root.PresentedViewController;

            return root;
        }

#elif WASM

        [JSImport("Guide_BeginShowMessageBox", "main.js")]
        internal static partial void Guide_BeginShowMessageBox(int playerIndex,
            string title,
            string text,
            string[] buttons,
            int focusButton,
            int icon,
            [JSMarshalAs<JSType.Function<JSType.Number>>] Action<int> callback);

        private static void ShowMessageBox(string title, string text, string[] labels,
            int focusButton, MessageBoxIcon icon, TaskCompletionSource<int?> tcs)
        {
            Guide_BeginShowMessageBox((int)PlayerIndex.One, title, text, labels, focusButton, (int)icon,
                button => tcs.TrySetResult(button == -1 ? null : button));
        }

#else

        // FNA picks its backend at runtime, so which of the two bindings is safe to P/Invoke is
        // only known then. Calling the wrong one is a DllNotFoundException on the first dialog.
        // This mirrors FNAPlatform's own rule, which is internal and cannot be asked
#if WINDOWS_UWP
        private const bool UseSdl2 = true;
#else
        private static readonly bool UseSdl2 =
            Environment.GetEnvironmentVariable("FNA_PLATFORM_BACKEND") == "SDL2";
#endif

        private static void ShowMessageBox(string title, string text, string[] labels,
            int focusButton, MessageBoxIcon icon, TaskCompletionSource<int?> tcs)
        {
            var window = global::Sonic4Ep1.pInstance?.Window?.Handle ?? IntPtr.Zero;

            var button = UseSdl2
                ? ShowSdl2(title, text, labels, focusButton, icon, window)
                : ShowSdl3(title, text, labels, focusButton, icon, window);

            tcs.TrySetResult(button);
        }

        private static int? ShowSdl2(string title, string text, string[] labels,
            int focusButton, MessageBoxIcon icon, IntPtr window)
        {
            var data = new SDL2.SDL.SDL_MessageBoxData
            {
                flags = Sdl2Flags(icon),
                window = window,
                title = title,
                message = text,
                numbuttons = labels.Length,
                buttons = new SDL2.SDL.SDL_MessageBoxButtonData[labels.Length]
            };

            for (int i = 0; i < labels.Length; i++)
            {
                data.buttons[i].text = labels[i];
                data.buttons[i].buttonid = i;
                if (i == focusButton)
                    data.buttons[i].flags = SDL2.SDL.SDL_MessageBoxButtonFlags.SDL_MESSAGEBOX_BUTTON_RETURNKEY_DEFAULT;
            }

            if (SDL2.SDL.SDL_ShowMessageBox(ref data, out int pressed) != 0)
                return null;

            return pressed;
        }

        private static unsafe int? ShowSdl3(string title, string text, string[] labels,
            int focusButton, MessageBoxIcon icon, IntPtr window)
        {
            // SDL3's binding takes raw utf8 rather than marshalling for us. Packing the title, the
            // message and every label into one blob means one fixed pins all of them at once
            var strings = new string[labels.Length + 2];
            strings[0] = title;
            strings[1] = text;
            Array.Copy(labels, 0, strings, 2, labels.Length);

            var offsets = new int[strings.Length];
            var total = 0;
            for (int i = 0; i < strings.Length; i++)
            {
                offsets[i] = total;
                total += Encoding.UTF8.GetByteCount(strings[i]) + 1;
            }

            var blob = new byte[total];
            for (int i = 0; i < strings.Length; i++)
                Encoding.UTF8.GetBytes(strings[i], 0, strings[i].Length, blob, offsets[i]);

            var buttons = new SDL3.SDL.SDL_MessageBoxButtonData[labels.Length];

            fixed (byte* blobPtr = blob)
            fixed (SDL3.SDL.SDL_MessageBoxButtonData* buttonPtr = buttons)
            {
                for (int i = 0; i < labels.Length; i++)
                {
                    buttons[i].buttonID = i;
                    buttons[i].text = blobPtr + offsets[i + 2];
                    if (i == focusButton)
                        buttons[i].flags = SDL3.SDL.SDL_MessageBoxButtonFlags.SDL_MESSAGEBOX_BUTTON_RETURNKEY_DEFAULT;
                }

                var data = new SDL3.SDL.SDL_MessageBoxData
                {
                    flags = Sdl3Flags(icon),
                    window = window,
                    title = blobPtr + offsets[0],
                    message = blobPtr + offsets[1],
                    numbuttons = labels.Length,
                    buttons = buttonPtr,
                    colorScheme = null
                };

                if (!SDL3.SDL.SDL_ShowMessageBox(ref data, out int pressed))
                    return null;

                return pressed;
            }
        }

        private static SDL2.SDL.SDL_MessageBoxFlags Sdl2Flags(MessageBoxIcon icon)
        {
            switch (icon)
            {
                case MessageBoxIcon.Error: return SDL2.SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_ERROR;
                case MessageBoxIcon.Warning: return SDL2.SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_WARNING;
                case MessageBoxIcon.Alert: return SDL2.SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_INFORMATION;
                default: return 0;
            }
        }

        private static SDL3.SDL.SDL_MessageBoxFlags Sdl3Flags(MessageBoxIcon icon)
        {
            switch (icon)
            {
                case MessageBoxIcon.Error: return SDL3.SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_ERROR;
                case MessageBoxIcon.Warning: return SDL3.SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_WARNING;
                case MessageBoxIcon.Alert: return SDL3.SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_INFORMATION;
                default: return 0;
            }
        }

#endif
    }
}
#endif
