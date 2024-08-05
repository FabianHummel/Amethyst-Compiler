using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitUnary_expression(AmethystParser.Unary_expressionContext context)
    {
        if (context.primary_expression() is { } primaryExpressionContext)
        {
            return VisitPrimary_expression(primaryExpressionContext);
        }
        
        var unaryExpression = context.unary_expression();
        
        var operatorToken = unaryExpression.GetChild(0).GetText();
        
        return VisitUnary_expression(unaryExpression);
    }
}