namespace Amethyst;

public class ConstantDecimal : AbstractDecimal, IConstantValue<double>, IScoreboardValue
{
    public required double Value { get; init; }

    public int AsInteger => (int)Value;
    
    public bool AsBoolean => Value != 0;
    
    protected override AbstractDecimal AsDecimal => this;
    
    public int ScoreboardValue => (int)Math.Round(Value * DataType.Scale);

    public IRuntimeValue ToRuntimeValue()
    {
        var location = ++Compiler.StackPointer;
        
        Compiler.AddCode($"scoreboard players set {location} amethyst {ScoreboardValue}");
        
        return new RuntimeDecimal
        {
            DecimalPlaces = DecimalPlaces, 
            Compiler = Compiler,
            Context = Context,
            Location = location,
            IsTemporary = true,
        };
    }

    public string ToNbtString()
    {
        return $"{Value.ToString("F" + DecimalPlaces)}d";
    }

    public string ToTextComponent()
    {
        return $$"""[{"text":"{{Value.ToString("F" + DecimalPlaces)}}","color":"gold"},{"text":"d","color":"red"}]""";
    }

    public bool Equals(IConstantValue? other)
    {
        if (other is not ConstantDecimal decimalConstant)
        {
            return false;
        }

        return Value.Equals(decimalConstant.Value);
    }

    public override AbstractString ToStringValue()
    {
        throw new NotImplementedException();
    }

    public override string ToTargetSelectorString()
    {
        return $"{Value}";
    }
}