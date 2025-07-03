using Amethyst.Model;

namespace Amethyst;

public class DecimalDataType : DataType
{
    public required int DecimalPlaces { get; init; }
}