using System.Reflection;
using System.Text;
using Amethyst.Model;
using Amethyst.Utility;
using CommandLine;
using static Amethyst.Constants;

namespace Amethyst;

using Tommy;

internal static class Program
{
    private static TomlTable Table { get; set; } = null!;

    private static readonly Context CompilationContext = new();

    private static FileSystemWatcher _srcWatcher = null!;
    private static FileSystemWatcher _configWatcher = null!;
    private static CancellationTokenSource? _onChangedSourceTokenSource;
    
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
                        _srcWatcher = new FileSystemWatcher();
                        _srcWatcher.Path = Path.Combine(Environment.CurrentDirectory, SOURCE_DIRECTORY);
                        _srcWatcher.NotifyFilter = NotifyFilters.Attributes |
                                                NotifyFilters.CreationTime |
                                                NotifyFilters.FileName |
                                                NotifyFilters.LastAccess |
                                                NotifyFilters.LastWrite |
                                                NotifyFilters.Size |
                                                NotifyFilters.Security;
                        _srcWatcher.Filters.Add("*.*");
                        _srcWatcher.Changed += OnChangedSource;
                        _srcWatcher.Created += OnChangedSource;
                        _srcWatcher.Deleted += OnChangedSource;
                        _srcWatcher.Renamed += OnChangedSource;
                        _srcWatcher.EnableRaisingEvents = true;
                        _srcWatcher.IncludeSubdirectories = true;
                        
                        _configWatcher = new FileSystemWatcher();
                        _configWatcher.Path = Environment.CurrentDirectory;
                        _configWatcher.NotifyFilter = NotifyFilters.Attributes |
                                                     NotifyFilters.CreationTime |
                                                     NotifyFilters.FileName |
                                                     NotifyFilters.LastAccess |
                                                     NotifyFilters.LastWrite |
                                                     NotifyFilters.Size |
                                                     NotifyFilters.Security;
                        _configWatcher.Filters.Add(CONFIG_FILE);
                        _configWatcher.Changed += OnChangedConfig;
                        _configWatcher.Created += OnChangedConfig;
                        _configWatcher.Deleted += OnChangedConfig;
                        _configWatcher.Renamed += OnChangedConfig;
                        _configWatcher.EnableRaisingEvents = true;
                    });
                    thread.Start();
                    
                    ReinitializeProject();

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
    
    private static void ReadAndSetConfigFile()
    {
        try
        {
            using (var reader = File.OpenText(CONFIG_FILE))
            {
                Table = TOML.Parse(reader);
            }
        }
        catch (Exception)
        {
            Console.WriteLine($"Missing or invalid configuration file '{CONFIG_FILE}' found in current working directory '{Environment.CurrentDirectory}'");
            throw;
        }
    }

    private static void SetMinecraftRootFolder()
    {
        if (Table["minecraft"].AsString is { } dir)
        {
            CompilationContext.MinecraftRoot = dir;
        }
        else if (Environment.OSVersion.Platform == PlatformID.Win32NT ||
                 Environment.OSVersion.Platform == PlatformID.Win32S ||
                 Environment.OSVersion.Platform == PlatformID.Win32Windows ||
                 Environment.OSVersion.Platform == PlatformID.WinCE)
        {
            CompilationContext.MinecraftRoot = MINECRAFT_ROOT_WINDOWS;
        }
        else if (Environment.OSVersion.Platform == PlatformID.MacOSX ||
                 Environment.OSVersion.Platform == PlatformID.Unix)
        {
            CompilationContext.MinecraftRoot = MINECRAFT_ROOT_MACOS;
        }
        else
        {
            CompilationContext.MinecraftRoot = MINECRAFT_ROOT_LINUX;
        }
        
        if (!Directory.Exists(CompilationContext.MinecraftRoot))
        {
            var defaultPath = Path.Combine(Environment.CurrentDirectory, SOURCE_DIRECTORY, DEFAULT_OUTPUT_DIRECTORY);
            Console.Error.WriteLine($"Minecraft root not found at default locations, using current directory as source if a world name has been specified for the output. ({defaultPath})");
            CompilationContext.MinecraftRoot = null;
        }
    }

    private static void SetSourcePath()
    {
        CompilationContext.SourcePath = Path.Combine(Environment.CurrentDirectory, SOURCE_DIRECTORY);
    }
    
    private static void SetDatapackName()
    {
        CompilationContext.Datapack!.Name = Table["datapack"]["name"].AsString ?? DEFAULT_DATAPACK_NAME;
    }
    
    private static void SetResourcepackName()
    {
        CompilationContext.Resourcepack!.Name = Table["resourcepack"]["name"].AsString ?? DEFAULT_RESOURCEPACK_NAME;
    }

    private static void SetDatapackOutputDirectory()
    {
        var outDir = Table["datapack"]["output"].AsString ?? (string?)null;

        if (outDir == null)
        {
            CompilationContext.Datapack!.OutputDir = Path.Combine(Environment.CurrentDirectory, DEFAULT_OUTPUT_DIRECTORY, CompilationContext.Datapack!.Name);
        }
        else if (outDir.StartsWith(@".\") || outDir.StartsWith("./"))
        {
            CompilationContext.Datapack!.OutputDir = Path.Combine(Environment.CurrentDirectory, outDir[2..], CompilationContext.Datapack!.Name);
        }
        else if (outDir.StartsWith(@"\") || outDir.StartsWith("/"))
        {
            CompilationContext.Datapack!.OutputDir = Path.Combine(outDir, CompilationContext.Datapack!.Name);
        }
        else if (CompilationContext.MinecraftRoot == null)
        {
            CompilationContext.Datapack!.OutputDir = Path.Combine(Environment.CurrentDirectory, outDir, CompilationContext.Datapack!.Name);
        }
        else
        {
            if (!Path.Exists(Path.Combine(CompilationContext.MinecraftRoot, "saves", outDir)))
            {
                throw new Exception(
                    $"World '{outDir}' not found in Minecraft saves directory '{Path.Combine(CompilationContext.MinecraftRoot, "saves")}'\n" +
                    "Available worlds: " + string.Join(", ", Directory.GetDirectories(Path.Combine(CompilationContext.MinecraftRoot, "saves")).Select(Path.GetFileName)));
            }
            CompilationContext.Datapack!.OutputDir = Path.Combine(CompilationContext.MinecraftRoot, "saves", outDir, "datapacks", CompilationContext.Datapack!.Name);
        }
    }
    
    private static void SetResourcepackOutputDirectory()
    {
        var outDir = Table["resourcepack"]["output"].AsString ?? (string?)null;
        
        if (outDir != null && (outDir.StartsWith(@".\") || outDir.StartsWith("./")))
        {
            CompilationContext.Resourcepack!.OutputDir = Path.Combine(Environment.CurrentDirectory, outDir[2..], CompilationContext.Resourcepack!.Name);
        }
        else if (outDir != null && (outDir.StartsWith(@"\") || outDir.StartsWith("/")))
        {
            CompilationContext.Resourcepack!.OutputDir = Path.Combine(outDir, CompilationContext.Resourcepack!.Name);
        }
        else if (CompilationContext.MinecraftRoot == null)
        {
            CompilationContext.Resourcepack!.OutputDir = Path.Combine(Environment.CurrentDirectory, outDir ?? DEFAULT_OUTPUT_DIRECTORY, CompilationContext.Resourcepack!.Name);
        }
        else
        {
            CompilationContext.Resourcepack!.OutputDir = Path.Combine(CompilationContext.MinecraftRoot, "resourcepacks", CompilationContext.Resourcepack!.Name);
        }
    }
    
    private static void CreateDatapackOutputFolder()
    {
        if (Directory.Exists(CompilationContext.Datapack!.OutputDir))
        {
            Directory.Delete(CompilationContext.Datapack!.OutputDir, true);
        }
        
        Directory.CreateDirectory(CompilationContext.Datapack!.OutputDir);
    }
    
    private static void CreateResourcepackOutputFolder()
    {
        if (Directory.Exists(CompilationContext.Resourcepack!.OutputDir))
        {
            Directory.Delete(CompilationContext.Resourcepack!.OutputDir, true);
        }
        
        Directory.CreateDirectory(CompilationContext.Resourcepack!.OutputDir);
    }

    private static void CreateDatapackMeta()
    {
        var mcMeta = Path.Combine(CompilationContext.Datapack!.OutputDir, "pack.mcmeta");
    
        var assembly = Assembly.GetExecutingAssembly();
        using (var stream = assembly.GetManifestResourceStream("Amethyst.Resources.datapack.pack.mcmeta")!)
        {
            using (var reader = new StreamReader(stream))
            {
                var mcMetaContents = reader.ReadToEnd();
                var description = Table["datapack"]["description"].AsString ?? DEFAULT_DATAPACK_DESCRIPTION;
                mcMetaContents = mcMetaContents.Replace("{{description}}", $"\"{description}\"");
                var packFormat = Table["datapack"]["pack_format"].AsInteger ?? DEFAULT_DATAPACK_FORMAT;
                mcMetaContents = mcMetaContents.Replace("{{pack_format}}", packFormat.ToString());
                File.WriteAllText(mcMeta, mcMetaContents);
            }
        }
    }
    
    private static void CreateResourcepackMeta()
    {
        var mcMeta = Path.Combine(CompilationContext.Resourcepack!.OutputDir, "pack.mcmeta");
    
        var assembly = Assembly.GetExecutingAssembly();
        using (var stream = assembly.GetManifestResourceStream("Amethyst.Resources.resourcepack.pack.mcmeta")!)
        {
            using (var reader = new StreamReader(stream))
            {
                var mcMetaContents = reader.ReadToEnd();
                var description = Table["resourcepack"]["description"].AsString ?? DEFAULT_RESOURCEPACK_DESCRIPTION;
                mcMetaContents = mcMetaContents.Replace("{{description}}", $"\"{description}\"");
                var packFormat = Table["resourcepack"]["pack_format"].AsInteger ?? DEFAULT_RESOURCEPACK_FORMAT;
                mcMetaContents = mcMetaContents.Replace("{{pack_format}}", packFormat.ToString());
                File.WriteAllText(mcMeta, mcMetaContents);
            }
        }
    }
    
    private static void CreateFunctionTags()
    {
        {
            var path = Path.Combine(CompilationContext.Datapack!.OutputDir, "data", "minecraft", "tags", "functions", "load.json");
            var content = File.ReadAllText(path);
            content = content.Replace("{{loading_functions}}", string.Join("", CompilationContext.Datapack!.LoadFunctions.Select(f => "," + f.GetCallablePath())));
            File.WriteAllText(path, content);
        }
        {
            var path = Path.Combine(CompilationContext.Datapack!.OutputDir, "data", "minecraft", "tags", "functions", "tick.json");
            var content = File.ReadAllText(path);
            content = content.Replace("{{tick_functions}}", string.Join("", CompilationContext.Datapack!.TickFunctions.Select(f => "," + f.GetCallablePath())));
            File.WriteAllText(path, content);
        }
    }
    
    private static void CopyDatapackTemplate()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var templateFiles = assembly.GetManifestResourceNames().Where(s => s.StartsWith("Amethyst.Resources.datapack"));
        foreach (var templateFile in templateFiles)
        {
            var path = templateFile["Amethyst.Resources.datapack.".Length..];
            path = path[..path.LastIndexOf('.')].Replace('.', Path.DirectorySeparatorChar) + path[path.LastIndexOf('.')..];
            path = Path.Combine(CompilationContext.Datapack!.OutputDir, path);
            using (var stream = assembly.GetManifestResourceStream(templateFile)!)
            {
                using (var reader = new BinaryReader(stream))
                {
                    Directory.CreateDirectory(path[..path.LastIndexOf(Path.DirectorySeparatorChar)]);
                    using (var writer = new BinaryWriter(File.OpenWrite(path)))
                    {
                        writer.Write(reader.ReadBytes((int)reader.BaseStream.Length));
                    }
                }
            }
        }
    }
    
    private static void CopyResourcepackTemplate()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var templateFiles = assembly.GetManifestResourceNames().Where(s => s.StartsWith("Amethyst.Resources.resourcepack"));
        foreach (var templateFile in templateFiles)
        {
            var path = templateFile["Amethyst.Resources.resourcepack.".Length..];
            path = path[..path.LastIndexOf('.')].Replace('.', Path.DirectorySeparatorChar) + path[path.LastIndexOf('.')..];
            path = Path.Combine(CompilationContext.Resourcepack!.OutputDir, path);
            using (var stream = assembly.GetManifestResourceStream(templateFile)!)
            {
                using (var reader = new BinaryReader(stream))
                {
                    Directory.CreateDirectory(path[..path.LastIndexOf(Path.DirectorySeparatorChar)]);
                    using (var writer = new BinaryWriter(File.OpenWrite(path)))
                    {
                        writer.Write(reader.ReadBytes((int)reader.BaseStream.Length));
                    }
                }
            }
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
    
    private static void CompileProject()
    {
        var IL = Directory.GetDirectories(CompilationContext.SourcePath).SelectMany(dir =>
        {
            var ns = new Namespace
            {
                Name = Path.GetFileName(dir),
                Context = CompilationContext
            };
            CompilationContext.Namespaces.Add(ns);
            
            return FindCompileTargets(dir).Select(target =>
            {
                return File.ReadAllText(target)
                    .Tokenize(ns)
                    .Parse(ns);
            });
        });
        
        CreateFunctionTags();
        
        try
        {
            IL.Compile(CompilationContext);
            Console.Out.WriteLine("Compilation finished.");
        }
        catch (SyntaxException e)
        {
            var relativeFile = Path.GetRelativePath(CompilationContext.SourcePath, e.File);
            Console.Error.WriteLine($"{relativeFile} ({e.Line}): {e.Message}.");
        }
    }
    
    private static void CreateDatapackAndResourcepackContext()
    {
        if (Table.HasKey("datapack"))
        {
            CompilationContext.Datapack = new Datapack();
            SetDatapackName();
            SetDatapackOutputDirectory();
            CreateDatapackOutputFolder();
            CopyDatapackTemplate();
            CreateDatapackMeta();
        }
        if (Table.HasKey("resourcepack"))
        {
            CompilationContext.Resourcepack = new Resourcepack();
            SetResourcepackName();
            SetResourcepackOutputDirectory();
            CreateResourcepackOutputFolder();
            CopyResourcepackTemplate();
            CreateResourcepackMeta();
        }
    }

    private static void ReinitializeProject()
    {
        try
        {
            ReadAndSetConfigFile();
            SetMinecraftRootFolder();
            SetSourcePath();
            RecompileProject();
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.Message);
        }
    }
    
    private static void RecompileProject()
    {
        CreateDatapackAndResourcepackContext();
        CompileProject();
    }

    private static void OnChangedSource(object sender, FileSystemEventArgs e)
    {
        _onChangedSourceTokenSource?.Cancel();
        _onChangedSourceTokenSource = new CancellationTokenSource();
        Task.Delay(100, _onChangedSourceTokenSource.Token).ContinueWith(t => {
            if (t.IsCompletedSuccessfully)
            {
                RecompileProject();
            }
        }, TaskScheduler.Default);
    }
    
    private static void OnChangedConfig(object sender, FileSystemEventArgs e)
    {
        _onChangedSourceTokenSource?.Cancel();
        _onChangedSourceTokenSource = new CancellationTokenSource();
        Task.Delay(100, _onChangedSourceTokenSource.Token).ContinueWith(t => {
            if (t.IsCompletedSuccessfully)
            {
                Console.WriteLine("Configuration changed, reinitializing project...");
                ReinitializeProject();
            }
        }, TaskScheduler.Default);
    }
}