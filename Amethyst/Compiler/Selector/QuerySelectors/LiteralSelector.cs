using Amethyst.Model;

namespace Amethyst;

public class LiteralSelector : BasicSelector
{
    public string[] Values { get; }
    
    public LiteralSelector(string[] values) : base(allowMultipleEqualityChecks: false, BasicType.String)
    {
        Values = values;
    }

    public override SelectorQueryResult Parse(string queryKey, bool isNegated, AbstractValue value)
    {
        if (value is ConstantString constantString && !Values.Contains(constantString.Value))
        {
            var allowedValues = string.Join(", ", Values.Select(v => $"'{v}'"));
            throw new SyntaxException($"Value for '{queryKey}' target selector query must be one of {allowedValues}", value.Context);
        }
        
        return base.Parse(queryKey, isNegated, value);
    }
    
    public override SelectorQueryResult Transform(string queryKey, SelectorQueryValue[] selectorQueryValues)
    {
        if (AllowMultipleEqualityChecks)
        {
            return base.Transform(queryKey, selectorQueryValues);
        }
        
        var equalityChecks = selectorQueryValues.Where(v => !v.IsNegated).ToArray();
        
        if (equalityChecks.Length <= 1)
        {
            return base.Transform(queryKey, selectorQueryValues);
        }

        var negatedValues = Values.Except(equalityChecks.Select(v => v.QueryValue)).Select(v =>
        {
            return new SelectorQueryValue(v, isNegated: true, context: null!, isRuntimeValue: false);
        });

        return new SelectorQueryResult(queryKey, negatedValues.ToArray());
    }
}