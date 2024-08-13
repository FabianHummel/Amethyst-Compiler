using Amethyst.Model;

namespace Amethyst;

public class AnyArrayResult : AbstractResult
{
    public required BasicType BasicType { get; init; }
    
    public override DataType DataType => new()
    {
        BasicType = BasicType,
        Modifier = Modifier.Array
    };
    
    public override BoolResult ToBool
    {
        get
        {
            Compiler.AddCode($"execute store success score {++Compiler.MemoryLocation} amethyst run data get storage amethyst: {Location}");
            return new BoolResult
            {
                Location = Compiler.MemoryLocation.ToString(),
                Compiler = Compiler,
                Context = Context
            };
        }
    }

    public override IntResult ToNumber
    {
        get
        {
            Compiler.AddCode($"execute store result score {++Compiler.MemoryLocation} amethyst run data get storage amethyst: {Location}");
            return new IntResult
            {
                Location = Compiler.MemoryLocation.ToString(),
                Compiler = Compiler,
                Context = Context
            };
        }
    }
}