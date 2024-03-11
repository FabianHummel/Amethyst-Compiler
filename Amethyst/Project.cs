using System.Reflection;
using Tommy;

namespace Amethyst;

public static class Project
{
    public static IEnumerable<string> FindCompileTargets(string sDir) 
    {
        foreach (var f in Directory.GetFiles(sDir, "*.amy"))
        {
            yield return f;
        }

        foreach (var d in Directory.GetDirectories(sDir)) 
        {
            foreach (var f in FindCompileTargets(d))
            {
                yield return f;
            }
        }
    }

    public static void RegenerateOutputFolder(string outputDir)
    {
        if (Directory.Exists(outputDir))
        {
            Directory.Delete(outputDir, true);
        }
        
        Directory.CreateDirectory(outputDir);
    }
    
    public static void CreateMcMeta(string outputDir, TomlTable table)
    {
        var mcMeta = outputDir + "/pack.mcmeta";

        var assembly = Assembly.GetExecutingAssembly();
        using (var stream = assembly.GetManifestResourceStream("Amethyst.res.pack.mcmeta"))
        {
            using (var reader = new StreamReader(stream!))
            {
                var mcMetaContents = reader.ReadToEnd();
                mcMetaContents = mcMetaContents.Replace("{{description}}", table["description"]);
                mcMetaContents = mcMetaContents.Replace("{{pack_format}}", table["pack_format"]);
                File.WriteAllText(mcMeta, mcMetaContents);
            }
        }
    }
}