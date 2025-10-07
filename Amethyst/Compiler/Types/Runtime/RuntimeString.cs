using Amethyst.Model;

namespace Amethyst;

public partial class RuntimeString : AbstractString, IRuntimeValue
{
    public required Location Location { get; init; }

    public bool IsTemporary { get; set; }

    public AbstractBoolean MakeBoolean()
    {
        var location = NextFreeLocation(DataLocation.Scoreboard);
        
        Compiler.AddCode($"execute store success score {location} run data get storage {Location}");
        
        return new RuntimeBoolean
        {
            Location = location,
            Compiler = Compiler,
            Context = Context
        };
    }

    public IRuntimeValue WithLocation(Location newLocation, bool temporary = true)
    {
        return new RuntimeString
        {
            Compiler = Compiler,
            Context = Context,
            Location = newLocation,
            IsTemporary = temporary
        };
    }

    public AbstractInteger MakeInteger()
    {
        var location = NextFreeLocation(DataLocation.Scoreboard);
        
        Compiler.AddCode($"execute store result score {location} run data get storage {Location}");
        
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
            resultLocation = NextFreeLocation(DataLocation.Storage);
        }
        
        var scope = Compiler.EvaluateScoped("_concat", _ =>
        {
            // Todo: sanitize string by escaping quotes and other special characters that may mess up the macro expansion
            Compiler.AddCode($"$data modify storage {resultLocation} set value \"$({Location})$({rhs.Location})\"");
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