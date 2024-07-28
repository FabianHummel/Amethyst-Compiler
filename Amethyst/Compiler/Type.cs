using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override DataType VisitType(AmethystParser.TypeContext context)
    {
        return new DataType
        {
            BasicType = BasicType.Int,
        };
    }
}