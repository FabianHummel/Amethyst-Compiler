using Amethyst.Model;

namespace Amethyst;

public abstract class AbstractString : AbstractAmethystValue
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.String
    };

    protected override AbstractString VisitAdd(AbstractValue rhs)
    {
        throw new NotImplementedException("String + ?");
    }

    public static AbstractString VisitAdd(AbstractValue lhs, AbstractString rhs)
    {
        throw new NotImplementedException("? + String");
    }
}