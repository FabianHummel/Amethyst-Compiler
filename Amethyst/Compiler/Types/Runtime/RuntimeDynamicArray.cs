using Amethyst.Model;

namespace Amethyst;

public class RuntimeDynamicArray : AbstractRuntimeArray
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.Array,
        Modifier = null
    };
}