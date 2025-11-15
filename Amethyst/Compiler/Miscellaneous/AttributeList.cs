using Amethyst.Language;

namespace Amethyst;

public partial class Compiler 
{
    /// <inheritdoc />
    /// <summary><p>Parses the list of attributes on e.g. a variable, function or record.</p>
    ///     <p><inheritdoc /></p></summary>
    /// <returns>A list of attributes added to the symbol.</returns>
    public override HashSet<string> VisitAttributeList(AmethystParser.AttributeListContext context)
    {
        return context.attribute()
            .Select(attributeContext => attributeContext.IDENTIFIER().GetText())
            .ToHashSet();
    }
}