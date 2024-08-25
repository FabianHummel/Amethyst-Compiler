using Amethyst.Model;

namespace Amethyst;

public class DynArrayResult : AbstractResult
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.Array,
        Modifier = null
    };

    public override AbstractResult CreateConstantValue()
    {
        return this;
    }
}