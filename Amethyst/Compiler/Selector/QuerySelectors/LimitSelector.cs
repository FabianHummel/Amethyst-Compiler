using Amethyst.Model;

namespace Amethyst;

public class LimitSelector : NumericSelector
{
    private AbstractValue _limitExpression = null!;
    
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