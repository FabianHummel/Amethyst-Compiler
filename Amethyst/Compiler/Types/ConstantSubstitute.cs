namespace Amethyst;

public class ConstantSubstitute : AbstractValue, IConstantValue<IRuntimeValue>
{
    public required IRuntimeValue Value { get; init; }
    
    public override AbstractDatatype Datatype => Value.Datatype;
    
    public int AsInteger => throw new InvalidOperationException("Constant substitutes cannot be converted to an integer value.");
    
    public bool AsBoolean => throw new InvalidOperationException("Constant substitutes cannot be converted to a boolean value.");
    
    public double AsDouble => throw new InvalidOperationException("Constant substitutes cannot be converted to a double value.");

    public IRuntimeValue ToRuntimeValue()
    {
        throw new InvalidOperationException("Constant substitutes cannot be converted to a runtime value.");
    }

    public string ToNbtString()
    {
        return Value.Datatype.DefaultValue;
    }

    public string ToTextComponent()
    {
        throw new InvalidOperationException("Constant substitutes cannot be converted to a text component.");
    }

    public bool Equals(IConstantValue? other)
    {
        throw new InvalidOperationException("Constant substitutes cannot be compared for equality.");
    }

    public override string ToTargetSelectorString()
    {
        return Value.ToTargetSelectorString();
    }
}