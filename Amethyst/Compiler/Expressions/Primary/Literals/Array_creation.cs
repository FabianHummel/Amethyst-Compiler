using System.Diagnostics;
using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitArray_creation(AmethystParser.Array_creationContext context)
    {
        if (context.expression() is not { } expressionContexts)
        {
            throw new UnreachableException();
        }
        
        if (expressionContexts.Length == 0)
        {
            AddCode($"data modify storage amethyst: {MemoryLocation} set value [];");
        }

        foreach (var expressionContext in expressionContexts)
        {
            var result = VisitExpression(expressionContext);

            var location = result.Location;

            if (result.DataType.IsScoreboardType)
            {
                MemoryLocation++;
                AddCode($"execute store result storage amethyst: {MemoryLocation} {result.DataType.StorageModifier} run scoreboard players get {result.Location} amethyst");
                location = MemoryLocation.ToString();
                MemoryLocation--;
            }
                
            AddCode($"data modify storage amethyst: {MemoryLocation} append from storage amethyst: {location}");
        }
        
        return new ArrayResult
        {
            Compiler = this,
            Location = MemoryLocation++.ToString(),
            Context = context
        };
    }
}