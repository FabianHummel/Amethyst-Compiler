namespace Amethyst.Model;

/// <summary>A selector query result holds all query values specific to a unique key.</summary>
public class SelectorQueryResult
{
    /// <summary>The query key. A list of all vanilla keys are found in
    /// <see cref="Constants.TargetSelectorQueryKeys" />, although plural versions are already excluded
    /// here.</summary>
    public string QueryKey { get; }

    /// <summary>A collection of all values that were used with the <see cref="QueryKey" />.</summary>
    public SelectorQueryValue[] QueryValues { get; }

    /// <summary>Whether any of the query values is a runtime value. This is important to determine whether
    /// the selector needs macro substitution.</summary>
    public bool ContainsRuntimeValues { get; }

    /// <summary>Holds the value of the limit expression if it was specified in the query.</summary>
    public AbstractValue? LimitExpression { get; set; }

    /// <summary>Creates a new instance <see cref="SelectorQueryResult" /> with the specified
    /// <paramref name="queryKey" /> and <paramref name="queryValue" />. This constructor initializes the
    /// query result with only a single value.</summary>
    /// <param name="queryKey">The query key.</param>
    /// <param name="queryValue">The query value.</param>
    public SelectorQueryResult(string queryKey, SelectorQueryValue queryValue)
        : this(queryKey, [queryValue])
    {
    }

    /// <summary>Creates a new instance <see cref="SelectorQueryResult" /> with the specified
    /// <paramref name="queryKey" /> and <paramref name="queryValues" />.</summary>
    /// <param name="queryKey">The query key.</param>
    /// <param name="queryValues">The query values.</param>
    public SelectorQueryResult(string queryKey, SelectorQueryValue[] queryValues)
    {
        QueryKey = queryKey;
        QueryValues = queryValues.ToArray();
        ContainsRuntimeValues = queryValues.Any(val => val.IsRuntimeValue);
    }

    /// <summary>Converts this query result to a string that is ready to be used in the target selector
    /// within the datapack code.</summary>
    /// <returns>MCFunction compliant code for this query result.</returns>
    public string ToTargetSelectorString()
    {
        return string.Join(",", QueryValues.Select(val => val.ToTargetSelectorString(QueryKey)));
    }
}