using Amethyst.Model;

namespace Amethyst;

public class RuntimeDynamicObject : AbstractRuntimeObject
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.Object,
        Modifier = null
    };
}