using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public class BooleanConstant : ConstantValue<bool>
{
    public override int AsInteger => AsBoolean ? 1 : 0;
    
    public override bool AsBoolean => Value;

    public override DataType DataType => new()
    {
        BasicType = BasicType.Bool,
        Modifier = null
    };

    public override RuntimeValue ToRuntimeValue()
    {
        var location = ++Compiler.StackPointer;
        
        Compiler.AddCode($"scoreboard players set {location} amethyst {AsInteger}");
        
        return new BooleanResult
        {
            Compiler = Compiler,
            Context = Context,
            Location = location,
            IsTemporary = true,
        };
    }

    public override string ToNbtString()
    {
        return Value.ToNbtString();
    }

    public override string ToTextComponent()
    {
        var value = Value ? "1" : "0";
        return $$"""[{"text":"{{value}}","color":"gold"},{"text":"b","color":"red"}]""";
    }
}