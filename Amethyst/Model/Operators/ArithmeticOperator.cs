namespace Amethyst.Model;

/// <summary>Represents arithmetic operators used in expressions.</summary>
public enum ArithmeticOperator
{
    [McfOperator("+")]
    [AmethystOperator("+")]
    ADD,
    [McfOperator("-")]
    [AmethystOperator("-")]
    SUBTRACT,
    [McfOperator("*")]
    [AmethystOperator("*")]
    MULTIPLY,
    [McfOperator("/")]
    [AmethystOperator("/")]
    DIVIDE,
    [McfOperator("%")]
    [AmethystOperator("%")]
    MODULO
}