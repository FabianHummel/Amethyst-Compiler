using Amethyst.Model;

namespace Amethyst;

public class ConstantDynamicArray : AbstractConstantArray
{
    public override AbstractDatatype Datatype => new ArrayDatatype();
    
    public override AbstractRuntimeArray ToRuntimeValue()
    {
        var location = Location.Storage(++Compiler.StackPointer);
        
        this.AddCode($"data modify storage {location} set value {ToNbtString()}");
        
        SubstituteRecursively(location);
        
        return new RuntimeDynamicArray
        {
            Location = location,
            Compiler = Compiler,
            Context = Context,
            IsTemporary = true
        };
    }
}