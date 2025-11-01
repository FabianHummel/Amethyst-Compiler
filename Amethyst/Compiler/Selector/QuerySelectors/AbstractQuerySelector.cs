using Amethyst.Model;

namespace Amethyst;

public abstract class AbstractQuerySelector<T> : AbstractQuerySelector
{
    public abstract SelectorQueryResult Parse(string queryKey, T value);
}

public abstract class AbstractQuerySelector
{
}