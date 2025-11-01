namespace Amethyst.Model;

public class SelectorQueryResult : SelectorQueryResultBase
{
    public string[] Values { get; }

    public SelectorQueryResult(string queryKey, string value, bool containsRuntimeValues, AbstractValue? limitExpression = null)
        : base(queryKey, $"{queryKey}={value}", containsRuntimeValues, limitExpression)
    {
        Values = [value];
    }
    
    public SelectorQueryResult(string queryKey, string[] values, bool containsRuntimeValues, AbstractValue? limitExpression = null)
        : base(queryKey, string.Join(',', values.Select(v => $"{queryKey}={v}")), containsRuntimeValues, limitExpression)
    {
        Values = values;
    }
}