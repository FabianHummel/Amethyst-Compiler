namespace Amethyst.Model;

/// <summary>Represents assignment operators used in expressions.</summary>
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