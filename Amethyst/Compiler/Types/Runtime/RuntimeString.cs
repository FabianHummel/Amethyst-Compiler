namespace Amethyst;

public partial class RuntimeString : AbstractString, IRuntimeValue
{
    public int Location { get; set; }

    public bool IsTemporary { get; set; }

    public AbstractBoolean MakeBoolean()
    {
        var location = ++Compiler.StackPointer;
        
        Compiler.AddCode($"execute store success score {location} amethyst run data get storage amethyst: {Location}");
        
        return new RuntimeBoolean
        {
            Location = Compiler.StackPointer,
            Compiler = Compiler,
            Context = Context
        };
    }

    public AbstractInteger MakeInteger()
    {
        var location = ++Compiler.StackPointer;
        
        Compiler.AddCode($"execute store result score {location} amethyst run data get storage amethyst: {Location}");
        
        return new RuntimeInteger
        {
            Location = location,
            Compiler = Compiler,
            Context = Context
        };
    }

    protected IRuntimeValue VisitAdd(IRuntimeValue rhs)
    {
        var resultLocation = Location;

        if (rhs.IsTemporary)
        {
            resultLocation = rhs.Location;
        }
        else if (!IsTemporary)
        {
            resultLocation = ++Compiler.StackPointer;
        }
        
        var scope = Compiler.EvaluateScoped("_concat", _ =>
        {
            // Todo: sanitize string by escaping quotes and other special characters that may mess up the macro expansion
            Compiler.AddCode($"$data modify storage amethyst: {resultLocation} set value \"$({Location})$({rhs.Location})\"");
        });
        
        Compiler.AddCode($"function {scope.McFunctionPath} with storage amethyst:");

        return new RuntimeString
        {
            Compiler = Compiler,
            Context = Context,
            Location = resultLocation,
            IsTemporary = true
        };
    }
}