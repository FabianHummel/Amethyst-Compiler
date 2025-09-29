using Amethyst.Model;

namespace Amethyst;

public class RuntimeStaticArray : AbstractRuntimeArray, IStaticCollection
{
    public required BasicType BasicType { get; init; }
    
    public override DataType DataType => new()
    {
        BasicType = BasicType,
        Modifier = Modifier.Array
    };
}