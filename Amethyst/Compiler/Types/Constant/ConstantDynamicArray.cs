using Amethyst.Model;

namespace Amethyst;

public class ConstantDynamicArray : AbstractConstantArray
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.Array,
        Modifier = null
    };
    
    public override AbstractRuntimeArray ToRuntimeValue()
    {
        var location = ++Compiler.StackPointer;
        
        Compiler.AddCode($"data modify storage amethyst: {location} set value {ToNbtString()}");
        
        SubstituteRecursively(Compiler, location.ToString());
        
        return new RuntimeDynamicArray
        {
            Location = location,
            Compiler = Compiler,
            Context = Context,
            IsTemporary = true
        };
    }
}