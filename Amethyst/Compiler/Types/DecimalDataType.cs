using Amethyst.Model;

namespace Amethyst;

public class DecimalDataType : DataType
{
    public const int DEFAULT_DECIMAL_PLACES = 2;
    
    public required int DecimalPlaces { get; init; }
    
    public int Scale => (int) Math.Pow(10, DecimalPlaces);
}