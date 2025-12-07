using Amethyst.Model;

namespace Amethyst;

/// <inheritdoc />
/// <summary>
///     <p>Parses a collection of values for a plural selector query. This is usually not supported in
///     Minecraft, but it is very helpful in Amethyst to heavily increase the ergonomics of the
///     language.</p>
///     <p><inheritdoc /></p></summary>
/// <example><p><c>@a[tags=["abc", "xyz"]]</c> → <c>@a[tag=abc,tag=xyz]</c></p></example>
public class MultiSelector : AbstractQuerySelector<AbstractValue>
{
    /// <summary>The singular version of the query key.</summary>
    /// <example><p><c>tags</c> → <c>tag</c></p> <p><c>gamemodes</c> → <c>gamemode</c></p></example>
    public string OriginalQueryKey { get; }

    /// <summary>Creates a new instance of a <see cref="MultiSelector" />.</summary>
    /// <param name="originalQueryKey">The singular version of the plural query key.</param>
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
            var queryValues = arrayConstant.Value.Select(constantValue =>
            {
                var selectorQueryValue = basicSelector.Parse(OriginalQueryKey, isNegated, (AbstractValue)constantValue).QueryValues.First();
                selectorQueryValue.IsRuntimeValue = constantValue is ConstantSubstitute;
                return selectorQueryValue;
            });
            
            return new SelectorQueryResult(OriginalQueryKey, queryValues.ToArray());
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