using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public abstract class ObjectConstantBase : ConstantValue<Dictionary<string, ConstantValue>>, ISubstitutable
{
    /// <summary>
    /// List of substitutions that need to be made in order to fully create the object.
    /// </summary>
    public List<KeyValuePair<object, RuntimeValue>>? Substitutions { get; init; }
    
    public override int AsInteger => Value.Count;
    
    public override bool AsBoolean => AsInteger > 0;
    
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
}