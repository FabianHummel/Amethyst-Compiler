using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public Result VisitOrTargeted(AmethystParser.OrContext context, string target)
    {
        if (context.and() is { } andContexts)
        {
            if (andContexts.Length == 1)
            {
                return VisitAndTargeted(andContexts[0], target: target);
            }
            
            // We create a scope to be able to instantly return from the function (if the first expression is true).
            var scope = EvaluateScoped("_or", () =>
            {
                foreach (var andContext in andContexts)
                {
                    if (VisitAndTargeted(andContext, target: $"_{StackPointer}") is { Type.IsBoolean: true } result)
                    {
                        // Early return if the first expression is true (we don't need to check the rest).
                        AddCode($"execute if score {result.Location} amethyst matches 1 run return fail");
                    }
                    else
                    {
                        throw new SyntaxException("Expected boolean expression.", andContext);
                    }
                }
            });
            
            AddCode($"function {scope.McFunctionPath}");
        }
        
        throw new UnreachableException();
    }
}