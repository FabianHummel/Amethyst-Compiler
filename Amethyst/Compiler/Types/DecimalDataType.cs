using Amethyst.Model;

namespace Amethyst;

public class DecimalDataType : DataType
{
    public required int DecimalPlaces { get; init; }
    
    public int Scale => (int) Math.Pow(10, DecimalPlaces);
}