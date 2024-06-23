using System.Reflection;
using System.Text;
using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Utility;
using CommandLine;
using static Amethyst.Constants;

namespace Amethyst;

using Tommy;

internal static class Program
{
    private static TomlTable Table { get; set; } = null!;

    private static readonly Context Context = new();

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
                        Console.Out.WriteLine("Starting Amethyst in watch mode...");
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
            Context.MinecraftRoot = dir;
        }
        else if (Environment.OSVersion.Platform == PlatformID.Win32NT ||
                 Environment.OSVersion.Platform == PlatformID.Win32S ||
                 Environment.OSVersion.Platform == PlatformID.Win32Windows ||
                 Environment.OSVersion.Platform == PlatformID.WinCE)
        {
            Context.MinecraftRoot = MINECRAFT_ROOT_WINDOWS;
        }
        else if (Environment.OSVersion.Platform == PlatformID.MacOSX ||
                 Environment.OSVersion.Platform == PlatformID.Unix)
        {
            Context.MinecraftRoot = MINECRAFT_ROOT_MACOS;
        }
        else
        {
            Context.MinecraftRoot = MINECRAFT_ROOT_LINUX;
        }
        
        if (!Directory.Exists(Context.MinecraftRoot))
        {
            var defaultPath = Path.Combine(Environment.CurrentDirectory, SOURCE_DIRECTORY, DEFAULT_OUTPUT_DIRECTORY);
            Console.Error.WriteLine($"Minecraft root not found at default locations, using current directory as source if a world name has been specified for the output. ({defaultPath})");
            Context.MinecraftRoot = null;
        }
    }

    private static void SetSourcePath()
    {
        Context.SourcePath = Path.Combine(Environment.CurrentDirectory, SOURCE_DIRECTORY);
    }
    
    private static void SetDatapackName()
    {
        Context.Datapack!.Name = Table["datapack"]["name"].AsString ?? DEFAULT_DATAPACK_NAME;
    }
    
    private static void SetResourcepackName()
    {
        Context.Resourcepack!.Name = Table["resourcepack"]["name"].AsString ?? DEFAULT_RESOURCEPACK_NAME;
    }

    private static void SetDatapackOutputDirectory()
    {
        var outDir = Table["datapack"]["output"].AsString ?? (string?)null;

        if (outDir == null)
        {
            Context.Datapack!.OutputDir = Path.Combine(Environment.CurrentDirectory, DEFAULT_OUTPUT_DIRECTORY, Context.Datapack!.Name);
        }
        else if (outDir.StartsWith(@".\") || outDir.StartsWith("./"))
        {
            Context.Datapack!.OutputDir = Path.Combine(Environment.CurrentDirectory, outDir[2..], Context.Datapack!.Name);
        }
        else if (outDir.StartsWith(@"\") || outDir.StartsWith("/"))
        {
            Context.Datapack!.OutputDir = Path.Combine(outDir, Context.Datapack!.Name);
        }
        else if (Context.MinecraftRoot == null)
        {
            Context.Datapack!.OutputDir = Path.Combine(Environment.CurrentDirectory, outDir, Context.Datapack!.Name);
        }
        else
        {
            if (!Path.Exists(Path.Combine(Context.MinecraftRoot, "saves", outDir)))
            {
                throw new Exception(
                    $"World '{outDir}' not found in Minecraft saves directory '{Path.Combine(Context.MinecraftRoot, "saves")}'\n" +
                    "Available worlds: " + string.Join(", ", Directory.GetDirectories(Path.Combine(Context.MinecraftRoot, "saves")).Select(Path.GetFileName)));
            }
            Context.Datapack!.OutputDir = Path.Combine(Context.MinecraftRoot, "saves", outDir, "datapacks", Context.Datapack!.Name);
        }
    }
    
    private static void SetResourcepackOutputDirectory()
    {
        var outDir = Table["resourcepack"]["output"].AsString ?? (string?)null;
        
        if (outDir != null && (outDir.StartsWith(@".\") || outDir.StartsWith("./")))
        {
            Context.Resourcepack!.OutputDir = Path.Combine(Environment.CurrentDirectory, outDir[2..], Context.Resourcepack!.Name);
        }
        else if (outDir != null && (outDir.StartsWith(@"\") || outDir.StartsWith("/")))
        {
            Context.Resourcepack!.OutputDir = Path.Combine(outDir, Context.Resourcepack!.Name);
        }
        else if (Context.MinecraftRoot == null)
        {
            Context.Resourcepack!.OutputDir = Path.Combine(Environment.CurrentDirectory, outDir ?? DEFAULT_OUTPUT_DIRECTORY, Context.Resourcepack!.Name);
        }
        else
        {
            Context.Resourcepack!.OutputDir = Path.Combine(Context.MinecraftRoot, "resourcepacks", Context.Resourcepack!.Name);
        }
    }
    
    private static void CreateDatapackOutputFolder()
    {
        if (Directory.Exists(Context.Datapack!.OutputDir))
        {
            Directory.Delete(Context.Datapack!.OutputDir, true);
        }
        
        Directory.CreateDirectory(Context.Datapack!.OutputDir);
    }
    
    private static void CreateResourcepackOutputFolder()
    {
        if (Directory.Exists(Context.Resourcepack!.OutputDir))
        {
            Directory.Delete(Context.Resourcepack!.OutputDir, true);
        }
        
        Directory.CreateDirectory(Context.Resourcepack!.OutputDir);
    }

    private static void CreateDatapackMeta()
    {
        var mcMeta = Path.Combine(Context.Datapack!.OutputDir, "pack.mcmeta");
    
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
        var mcMeta = Path.Combine(Context.Resourcepack!.OutputDir, "pack.mcmeta");
    
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
            var path = Path.Combine(Context.Datapack!.OutputDir, "data", "minecraft", "tags", "functions", "load.json");
            var content = File.ReadAllText(path);
            content = content.Replace("{{loading_functions}}", string.Join("", Context.Datapack!.LoadFunctions.Select(f => $",\n    \"{f}\"")));
            File.WriteAllText(path, content);
        }
        {
            var path = Path.Combine(Context.Datapack!.OutputDir, "data", "minecraft", "tags", "functions", "tick.json");
            var content = File.ReadAllText(path);
            content = content.Replace("{{ticking_functions}}", string.Join("", Context.Datapack!.TickFunctions.Select(f => $",\n    \"{f}\"")));
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
            path = Path.Combine(Context.Datapack!.OutputDir, path);
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
            path = Path.Combine(Context.Resourcepack!.OutputDir, path);
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
        var files = Directory.GetDirectories(Context.SourcePath).SelectMany(dir =>
        {
            var nsName = Path.GetFileName(dir);
            var ns = new Namespace
            {
                Name = nsName,
                Parent = null,
                Context = Context
            };
            Context.Namespaces.Add(nsName, ns);

            return FindCompileTargets(dir).Select(target =>
            {
                var content = File.ReadAllText(target);
                try
                {
                    return content.Parse(ns);
                }
                catch (SyntaxException e)
                {
                    var relativeFile = Path.GetRelativePath(Context.SourcePath, e.File);
                    Console.Error.WriteLine($"Syntax error: {relativeFile} ({e.Line}:{e.PosInLine}): {e.Message}.");
                    throw;
                }
            });
        }).ToList();
        
        CreateFunctionTags();
        
        try
        {
            files.Compile(Context);
            Console.Out.WriteLine("Compilation finished.");
        }
        catch (CompilationException e)
        {
            var relativeFile = Path.GetRelativePath(Context.SourcePath, e.File);
            Console.Error.WriteLine($"Compilation error: {relativeFile} ({e.Line}:{e.PosInLine}): {e.Message}.");
        }
    }
    
    private static void CreateDatapackAndResourcepackContext()
    {
        if (Table.HasKey("datapack"))
        {
            Context.Datapack = new Datapack { Context = Context };
            SetDatapackName();
            SetDatapackOutputDirectory();
            CreateDatapackOutputFolder();
            CopyDatapackTemplate();
            CreateDatapackMeta();
        }
        if (Table.HasKey("resourcepack"))
        {
            Context.Resourcepack = new Resourcepack { Context = Context };
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
        catch
        {
            // ignored
        }
    }
    
    private static void RecompileProject()
    {
        Context.Namespaces.Clear();
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
                Console.Clear();
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
                Console.Clear();
                Console.WriteLine("Configuration changed, reinitializing project...");
                ReinitializeProject();
            }
        }, TaskScheduler.Default);
    }
}