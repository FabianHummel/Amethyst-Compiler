using Amethyst.Model.Attributes;

namespace Amethyst.Utility;

public static class EnumExtension
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
        return attribute?.DefaultValue ?? value.ToString();
    }
    
    public static string GetSubstitutionModifier<T>(this T value) where T : Enum
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = Attribute.GetCustomAttribute(field!, typeof(SubstitutionModifierAttribute)) as SubstitutionModifierAttribute;
        return attribute?.SubstitutionModifier ?? string.Empty;
    }

    public static string GetMcfOperatorSymbol(this Enum op)
    {
        var field = op.GetType().GetField(op.ToString());
        var attribute = (McfOperatorAttribute)Attribute.GetCustomAttribute(field, typeof(McfOperatorAttribute));
        return attribute!.Operator;
    }
    
    public static string GetAmethystOperatorSymbol(this Enum op)
    {
        var field = op.GetType().GetField(op.ToString());
        var attribute = (AmethystOperatorAttribute)Attribute.GetCustomAttribute(field, typeof(AmethystOperatorAttribute));
        return attribute!.Operator;
    }
    
    public static T GetEnumFromMcfOperator<T>(string mcfOperator) where T : Enum
    {
        foreach (var field in typeof(T).GetFields())
        {
            if (Attribute.GetCustomAttribute(field, typeof(McfOperatorAttribute)) is McfOperatorAttribute attribute && attribute.Operator == mcfOperator)
            {
                return (T)field.GetValue(null)!;
            }
        }
        throw new ArgumentException($"No enum value found for Mcf operator '{mcfOperator}' in enum '{typeof(T).Name}'.");
    }
}