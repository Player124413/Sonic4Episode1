using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Sonic4Episode1.Viewer;

internal enum StageMarkerKind
{
    Event,
    Decoration,
    Ring,
}

internal sealed class StageMarker
{
    public StageMarkerKind Kind;
    public int Id;
    public Vector2 Position;

    // events carry a trigger rectangle in the same 256 unit space, empty on everything else
    public Vector2 RectMin;
    public Vector2 RectMax;
    public bool HasRect;

    public ushort Flag;
    public ushort User;
}

// Events, decorations and rings out of .ev, .dc and .rg. None of these are drawn as what they are:
// the viewer has no object models and does not want them, so they are markers on the map with an
// id you can read off. Placement is the interesting part and it is exact.
internal static class StageMarkers
{
    public const float CellSize = AppMain.GMD_MAP_EVENT_BLOCK_SIZE;

    public static List<StageMarker> Build(StageMap map)
    {
        var markers = new List<StageMarker>();

        Walk(map.Ev.cells, map.Ev.map_w, (cell, info) => markers.Add(new StageMarker
        {
            Kind = StageMarkerKind.Event,
            Id = info.id,
            Position = World(cell, map.Ev.map_w, info.x, info.y),
            RectMin = World(cell, map.Ev.map_w, info.x + info.rect_x, info.y + info.rect_y),
            RectMax = World(cell, map.Ev.map_w, info.x + info.rect_x + info.rect_w, info.y + info.rect_y + info.rect_h),
            HasRect = info.rect_w != 0 || info.rect_h != 0,
            Flag = info.flag,
            User = info.user,
        }));

        Walk(map.Dc.cells, map.Dc.map_w, (cell, info) => markers.Add(new StageMarker
        {
            Kind = StageMarkerKind.Decoration,
            Id = info.id,
            Position = World(cell, map.Dc.map_w, info.x, info.y),
        }));

        Walk(map.Rg.cells, map.Rg.map_w, (cell, info) => markers.Add(new StageMarker
        {
            Kind = StageMarkerKind.Ring,
            Id = -1,
            Position = World(cell, map.Rg.map_w, info.x, info.y),
        }));

        return markers;
    }

    // x and y are within the cell, which is why they fit in a byte
    private static Vector2 World(int cell, int map_w, int x, int y)
    {
        return new Vector2(
            (cell % map_w) * CellSize + x,
            -((cell / map_w) * CellSize + y));
    }

    private static void Walk<T>(T[][] cells, int map_w, Action<int, T> visit)
    {
        if (cells == null)
            return;

        for (int cell = 0; cell < cells.Length; cell++)
        {
            foreach (var info in cells[cell])
                visit(cell, info);
        }
    }
}
