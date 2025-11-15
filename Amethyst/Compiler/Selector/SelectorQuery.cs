using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    /// <summary>
    ///     <p>Parses a selector query and checks if the result is a <see cref="SelectorQueryResult" />.</p>
    /// </summary>
    public SelectorQueryResult VisitSelectorQuery(AmethystParser.SelectorQueryContext context)
    {
        if (Visit(context) is not SelectorQueryResult result)
        {
            throw new InvalidOperationException($"Selector query did not return a {nameof(SelectorQueryResult)}.");
        }
        
        return result;
    }
}