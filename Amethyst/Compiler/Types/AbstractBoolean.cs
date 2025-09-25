using Amethyst.Model;

namespace Amethyst;

public abstract class AbstractBoolean : AbstractNumericValue
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.Bool
    };
}