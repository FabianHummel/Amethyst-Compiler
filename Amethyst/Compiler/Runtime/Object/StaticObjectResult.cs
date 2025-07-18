using Amethyst.Model;

namespace Amethyst;

public class StaticObjectResult : ObjectBase
{
    public required BasicType BasicType { get; init; }
    
    public override DataType DataType => new()
    {
        BasicType = BasicType,
        Modifier = Modifier.Object
    };
}