using Amethyst.Model;

namespace Amethyst;

public partial class RuntimeEntity : AbstractValue, IRuntimeValue
{
    public int Location { get; set; }
    
    public bool IsTemporary { get; set; }

    public override DataType DataType => new DataType
    {
        BasicType = BasicType.Entity
    };

    public override AbstractString ToStringValue()
    {
        throw new NotImplementedException();
    }

    public AbstractBoolean MakeBoolean()
    {
        throw new NotImplementedException("If the entity(s) exist");
    }

    public AbstractInteger MakeInteger()
    {
        throw new NotImplementedException("Amount of entities");
    }
}