using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

/// <inheritdoc />
/// <summary>
///     <p>Parses any basic selector value that is representable by a <see cref="AbstractValue" />.</p>
///     <p><inheritdoc /></p></summary>
public class BasicSelector : AbstractQuerySelector<AbstractValue>
{
    /// <summary>Whether values with the same query key can be used multiple times for equality checks.</summary>
    protected bool AllowMultipleEqualityChecks { get; }

    /// <summary>The basic type that is allowed for the <see cref="AbstractValue" />.</summary>
    public BasicType BasicType { get; }

    /// <summary>Creates a new instance of a <see cref="BasicSelector" />.</summary>
    /// <param name="allowMultipleEqualityChecks">Whether values with the same query key can be used
    /// multiple times for equality checks.</param>
    /// <param name="basicType">The basic type that is allowed for the query value.</param>
    public BasicSelector(bool allowMultipleEqualityChecks, BasicType basicType)
    {
        AllowMultipleEqualityChecks = allowMultipleEqualityChecks;
        BasicType = basicType;
    }
    
    public override SelectorQueryResult Parse(string queryKey, bool isNegated, AbstractValue value)
    {
        if (value.Datatype.Modifier != null || value.Datatype.BasicType != BasicType)
        {
            throw new SyntaxException($"Expected value of type '{BasicType.GetDescription()}' for '{queryKey}' target selector query, but got '{value.Datatype}'.", value.Context);
        }

        var queryValue = new SelectorQueryValue(value.ToTargetSelectorString(), isNegated, value.Context, value is IRuntimeValue);
        return new SelectorQueryResult(queryKey, queryValue);
    }

    public override SelectorQueryResult Transform(string queryKey, SelectorQueryValue[] selectorQueryValues)
    {
        if (!AllowMultipleEqualityChecks && selectorQueryValues.Count(v => !v.IsNegated) > 1)
        {
            var excessQueryValue = selectorQueryValues[1];
            throw new SyntaxException($"Cannot compare multiple values for equality in '{queryKey}' target selector query", excessQueryValue.Context);
        }
        
        if (selectorQueryValues.Any(v => !v.IsNegated) && selectorQueryValues.FirstOrDefault(v => v.IsNegated) is { } negatedValue)
        {
            throw new SyntaxException("Cannot compare query value for inequality, if already comparing for equality.", negatedValue.Context);
        }
        
        return base.Transform(queryKey, selectorQueryValues);
    }
}