namespace Amethyst.Model;

/// <summary>Collection of primitive Amethyst data types. Combined with a <see cref="Modifier" /> they
/// result in a fully qualified <see cref="AbstractDatatype" /> (with some exceptions like
/// <see cref="DecimalDatatype" />).</summary>
/// <seealso cref="Modifier" />
public enum BasicType
{
    [Description("int")]
    [DefaultValue("0")]
    Int,
    [Description("dec")]
    [DefaultValue("0d")]
    Dec,
    [Description("string")]
    [DefaultValue("\"\"")]
    String,
    [Description("bool")]
    [DefaultValue("0b")]
    Bool,
    [Description("array")]
    [DefaultValue("[]")]
    [SubstitutionModifier("[{0}]")]
    Array,
    [Description("object")]
    [DefaultValue("{}")]
    [SubstitutionModifier(".data.{0}")]
    Object,
    [Description("raw")]
    Raw,
    [Description("entity")]
    Entity
}