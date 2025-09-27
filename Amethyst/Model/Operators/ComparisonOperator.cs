namespace Amethyst.Model;

public enum ComparisonOperator
{
    [McfOperator("<")]
    [AmethystOperator("<")]
    LESS_THAN,
    [McfOperator("<=")]
    [AmethystOperator("<=")]
    LESS_THAN_OR_EQUAL,
    [McfOperator(">")]
    [AmethystOperator(">")]
    GREATER_THAN,
    [McfOperator(">=")]
    [AmethystOperator(">=")]
    GREATER_THAN_OR_EQUAL,
    [AmethystOperator("==")]
    EQUAL,
    [AmethystOperator("!=")]
    NOT_EQUAL
}