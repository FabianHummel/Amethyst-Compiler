using Amethyst.Language;
using Amethyst.Model.Types;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitObject_creation_expression(AmethystParser.Object_creation_expressionContext context)
    {
        return null!;
    }
}