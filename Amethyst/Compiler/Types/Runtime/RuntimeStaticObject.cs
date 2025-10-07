using Amethyst.Model;

namespace Amethyst;

public class RuntimeStaticObject : AbstractRuntimeObject
{
    public required BasicType BasicType { get; init; }
    
    public override AbstractDatatype Datatype => AbstractDatatype.Parse(BasicType, Modifier.Object);
    
    public override IRuntimeValue WithLocation(Location newLocation, bool temporary = true)
    {
        return new RuntimeStaticObject
        {
            Compiler = Compiler,
            Context = Context,
            Location = newLocation,
            IsTemporary = temporary,
            BasicType = BasicType
        };
    }
}