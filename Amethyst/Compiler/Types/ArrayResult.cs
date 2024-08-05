using Amethyst.Model;

namespace Amethyst;

public class ArrayResult : AbstractResult
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.Array,
        Modifier = null
    };
}