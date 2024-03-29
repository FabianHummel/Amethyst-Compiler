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
        var mcMeta = Path.Combine(outputDir, "pack.mcmeta");

        var assembly = Assembly.GetExecutingAssembly();
        using (var stream = assembly.GetManifestResourceStream("Amethyst.res.pack.mcmeta")!)
        {
            using (var reader = new StreamReader(stream))
            {
                var mcMetaContents = reader.ReadToEnd();
                mcMetaContents = mcMetaContents.Replace("{{description}}", $"\"{table["description"].AsString}\"");
                mcMetaContents = mcMetaContents.Replace("{{pack_format}}", table["pack_format"]);
                File.WriteAllText(mcMeta, mcMetaContents);
            }
        }
    }

    public static void CreateDataFolders(string outDir, TomlTable table)
    {
        var dataDir = Path.Combine(outDir, "data");
        Directory.CreateDirectory(dataDir);
        var namespaceDir = Path.Combine(dataDir, table["namespace"].AsString, "functions");
        Directory.CreateDirectory(namespaceDir);
        var minecraftDir = Path.Combine(dataDir, "minecraft/tags/functions/");
        Directory.CreateDirectory(minecraftDir);
    }

    public static void CreateFunctionTags(string outDir, Environment context)
    {
        var minecraftDir = Path.Combine(outDir, "minecraft/tags/functions/");
        
        var assembly = Assembly.GetExecutingAssembly();
        using (var stream = assembly.GetManifestResourceStream("Amethyst.res.tick.json")!)
        {
            using (var reader = new StreamReader(stream))
            {
                var tickingFunctions = reader.ReadToEnd();
                var content = string.Join(",\n    ", context.TickingFunctions.Select(i => $"\"{i}\""));
                tickingFunctions = tickingFunctions.Replace("{{ticking_functions}}", content);
                File.WriteAllText(Path.Combine(minecraftDir + "tick.json"), tickingFunctions);
            }
        }
        
        using (var stream = assembly.GetManifestResourceStream("Amethyst.res.load.json")!)
        {
            using (var reader = new StreamReader(stream))
            {
                var loadingFunctions = reader.ReadToEnd();
                var functions = context.InitializingFunctions.Select(i => $"\"{i}\"").ToList();
                var content = string.Join(",\n    ", functions);
                loadingFunctions = loadingFunctions
                    .Replace("{{amethyst_init}}", $"\"amethyst:_init\"{(functions.Count > 0 ? "," : "")}")
                    .Replace("{{loading_functions}}", content);
                File.WriteAllText(Path.Combine(minecraftDir + "load.json"), loadingFunctions);
            }
        }
    }
    
    public static void CopyAmethystInternalModule(string outDir)
    {
        var moduleDir = Path.Combine(outDir, "amethyst");
        
        var assembly = Assembly.GetExecutingAssembly();
        var templateFiles = assembly.GetManifestResourceNames().Where(s => s.StartsWith("Amethyst.res.amethyst"));
        foreach (var templateFile in templateFiles)
        {
            var path = templateFile["Amethyst.res.amethyst".Length..];
            path = path[..path.LastIndexOf('.')].Replace(".", "/") + path[path.LastIndexOf('.')..];
            path = moduleDir + path;
            using (var stream = assembly.GetManifestResourceStream(templateFile)!)
            {
                using (var reader = new StreamReader(stream))
                {
                    var content = reader.ReadToEnd();
                    Directory.CreateDirectory(path[..path.LastIndexOf('/')]);
                    File.WriteAllText(path, content);
                }
            }
        }
    }
}