using System.Diagnostics.CodeAnalysis;
using Amethyst.Language;
using Amethyst.Model;
using Antlr4.Runtime;
using Type = Amethyst.Model.Type;

namespace Amethyst;

public partial class Compiler : AmethystBaseVisitor<object?>
{
    private Context Context { get; }
    private Scope Scope { get; set; } = null!;
    private int TotalVariableCount { get; set; } = 0;
    private int TotalRecordCount { get; set; } = 0;
    private int StackPointer { get; set; } = 0;
    private Namespace Namespace { get; set; } = null!;
    private SourceFile SourceFile { get; set; } = null!;
    
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
    
    private void AddCode(string code)
    {
        Scope.AddCode(code);
    }
    
    private void AddInitCode(string code)
    {
        Namespace.AddInitCode(code);
    }
    
    private Scope EvaluateScoped(string name, Action action)
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