using Amethyst.Model;

namespace Amethyst;

public class DecimalResult : NumericBase
{
    public required int DecimalPlaces { get; init; }
    
    public override DecimalDataType DataType => new()
    {
        BasicType = BasicType.Dec,
        Modifier = null,
        DecimalPlaces = DecimalPlaces
    };

    public override IntegerResult MakeInteger()
    {
        var location = Location;
        if (!IsTemporary)
        {
            location = ++Compiler.StackPointer;
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
}