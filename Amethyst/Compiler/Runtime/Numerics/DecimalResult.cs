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
    
    public int Scale => (int) Math.Pow(10, DecimalPlaces);

    public override IntegerResult MakeInteger()
    {
        var location = Location;
        if (!IsTemporary)
        {
            location = (++Compiler.StackPointer).ToString();
            AddCode($"scoreboard players operation {location} amethyst = {Location} amethyst");
        }

        AddCode($"scoreboard players operation {location} amethyst *= .{Scale} amethyst_const");

        return new IntegerResult
        {
            Compiler = Compiler,
            Context = Context,
            Location = location,
        };
    }
}