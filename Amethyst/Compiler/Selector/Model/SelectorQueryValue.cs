using Antlr4.Runtime;

namespace Amethyst.Model;

/// <summary>Represents a single value within a selector query.</summary>
public class SelectorQueryValue
{
    /// <summary>The query value.</summary>
    public string QueryValue { get; }

    /// <summary>Whether this value is negated.</summary>
    public bool IsNegated { get; }

    /// <summary>The parser rule context used for error handling.</summary>
    public ParserRuleContext Context { get; }

    /// <summary>Whether the actual underlying value is only known during runtime. This is important to
    /// determine whether the selector needs macro substitution.</summary>
    public bool IsRuntimeValue { get; set; }

    /// <summary>Whether the <see cref="QueryValue" /> already contains the query key. This is a little
    /// weird, but helps keeping things simple and organized.</summary>
    public bool AlreadyContainsQueryKey { get; }

    /// <summary>Creates a new instance of a <see cref="SelectorQueryValue" />.</summary>
    /// <param name="queryValue">The query value"</param>
    /// <param name="isNegated">Whether this value is negated.</param>
    /// <param name="context">The parser rule context used for error handling.</param>
    /// <param name="isRuntimeValue">Whether the value is a runtime value.</param>
    /// <param name="alreadyContainsQueryKey">Whether the <paramref name="queryValue" /> already contains
    /// the query key.</param>
    public SelectorQueryValue(string queryValue, bool isNegated, ParserRuleContext context, bool isRuntimeValue, bool alreadyContainsQueryKey = false)
    {
        QueryValue = queryValue;
        IsNegated = isNegated;
        Context = context;
        IsRuntimeValue = isRuntimeValue;
        AlreadyContainsQueryKey = alreadyContainsQueryKey;
    }

    /// <summary>Converts this query result to a string that is ready to be used in the target selector
    /// within the datapack code. If the query value already contains the respective query key (
    /// <see cref="AlreadyContainsQueryKey" />), then the value is immediately returned.</summary>
    /// <param name="queryKey">The query key that belongs to this query value.</param>
    /// <returns>MCFunction compliant code for this query result.</returns>
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