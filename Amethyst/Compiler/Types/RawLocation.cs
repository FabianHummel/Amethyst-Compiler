using Amethyst.Model;

namespace Amethyst;

public partial class RawLocation : AbstractValue, IRuntimeValue
{
    public required Location Location { get; init; }
    
    public bool IsTemporary { get; init; }

    public override AbstractDatatype Datatype => new UnknownDatatype(Location.DataLocation);
    
    public AbstractBoolean MakeBoolean()
    {
        var location = NextFreeLocation(DataLocation.Scoreboard);
        
        Compiler.AddCode($"execute store success score {location} run data get storage {Location}");
        
        return new RuntimeBoolean
        {
            Location = location,
            Compiler = Compiler,
            Context = Context
        };
    }

    public IRuntimeValue WithLocation(Location newLocation, bool temporary = true)
    {
        return new RawLocation
        {
            Location = newLocation,
            Compiler = Compiler,
            Context = Context,
            IsTemporary = temporary
        };
    }
}