using Amethyst.Model;

namespace Amethyst;

public class DistanceSelector : RangeSelector
{
    public DistanceSelector() : base(allowDecimals: true, minValue: 0)
    {
    }

    public override SelectorQueryResult Parse(string queryKey, Compiler.RangeExpressionResult value)
    {
        if (value.Start == null)
        {
            value.OverwrittenStartValue = "0";
        }
        
        return base.Parse(queryKey, value);
    }
}