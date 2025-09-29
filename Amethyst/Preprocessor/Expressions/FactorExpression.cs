using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public partial class Compiler
{
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