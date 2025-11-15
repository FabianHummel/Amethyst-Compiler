using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

[ForwardDefaultInterfaceMethods(typeof(IRuntimeValue))]
public partial class RuntimeEntity : AbstractValue, IRuntimeValue
{
    public required Location Location { get; init; }
    
    public bool IsTemporary { get; init; }

    public override AbstractDatatype Datatype => new EntityDatatype();

    public AbstractBoolean MakeBoolean()
    {
        throw new NotImplementedException("TODO: true, if the entity(s) exist");
    }

    public IRuntimeValue WithLocation(Location newLocation, bool temporary = true)
    {
        return new RuntimeEntity
        {
            Compiler = Compiler,
            Context = Context,
            Location = newLocation,
            IsTemporary = temporary
        };
    }

    public AbstractInteger MakeInteger()
    {
        throw new NotImplementedException("TODO: Amount of entities in the selector");
    }
}