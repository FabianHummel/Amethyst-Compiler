using Amethyst.Language;
using static Amethyst.Utility.ParserUtility;

namespace Amethyst;

public partial class Compiler
{
    private static IEnumerable<AmethystParser.ExpressionContext> FlattenConjunctionExpressions(AmethystParser.ExpressionContext context)
    {
        return FlattenParserRules<
            AmethystParser.ExpressionContext,
            AmethystParser.ConjunctionExpressionContext>(
            context, target => target.expression());
    }
    
    public override AbstractValue VisitConjunctionExpression(AmethystParser.ConjunctionExpressionContext context)
    {
        var expressionContexts = FlattenConjunctionExpressions(context);
        
        bool isAlwaysFalse = false;
        
        var previousSP = StackPointer;
        
        // We create a scope to be able to early return from the function.
        var scope = EvaluateScoped("_and", cancel =>
        {
            foreach (var expressionContext in expressionContexts)
            {
                var expression = VisitExpression(expressionContext);
                
                if (expression.ToBoolean() is not { } booleanResult)
                {
                    throw new SyntaxException("Expected boolean result.", expressionContext);
                }

                if (booleanResult is ConstantBoolean booleanConstant)
                {
                    if (!booleanConstant.Value)
                    {
                        cancel();
                        isAlwaysFalse = true;
                        return;
                    }

                    continue;
                }

                if (booleanResult is IRuntimeValue runtimeValue)
                {
                    // Early return if the current expression is false (we don't need to check the rest).
                    AddCode($"execute if score {runtimeValue.Location} amethyst matches 0 run return fail");
                }
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
        
        return new RuntimeBoolean
        {
            Location = location,
            Compiler = this,
            Context = context,
            IsTemporary = true
        };
    }
}