using System.Diagnostics;
using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitGroup_expression(AmethystParser.Group_expressionContext context)
    {
        if (context.group() is not { } groupContext)
        {
            throw new UnreachableException();
        }
        
        return VisitExpression(groupContext.expression());
    }
}