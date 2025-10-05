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
    public Dictionary<string, Symbol> Symbols { get; } = new();

    private readonly TextWriter _writer = new StringWriter();
    
    public string FilePath => Path.Combine(
        Context.Configuration.Datapack!.OutputDir, 
        GetDataSubpath(DatapackFunctionsDirectory) + McfunctionFileExtension);
    
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
    /// <param name="registry">The registry to get the data subpath for. (function, predicate, etc...)</param>
    /// <returns>The data subpath for the given location.</returns>
    public string GetDataSubpath(string registry)
    {
        var path = "";
        var current = this;
        while (current.Parent is not null)
        {
            path = Path.Combine(current.Name, path);
            current = current.Parent;
        }
        return Path.Combine("data", current.Name, registry, path);
    }

    public void CreateFunctionFile()
    {
        var dirPath = Path.GetDirectoryName(FilePath)!;
        Directory.CreateDirectory(dirPath);
        
        using var writer = File.CreateText(FilePath);
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("Amethyst.Resources.template.mcfunction")!;
        using var reader = new StreamReader(stream);
        var template = reader.ReadToEnd();
        template = template.Replace(Substitutions["amethyst_version"], SubstitutionValues["amethyst_version"].ToString());
        template = template.Replace(Substitutions["date"], SubstitutionValues["date"].ToString());
        writer.Write(template);
    }
    
    public void AddCode(string code)
    {
        _writer.WriteLine(code);
    }

    public bool TryGetSymbol(string identifier, [NotNullWhen(true)] out Symbol? symbol)
    {
        if (Symbols.TryGetValue(identifier, out symbol))
        {
            return true;
        }
        
        if (Parent is not null)
        {
            return Parent.TryGetSymbol(identifier, out symbol);
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
        
        GC.SuppressFinalize(this);
    }
}