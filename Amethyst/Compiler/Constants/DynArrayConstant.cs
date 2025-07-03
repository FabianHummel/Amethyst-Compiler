using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public class DynArrayConstant : ConstantValue<ConstantValue[]>
{
    public override int AsInteger => Value.Length;
    
    public override bool AsBoolean => AsInteger > 0;

    public override DataType DataType => new()
    {
        BasicType = BasicType.Array,
        Modifier = null
    };
    
    public override RuntimeValue ToRuntimeValue()
    {
        var location = ++Compiler.StackPointer;
        
        Compiler.AddCode($"data modify storage amethyst: {location} set value {ToNbtString()}");
        
        return new DynArrayResult
        {
            Location = location.ToString(),
            Compiler = Compiler,
            Context = Context,
            IsTemporary = true
        };
    }

    public override string ToNbtString()
    {
        return $"[{string.Join(",", Value.Select(v => $"{{_:{v.ToNbtString()}}}"))}]";
    }
}