using Amethyst.Model;

namespace Amethyst;

public class ObjectResult : AbstractResult
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.Object,
        Modifier = null
    };
}