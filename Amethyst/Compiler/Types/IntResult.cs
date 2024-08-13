using Amethyst.Model;

namespace Amethyst;

public class IntResult : NumericBase
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.Int,
        Modifier = null
    };

    public override BoolResult ToBool => new()
    {
        Compiler = Compiler,
        Context = Context,
        Location = Location
    };

    public override IntResult ToNumber => this;
}