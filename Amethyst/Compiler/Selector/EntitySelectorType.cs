using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public partial class Compiler
{
    public override EntityTargetResult VisitEntitySelectorType(AmethystParser.EntitySelectorTypeContext context)
    {
        var selector = context.GetText();
        var entityTarget = EnumExtension.GetEnumFromMcfOperator<EntityTarget>(selector);
        return new EntityTargetResult(entityTarget);
    }
}