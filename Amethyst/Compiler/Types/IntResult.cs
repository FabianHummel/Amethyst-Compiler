using Amethyst.Model;

namespace Amethyst;

public class IntResult : NumericBase
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.Int,
        Modifier = null
    };

    public override BoolResult ToBool => new()
    {
        Compiler = Compiler,
        Context = Context,
        Location = Location
    };

    public override IntResult ToNumber => this;
    
    public override AbstractResult CreateConstantValue()
    {
        AddCode($"scoreboard players set {MemoryLocation} amethyst {(int)ConstantValue! * DataType.Scale}");
        
        return new IntResult
        {
            Compiler = Compiler,
            Context = Context,
            Location = MemoryLocation.ToString(),
            IsTemporary = true,
        };
    }
}