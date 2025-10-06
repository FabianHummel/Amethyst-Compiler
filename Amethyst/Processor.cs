using System.Globalization;
using System.Text.RegularExpressions;
using System.Reflection;
using Amethyst.Model;
using Amethyst.Utility;
using Tomlet;

namespace Amethyst;

using static Constants;
using static ConsoleUtility;

public class Processor
{
    private readonly bool _rethrowErrors;
    
    public Context Context { get; }
    
    public Processor(string rootDir, CompilerFlags compilerFlags, bool rethrowErrors = false)
    {
        _rethrowErrors = rethrowErrors;
        var configuration = ReadAndSetConfigFile(rootDir);
        if (!Regex.IsMatch(configuration.ProjectId, "^[a-z0-9_]+$"))
        {
            throw new InvalidOperationException("Project ID must contain only lowercase letters, digits, or underscores.");
        }
        Context = new Context(rootDir, compilerFlags, configuration);
        PrintConfigInfo(configuration);
        RecompileProject();
    }
    
    public Processor(Context context, bool rethrowErrors = false)
    {
        _rethrowErrors = rethrowErrors;
        Context = new Context(context);
        RecompileProject();
    }
    
    private void RecompileProject()
    {
        try
        {
            CreateDatapackAndResourcepackContext(Context.Configuration);
            CompileProject(Context.Configuration);
        }
        catch (Exception e)
        {
            PrintError(e.Message);
            
            if (Context.CompilerFlags.HasFlag(CompilerFlags.Debug))
            {
                PrintError(e.StackTrace ?? string.Empty);
            }
            
            if (_rethrowErrors)
            {
                throw;
            }
        }
    }

    private static Configuration ReadAndSetConfigFile(string rootDir)
    {
        try
        {
            var configPath = Path.Combine(rootDir, ConfigFile);
            using var reader = File.OpenText(configPath);
            var tomlConfiguration = reader.ReadToEnd();
            return TomletMain.To<Configuration>(tomlConfiguration);
        }
        catch (Exception)
        {
            PrintError($"Missing or invalid configuration file '{ConfigFile}' found in project directory '{rootDir}'");
            throw;
        }
    }

    private void PrintConfigInfo(Configuration config)
    {
        var minecraftRootFolder = GetMinecraftRootFolder(config.MinecraftRoot);
        if (!Directory.Exists(minecraftRootFolder))
        {
            var defaultPath = Path.Combine(Context.RootDir, SourceDirectory, DefaultOutputDirectory);
            PrintWarning($"Minecraft root not found at default locations, using current directory as source if a world name has been specified for the output. ({defaultPath})");
        }
        
        if (!Context.CompilerFlags.HasFlag(CompilerFlags.Debug))
        {
            return;
        }
        
        PrintDebug($"Minecraft root = '{config.MinecraftRoot}'.");

        if (config.Datapack is { } datapack)
        {
            PrintDebug($"Datapack name = '{datapack.Name}'.");
        }

        if (config.Resourcepack is { } resourcepack)
        {
            PrintDebug($"Resourcepack name = '{resourcepack.Name}'.");
        }
    }

    private static string GetMinecraftRootFolder(string? minecraftRoot)
    {
        if (minecraftRoot != null)
        {
            return minecraftRoot;
        }
        
        if (Environment.OSVersion.Platform == PlatformID.Win32NT ||
            Environment.OSVersion.Platform == PlatformID.Win32S ||
            Environment.OSVersion.Platform == PlatformID.Win32Windows ||
            Environment.OSVersion.Platform == PlatformID.WinCE)
        {
            return MinecraftRootWindows;
        }
        if (Environment.OSVersion.Platform == PlatformID.MacOSX ||
                 Environment.OSVersion.Platform == PlatformID.Unix)
        {
            return MinecraftRootMacos;
        }
        
        return MinecraftRootLinux;
    }

    private void SetDatapackOutputDirectory(Datapack datapack, string? minecraftRoot)
    {
        string actualOutDir;
        if (datapack.Output == null && minecraftRoot != null)
        {
            var minecraftRootFolder = GetMinecraftRootFolder(minecraftRoot);
            if (Path.Exists(Path.Combine(minecraftRootFolder, "world")))
            {
                actualOutDir = Path.Combine(minecraftRootFolder, "world", "datapacks");
                goto end;
            }
        }
        
        if (datapack.Output == null)
        {
            actualOutDir = Path.Combine(Context.RootDir, DefaultOutputDirectory);
        }
        else if (minecraftRoot == null)
        {
            actualOutDir = Path.Combine(Context.RootDir, datapack.Output);
        }
        else
        {
            var minecraftRootFolder = GetMinecraftRootFolder(minecraftRoot);
            if (!Path.Exists(Path.Combine(minecraftRootFolder, "saves", datapack.Output)))
            {
                throw new InvalidOperationException(
                    $"World '{datapack.Output}' not found in Minecraft saves directory '{Path.Combine(minecraftRootFolder, "saves")}'\n" +
                    "Available worlds: " + string.Join(", ", Directory.GetDirectories(Path.Combine(minecraftRootFolder, "saves")).Select(Path.GetFileName)));

            }

            actualOutDir = Path.Combine(minecraftRootFolder, "saves", datapack.Output, "datapacks");
        }
        
        end:
        datapack.OutputDir = Path.Combine(actualOutDir, datapack.Name);

        if (Context.CompilerFlags.HasFlag(CompilerFlags.Debug))
        {
            PrintDebug($"Datapack output directory: '{datapack.OutputDir}'.");
        }
    }
    
    private void SetResourcepackOutputDirectory(Resourcepack resourcepack, string? minecraftRoot)
    {
        string actualOutDir;
        if (resourcepack.Output == null && minecraftRoot != null)
        {
            var minecraftRootFolder = GetMinecraftRootFolder(minecraftRoot);
            if (Path.Exists(Path.Combine(minecraftRootFolder, "world")))
            {
                actualOutDir = Path.Combine(minecraftRootFolder, "world", "_tmp_resourcepack");
                Directory.CreateDirectory(actualOutDir);
                goto end;
            }
        }
        
        if (resourcepack.Output == null)
        {
            actualOutDir = Path.Combine(Context.RootDir, DefaultOutputDirectory);
        }
        else if (minecraftRoot == null)
        {
            actualOutDir = Path.Combine(Context.RootDir, resourcepack.Output);
        }
        else
        {
            var minecraftRootFolder = GetMinecraftRootFolder(minecraftRoot);
            actualOutDir = Path.Combine(minecraftRootFolder, "resourcepacks", resourcepack.Output);
        }
        
        end:
        resourcepack.OutputDir = Path.Combine(actualOutDir, resourcepack.Name);

        if (Context.CompilerFlags.HasFlag(CompilerFlags.Debug))
        {
            PrintDebug($"Resourcepack output directory: '{resourcepack.OutputDir}'.");
        }
    }
    
    private static void CreateOutputFolder(string outputDir)
    {
        if (Directory.Exists(outputDir))
        {
            Directory.Delete(outputDir, true);
        }
        
        Directory.CreateDirectory(outputDir);
    }

    private void CreateMeta(string outputDir, string datapackOrResourcepack, Func<string, string> substitutions)
    {
        var cts = PrintLongTask($"Creating {datapackOrResourcepack.ToLower(CultureInfo.InvariantCulture)} meta file", out var getElapsed);
        var mcMeta = Path.Combine(outputDir, "pack.mcmeta");
    
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream($"Amethyst.Resources.{datapackOrResourcepack.ToLower(CultureInfo.InvariantCulture)}.pack.mcmeta")!;
        using var reader = new StreamReader(stream);
        var mcMetaContents = reader.ReadToEnd();
        File.WriteAllText(mcMeta, substitutions(mcMetaContents));

        cts.Cancel();
        
        if (Context.CompilerFlags.HasFlag(CompilerFlags.Debug))
        {
            PrintDebugMessageWithTime($"{datapackOrResourcepack} meta file created at '{mcMeta}'.", getElapsed());
        }
    }
    
    private void CreateFunctionTags(Datapack datapack)
    {
        var cts = PrintLongTask("Creating function tags", out var getElapsed);
        {
            var path = Path.Combine(datapack.OutputDir, $"data/minecraft/tags/{DatapackFunctionsDirectory}/load.json");
            var content = File.ReadAllText(path);
            var loadFunctionsText = string.Join("", datapack.LoadFunctions.Select(f => $",\n    \"{f}\""));
            content = content.Replace(Substitutions["loading_functions"], loadFunctionsText);
            File.WriteAllText(path, content);
        }
        {
            var path = Path.Combine(datapack.OutputDir, $"data/minecraft/tags/{DatapackFunctionsDirectory}/tick.json");
            var content = File.ReadAllText(path);
            var tickFunctionsText = string.Join("", datapack.TickFunctions.Select(f => $",\n    \"{f}\""));
            content = content.Replace(Substitutions["ticking_functions"], tickFunctionsText);
            File.WriteAllText(path, content);
        }
        
        cts.Cancel();

        if (Context.CompilerFlags.HasFlag(CompilerFlags.Debug))
        {
            PrintDebugMessageWithTime("Function tags created.", getElapsed());
        }
    }
    
    private void CopyTemplate(string outputDir, string datapackOrResourcepack)
    {
        var cts = PrintLongTask($"Copying {datapackOrResourcepack.ToLower(CultureInfo.InvariantCulture)} template", out var getElapsed);
        
        AssemblyUtility.CopyAssemblyFolder($"Amethyst.Resources.{datapackOrResourcepack.ToLower(CultureInfo.InvariantCulture)}", outputDir);
        
        cts.Cancel();

        if (Context.CompilerFlags.HasFlag(CompilerFlags.Debug))
        {
            PrintDebugMessageWithTime($"{datapackOrResourcepack} template copied.", getElapsed());
        }
    }
    
    private void CreateDatapackAndResourcepackContext(Configuration configuration)
    {
        var cts = PrintLongTask("Creating project structure", out var getElapsed);
        
        if (configuration.Datapack is { } datapack)
        {
            SetDatapackOutputDirectory(datapack, configuration.MinecraftRoot);
            CreateOutputFolder(datapack.OutputDir);
            CopyTemplate(datapack.OutputDir, "Datapack");
            CreateMeta(datapack.OutputDir, "Datapack", content => content
                .Replace(Substitutions["description"], $"\"{datapack.Description}\"")
                .Replace(Substitutions["pack_format"], datapack.PackFormat.ToString(), StringComparison.InvariantCulture)
                .Replace(Substitutions["pack_id"], $"\"{configuration.ProjectId}\""));
        }
        if (configuration.Resourcepack is { } resourcepack)
        {
            SetResourcepackOutputDirectory(resourcepack, configuration.MinecraftRoot);
            CreateOutputFolder(resourcepack.OutputDir);
            CopyTemplate(resourcepack.OutputDir, "Resourcepack");
            CreateMeta(resourcepack.OutputDir, "Datapack", content => content
                .Replace(Substitutions["description"], $"\"{resourcepack.Description}\"")
                .Replace(Substitutions["pack_format"], resourcepack.PackFormat.ToString(), StringComparison.InvariantCulture)
                .Replace(Substitutions["pack_id"], $"\"{configuration.ProjectId}\""));
        }
        
        cts.Cancel();
        PrintMessageWithTime("Project structure created.", getElapsed());
    }
    
    private void CompileProject(Configuration configuration)
    {
        var cts = PrintLongTask("Compiling program", out var getElapsed);
        try
        {
            if (configuration.Datapack is { } datapack)
            {
                ProcessDatapackOrResourcepack(true, datapack.OutputDir);
            }

            if (configuration.Resourcepack is { } resourcepack)
            {
                ProcessDatapackOrResourcepack(false, resourcepack.OutputDir);
            }

            var compiler = new Compiler(Context);
            compiler.CompileProject();
        }
        catch (AmethystException e)
        {
            cts.Cancel();
            PrintError(e.Message);

            if (_rethrowErrors)
            {
                throw;
            }

            return;
        }
        
        if (configuration.Datapack is { } datapack2)
        {
            CreateFunctionTags(datapack2);
        }
        
        cts.Cancel();
        PrintMessageWithTime("Program compiled.", getElapsed());
    }

    private void ProcessDatapackOrResourcepack(bool isDatapack, string outputDir)
    {
        var dirName = isDatapack ? "data" : "assets";
        var sourceDir = Path.Combine(Context.SourcePath, dirName);
        var dataOrAssetsDir = Path.Combine(outputDir, dirName);
        
        // Copy everything except amethyst source code to output folder
        FilesystemUtility.CopyDirectory(sourceDir, dataOrAssetsDir, filePath =>
        {
            return Path.GetExtension(filePath) != SourceFileExtension;
        });

        // Scan namespaces
        foreach (var nsPath in Directory.GetDirectories(sourceDir))
        {
            RegisterAndIndexNamespace(nsPath);
        }
        
        ParseInternalNamespace(dirName);
    }
    
    private void RegisterAndIndexNamespace(string nsPath)
    {
        var nsName = Path.GetFileName(nsPath);
        
        if (!Context.Namespaces.TryGetValue(nsName, out var ns))
        {
            if (Context.CompilerFlags.HasFlag(CompilerFlags.Debug))
            {
                PrintDebug($"Registering namespace: {nsName}");
            }
            
            Context.Namespaces.Add(nsName, ns = new Namespace
            {
                Context = Context,
                Name = nsName
            });
        }
        
        var parser = new Parser();

        foreach (var registryPath in Directory.GetDirectories(nsPath))
        {
            var registryName = Path.GetFileName(registryPath);
            parser.RegistryName = registryName;
            
            // Scan amethyst source code files in each registry
            foreach (var filePath in Directory.GetFiles(registryPath, SourceFile, SearchOption.AllDirectories))
            {
                if (!ns.Registries.TryGetValue(registryName, out var registry))
                {
                    ns.Registries.Add(registryName, registry = new SourceFolder
                    {
                        Context = Context
                    });
                }
                
                var relativePath = Path.GetRelativePath(registryPath, filePath);
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var folder = registry.CreateOrGetFolderForPath(relativePath);
                var sourceFile = new SourceFile
                {
                    Path = filePath,
                    Name = $"{nsName}:{relativePath}",
                    RootScope = new Scope
                    {
                        Context = Context,
                        Name = nsName,
                        Parent = null
                    }
                };
                parser.Parse(sourceFile);
                folder.SourceFiles.Add(fileName, sourceFile);
            }
        }
    }
    
    private void ParseInternalNamespace(string dirName)
    {
        // TODO: Parse all contents of $(dirName)/amethyst/**/api/*.amy
    }
    
    public static void PrintAmethystLogoAndVersion(bool reduceColors, CompilerFlags compilerFlags)
    {
        if (reduceColors)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(" Amethyst ");
        }
        else
        {
            Console.Write(" \e[1m" +
                          $"\e[38;5;{98}mA" +
                          $"\e[38;5;{98}mm" +
                          $"\e[38;5;{98}me" +
                          $"\e[38;5;{140}mt" +
                          $"\e[38;5;{140}mh" +
                          $"\e[38;5;{183}my" +
                          $"\e[38;5;{183}ms" +
                          $"\e[38;5;{183}mt" +
                          "\e[22m ");
        }
        
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("v" + AmethystVersion);
        if (compilerFlags.HasFlag(CompilerFlags.Watch))
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(" (watch mode)");
        }
        if (compilerFlags.HasFlag(CompilerFlags.Debug))
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write(" (debug mode)");
        }
        Console.WriteLine();
        Console.ResetColor();
        Console.WriteLine();
    }
}