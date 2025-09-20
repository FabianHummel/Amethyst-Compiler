using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public partial class Compiler
{
    public override object VisitPreprocessorComparisonExpression(AmethystParser.PreprocessorComparisonExpressionContext context)
    {
        var expressionContexts = context.preprocessorExpression();
        var lhs = VisitPreprocessorExpression(expressionContexts[0]);
        var rhs = VisitPreprocessorExpression(expressionContexts[1]);
        
        var operatorToken = context.GetChild(1).GetText();
        var op = Enum.GetValues<ComparisonOperator>()
            .First(op => op.GetAmethystOperatorSymbol() == operatorToken);

        return lhs.Calculate(rhs, op);
    }
}