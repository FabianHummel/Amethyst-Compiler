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

        bool isAlwaysTrue = false;
        
        var previousSP = StackPointer;
        
        // We create a scope to be able to early return from the function.
        var scope = EvaluateScoped("_or", cancel =>
        {
            foreach (var andExpressionContext in andExpressionContexts)
            {
                if (VisitAnd_expression(andExpressionContext) is not { } andExpression)
                {
                    throw new SyntaxException("Expected and expression.", andExpressionContext);
                }
                
                if (andExpression.ToBoolean() is not { } booleanResult)
                {
                    throw new SyntaxException("Expected boolean result.", andExpressionContext);
                }

                if (booleanResult is BooleanConstant { Value: true })
                {
                    cancel();
                    isAlwaysTrue = true;
                    return;
                }

                var current = booleanResult.ToRuntimeValue();
                
                // Early return if the current expression is true (we don't need to check the rest).
                AddCode($"execute unless score {current.Location} amethyst matches 0 run return 1");
            }
            
            AddCode("return fail");
        });
        
        // Reset the stack pointer to the one before evaluating the current expression, as we don't need the allocated variables anymore.
        StackPointer = previousSP;

        var location = ++StackPointer;

        if (isAlwaysTrue)
        {
            AddCode($"scoreboard players set {location} amethyst 1");
        }
        else
        {
            AddCode($"execute store success score {location} amethyst run function {scope.McFunctionPath}");
        }

        return new BooleanResult
        {
            Location = location.ToString(),
            Compiler = this,
            Context = context,
            IsTemporary = true
        };
    }
}