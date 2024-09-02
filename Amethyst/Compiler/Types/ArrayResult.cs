using Amethyst.Model;

namespace Amethyst;

public class ArrayResult : AbstractResult
{
    public required BasicType BasicType { get; init; }
    
    public override DataType DataType => new()
    {
        BasicType = BasicType,
        Modifier = Modifier.Array
    };

    public override BooleanResult MakeBoolean()
    {
        Compiler.AddCode($"execute store success score {MemoryLocation} amethyst run data get storage amethyst: {Location}");
        
        return new BooleanResult
        {
            Location = MemoryLocation.ToString(),
            Compiler = Compiler,
            Context = Context,
            IsTemporary = true
        };
    }

    public override IntegerResult MakeNumber()
    {
        Compiler.AddCode($"execute store result score {MemoryLocation} amethyst run data get storage amethyst: {Location}");
        
        return new IntegerResult
        {
            Location = MemoryLocation.ToString(),
            Compiler = Compiler,
            Context = Context,
            IsTemporary = true
        };
    }

    public override AbstractResult MakeVariable()
    {
        if (Location != null)
        {
            return this;
        }
        
        AddCode($"data modify storage amethyst: {MemoryLocation} set value {ConstantValue}");

        SubstituteRecursively(MemoryLocation.ToString());

        return new DynArrayResult
        {
            Location = MemoryLocation++.ToString(),
            Compiler = Compiler,
            Context = Context,
            IsTemporary = true,
        };
    }
}