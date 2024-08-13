using System.ComponentModel;

namespace Amethyst.Model;

public enum Operator
{
    [Description("+")]
    ADD,
    [Description("-")]
    SUBTRACT,
    [Description("*")]
    MULTIPLY,
    [Description("/")]
    DIVIDE,
    [Description("%")]
    MODULO
}

public static class OperatorExtensions
{
    public static string ToSymbol(this Operator op)
    {
        var field = op.GetType().GetField(op.ToString());
        var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
        return attribute!.Description;
    }
}