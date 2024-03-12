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

    public static void CreateDataFolders(string outDir, TomlTable table)
    {
        var dataDir = outDir + "/data";
        Directory.CreateDirectory(dataDir);
        var namespaceDir = dataDir + "/" + table["namespace"] + "/functions";
        Directory.CreateDirectory(namespaceDir);
        var minecraftDir = dataDir + "/minecraft/tags/functions";
        Directory.CreateDirectory(minecraftDir);
        
        var assembly = Assembly.GetExecutingAssembly();
        using (var stream = assembly.GetManifestResourceStream("Amethyst.res.tick.json"))
        {
            using (var reader = new StreamReader(stream!))
            {
                var tickingFunctions = reader.ReadToEnd();
                tickingFunctions = tickingFunctions.Replace("{{ticking_functions}}", "");
                File.WriteAllText(minecraftDir + "/tick.json", tickingFunctions);
            }
        }
        
        using (var stream = assembly.GetManifestResourceStream("Amethyst.res.load.json"))
        {
            using (var reader = new StreamReader(stream!))
            {
                var loadingFunctions = reader.ReadToEnd();
                loadingFunctions = loadingFunctions.Replace("{{loading_functions}}", "");
                File.WriteAllText(minecraftDir + "/load.json", loadingFunctions);
            }
        }
    }
}