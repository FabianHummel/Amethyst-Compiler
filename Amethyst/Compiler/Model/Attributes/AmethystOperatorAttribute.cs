namespace Amethyst.Model;

/// <summary>Maps an arithmetic operator to the actual amethyst operator token.</summary>
/// <seealso cref="McfTokenAttribute" />
[AttributeUsage(AttributeTargets.Field)]
public class AmethystOperatorAttribute : Attribute
{
    public string Operator { get; }

    public AmethystOperatorAttribute(string @operator)
    {
        Operator = @operator;
    }
}