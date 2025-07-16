using Amethyst.Model;

namespace Amethyst;

public class EntityResult : RuntimeValue
{
    public override DataType DataType => new DataType
    {
        BasicType = BasicType.Entity
    };
    
    public override BooleanResult MakeBoolean()
    {
        throw new NotImplementedException();
    }

    public override IntegerResult MakeInteger()
    {
        throw new NotImplementedException();
    }
}