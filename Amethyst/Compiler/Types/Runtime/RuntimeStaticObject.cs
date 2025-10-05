using Amethyst.Model;

namespace Amethyst;

public class RuntimeStaticObject : AbstractRuntimeObject
{
    public required BasicType BasicType { get; init; }
    
    public override DataType DataType => new()
    {
        BasicType = BasicType,
        Modifier = Modifier.Object
    };
}