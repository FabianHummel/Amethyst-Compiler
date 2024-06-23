using Amethyst.Language;
using Amethyst.Model;
using Antlr4.Runtime;

namespace Amethyst;

public class AmethystParseListener : AmethystBaseListener
{
    private Namespace Context { get; }
    
    public AmethystParseListener(Namespace context)
    {
        Context = context;
    }
    
    private List<string> GetNamespacePath(List<string> path, RuleContext context)
    {
        if (context.Parent is AmethystParser.Namespace_declarationContext ns)
        {
            path.InsertRange(0, ns.namespace_identifier().GetText().Split("::"));
            return GetNamespacePath(path, ns);
        }

        return new List<string> { Context.Name };
    }
    
    private Namespace CreateOrGetNamespace(IReadOnlyList<string> path, Namespace context)
    {
        if (path.Count == 0 || path[0] == context.Name)
        {
            return context;
        }

        var name = path[0];
        if (context.Namespaces.TryGetValue(name, out var ns))
        {
            return CreateOrGetNamespace(path.Skip(1).ToList(), ns);
        }

        var newNs = new Namespace
        {
            Parent = context,
            Context = context.Context,
            Name = name,
        };
        
        context.Namespaces.Add(name, newNs);
        
        return CreateOrGetNamespace(path.Skip(1).ToList(), newNs);
    }
    
    public override void ExitFunction_declaration(AmethystParser.Function_declarationContext context)
    {
        var path = GetNamespacePath(new List<string>(), context);
        var ns = CreateOrGetNamespace(path, Context);
        var name = context.identifier().GetText();
        ns.Functions.Add(name, context);
    }

    public override void ExitNamespace_declaration(AmethystParser.Namespace_declarationContext context)
    {
        var path = GetNamespacePath(new List<string>(), context);
        CreateOrGetNamespace(path, Context);
    }
}