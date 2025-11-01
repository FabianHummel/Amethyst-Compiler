using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public partial class Compiler
{
    public override SelectorQueryResult VisitRangeSelector(AmethystParser.RangeSelectorContext context)
    {
        var queryKey = context.IDENTIFIER().GetText();

        var isQueryKeyKnown = Constants.TargetSelectorQueryKeys.TryGetValue(queryKey, out var selector);

        bool allowDecimals;
        if (!isQueryKeyKnown)
        {
            ConsoleUtility.PrintWarning($"Unknown target selector query key '{queryKey}'. Parsing result as-is.");
            allowDecimals = true;
        }
        else if (selector is not RangeSelector rangeSelector)
        {
            throw new SyntaxException("Expected range selector for '{queryKey}' target selector query.", context);
        }
        else
        {
            allowDecimals = rangeSelector.AllowDecimals;
        }
        
        var result = VisitRangeExpression(context.rangeExpression(), allowDecimals);

        if (!isQueryKeyKnown)
        {
            return new SelectorQueryResult(queryKey, $"{queryKey}={result.ToTargetSelectorString()}", result.ContainsRuntimeValues);
        }
        
        if (selector is not AbstractQuerySelector<RangeExpressionResult> rangeExpressionQuerySelector)
        {
            throw new InvalidOperationException($"Expected range expression selector for '{queryKey}' target selector.");
        }
        
        return rangeExpressionQuerySelector.Parse(queryKey, result);
    }
}