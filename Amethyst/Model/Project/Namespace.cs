using static Amethyst.Model.Constants;

namespace Amethyst.Model;

public class Namespace : IDisposable
{
    public Namespace(string name, Context context)
    {
        Name = name;
        _context = context;
    }

    public string Name { get; }
    public HashSet<int> ScoreboardConstants { get; } = new();
    public Dictionary<string, Record> RecordDeclarations { get; } = new();

    private readonly Context _context;
    private TextWriter? _writer;
    
    public string FilePath => Path.Combine(
        _context.Configuration.Datapack!.OutputDir,
        DatapackRootDir,
        Name, 
        DatapackFunctionsDirectory, 
        InitFunctionName + McfunctionFileExtension);
    
    public string McFunctionPath => $"{Name}:_init";
    
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

    private void AddInitCode(string code)
    {
        if (_writer == null)
        {
            _writer = new StringWriter();
        }
        
        _writer.WriteLine(code);
    }
}