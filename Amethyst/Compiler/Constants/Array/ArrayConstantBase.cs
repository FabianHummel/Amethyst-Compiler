using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public abstract class ArrayConstantBase : ConstantValue<ConstantValue[]>, ISubstitutable, IIndexable, IMemberAccess
{
    /// <summary>
    /// List of substitutions that need to be made in order to fully create the object.
    /// </summary>
    public List<KeyValuePair<object, RuntimeValue>>? Substitutions { get; init; }
    
    public override int AsInteger => Value.Length;
    
    public override bool AsBoolean => AsInteger > 0;
    
    public abstract override ArrayBase ToRuntimeValue();
    
    public void SubstituteRecursively(Compiler compiler, string substitutionModifierPrefix = "")
    {
        for (var i = 0; i < Value.Length; i++)
        {
            var constantValue = Value[i];
            
            if (constantValue is ISubstitutable substitutable)
            {
                var substitutionModifier = substitutionModifierPrefix + DataType.GetSubstitutionModifier(i);
                
                substitutable.SubstituteRecursively(compiler, substitutionModifier);
            }
        }

        if (Substitutions != null)
        {
            foreach (var (i, element) in Substitutions)
            {
                var substitutionModifier = substitutionModifierPrefix + DataType.GetSubstitutionModifier(i);
                
                if (element.DataType.Location == DataLocation.Scoreboard)
                {
                    compiler.AddCode($"execute store result storage amethyst: {substitutionModifier} {element.DataType.StorageModifier} run scoreboard players get {element.Location} amethyst");
                }
                else
                {
                    compiler.AddCode($"data modify storage amethyst: {substitutionModifier} set from storage amethyst: {element.Location}");
                }
            }
        }
    }
    
    public override string ToNbtString()
    {
        return $"[{string.Join(",", Value.Select(v => v.ToNbtString()))}]";
    }

    public override string ToTextComponent()
    {
        if (Value.Length == 0)
        {
            return "\"[]\"";
        }
        
        var content = string.Join(""",", ",""", Value.Select(v => v.ToTextComponent()));
        return $"""["[",{content},"]"]""";
    }

    public override bool Equals(ConstantValue? other)
    {
        if (other is not ArrayConstantBase arrayConstantBase)
        {
            return false;
        }
        
        return Value.SequenceEqual(arrayConstantBase.Value);
    }

    public AbstractResult GetIndex(AbstractResult index)
    {
        if (index is IntegerConstant integerConstant)
        {
            var value = integerConstant.Value;

            if (value < -Value.Length || value >= Value.Length)
            {
                throw new SyntaxException($"Index {value} out of bounds for array of length {Value.Length}.", index.Context);
            }

            if (value < 0)
            {
                value += Value.Length;
            }

            return Value[value];
        }

        if (index is IntegerResult integerResult)
        {
            // if the index is a runtime value, convert the array constant
            // to a runtime array and continue the evaluation.
            var runtimeValue = ToRuntimeValue();

            return runtimeValue.GetIndex(integerResult);
        }
        
        throw new SyntaxException("Expected integer index.", index.Context);
    }

    public override string ToTargetSelectorString()
    {
        throw new UnreachableException("Array cannot be converted to a target selector's value.");
    }

    public AbstractResult GetMember(string memberName)
    {
        return memberName switch
        {
            "length" => new IntegerConstant
            {
                Compiler = Compiler,
                Context = Context,
                Value = Value.Length,
            },
            _ => throw new SemanticException($"Array does not have a member named '{memberName}'.")
        };
    }
}