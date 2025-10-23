using Amethyst.Model;

namespace Amethyst;

public class RuntimeStaticArray : AbstractRuntimeArray
{
    public required BasicType BasicType { get; init; }
    
    public override AbstractDatatype Datatype => AbstractDatatype.Parse(BasicType, Modifier.Array);
    
    public override IRuntimeValue WithLocation(Location newLocation, bool temporary = true)
    {
        return new RuntimeStaticArray
        {
            Compiler = Compiler,
            Context = Context,
            Location = newLocation,
            IsTemporary = temporary,
            BasicType = BasicType
        };
    }
}