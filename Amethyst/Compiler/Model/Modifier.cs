namespace Amethyst.Model;

public enum Modifier
{
    [Description("[]")]
    [DefaultValue("[]")]
    [SubstitutionModifier("[{0}]")]
    Array,
    [Description("{}")]
    [DefaultValue("{}")]
    [SubstitutionModifier(".data.{0}")]
    Object
}