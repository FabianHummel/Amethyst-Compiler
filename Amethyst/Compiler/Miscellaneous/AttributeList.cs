using Amethyst.Language;

namespace Amethyst;

public partial class Compiler 
{
    public override HashSet<string> VisitAttributeList(AmethystParser.AttributeListContext context)
    {
        return context.attribute()
            .Select(attributeContext => attributeContext.IDENTIFIER().GetText())
            .ToHashSet();
    }
}