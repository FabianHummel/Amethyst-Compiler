namespace Amethyst.Model;

public class SelectorQueryResult
{
    public string QueryKey { get; }
    public string QueryString { get; }
    public bool ContainsRuntimeValues { get; }
    public AbstractValue? LimitExpression { get; }

    public SelectorQueryResult(string queryKey, string queryString, bool containsRuntimeValues, AbstractValue? limitExpression = null)
    {
        QueryKey = queryKey;
        QueryString = queryString;
        ContainsRuntimeValues = containsRuntimeValues;
        LimitExpression = limitExpression;
    }

    public override string ToString()
    {
        return QueryString;
    }
}