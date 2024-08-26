using Amethyst.Model;
using Antlr4.Runtime;

namespace Amethyst;

public abstract partial class AbstractResult
{
    /// <summary>
    /// The data type of the result.
    /// </summary>
    public abstract DataType DataType { get; }
    
    /// <summary>
    /// The memory location where the result is stored at.
    /// Corresponds to a player name inside a scoreboard for numeric values and a path to data storage for complex types.
    /// </summary>
    public string? Location { get; init; }
    
    /// <summary>
    /// Marks the assigned variable as temporary, meaning it can be overwritten by another variable.
    /// Useful for intermediate results that are not needed after the current operation, e.g. within a calculation.
    /// </summary>
    public bool IsTemporary { get; init; }
    
    /// <summary>
    /// The constant value of the result that can be directly incorporated into another result without storing it in a variable.
    /// Literals are always stored as constants first and only turn into variables if they are assigned to a variable. 
    /// </summary>
    public object? ConstantValue { get; init; }
    
    /// <summary>
    /// List of substitutions that need to be made in order to validate the result.
    /// Especially used for array or object based results where a variable's value needs to be copied to an indexed location.
    /// </summary>
    public List<KeyValuePair<object, AbstractResult>>? Substitutions { get; init; }
    
    public required Compiler Compiler { get; init; }
    
    public required ParserRuleContext Context { get; init; }

    /// <summary>
    /// Turns this result into a boolean result by logically converting it to its boolean equivalent.
    /// </summary>
    /// <returns>The boolean result (numeric value of 0 or 1).</returns>
    /// <exception cref="SyntaxException">If the conversion is not possible.</exception>
    public virtual BooleanResult MakeBoolean()
    {
        throw new SyntaxException($"Conversion of {DataType} to {BasicType.Bool} not permitted.", Context);
    }

    /// <summary>
    /// Turns this result into a number result by converting it to its numeric equivalent.
    /// </summary>
    /// <returns>The number result.</returns>
    /// <exception cref="SyntaxException">If the conversion is not possible.</exception>
    public virtual IntegerResult MakeNumber()
    {
        throw new SyntaxException($"Conversion of {DataType} to {BasicType.Int} not permitted.", Context);
    }

    /// <summary>
    /// Converts this constant value to a variable by assigning it a memory location.
    /// </summary>
    /// <returns>The result with a place in memory.</returns>
    /// <exception cref="SyntaxException">If the value is already a variable or cannot be converted to a variable.</exception>
    public virtual AbstractResult MakeVariable()
    {
        if (ConstantValue == null)
        {
            throw new SyntaxException("This value is already a variable.", Context);
        }

        throw new SyntaxException($"Cannot make {DataType} a constant value.", Context);
    }
    
    protected int MemoryLocation
    {
        get => Compiler.MemoryLocation;
        set => Compiler.MemoryLocation = value;
    }

    protected void AddCode(string code)
    {
        Compiler.AddCode(code);
    }

    protected void AddInitCode(string code)
    {
        Compiler.AddInitCode(code);
    }

    protected void SubstituteRecursively(string substitutionModifierPrefix = "")
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
}
