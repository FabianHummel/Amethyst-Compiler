using Amethyst.Model;

namespace Amethyst;

/// <inheritdoc cref="AbstractQuerySelector" />
/// <summary>
///     <p>Parses a literal query value - a value that must be included in the valid range of literals.
///     Normally, Minecraft does not support multiple equality checks that <i>could</i> be interpreted
///     as if the value matches <b>ANY</b> of the supplied values, but only a collection of inequality
///     checks that the value <b>MUST NOT</b> be.</p>
///     <p>Amethyst fixes bridges this gap by transforming multiple equality checks to their inequality
///     counterpart.</p>
///     <p><inheritdoc cref="AbstractQuerySelector" /></p></summary>
/// <example>
///     <p><c>@a[gamemodes=["creative", "spectator"]]</c> →
///     <c>@a[gamemode=!survival,gamemode=!adventure]</c></p>
///     <p><c>@a[gamemode="adventure"]</c> → <c>@a[gamemode=adventure]</c></p>
///     <p><c>@a[gamemode=!"survival"]</c> → <c>@a[gamemode=!survival]</c></p></example>
public class LiteralSelector : BasicSelector
{
    /// <summary>The allowed values of the selector query.</summary>
    public string[] Values { get; }

    /// <summary>Creates a new instance of a <see cref="LiteralSelector" /></summary>
    /// <param name="values">The allowed values.</param>
    public LiteralSelector(string[] values) : base(allowMultipleEqualityChecks: false, BasicType.String)
    {
        Values = values;
    }

    public override SelectorQueryResult Parse(string queryKey, bool isNegated, AbstractValue value)
    {
        if (value is ConstantString constantString && !Values.Contains(constantString.Value))
        {
            var allowedValues = string.Join(", ", Values.Select(v => $"'{v}'"));
            throw new SyntaxException($"Value for '{queryKey}' target selector query must be one of {allowedValues}", value.Context);
        }
        
        return base.Parse(queryKey, isNegated, value);
    }
    
    public override SelectorQueryResult Transform(string queryKey, SelectorQueryValue[] selectorQueryValues)
    {
        if (AllowMultipleEqualityChecks)
        {
            return base.Transform(queryKey, selectorQueryValues);
        }
        
        var equalityChecks = selectorQueryValues.Where(v => !v.IsNegated).ToArray();
        
        if (equalityChecks.Length <= 1)
        {
            return base.Transform(queryKey, selectorQueryValues);
        }

        var negatedValues = Values.Except(equalityChecks.Select(v => v.QueryValue)).Select(v =>
        {
            return new SelectorQueryValue(v, isNegated: true, context: null!, isRuntimeValue: false);
        });

        return new SelectorQueryResult(queryKey, negatedValues.ToArray());
    }
}