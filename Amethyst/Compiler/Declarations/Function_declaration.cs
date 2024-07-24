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
        
        var functionName = context.identifier().GetText();
        VisitBlockNamed(context.block(), Namespace.Functions[functionName].Scope.Name!);
        
        return null;
    }
}