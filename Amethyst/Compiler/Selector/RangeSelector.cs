using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public partial class Compiler
{
    /// <inheritdoc />
    /// <summary><p>Parses a selector query that uses a range expression as the value.</p>
    ///     <p><inheritdoc /></p></summary>
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
            var value = new SelectorQueryValue(result.ToTargetSelectorString(), isNegated: false, context: result.Context, isRuntimeValue: result.ContainsRuntimeValues);
            return new SelectorQueryResult(queryKey, value);
        }
        
        if (selector is not AbstractQuerySelector<RangeExpressionResult> rangeExpressionQuerySelector)
        {
            throw new InvalidOperationException($"Expected range expression selector for '{queryKey}' target selector.");
        }
        
        return rangeExpressionQuerySelector.Parse(queryKey, isNegated: false, result);
    }
}