using System.Globalization;
using System.Text.RegularExpressions;
using System.Reflection;
using Amethyst.Model;
using Amethyst.Utility;
using Tomlet;

namespace Amethyst;

using static Constants;
using static ConsoleUtility;

/// <summary>The processor class acts as the build system of the compiler. It parses the configuration
/// file, collects source files, and invokes the compiler with the correct settings. The processor is
/// also in charge to create the datapack and resourcepack template and much more.</summary>
public class Processor
{
    /// <summary>Whether to rethrow compilation errors. This is useful to forcefully exit the program upon
    /// a compilation error or to manually handle the exception. This option is used in the unit test
    /// runner to stop execution when the project fails to compile.</summary>
    private readonly bool _rethrowErrors;

    /// <summary>A reference to the <see cref="Parser" /> that is used to parse amethyst source files.</summary>
    private readonly Parser _parser = new();

    /// <summary>A reference to the context that holds relevant information during compilation.</summary>
    public Context Context { get; }

    /// <summary>Creates a new processor that reads the configuration and recompiles the project.</summary>
    /// <param name="rootDir">The path where to look for the project's configuration and source directory.</param>
    /// <param name="compilerFlags">Compiler flags that are parsed from the command line arguments</param>
    /// <param name="rethrowErrors">Sets <see cref="_rethrowErrors" /></param>
    public Processor(string rootDir, CompilerFlags compilerFlags, bool rethrowErrors = false)
    {
        _rethrowErrors = rethrowErrors;
        var configuration = ParseConfiguration(rootDir);
        Context = new Context(rootDir, compilerFlags, configuration);
        PrintConfigInfo(configuration);
        RecompileProject();
    }

    /// <summary>Creates a processor that only recompiles the project, but does not reload the project's
    /// configuration.</summary>
    /// <param name="context">The previous compilation context where some properties are cleared and some
    /// are copied.</param>
    /// <param name="rethrowErrors">Sets <see cref="_rethrowErrors" /></param>
    public Processor(Context context, bool rethrowErrors = false)
    {
        _rethrowErrors = rethrowErrors;
        Context = new Context(context);
        RecompileProject();
    }

    /// <summary>Creates an empty function file at the specified <paramref name="filePath" />. The template
    /// function is substituted with properties like the amethyst version and the current date.</summary>
    /// <param name="filePath">The path where to create the function file.</param>
    public static void CreateFunctionFile(string filePath)
    {
        var dirPath = Path.GetDirectoryName(filePath)!;
        Directory.CreateDirectory(dirPath);
        
        using var writer = File.CreateText(filePath);
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("Amethyst.Resources.template.mcfunction")!;
        using var reader = new StreamReader(stream);
        var template = reader.ReadToEnd();
        template = template.Replace(Substitutions["amethyst_version"], SubstitutionValues["amethyst_version"].ToString());
        template = template.Replace(Substitutions["date"], SubstitutionValues["date"].ToString());
        writer.Write(template);
    }

    /// <summary>Prints the amethyst logo, version and extra information about the compilation process.</summary>
    /// <param name="reduceColors">If true, falls back to print plain text. This is due to issues with some
    /// terminal emulators and console outputs.</param>
    /// <param name="compilerFlags">The parsed compiler flags from the command line</param>
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

    /// <summary>Recompiles the project by generating the compilation context for both a datapack and
    /// resourcepack and subsequently compiles the project. If an exception is thrown, this function prints
    /// the error and rethrows the exception, if <see cref="_rethrowErrors" /> is set to true.</summary>
    /// <exception cref="Exception">The compilation process had any error while
    /// <see cref="_rethrowErrors" /> is set to true.</exception>
    private void RecompileProject()
    {
        CreateDatapackAndResourcepackContext(Context.Configuration);
        
        try
        {
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

    /// <summary>Parses the project's configuration file into a <see cref="Configuration" /> structure and
    /// validates the data.</summary>
    /// <param name="rootDir">The path where to look for the configuration file</param>
    /// <returns>The parsed and validated configuration</returns>
    /// <exception cref="InvalidOperationException">The project's identifier does not match the naming
    /// requirements.</exception>
    private static Configuration ParseConfiguration(string rootDir)
    {
        try
        {
            var configPath = Path.Combine(rootDir, ConfigFile);
            using var reader = File.OpenText(configPath);
            var tomlConfiguration = reader.ReadToEnd();
            var configuration = TomletMain.To<Configuration>(tomlConfiguration);
            
            if (!Regex.IsMatch(configuration.ProjectId, "^[a-z0-9_]+$"))
            {
                throw new InvalidOperationException("Project ID must contain only lowercase letters, digits, or underscores.");
            }
            
            return configuration;
        }
        catch (Exception)
        {
            PrintError($"Missing or invalid configuration file '{ConfigFile}' found in project directory '{rootDir}'");
            throw;
        }
    }

    /// <summary>Prints configuration-related information to the console and warns the user if specific
    /// configurations look suspicious.</summary>
    /// <param name="config">The configuration that is used to check and print its information.</param>
    private void PrintConfigInfo(Configuration config)
    {
        var minecraftRootFolder = GetMinecraftRootDirectory(config.MinecraftRoot);
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

    /// <summary>Tries to find Minecraft's root directory (<c>.minecraft</c>) based on the user's operating
    /// system:
    /// <list type="bullet"><item><b>Windows:</b> <see cref="Constants.MinecraftRootWindows" /></item>
    ///     <item><b>macOS:</b> <see cref="Constants.MinecraftRootMacos" /></item>
    ///     <item><b>Linux:</b> <see cref="Constants.MinecraftRootLinux" /></item></list>
    /// </summary>
    /// <param name="minecraftRoot">If not null, immediately returns this path. Purely acts as syntactic
    /// sugar to improve the code visually.</param>
    /// <returns>An absolute path to Minecraft's root directory</returns>
    private static string GetMinecraftRootDirectory(string? minecraftRoot)
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

    /// <summary>Sets the datapack's final output directory based on different configuration and
    /// environmental factors. This is needed to distinguish between a server and a client setup, because
    /// the file structure is different.</summary>
    /// <param name="datapack">The parsed datapack configuration</param>
    /// <param name="minecraftRoot">A path to Minecraft's root directory, if it's specified in the
    /// configuration</param>
    /// <exception cref="InvalidOperationException">The specified world is not found under the saves
    /// directory of Minecraft.</exception>
    private void SetDatapackOutputDirectory(Datapack datapack, string? minecraftRoot)
    {
        string actualOutDir;
        if (datapack.Output == null && minecraftRoot != null)
        {
            var minecraftRootFolder = GetMinecraftRootDirectory(minecraftRoot);
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
            var minecraftRootFolder = GetMinecraftRootDirectory(minecraftRoot);
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

    /// <summary>Works similar to <see cref="SetDatapackOutputDirectory" />, but does not look for a
    /// specified world where to put the data, as resourcepacks can only be in one single location.</summary>
    /// <param name="resourcepack">The parsed resourcepack configuration</param>
    /// <param name="minecraftRoot">A path to Minecraft's root directory, if it's specified in the
    /// configuration</param>
    private void SetResourcepackOutputDirectory(Resourcepack resourcepack, string? minecraftRoot)
    {
        string actualOutDir;
        if (resourcepack.Output == null && minecraftRoot != null)
        {
            var minecraftRootFolder = GetMinecraftRootDirectory(minecraftRoot);
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
            var minecraftRootFolder = GetMinecraftRootDirectory(minecraftRoot);
            actualOutDir = Path.Combine(minecraftRootFolder, "resourcepacks", resourcepack.Output);
        }
        
        end:
        resourcepack.OutputDir = Path.Combine(actualOutDir, resourcepack.Name);

        if (Context.CompilerFlags.HasFlag(CompilerFlags.Debug))
        {
            PrintDebug($"Resourcepack output directory: '{resourcepack.OutputDir}'.");
        }
    }

    /// <summary>Clears the output folder and creates an empty directory at the specified
    /// <paramref name="outputDir" />.</summary>
    /// <param name="outputDir">The path where to create the output folder</param>
    private static void CreateOutputFolder(string outputDir)
    {
        if (Directory.Exists(outputDir))
        {
            Directory.Delete(outputDir, true);
        }
        
        Directory.CreateDirectory(outputDir);
    }

    /// <summary>Generates metadata for a datapack or resourcepack in the form of a <c>.mcmeta</c> file.</summary>
    /// <param name="outputDir">The path where to create the metadata file</param>
    /// <param name="datapackOrResourcepack">Whether the metadata is for a datapack or resourcepack.</param>
    /// <param name="substitutions">Provides data that is used to fill the metadata such as the project
    /// version and name.</param>
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

    /// <summary>Creates function tags in the <c>minecraft</c> namespace that specify which functions to
    /// run every tick or only once on startup. This is done after compilation, because we don't know which
    /// functions will be generated beforehand.</summary>
    /// <param name="datapack">The parsed datapack configuration</param>
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

    /// <summary>Copies the datapack or resourcepack template to the specified
    /// <paramref name="outputDir" />. This includes amethyst's internal functions and APIs that the
    /// resulting code may use.</summary>
    /// <param name="outputDir">The path where to copy the template to</param>
    /// <param name="datapackOrResourcepack">Whether to copy the datapack or resourcepack template.</param>
    private void CopyTemplate(string outputDir, string datapackOrResourcepack)
    {
        var cts = PrintLongTask($"Copying {datapackOrResourcepack.ToLower(CultureInfo.InvariantCulture)} template", out var getElapsed);

        var assemblyPath = $"Amethyst.Resources.{datapackOrResourcepack.ToLower(CultureInfo.InvariantCulture)}";
        AssemblyUtility.CopyAssemblyFolder(assemblyPath, outputDir, exclude: SourceFileExtension);
        
        cts.Cancel();

        if (Context.CompilerFlags.HasFlag(CompilerFlags.Debug))
        {
            PrintDebugMessageWithTime($"{datapackOrResourcepack} template copied.", getElapsed());
        }
    }

    /// <summary>Creates the context and environment for the datapack and resourcepack, based on the
    /// provided <paramref name="configuration" />. After this function call, all necessary files and
    /// directories are created for compilation.</summary>
    /// <param name="configuration">The provided configuration</param>
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

    /// <summary>Compiles the project. First creates the context and environment needed for compilation and
    /// then invokes the compiler. Any errors during compilation are printed to the console and are
    /// rethrown if <see cref="_rethrowErrors" /> is set to true.</summary>
    /// <param name="configuration">The project's configuration</param>
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

    /// <summary>The first step of compilation is to process the datapack or resourcepack with the given
    /// <paramref name="outputDir" />. All files that are not related to direct compilation such as
    /// configuration files or Minecraft functions are immediately copied to the output directory. Then,
    /// all namespaces including amethyst's internal namespace are processed.</summary>
    /// <param name="isDatapack">Whether a datapack or resourcepack is being processed.</param>
    /// <param name="outputDir">The path where to copy files to.</param>
    private void ProcessDatapackOrResourcepack(bool isDatapack, string outputDir)
    {
        var packDirName = isDatapack ? "data" : "assets";
        var sourceDir = Path.Combine(Context.SourcePath, packDirName);
        var dataOrAssetsDir = Path.Combine(outputDir, packDirName);
        
        // Copy everything except amethyst source code to output folder
        FilesystemUtility.CopyDirectory(sourceDir, dataOrAssetsDir, filePath =>
        {
            return Path.GetExtension(filePath) != SourceFileExtension;
        });

        // Scan namespaces
        ParseUserNamespaces(packDirName, sourceDir);
        
        ParseInternalNamespace(packDirName, isDatapack);
    }

    /// <summary>Processes all user defined namespaces and parses the containing source files using
    /// <see cref="_parser" />.</summary>
    /// <param name="packDirName">The pack directory name, which can be either <c>data</c> for a datapack
    /// or <c>assets</c> for a resourcepack.</param>
    /// <param name="sourceDir">The path to the source directory of the project.</param>
    private void ParseUserNamespaces(string packDirName, string sourceDir)
    {
        // Scan amethyst source code files in each registry
        foreach (var filePath in Directory.GetFiles(sourceDir, SourceFile, SearchOption.AllDirectories))
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var relativePath = Path.GetRelativePath(sourceDir, filePath);
            var pathSegments = relativePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var sourceFile = new SourceFile([packDirName, ..pathSegments[..^1], fileName]);
            Context.SourceFiles.Add(sourceFile.GetFullPath(), sourceFile);
            sourceFile.Scope = new Scope(Context, sourceFile)
            {
                Parent = null
            };
            
            _parser.Parse(sourceFile, filePath);
        }
    }

    /// <summary>Processes amethyst's internal namespace. In fact, only the API is parsed, as all other
    /// components are already written in MCFunction directly for improved performance and flexibility.</summary>
    /// <param name="packDirName">The pack directory name, which can be either <c>data</c> for a datapack
    /// or <c>assets</c> for a resourcepack.</param>
    /// <param name="isDatapack">Whether to parse the internal datapack or resourcepack.</param>
    /// <exception cref="InvalidOperationException"></exception>
    private void ParseInternalNamespace(string packDirName, bool isDatapack)
    {
        var packGroupName = isDatapack ? "datapack" : "resourcepack";
        var resourcePath = $"Amethyst.Resources.{packGroupName}.{packDirName}";
        
        var sourceFilePaths = AssemblyUtility.GetEmbeddedResources(resourcePath)
            .Where(path => path.EndsWith(SourceFileExtension));
        
        foreach (var absoluteResourcePath in sourceFilePaths)
        {
            var relativeResourcePath = absoluteResourcePath[(resourcePath.Length + 1)..];
            var pathComponents = relativeResourcePath.Split('.').ToList();
            
            var apiFolder = pathComponents[2];
            if (apiFolder != "api")
            {
                throw new InvalidOperationException($"Invalid internal namespace structure, expected 'api' folder but got '{apiFolder}'");
            }
            pathComponents.RemoveAt(index: 2);

            var sourceFile = new SourceFile([packDirName, ..pathComponents[..^1]], isInternal: true);
            Context.SourceFiles.Add(sourceFile.GetFullPath(), sourceFile);
            sourceFile.Scope = new Scope(Context, sourceFile)
            {
                Parent = null
            };
            
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(absoluteResourcePath)!;
            _parser.Parse(sourceFile, stream);
        }
    }
}