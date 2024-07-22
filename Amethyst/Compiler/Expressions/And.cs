using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public Result VisitAndTargeted(AmethystParser.AndContext context, string target)
    {
        if (context.equality() is { } equalityContexts)
        {
            if (equalityContexts.Length == 1)
            {
                return VisitEqualityTargeted(equalityContexts[0], target: target);
            }
            
            // We create a scope to be able to instantly return from the function (if the first expression is true).
            var scope = EvaluateScoped("_and", () =>
            {
                foreach (var equalityContext in equalityContexts)
                {
                    if (VisitEqualityTargeted(equalityContext, target: $"_{StackPointer}") is { Type.IsBoolean: true } result)
                    {
                        // Early return if the first expression is false (we don't need to check the rest).
                        AddCode($"execute if score {result.Location} amethyst matches 0 run return fail");
                    }
                    else
                    {
                        throw new SyntaxException("Expected boolean expression.", equalityContext);
                    }
                }
            });
            
            AddCode($"function {scope.McFunctionPath}");
        }

        throw new UnreachableException();
    }
}