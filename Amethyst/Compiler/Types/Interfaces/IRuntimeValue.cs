using Amethyst.Model;
using Antlr4.Runtime;

namespace Amethyst;

public interface IRuntimeValue
{
    /// <summary>The memory location where the result is stored at. Corresponds to a player name inside a
    /// scoreboard for numeric values and a path to data storage for complex types.</summary>
    Location Location { get; }

    /// <summary>Marks the assigned variable as temporary, meaning it can be overwritten by another
    /// variable. Useful for intermediate results that are not needed after the current operation, e.g.
    /// within a calculation.</summary>
    bool IsTemporary { get; }

    ParserRuleContext Context { get; }
    
    Compiler Compiler { get; }
    
    AbstractDatatype Datatype { get; }

    /// <summary>Turns this result into a boolean result by logically converting it to its boolean
    /// equivalent.</summary>
    /// <returns>The boolean result (numeric value of 0 or 1).</returns>
    AbstractBoolean MakeBoolean();
    
    IRuntimeValue WithLocation(Location newLocation, bool temporary = true);

    public sealed Location NextFreeLocation(DataLocation dataLocation)
    {
        if (IsTemporary)
        {
            return Location;
        }
        
        var numericLocation = ++Compiler.StackPointer;
        return new Location
        {
            DataLocation = dataLocation,
            Name = numericLocation.ToString()
        };
    }

    /// <summary>Ensures that the current value is backed up in a temporary variable, so it can be freely
    /// modified.</summary>
    /// <returns>A backup of the current value.</returns>
    public sealed IRuntimeValue EnsureBackedUp()
    {
        if (IsTemporary)
        {
            return this;
        }
    
        var location = NextFreeLocation(Location.DataLocation);
        
        if (Location.DataLocation == DataLocation.Scoreboard)
        {
            Compiler.AddCode($"scoreboard players operation {location} = {Location}");
        }
        else
        {
            Compiler.AddCode($"data modify storage {location} set from storage {Location}");
        }

        return WithLocation(location, temporary: true);
    }

    /// <summary>Converts this runtime value into a constant substitute that can be used in places where a
    /// constant is required.</summary>
    /// <returns>A constant value representing this runtime value.</returns>
    public sealed ConstantSubstitute AsConstantSubstitute => new()
    {
        Compiler = Compiler,
        Context = Context,
        Value = this 
    };

    /// <summary>Returns this value's representation as a target selector string. In this case only for use
    /// in macros.</summary>
    /// <returns>The target selector string.</returns>
    public sealed string ToTargetSelectorString()
    {
        var storageValue = EnsureInStorage();
        return $"$({storageValue.Location.Name})";
    }

    /// <summary>Ensures that this value is stored in storage, likely to be able to use it as macro input.</summary>
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
