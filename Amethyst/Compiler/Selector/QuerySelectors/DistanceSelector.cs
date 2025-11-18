using Amethyst.Model;

namespace Amethyst;

/// <inheritdoc cref="AbstractQuerySelector" />
/// <summary>
///     <p>Parses the distance selector query. A distance query has the special property that the lower
///     bound of the supplied range must be greater or equal to zero, so the bound is overwritten in
///     the <see cref="Compiler.RangeExpressionResult" />.</p>
///     <p><inheritdoc cref="AbstractQuerySelector" /></p></summary>
public class DistanceSelector : RangeSelector
{
    /// <summary>Creates a new instance of a <see cref="DistanceSelector" />.</summary>
    public DistanceSelector() : base(allowDecimals: true, minValue: 0)
    {
    }

    public override SelectorQueryResult Parse(string queryKey, bool isNegated, Compiler.RangeExpressionResult value)
    {
        if (value.Start == null)
        {
            value.OverwrittenStartValue = "0";
        }
        
        return base.Parse(queryKey, isNegated, value);
    }
}