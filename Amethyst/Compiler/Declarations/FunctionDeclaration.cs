using Amethyst.Language;
using Amethyst.Model;
using static Amethyst.Constants;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitFunctionDeclaration(AmethystParser.FunctionDeclarationContext context)
    {
        if (Context.Datapack is null)
        {
            throw new SyntaxException($"Consider configuring a datapack in '{CONFIG_FILE}' in order to use functions.", context);
        }
        
        var functionName = context.IDENTIFIER().GetText();
        if (Scope.TryGetSymbol(functionName, out _))
        {
            throw new SymbolAlreadyDeclaredException(functionName, context);
        }
        
        var scope = VisitBlockNamed(context.block(), "_func");

        var attributes = VisitAttributeList(context.attributeList());
        
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