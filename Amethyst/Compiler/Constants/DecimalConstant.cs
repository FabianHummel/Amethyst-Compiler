using Amethyst.Model;

namespace Amethyst;

public class DecimalConstant : ConstantValue<double>, INumericConstant
{
    public required int DecimalPlaces { get; init; }
    
    public double AsDouble => Value;

    public override int AsInteger => (int)AsDouble;
    
    public override bool AsBoolean => AsDouble != 0;
    
    public int ScoreboardValue => (int)Math.Round(Value * DataType.Scale);
    
    public override DecimalDataType DataType => new()
    {
        BasicType = BasicType.Dec,
        Modifier = null,
        DecimalPlaces = DecimalPlaces
    };

    public override RuntimeValue ToRuntimeValue()
    {
        var location = ++Compiler.StackPointer;
        
        Compiler.AddCode($"scoreboard players set {location} amethyst {ScoreboardValue}");
        
        return new DecimalResult
        {
            DecimalPlaces = DecimalPlaces, 
            Compiler = Compiler,
            Context = Context,
            Location = location,
            IsTemporary = true,
        };
    }

    public override string ToNbtString()
    {
        return $"{AsDouble.ToString("F" + DecimalPlaces)}d";
    }

    public override string ToTextComponent()
    {
        return $$"""[{"text":"{{Value.ToString("F" + DecimalPlaces)}}","color":"gold"},{"text":"d","color":"red"}]""";
    }

    public override bool Equals(ConstantValue? other)
    {
        if (other is not DecimalConstant decimalConstant)
        {
            return false;
        }

        return Value.Equals(decimalConstant.Value);
    }

    public override string ToTargetSelectorString()
    {
        return $"{Value}";
    }
}