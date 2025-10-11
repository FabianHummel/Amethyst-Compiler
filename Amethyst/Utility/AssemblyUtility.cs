using System.Reflection;

namespace Amethyst.Utility;

public static class AssemblyUtility
{
    private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();
    
    public static void CopyAssemblyFolder(string assemblyPath, string outputDir)
    {
        foreach (var templateFile in GetEmbeddedResources(assemblyPath))
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
    }
    
    public static IEnumerable<string> GetEmbeddedResources(string path)
    {
        return _assembly.GetManifestResourceNames()
            .Where(s => s.StartsWith(path));
    }
}