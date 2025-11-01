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

        if (!Constants.TargetSelectorQueryKeys.TryGetValue(queryKey, out var selector))
        {
            return new SelectorQueryResult(queryKey, $"{queryKey}={result.ToTargetSelectorString()}", result is IRuntimeValue);
        }

        if (selector is not AbstractQuerySelector<AbstractValue> abstractValueQuerySelector)
        {
            throw new InvalidOperationException($"Expected abstract value selector for '{queryKey}' target selector.");
        }

        return abstractValueQuerySelector.Parse(queryKey, result);
    }
}