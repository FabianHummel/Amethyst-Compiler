using Amethyst.Language;
using Amethyst.Model;
using static Amethyst.Constants;

namespace Amethyst;

public class AmethystParseListener : AmethystBaseListener
{
    private Parser Parser { get; }
    
    public AmethystParseListener(Parser parser)
    {
        Parser = parser;
    }
    
    public override void ExitFunction_declaration(AmethystParser.Function_declarationContext context)
    {
        if (Parser.Ns == null)
        {
            throw new Exception("Namespace is not defined. Either place the file in a folder or use the namespace keyword.");
        }
        
        var name = context.identifier().GetText();
        
        if (Parser.Ns.Functions.ContainsKey(name))
        {
            throw new SyntaxException($"Function '{name}' is already defined in namespace '{Parser.Ns.Scope.Name}'.", context);
        }
        
        Parser.Ns.Functions.Add(name, new Function
        {
            Scope = new Scope
            {
                Name = Parser.Ns.GetFunctionName(name),
                Context = Parser.Context,
                Parent = Parser.Ns.Scope
            },
            Attributes = context.attribute_list().attribute().Select(attributeContext =>
            { 
                return attributeContext.identifier().GetText();
            }).ToList()
        });
    }

    public override void ExitNamespace_declaration(AmethystParser.Namespace_declarationContext context)
    {
        var name = context.identifier().GetText();
        if (Parser.Ns is { } ns)
        {
            throw new SyntaxException($"Namespace '{name}' cannot be used instead of '{ns.Scope.Name}' because the file is placed inside '/{SOURCE_DIRECTORY}/{ns.Scope.Name}'. " +
                                      $"Either place the file directly inside '/{SOURCE_DIRECTORY}' or remove the namespace declaration.",
                context.identifier().Start.Line,
                context.identifier().Start.Column,
                Parser.FilePath);
        }

        Parser.Ns = new Namespace
        {
            Context = Parser.Context,
            Scope = new Scope
            {
                Name = name,
                Parent = null,
                Context = Parser.Context
            }
        };

        Parser.Ns.Functions.Add("_load", new Function
        {
            Scope = new Scope
            {
                Name = Parser.Ns.GetFunctionName("_load"),
                Context = Parser.Context,
                Parent = Parser.Ns.Scope
            },
            Attributes = new List<string> { ATTRIBUTE_LOAD_FUNCTION }
        });

        Parser.Context.Namespaces.Add(Parser.Ns);
    }
}