using Amethyst.Model;
using Antlr4.Runtime;

namespace Amethyst;

public abstract partial class AbstractResult
{
    public abstract DataType DataType { get; }
    
    public string? Location { get; init; }
    public required Compiler Compiler { get; init; }
    public required ParserRuleContext Context { get; init; }
    public bool IsTemporary { get; init; }
    public object? ConstantValue { get; init; }
    public List<KeyValuePair<object, AbstractResult>>? Substitutions { get; init; }

    public virtual BooleanResult MakeBoolean() => throw new SyntaxException($"Conversion of {DataType} to {BasicType.Bool} not permitted.", Context);

    public virtual IntegerResult MakeNumber()
    {
        throw new SyntaxException($"Conversion of {DataType} to {BasicType.Int} not permitted.", Context);
    }

    public virtual AbstractResult MakeVariable()
    {
        throw new SyntaxException($"Cannot make {DataType} a constant value.", Context);
    }
    
    protected virtual string GetSubstitutionModifier(object index)
    {
        throw new SyntaxException($"Cannot get substitution modifier for {DataType}.", Context);
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
                if (element.Substitutions != null)
                {
                    element.SubstituteRecursively(substitutionModifierPrefix + GetSubstitutionModifier(i));
                    continue;
                }
                
                if (element.DataType.IsStorageType)
                {
                    AddCode($"data modify storage amethyst: {substitutionModifierPrefix}{GetSubstitutionModifier(i)} set from storage amethyst: {element.Location}");
                }
                else if (element.DataType.IsScoreboardType)
                {
                    AddCode($"execute store result storage amethyst: {substitutionModifierPrefix}{GetSubstitutionModifier(i)} {element.DataType.StorageModifier} run scoreboard players get {element.Location} amethyst");
                }
            }
        }
    }
}
