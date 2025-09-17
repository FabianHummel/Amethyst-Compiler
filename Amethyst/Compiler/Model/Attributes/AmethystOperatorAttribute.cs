namespace Amethyst.Model;

[AttributeUsage(AttributeTargets.Field)]
public class AmethystOperatorAttribute : Attribute
{
    public string Operator { get; }

    public AmethystOperatorAttribute(string @operator)
    {
        Operator = @operator;
    }
}