using Amethyst.Model;

namespace Amethyst;

public class DecResult : NumericBase
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.Dec,
        Modifier = null
    };

    public override BoolResult ToBool => new()
    {
        Compiler = Compiler,
        Context = Context,
        Location = Location
    };

    public override IntResult ToNumber
    {
        get
        {
            var location = Location;
            if (!IsTemporary)
            {
                location = MemoryLocation++.ToString();
                AddCode($"scoreboard players operation {location} amethyst = {Location} amethyst");
            }
            
            AddCode($"scoreboard players operation {location} amethyst *= .{DataType.Scale} amethyst_const");
            
            return new IntResult
            {
                Compiler = Compiler,
                Context = Context,
                Location = location,
            };
        }
    }
    
    public override AbstractResult CreateConstantValue()
    {
        AddCode($"scoreboard players set {MemoryLocation} amethyst {(double)ConstantValue! * DataType.Scale}");
        
        return new BoolResult
        {
            Compiler = Compiler,
            Context = Context,
            Location = MemoryLocation.ToString(),
            IsTemporary = true,
        };
    }
}