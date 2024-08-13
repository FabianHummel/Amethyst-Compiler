using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitOr_expression(AmethystParser.Or_expressionContext context)
    {
        if (context.and_expression() is not { } andExpressionContexts)
        {
            throw new UnreachableException();
        }
        
        if (andExpressionContexts.Length == 1)
        {
            return VisitAnd_expression(andExpressionContexts[0]);
        }
        
        // We create a scope to be able to instantly return from the function (if the first expression is true).
        var scope = EvaluateScoped("_or", () =>
        {
            foreach (var andExpressionContext in andExpressionContexts)
            {
                var previousMemoryLocation = MemoryLocation;
                var result = VisitAnd_expression(andExpressionContext).ToBool;
                MemoryLocation = previousMemoryLocation;
                
                // Early return if the first expression is true (we don't need to check the rest).
                AddCode($"execute unless score {result.Location} amethyst matches 0 run return fail");
            }
        });
            
        AddCode($"function {scope.McFunctionPath}");

        return new BoolResult
        {
            Location = MemoryLocation.ToString(),
            Compiler = this,
            Context = context
        };
    }
}