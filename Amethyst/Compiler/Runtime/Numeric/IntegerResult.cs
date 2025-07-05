using Amethyst.Model;

namespace Amethyst;

public class IntegerResult : NumericBase
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.Int,
        Modifier = null
    };

    public override IntegerResult MakeInteger()
    {
        return this;
    }
}