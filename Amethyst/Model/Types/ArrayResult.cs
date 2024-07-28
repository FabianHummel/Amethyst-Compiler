namespace Amethyst.Model.Types;

public class ArrayResult : AbstractResult
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.Array,
        Modifier = null
    };
}