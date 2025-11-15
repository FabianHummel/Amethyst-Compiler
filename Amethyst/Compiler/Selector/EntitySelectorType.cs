using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public partial class Compiler
{
    /// <inheritdoc />
    /// <summary><p>Parses an entity selector type.</p><p><inheritdoc /></p></summary>
    public override EntityTargetResult VisitEntitySelectorType(AmethystParser.EntitySelectorTypeContext context)
    {
        var selector = context.GetText();
        var entityTarget = EnumExtension.GetEnumFromMcfOperator<EntityTarget>(selector);
        return new EntityTargetResult(entityTarget);
    }
}