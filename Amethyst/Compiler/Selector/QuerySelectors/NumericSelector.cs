using Amethyst.Model;

namespace Amethyst;

public class NumericSelector : AbstractQuerySelector<AbstractValue>
{
    public bool AllowDecimals { get; }
    public double? MinValue { get; }
    public double? MaxValue { get; }
    
    public NumericSelector(bool allowDecimals, double? minValue = null, double? maxValue = null)
    {
        AllowDecimals = allowDecimals;
        MinValue = minValue;
        MaxValue = maxValue;
    }
    
    public override SelectorQueryResult Parse(string queryKey, AbstractValue value)
    {
        CheckValue(value, queryKey);

        var result = value.ToTargetSelectorString();

        return new SelectorQueryResult(queryKey, $"{queryKey}={result}", value is IRuntimeValue);
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