namespace Amethyst.Model;

public enum AssignmentOperator
{
    [AmethystOperator("=")]
    ASSIGN,
    [AmethystOperator("+=")]
    ADD,
    [AmethystOperator("-=")]
    SUBTRACT,
    [AmethystOperator("*=")]
    MULTIPLY,
    [AmethystOperator("/=")]
    DIVIDE,
    [AmethystOperator("%=")]
    MODULO
}