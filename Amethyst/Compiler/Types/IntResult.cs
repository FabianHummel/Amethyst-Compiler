using Amethyst.Model;

namespace Amethyst;

public class IntResult : AbstractResult
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.Int,
        Modifier = null
    };

    public override AbstractResult ToBool => this;

    public override AbstractResult ToNumber => this;
}