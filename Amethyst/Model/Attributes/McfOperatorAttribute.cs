namespace Amethyst.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class McfOperatorAttribute : Attribute
{
    public string Operator { get; }

    public McfOperatorAttribute(string @operator)
    {
        Operator = @operator;
    }
}