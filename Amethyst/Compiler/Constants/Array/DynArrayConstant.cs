using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public class DynArrayConstant : ArrayConstantBase
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.Array,
        Modifier = null
    };
    
    public override RuntimeValue ToRuntimeValue()
    {
        var location = ++Compiler.StackPointer;
        
        Compiler.AddCode($"data modify storage amethyst: {location} set value {ToNbtString()}");
        
        SubstituteRecursively(Compiler, location.ToString());
        
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