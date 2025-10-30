using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public SelectorQueryResult VisitNumericSelector(string queryKey, AmethystParser.ExpressionContext context, bool allowDecimals)
    {
        var result = VisitExpression(context);
        
        if (!allowDecimals && result is ConstantDecimal or RuntimeDecimal)
        {
            throw new SyntaxException("Unexpected decimal value.", context);
        }

        var queryResult = result.ToTargetSelectorString();

        return new SelectorQueryResult(queryKey, $"{queryKey}={queryResult}", result is IRuntimeValue);
    }
}