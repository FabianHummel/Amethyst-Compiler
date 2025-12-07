using System.Diagnostics.CodeAnalysis;

namespace Amethyst;

public abstract class AbstractConstantArray : AbstractArray, IConstantValue<IConstantValue[]>, ISubstitutable, IIndexable, IMemberAccess
{
    public required IConstantValue[] Value { get; init; }

    /// <summary>List of substitutions that need to be made in order to fully create the object.</summary>
    public List<KeyValuePair<object, IRuntimeValue>>? Substitutions { get; init; }
    
    public int AsInteger => Value.Length;
    
    public bool AsBoolean => AsInteger > 0;
    
    public double AsDouble => AsInteger;
    
    public string AsString => $"[{string.Join(",", Value.Select(v => v.AsString))}]";

    public abstract IRuntimeValue ToRuntimeValue();

    public void SubstituteRecursively(string substitutionModifierPrefix = "")
    {
        for (var i = 0; i < Value.Length; i++)
        {
            var constantValue = Value[i];
            
            if (constantValue is ISubstitutable substitutable)
            {
                var substitutionModifier = substitutionModifierPrefix + Datatype.GetSubstitutionModifier(i);
                
                substitutable.SubstituteRecursively(substitutionModifier);
            }
        }

        if (Substitutions != null)
        {
            foreach (var (i, element) in Substitutions)
            {
                var substitutionModifier = substitutionModifierPrefix + Datatype.GetSubstitutionModifier(i);
                
                if (element.Datatype.IsScoreboardType(out var scoreboardDatatype))
                {
                    this.AddCode($"execute store result storage {substitutionModifier} {scoreboardDatatype.StorageModifier} run scoreboard players get {element.Location}");
                }
                else
                {
                    this.AddCode($"data modify storage {substitutionModifier} set from storage {element.Location}");
                }
            }
        }
    }
    
    public string ToNbtString()
    {
        return $"[{string.Join(",", Value.Select(v => v.ToNbtString()))}]";
    }

    public string ToTextComponent()
    {
        if (Value.Length == 0)
        {
            return "\"[]\"";
        }
        
        var content = string.Join(""",", ",""", Value.Select(v => v.ToTextComponent()));
        return $"""["[",{content},"]"]""";
    }

    public bool Equals(IConstantValue? other)
    {
        if (other is not AbstractConstantArray arrayConstantBase)
        {
            return false;
        }
        
        return Value.SequenceEqual(arrayConstantBase.Value);
    }

    public AbstractValue GetIndex(AbstractValue index)
    {
        if (index is ConstantInteger integerConstant)
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

            return (AbstractValue)Value[value];
        }

        if (index is RuntimeInteger integerResult)
        {
            // if the index is a runtime value, convert the array constant
            // to a runtime array and continue the evaluation.
            var runtimeValue = (AbstractRuntimeArray)ToRuntimeValue();

            return runtimeValue.GetIndex(integerResult);
        }
        
        throw new SyntaxException("Expected integer index.", index.Context);
    }

    public override string ToTargetSelectorString()
    {
        throw new SyntaxException("Arrays cannot be used as a value in target selectors.", Context);
    }

    public AbstractValue? GetMember(string memberName) => memberName switch
    {
        "length" => new ConstantInteger
        {
            Compiler = Compiler,
            Context = Context,
            Value = Value.Length
        },
        _ => null
    };

    public static bool TryParse(IEnumerable<object> enumerable, [NotNullWhen(true)] out AbstractConstantArray? result)
    {
        var elements = new List<IConstantValue>();
        
        foreach (var item in enumerable)
        {
            if (!IConstantValue.TryParse(item, out var constantValue))
            {
                result = null;
                return false;
            }

            elements.Add(constantValue);
        }
        
        result = new ConstantDynamicArray
        {
            Context = null!,
            Compiler = null!,
            Value = elements.ToArray()
        };
        return true;
    }
}