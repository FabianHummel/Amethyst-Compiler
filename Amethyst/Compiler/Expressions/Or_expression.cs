using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Model.Types;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitOr(AmethystParser.OrContext context)
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
                var previousMemoryLocation = MemoryLocation;
                if (VisitAnd(andContext).ToBool is not { } result)
                {
                    throw new SyntaxException("Expected boolean expression.", andContext);
                }

                MemoryLocation = previousMemoryLocation;
                // Early return if the first expression is true (we don't need to check the rest).
                AddCode($"execute unless score {result.Location} amethyst matches 0 run return fail");
            }
        });
            
        AddCode($"function {scope.McFunctionPath}");

        return new BoolResult
        {
            Location = MemoryLocation.ToString(),
            Compiler = this
        };
    }
}