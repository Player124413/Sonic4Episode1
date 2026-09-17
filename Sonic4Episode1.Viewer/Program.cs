using System;
using System.IO;
using Sonic4Episode1.Viewer;

// viewer [model.amb|model.zno] [textures.amb]
// with no arguments it opens the staged HD ring (the big-endian Xbox 360 pair)
// --survey-motion <dir> reports which submotion key formats the archives under it use
if (args.Length > 0 && args[0] == "--survey-motion")
{
    if (args.Length < 2)
    {
        Console.Error.WriteLine("usage: viewer --survey-motion <directory>");
        return 1;
    }

    return MotionSurvey.Run(args[1]);
}

// --survey-ame <dir> parses every .AME under it and reports node types, byte order and failures
if (args.Length > 0 && args[0] == "--survey-ame")
{
    if (args.Length < 2)
    {
        Console.Error.WriteLine("usage: viewer --survey-ame <directory> [--textures]");
        return 1;
    }

    return AmeSurvey.Run(args[1], args.Contains("--textures"));
}

// --check-motion <dir|file.amb> runs every motion through evaluation and reports what breaks
if (args.Length > 0 && args[0] == "--check-motion")
{
    if (args.Length < 2)
    {
        Console.Error.WriteLine("usage: viewer --check-motion <directory or *_MTN*.AMB>");
        return 1;
    }

    return MotionHarness.Run(args[1]);
}

// --describe-model <archive> lists the objects an archive holds
if (args.Length > 0 && args[0] == "--describe-model")
{
    if (args.Length < 2)
    {
        Console.Error.WriteLine("usage: viewer --describe-model <archive.amb>");
        return 1;
    }

    return MotionDescribe.RunModel(args[1]);
}

// --describe-motion <archive> <first> [last] prints what those motions are made of
if (args.Length > 0 && args[0] == "--describe-motion")
{
    if (args.Length < 3)
    {
        Console.Error.WriteLine("usage: viewer --describe-motion <archive.amb> <first> [last]");
        return 1;
    }

    int first = int.Parse(args[2]);
    int last = args.Length > 3 ? int.Parse(args[3]) : first;
    return MotionDescribe.Run(args[1], first, last);
}

// --stage-shaders <id|name> <content root> reports the permutation each block material selects
if (args.Length > 2 && args[0] == "--stage-shaders")
{
    return StageShaderSurvey.Run(args[1], args[2]);
}

// --deco-shaders <id|name> <content root> does the same for the stage's decorations
if (args.Length > 2 && args[0] == "--deco-shaders")
{
    return StageDecoSurvey.Run(args[1], args[2]);
}

// --stage-info <id|name> [content root] prints what a stage loads, without opening a window
if (args.Length > 1 && args[0] == "--stage-info")
{
    return StageProgram.Describe(args[1], args.Length > 2 ? args[2] : null);
}

// --stage [id|name] [content root] reports what a stage loads; with no id it lists them
if (args.Length > 0 && args[0] == "--stage")
{
    return StageProgram.Run(args.Length > 1 ? args[1] : null, args.Length > 2 ? args[2] : null);
}

string modelPath = args.Length > 0 ? args[0] : null;
string texturePath = args.Length > 1 ? args[1] : null;
string motionPath = args.Length > 2 ? args[2] : null;

if (modelPath == null)
{
    string ring = FindRepoPath("Content/bin/DesktopGL/Content/G_COM/RING");
    if (ring == null)
    {
        Console.Error.WriteLine("could not find Content/bin/DesktopGL/Content/G_COM/RING; pass a model path instead");
        return 1;
    }

    modelPath = Path.Combine(ring, "RING_MDL_HD.AMB");
    texturePath = Path.Combine(ring, "RING_TEX_HD.AMB");
}

if (!File.Exists(modelPath))
{
    Console.Error.WriteLine($"no such model: {modelPath}");
    return 1;
}

Console.WriteLine($"model:    {modelPath}");
Console.WriteLine($"textures: {texturePath ?? "(none)"}");
Console.WriteLine($"motion:   {motionPath ?? "(none)"}");
Console.WriteLine("keys: arrows orbit, +/- zoom, U nnstdshader/BasicEffect, C cycles cull mode,");
Console.WriteLine("      F flips V, W wireframe, space stops the spin, esc quits");
Console.WriteLine("      M cycles motion (past the last is the bind pose), P pauses, [ ] step a frame");

using var viewer = new ViewerGame(modelPath, texturePath, motionPath);
viewer.Run();
return 0;

static string FindRepoPath(string relative)
{
    var dir = new DirectoryInfo(AppContext.BaseDirectory);
    while (dir != null)
    {
        string candidate = Path.Combine(dir.FullName, relative);
        if (Directory.Exists(candidate))
            return candidate;
        dir = dir.Parent;
    }
    return null;
}
