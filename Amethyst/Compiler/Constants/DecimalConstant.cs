using Amethyst.Model;

namespace Amethyst;

public class DecimalConstant : ConstantValue<double>
{
    public required int DecimalPlaces { get; init; }
    
    public double AsDouble => Value;

    public override int AsInteger => (int)AsDouble;
    
    public override bool AsBoolean => AsDouble != 0;
    
    public override DecimalDataType DataType => new()
    {
        BasicType = BasicType.Dec,
        Modifier = null,
        DecimalPlaces = DecimalPlaces
    };

    public override RuntimeValue ToRuntimeValue()
    {
        var location = ++Compiler.StackPointer;
        
        var value = (int)Math.Round(Value * DataType.Scale);
        
        Compiler.AddCode($"scoreboard players set {location} amethyst {value}");
        
        return new DecimalResult
        {
            DecimalPlaces = DecimalPlaces, 
            Compiler = Compiler,
            Context = Context,
            Location = location.ToString(),
            IsTemporary = true,
        };
    }

    public override string ToNbtString()
    {
        return $"{AsDouble.ToString("F" + DecimalPlaces)}d";
    }
}