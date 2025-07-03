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
        Compiler.AddCode($"scoreboard players set {Compiler.StackPointer} amethyst {AsInteger}");
        
        return new BooleanResult
        {
            Compiler = Compiler,
            Context = Context,
            Location = Compiler.StackPointer++.ToString(),
            IsTemporary = true,
        };
    }

    public override string ToNbtString()
    {
        return Value.ToNbtString();
    }
}