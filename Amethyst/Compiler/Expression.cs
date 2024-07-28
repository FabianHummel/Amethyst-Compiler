using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model.Types;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitExpression(AmethystParser.ExpressionContext context)
    {
        if (context.conditional() is { } conditionalContext)
        {
            return VisitConditional(conditionalContext);
        }
        // if (context.assignment() is { } assignmentContext)
        // {
        //     return VisitAssignment(assignmentContext);
        // }

        throw new UnreachableException();
    }
}