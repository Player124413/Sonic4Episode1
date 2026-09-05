using System;

public static class ContentFailure
{
    // the web head reloads the page. Anywhere else this stays null and the overlay only reports
    public static Action Restart { get; set; }
}
