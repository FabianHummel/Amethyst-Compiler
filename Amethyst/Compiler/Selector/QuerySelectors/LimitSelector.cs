using Amethyst.Model;

namespace Amethyst;

public class LimitSelector : NumericSelector
{
    public LimitSelector() : base(allowDecimals: false, minValue: 1)
    {
    }

    public override SelectorQueryResult Parse(string queryKey, AbstractValue value)
    {
        var result = base.Parse(queryKey, value);
        result.LimitExpression = value;
        return result;
    }
}