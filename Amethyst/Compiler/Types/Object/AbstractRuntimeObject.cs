using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

[ForwardDefaultInterfaceMethods(typeof(IRuntimeValue))]
public abstract partial class AbstractRuntimeObject : AbstractObject, IRuntimeValue, IIndexable
{
    public required Location Location { get; init; }
    
    public bool IsTemporary { get; init; }

    public AbstractBoolean MakeBoolean()
    {
        var location = NextFreeLocation(DataLocation.Scoreboard);
        
        this.AddCode($"execute store success score {location} run data get storage {Location}.keys[0]");
        
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
        
        this.AddCode($"execute store result score {location} run data get storage {Location}.keys");
        
        return new RuntimeInteger
        {
            Location = location,
            Compiler = Compiler,
            Context = Context,
            IsTemporary = true
        };
    }

    public AbstractValue GetIndex(AbstractValue index)
    {
        var location = NextFreeLocation(DataLocation.Storage);
        
        if (index is ConstantString stringConstant)
        {
            this.AddCode($"execute store result storage {location} run data get storage {Location}.data.{stringConstant.Value}");
        }
        
        else if (index is RuntimeString stringResult)
        {
            string mcFunctionPath;
            using (Compiler.EvaluateScoped("_index"))
            {
                mcFunctionPath = Compiler.Scope.McFunctionPath;
                this.AddCode($"$execute store result storage {location} run data get storage {Location}.data.$({stringResult.Location.Name})");
            }
            
            this.AddCode($"function {mcFunctionPath} with storage amethyst:");
        }
        
        else
        {
            throw new SyntaxException("Expected string index.", index.Context);
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