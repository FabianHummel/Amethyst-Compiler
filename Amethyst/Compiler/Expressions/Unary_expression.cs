using Amethyst.Language;
using Amethyst.Model.Types;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitUnary(AmethystParser.UnaryContext context)
    {
        if (context.primary() is { } primaryContext)
        {
            return VisitPrimary(primaryContext);
        }
        
        var unaryExpression = context.unary();
        
        var operatorToken = unaryExpression.GetChild(0).GetText();
        
        return VisitUnary(unaryExpression);
    }
}