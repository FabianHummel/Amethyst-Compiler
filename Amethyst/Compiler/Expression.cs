using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model.Types;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitExpression(AmethystParser.ExpressionContext context)
    {
        if (context.conditional_expression() is { } conditionalExpressionContext)
        {
            return VisitConditional_expression(conditionalExpressionContext);
        }

        throw new UnreachableException();
    }
}