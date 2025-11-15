namespace Amethyst;

public abstract class AbstractString : AbstractValue
{
    public override StringDatatype Datatype => new();

    protected override AbstractString VisitAdd(AbstractValue rhs)
    {
        throw new NotImplementedException("String + ?");
    }
}