using Amethyst.Language;

namespace Amethyst;

public partial class Compiler 
{
    public override HashSet<string> VisitAttribute_list(AmethystParser.Attribute_listContext context)
    {
        return context.attribute()
            .Select(attributeContext => attributeContext.IDENTIFIER().GetText())
            .ToHashSet();
    }
}