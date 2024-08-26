
using Amethyst.Attributes;

namespace Amethyst.Model;

public enum Operator
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