using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Model.Types;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitAnd(AmethystParser.AndContext context)
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
                var previousMemoryLocation = MemoryLocation;
                if (VisitEquality(equalityContext).ToBool is not { } result)
                {
                    throw new SyntaxException("Expected boolean expression.", equalityContext);
                }

                MemoryLocation = previousMemoryLocation;
                // Early return if the first expression is false (we don't need to check the rest).
                AddCode($"execute if score {result.Location} amethyst matches 0 run return fail");
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