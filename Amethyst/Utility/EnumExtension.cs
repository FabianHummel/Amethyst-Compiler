
using Amethyst.Attributes;
using Amethyst.Model;

namespace Amethyst.Utility;

public static class EnumExtension
{
    public static string GetDescription<T>(this T value) where T : Enum
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = Attribute.GetCustomAttribute(field!, typeof(DescriptionAttribute)) as DescriptionAttribute;
        return attribute?.Description ?? value.ToString();
    }

    public static object GetDefaultValue<T>(this T value) where T : Enum
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = Attribute.GetCustomAttribute(field!, typeof(DefaultValueAttribute)) as DefaultValueAttribute;
        return attribute?.DefaultValue ?? value;
    }
    
    public static string GetSubstitutionModifier<T>(this T value) where T : Enum
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = Attribute.GetCustomAttribute(field!, typeof(SubstitutionModifierAttribute)) as SubstitutionModifierAttribute;
        return attribute?.SubstitutionModifier ?? string.Empty;
    }

    public static string GetMcfOperatorSymbol(this Operator op)
    {
        var field = op.GetType().GetField(op.ToString());
        var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
        return attribute!.Description;
    }
}