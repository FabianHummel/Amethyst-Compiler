using Amethyst.Model;

namespace Amethyst;

public class StringResult : AbstractResult
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.String,
        Modifier = null
    };
    
    public override AbstractResult ToBool
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

    public override AbstractResult ToNumber
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