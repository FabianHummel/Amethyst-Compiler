using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitExpression(AmethystParser.ExpressionContext context)
    {
        return (AbstractResult)Visit(context)!;
    }
}