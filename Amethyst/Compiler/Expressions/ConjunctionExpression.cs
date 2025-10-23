using Amethyst.Language;
using Amethyst.Model;
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
        string mcFunctionPath;
        using (this.EvaluateScoped("_and"))
        {
            mcFunctionPath = Scope.McFunctionPath;
            foreach (var expressionContext in expressionContexts)
            {
                var expression = VisitExpression(expressionContext);
                
                if (expression.ToBoolean() is not { } booleanResult)
                {
                    throw new SyntaxException("Expected boolean result.", expressionContext);
                }

                if (booleanResult is ConstantBoolean { Value: false })
                {
                    Scope.Cancel();
                    isAlwaysFalse = true;
                }
                if (booleanResult is IRuntimeValue runtimeValue)
                {
                    // Early return if the current expression is false (we don't need to check the rest).
                    this.AddCode($"execute if score {runtimeValue.Location} matches 0 run return fail");
                }
            }
            
            this.AddCode("return 1");
        }

        // Reset the stack pointer to the one before evaluating the current expression, as we don't need the allocated variables anymore.
        StackPointer = previousSP;
        
        var location = Location.Scoreboard(++StackPointer);
        
        if (isAlwaysFalse)
        {
            this.AddCode($"scoreboard players set {location} 0");
        }
        else
        {
            this.AddCode($"execute store success score {location} run function {mcFunctionPath}");
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