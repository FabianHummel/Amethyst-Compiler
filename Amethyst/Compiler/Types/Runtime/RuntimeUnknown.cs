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

    public override AbstractString ToStringValue()
    {
        throw new NotImplementedException("Cannot convert unknown result to string.");
    }

    public AbstractBoolean MakeBoolean()
    {
        throw new NotImplementedException("Cannot convert unknown result to boolean.");
    }

    public AbstractInteger MakeInteger()
    {
        throw new NotImplementedException("Cannot convert unknown result to integer.");
    }

    public IRuntimeValue ToRuntimeValue()
    {
        throw new SyntaxException("Cannot convert unknown result to runtime value.", Context);
    }
}