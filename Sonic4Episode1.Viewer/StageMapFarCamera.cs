using Microsoft.Xna.Framework;

namespace Sonic4Episode1.Viewer;

// gmMapFarCameraGetPos and the per-zone GetCameraPos wrappers, which are identical for zone types
// 0, 1, 2 and 4 apart from which scroll table they take.
internal static class StageMapFarCamera
{
    // gmMapFarCameraGetPos plants the camera here and the zone wrapper then swizzles it onto X
    public const float Depth = 160f;

    // ObjCameraInit for camera 1, GmCamera.cs:41-43. Not the ObjCamera defaults of 45 / 1 / 60000.
    public const float FovDegrees = 40f;
    public const float NearPlane = 0.1f;
    public const float FarPlane = 32768f;

    public static float Speed(StageMapFarScroll scroll, float mapSize) =>
        mapSize == 0f ? 0f : scroll.Width * scroll.LoopNum / mapSize;

    public static float Scrolled(float cameraPos, StageMapFarScroll scroll, float speed)
    {
        float travelled = speed * cameraPos;

        if (scroll.LoopNum == 0)
            return travelled;

        if (scroll.Width * scroll.LoopNum <= travelled)
            return scroll.Pos + scroll.Width;

        if (scroll.Width == 0)
            return scroll.Pos + travelled;

        int whole = (int)travelled / scroll.Width;
        return scroll.Pos + (travelled - whole * scroll.Width);
    }

    // The background lives with X as depth: the zone wrappers end on
    // (x, y, z) <- (160, scrolled y, -scrolled x). That is why the sky turns about Y and why the
    // rock gates read pos.z for the horizontal position.
    public static Vector3 Position(
        Vector2 mainEye, Vector2 mapSize, StageMapFarScroll x, StageMapFarScroll y)
    {
        float scrolledX = Scrolled(mainEye.X, x, Speed(x, mapSize.X));
        float scrolledY = Scrolled(mainEye.Y + mapSize.Y, y, Speed(y, mapSize.Y));

        return new Vector3(Depth, scrolledY, -scrolledX);
    }

    // g_gm_map_far_camera_target_offset
    public static Vector3 Target(Vector3 position, int zoneType) =>
        position + (zoneType == 5 ? new Vector3(0f, 0f, -200f) : new Vector3(-160f, 0f, 0f));
}
