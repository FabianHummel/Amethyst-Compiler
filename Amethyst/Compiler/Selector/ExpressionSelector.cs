using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public partial class Compiler
{
    public override SelectorQueryResult VisitExpressionSelector(AmethystParser.ExpressionSelectorContext context)
    {
        var queryKey = context.IDENTIFIER().GetText();

        if (!Constants.TargetSelectorQueryKeys.ContainsKey(queryKey))
        {
            ConsoleUtility.PrintWarning($"Unknown target selector query key '{queryKey}'. Parsing result as-is.");
        }
        
        var result = VisitExpression(context.expression());
        
        var isNegated = context.NOT() != null;

        if (!Constants.TargetSelectorQueryKeys.TryGetValue(queryKey, out var selector))
        {
            var value = new SelectorQueryValue(result.ToTargetSelectorString(), isNegated, result.Context, result is IRuntimeValue);
            return new SelectorQueryResult(queryKey, value);
        }

        if (selector is not AbstractQuerySelector<AbstractValue> abstractValueQuerySelector)
        {
            throw new InvalidOperationException($"Expected abstract value selector for '{queryKey}' target selector.");
        }

        return abstractValueQuerySelector.Parse(queryKey, isNegated, result);
    }
}