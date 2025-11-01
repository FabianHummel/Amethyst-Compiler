namespace Amethyst.Model;

public class SelectorQueryResultBase
{
    public string QueryKey { get; }
    public string QueryValue { get; }
    public bool ContainsRuntimeValues { get; }
    public AbstractValue? LimitExpression { get; set; }

    public SelectorQueryResultBase(string queryKey, string queryValue, bool containsRuntimeValues, AbstractValue? limitExpression = null)
    {
        QueryKey = queryKey;
        QueryValue = queryValue;
        ContainsRuntimeValues = containsRuntimeValues;
        LimitExpression = limitExpression;
    }

    public override string ToString()
    {
        return QueryValue;
    }
}