namespace Amethyst;

public abstract partial class RuntimeValue : AbstractResult
{
    /// <summary>
    /// The memory location where the result is stored at.
    /// Corresponds to a player name inside a scoreboard for numeric values and a path to data storage for complex types.
    /// </summary>
    public required int Location { get; set; }
    
    /// <summary>
    /// Marks the assigned variable as temporary, meaning it can be overwritten by another variable.
    /// Useful for intermediate results that are not needed after the current operation, e.g. within a calculation.
    /// </summary>
    public bool IsTemporary { get; set; }
    
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

    public override RuntimeValue ToRuntimeValue()
    {
        return this;
    }

    protected void AddCode(string code)
    {
        Compiler.AddCode(code);
    }

    protected void AddInitCode(string code)
    {
        Compiler.AddInitCode(code);
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

        var location = ++Compiler.StackPointer;
        
        if (DataType.IsScoreboardType)
        {
            AddCode($"scoreboard players operation {location} amethyst = {Location} amethyst");
        }
        else 
        {
            AddCode($"data modify storage amethyst: {location} set from storage amethyst: {Location}");
        }

        var clone = (RuntimeValue)MemberwiseClone();
        clone.Location = location;
        clone.IsTemporary = true;
        return clone;
    }

    public ConstantSubstitute ToConstantSubstitute()
    {
        return new ConstantSubstitute
        {
            Compiler = Compiler,
            Context = Context,
            Value = this 
        };
    }
}
