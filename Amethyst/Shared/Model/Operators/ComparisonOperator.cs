namespace Amethyst.Model;

/// <summary>Represents comparison operators used in expressions.</summary>
public enum ComparisonOperator
{
    [McfToken("<")]
    [AmethystOperator("<")]
    LESS_THAN,
    [McfToken("<=")]
    [AmethystOperator("<=")]
    LESS_THAN_OR_EQUAL,
    [McfToken(">")]
    [AmethystOperator(">")]
    GREATER_THAN,
    [McfToken(">=")]
    [AmethystOperator(">=")]
    GREATER_THAN_OR_EQUAL,
    [AmethystOperator("==")]
    EQUAL,
    [AmethystOperator("!=")]
    NOT_EQUAL
}