namespace Amethyst.Model.Types;

public class DecResult : AbstractResult
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.Dec,
        Modifier = null
    };

    public override AbstractResult ToBool => this;

    public override AbstractResult ToNumber => this;
}