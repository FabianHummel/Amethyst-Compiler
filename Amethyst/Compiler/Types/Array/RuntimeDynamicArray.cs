using Amethyst.Model;

namespace Amethyst;

public class RuntimeDynamicArray : AbstractRuntimeArray
{
    public override AbstractDatatype Datatype => new ArrayDatatype();
    
    public override IRuntimeValue WithLocation(Location newLocation, bool temporary = true)
    {
        return new RuntimeDynamicArray
        {
            Compiler = Compiler,
            Context = Context,
            Location = newLocation,
            IsTemporary = temporary
        };
    }
}