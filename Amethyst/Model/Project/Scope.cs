using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using static Amethyst.Constants;

namespace Amethyst.Model;

public class Scope : IDisposable
{
    public string Name { get; }
    public required Scope? Parent { get; init; }
    
    public Dictionary<string, int> Scopes { get; } = new();
    public Dictionary<string, Symbol> Symbols { get; } = new();

    private readonly SourceFile _sourceFile;
    private readonly Context _context;
    
    private readonly TextWriter _writer = new StringWriter();
    private readonly TextWriter _initWriter = new StringWriter();

    public Scope(Context context, SourceFile sourceFile)
        : this("", context, sourceFile)
    {
    }
    
    public Scope(string name, Context context, SourceFile sourceFile)
    {
        Name = name;
        _sourceFile = sourceFile;
        _context = context;
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
    
    public void AddInitCode(string code)
    {
        _initWriter.WriteLine(code);
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

    public static Scope Reparent(Scope compilerScope, string scopeName)
    {
        return new Scope(scopeName, compilerScope._context, compilerScope._sourceFile)
        {
            Parent = compilerScope
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
    
    public static void AddInitCode(this Compiler compiler, string code)
    {
        compiler.Scope.AddInitCode(code);
    }

    public static string EvaluateScoped(this Compiler compiler, string name, Action<Action> action)
    {
        return compiler.EvaluateScoped(name, (_, cancel) => action(cancel));
    }

    public static string EvaluateScoped(this Compiler compiler, string name, Action<Scope, Action> action)
    {
        if (!compiler.Scope.Scopes.TryAdd(name, 0))
        {
            compiler.Scope.Scopes[name]++;
        }
        
        var scopeName = name + compiler.Scope.Scopes[name];
        var newScope = Scope.Reparent(compiler.Scope, scopeName);

        using var scope = new Scope.GlobalBackup(compiler, newScope);
        var mcFunctionPath = EvaluateScopeInternal(compiler, newScope, action);
        return mcFunctionPath;
    }

    private static string EvaluateScopeInternal(Compiler compiler, Scope scope, Action<Scope, Action> action)
    {
        var isCancelled = false;
        action(scope, () =>
        {
            isCancelled = true;
        });
        
        var mcFunctionPath = scope.McFunctionPath;
        compiler.Scope.Dispose(isCancelled);
        return mcFunctionPath;
    }
}