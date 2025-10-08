using Amethyst.Model;

namespace Amethyst;

public class ConstantStaticArray : AbstractConstantArray
{
    public required BasicType BasicType { get; init; }
    
    public override AbstractDatatype Datatype => AbstractDatatype.Parse(BasicType, Modifier.Array);
    
    public override AbstractRuntimeArray ToRuntimeValue()
    {
        var location = Location.Storage(++Compiler.StackPointer);
        
        Compiler.AddCode($"data modify storage {location} set value {ToNbtString()}");
        
        SubstituteRecursively(Compiler, location);
        
        return new RuntimeStaticArray
        {
            BasicType = BasicType,
            Location = location,
            Compiler = Compiler,
            Context = Context,
            IsTemporary = true
        };
    }
}