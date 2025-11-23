using Amethyst.Model;

namespace Amethyst;

/// <summary>
///     <p>A constant substitute is a constant value that is used in place of runtime values during
///     creation of complex values such as arrays or objects. For example, given an array that is made
///     up of three constant values (<c>1</c>, <c>2</c> and <c>3</c>) and a variable with a value that
///     is only known during runtime, it is created initialized with four elements in total, three of
///     which are embedded directly and the last one with a <see cref="ConstantSubstitute" />. Later,
///     each individual <see cref="ConstantSubstitute" /> is substituted with the actual value.</p>
///     <p>Each basic amethyst type (<see cref="BasicType" />) has a way of representing a placeholder
///     value that is used in place of the actual value using the <see cref="DefaultValueAttribute" />.</p>
///     <p>Note that constant substitutes are actually never used during runtime code evaluation
///     because they immediately disappear after the successful creation of the enclosing value.</p>
///     <p><inheritdoc cref="AbstractValue" /></p> <p><inheritdoc cref="IConstantValue" /></p>
/// </summary>
public class ConstantSubstitute : AbstractValue, IConstantValue<IRuntimeValue>
{
    public required IRuntimeValue Value { get; init; }
    
    public override AbstractDatatype Datatype => Value.Datatype;
    
    public int AsInteger => throw new InvalidOperationException("Constant substitutes cannot be converted to an integer value.");
    
    public bool AsBoolean => throw new InvalidOperationException("Constant substitutes cannot be converted to a boolean value.");
    
    public double AsDouble => throw new InvalidOperationException("Constant substitutes cannot be converted to a double value.");
    
    public string AsString => throw new InvalidOperationException("Constant substitutes cannot be converted to a string value.");

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
        return Value.Location.ToMacroPlaceholder();
    }
}