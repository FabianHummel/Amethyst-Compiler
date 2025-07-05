using Amethyst.Model;

namespace Amethyst;

public class DynObjectResult : ObjectBase
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.Object,
        Modifier = null
    };
}