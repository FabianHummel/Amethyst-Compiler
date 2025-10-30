using System.Reflection;

namespace Amethyst.Utility;

/// <summary>Utility methods for working with assemblies and embedded resources.</summary>
public static class AssemblyUtility
{
    /// <summary>The assembly containing the embedded resources. This is typically the executing assembly.</summary>
    private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();

    /// <summary>Copies all embedded resources from the specified assembly path to the output directory.</summary>
    /// <param name="assemblyPath">The path within the assembly to copy resources from.</param>
    /// <param name="outputDir">The directory to copy resources to.</param>
    /// <param name="exclude">An optional file name to exclude from copying.</param>
    public static void CopyAssemblyFolder(string assemblyPath, string outputDir, string? exclude = null)
    {
        foreach (var templateFile in GetEmbeddedResources(assemblyPath))
        {
            if (exclude != null && templateFile.EndsWith(exclude)) continue;
            
            CopyAssemblyResource(assemblyPath, outputDir, templateFile);
        }
    }

    /// <summary>Copies a single embedded resource from the assembly to the output directory.</summary>
    /// <param name="assemblyPath">The path within the assembly to copy resources from.</param>
    /// <param name="outputDir">The directory to copy resources to.</param>
    /// <param name="templateFile">The specific embedded resource to copy.</param>
    private static void CopyAssemblyResource(string assemblyPath, string outputDir, string templateFile)
    {
        var path = templateFile[(assemblyPath.Length + 1)..];
        path = path[..path.LastIndexOf('.')].Replace('.', Path.DirectorySeparatorChar) + path[path.LastIndexOf('.')..];
        path = Path.Combine(outputDir, path);
            
        var dir = path[..path.LastIndexOf(Path.DirectorySeparatorChar)];
        Directory.CreateDirectory(dir);
            
        using var stream = _assembly.GetManifestResourceStream(templateFile)!;
        using var reader = new BinaryReader(stream);
        var rawData = reader.ReadBytes((int)reader.BaseStream.Length);
            
        using var writer = new BinaryWriter(File.OpenWrite(path));
        writer.Write(rawData);
    }

    /// <summary>Gets all embedded resource names that start with the specified path.</summary>
    /// <param name="path">The path prefix to filter embedded resources.</param>
    /// <returns>An enumerable of embedded resource names that start with the specified path.</returns>
    public static IEnumerable<string> GetEmbeddedResources(string path)
    {
        return _assembly.GetManifestResourceNames()
            .Where(s => s.StartsWith(path));
    }
}