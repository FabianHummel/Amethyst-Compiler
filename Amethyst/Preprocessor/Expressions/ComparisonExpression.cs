using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public partial class Compiler
{
    /// <summary><p>Compares two preprocessor expressions using a comparison operator. </p>
    ///     <p><inheritdoc /></p></summary>
    /// <exception cref="SyntaxException">Thrown when an invalid comparison operator is encountered.</exception>
    /// <seealso cref="VisitComparisonExpression" />
    public override object VisitPreprocessorComparisonExpression(AmethystParser.PreprocessorComparisonExpressionContext context)
    {
        var expressionContexts = context.preprocessorExpression();
        var lhs = VisitPreprocessorExpression(expressionContexts[0]);
        var rhs = VisitPreprocessorExpression(expressionContexts[1]);
        
        var operatorToken = context.GetChild(1).GetText();
        var op = Enum.GetValues<ComparisonOperator>()
            .First(op => op.GetAmethystOperatorSymbol() == operatorToken);

        return op switch
        {
            ComparisonOperator.LESS_THAN => lhs < rhs,
            ComparisonOperator.LESS_THAN_OR_EQUAL => lhs <= rhs,
            ComparisonOperator.GREATER_THAN => lhs > rhs,
            ComparisonOperator.GREATER_THAN_OR_EQUAL => lhs >= rhs,
            ComparisonOperator.EQUAL => lhs == rhs,
            ComparisonOperator.NOT_EQUAL => lhs != rhs,
            _ => throw new SyntaxException($"Invalid operator '{op}'.", context)
        };
    }
}