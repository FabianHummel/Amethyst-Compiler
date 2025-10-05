using System.Diagnostics.CodeAnalysis;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public abstract class AbstractConstantObject : AbstractObject, IConstantValue<Dictionary<string, IConstantValue>>, ISubstitutable, IIndexable, IMemberAccess
{
    public required Dictionary<string, IConstantValue> Value { get; init; }
    
    /// <summary>
    /// List of substitutions that need to be made in order to fully create the object.
    /// </summary>
    public List<KeyValuePair<object, IRuntimeValue>>? Substitutions { get; init; }
    
    public int AsInteger => Value.Count;
    
    public bool AsBoolean => AsInteger > 0;
    
    public abstract IRuntimeValue ToRuntimeValue();

    public void SubstituteRecursively(Compiler compiler, string substitutionModifierPrefix = "")
    {
        foreach (var (key, constantValue) in Value)
        {
            if (constantValue is ISubstitutable substitutable)
            {
                var substitutionModifier = substitutionModifierPrefix + DataType.GetSubstitutionModifier(key);
                
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
    
    public string ToNbtString()
    {
        return $"{{keys:[{string.Join(",", Value.Keys.Select(key => key.ToNbtString()))}]," +
               $"data:{{{string.Join(',', Value.Select(kvp => $"{kvp.Key.ToNbtString()}:{kvp.Value.ToNbtString()}"))}}}}}";
    }

    public string ToTextComponent()
    {
        if (Value.Count == 0)
        {
            return "\"{}\"";
        }

        var content = string.Join(""",", ",""", Value.Select(kvp => 
            $$"""{"text":"{{kvp.Key}}","color":"aqua"},": ",{{kvp.Value.ToTextComponent()}}"""));
        return $$"""["{",{{content}},"}"]""";
    }

    public bool Equals(IConstantValue? other)
    { 
        if (other is not AbstractConstantObject objectConstantBase)
        {
            return false;
        }

        if (Value.Count != objectConstantBase.Value.Count)
        {
            return false;
        }

        foreach (var (key, value) in Value)
        {
            if (!objectConstantBase.Value.TryGetValue(key, out var otherValue) || !value.Equals(otherValue))
            {
                return false;
            }
        }

        return true;
    }

    public AbstractValue GetIndex(AbstractValue index)
    {
        if (index is ConstantString stringConstant)
        {
            if (Value.TryGetValue(stringConstant.Value, out var value))
            {
                return (AbstractValue)value;
            }

            throw new SyntaxException($"Key '{stringConstant.Value}' not found in object.", index.Context);
        }

        if (index is RuntimeString stringResult)
        {
            // if the index is a runtime value, convert the object constant
            // to a runtime object and continue the evaluation.
            var runtimeValue = (AbstractRuntimeObject)ToRuntimeValue();

            return runtimeValue.GetIndex(stringResult);
        }

        throw new SyntaxException("Expected string index.", index.Context);
    }
    
    private IEnumerable<ConstantString> GetKeysAsStringConstants()
    {
        return Value.Keys.Select(key => new ConstantString
        {
            Compiler = Compiler,
            Context = Context,
            Value = key
        });
    }
    
    public override string ToTargetSelectorString()
    {
        throw new SyntaxException("Objects cannot be used as a value in target selectors.", Context);
    }

    public AbstractValue? GetMember(string memberName)
    {
        if (Value.TryGetValue(memberName, out var value))
        {
            return (AbstractValue)value;
        }
        
        return memberName switch
        {
            "count" => new ConstantInteger
            {
                Compiler = Compiler,
                Context = Context,
                Value = Value.Count
            },
            "keys" => new ConstantStaticArray
            {
                Compiler = Compiler,
                Context = Context,
                BasicType = BasicType.String,
                Value = GetKeysAsStringConstants().Cast<IConstantValue>().ToArray()
            },
            _ => null
        };
    }

    public static bool TryParse(IDictionary<string, object> dictionary, [NotNullWhen(true)] out AbstractConstantObject? result)
    {
        var pairs = new Dictionary<string, IConstantValue>();
        
        foreach (var (key, value) in dictionary)
        {
            if (!IConstantValue.TryParse(value, out var constantValue))
            {
                result = null;
                return false;
            }

            pairs.Add(key, constantValue);
        }
        
        result = new ConstantDynamicObject
        {
            Context = null!,
            Compiler = null!,
            Value = pairs
        };
        return true;
    }
}