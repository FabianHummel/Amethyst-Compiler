using Amethyst.Language;
using Amethyst.Model;
using Type = Amethyst.Model.Type;

namespace Amethyst;

public partial class Compiler
{
    public Result VisitEqualityTargeted(AmethystParser.EqualityContext context, string target)
    {
        AddCode("# Evaluate equality expression.");
        return new Result
        {
            Location = target,
            Type = new Type
            {
                BasicType = BasicType.Bool,
            }
        };
    }
}