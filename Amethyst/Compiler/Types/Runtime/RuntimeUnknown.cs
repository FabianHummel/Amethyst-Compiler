using Amethyst.Model;

namespace Amethyst;

public partial class RuntimeUnknown : AbstractValue, IRuntimeValue
{
    public required Location Location { get; init; }
    
    public bool IsTemporary { get; set; }

    public override AbstractDatatype Datatype => new UnknownDatatype(Location.DataLocation);

    public AbstractBoolean MakeBoolean()
    {
        throw new InvalidOperationException("Cannot convert unknown results to a boolean value.");
    }

    public IRuntimeValue WithLocation(Location newLocation, bool temporary = true)
    {
        return new RuntimeUnknown
        {
            Compiler = Compiler,
            Context = Context,
            Location = newLocation,
            IsTemporary = temporary
        };
    }

    public AbstractInteger MakeInteger()
    {
        throw new InvalidOperationException("Cannot convert unknown results to an integer value.");
    }
}