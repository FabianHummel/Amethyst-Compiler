using System.ComponentModel;

namespace Amethyst.Utility;

public static class EnumDescriptionExtension
{
    public static string GetDescription<T>(this T value) where T : Enum
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = Attribute.GetCustomAttribute(field!, typeof(DescriptionAttribute)) as DescriptionAttribute;
        return attribute?.Description ?? value.ToString();
    }
    
    public static string GetDefaultValue<T>(this T value) where T : Enum
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = Attribute.GetCustomAttribute(field!, typeof(DefaultValueAttribute)) as DefaultValueAttribute;
        return attribute?.Value?.ToString() ?? value.ToString();
    }
}