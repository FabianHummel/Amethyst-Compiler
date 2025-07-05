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
        throw new UnreachableException();
    }

    public override string ToNbtString()
    {
        return Value.DataType.DefaultValue;
    }
}