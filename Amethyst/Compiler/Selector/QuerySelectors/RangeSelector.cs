using Amethyst.Model;

namespace Amethyst;

/// <inheritdoc />
/// <summary><p>Parses a ranged selector query.</p> <p><inheritdoc /></p></summary>
public class RangeSelector : AbstractQuerySelector<Compiler.RangeExpressionResult>
{
    /// <summary>Whether to allow decimals in the supplied numeric range.</summary>
    public bool AllowDecimals { get; }
    
    /// <summary>The lowest allowed value in the lower bound.</summary>
    public double? MinValue { get; }
    
    /// <summary>The highest allowed value in the upper bound.</summary>
    public double? MaxValue { get; }

    /// <summary>A reference to the underlying numeric selector that helps validate the individual bounds
    /// of the range. Simply helps reducing duplicated code.</summary>
    private readonly NumericSelector _numericSelector;
    
    /// <summary>Creates a new instance of a <see cref="RangeSelector" />.</summary>
    /// <param name="allowDecimals">Whether to allow decimals.</param>
    /// <param name="minValue">The minimum allowed value in the lower bound.</param>
    /// <param name="maxValue">The maximum allowed value in the upper bound.</param>
    public RangeSelector(bool allowDecimals, double? minValue = null, double? maxValue = null)
    {
        AllowDecimals = allowDecimals;
        MinValue = minValue;
        MaxValue = maxValue;
        
        _numericSelector = new NumericSelector(allowDecimals, minValue, maxValue);
    }
    
    public override SelectorQueryResult Parse(string queryKey, bool isNegated, Compiler.RangeExpressionResult value)
    {
        if (MinValue.HasValue && value.Start == null && value.OverwrittenStartValue == null)
        {
            throw new SyntaxException($"Range for '{queryKey}' target selector query requires a lower bound of at least {MinValue.Value}.", value.Context);
        }
        
        if (MaxValue.HasValue && value.Stop == null)
        {
            throw new SyntaxException($"Range for '{queryKey}' target selector query requires an upper bound of at most {MaxValue.Value}.", value.Context);
        }
        
        if (value.Start is { } start)
        {
            _numericSelector.CheckValue(start, queryKey);
        }
        
        if (value.Stop is { } stop)
        {
            _numericSelector.CheckValue(stop, queryKey);
        }

        var queryValue = new SelectorQueryValue(value.ToTargetSelectorString(), isNegated, value.Context, value.ContainsRuntimeValues);
        return new SelectorQueryResult(queryKey, queryValue);
    }
}