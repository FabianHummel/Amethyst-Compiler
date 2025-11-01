using Amethyst.Model;

namespace Amethyst;

public class MultiSelector : AbstractQuerySelector<AbstractValue>
{
    public string OriginalQueryKey { get; }
    
    public MultiSelector(string originalQueryKey)
    {
        OriginalQueryKey = originalQueryKey;
    }
    
    public override SelectorQueryResult Parse(string queryKey, AbstractValue value)
    {
        if (!Constants.TargetSelectorQueryKeys.TryGetValue(OriginalQueryKey, out var selector) || selector is not BasicSelector basicSelector)
        {
            throw new InvalidOperationException($"Could not find matching basic selector query '{OriginalQueryKey}' for '{queryKey}'.");
        }
        
        if (value is not AbstractArray arrayValue || arrayValue.Datatype.BasicType != basicSelector.BasicType)
        {
            throw new SyntaxException($"Expected '{basicSelector.BasicType}' array for '{queryKey}' target selector query, but got '{value.Datatype}'.", value.Context);
        }

        var queryString = ParseInternal(arrayValue, out var containsRuntimeValues);

        return new SelectorQueryResult("tag", queryString, containsRuntimeValues);
    }

    private string ParseInternal(AbstractArray array, out bool containsRuntimeValues)
    {
        containsRuntimeValues = false;

        if (!Constants.TargetSelectorQueryKeys.TryGetValue(OriginalQueryKey, out var selector) || selector is not BasicSelector basicSelector)
        {
            throw new InvalidOperationException($"Could not find matching basic query selector for '{OriginalQueryKey}' query key.");
        }
        
        if (array is ConstantStaticArray arrayConstant)
        {
            if (arrayConstant.Value.Any(constantValue => constantValue is ConstantSubstitute))
            {
                containsRuntimeValues = true;
            }
            
            return string.Join(",", arrayConstant.Value.Select(constantValue =>
            {
                var queryResult = basicSelector.Parse(OriginalQueryKey, (AbstractValue)constantValue);
                return queryResult.QueryString;
            }));
        }

        if (array is RuntimeStaticArray and IRuntimeValue arrayResult)
        {
            containsRuntimeValues = true;
            
            var location = arrayResult.NextFreeLocation(DataLocation.Storage);
            
            array.AddCode($"data modify storage amethyst:internal data.prefix.prefix set value \"{OriginalQueryKey}=\"");
            array.AddCode($"data modify storage amethyst:internal data.prefix.in set from storage {arrayResult.Location}");
            array.AddCode("function amethyst:api/data/prefix");
            
            return $"$({location.Name})";
        }

        throw new InvalidOperationException($"Unexpected query selector type '{array}' in {nameof(MultiSelector)}.");
    }
}