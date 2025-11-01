using Amethyst.Model;

namespace Amethyst;

public class MultiSelector : AbstractQuerySelector<AbstractValue>
{
    public string OriginalQueryKey { get; }
    
    public MultiSelector(string originalQueryKey)
    {
        OriginalQueryKey = originalQueryKey;
    }
    
    public override SelectorQueryResultBase Parse(string queryKey, AbstractValue value)
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
            var values = arrayConstant.Value.SelectMany(constantValue =>
            {
                var queryResult = basicSelector.Parse(OriginalQueryKey, (AbstractValue)constantValue);
                return queryResult.Values;
            });
            
            var containsRuntimeValues = arrayConstant.Value.Any(constantValue => constantValue is ConstantSubstitute);
            
            return new SelectorQueryResult(OriginalQueryKey, values.ToArray(), containsRuntimeValues);
        }

        if (arrayValue is RuntimeStaticArray and IRuntimeValue arrayResult)
        {
            var location = arrayResult.NextFreeLocation(DataLocation.Storage);
            
            arrayValue.AddCode($"data modify storage amethyst:internal data.prefix.prefix set value \"{OriginalQueryKey}=\"");
            arrayValue.AddCode($"data modify storage amethyst:internal data.prefix.in set from storage {arrayResult.Location}");
            arrayValue.AddCode("function amethyst:api/data/prefix");
            
            return new SelectorQueryResultBase(OriginalQueryKey, $"$({location.Name})", containsRuntimeValues: true);
        }

        throw new InvalidOperationException($"Unexpected query selector type '{arrayValue}' in {nameof(MultiSelector)}.");
    }
}