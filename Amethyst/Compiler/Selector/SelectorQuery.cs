using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public SelectorQueryResultBase VisitSelectorQuery(AmethystParser.SelectorQueryContext context)
    {
        if (Visit(context) is not SelectorQueryResultBase result)
        {
            throw new InvalidOperationException($"Selector query did not return a {nameof(SelectorQueryResultBase)}.");
        }
        
        return result;
    }
}