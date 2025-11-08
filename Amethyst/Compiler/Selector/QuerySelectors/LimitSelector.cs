using Amethyst.Model;

namespace Amethyst;

/// <inheritdoc cref="AbstractQuerySelector" />
/// <summary>
///     <p>Parses the limit selector query. The limit query has the special property that it plays a
///     major factor in determining whether a selector is matching only a single target. During
///     transformation of the final query result, the limit expression is set to the passed value.</p>
///     <p><inheritdoc cref="AbstractQuerySelector" /></p></summary>
public class LimitSelector : NumericSelector
{
    /// <summary>The value's expression used in the selector query.</summary>
    private AbstractValue _limitExpression = null!;
    
    /// <summary>Creates a new instance of a <see cref="LimitSelector" />.</summary>
    public LimitSelector() : base(allowDecimals: false, minValue: 1)
    {
    }

    public override SelectorQueryResult Parse(string queryKey, bool isNegated, AbstractValue value)
    {
        _limitExpression = value;
        return base.Parse(queryKey, isNegated, value);
    }

    public override SelectorQueryResult Transform(string queryKey, SelectorQueryValue[] selectorQueryValues)
    {
        var selectorQueryResult = base.Transform(queryKey, selectorQueryValues);
        selectorQueryResult.LimitExpression = _limitExpression;
        return selectorQueryResult;
    }
}