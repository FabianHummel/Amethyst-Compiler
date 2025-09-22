namespace Amethyst;

public partial class RuntimeInteger : AbstractInteger, IRuntimeValue
{
    public int Location { get; set; }
    
    public bool IsTemporary { get; set; }
    
    public override AbstractString ToStringValue()
    {
        throw new NotImplementedException("Convert int to string with mcfunction and return RuntimeString");
    }
    
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