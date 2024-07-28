namespace Amethyst.Model.Types;

public class AnyObjectResult : AbstractResult
{
    public required BasicType BasicType { get; init; }
    
    public override DataType DataType => new()
    {
        BasicType = BasicType,
        Modifier = Modifier.Object
    };
}