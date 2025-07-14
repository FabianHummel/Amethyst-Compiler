using System.Diagnostics;
using Amethyst.Model;

namespace Amethyst;

public class ConstantSubstitute : ConstantValue<RuntimeValue>
{
    public override DataType DataType => Value.DataType;
    
    public override int AsInteger => throw new UnreachableException();
    
    public override bool AsBoolean => throw new UnreachableException();
    
    public override RuntimeValue ToRuntimeValue()
    {
        throw new UnreachableException("ConstantSubstitute cannot be converted to a runtime value.");
    }

    public override string ToNbtString()
    {
        return Value.DataType.DefaultValue;
    }

    public override string ToTextComponent()
    {
        throw new UnreachableException("ConstantSubstitute cannot be converted to a text component.");
    }

    public override bool Equals(ConstantValue? other)
    {
        throw new UnreachableException("ConstantSubstitute cannot be compared for equality.");
    }
}