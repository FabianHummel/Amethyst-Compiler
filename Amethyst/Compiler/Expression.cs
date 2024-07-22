using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public Result VisitExpressionTargeted(AmethystParser.ExpressionContext context, string target)
    {
        if (context.conditional() is { } conditionalContext)
        {
            return VisitConditionalTargeted(conditionalContext, target: target);
        }
        // if (context.assignment() is { } assignmentContext)
        // {
        //     return VisitAssignment(assignmentContext);
        // }

        throw new UnreachableException();
    }
}