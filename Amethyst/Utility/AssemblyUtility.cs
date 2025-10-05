using System.Reflection;
using System.Text.RegularExpressions;
using Amethyst.Model;

namespace Amethyst.Utility;

public static class AssemblyUtility
{
    public static void CopyAssemblyFolder(string assemblyPath, string outputDir)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var templateFiles = assembly.GetManifestResourceNames()
            .Where(s => s.StartsWith(assemblyPath));
        foreach (var templateFile in templateFiles)
        {
            var path = templateFile[(assemblyPath.Length + 1)..];
            path = path[..path.LastIndexOf('.')].Replace('.', Path.DirectorySeparatorChar) + path[path.LastIndexOf('.')..];
            path = Path.Combine(outputDir, path);
            using (var stream = assembly.GetManifestResourceStream(templateFile)!)
            {
                using (var reader = new BinaryReader(stream))
                {
                    var dir = path[..path.LastIndexOf(Path.DirectorySeparatorChar)];
                    Directory.CreateDirectory(dir);
                    using (var writer = new BinaryWriter(File.OpenWrite(path)))
                    {
                        writer.Write(reader.ReadBytes((int)reader.BaseStream.Length));
                    }
                }
            }
        }
    }
}