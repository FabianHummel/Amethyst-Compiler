using Amethyst.Model;

namespace Amethyst.Utility;

/// <summary>Extension methods for working with Enums and their associated attributes.</summary>
public static class EnumExtension
{
    /// <summary>Gets the description of an enum value from its DescriptionAttribute.</summary>
    /// <param name="value">The enum value.</param>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <returns>The description string if the attribute is present; otherwise, the enum value as a string.</returns>
    public static string GetDescription<T>(this T value) where T : Enum
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = Attribute.GetCustomAttribute(field!, typeof(DescriptionAttribute)) as DescriptionAttribute;
        return attribute?.Description ?? value.ToString();
    }

    /// <summary>Gets the default value of an enum value from its DefaultValueAttribute.</summary>
    /// <param name="value">The enum value.</param>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <returns>The default value string if the attribute is present; otherwise, the enum value as a
    /// string.</returns>
    public static string GetDefaultValue<T>(this T value) where T : Enum
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = Attribute.GetCustomAttribute(field!, typeof(DefaultValueAttribute)) as DefaultValueAttribute;
        return attribute?.DefaultValue ?? value.ToString();
    }

    /// <summary>Gets the substitution modifier of an enum value from its SubstitutionModifierAttribute.</summary>
    /// <param name="value">The enum value.</param>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <returns>>The substitution modifier string if the attribute is present; otherwise, an empty string.</returns>
    public static string GetSubstitutionModifier<T>(this T value) where T : Enum
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = Attribute.GetCustomAttribute(field!, typeof(SubstitutionModifierAttribute)) as SubstitutionModifierAttribute;
        return attribute?.SubstitutionModifier ?? string.Empty;
    }

    /// <summary>Gets the Mcf operator symbol associated with the enum value.</summary>
    /// <param name="op">The enum value representing the operator.</param>
    /// <returns>The Mcf operator symbol as a string.</returns>
    public static string GetMcfOperatorSymbol(this Enum op)
    {
        var field = op.GetType().GetField(op.ToString());
        var attribute = (McfTokenAttribute)Attribute.GetCustomAttribute(field!, typeof(McfTokenAttribute))!;
        return attribute.Token;
    }

    /// <summary>Gets the Amethyst operator symbol associated with the enum value.</summary>
    /// <param name="op">The enum value representing the operator.</param>
    /// <returns>The Amethyst operator symbol as a string.</returns>
    public static string GetAmethystOperatorSymbol(this Enum op)
    {
        var field = op.GetType().GetField(op.ToString());
        var attribute = (AmethystOperatorAttribute)Attribute.GetCustomAttribute(field!, typeof(AmethystOperatorAttribute))!;
        return attribute.Operator;
    }

    /// <summary>Gets the enum value corresponding to the given Mcf operator symbol.</summary>
    /// <param name="mcfOperator">The Mcf operator symbol.</param>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <returns>The enum value associated with the Mcf operator symbol.</returns>
    /// <exception cref="ArgumentException">Thrown if no matching enum value is found.</exception>
    public static T GetEnumFromMcfOperator<T>(string mcfOperator) where T : Enum
    {
        foreach (var field in typeof(T).GetFields())
        {
            if (Attribute.GetCustomAttribute(field, typeof(McfTokenAttribute)) is McfTokenAttribute attribute && attribute.Token == mcfOperator)
            {
                return (T)field.GetValue(null)!;
            }
        }
        throw new ArgumentException($"No enum value found for Mcf operator '{mcfOperator}' in enum '{typeof(T).Name}'.");
    }
}