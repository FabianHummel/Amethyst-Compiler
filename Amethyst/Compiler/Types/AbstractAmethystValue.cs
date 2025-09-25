namespace Amethyst;

public abstract class AbstractAmethystValue : AbstractValue
{
    protected override AbstractValue VisitAdd(AbstractValue rhs)
    {
        if (rhs is AbstractString abstractString)
        {
            return AbstractString.VisitAdd(this, abstractString);
        }

        return base.VisitAdd(rhs);
    }
}