
using Amethyst.Attributes;

namespace Amethyst.Model;

public enum ArithmeticOperator
{
    [McfOperator("+")]
    ADD,
    [McfOperator("-")]
    SUBTRACT,
    [McfOperator("*")]
    MULTIPLY,
    [McfOperator("/")]
    DIVIDE,
    [McfOperator("%")]
    MODULO
}