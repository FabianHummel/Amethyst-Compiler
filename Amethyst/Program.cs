using System.Reflection;
using CommandLine;

namespace Amethyst;

using Tommy;
using Crayon;

internal static class Program
{
    private static TomlTable Table { get; set; }
    private static string MinecraftRoot { get; set; }
    private static string OutputDir { get; set; }
    private static string DataDir { get; set; }
    private static string ProjectName { get; set; }
    private static string ProjectNamespace { get; set; }
    
    private static FileSystemWatcher _watcher;
    
    private static void Main(string[] args)
    {
        CommandLine.Parser.Default.ParseArguments<Options>(args)
            .WithParsed(o =>
            {
                if (o.Watch)
                {
                    var thread = new Thread(() =>
                    {
                        Console.Out.WriteLine("Starting Amethyst in watch mode");
                        _watcher = new FileSystemWatcher();
                        _watcher.Path = Environment.CurrentDirectory;
                        _watcher.NotifyFilter = NotifyFilters.Attributes |
                                                NotifyFilters.CreationTime |
                                                NotifyFilters.FileName |
                                                NotifyFilters.LastAccess |
                                                NotifyFilters.LastWrite |
                                                NotifyFilters.Size |
                                                NotifyFilters.Security;
                        _watcher.Filters.Add("*.amy");
                        _watcher.Filters.Add("amethyst.toml");
                        _watcher.Changed += OnChangedSource;
                        _watcher.Created += OnChangedSource;
                        _watcher.Deleted += OnChangedSource;
                        _watcher.Renamed += OnChangedSource;
                        _watcher.EnableRaisingEvents = true;
                        _watcher.IncludeSubdirectories = true;
                    });
                    thread.Start();
                    
                    ReadAndSetConfigFile();
                    SetMinecraftRootFolder();
                    SetProjectName();
                    SetProjectNamespace();
                    SetOutputDirectory();
                    RecreateProjectStructure();
                    CopyAmethystInternalModule();
                    RecompileProject();

                    while (Console.ReadLine() != "exit")
                    {
                        
                    }
                }
            })
            .WithNotParsed(errors =>
            {
                foreach (var error in errors)
                {
                    Console.Error.WriteLine($"Invalid argument {error.Tag}");
                }
            });
    }
    
    private static void DeleteOutputFiles()
    {
        if (Directory.Exists(OutputDir))
        {
            Directory.Delete(OutputDir, true);
        }
    }
    
    private static void ReadAndSetConfigFile()
    {
        try
        {
            using (var reader = File.OpenText("amethyst.toml"))
            {
                Table = TOML.Parse(reader);
            }
        }
        catch (Exception)
        {
            Console.WriteLine($"Missing or invalid configuration file 'amethyst.toml' found in current working directory '{Environment.CurrentDirectory}'");
            throw;
        }
    }

    private static void SetMinecraftRootFolder()
    {
        if (Table["minecraft"].AsString is { } dir)
        {
            MinecraftRoot = dir;
        }
        else if (Environment.OSVersion.Platform == PlatformID.Win32NT ||
                 Environment.OSVersion.Platform == PlatformID.Win32S ||
                 Environment.OSVersion.Platform == PlatformID.Win32Windows ||
                 Environment.OSVersion.Platform == PlatformID.WinCE)
        {
            MinecraftRoot = Environment.ExpandEnvironmentVariables("%APPDATA%\\.minecraft");
        }
        else if (Environment.OSVersion.Platform == PlatformID.MacOSX ||
                 Environment.OSVersion.Platform == PlatformID.Unix)
        {
            MinecraftRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library/Application Support/minecraft");
        }
        else
        {
            MinecraftRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".minecraft");
        }
        
        if (!Directory.Exists(MinecraftRoot))
        {
            Console.Error.WriteLine($"Minecraft root not found at {MinecraftRoot}, using current directory as root");
            MinecraftRoot = Environment.CurrentDirectory;
        }
    }
    
    private static void SetProjectName()
    {
        ProjectName = Table["name"].AsString ?? throw new Exception("Name not configured");
    }

    private static void SetProjectNamespace()
    {
        ProjectNamespace = Table["namespace"].AsString ?? throw new Exception("Namespace not configured");
    }

    private static void SetOutputDirectory()
    {
        var minecraftRootExists = Directory.Exists(MinecraftRoot);
        var outDir = (string?)Table["output"].AsString;
        if (minecraftRootExists && outDir == null)
        {
            OutputDir = Path.Combine(MinecraftRoot, "datapacks");
        }
        else if (!minecraftRootExists || outDir!.StartsWith("./"))
        {
            OutputDir = Path.Combine(Environment.CurrentDirectory, outDir![2..]);
        }
        else if (outDir.StartsWith("/"))
        {
            OutputDir = outDir;
        }
        else
        {
            OutputDir = Path.Combine(MinecraftRoot, "saves", outDir, "datapacks", ProjectName);
        }
    }
    
    private static IEnumerable<string> FindCompileTargets(string sDir) 
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

    private static void RegenerateOutputFolder()
    {
        if (Directory.Exists(OutputDir))
        {
            Directory.Delete(OutputDir, true);
        }
        
        Directory.CreateDirectory(OutputDir);
    }

    private static void CreateMcMeta()
    {
        var mcMeta = Path.Combine(OutputDir, "pack.mcmeta");

        var assembly = Assembly.GetExecutingAssembly();
        using (var stream = assembly.GetManifestResourceStream("Amethyst.res.pack.mcmeta")!)
        {
            using (var reader = new StreamReader(stream))
            {
                var mcMetaContents = reader.ReadToEnd();
                mcMetaContents = mcMetaContents.Replace("{{description}}", $"\"{Table["description"].AsString}\"");
                mcMetaContents = mcMetaContents.Replace("{{pack_format}}", Table["pack_format"]);
                File.WriteAllText(mcMeta, mcMetaContents);
            }
        }
    }

    private static void CreateDataFolders()
    {
        DataDir = Path.Combine(OutputDir, "data");
        Directory.CreateDirectory(DataDir);
        var namespaceDir = Path.Combine(DataDir, Table["namespace"].AsString, "functions");
        Directory.CreateDirectory(namespaceDir);
        var minecraftDir = Path.Combine(DataDir, "minecraft/tags/functions/");
        Directory.CreateDirectory(minecraftDir);
    }

    public static void CreateFunctionTags(IEnumerable<string> tickingFunctions, IEnumerable<string> initializingFunctions)
    {
        var minecraftDir = Path.Combine(DataDir, "minecraft/tags/functions/");
        
        var assembly = Assembly.GetExecutingAssembly();
        using (var stream = assembly.GetManifestResourceStream("Amethyst.res.tick.json")!)
        {
            using (var reader = new StreamReader(stream))
            {
                var tickingFunctionsTemplate = reader.ReadToEnd();
                var functions = tickingFunctions.Select(i => $"\"{i}\"").ToList();
                var content = string.Join(",\n    ", functions);
                tickingFunctionsTemplate = tickingFunctionsTemplate
                    .Replace("{{ticking_functions}}", content);
                File.WriteAllText(Path.Combine(minecraftDir + "tick.json"), tickingFunctionsTemplate);
            }
        }
        
        using (var stream = assembly.GetManifestResourceStream("Amethyst.res.load.json")!)
        {
            using (var reader = new StreamReader(stream))
            {
                var initializingFunctionsTemplate = reader.ReadToEnd();
                var functions = initializingFunctions.Select(i => $"\"{i}\"").ToList();
                var content = string.Join(",\n    ", functions);
                initializingFunctionsTemplate = initializingFunctionsTemplate
                    .Replace("{{amethyst_init}}", $"\"amethyst:_init\"{(functions.Count > 0 ? "," : "")}")
                    .Replace("{{loading_functions}}", content);
                File.WriteAllText(Path.Combine(minecraftDir + "load.json"), initializingFunctionsTemplate);
            }
        }
    }
    
    private static void CopyAmethystInternalModule()
    {
        var moduleDir = Path.Combine(DataDir, "amethyst");
        
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

    private static void RecreateProjectStructure()
    {
        RegenerateOutputFolder();
        CreateMcMeta();
        CreateDataFolders();
    }
    
    private static void RecompileProject()
    {
        // var _frame = 0;
        // var animation = new Timer(DrawLoadingAnimation, null, 0, 100);
        var targets = FindCompileTargets(Environment.CurrentDirectory);
        try
        {
            CompileTargets(targets);
            Console.Out.WriteLine("Compilation finished.");
        }
        catch
        {
            // ignore
        }
        finally
        {
            // animation.Dispose();
        }
        return;

        // void DrawLoadingAnimation(object? state)
        // {
        //     var frames = new[]
        //     {
        //         "⠋",
        //         "⠙",
        //         "⠹",
        //         "⠸",
        //         "⠼",
        //         "⠴",
        //         "⠦",
        //         "⠧",
        //         "⠇",
        //         "⠏"
        //     };
        //     
        //     Console.Out.Write($"\rCompiling project {frames[_frame++ % frames.Length]}");
        // }
    }

    private static void CompileTargets(IEnumerable<string> targets)
    {
        try
        {
            foreach (var target in targets)
            {
                File.ReadAllText(target)
                    .Tokenize(target)
                    .Parse(target)
                    .Optimize()
                    .Preprocess()
                    .Compile(ProjectNamespace, DataDir, target);
            }
        }
        catch (SyntaxException e)
        {
            var relativeFile = Path.GetRelativePath(Environment.CurrentDirectory, e.File);
            Console.Error.WriteLine($"{relativeFile} ({e.Line}): {e.Message}.");
        }
    }

    private static void OnChangedSource(object sender, FileSystemEventArgs e)
    {
        DeleteOutputFiles();
        ReadAndSetConfigFile();
        SetMinecraftRootFolder();
        SetProjectName();
        SetProjectNamespace();
        SetOutputDirectory();
        RecreateProjectStructure();
        RecompileProject();
    }
}