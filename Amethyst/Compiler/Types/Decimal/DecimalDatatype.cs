using Amethyst.Model;

namespace Amethyst;

public class DecimalDatatype : AbstractScoreboardDatatype
{
    public const int DEFAULT_DECIMAL_PLACES = 2;

    public int DecimalPlaces { get; init; }

    public DecimalDatatype(int decimalPlaces = DEFAULT_DECIMAL_PLACES)
    {
        DecimalPlaces = decimalPlaces;
    }
    
    public int Scale => (int) Math.Pow(10, DecimalPlaces);
    
    public override BasicType BasicType => BasicType.Dec;

    public override string StorageModifier => $"double {1.0 / Scale}";

    public override string ToString()
    {
        return $"{base.ToString()}({DecimalPlaces})";
    }
}