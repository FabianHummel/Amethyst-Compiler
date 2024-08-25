using Amethyst.Model;

namespace Amethyst;

public class BoolResult : NumericBase
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.Bool,
        Modifier = null
    };

    public override BoolResult ToBool => this;

    public override IntResult ToNumber => new()
    {
        Compiler = Compiler,
        Context = Context,
        Location = Location
    };
    
    
    public override AbstractResult CreateConstantValue()
    {
        AddCode($"scoreboard players set {MemoryLocation} amethyst {ConstantValue}");
        
        return new BoolResult
        {
            Compiler = Compiler,
            Context = Context,
            Location = MemoryLocation.ToString(),
            IsTemporary = true,
        };
    }
}