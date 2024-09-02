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
                
                if (VisitAnd_expression(andExpressionContext) is not { } andExpression)
                {
                    throw new SyntaxException("Expected and expression.", andExpressionContext);
                }
                
                if (andExpression.MakeBoolean() is not { } booleanResult)
                {
                    throw new SyntaxException("Expected boolean result.", andExpressionContext);
                }
                
                if (booleanResult.ConstantValue is bool constantValue)
                {
                    if (constantValue)
                    {
                        AddCode("return 1");
                    }
                    
                    continue;
                }

                var current = booleanResult.MakeVariable();
                
                // Early return if the current expression is true (we don't need to check the rest).
                AddCode($"execute unless score {current.Location} amethyst matches 0 run return 1");
                
                // Reset the memory location to the one before evaluating the current expression, as we don't need the allocated variables anymore.
                MemoryLocation = previousMemoryLocation;
            }
            
            AddCode("return fail");
        });
        
        AddCode($"execute store success score {MemoryLocation} amethyst run function {scope.McFunctionPath}");

        return new BooleanResult
        {
            Location = MemoryLocation.ToString(),
            Compiler = this,
            Context = context,
            IsTemporary = true
        };
    }
}