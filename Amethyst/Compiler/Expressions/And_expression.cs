using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;
using Type = Amethyst.Model.Type;

namespace Amethyst;

public partial class Compiler
{
    public override Result VisitAnd(AmethystParser.AndContext context)
    {
        if (context.equality() is not { } equalityContexts)
        {
            throw new UnreachableException();
        }
        
        if (equalityContexts.Length == 1)
        {
            return VisitEquality(equalityContexts[0]);
        }
            
        // We create a scope to be able to instantly return from the function (if the first expression is true).
        var scope = EvaluateScoped("_and", () =>
        {
            foreach (var equalityContext in equalityContexts)
            {
                if (VisitEquality(equalityContext) is { Type.IsBoolean: true } result)
                {
                    // Early return if the first expression is false (we don't need to check the rest).
                    AddCode($"execute if score {result.Location} amethyst matches 0 run return fail");
                }
                else
                {
                    throw new SyntaxException("Expected boolean expression.", equalityContext);
                }
                
                MemoryLocation--;
            }
        });
            
        AddCode($"function {scope.McFunctionPath}");
        
        return new Result
        {
            Location = MemoryLocation.ToString(),
            Type = new Type
            {
                BasicType = BasicType.Bool
            }
        };
    }
}