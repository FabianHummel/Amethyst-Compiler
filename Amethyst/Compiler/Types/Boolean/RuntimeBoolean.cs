using Amethyst.Model;

namespace Amethyst;

public partial class RuntimeBoolean : AbstractBoolean, IRuntimeValue
{
    public required Location Location { get; init; }
    
    public bool IsTemporary { get; init; }

    protected override RuntimeDecimal AsDecimal => new()
    {
        Compiler = Compiler,
        Context = Context,
        DecimalPlaces = 0,
        IsTemporary = IsTemporary,
        Location = Location
    };

    public AbstractBoolean MakeBoolean()
    {
        return this;
    }

    public IRuntimeValue WithLocation(Location newLocation, bool temporary = true)
    {
        return new RuntimeBoolean
        {
            Compiler = Compiler,
            Context = Context,
            Location = newLocation,
            IsTemporary = temporary
        };
    }

    public AbstractInteger MakeInteger()
    {
        return new RuntimeInteger
        {
            Compiler = Compiler,
            Context = Context,
            Location = Location,
            IsTemporary = IsTemporary
        };
    }
}