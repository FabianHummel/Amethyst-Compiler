namespace Amethyst;

public abstract class AbstractAmethystValue : AbstractValue
{
    protected override AbstractValue VisitAdd(AbstractValue rhs)
    {
        // TODO: I don't like this being here. Find a way to move this to the specific type (e.g. AbstractString). USE DOUBLE DISPATCH!! (Separate function where the RHS is defined and the LHS is not and calls these functions instead.)
        if (rhs is AbstractString abstractString)
        {
            return AbstractString.VisitAdd(this, abstractString);
        }

        return base.VisitAdd(rhs);
    }
}