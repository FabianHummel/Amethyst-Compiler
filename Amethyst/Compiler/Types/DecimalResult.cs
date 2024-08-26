using Amethyst.Model;

namespace Amethyst;

public class DecimalResult : NumericBase
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.Dec,
        Modifier = null
    };

    public override BooleanResult MakeBoolean() => new()
    {
        Compiler = Compiler,
        Context = Context,
        Location = Location
    };

    public override IntegerResult MakeNumber()
    {
        var location = Location;
        if (!IsTemporary)
        {
            location = MemoryLocation++.ToString();
            AddCode($"scoreboard players operation {location} amethyst = {Location} amethyst");
        }

        AddCode($"scoreboard players operation {location} amethyst *= .{DataType.Scale} amethyst_const");

        return new IntegerResult
        {
            Compiler = Compiler,
            Context = Context,
            Location = location,
        };
    }

    public override AbstractResult MakeVariable()
    {
        AddCode($"scoreboard players set {MemoryLocation} amethyst {(int)((double)ConstantValue! * DataType.Scale!)}");
        
        return new BooleanResult
        {
            Compiler = Compiler,
            Context = Context,
            Location = MemoryLocation++.ToString(),
            IsTemporary = true,
        };
    }
}