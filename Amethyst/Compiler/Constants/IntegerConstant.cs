using Amethyst.Model;

namespace Amethyst;

public class IntegerConstant : ConstantValue<int>
{
    public override int AsInteger => Value;
    
    public override bool AsBoolean => AsInteger != 0;
    
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
            Location = location.ToString(),
            IsTemporary = true
        };
    }

    public override string ToNbtString()
    {
        return Value.ToString();
    }
}