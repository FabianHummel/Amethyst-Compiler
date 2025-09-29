namespace Amethyst;

public partial class RuntimeInteger : AbstractInteger, IRuntimeValue
{
    public int Location { get; set; }
    
    public bool IsTemporary { get; set; }

    protected override AbstractDecimal AsDecimal => new RuntimeDecimal
    {
        Compiler = Compiler,
        Context = Context,
        DecimalPlaces = 0,
        IsTemporary = IsTemporary,
        Location = Location
    };

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

    public AbstractInteger MakeInteger()
    {
        return this;
    }
}