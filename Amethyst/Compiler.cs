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
    
    private void ThrowSyntaxError(string message, ParserRuleContext context)
    {
        throw new SyntaxException(message, context.Start.Line, context.Start.Column, SourceFile.Path);
    }
    
    private bool InferType(object? result, [MaybeNullWhen(false)] out Type type)
    {
        type = new Type { BasicType = BasicType.Int };
        return true;
    }
    
    private void AddCode(string code)
    {
        Scope.AddCode(code);
    }
    
    private void AddInitCode(string code)
    {
        Namespace.AddInitCode(code);
    }
}