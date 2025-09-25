namespace Amethyst;

public partial class RuntimeBoolean : AbstractBoolean, IRuntimeValue
{
    public int Location { get; set; }
    
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

    public override AbstractString ToStringValue()
    {
        throw new NotImplementedException("Convert bool to string with mcfunction and return RuntimeString");
    }

    public AbstractBoolean MakeBoolean()
    {
        return this;
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