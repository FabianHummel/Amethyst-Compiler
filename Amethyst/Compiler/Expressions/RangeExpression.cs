using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public RangeExpression VisitRangeExpression(AmethystParser.RangeExpressionContext context, bool allowDecimals)
    {
        var result = new RangeExpression();

        if (context.GetChild(0) is AmethystParser.ExpressionContext startExpressionContext)
        {
            result.Start = VisitExpression(startExpressionContext);
        }
        
        if (context.GetChild(context.ChildCount - 1) is AmethystParser.ExpressionContext stopExpressionContext)
        {
            result.Stop = VisitExpression(stopExpressionContext);
        }
        
        if (result.Start is null && result.Stop is null)
        {
            throw new SyntaxException("Invalid range expression. At least one of the range bounds must be specified.", context);
        }
            
        if (!allowDecimals && (result.Start is ConstantDecimal or RuntimeDecimal || result.Stop is ConstantDecimal or RuntimeDecimal))
        {
            throw new SyntaxException("Unexpected decimal value.", context);
        }

        return result;
    }
}