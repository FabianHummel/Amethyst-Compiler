namespace Amethyst;

public class ConstantInteger : AbstractInteger, IConstantValue<int>
{
    public override AbstractString ToStringValue()
    {
        return new ConstantString
        {
            Compiler = Compiler,
            Context = Context,
            Value = Value.ToString()
        };
    }
    
    public int Value { get; init; }
    
    public int AsInteger => Value;
    
    public bool AsBoolean => AsInteger != 0;
    
    public int ScoreboardValue => Value;

    public IRuntimeValue ToRuntimeValue()
    {
        var location = ++Compiler.StackPointer;
        
        Compiler.AddCode($"scoreboard players set {location} amethyst {Value}");
        
        return new RuntimeInteger
        {
            Compiler = Compiler,
            Context = Context,
            Location = location,
            IsTemporary = true
        };
    }

    public string ToNbtString()
    {
        return Value.ToString();
    }

    public string ToTextComponent()
    {
        return $$"""{"text":"{{Value}}","color":"gold"}""";
    }

    public bool Equals(IConstantValue? other)
    {
        if (other is not ConstantInteger integerConstant)
        {
            return false;
        }

        return Value.Equals(integerConstant.Value);
    }

    public override string ToTargetSelectorString()
    {
        return $"{Value}";
    }
}