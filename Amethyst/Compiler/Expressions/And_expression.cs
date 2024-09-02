using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitAnd_expression(AmethystParser.And_expressionContext context)
    {
        if (context.equality_expression() is not { } equalityExpressionContexts)
        {
            throw new UnreachableException();
        }
        
        if (equalityExpressionContexts.Length == 1)
        {
            return VisitEquality_expression(equalityExpressionContexts[0]);
        }
        
        // We create a scope to be able to instantly return from the function (if the first expression is true).
        var scope = EvaluateScoped("_and", () =>
        {
            foreach (var equalityExpressionContext in equalityExpressionContexts)
            {
                var previousMemoryLocation = MemoryLocation;
                
                if (VisitEquality_expression(equalityExpressionContext) is not { } equalityExpression)
                {
                    throw new SyntaxException("Expected equality expression.", equalityExpressionContext);
                }
                
                if (equalityExpression.MakeBoolean() is not { } booleanResult)
                {
                    throw new SyntaxException("Expected boolean result.", equalityExpressionContext);
                }
                
                if (booleanResult.ConstantValue is bool constantValue)
                {
                    if (!constantValue)
                    {
                        AddCode("return fail");
                    }
                    
                    continue;
                }

                var current = booleanResult.MakeVariable();
                
                // Early return if the current expression is false (we don't need to check the rest).
                AddCode($"execute if score {current.Location} amethyst matches 0 run return fail");
                
                // Reset the memory location to the one before evaluating the current expression, as we don't need the allocated variables anymore.
                MemoryLocation = previousMemoryLocation;
            }
            
            AddCode("return 1");
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