using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using static Amethyst.Constants;

namespace Amethyst.Model;

public class Scope : IDisposable
{
    public required string Name { get; init; }
    public required Scope? Parent { get; init; }
    public required Context Context { get; init; }
    public Dictionary<string, int> Scopes { get; } = new();
    public Dictionary<string, Variable> Variables { get; } = new();
    public Dictionary<string, Record> Records { get; } = new();

    private TextWriter _writer = new StringWriter();
    
    public string FilePath => Path.Combine(Context.Datapack!.OutputDir, GetDataSubpath(DATAPACK_FUNCTIONS_DIRECTORY) + MCFUNCTION_FILE_EXTENSION);
    
    public string McFunctionPath
    {
        get
        {
            var sb = new StringBuilder("");
            var current = this;
            while (current.Parent is not null)
            {
                sb.Insert(0, $"/{current.Name}");
                current = current.Parent;
            }
            return $"{current.Name}:{sb.ToString()[1..]}";
        }
    }

    public string ScoreboardName
    {
        get
        {
            var sb = new StringBuilder("");
            var current = this;
            while (current.Parent is not null)
            {
                sb.Insert(0, $".{current.Name}");
                current = current.Parent;
            }
            return $"{current.Name}_{sb.ToString()[1..]}";
        }
    }

    /// <summary>
    /// Gets the data subpath for the given location. Path structure is 'data/{root_namespace}/{location}/...'
    /// </summary>
    /// <param name="location">The location to get the data subpath for. (functions, predicates, etc...)</param>
    /// <returns>The data subpath for the given location.</returns>
    public string GetDataSubpath(string location)
    {
        var path = "";
        var current = this;
        while (current.Parent is not null)
        {
            path = Path.Combine(current.Name, path);
            current = current.Parent;
        }
        return Path.Combine("data", current.Name, location, path);
    }

    public void CreateFunctionFile()
    {
        var directory = Path.GetDirectoryName(FilePath)!;
        Directory.CreateDirectory(directory);
        
        using var writer = File.CreateText(FilePath);
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("Amethyst.Resources.template.mcfunction")!;
        using var reader = new StreamReader(stream);
        var template = reader.ReadToEnd();
        template = template.Replace(SUBSTITUTIONS["amethyst_version"], SUBSTITUTION_VALUES["amethyst_version"].ToString());
        template = template.Replace(SUBSTITUTIONS["date"], SUBSTITUTION_VALUES["date"].ToString());
        writer.Write(template);
    }
    
    public void AddCode(string code)
    {
        _writer.WriteLine(code);
    }

    public bool TryGetVariable(string identifier, [NotNullWhen(true)] out Variable? variable)
    {
        if (Variables.TryGetValue(identifier, out variable))
        {
            return true;
        }
        
        if (Parent is not null)
        {
            return Parent.TryGetVariable(identifier, out variable);
        }
        
        return false;
    }

    public bool TryGetRecord(string identifier, [NotNullWhen(true)] out Record? record)
    {
        if (Records.TryGetValue(identifier, out record))
        {
            return true;
        }
        
        if (Parent is not null)
        {
            return Parent.TryGetRecord(identifier, out record);
        }
        
        return false;
    }

    public void Dispose()
    {
        if (!File.Exists(FilePath))
        {
            CreateFunctionFile();
        }
        
        File.AppendAllText(FilePath, _writer.ToString());
        
        _writer.Dispose();
    }
}