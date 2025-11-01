using Amethyst.Model;

namespace Amethyst;

public class LiteralSelector : BasicSelector
{
    public string[] Values { get; }
    
    public LiteralSelector(string[] values) : base(allowMultipleEqualityChecks: true, BasicType.String)
    {
        Values = values;
    }

    public override SelectorQueryResult Parse(string queryKey, AbstractValue value)
    {
        if (value is ConstantString constantString && !Values.Contains(constantString.Value))
        {
            var allowedValues = string.Join(", ", Values.Select(v => $"'{v}'"));
            throw new SyntaxException($"Value for '{queryKey}' target selector query must be one of {allowedValues}", value.Context);
        }
        
        return base.Parse(queryKey, value);
    }
}