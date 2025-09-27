using Amethyst.Language;
using static Amethyst.Utility.ParserUtility;

namespace Amethyst;

public partial class Compiler
{
    private static IEnumerable<AmethystParser.ExpressionContext> FlattenDisjunctionExpressions(AmethystParser.ExpressionContext context)
    {
        return FlattenParserRules<
            AmethystParser.ExpressionContext, 
            AmethystParser.DisjunctionExpressionContext>(
            context, target => target.expression());
    }
    
    public override AbstractValue VisitDisjunctionExpression(AmethystParser.DisjunctionExpressionContext context)
    {
        var expressionContexts = FlattenDisjunctionExpressions(context);

        bool isAlwaysTrue = false;
        
        var previousSP = StackPointer;
        
        // We create a scope to be able to early return from the function.
        var scope = EvaluateScoped("_or", cancel =>
        {
            foreach (var expressionContext in expressionContexts)
            {
                var expression = VisitExpression(expressionContext);
                
                if (expression.ToBoolean() is not { } booleanResult)
                {
                    throw new SyntaxException("Expected boolean result.", expressionContext);
                }

                if (booleanResult is ConstantBoolean { Value: true })
                {
                    cancel();
                    isAlwaysTrue = true;
                    return;
                }
                
                if (booleanResult is IRuntimeValue runtimeValue)
                {
                    // Early return if the current expression is true (we don't need to check the rest).
                    AddCode($"execute unless score {runtimeValue.Location} amethyst matches 0 run return 1");
                }
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

        return new RuntimeBoolean
        {
            Location = location,
            Compiler = this,
            Context = context,
            IsTemporary = true
        };
    }
}