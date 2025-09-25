using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractValue VisitGroupedExpression(AmethystParser.GroupedExpressionContext context)
    {
        var groupContext = context.group();
        return VisitExpression(groupContext.expression());
    }
}