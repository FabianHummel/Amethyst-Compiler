using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    /// <inheritdoc />
    /// <summary>
    ///     <p>A group expression that is denoted by an expression wrapped in parentheses. The expression
    ///     inside the parentheses is evaluated first before anything outside the parentheses.</p>
    ///     <p><inheritdoc /></p></summary>
    /// <example><p>Without grouping: <c>2 * 3 + 4</c>→<c>10</c></p>
    ///     <p>With grouping: <c>2 * (3 + 4)</c>→<c>14</c></p></example>
    public override AbstractValue VisitGroupedExpression(AmethystParser.GroupedExpressionContext context)
    {
        var groupContext = context.group();
        return VisitExpression(groupContext.expression());
    }
}