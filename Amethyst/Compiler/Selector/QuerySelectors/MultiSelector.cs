using Amethyst.Model;

namespace Amethyst;

public class MultiSelector : AbstractQuerySelector<AbstractValue>
{
    public string OriginalQueryKey { get; }
    
    public MultiSelector(string originalQueryKey)
    {
        OriginalQueryKey = originalQueryKey;
    }
    
    public override SelectorQueryResult Parse(string queryKey, bool isNegated, AbstractValue value)
    {
        if (!Constants.TargetSelectorQueryKeys.TryGetValue(OriginalQueryKey, out var selector) || selector is not BasicSelector basicSelector)
        {
            throw new InvalidOperationException($"Could not find matching basic selector query '{OriginalQueryKey}' for '{queryKey}'.");
        }
        
        if (value is not AbstractArray arrayValue || arrayValue.Datatype.BasicType != basicSelector.BasicType)
        {
            throw new SyntaxException($"Expected '{basicSelector.BasicType}' array for '{queryKey}' target selector query, but got '{value.Datatype}'.", value.Context);
        }
        
        if (arrayValue is ConstantStaticArray arrayConstant)
        {
            return new SelectorQueryResult(OriginalQueryKey, arrayConstant.Value.Select(constantValue =>
            {
                var selectorQueryValue = basicSelector.Parse(OriginalQueryKey, isNegated, (AbstractValue)constantValue).QueryValues.First();
                selectorQueryValue.IsRuntimeValue = constantValue is ConstantSubstitute;
                return selectorQueryValue;
            }));
        }

        if (arrayValue is RuntimeStaticArray and IRuntimeValue arrayResult)
        {
            var location = arrayResult.NextFreeLocation(DataLocation.Storage);

            var negation = isNegated ? "!" : "";
            arrayValue.AddCode($"data modify storage amethyst:internal data.prefix.prefix set value \"{OriginalQueryKey}={negation}\"");
            arrayValue.AddCode($"data modify storage amethyst:internal data.prefix.in set from storage {arrayResult.Location}");
            arrayValue.AddCode("function amethyst:api/data/prefix");

            var queryValue = new SelectorQueryValue($"$({location.Name})", isNegated, value.Context, isRuntimeValue: true, alreadyContainsQueryKey: true);
            return new SelectorQueryResult(OriginalQueryKey, queryValue);
        }
        
        throw new InvalidOperationException($"Unexpected query selector type '{arrayValue}' in {nameof(MultiSelector)}.");
    }
}