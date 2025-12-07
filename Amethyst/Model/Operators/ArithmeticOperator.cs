namespace Amethyst.Model;

/// <summary>Represents arithmetic operators used in expressions.</summary>
public enum ArithmeticOperator
{
    [McfToken("+")]
    [AmethystOperator("+")]
    ADD,
    [McfToken("-")]
    [AmethystOperator("-")]
    SUBTRACT,
    [McfToken("*")]
    [AmethystOperator("*")]
    MULTIPLY,
    [McfToken("/")]
    [AmethystOperator("/")]
    DIVIDE,
    [McfToken("%")]
    [AmethystOperator("%")]
    MODULO
}