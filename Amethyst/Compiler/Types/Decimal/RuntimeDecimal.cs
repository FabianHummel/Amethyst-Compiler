using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

[ForwardDefaultInterfaceMethods(typeof(IRuntimeValue))]
public partial class RuntimeDecimal : AbstractDecimal, IRuntimeValue
{
    public required Location Location { get; init; }
    
    public bool IsTemporary { get; init; }
    
    protected override AbstractDecimal AsDecimal => this;

    public AbstractBoolean MakeBoolean()
    {
        return new RuntimeBoolean
        {
            Compiler = Compiler,
            Context = Context,
            Location = Location,
            IsTemporary = IsTemporary
        };
    }

    public IRuntimeValue WithLocation(Location newLocation, bool temporary = true)
    {
        return new RuntimeDecimal
        {
            Compiler = Compiler,
            Context = Context,
            Location = newLocation,
            IsTemporary = temporary,
            DecimalPlaces = DecimalPlaces
        };
    }

    public AbstractInteger MakeInteger()
    {
        var location = NextFreeLocation(DataLocation.Scoreboard);
        
        this.AddCode($"scoreboard players operation {location} = {Location}");

        return new RuntimeInteger
        {
            Compiler = Compiler,
            Context = Context,
            Location = location,
            IsTemporary = true
        };
    }
}