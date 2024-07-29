using Amethyst.Model;

namespace Amethyst;

public class BoolResult : AbstractResult
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.Bool,
        Modifier = null
    };

    public override AbstractResult ToBool => this;

    public override AbstractResult ToNumber => this;
}