using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Sonic4Episode1.Viewer;

// the camera's world-space extent on the map plane, which is all the culling a flat map needs
internal readonly struct RectangleF
{
    public readonly float Left, Right, Bottom, Top;

    public RectangleF(float left, float right, float bottom, float top)
    {
        Left = left;
        Right = right;
        Bottom = bottom;
        Top = top;
    }

    public bool Contains(Vector2 point, float margin)
    {
        return point.X >= Left - margin && point.X <= Right + margin
            && point.Y >= Bottom - margin && point.Y <= Top + margin;
    }
}

internal sealed class StageBlock
{
    public int LayerIndex;
    public int CellX;
    public int CellY;
    public AppMain.MP_BLOCK Mp;
    public Matrix World;

    // block centre in world space, for culling and picking; a block is 64 across
    public Vector2 Centre;
}

// Where a block goes, shared by both arms. No graphics device, no archives: grids in, transforms out.
internal static class StagePlacement
{
    public const float BlockSize = AppMain.GMD_MAP_BLOCK_SIZE;

    // Walks one layer the way gmMapSetDrawMapRangeTvx walks a draw window, only over the whole grid
    // rather than a window: an empty cell defers to its .md overhang, and a cell already claimed by
    // an overhang is skipped. x outer, y inner, so the first writer wins the cell as it does there.
    public static List<StageBlock> Place(StageLayer layer, int layerIndex)
    {
        var placed = new List<StageBlock>();
        var claimed = new bool[layer.Width * layer.Height];

        for (int x = 0; x < layer.Width; x++)
        for (int y = 0; y < layer.Height; y++)
        {
            int cell = y * layer.Width + x;
            var mp = layer.Mp.blocks[cell];
            int px = x;
            int py = y;

            if (mp.id == 0)
            {
                var md = layer.Md.blocks[cell];
                if ((md.ofst_x | md.ofst_y) == 0)
                    continue;

                int source = cell + layer.Width * md.ofst_y + md.ofst_x;
                if (source < 0 || source >= layer.Mp.blocks.Length)
                    continue;

                mp = layer.Mp.blocks[source];
                px += md.ofst_x;
                py += md.ofst_y;
            }

            if (mp.id == 0)
                continue;

            int target = py * layer.Width + px;
            if (target < 0 || target >= claimed.Length || claimed[target])
                continue;

            claimed[target] = true;

            placed.Add(new StageBlock
            {
                LayerIndex = layerIndex,
                CellX = px,
                CellY = py,
                Mp = mp,
                Centre = new Vector2((px + 0.5f) * BlockSize, (-py - 0.5f) * BlockSize),
            });
        }

        return placed;
    }

    // gmMap.cpp:1140-1300. The block model is authored 20 units wide against a 64 unit grid, which
    // is what GMD_OBJ_DRAW_SCALE 3.2 is for, and node 0's translation recentres it on the cell.
    // NN post-multiplies, so this is that chain written back to front for XNA's row vectors.
    public static Matrix BlockMatrix(AppMain.MP_BLOCK mp, NNS_VECTOR node0, float dx, float dy, float z)
    {
        float scale = AppMain.GMD_OBJ_DRAW_SCALE;
        float half = AppMain.GMD_MAP_BLOCK_LOCAL_SIZE * 0.5f;

        var recentre = Matrix.CreateTranslation(-node0.x - half, -node0.y - half, -node0.z);

        // gmMapCreateUsePrimMatrix uses the same flip index, flip_h | (flip_v << 1)
        var flip = Matrix.CreateScale(
            mp.flip_h != 0 ? -scale : scale,
            mp.flip_v != 0 ? -scale : scale,
            scale);

        // rot counts quarter turns: nnRotateZMatrix takes 0x4000 of NN's 0x10000 circle per step
        var rotate = Matrix.CreateRotationZ(mp.rot * MathHelper.PiOver2);

        var place = Matrix.CreateTranslation((dx + 0.5f) * BlockSize, (-dy - 0.5f) * BlockSize, z);

        return recentre * flip * rotate * place;
    }
}
