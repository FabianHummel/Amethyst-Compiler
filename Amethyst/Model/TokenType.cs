namespace Amethyst.Model;

public enum TokenType
{
    // Single-character tokens.
    LEFT_PAREN,
    RIGHT_PAREN,
    LEFT_BRACE,
    RIGHT_BRACE,
    LEFT_BRACKET,
    RIGHT_BRACKET,
    COMMA,
    DOT,
    MINUS,
    PLUS,
    SEMICOLON,
    COLON,
    SLASH,
    STAR,
    MODULO,

    // One or two character tokens.
    BANG,
    BANG_EQUAL,
    EQUAL,
    EQUAL_EQUAL,
    GREATER,
    GREATER_EQUAL,
    LESS,
    LESS_EQUAL,
    PLUS_EQUAL,
    MINUS_EQUAL,
    SLASH_EQUAL,
    STAR_EQUAL,
    MODULO_EQUAL,

    // Literals.
    IDENTIFIER, 
    STRING,
    NUMBER,

    // Keywords.
    AND,
    ELSE,
    FALSE,
    FUNCTION,
    TICKING,
    INITIALIZING,
    NAMESPACE,
    FOR,
    IF,
    NULL,
    OR,
    XOR,
    BREAK,
    RETURN,
    CONTINUE,
    TRUE, 
    VAR, 
    WHILE,
    PRINT,
    COMMENT,
    
    // File.
    EOF,
}