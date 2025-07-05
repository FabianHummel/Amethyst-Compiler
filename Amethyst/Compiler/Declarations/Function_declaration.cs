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
        
        var scope = VisitBlockNamed(context.block(), "_func");
        
        var attributes = context.attribute_list().attribute()
            .Select(attributeContext => attributeContext.identifier().GetText())
            .ToHashSet();
        
        if (attributes.Contains(ATTRIBUTE_TICK_FUNCTION))
        {
            Context.Datapack.TickFunctions.Add(scope.McFunctionPath);
        }
        
        if (attributes.Contains(ATTRIBUTE_LOAD_FUNCTION))
        {
            Context.Datapack.LoadFunctions.Add(scope.McFunctionPath);
        }
        
        if (attributes.Contains(ATTRIBUTE_EXPORT_FUNCTION))
        {
            Context.Datapack.ExportedFunctions.Add(scope.McFunctionPath, functionName);
        }
        
        return null;
    }
}