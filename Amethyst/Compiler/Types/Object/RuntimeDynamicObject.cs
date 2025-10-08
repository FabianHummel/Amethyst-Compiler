using Amethyst.Model;

namespace Amethyst;

public class RuntimeDynamicObject : AbstractRuntimeObject
{
    public override AbstractDatatype Datatype => new ObjectDatatype();
    
    public override IRuntimeValue WithLocation(Location newLocation, bool temporary = true)
    {
        return new RuntimeDynamicObject
        {
            Compiler = Compiler,
            Context = Context,
            Location = newLocation,
            IsTemporary = temporary
        };
    }
}