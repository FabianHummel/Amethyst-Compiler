using Amethyst.Model;

namespace Amethyst;

public class DecimalResult : NumericBase
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.Dec,
        Modifier = null
    };

    protected override double AsDecimal => (double)ConstantValue!;

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
        if (Location != null)
        {
            return this;
        }
        
        var value = Math.Round((double)(AsDecimal * DataType.Scale!));
        AddCode($"scoreboard players set {MemoryLocation} amethyst {value}");
        
        return new DecimalResult
        {
            Compiler = Compiler,
            Context = Context,
            Location = MemoryLocation++.ToString(),
            IsTemporary = true,
        };
    }
}