using Amethyst.Language;
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
        
        var previousFunction = Scope.Name;
        var functionName = context.identifier().GetText();
        Scope.Name = Namespace.Functions[functionName].Name;
        
        VisitBlock(context.block());
        
        Scope.Name = previousFunction;
        
        return null;
    }
}