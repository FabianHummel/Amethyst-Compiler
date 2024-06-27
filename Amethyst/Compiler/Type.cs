using Amethyst.Language;
using Amethyst.Model;
using Type = Amethyst.Model.Type;

namespace Amethyst;

public partial class Compiler
{
    public override Type VisitType(AmethystParser.TypeContext context)
    {
        return new Type
        {
            BasicType = BasicType.Int,
        };
    }
}