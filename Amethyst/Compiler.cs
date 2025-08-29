using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler : AmethystBaseVisitor<object?>
{
    internal Context Context { get; }
    internal int TotalRecordCount { get; set; } = 0;
    internal int StackPointer { get; set; } = 0;
    
    internal Namespace Namespace { get; set; } = null!;
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
            Namespace = ns;
            CompileNamespace();
        }
    }
    
    public void CompileNamespace()
    {
        if (Namespace.Registries.TryGetValue(Constants.DATAPACK_FUNCTIONS_DIRECTORY, out var functionRegistry))
        {
            foreach (var sourceFile in GetSourceFilesInFolder(functionRegistry))
            {
                SourceFile = sourceFile;
                Scope = sourceFile.RootScope;
                
                foreach (var entryPoint in sourceFile.EntryPointFunctions.Values)
                {
                    VisitFunction_declaration(entryPoint);
                }
            }
        }

        foreach (var (registryName, registry) in Namespace.Registries)
        {
            if (registryName == Constants.DATAPACK_FUNCTIONS_DIRECTORY) continue;
            foreach (var sourceFile in GetSourceFilesInFolder(registry))
            {
                SourceFile = sourceFile;
                Scope = sourceFile.RootScope;
                
                foreach (var exportedSymbol in sourceFile.ExportedSymbols.Values)
                {
                    VisitDeclaration(exportedSymbol);
                }
            }
        }
    }

    public static IEnumerable<SourceFile> GetSourceFilesInFolder(SourceFolder sourceFolder)
    {
        return sourceFolder.SourceFiles.Values
            .Concat(sourceFolder.Children.SelectMany(pair => GetSourceFilesInFolder(pair.Value)));
    }
    
    internal void AddCode(string code)
    {
        Scope.AddCode(code);
    }
    
    internal void AddInitCode(string code)
    {
        Namespace.AddInitCode(code);
    }
    
    internal Scope EvaluateScoped(string name, Action<Action> action)
    {
        if (!Scope.Scopes.TryAdd(name, 0))
        {
            Scope.Scopes[name]++;
        }
        
        var previousScope = Scope;
        var newScope = Scope = new Scope
        {
            Name = name + Scope.Scopes[name],
            Parent = previousScope,
            Context = Context
        };
        
        var isCancelled = false;
        
        action(() => isCancelled = true);

        if (!isCancelled)
        {
            Scope.Dispose();
        }
        
        Scope = previousScope;
        
        return newScope;
    }
}