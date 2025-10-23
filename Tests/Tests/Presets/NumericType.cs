using Amethyst;
using JetBrains.Annotations;

namespace Tests.Presets;

public class NumericTypeTestCase(AbstractDatatype type, string name)
{
    [UsedImplicitly]
    public AbstractDatatype Type { get; } = type;

    [UsedImplicitly]
    public string Name { get; } = name;
        
    public override string ToString() => Name;
    
    public static readonly IEnumerable<NumericTypeTestCase> DefaultPreset =
    [
        new(new IntegerDatatype(), "integer"),
        new(new DecimalDatatype(2), "decimal"),
        new(new BooleanDatatype(), "boolean")
    ];

    public static readonly IEnumerable<NumericTypeTestCase> AllPreset =
    [
        ..DefaultPreset,
        new(new DecimalDatatype(1), "decimal"),
        new(new DecimalDatatype(3), "decimal")
    ];
}