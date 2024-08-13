using Amethyst.Model;

namespace Amethyst;

public class BoolResult : NumericBase
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.Bool,
        Modifier = null
    };

    public override BoolResult ToBool => this;

    public override IntResult ToNumber => new()
    {
        Compiler = Compiler,
        Context = Context,
        Location = Location
    };
}