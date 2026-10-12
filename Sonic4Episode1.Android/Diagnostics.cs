// Startup diagnostics for the Android port.
//
// The app can not be debugged over adb on every device, so every interesting step of the
// startup path is written in four places:
//   * logcat (tag "Sonic4Ep1", plus errors on "Sonic4Ep1-Crash")
//   * a small on screen log, so the last step is still visible if the process dies
//   * <files>/startup-trace.txt and <external files>/startup-trace.txt
//   * <files>/crash-<timestamp>.txt when something went wrong
//
// Everything is wrapped in try/catch: diagnostics must never be the reason for a crash.

using System;
using System.IO;
using System.Text;
using Android.Util;

namespace Sonic4Episode1Android
{
    /// <summary>
    /// Collects what happens while the app starts up so a crash can be reported instead of
    /// just closing the app.
    /// </summary>
    public static class Diagnostics
    {
        public const string LogTag = "Sonic4Ep1";
        public const string CrashLogTag = "Sonic4Ep1-Crash";

        private const string TraceFileName = "startup-trace.txt";

        private static readonly object Gate = new object();
        private static readonly StringBuilder Screen = new StringBuilder();
        private static string lastFailure;

        /// <summary>Everything that happened so far, one line per step.</summary>
        public static string ScreenText
        {
            get { lock (Gate) { return Screen.ToString(); } }
        }

        /// <summary>The last exception that was reported, or null.</summary>
        public static string LastFailure
        {
            get { lock (Gate) { return lastFailure; } }
        }

        /// <summary>Records a startup step.</summary>
        public static void Step(string message)
        {
            string line = DateTime.Now.ToString("HH:mm:ss.fff") + "  " + message;

            lock (Gate)
                Screen.Append(line).Append('\n');

            try { Log.Info(LogTag, line); } catch (Exception) { }

            AppendTrace(line);
        }

        /// <summary>Records an exception, including a stack trace, everywhere it can be read.</summary>
        public static void Fail(string context, Exception exception)
        {
            string body = exception == null ? "(no exception object)" : exception.ToString();
            string text = context + "\n" + body;

            lock (Gate)
            {
                lastFailure = text;
                Screen.Append('\n').Append("!!! ").Append(text).Append('\n');
            }

            try { Log.Error(CrashLogTag, text); } catch (Exception) { }

            AppendTrace("!!! " + context);
            AppendTrace(body);

            try { WriteReport("crash-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".txt", text); } catch (Exception) { }
            try { WriteReport("crash-latest.txt", text); } catch (Exception) { }
        }

        /// <summary>The crash report text that the "Share" button hands to other apps.</summary>
        public static string BuildReport()
        {
            var sb = new StringBuilder();
            sb.Append("Sonic 4: Episode I (Android port) - startup report\n");
            sb.Append("time: ").Append(DateTime.Now.ToString("u")).Append('\n');
            sb.Append("package: ").Append(PackageName).Append('\n');
            sb.Append("android: ").Append(Android.OS.Build.VERSION.Release).Append(" (API ")
              .Append(Android.OS.Build.VERSION.SdkInt).Append(")\n");
            sb.Append("device: ").Append(Android.OS.Build.Manufacturer).Append(' ').Append(Android.OS.Build.Model).Append('\n');
            sb.Append("abis: ").Append(string.Join(", ", Android.OS.Build.SupportedAbis ?? new System.Collections.Generic.List<string>())).Append('\n');
            sb.Append("runtime: ").Append(Environment.Version).Append('\n');
            sb.Append('\n');

            string failure = LastFailure;
            if (!string.IsNullOrEmpty(failure))
            {
                sb.Append("--- failure ---\n").Append(failure).Append('\n');
            }

            sb.Append("--- steps ---\n").Append(ScreenText);
            return sb.ToString();
        }

        public static string PackageName
        {
            get
            {
                try
                {
                    var context = Android.App.Application.Context;
                    return context == null ? "(unknown)" : context.PackageName;
                }
                catch (Exception)
                {
                    return "(unknown)";
                }
            }
        }

        /// <summary>Writes a file into the app's private and its external files directory.</summary>
        public static void WriteReport(string fileName, string text)
        {
            var context = Android.App.Application.Context;
            if (context == null)
                return;

            WriteFile(context.FilesDir == null ? null : context.FilesDir.AbsolutePath, fileName, text);

            try
            {
                var external = context.GetExternalFilesDir(null);
                if (external != null)
                    WriteFile(external.AbsolutePath, fileName, text);
            }
            catch (Exception)
            {
                // No external storage - the private copy is enough.
            }
        }

        private static void WriteFile(string directory, string fileName, string text)
        {
            if (string.IsNullOrEmpty(directory))
                return;

            try
            {
                Directory.CreateDirectory(directory);
                File.WriteAllText(Path.Combine(directory, fileName), text);
            }
            catch (Exception)
            {
                // Diagnostics are best effort only.
            }
        }

        private static void AppendTrace(string line)
        {
            try
            {
                var context = Android.App.Application.Context;
                if (context == null || context.FilesDir == null)
                    return;

                string path = Path.Combine(context.FilesDir.AbsolutePath, TraceFileName);
                File.AppendAllText(path, line + "\n");
            }
            catch (Exception)
            {
                // Ignored on purpose.
            }
        }
    }
}
