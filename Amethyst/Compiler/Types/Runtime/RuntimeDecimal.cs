namespace Amethyst;

public partial class RuntimeDecimal : AbstractDecimal, IRuntimeValue
{
    public int Location { get; set; }
    
    public bool IsTemporary { get; set; }
    
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

    public AbstractInteger MakeInteger()
    {
        var location = Location;
        
        if (!IsTemporary)
        {
            location = ++Compiler.StackPointer;
            AddCode($"scoreboard players operation {location} amethyst = {Location} amethyst");
        }

        return new RuntimeInteger
        {
            Compiler = Compiler,
            Context = Context,
            Location = location,
            IsTemporary = true
        };
    }
}