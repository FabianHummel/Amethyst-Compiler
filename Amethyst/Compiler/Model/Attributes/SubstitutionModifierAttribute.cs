namespace Amethyst.Model;

[AttributeUsage(AttributeTargets.Field)]
public class SubstitutionModifierAttribute : Attribute
{
    public string SubstitutionModifier { get; }

    public SubstitutionModifierAttribute(string substitutionModifier)
    {
        SubstitutionModifier = substitutionModifier;
    }
}