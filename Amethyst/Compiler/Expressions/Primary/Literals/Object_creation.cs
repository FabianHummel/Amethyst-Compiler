using System.Diagnostics;
using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitObject_creation(AmethystParser.Object_creationContext context)
    {
        if (context.object_element() is not { } objectElementContexts)
        {
            throw new UnreachableException();
        }
        
        if (objectElementContexts.Length == 0)
        {
            AddCode($"data modify storage amethyst: {MemoryLocation} set value {{}}");
        }

        foreach (var objectElementContext in objectElementContexts)
        {
            var key = objectElementContext.identifier().IDENTIFIER().Symbol;
            
            var result = VisitExpression(objectElementContext.expression());
            
            if (result.DataType.IsScoreboardType)
            {
                AddCode($"execute store result storage amethyst: {MemoryLocation}.{key} {result.DataType.StorageModifier} run scoreboard players get {result.Location} amethyst");
            }
            else
            {
                AddCode($"data modify storage amethyst: {MemoryLocation}.{key} set value {result.Location}");
            }
        }
        
        return new DynObjectResult
        {
            Compiler = this,
            Location = MemoryLocation++.ToString(),
            Context = context
        };
    }
}