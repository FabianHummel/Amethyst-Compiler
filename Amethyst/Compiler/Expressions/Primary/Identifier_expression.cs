using Amethyst.Language;
using Amethyst.Model.Types;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitIdentifier_expression(AmethystParser.Identifier_expressionContext context)
    {
        return null!;
    }
}