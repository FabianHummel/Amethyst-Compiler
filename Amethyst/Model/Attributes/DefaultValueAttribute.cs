namespace Amethyst.Model.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class DefaultValueAttribute : Attribute
{
    public string DefaultValue { get; }

    public DefaultValueAttribute(string defaultValue)
    {
        DefaultValue = defaultValue;
    }
}