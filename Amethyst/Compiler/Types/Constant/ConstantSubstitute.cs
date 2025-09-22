using System.Diagnostics;
using Amethyst.Model;

namespace Amethyst;

public class ConstantSubstitute : AbstractValue, IConstantValue<IRuntimeValue>
{
    public required IRuntimeValue Value { get; init; }
    
    public override DataType DataType => Value.DataType;
    
    public override AbstractString ToStringValue()
    {
        throw new UnreachableException("ConstantSubstitute cannot be converted to a string.");
    }

    public int AsInteger => throw new UnreachableException();
    
    public bool AsBoolean => throw new UnreachableException();
    
    public IRuntimeValue ToRuntimeValue()
    {
        throw new UnreachableException("ConstantSubstitute cannot be converted to a runtime value.");
    }

    public string ToNbtString()
    {
        return Value.DataType.DefaultValue;
    }

    public string ToTextComponent()
    {
        throw new UnreachableException("ConstantSubstitute cannot be converted to a text component.");
    }

    public bool Equals(IConstantValue? other)
    {
        throw new UnreachableException("ConstantSubstitute cannot be compared for equality.");
    }

    public override string ToTargetSelectorString()
    {
        return Value.ToTargetSelectorString();
    }
}