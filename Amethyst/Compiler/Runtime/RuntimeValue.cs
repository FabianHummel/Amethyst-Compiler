using Amethyst.Model;

namespace Amethyst;

public abstract partial class RuntimeValue : AbstractResult
{
    /// <summary>
    /// The memory location where the result is stored at.
    /// Corresponds to a player name inside a scoreboard for numeric values and a path to data storage for complex types.
    /// </summary>
    public required string Location { get; set; }
    
    /// <summary>
    /// Marks the assigned variable as temporary, meaning it can be overwritten by another variable.
    /// Useful for intermediate results that are not needed after the current operation, e.g. within a calculation.
    /// </summary>
    public bool IsTemporary { get; set; }
    
    /// <summary>
    /// List of substitutions that need to be made in order to validate the result.
    /// Especially used for array or object based results where a variable's value needs to be copied to an indexed location.
    /// </summary>
    public List<KeyValuePair<object, RuntimeValue>>? Substitutions { get; init; }

    /// <summary>
    /// Turns this result into a boolean result by logically converting it to its boolean equivalent.
    /// </summary>
    /// <returns>The boolean result (numeric value of 0 or 1).</returns>
    public abstract BooleanResult MakeBoolean();

    /// <summary>
    /// Turns this result into a number result by converting it to its numeric equivalent.
    /// </summary>
    /// <returns>The number result.</returns>
    public abstract IntegerResult MakeInteger();

    protected void AddCode(string code)
    {
        Compiler.AddCode(code);
    }

    protected void AddInitCode(string code)
    {
        Compiler.AddInitCode(code);
    }

    public void SubstituteRecursively(string substitutionModifierPrefix = "")
    {
        if (Substitutions != null)
        {
            foreach (var (i, element) in Substitutions)
            {
                var substitutionModifier = substitutionModifierPrefix + DataType.GetSubstitutionModifier(i);
                
                if (element.Substitutions != null)
                {
                    element.SubstituteRecursively(substitutionModifier);
                    continue;
                }
                
                if (element.DataType.IsStorageType)
                {
                    AddCode($"data modify storage amethyst: {substitutionModifier} set from storage amethyst: {element.Location}");
                }
                else if (element.DataType.IsScoreboardType)
                {
                    AddCode($"execute store result storage amethyst: {substitutionModifier} {element.DataType.StorageModifier} run scoreboard players get {element.Location} amethyst");
                }
            }
        }
    }

    /// <summary>
    /// Ensures that the current value is backed up in a temporary variable, so it can be freely modified.
    /// </summary>
    /// <returns>A backup of the current value.</returns>
    public RuntimeValue EnsureBackedUp()
    {
        if (IsTemporary)
        {
            return this;
        }
        
        var location = (++Compiler.StackPointer).ToString();
        
        if (DataType.IsStorageType)
        {
            AddCode($"data modify storage amethyst: {location} set from storage amethyst: {Location}");
        }
        else if (DataType.IsScoreboardType)
        {
            AddCode($"scoreboard players operation {location} amethyst = {Location} amethyst");
        }
        else
        {
            throw new SyntaxException($"Cannot back up {DataType.BasicType} type.", Context);
        }

        var clone = (RuntimeValue)MemberwiseClone();
        clone.Location = location;
        clone.IsTemporary = true;
        return clone;
    }
}
