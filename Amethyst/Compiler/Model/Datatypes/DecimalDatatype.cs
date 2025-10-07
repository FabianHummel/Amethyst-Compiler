namespace Amethyst.Model;

public class DecimalDatatype : AbstractScoreboardDatatype
{
    public const int DEFAULT_DECIMAL_PLACES = 2;

    public required int DecimalPlaces { get; init; }
    
    public int Scale => (int) Math.Pow(10, DecimalPlaces);

    public override BasicType BasicType => BasicType.Dec;

    public override string StorageModifier => $"double {1.0 / Scale}";
}