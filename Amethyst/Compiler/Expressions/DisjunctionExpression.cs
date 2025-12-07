using Amethyst.Language;
using Amethyst.Model;
using static Amethyst.Utility.ParserUtility;

namespace Amethyst;

public partial class Compiler
{
    /// <summary>Flattens all disjunction expressions into a single list. This is used to evaluate an
    /// entire chain of disjunctions in one go and not having to spam single evaluations.</summary>
    /// <param name="context">The root context where to look for disjunction expressions.</param>
    /// <returns>A list of all chained disjunction expressions within the context.</returns>
    private static IEnumerable<AmethystParser.ExpressionContext> FlattenDisjunctionExpressions(AmethystParser.ExpressionContext context)
    {
        return FlattenParserRules<
            AmethystParser.ExpressionContext, 
            AmethystParser.DisjunctionExpressionContext>(
            context, target => target.expression());
    }

    /// <inheritdoc />
    /// <summary>
    ///     <p>Evaluates a chain of disjunctions. If any of the elements is known to be true, the chain
    ///     immediately results a truthy value.</p>
    ///     <p><inheritdoc /></p></summary>
    /// <example><p><c>abc || xyz || true || false</c> → <c>true</c></p></example>
    /// <example><p><c>abc || xyz</c> → <see cref="RuntimeBoolean" /></p></example>
    /// <exception cref="SyntaxException">An element of the chain is not interpretable as a boolean value.</exception>
    public override AbstractValue VisitDisjunctionExpression(AmethystParser.DisjunctionExpressionContext context)
    {
        var expressionContexts = FlattenDisjunctionExpressions(context);

        bool isAlwaysTrue = false;
        
        var previousSP = StackPointer;
        
        // We create a scope to be able to early return from the function.
        string mcFunctionPath;
        using (this.EvaluateScoped("_or"))
        {
            mcFunctionPath = Scope.McFunctionPath;
            foreach (var expressionContext in expressionContexts)
            {
                var expression = VisitExpression(expressionContext);
                
                if (expression.ToBoolean() is not { } booleanResult)
                {
                    throw new SyntaxException("Expected boolean result.", expressionContext);
                }

                if (booleanResult is ConstantBoolean { Value: true })
                {
                    Scope.Cancel();
                    isAlwaysTrue = true;
                }
                
                if (booleanResult is IRuntimeValue runtimeValue)
                {
                    // Early return if the current expression is true (we don't need to check the rest).
                    this.AddCode($"execute unless score {runtimeValue.Location} matches 0 run return 1");
                }
            }
            
            this.AddCode("return fail");
        }
        
        // Reset the stack pointer to the one before evaluating the current expression, as we don't need the allocated variables anymore.
        StackPointer = previousSP;

        var location = Location.Scoreboard(++StackPointer);

        if (isAlwaysTrue)
        {
            this.AddCode($"scoreboard players set {location} 1");
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