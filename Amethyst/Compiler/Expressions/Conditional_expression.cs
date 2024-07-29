using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model.Types;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitConditional_expression(AmethystParser.Conditional_expressionContext context)
    {
        if (context.or_expression() is not { } orExpressionContext)
        {
            throw new UnreachableException();
        }
        
        return VisitOr_expression(orExpressionContext);
    }
}