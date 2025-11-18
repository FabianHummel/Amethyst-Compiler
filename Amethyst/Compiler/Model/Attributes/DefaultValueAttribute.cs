namespace Amethyst.Model;

/// <summary>Represents a Minecraft type's default value that is used for allocation of new values
/// during runtime.</summary>
[AttributeUsage(AttributeTargets.Field)]
public class DefaultValueAttribute : Attribute
{
    public string DefaultValue { get; }

    public DefaultValueAttribute(string defaultValue)
    {
        DefaultValue = defaultValue;
    }
}