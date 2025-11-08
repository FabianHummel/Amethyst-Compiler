using static Amethyst.Model.Constants;

namespace Amethyst.Model;

/// <summary>Represents a namespace within the project.</summary>
public class Namespace : IDisposable
{
    /// <summary>Initializes a new instance of the <see cref="Namespace" /> class.</summary>
    /// <param name="name">The name of the namespace.</param>
    /// <param name="context">The compilation context.</param>
    public Namespace(string name, Context context)
    {
        Name = name;
        _context = context;
    }

    /// <summary>The name of the namespace.</summary>
    public string Name { get; }

    /// <summary>A set of scoreboard constants used within this namespace. These constants are initialized
    /// in the namespace's init function.</summary>
    public HashSet<int> ScoreboardConstants { get; } = new();

    /// <summary>A dictionary of record declarations within this namespace, mapping record names to their
    /// corresponding <see cref="Record" /> objects. Just like scoreboard constants (
    /// <see cref="ScoreboardConstants" />), records also require initialization in the namespace's init
    /// function.</summary>
    public Dictionary<string, Record> RecordDeclarations { get; } = new();

    /// <summary>The compilation context associated with this namespace.</summary>
    private readonly Context _context;

    /// <summary>A <see cref="TextWriter" /> used to accumulate the initialization code for the namespace's
    /// init function.</summary>
    private TextWriter? _writer;

    /// <summary>The absolute file path for the namespace's init function.</summary>
    public string FilePath => Path.Combine(
        _context.Configuration.Datapack!.OutputDir,
        DatapackRootDir,
        Name, 
        DatapackFunctionsDirectory, 
        InitFunctionName + McfunctionFileExtension);

    /// <summary>The Minecraft function path for the namespace's init function.</summary>
    public string McFunctionPath => $"{Name}:_init";

    /// <summary>Disposes of the namespace, finalizing and writing the init function to disk.</summary>
    public void Dispose()
    {
        foreach (var constant in ScoreboardConstants)
        {
            AddInitCode($"scoreboard players set #{constant} amethyst_const {constant}");
        }

        foreach (var recordDeclaration in RecordDeclarations)
        {
            AddInitCode($"scoreboard objectives add {recordDeclaration.Key} dummy");

            if (_context.CompilerFlags.HasFlag(CompilerFlags.Debug))
            {
                AddInitCode($$"""scoreboard objectives modify {{recordDeclaration.Key}} displayname ["",{"text":"Record ","bold":true},{"text":"{{recordDeclaration.Key}}","color":"dark_purple"},{"text":" @ "},{"text":"{{recordDeclaration.Value.McFunctionPath}}/","color":"gray"},{"text":"{{recordDeclaration.Value.Name}}","color":"light_purple"}]""");
            }
        }

        if (_writer == null)
        {
            GC.SuppressFinalize(this);
            return;
        }
        
        _context.Configuration.Datapack!.LoadFunctions.Add(McFunctionPath);

        if (!File.Exists(FilePath))
        {
            Processor.CreateFunctionFile(FilePath);
        }
        
        File.AppendAllText(FilePath, _writer.ToString());
        _writer.Dispose();
        
        GC.SuppressFinalize(this);
    }

    /// <summary>Adds initialization code to the namespace's init function.</summary>
    /// <param name="code">The initialization code to add.</param>
    private void AddInitCode(string code)
    {
        if (_writer == null)
        {
            _writer = new StringWriter();
        }
        
        _writer.WriteLine(code);
    }
}