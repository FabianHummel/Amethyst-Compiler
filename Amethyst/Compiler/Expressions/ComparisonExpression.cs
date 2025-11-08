using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public partial class Compiler
{
    /// <inheritdoc />
    /// <summary><p>Compares two values whether they are greater or less than another.</p>
    ///     <p><inheritdoc /></p></summary>
    /// <exception cref="SyntaxException">The operator for this comparison is invalid.</exception>
    public override AbstractValue VisitComparisonExpression(AmethystParser.ComparisonExpressionContext context)
    {
        var expressionContexts = context.expression();
        var left = VisitExpression(expressionContexts[0]);
        var right = VisitExpression(expressionContexts[1]);
        var operatorToken = context.GetChild(1).GetText();
        
        var op = Enum.GetValues<ComparisonOperator>()
            .First(op => op.GetAmethystOperatorSymbol() == operatorToken);

        return op switch
        {
            ComparisonOperator.LESS_THAN => left < right,
            ComparisonOperator.LESS_THAN_OR_EQUAL => left <= right,
            ComparisonOperator.GREATER_THAN => left > right,
            ComparisonOperator.GREATER_THAN_OR_EQUAL => left >= right,
            _ => throw new SyntaxException($"Invalid operator '{op}'.", context)
        };
    }
}