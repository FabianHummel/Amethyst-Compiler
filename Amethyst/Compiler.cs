using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler : AmethystBaseVisitor<object?>, IArithmeticBase
{
    internal Context Context { get; }
    internal Scope Scope { get; set; } = null!;
    internal int TotalRecordCount { get; set; } = 0;
    internal int MemoryLocation { get; set; } = 0;
    internal Namespace Namespace { get; set; } = null!;
    internal SourceFile SourceFile { get; set; } = null!;
    
    public Compiler(Context context)
    {
        Context = context;
    }

    public void Compile()
    {
        foreach (var ns in Context.Namespaces)
        {
            Namespace = ns;
            
            Scope = new Scope
            {
                Name = ns.Functions["_load"].Scope.Name,
                Parent = ns.Scope,
                Context = Context
            };
            
            Scope.CreateFunctionFile();
            
            foreach (var file in ns.Files)
            {
                SourceFile = file;
                VisitFile(file.Context);
            }
        }
    }
    
    internal void AddCode(string code)
    {
        Scope.AddCode(code);
    }
    
    internal void AddInitCode(string code)
    {
        Namespace.AddInitCode(code);
    }
    
    internal Scope EvaluateScoped(string name, Action action)
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
        
        action();
        
        Scope = previousScope;
        
        return newScope;
    }
}