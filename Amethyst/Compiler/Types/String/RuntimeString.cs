using Amethyst.Model;

namespace Amethyst;

public partial class RuntimeString : AbstractString, IRuntimeValue
{
    public required Location Location { get; init; }

    public bool IsTemporary { get; init; }

    public AbstractBoolean MakeBoolean()
    {
        var location = NextFreeLocation(DataLocation.Scoreboard);
        
        this.AddCode($"execute store success score {location} run data get storage {Location}");
        
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
        
        this.AddCode($"execute store result score {location} run data get storage {Location}");
        
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
        
        string mcFunctionPath;
        using (Compiler.EvaluateScoped("_concat"))
        {
            mcFunctionPath = Compiler.Scope.McFunctionPath;
            // Todo: sanitize string by escaping quotes and other special characters that may mess up the macro expansion
            this.AddCode($"$data modify storage {resultLocation} set value \"$({Location})$({rhs.Location})\"");
        }
        
        this.AddCode($"function {mcFunctionPath} with storage amethyst:");

        return new RuntimeString
        {
            Compiler = Compiler,
            Context = Context,
            Location = resultLocation,
            IsTemporary = true
        };
    }
}