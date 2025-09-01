using Amethyst.Language;
using Amethyst.Model;
using static Amethyst.Constants;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitFunction_declaration(AmethystParser.Function_declarationContext context)
    {
        if (Context.Datapack is null)
        {
            throw new Exception($"Consider configuring the datapack in '{CONFIG_FILE}' in order to use functions.");
        }
        
        if (context.IDENTIFIER() is not { } functionNameContext)
        {
            throw new SyntaxException("Expected function name.", context);
        }
        
        var functionName = functionNameContext.GetText();
        
        if (Scope.TryGetSymbol(functionName, out _))
        {
            throw new SyntaxException($"The symbol '{functionName}' has already been declared.", context);
        }
        
        var scope = VisitBlockNamed(context.block(), "_func");

        var attributes = VisitAttribute_list(context.attribute_list());
        
        if (attributes.Contains(ATTRIBUTE_TICK_FUNCTION))
        {
            Context.Datapack.TickFunctions.Add(scope.McFunctionPath);
        }
        
        if (attributes.Contains(ATTRIBUTE_LOAD_FUNCTION))
        {
            Context.Datapack.LoadFunctions.Add(scope.McFunctionPath);
        }
        
        Scope.Symbols.Add(functionName, new Function
        {
            Attributes = attributes
        });
        
        return null;
    }
}