using Amethyst.Language;
using Amethyst.Model.Types;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitArray_creation_expression(AmethystParser.Array_creation_expressionContext context)
    {
        return null!;
    }
}