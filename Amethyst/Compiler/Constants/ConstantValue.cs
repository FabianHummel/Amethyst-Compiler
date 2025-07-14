namespace Amethyst;

public abstract class ConstantValue : AbstractResult, IEquatable<ConstantValue>
{
    public abstract int AsInteger { get; }

    public abstract bool AsBoolean { get; }

    /// <summary>
    /// Converts this constant value to a string that can be used in NBT data.
    /// </summary>
    /// <returns>The NBT string representation of this constant value.</returns>
    public abstract string ToNbtString();
    
    /// <summary>
    /// Converts this result to a string that can be used to display the value in Minecraft.
    /// </summary>
    /// <returns>A JSON text component that can be used with '/tellraw'.</returns>
    public abstract string ToTextComponent();

    /// <summary>
    /// Checks if two constant values are value equal.
    /// </summary>
    /// <param name="other">The other constant value to compare with.</param>
    public abstract bool Equals(ConstantValue? other);
}

public abstract class ConstantValue<T> : ConstantValue
{
    /// <summary>
    /// The constant value of the result that can be directly incorporated into another result without storing it in a variable.
    /// Literals are always stored as constants first and only turn into variables if they are assigned to a variable. 
    /// </summary>
    public required T Value { get; init; }
}