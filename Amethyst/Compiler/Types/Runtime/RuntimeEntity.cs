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

    public AbstractBoolean MakeBoolean()
    {
        throw new NotImplementedException("TODO: true, if the entity(s) exist");
    }

    public AbstractInteger MakeInteger()
    {
        throw new NotImplementedException("TODO: Amount of entities in the selector");
    }
}