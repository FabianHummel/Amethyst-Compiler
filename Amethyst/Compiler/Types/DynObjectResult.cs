using Amethyst.Model;

namespace Amethyst;

public class DynObjectResult : AbstractResult
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.Object,
        Modifier = null
    };

    public override AbstractResult CreateConstantValue()
    {
        return this;
    }
}