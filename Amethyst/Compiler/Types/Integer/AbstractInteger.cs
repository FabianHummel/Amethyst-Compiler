namespace Amethyst;

public abstract class AbstractInteger : AbstractNumericValue
{
    protected override IntegerDatatype ScoreboardDatatype => new();
}