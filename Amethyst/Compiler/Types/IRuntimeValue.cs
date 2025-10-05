using Amethyst.Model;
using Amethyst.Utility;
using Antlr4.Runtime;

namespace Amethyst;

[ForwardDefaultInterfaceMethods]
public interface IRuntimeValue
{
    /// <summary>
    /// The memory location where the result is stored at.
    /// Corresponds to a player name inside a scoreboard for numeric values and a path to data storage for complex types.
    /// </summary>
    int Location { get; set; }
    
    /// <summary>
    /// Marks the assigned variable as temporary, meaning it can be overwritten by another variable.
    /// Useful for intermediate results that are not needed after the current operation, e.g. within a calculation.
    /// </summary>
    bool IsTemporary { get; set; }

    ParserRuleContext Context { get; }
    
    Compiler Compiler { get; }
    
    DataType DataType { get; }
    
    /// <summary>
    /// Turns this result into a boolean result by logically converting it to its boolean equivalent.
    /// </summary>
    /// <returns>The boolean result (numeric value of 0 or 1).</returns>
    AbstractBoolean MakeBoolean();

    public sealed int NextFreeLocation()
    {
        if (IsTemporary)
        {
            return Location;
        }
        
        return ++Compiler.StackPointer;
    }
    
    /// <summary>
    /// Ensures that the current value is backed up in a temporary variable, so it can be freely modified.
    /// </summary>
    /// <returns>A backup of the current value.</returns>
    public sealed IRuntimeValue EnsureBackedUp()
    {
        if (IsTemporary)
        {
            return this;
        }
    
        var location = NextFreeLocation();
        
        if (DataType.Location == DataLocation.Scoreboard)
        {
            Compiler.AddCode($"scoreboard players operation {location} amethyst = {Location} amethyst");
        }
        else
        {
            Compiler.AddCode($"data modify storage amethyst: {location} set from storage amethyst: {Location}");
        }

        var clone = (IRuntimeValue)((AbstractValue)this).Clone();
        clone.Location = location;
        clone.IsTemporary = true;
        return clone;
    }
    
    /// <summary>
    /// Converts this runtime value into a constant substitute that can be used in places where a constant is required.
    /// </summary>
    /// <returns>A constant value representing this runtime value.</returns>
    public sealed ConstantSubstitute AsConstantSubstitute => new()
    {
        Compiler = Compiler,
        Context = Context,
        Value = this 
    };

    /// <summary>
    /// Returns this value's representation as a target selector string. In this case only for use in macros.
    /// </summary>
    /// <returns>The target selector string.</returns>
    public sealed string ToTargetSelectorString()
    {
        var storageValue = EnsureInStorage().EnsureBackedUp();
        return $"$({storageValue.Location})";
    }
    
    /// <summary>
    /// Ensures that this value is stored in storage, likely to be able to use it as macro input.
    /// </summary>
    /// <returns>A runtime value pointing to a storage location</returns>
    private IRuntimeValue EnsureInStorage()
    {
        if (this is AbstractNumericValue numericValue)
        {
            return numericValue.EnsureInStorage();
        }
    
        return this;
    }
}
