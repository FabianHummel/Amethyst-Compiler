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
                
                if (element.DataType.IsScoreboardType)
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
}