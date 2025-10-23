using Amethyst.Model;

namespace Amethyst;

public abstract partial class AbstractRuntimeArray : AbstractArray, IRuntimeValue, IIndexable
{
    public required Location Location { get; init; }
    
    public bool IsTemporary { get; init; }

    public AbstractBoolean MakeBoolean()
    {
        var location = NextFreeLocation(DataLocation.Scoreboard);
        
        this.AddCode($"execute store success score {location} run data get storage {Location}[0]");
        
        return new RuntimeBoolean
        {
            Location = location,
            Compiler = Compiler,
            Context = Context,
            IsTemporary = true
        };
    }

    public abstract IRuntimeValue WithLocation(Location newLocation, bool temporary = true);

    public AbstractInteger MakeInteger()
    {
        var location = NextFreeLocation(DataLocation.Scoreboard);
        
        this.AddCode($"execute store result score {location} run data get storage {Location}");
        
        return new RuntimeInteger
        {
            Location = location,
            Compiler = Compiler,
            Context = Context,
            IsTemporary = true
        };
    }

    // TODO: Maybe directly return a RuntimeUnknown with a modified path location pointing to the element at the index?
    public AbstractValue GetIndex(AbstractValue index)
    {
        var location = NextFreeLocation(DataLocation.Storage);
        
        if (index is ConstantInteger integerConstant)
        {
            this.AddCode($"execute store result storage {location} run data get storage {Location}[{integerConstant.Value}]");
        }
        else if (index is RuntimeInteger and IRuntimeValue integerResult)
        {
            var indexLocation = integerResult.NextFreeLocation(DataLocation.Storage);
            Compiler.StackPointer--;
            
            this.AddCode($"execute store result storage {indexLocation} run scoreboard players get {integerResult.Location}");
            
            string mcFunctionPath;
            using (Compiler.EvaluateScoped("_index"))
            {
                mcFunctionPath = Compiler.Scope.McFunctionPath;
                this.AddCode($"$execute store result storage {location} run data get storage {Location}[$({integerResult.Location})]");
            }
            
            this.AddCode($"function {mcFunctionPath} with storage amethyst:");
        }
        else
        {
            throw new SyntaxException("Expected integer index.", index.Context);
        }

        return new RawLocation
        {
            Compiler = Compiler,
            Context = Context,
            Location = location,
            IsTemporary = true
        };
    }
}