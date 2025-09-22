namespace Amethyst;

public partial class RuntimeBoolean : AbstractBoolean, IRuntimeValue
{
    public int Location { get; set; }
    
    public bool IsTemporary { get; set; }

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