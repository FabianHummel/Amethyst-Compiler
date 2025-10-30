namespace Amethyst.Model;

/// <summary>Represents logical operators used in expressions.</summary>
public enum LogicalOperator
{
    [AmethystOperator("&&")]
    AND,
    [AmethystOperator("||")]
    OR,
    [AmethystOperator("!")]
    NOT
}