using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    public AbstractValue VisitExpression(AmethystParser.ExpressionContext context)
    {
        return (AbstractValue)Visit(context)!;
    }
}