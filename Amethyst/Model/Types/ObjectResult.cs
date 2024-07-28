namespace Amethyst.Model.Types;

public class ObjectResult : AbstractResult
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.Object,
        Modifier = null
    };
}