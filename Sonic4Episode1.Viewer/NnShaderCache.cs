using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace Sonic4Episode1.Viewer;

// The 284 built nnstdshader permutations, read straight out of the repo rather than copied to the
// output, and kept once per device rather than once per thing being drawn.
internal sealed class NnShaderCache
{
    private readonly GraphicsDevice _device;
    private readonly Dictionary<string, NnStdShaderEffect> _shaders =
        new Dictionary<string, NnStdShaderEffect>(StringComparer.OrdinalIgnoreCase);

    private string _directory;
    private bool _reported;

    public NnShaderCache(GraphicsDevice device)
    {
        _device = device;
    }

    public NnStdShaderEffect Get(string permutation)
    {
        if (permutation == null)
            return null;

        if (_shaders.TryGetValue(permutation, out var shader))
            return shader;

        shader = Load(permutation);
        _shaders[permutation] = shader;
        return shader;
    }

    private NnStdShaderEffect Load(string permutation)
    {
        _directory ??= FindRepoDirectory("Shaders/FNA/nnstd");
        if (_directory == null)
        {
            if (!_reported)
                Console.Error.WriteLine("could not find Shaders/FNA/nnstd; run Shaders/nn/Build-Permutations.ps1");
            _reported = true;
            return null;
        }

        string path = Path.Combine(_directory, permutation + ".fxb");
        if (!File.Exists(path))
        {
            Console.Error.WriteLine($"no shader built for key {permutation}");
            return null;
        }

        try
        {
            return new NnStdShaderEffect(_device, File.ReadAllBytes(path));
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"ubershader {permutation} failed to load: " + ex);
            return null;
        }
    }

    // what Build-Permutations.ps1 actually produced, for the surveys that report whether a key a
    // material selects has a shader behind it
    public static HashSet<string> BuiltPermutations()
    {
        var dir = FindRepoDirectory("Shaders/FNA/nnstd");
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (dir != null)
        {
            foreach (var path in Directory.EnumerateFiles(dir, "*.fxb"))
                set.Add(Path.GetFileNameWithoutExtension(path));
        }

        return set;
    }

    public static string FindRepoDirectory(string relative)
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
}
