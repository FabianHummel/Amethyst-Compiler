using Amethyst.Utility;

namespace Amethyst;

public interface IStringConcatenable
{
    AbstractString Concatenate(AbstractString rhs);
    
    [OverrideForBaseType(typeof(AbstractValue), "protected")]
    public AbstractValue VisitAdd(AbstractString rhs)
    {
        return Concatenate(rhs);
    }
}