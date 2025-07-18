using System.Diagnostics;
using Amethyst.Language;

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
        
        bool isAlwaysFalse = false;
        
        var previousSP = StackPointer;
        
        // We create a scope to be able to early return from the function.
        var scope = EvaluateScoped("_and", cancel =>
        {
            foreach (var equalityExpressionContext in equalityExpressionContexts)
            {
                if (VisitEquality_expression(equalityExpressionContext) is not { } equalityExpression)
                {
                    throw new SyntaxException("Expected equality expression.", equalityExpressionContext);
                }
                
                if (equalityExpression.ToBoolean() is not { } booleanResult)
                {
                    throw new SyntaxException("Expected boolean result.", equalityExpressionContext);
                }

                if (booleanResult is BooleanConstant booleanConstant)
                {
                    if (!booleanConstant.Value)
                    {
                        cancel();
                        isAlwaysFalse = true;
                        return;
                    }

                    continue;
                }
                
                var current = booleanResult.ToRuntimeValue();
                
                // Early return if the current expression is false (we don't need to check the rest).
                AddCode($"execute if score {current.Location} amethyst matches 0 run return fail");
            }
            
            AddCode("return 1");
        });

        // Reset the stack pointer to the one before evaluating the current expression, as we don't need the allocated variables anymore.
        StackPointer = previousSP;
        
        var location = ++StackPointer;
        
        if (isAlwaysFalse)
        {
            AddCode($"scoreboard players set {location} amethyst 0");
        }
        else
        {
            AddCode($"execute store success score {location} amethyst run function {scope.McFunctionPath}");
        }
        
        return new BooleanResult
        {
            Location = location,
            Compiler = this,
            Context = context,
            IsTemporary = true
        };
    }
}