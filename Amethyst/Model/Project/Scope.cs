using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using static Amethyst.Constants;

namespace Amethyst.Model;

public class Scope : IDisposable
{
    public string Name { get; }
    public required Scope? Parent { get; init; }
    
    public Dictionary<string, int> Scopes { get; } = new();
    public Dictionary<string, Symbol> Symbols { get; } = new();
    
    private readonly Compiler _compiler;
    private readonly Namespace _ns;
    private readonly SourceFile _sourceFile;
    
    private readonly TextWriter _writer = new StringWriter();

    public Scope(Compiler compiler, string name)
    {
        _compiler = compiler;
        _ns = compiler.Namespace;
        _sourceFile = compiler.SourceFile;
        Name = name;
    }

    public string FilePath => Path.Combine(
        _compiler.Context.Configuration.Datapack!.OutputDir, 
        "data",
        _ns.Name,
        DatapackFunctionsDirectory,
        _sourceFile.GetPath(),
        GetPath() + McfunctionFileExtension);
    
    public string GetPath()
    {
        return Path.Combine(Parent?.GetPath() ?? "", Name);
    }
    
    public string McFunctionPath
    {
        get
        {
            var path = Path.Combine(_sourceFile.GetPath(), GetPath());
            return $"{_ns.Name}:{path.Replace(Path.DirectorySeparatorChar, '/')}";
        }
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
        Dispose(false);
        
        GC.SuppressFinalize(this);
    }

    public void Dispose(bool cancelled)
    {
        if (cancelled)
        {
            return;
        }
        
        if (!File.Exists(FilePath))
        {
            CreateFunctionFile();
        }
        
        File.AppendAllText(FilePath, _writer.ToString());
        
        _writer.Dispose();
    }

    public override string ToString()
    {
        return $"{McFunctionPath} ({FilePath})";
    }

    private void CreateFunctionFile()
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
}

internal sealed class DisposableScope : IDisposable
{
    private readonly Compiler _owner;
    private readonly Scope _previous;
    
    public DisposableScope(Compiler owner, Scope scope)
    {
        _owner = owner;
        _previous = owner.Scope;
        owner.Scope = scope;
    }
    
    public void Dispose()
    {
        _owner.Scope = _previous;
    }
}