using Amethyst.Model;

namespace Amethyst;

public partial class RuntimeUnknown : AbstractValue, IRuntimeValue
{
    public int Location { get; set; }
    
    public bool IsTemporary { get; set; }

    public override DataType DataType => new()
    {
        BasicType = BasicType.Unknown
    };

    public AbstractBoolean MakeBoolean()
    {
        throw new InvalidOperationException("Cannot convert unknown results to a boolean value.");
    }

    public AbstractInteger MakeInteger()
    {
        throw new InvalidOperationException("Cannot convert unknown results to an integer value.");
    }
}