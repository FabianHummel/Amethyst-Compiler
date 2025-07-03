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
        
        // We create a scope to be able to early return from the function.
        var scope = EvaluateScoped("_or", () =>
        {
            foreach (var andExpressionContext in andExpressionContexts)
            {
                var previousSP = StackPointer;
                
                if (VisitAnd_expression(andExpressionContext) is not { } andExpression)
                {
                    throw new SyntaxException("Expected and expression.", andExpressionContext);
                }
                
                if (andExpression.ToBoolean() is not { } booleanResult)
                {
                    throw new SyntaxException("Expected boolean result.", andExpressionContext);
                }

                if (booleanResult is BooleanConstant booleanConstant)
                {
                    if (booleanConstant.AsBoolean)
                    {
                        AddCode("return 1");
                    }
                    
                    continue;
                }

                var current = booleanResult.ToRuntimeValue();
                
                // Early return if the current expression is true (we don't need to check the rest).
                AddCode($"execute unless score {current.Location} amethyst matches 0 run return 1");
                
                // Reset the stack pointer to the one before evaluating the current expression, as we don't need the allocated variables anymore.
                StackPointer = previousSP;
            }
            
            AddCode("return fail");
        });

        var location = ++StackPointer;
        
        // run the actual expression
        AddCode($"execute store success score {location} amethyst run function {scope.McFunctionPath}");

        return new BooleanResult
        {
            Location = location.ToString(),
            Compiler = this,
            Context = context,
            IsTemporary = true
        };
    }
}