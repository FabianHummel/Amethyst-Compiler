using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public SelectorQueryResult VisitStringSelector(string queryKey, AmethystParser.ExpressionContext context)
    {
        var result = VisitExpression(context);
        
        var queryResult = result.ToTargetSelectorString();
        
        return new SelectorQueryResult(queryKey, $"{queryKey}={queryResult}", result is IRuntimeValue);
    }
}