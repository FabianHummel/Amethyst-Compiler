using Amethyst.Model;

namespace Amethyst;

public abstract class AbstractDecimal : AbstractNumericValue
{
    public override DecimalDataType DataType => new()
    {
        BasicType = BasicType.Dec,
        Modifier = null,
        DecimalPlaces = DecimalPlaces
    };
    
    public required int DecimalPlaces { get; init; }
}