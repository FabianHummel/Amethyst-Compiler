namespace Amethyst.Model.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class AmethystOperatorAttribute : Attribute
{
    public string Operator { get; }

    public AmethystOperatorAttribute(string @operator)
    {
        Operator = @operator;
    }
}