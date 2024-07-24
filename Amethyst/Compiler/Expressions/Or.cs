using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;
using Type = Amethyst.Model.Type;

namespace Amethyst;

public partial class Compiler
{
    public override Result VisitOr(AmethystParser.OrContext context)
    {
        if (context.and() is not { } andContexts)
        {
            throw new UnreachableException();
        }
        
        if (andContexts.Length == 1)
        {
            return VisitAnd(andContexts[0]);
        }
            
        // We create a scope to be able to instantly return from the function (if the first expression is true).
        var scope = EvaluateScoped("_or", () =>
        {
            foreach (var andContext in andContexts)
            {
                if (VisitAnd(andContext) is { Type.IsBoolean: true } result)
                {
                    // Early return if the first expression is true (we don't need to check the rest).
                    AddCode($"execute if score {result.Location} amethyst matches 1 run return fail");
                }
                else
                {
                    throw new SyntaxException("Expected boolean expression.", andContext);
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