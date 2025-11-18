using Amethyst.Model;

namespace Amethyst;

/// <inheritdoc cref="AbstractQuerySelector" />
/// <typeparam name="T">The type of value the query selector is able to process.</typeparam>
public abstract class AbstractQuerySelector<T> : AbstractQuerySelector
{
    /// <summary>Parses and processes a selector query into a full <see cref="SelectorQueryResult" />.</summary>
    /// <param name="queryKey">The query key. This may also be a plural version that needs extra
    /// processing. See <see cref="MultiSelector" />.</param>
    /// <param name="isNegated">Whether the query is negated.</param>
    /// <param name="value">The Amethyst value that was used in the selector query.</param>
    /// <returns>The processed query result ready to be used in datapack code.</returns>
    public abstract SelectorQueryResult Parse(string queryKey, bool isNegated, T value);
}

/// <summary>A query selector parses a selector query and processes it until it results in a finished
/// output that is ready to be used in the resulting datapack code.</summary>
public abstract class AbstractQuerySelector
{
    /// <summary>Transforms a collection of <paramref name="selectorQueryValues" /> together with their
    /// respective <paramref name="queryKey" /> into a different <see cref="SelectorQueryResult" />. This
    /// allows making changes after all selector queries have been parsed to further optimize or validate
    /// the target selector.</summary>
    /// <param name="queryKey">The query key. This is not the plural version of query selectors.</param>
    /// <param name="selectorQueryValues">The query values that were used with the
    /// <paramref name="queryKey" />.</param>
    /// <returns>The final query result after any modifications.</returns>
    public virtual SelectorQueryResult Transform(string queryKey, SelectorQueryValue[] selectorQueryValues)
    {
        return new SelectorQueryResult(queryKey, selectorQueryValues.ToArray());
    }
}