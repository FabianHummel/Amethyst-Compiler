namespace Amethyst.Model;

/// <summary>Represents a valid NBT placeholder value that is used in place of the actual runtime value
/// that is substituted when constructing a complex type such as an array or object.</summary>
[AttributeUsage(AttributeTargets.Field)]
public class SubstitutionModifierAttribute : Attribute
{
    public string SubstitutionModifier { get; }

    public SubstitutionModifierAttribute(string substitutionModifier)
    {
        SubstitutionModifier = substitutionModifier;
    }
}