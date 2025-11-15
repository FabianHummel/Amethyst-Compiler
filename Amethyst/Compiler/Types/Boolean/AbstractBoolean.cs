namespace Amethyst;

public abstract class AbstractBoolean : AbstractNumericValue
{
    protected override BooleanDatatype ScoreboardDatatype => new();
}