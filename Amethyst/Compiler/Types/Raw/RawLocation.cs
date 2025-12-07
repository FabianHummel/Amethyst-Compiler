using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

[ForwardDefaultInterfaceMethods(typeof(IRuntimeValue))]
public partial class RawLocation : AbstractValue, IRuntimeValue
{
    public required Location Location { get; init; }
    
    public bool IsTemporary { get; init; }

    public override AbstractDatatype Datatype => new RawDatatype(Location.DataLocation)
    {
        Namespace = Location.Namespace,
        Name = Location.Name
    };
    
    public AbstractBoolean MakeBoolean()
    {
        var location = NextFreeLocation(DataLocation.Scoreboard);
        
        this.AddCode($"execute store success score {location} run data get storage {Location}");
        
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