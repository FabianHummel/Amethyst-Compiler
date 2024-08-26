namespace Amethyst.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class DefaultValueAttribute : Attribute
{
    public object DefaultValue { get; }

    public DefaultValueAttribute(object defaultValue)
    {
        DefaultValue = defaultValue;
    }
}