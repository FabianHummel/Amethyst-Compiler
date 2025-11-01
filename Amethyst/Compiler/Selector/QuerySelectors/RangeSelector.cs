using Amethyst.Model;

namespace Amethyst;

public class RangeSelector : AbstractQuerySelector<Compiler.RangeExpressionResult>
{
    public bool AllowDecimals { get; }
    public double? MinValue { get; }
    public double? MaxValue { get; }
    
    private readonly NumericSelector _numericSelector;
    
    public RangeSelector(bool allowDecimals, double? minValue = null, double? maxValue = null)
    {
        AllowDecimals = allowDecimals;
        MinValue = minValue;
        MaxValue = maxValue;
        
        _numericSelector = new NumericSelector(allowDecimals, minValue, maxValue);
    }
    
    public override SelectorQueryResult Parse(string queryKey, Compiler.RangeExpressionResult value)
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
        
        return new SelectorQueryResult(queryKey, $"{queryKey}={value.ToTargetSelectorString()}", value.ContainsRuntimeValues);
    }
}