using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public abstract class ObjectConstantBase : ConstantValue<Dictionary<string, ConstantValue>>, 
    ISubstitutable, 
    IIndexable,
    IMemberAccess
{
    /// <summary>
    /// List of substitutions that need to be made in order to fully create the object.
    /// </summary>
    public List<KeyValuePair<object, RuntimeValue>>? Substitutions { get; init; }
    
    public override int AsInteger => Value.Count;
    
    public override bool AsBoolean => AsInteger > 0;
    
    public abstract override ObjectBase ToRuntimeValue();
    
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
    
    public override string ToNbtString()
    {
        return $"{{keys:[{string.Join(",", Value.Keys.Select(key => key.ToNbtString()))}]," +
               $"data:{{{string.Join(',', Value.Select(kvp => $"{kvp.Key.ToNbtString()}:{kvp.Value.ToNbtString()}"))}}}}}";
    }

    public override string ToTextComponent()
    {
        if (Value.Count == 0)
        {
            return "\"{}\"";
        }

        var content = string.Join(""",", ",""", Value.Select(kvp => 
            $$"""{"text":"{{kvp.Key}}","color":"aqua"},": ",{{kvp.Value.ToTextComponent()}}"""));
        return $$"""["{",{{content}},"}"]""";
    }

    public override bool Equals(ConstantValue? other)
    { 
        if (other is not ObjectConstantBase objectConstantBase)
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

    public AbstractResult GetIndex(AbstractResult index)
    {
        if (index is StringConstant stringConstant)
        {
            if (Value.TryGetValue(stringConstant.Value, out var value))
            {
                return value;
            }

            throw new SyntaxException($"Key '{stringConstant.Value}' not found in object.", index.Context);
        }

        if (index is StringResult stringResult)
        {
            // if the index is a runtime value, convert the object constant
            // to a runtime object and continue the evaluation.
            var runtimeValue = ToRuntimeValue();

            return runtimeValue.GetIndex(stringResult);
        }

        throw new SyntaxException("Expected string index.", index.Context);
    }
    
    private IEnumerable<StringConstant> GetKeysAsStringConstants()
    {
        return Value.Keys.Select(key => new StringConstant
        {
            Compiler = Compiler,
            Context = Context,
            Value = key
        });
    }
    
    public override string ToTargetSelectorString()
    {
        throw new UnreachableException("Object cannot be converted to a target selector's value.");
    }

    public AbstractResult GetMember(string memberName, AmethystParser.IdentifierContext identifierContext)
    {
        if (Value.TryGetValue(memberName, out var value))
        {
            return value;
        }
        
        return memberName switch
        {
            "count" => new IntegerConstant
            {
                Compiler = Compiler,
                Context = Context,
                Value = Value.Count
            },
            "keys" => new StaticArrayConstant
            {
                Compiler = Compiler,
                Context = Context,
                BasicType = BasicType.String,
                Value = GetKeysAsStringConstants().Cast<ConstantValue>().ToArray()
            },
            _ => throw new SyntaxException($"Member '{memberName}' not found in object.", identifierContext)
        };
    }
}