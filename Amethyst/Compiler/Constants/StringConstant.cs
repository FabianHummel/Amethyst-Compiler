using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public class StringConstant : ConstantValue<string>
{
    public override int AsInteger => Value.Length;
    
    public override bool AsBoolean => !string.IsNullOrEmpty(Value);
    
    public override DataType DataType => new()
    {
        BasicType = BasicType.String,
        Modifier = null
    };

    public override RuntimeValue ToRuntimeValue()
    {
        var location = ++Compiler.StackPointer;
        
        Compiler.AddCode($"data modify storage amethyst: {location} set value {ToNbtString()}");
        
        return new StringResult
        {
            Compiler = Compiler,
            Context = Context,
            Location = location.ToString(),
            IsTemporary = true
        };
    }

    public override string ToNbtString()
    {
        return Value.ToNbtString();
    }
}