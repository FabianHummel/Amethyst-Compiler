using Amethyst.Model;

namespace Amethyst;

public abstract class AbstractInteger : AbstractNumericValue
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.Int
    };
}