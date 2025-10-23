using JetBrains.Annotations;

namespace Tests.Presets;

public class ArithmeticOperatorTestCase(string op, string name)
{
    [UsedImplicitly]
    public string Operator { get; } = op;

    [UsedImplicitly]
    public string Name { get; } = name;
        
    public override string ToString() => Name;
    
    public static readonly IEnumerable<ArithmeticOperatorTestCase> Preset = new[]
    {
        new ArithmeticOperatorTestCase("+", "addition"),
        new ArithmeticOperatorTestCase("-", "subtraction"),
        new ArithmeticOperatorTestCase("*", "multiplication"),
        new ArithmeticOperatorTestCase("/", "division"),
        new ArithmeticOperatorTestCase("%", "modulus")
    };
    
    public static readonly IEnumerable<ArithmeticOperatorTestCase> PresetWithoutDivision = new[]
    {
        new ArithmeticOperatorTestCase("+", "addition"),
        new ArithmeticOperatorTestCase("-", "subtraction"),
        new ArithmeticOperatorTestCase("*", "multiplication"),
        new ArithmeticOperatorTestCase("%", "modulus")
    };
}