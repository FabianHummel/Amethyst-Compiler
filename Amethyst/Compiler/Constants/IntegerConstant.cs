using Amethyst.Model;

namespace Amethyst;

public class IntegerConstant : ConstantValue<int>, INumericConstant
{
    public override int AsInteger => Value;
    
    public override bool AsBoolean => AsInteger != 0;
    
    public int ScoreboardValue => Value;
    
    public override DataType DataType => new()
    {
        BasicType = BasicType.Int,
        Modifier = null
    };

    public override RuntimeValue ToRuntimeValue()
    {
        var location = ++Compiler.StackPointer;
        
        Compiler.AddCode($"scoreboard players set {location} amethyst {Value}");
        
        return new IntegerResult
        {
            Compiler = Compiler,
            Context = Context,
            Location = location,
            IsTemporary = true
        };
    }

    public override string ToNbtString()
    {
        return Value.ToString();
    }

    public override string ToTextComponent()
    {
        return $$"""{"text":"{{Value}}","color":"gold"}""";
    }

    public override bool Equals(ConstantValue? other)
    {
        if (other is not IntegerConstant integerConstant)
        {
            return false;
        }

        return Value.Equals(integerConstant.Value);
    }
}