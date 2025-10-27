using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override SelectorQueryResult VisitRangeSelector(AmethystParser.RangeSelectorContext context)
    {
        var identifier = context.IDENTIFIER();
        var queryKey = identifier.GetText();
            
        if (queryKey is "distance" or "x_rotation" or "y_rotation")
        {
            var rangeExpressionContext = context.rangeExpression();
            
            if (VisitRangeExpression(rangeExpressionContext, allowDecimals: false) is { } rangeExpression)
            {
                return new SelectorQueryResult(queryKey, $"{queryKey}={rangeExpression}", rangeExpression.ContainsRuntimeValues);
            }
        }
        
        throw new SyntaxException($"Invalid query key '{queryKey}' for range selector.", context);
    }
}