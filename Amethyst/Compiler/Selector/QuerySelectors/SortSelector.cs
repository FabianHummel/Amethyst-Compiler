using Amethyst.Model;

namespace Amethyst;

/// <inheritdoc cref="AbstractQuerySelector" />
/// <summary>
///     <p>Parses the sort selector query. It has special logic how it may be used in the target
///     selector.</p>
///     <p><inheritdoc cref="AbstractQuerySelector" /></p></summary>
public class SortSelector : LiteralSelector
{
    /// <summary>Creates a new instance of a <see cref="SortSelector" />.</summary>
    /// <param name="values">The allowed values.</param>
    public SortSelector(string[] values) : base(values)
    {
    }

    public override SelectorQueryResult Transform(string queryKey, SelectorQueryValue[] selectorQueryValues)
    {
        var equalityChecks = selectorQueryValues.Where(v => !v.IsNegated).ToArray();
        
        if (equalityChecks.Length > 1)
        {
            throw new SyntaxException($"Target selector query '{queryKey}' does not permit multiple equality checks.", equalityChecks[1].Context);
        }
        
        if (selectorQueryValues.FirstOrDefault(v => v.IsNegated) is { } negatedValue)
        {
            throw new SyntaxException($"Target selector query '{queryKey}' must not compare for inequality.", negatedValue.Context);
        }
        
        return base.Transform(queryKey, selectorQueryValues);
    }
}