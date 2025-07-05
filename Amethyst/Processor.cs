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
        var projectId = Table["id"];

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
        using (var stream = assembly.GetManifestResourceStream("Amethyst.Resources.datapack.pack.mcmeta")!)
        {
            using (var reader = new StreamReader(stream))
            {
                var mcMetaContents = reader.ReadToEnd();
                var description = Table["datapack"]["description"].AsString ?? DEFAULT_DATAPACK_DESCRIPTION;
                var packFormat = Table["datapack"]["pack_format"].AsInteger ?? DEFAULT_DATAPACK_FORMAT;
                File.WriteAllText(mcMeta, mcMetaContents
                    .Replace(SUBSTITUTIONS["pack_id"], $"\"{Context.ProjectId}\"")
                    .Replace(SUBSTITUTIONS["description"], $"\"{description}\"")
                    .Replace(SUBSTITUTIONS["pack_format"], packFormat.ToString()));
            }
        }
        
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
        using (var stream = assembly.GetManifestResourceStream("Amethyst.Resources.resourcepack.pack.mcmeta")!)
        {
            using (var reader = new StreamReader(stream))
            {
                var mcMetaContents = reader.ReadToEnd();
                var description = Table["resourcepack"]["description"].AsString ?? DEFAULT_RESOURCEPACK_DESCRIPTION;
                mcMetaContents = mcMetaContents.Replace(SUBSTITUTIONS["description"], $"\"{description}\"");
                var packFormat = Table["resourcepack"]["pack_format"].AsInteger ?? DEFAULT_RESOURCEPACK_FORMAT;
                mcMetaContents = mcMetaContents.Replace(SUBSTITUTIONS["pack_format"], packFormat.ToString());
                File.WriteAllText(mcMeta, mcMetaContents);
            }
        }
        
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

    private void CreateExportedFunctions()
    {
        var cts = PrintLongTask("Creating exported functions", out var getElapsed);

        var apiPath = Path.Combine(Context.Datapack!.OutputDir, $"data/{Context.ProjectId}/function/api/");

        Directory.CreateDirectory(apiPath);
        
        foreach (var (mcfPath, name) in Context.Datapack!.ExportedFunctions)
        {
            var path = Path.Combine(apiPath, name + MCFUNCTION_FILE_EXTENSION);
            File.WriteAllText(path, $"function {mcfPath}");
        }
        
        cts.Cancel();
        
        if (Context.CompilerFlags.HasFlag(CompilerFlags.Debug))
        {
            PrintDebugMessageWithTime("Exported functions created.", getElapsed());
        }
    }
    
    private void CopyDatapackTemplate()
    {
        var cts = PrintLongTask("Copying datapack template", out var getElapsed);
        
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
        
        cts.Cancel();

        if (Context.CompilerFlags.HasFlag(CompilerFlags.Debug))
        {
            PrintDebugMessageWithTime("Datapack template copied.", getElapsed());
        }
    }
    
    private void CopyResourcepackTemplate()
    {
        var cts = PrintLongTask("Copying resourcepack template", out var getElapsed);
        
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
        
        cts.Cancel();

        if (Context.CompilerFlags.HasFlag(CompilerFlags.Debug))
        {
            PrintDebugMessageWithTime("Resourcepack template copied.", getElapsed());
        }
    }
    
    private IEnumerable<string> FindCompileTargets(string sDir) 
    {
        var cts = PrintLongTask($"Searching for {SOURCE_FILE} files", out var getElapsed);
        var count = 0;
        
        foreach (var f in Directory.GetFiles(sDir, SOURCE_FILE))
        {
            count++;
            yield return f;
        }

        foreach (var d in Directory.GetDirectories(sDir)) 
        {
            foreach (var f in FindCompileTargets(d))
            {
                count++;
                yield return f;
            }
        }
        
        cts.Cancel();

        if (Context.CompilerFlags.HasFlag(CompilerFlags.Debug))
        {
            PrintDebugMessageWithTime($"Found {count} {SOURCE_FILE} files.", getElapsed());
        }
    }
    
    private void CompileProject()
    {
        var cts = PrintLongTask("Compiling program", out var getElapsed);
        try
        {
            foreach (var directory in Directory.GetDirectories(Context.SourcePath))
            {
                var name = Path.GetFileName(directory);
                
                if (RESERVED_NAMESPACES.Contains(name))
                {
                    throw new Exception($"The directory '{name}' is reserved and cannot be used as a namespace.");
                }
                
                if (Directory.GetFiles(directory, SOURCE_FILE).Length == 0)
                {
                    var relativeFile = Path.GetRelativePath(Context.SourcePath, directory);
                    if (Context.CompilerFlags.HasFlag(CompilerFlags.Debug))
                    {
                        PrintDebug($"Skipping empty namespace '/{relativeFile}'.");
                    }

                    continue;
                }
                
                var ns = new Namespace
                {
                    Context = Context,
                    Scope = new Scope
                    {
                        Name = name,
                        Parent = null,
                        Context = Context
                    }
                };

                var loadFunction = new Function
                {
                    Scope = new Scope
                    {
                        Name = "_load",
                        Context = Context,
                        Parent = ns.Scope
                    },
                    Attributes = new List<string> { ATTRIBUTE_LOAD_FUNCTION }
                };
                
                ns.Functions.Add("_load", loadFunction);

                if (Context.Datapack != null)
                {
                    Context.Datapack.LoadFunctions.Add(loadFunction.McFunctionPath);
                }

                Context.Namespaces.Add(ns);
                
                foreach (var target in FindCompileTargets(directory))
                {
                    var fileStream = File.OpenRead(target);
                    var tree = Context.Parse(fileStream, target, ns, out var @namespace);
                    var file = new SourceFile { Context = tree, Path = target };
                    @namespace.Files.Add(file);
                }
            }

            foreach (var target in Directory.GetFiles(Context.SourcePath, SOURCE_FILE))
            {
                var fileStream = File.OpenRead(target);
                var tree = Context.Parse(fileStream, target, null, out var @namespace);
                var file = new SourceFile { Context = tree, Path = target };
                @namespace.Files.Add(file);
            }
            
            Context.Compile();
            
            CreateFunctionTags();
            CreateExportedFunctions();
            
            cts.Cancel();
            PrintMessageWithTime("Program compiled.", getElapsed());
        }
        catch (SyntaxException e)
        {
            cts.Cancel();
            var relativeFile = Path.GetRelativePath(Context.SourcePath, e.File);
            PrintError($"Syntax error: {relativeFile} ({e.Line}:{e.PosInLine}): {e.Message}");
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
        }
    }
}