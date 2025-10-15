using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler : AmethystParserBaseVisitor<object?>
{
    internal Context Context { get; }
    internal int TotalRecordCount { get; set; }
    internal int StackPointer { get; set; }
    
    internal Namespace Namespace { get; set; } = null!;
    internal Registry Registry { get; set; } = null!;
    internal SourceFile SourceFile { get; set; } = null!;
    internal Scope Scope { get; set; } = null!;
    
    public Compiler(Context context)
    {
        Context = context;
    }
    
    public void CompileProject()
    {
        foreach (var ns in Context.Namespaces.Values)
        {
            using var scope = new DisposableNamespace(this, ns);
            CompileNamespace(ns);
        }
    }
    
    public void AddCode(string code)
    {
        Scope.AddCode(code);
    }
    
    public void AddInitCode(string code)
    {
        Namespace.AddInitCode(code);
    }

    public string EvaluateScoped(string name, Action<Action> action)
    {
        return EvaluateScoped(name, (_, cancel) => action(cancel));
    }

    public string EvaluateScoped(string name, Action<Scope, Action> action)
    {
        if (!Scope.Scopes.TryAdd(name, 0))
        {
            Scope.Scopes[name]++;
        }
        
        var scopeName = name + Scope.Scopes[name];
        var newScope = new Scope(this, scopeName)
        {
            Parent = Scope
        };

        using var scope = new DisposableScope(this, newScope);
        var mcFunctionPath = EvaluateScopeInternal(newScope, action);
        return mcFunctionPath;
    }

    private string EvaluateScopeInternal(Scope scope, Action<Scope, Action> action)
    {
        var isCancelled = false;
        action(scope, () =>
        {
            isCancelled = true;
        });
        
        var mcFunctionPath = scope.McFunctionPath;
        Scope.Dispose(isCancelled);
        return mcFunctionPath;
    }
    
    private void CompileNamespace(Namespace ns)
    {
        foreach (var registry in ns.Registries.Values)
        {
            using var scope = new DisposableRegistry(this, registry);
            CompileRegistry(registry);
        }
    }
    
    private void CompileRegistry(Registry registry)
    {
        var sourceFiles = registry.Root.GetAllLeaves()
            .Select(leaf => leaf.Value);
        
        foreach (var sourceFile in sourceFiles)
        {
            using var scope = new DisposableSourceFile(this, sourceFile, new Scope(this, sourceFile.Name)
            {
                Parent = null
            });
            CompileSourceFile(sourceFile);
        }
    }
    
    private void CompileSourceFile(SourceFile sourceFile)
    {
        sourceFile.Scope = new Scope(this, sourceFile.Name)
        {
            Parent = Scope
        };
        
        foreach (var entryPoint in sourceFile.EntryPointFunctions.Values)
        {
            VisitFunctionDeclaration(entryPoint);
        }
            
        foreach (var (symbolName, symbol) in sourceFile.ExportedSymbols)
        {
            if (Scope.Symbols.ContainsKey(symbolName))
            {
                continue;
            }
                    
            VisitDeclaration(symbol);
        }
    }
}