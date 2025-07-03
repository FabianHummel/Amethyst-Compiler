namespace Amethyst;

public abstract class ConstantValue : AbstractResult
{
    public abstract int AsInteger { get; }
    
    public abstract bool AsBoolean { get; }

    /// <summary>
    /// Converts this constant value to a variable by assigning it a fixed memory location.
    /// </summary>
    /// <returns>The result with a place in memory.</returns>
    public abstract RuntimeValue ToRuntimeValue();
    
    /// <summary>
    /// Converts this constant value to a string that can be used in NBT data.
    /// </summary>
    /// <returns>The NBT string representation of this constant value.</returns>
    public abstract string ToNbtString();
}

public abstract class ConstantValue<T> : ConstantValue
{
    /// <summary>
    /// The constant value of the result that can be directly incorporated into another result without storing it in a variable.
    /// Literals are always stored as constants first and only turn into variables if they are assigned to a variable. 
    /// </summary>
    public required T Value { get; init; }
}