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
    INTEGER,
    DECIMAL,

    // Keywords.
    IF, ELSE, FOR, FOREACH, WHILE,
    AND, OR, XOR,
    BREAK, CONTINUE, RETURN,
    TRUE, FALSE,
    FUNCTION, NAMESPACE,
    VAR, RECORD,
    DEBUG, COMMENT,
    DT_STRING, DT_INT, DT_DEC, DT_BOOL, DT_ARRAY, DT_OBJECT,
    
    // File.
    EOF,
}