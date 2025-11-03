using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using static Amethyst.Model.Constants;

namespace Amethyst.Model;

public class Scope : IDisposable
{
    public string Name { get; }
    public required Scope? Parent { get; init; }
    
    public Dictionary<string, int> Scopes { get; } = new();
    public Dictionary<string, Symbol> Symbols { get; } = new();

    private readonly Context _context;
    private readonly SourceFile _sourceFile;
    private bool _isCancelled;
    
    private readonly TextWriter _writer = new StringWriter();

    public Scope(Context context, SourceFile sourceFile)
        : this("", context, sourceFile)
    {
    }
    
    public Scope(string name, Context context, SourceFile sourceFile)
    {
        Name = name;
        _context = context;
        _sourceFile = sourceFile;
    }

    public string FilePath => Path.Combine(
        _context.Configuration.Datapack!.OutputDir, 
        _sourceFile.GetFullPath(),
        GetPath() + McfunctionFileExtension);
    
    public string GetPath()
    {
        return Path.Combine(Parent?.GetPath() ?? "", Name);
    }
    
    public string McFunctionPath
    {
        get
        {
            var mcFunctionPath = GetPath().Replace(Path.DirectorySeparatorChar, '/');
            return $"{_sourceFile.McFunctionPath}/{mcFunctionPath}";
        }
    }

    public void AddCode(string code)
    {
        _writer.WriteLine(code);
    }
    
    public void Cancel()
    {
        _isCancelled = true;
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
        if (_isCancelled)
        {
            return;
        }
        
        if (!File.Exists(FilePath))
        {
            Processor.CreateFunctionFile(FilePath);
        }
        File.AppendAllText(FilePath, _writer.ToString());
        _writer.Dispose();
        
        GC.SuppressFinalize(this);
    }

    public override string ToString()
    {
        return $"{McFunctionPath} ({FilePath})";
    }

    public static Scope Reparent(Scope parent, string name, bool preserveName = false)
    {
        if (preserveName)
        {
            goto createScope;
        }
        
        if (!parent.Scopes.TryAdd(name, 0))
        {
            parent.Scopes[name]++;
        }
        
        name += parent.Scopes[name];
        
    createScope:
        return new Scope(name, parent._context, parent._sourceFile)
        {
            Parent = parent
        };
    }
    
    internal sealed class GlobalBackup : IDisposable
    {
        private readonly Compiler _owner;
        private readonly Scope _previous;
    
        public GlobalBackup(Compiler owner, Scope scope)
        {
            _owner = owner;
            _previous = owner.Scope;
            owner.Scope = scope;
        }
    
        public void Dispose()
        {
            _owner.Scope.Dispose();
            _owner.Scope = _previous;
        }
    }
}

public static partial class CompilerExtensions
{
    public static void AddCode(this Compiler compiler, string code)
    {
        compiler.Scope.AddCode(code);
    }

    internal static Scope.GlobalBackup EvaluateScoped(this Compiler compiler, string name, bool preserveName = false)
    {
        var newScope = Scope.Reparent(compiler.Scope, name, preserveName);
        return new Scope.GlobalBackup(compiler, newScope);
    }
}