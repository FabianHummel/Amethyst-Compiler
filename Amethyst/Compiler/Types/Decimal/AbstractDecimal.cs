namespace Amethyst;

public abstract class AbstractDecimal : AbstractNumericValue
{
    protected override DecimalDatatype ScoreboardDatatype => new()
    {
        DecimalPlaces = DecimalPlaces
    };
    
    public required int DecimalPlaces { get; set; }
}