namespace Amethyst.Model;

public class SelectorQueryResult
{
    public string QueryKey { get; }
    public SelectorQueryValue[] QueryValues { get; }
    public bool ContainsRuntimeValues { get; }
    public AbstractValue? LimitExpression { get; set; }

    public SelectorQueryResult(string queryKey, SelectorQueryValue queryValue)
        : this(queryKey, [queryValue])
    {
    }
    
    public SelectorQueryResult(string queryKey, IEnumerable<SelectorQueryValue> queryValues)
    {
        QueryKey = queryKey;
        QueryValues = queryValues.ToArray();
        ContainsRuntimeValues = queryValues.Any(val => val.IsRuntimeValue);
    }

    public string ToTargetSelectorString()
    {
        return string.Join(",", QueryValues.Select(val => val.ToTargetSelectorString(QueryKey)));
    }
}