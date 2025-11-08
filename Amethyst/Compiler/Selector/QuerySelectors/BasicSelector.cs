using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public class BasicSelector : AbstractQuerySelector<AbstractValue>
{
    public bool AllowMultipleEqualityChecks { get; } // TODO: Add 'SelectorCreationContext' that contains a list of already used query keys, and use that to determine if multiple equality checks are allowed-
                                                     //  also add a flag whether a query is inverted with '!' operator (update ANTLR grammar)
    public BasicType BasicType { get; }
    
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