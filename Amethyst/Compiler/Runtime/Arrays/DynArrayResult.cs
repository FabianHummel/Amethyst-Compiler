using Amethyst.Model;

namespace Amethyst;

public class DynArrayResult : ArrayBase
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.Array,
        Modifier = null
    };
}