using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public SelectorQueryResult VisitSelectorQuery(AmethystParser.SelectorQueryContext context)
    {
        if (Visit(context) is not SelectorQueryResult result)
        {
            throw new InvalidOperationException($"Selector query did not return a {nameof(SelectorQueryResult)}.");
        }
        
        return result;
    }
}