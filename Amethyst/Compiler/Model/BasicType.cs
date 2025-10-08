namespace Amethyst.Model;

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
    [Description("unknown")]
    Unknown,
    [Description("entity")]
    Entity
}