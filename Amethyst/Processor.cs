using System.Text.RegularExpressions;
using System.Reflection;
using Amethyst.Model;
using Amethyst.Utility;
using Tommy;

namespace Amethyst;

using static Constants;
using static ConsoleUtility;

public class Processor
{
    private TomlTable Table { get; set; } = null!;
    public Context Context { get; } = new();
    
    public void ReinitializeProject()
    {
        ReadAndSetConfigFile();
        SetMinecraftRootFolder();
        SetSourcePath();
        RecompileProject();
    }
    
    public void RecompileProject()
    {
        Context.Clear();
        try
        {
            SetProjectId();
            CreateDatapackAndResourcepackContext();
            CompileProject();
        }
        catch (Exception e)
        {
            PrintError(e.Message);
            
            if (Context.CompilerFlags.HasFlag(CompilerFlags.Debug))
            {
                PrintError(e.StackTrace ?? string.Empty);
            }
        }
    }

    private void ReadAndSetConfigFile()
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
            PrintError($"Missing or invalid configuration file '{CONFIG_FILE}' found in current working directory '{Environment.CurrentDirectory}'");
            throw;
        }
    }

    private void SetMinecraftRootFolder()
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
            PrintWarning($"Minecraft root not found at default locations, using current directory as source if a world name has been specified for the output. ({defaultPath})");
            Context.MinecraftRoot = null;
        }
    }

    private void SetSourcePath()
    {
        Context.SourcePath = Path.Combine(Environment.CurrentDirectory, SOURCE_DIRECTORY);
    }

    private void SetProjectId()
    {
        var projectId = Table["id"].AsString;

        if (projectId == null)
        {
            throw new Exception($"Project ID not specified in '{CONFIG_FILE}'. Please set the 'id' field to a unique identifier for your project.");
        }

        if (!Regex.IsMatch(projectId, @"^[a-z0-9_]+$"))
        {
            throw new Exception("Project ID must contain only lowercase letters, digits, or underscores.");
        }

        Context.ProjectId = projectId;
    }
    
    private void SetDatapackName()
    {
        Context.Datapack!.Name = Table["datapack"]["name"].AsString ?? DEFAULT_DATAPACK_NAME;

        if (Context.CompilerFlags.HasFlag(CompilerFlags.Debug))
        {
            PrintDebug($"Datapack name = '{Context.Datapack.Name}'.");
        }
    }
    
    private void SetResourcepackName()
    {
        Context.Resourcepack!.Name = Table["resourcepack"]["name"].AsString ?? DEFAULT_RESOURCEPACK_NAME;

        if (Context.CompilerFlags.HasFlag(CompilerFlags.Debug))
        {
            PrintDebug($"Resourcepack name = '{Context.Resourcepack.Name}'.");
        }
    }

    private void SetDatapackOutputDirectory()
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

        if (Context.CompilerFlags.HasFlag(CompilerFlags.Debug))
        {
            PrintDebug($"Datapack output directory: '{Context.Datapack.OutputDir}'.");
        }
    }
    
    private void SetResourcepackOutputDirectory()
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

        if (Context.CompilerFlags.HasFlag(CompilerFlags.Debug))
        {
            PrintDebug($"Resourcepack output directory: '{Context.Resourcepack.OutputDir}'.");
        }
    }
    
    private void CreateDatapackOutputFolder()
    {
        if (Directory.Exists(Context.Datapack!.OutputDir))
        {
            Directory.Delete(Context.Datapack!.OutputDir, true);
        }
        
        Directory.CreateDirectory(Context.Datapack!.OutputDir);
    }
    
    private void CreateResourcepackOutputFolder()
    {
        if (Directory.Exists(Context.Resourcepack!.OutputDir))
        {
            Directory.Delete(Context.Resourcepack!.OutputDir, true);
        }
        
        Directory.CreateDirectory(Context.Resourcepack!.OutputDir);
    }

    private void CreateDatapackMeta()
    {
        var cts = PrintLongTask("Creating datapack meta file", out var getElapsed);
        var mcMeta = Path.Combine(Context.Datapack!.OutputDir, "pack.mcmeta");
    
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("Amethyst.Resources.datapack.pack.mcmeta")!;
        using var reader = new StreamReader(stream);
        
        var mcMetaContents = reader.ReadToEnd();
        var description = Table["datapack"]["description"].AsString ?? DEFAULT_DATAPACK_DESCRIPTION;
        var packFormat = Table["datapack"]["pack_format"].AsInteger ?? DEFAULT_DATAPACK_FORMAT;
        
        File.WriteAllText(mcMeta, mcMetaContents
            .Replace(SUBSTITUTIONS["pack_id"], $"\"{Context.ProjectId}\"")
            .Replace(SUBSTITUTIONS["description"], $"\"{description}\"")
            .Replace(SUBSTITUTIONS["pack_format"], packFormat.ToString()));

        cts.Cancel();

        if (Context.CompilerFlags.HasFlag(CompilerFlags.Debug))
        {
            PrintDebugMessageWithTime($"Datapack meta file created at '{mcMeta}'.", getElapsed());
        }
    }
    
    private void CreateResourcepackMeta()
    {
        var cts = PrintLongTask("Creating resourcepack meta file", out var getElapsed);
        var mcMeta = Path.Combine(Context.Resourcepack!.OutputDir, "pack.mcmeta");
    
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("Amethyst.Resources.resourcepack.pack.mcmeta")!;
        using var reader = new StreamReader(stream);
        
        var mcMetaContents = reader.ReadToEnd();
        var description = Table["resourcepack"]["description"].AsString ?? DEFAULT_RESOURCEPACK_DESCRIPTION;
        mcMetaContents = mcMetaContents.Replace(SUBSTITUTIONS["description"], $"\"{description}\"");
        var packFormat = Table["resourcepack"]["pack_format"].AsInteger ?? DEFAULT_RESOURCEPACK_FORMAT;
        mcMetaContents = mcMetaContents.Replace(SUBSTITUTIONS["pack_format"], packFormat.ToString());
        
        File.WriteAllText(mcMeta, mcMetaContents);

        cts.Cancel();
        
        if (Context.CompilerFlags.HasFlag(CompilerFlags.Debug))
        {
            PrintDebugMessageWithTime($"Resourcepack meta file created at '{mcMeta}'.", getElapsed());
        }
    }
    
    private void CreateFunctionTags()
    {
        var cts = PrintLongTask("Creating function tags", out var getElapsed);
        {
            var path = Path.Combine(Context.Datapack!.OutputDir, $"data/minecraft/tags/{DATAPACK_FUNCTIONS_DIRECTORY}/load.json");
            var content = File.ReadAllText(path);
            content = content.Replace(SUBSTITUTIONS["loading_functions"], string.Join("", Context.Datapack!.LoadFunctions.Select(f => $",\n    \"{f}\"")));
            File.WriteAllText(path, content);
        }
        {
            var path = Path.Combine(Context.Datapack!.OutputDir, $"data/minecraft/tags/{DATAPACK_FUNCTIONS_DIRECTORY}/tick.json");
            var content = File.ReadAllText(path);
            content = content.Replace(SUBSTITUTIONS["ticking_functions"], string.Join("", Context.Datapack!.TickFunctions.Select(f => $",\n    \"{f}\"")));
            File.WriteAllText(path, content);
        }
        
        cts.Cancel();

        if (Context.CompilerFlags.HasFlag(CompilerFlags.Debug))
        {
            PrintDebugMessageWithTime("Function tags created.", getElapsed());
        }
    }
    
    private void CopyDatapackTemplate()
    {
        var cts = PrintLongTask("Copying datapack template", out var getElapsed);
        
        AssemblyUtility.CopyAssemblyFolder("Amethyst.Resources.datapack", Context.Datapack!.OutputDir);
        
        cts.Cancel();

        if (Context.CompilerFlags.HasFlag(CompilerFlags.Debug))
        {
            PrintDebugMessageWithTime("Datapack template copied.", getElapsed());
        }
    }
    
    private void CopyResourcepackTemplate()
    {
        var cts = PrintLongTask("Copying resourcepack template", out var getElapsed);
        
        AssemblyUtility.CopyAssemblyFolder("Amethyst.Resources.resourcepack", Context.Resourcepack!.OutputDir);
        
        cts.Cancel();

        if (Context.CompilerFlags.HasFlag(CompilerFlags.Debug))
        {
            PrintDebugMessageWithTime("Resourcepack template copied.", getElapsed());
        }
    }
    
    private void CreateDatapackAndResourcepackContext()
    {
        var cts = PrintLongTask("Creating project structure", out var getElapsed);
        
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
        
        cts.Cancel();
        PrintMessageWithTime("Project structure created.", getElapsed());
    }
    
    private void CompileProject()
    {
        var cts = PrintLongTask("Compiling program", out var getElapsed);
        try
        {
            if (Context.Datapack != null)
            {
                ProcessDatapackOrResourcepack(true);
            }
            if (Context.Resourcepack != null)
            {
                ProcessDatapackOrResourcepack(false);
            }
        
            var compiler = new Compiler(Context);
            compiler.CompileProject();
        }
        catch (SyntaxException e)
        {
            cts.Cancel();
            PrintError($"Syntax error: {e.File} ({e.Line}:{e.Column}): {e.Message}");
            return;
        }
        
        CreateFunctionTags();
        
        cts.Cancel();
        PrintMessageWithTime("Program compiled.", getElapsed());
    }

    private void ProcessDatapackOrResourcepack(bool isDatapack)
    {
        var dirName = isDatapack ? "data" : "assets";
        var sourceDir = Path.Combine(Context.SourcePath, dirName);
        var outputDir = Path.Combine(isDatapack ? Context.Datapack!.OutputDir : Context.Resourcepack!.OutputDir, dirName);
        
        // Copy everything except amethyst source code to output folder
        FilesystemUtility.CopyDirectory(sourceDir, outputDir, filePath => {
            return Path.GetExtension(filePath) != SOURCE_FILE_EXTENSION;
        });

        // Scan namespaces
        foreach (var nsPath in Directory.GetDirectories(sourceDir))
        {
            RegisterAndIndexNamespace(nsPath);
        }
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
            foreach (var filePath in Directory.GetFiles(registryPath, SOURCE_FILE, SearchOption.AllDirectories))
            {
                if (!ns.Registries.TryGetValue(registryName, out var registry))
                {
                    ns.Registries.Add(registryName, registry = new SourceFolder
                    {
                        Context = Context,
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
}