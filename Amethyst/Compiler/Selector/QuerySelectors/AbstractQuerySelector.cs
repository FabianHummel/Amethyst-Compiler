using Amethyst.Model;
using Antlr4.Runtime;

namespace Amethyst;

public abstract class AbstractQuerySelector<T> : AbstractQuerySelector
{
    public abstract SelectorQueryResult Parse(string queryKey, bool isNegated, T value);
}

public abstract class AbstractQuerySelector
{
    public virtual SelectorQueryResult Transform(string queryKey, SelectorQueryValue[] selectorQueryValues)
    {
        return new SelectorQueryResult(queryKey, selectorQueryValues.ToArray());
    }
}