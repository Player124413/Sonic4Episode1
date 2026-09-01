using Microsoft.Xna.Framework;

namespace Sonic4Episode1.Viewer;

// GmMapGetAddMapCameraPos, which is the whole parallax system: a layer smaller than the main map
// covers the same screen travel over a shorter distance, and clamps flat at both edges.
internal static class StageParallax
{
    // GMD_OBJ_LCD_X is GMD_CAMERA_SCALE times the display height, and _Y folds in the draw aspect,
    // which amIPhone sets to height/width. Widescreen is 1280x720. Both truncate to short.
    // Parallax is defined against this, not against the viewer's window, so the layers line up the
    // way they were authored however far out the free camera is zoomed.
    public static readonly Vector2 GameCamera = new Vector2(
        (short)(AppMain.GMD_CAMERA_SCALE * 720f),
        (short)(AppMain.GMD_CAMERA_SCALE * 720f * (720f / 1280f)));

    public static Vector2 ScrollScale(Vector2 layerSize, Vector2 mainSize)
    {
        return new Vector2(
            (layerSize.X - GameCamera.X) / (mainSize.X - GameCamera.X),
            (layerSize.Y - GameCamera.Y) / (mainSize.Y - GameCamera.Y));
    }

    // main is the main camera's disp_pos, so its y is negative: the map runs down in -y
    public static Vector2 CameraFor(Vector2 main, Vector2 layerSize, Vector2 mainSize, Vector2 scale)
    {
        var half = GameCamera * 0.5f;

        float x;
        if (main.X <= half.X)
            x = half.X;
        else if (main.X >= mainSize.X - half.X)
            x = layerSize.X - half.X;
        else
            x = half.X + (main.X - half.X) * scale.X;

        float y;
        if (-main.Y <= half.Y)
            y = -half.Y;
        else if (-main.Y >= mainSize.Y - half.Y)
            y = -(layerSize.Y - half.Y);
        else
            y = -(half.Y + (-main.Y - half.Y) * scale.Y);

        return new Vector2(x, y);
    }
}
