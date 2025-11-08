using Amethyst.Model;

namespace Amethyst;

/// <inheritdoc />
/// <summary><p>Parses a numeric value.</p> <p><inheritdoc /></p></summary>
public class NumericSelector : AbstractQuerySelector<AbstractValue>
{
    /// <summary>Whether to allow decimals to be used as a query value.</summary>
    public bool AllowDecimals { get; }
    
    /// <summary>The lowest allowed value.</summary>
    public double? MinValue { get; }
    
    /// <summary>The highest allowed value.</summary>
    public double? MaxValue { get; }

    /// <summary>Creates a new instance of a <see cref="NumericSelector" />.</summary>
    /// <param name="allowDecimals">Whether to allow decimals.</param>
    /// <param name="minValue">The minimum allowed value.</param>
    /// <param name="maxValue">The maximum allowed value.</param>
    public NumericSelector(bool allowDecimals, double? minValue = null, double? maxValue = null)
    {
        AllowDecimals = allowDecimals;
        MinValue = minValue;
        MaxValue = maxValue;
    }
    
    public override SelectorQueryResult Parse(string queryKey, bool isNegated, AbstractValue value)
    {
        CheckValue(value, queryKey);

        var queryValue = new SelectorQueryValue(value.ToTargetSelectorString(), isNegated, value.Context, value is IRuntimeValue);
        return new SelectorQueryResult(queryKey, queryValue);
    }
    
    public void CheckValue(AbstractValue value, string queryKey)
    {
        if (value is not AbstractNumericValue numericValue)
        {
            throw new SyntaxException($"Expected numeric value for '{queryKey}' target selector query.", value.Context);
        }
        
        if (!AllowDecimals && numericValue is AbstractDecimal)
        {
            throw new SyntaxException($"Unexpected decimal value for '{queryKey}' target selector query.", value.Context);
        }

        if (value is IConstantValue constantValue && (constantValue.AsDouble < MinValue || constantValue.AsDouble > MaxValue))
        {
            if (MinValue.HasValue && MaxValue.HasValue)
            {
                throw new SyntaxException($"Value for '{queryKey}' target selector query must be between {MinValue} and {MaxValue}.", value.Context);
            }

            if (MinValue.HasValue)
            {
                throw new SyntaxException($"Value for '{queryKey}' target selector query must be at least {MinValue}.", value.Context);
            }

            if (MaxValue.HasValue)
            {
                throw new SyntaxException($"Value for '{queryKey}' target selector query must be at most {MaxValue}.", value.Context);
            }
        }
    }
}