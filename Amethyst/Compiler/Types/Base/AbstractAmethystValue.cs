namespace Amethyst;

public abstract class AbstractAmethystValue : AbstractValue
{
    protected override AbstractValue VisitAdd(AbstractValue rhs)
    {
        // TODO: I don't like this being here. Find a way to move this to the specific type (e.g. AbstractString)
        if (rhs is AbstractString abstractString)
        {
            return AbstractString.VisitAdd(this, abstractString);
        }

        return base.VisitAdd(rhs);
    }
}