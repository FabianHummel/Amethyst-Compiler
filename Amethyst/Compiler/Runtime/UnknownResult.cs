using Amethyst.Model;

namespace Amethyst;

public class UnknownResult : RuntimeValue
{
    public override DataType DataType => new DataType
    {
        BasicType = BasicType.Unknown
    };

    public override BooleanResult MakeBoolean()
    {
        throw new NotImplementedException("Cannot convert unknown result to boolean.");
    }

    public override IntegerResult MakeInteger()
    {
        throw new NotImplementedException("Cannot convert unknown result to integer.");
    }

    public override RuntimeValue ToRuntimeValue()
    {
        throw new SyntaxException("Cannot convert unknown result to runtime value.", Context);
    }
}