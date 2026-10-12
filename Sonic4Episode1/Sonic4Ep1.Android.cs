// Android specific helpers for the Sonic 4: Episode I port.
// This file is shared between every project (see Sonic4Episode1.projitems), the body is
// only compiled when targeting Android so other platforms are not affected.

#if ANDROID || __ANDROID__
using System;
using System.IO;

/// <summary>
/// Small platform helpers used by the shared game code on Android.
/// </summary>
public static class Sonic4Ep1Android
{
    /// <summary>
    /// Directory the game stores its save file ("sonic4_ep1.xml") and its
    /// resume state ("laststate.dat") in.
    /// </summary>
    /// <remarks>
    /// On Android <see cref="Directory.GetCurrentDirectory"/> points at "/" which is not
    /// writable, so the app private files directory has to be used instead.
    /// </remarks>
    public static string StoragePath
    {
        get
        {
            try
            {
                var context = Android.App.Application.Context;
                if (context != null && context.FilesDir != null)
                    return context.FilesDir.AbsolutePath;
            }
            catch (Exception)
            {
                // Fall through to the temp directory below, saves are simply not persistent then.
            }

            return Path.GetTempPath();
        }
    }

    /// <summary>
    /// Opens an url in the device browser (used by the "SEGA" button on the title screen).
    /// </summary>
    public static void OpenUrl(string url)
    {
        try
        {
            var intent = new Android.Content.Intent(Android.Content.Intent.ActionView, Android.Net.Uri.Parse(url));
            intent.AddFlags(Android.Content.ActivityFlags.NewTask);
            Android.App.Application.Context.StartActivity(intent);
        }
        catch (Exception)
        {
            // No browser installed / no activity able to handle the intent.
        }
    }
}
#endif
