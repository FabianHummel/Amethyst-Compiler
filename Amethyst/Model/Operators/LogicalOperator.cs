using Amethyst.Attributes;

namespace Amethyst.Model;

public enum LogicalOperator
{
    [AmethystOperator("&&")]
    AND,
    [AmethystOperator("||")]
    OR,
    [AmethystOperator("!")]
    NOT
}