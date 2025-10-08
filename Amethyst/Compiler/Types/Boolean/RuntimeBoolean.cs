using Amethyst.Model;

namespace Amethyst;

public partial class RuntimeBoolean : AbstractBoolean, IRuntimeValue
{
    public required Location Location { get; init; }
    
    public bool IsTemporary { get; set; }

    protected override AbstractDecimal AsDecimal
    {
        get
        {
            var value = EnsureBackedUp();
            return new RuntimeDecimal
            {
                Compiler = Compiler,
                Context = Context,
                DecimalPlaces = 0,
                IsTemporary = true,
                Location = value.Location
            };
        }
    }

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