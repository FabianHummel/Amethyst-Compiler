using Amethyst.Language;

namespace Amethyst;

public partial class Compiler 
{
    public override List<string> VisitAttribute_list(AmethystParser.Attribute_listContext context)
    {
        return context.attribute()
            .Select(attributeContext => attributeContext.identifier().GetText())
            .ToList();
    }
}