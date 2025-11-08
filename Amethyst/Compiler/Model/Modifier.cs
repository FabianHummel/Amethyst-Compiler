namespace Amethyst.Model;

/// <summary>Collection of datatype modifiers.</summary>
/// <example><p></p><c>int[]</c> is a <see cref="IntegerDatatype" /> primitive type with an
/// <see cref="Array" /> modifier that makes it an integer array.</example>
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