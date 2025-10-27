using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public partial class Compiler
{
    /// <summary>Calculates two preprocessor expressions using addition or subtraction operator.<br />
    /// <inheritdoc /></summary>
    /// <exception cref="SyntaxException">Thrown when an invalid comparison operator is encountered.</exception>
    /// <seealso cref="VisitTermExpression" />
    public override object VisitPreprocessorTermExpression(AmethystParser.PreprocessorTermExpressionContext context)
    {
        var expressionContexts = context.preprocessorExpression();
        var lhs = VisitPreprocessorExpression(expressionContexts[0]);
        var rhs = VisitPreprocessorExpression(expressionContexts[1]);
        
        var operatorToken = context.GetChild(1).GetText();
        var op = Enum.GetValues<ArithmeticOperator>()
            .First(op => op.GetAmethystOperatorSymbol() == operatorToken);

        return op switch
        {
            ArithmeticOperator.ADD => lhs + rhs,
            ArithmeticOperator.SUBTRACT => lhs - rhs,
            _ => throw new SyntaxException($"Invalid operator '{op}'.", context)
        };
    }
}