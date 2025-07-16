using System.Diagnostics;
using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitExpression(AmethystParser.ExpressionContext context)
    {
        if (context.conditional_expression() is { } conditionalExpressionContext)
        {
            return VisitConditional_expression(conditionalExpressionContext);
        }

        if (context.assignment_expression() is { } assignmentExpressionContext)
        {
            throw new NotImplementedException();
        }
        
        throw new UnreachableException();
    }
}