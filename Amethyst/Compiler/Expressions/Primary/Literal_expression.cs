using Amethyst.Language;
using Amethyst.Model.Types;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitLiteral_expression(AmethystParser.Literal_expressionContext context)
    {
        return null!;
    }
}