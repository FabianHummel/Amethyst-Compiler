using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public partial class Compiler
{
    /// <summary>Calculates two preprocessor expressions using a multiplication, division, or modulo
    /// operator.<br /><inheritdoc /></summary>
    /// <exception cref="SyntaxException">Thrown when an invalid comparison operator is encountered.</exception>
    /// <seealso cref="VisitFactorExpression" />
    public override object VisitPreprocessorFactorExpression(AmethystParser.PreprocessorFactorExpressionContext context)
    {
        var expressionContexts = context.preprocessorExpression();
        var lhs = VisitPreprocessorExpression(expressionContexts[0]);
        var rhs = VisitPreprocessorExpression(expressionContexts[1]);
        
        var operatorToken = context.GetChild(1).GetText();
        var op = Enum.GetValues<ArithmeticOperator>()
            .First(op => op.GetAmethystOperatorSymbol() == operatorToken);

        return op switch
        {
            ArithmeticOperator.MULTIPLY => lhs * rhs,
            ArithmeticOperator.DIVIDE => lhs / rhs,
            ArithmeticOperator.MODULO => lhs % rhs,
            _ => throw new SyntaxException($"Invalid operator '{op}'.", context)
        };
    }
}