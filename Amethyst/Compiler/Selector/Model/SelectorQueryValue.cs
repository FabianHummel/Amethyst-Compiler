using Antlr4.Runtime;

namespace Amethyst.Model;

public class SelectorQueryValue
{
    public string QueryValue { get; }
    public bool IsNegated { get; }
    public ParserRuleContext Context { get; }
    public bool IsRuntimeValue { get; set; }
    public bool AlreadyContainsQueryKey { get; }

    public SelectorQueryValue(string queryValue, bool isNegated, ParserRuleContext context, bool isRuntimeValue, bool alreadyContainsQueryKey = false)
    {
        QueryValue = queryValue;
        IsNegated = isNegated;
        Context = context;
        IsRuntimeValue = isRuntimeValue;
        AlreadyContainsQueryKey = alreadyContainsQueryKey;
    }
    
    public string ToTargetSelectorString(string queryKey)
    {
        if (AlreadyContainsQueryKey)
        {
            return QueryValue;
        }
        
        var negation = IsNegated ? "!" : "";

        return $"{queryKey}={negation}{QueryValue}";
    }
}