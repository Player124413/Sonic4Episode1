using System;
using System.IO;

namespace Sonic4Episode1.Viewer;

internal static class StageProgram
{
    // no graphics device, so this runs anywhere and is the quickest way to diff one tree's stage
    // against another's
    public static int Describe(string stage, string root)
    {
        if (!StageTable.TryResolve(stage, out int stageId) || !StageTable.HasBlockMap(stageId))
        {
            Console.Error.WriteLine($"no stage with a block map called {stage}");
            return 1;
        }

        root ??= DefaultRoot();
        if (root == null)
        {
            Console.Error.WriteLine("could not find Content/bin/DesktopGL/Content; pass a content root instead");
            return 1;
        }

        var map = new StageMap(root, stageId);

        try
        {
            map.Load();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 1;
        }

        Console.Write(map.Summary());
        return 0;
    }

    public static int Run(string stage, string root)
    {
        if (stage == null)
        {
            Console.WriteLine("usage: viewer --stage <id|name> [content root]");
            Console.WriteLine();
            foreach (var line in StageTable.Describe())
                Console.WriteLine(line);
            return 0;
        }

        if (!StageTable.TryResolve(stage, out int stageId))
        {
            Console.Error.WriteLine($"no stage called {stage}; run --stage with no argument to list them");
            return 1;
        }

        if (!StageTable.HasBlockMap(stageId))
        {
            Console.Error.WriteLine($"stage {stageId} has no block map");
            return 1;
        }

        root ??= DefaultRoot();
        if (root == null)
        {
            Console.Error.WriteLine("could not find Content/bin/DesktopGL/Content; pass a content root instead");
            return 1;
        }

        var map = new StageMap(root, stageId);
        try
        {
            map.Load();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 1;
        }

        Console.Write(map.Summary());

        if (map.ModelFormat != StageModelFormat.NnObject)
        {
            Console.Error.WriteLine($"{map.Name} in {root} ships {map.ModelFormat} models;"
                + " point --stage at an XBLA or PC tree to see it drawn");
            return 1;
        }

        Console.WriteLine("mouse: right or middle drag pans, wheel zooms about the cursor,");
        Console.WriteLine("       left click reports the block under it");
        Console.WriteLine("keys:  WASD or arrows pan, +/- zoom, G wireframe, O markers, B background,");
        Console.WriteLine("       K decorations, 1-9 toggle a layer, F1 dumps one frame's draw,");
        Console.WriteLine("       T bypasses alpha test, Y forces opaque blending, esc quits");

        using var game = new StageGame(map);
        game.Run();
        return 0;
    }

    private static string DefaultRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            string candidate = Path.Combine(dir.FullName, "Content", "bin", "DesktopGL", "Content");
            if (Directory.Exists(candidate))
                return candidate;
            dir = dir.Parent;
        }
        return null;
    }
}
