using Amethyst.Model.Attributes;

namespace Amethyst.Model;

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