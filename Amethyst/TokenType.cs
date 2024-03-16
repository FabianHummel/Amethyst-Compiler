using System.ComponentModel;

namespace Amethyst;

public enum TokenType
{
    // Single-character tokens.
    [Description("(")]
    LEFT_PAREN,
    [Description(")")]
    RIGHT_PAREN,
    [Description("{")]
    LEFT_BRACE,
    [Description("}")]
    RIGHT_BRACE,
    [Description("[")]
    LEFT_BRACKET,
    [Description("]")]
    RIGHT_BRACKET,
    [Description(",")]
    COMMA,
    [Description(".")]
    DOT,
    [Description("-")]
    MINUS,
    [Description("+")]
    PLUS,
    [Description(";")]
    SEMICOLON,
    [Description(":")]
    COLON,
    [Description("/")]
    SLASH,
    [Description("*")]
    STAR,
    [Description("%")]
    MODULO,

    // One or two character tokens.
    [Description("!")]
    BANG,
    [Description("!=")]
    BANG_EQUAL,
    [Description("=")]
    EQUAL,
    [Description("==")]
    EQUAL_EQUAL,
    [Description(">")]
    GREATER,
    [Description(">=")]
    GREATER_EQUAL,
    [Description("<")]
    LESS,
    [Description("<=")]
    LESS_EQUAL,
    [Description("+=")]
    PLUS_EQUAL,
    [Description("-=")]
    MINUS_EQUAL,
    [Description("/=")]
    SLASH_EQUAL,
    [Description("*=")]
    STAR_EQUAL,
    [Description("%=")]
    MODULO_EQUAL,

    // Literals.
    [Description("Identifier")]
    IDENTIFIER, 
    [Description("String")]
    STRING,
    [Description("Number")]
    NUMBER,

    // Keywords.
    [Description("and")]
    AND,
    [Description("else")]
    ELSE,
    [Description("false")]
    FALSE,
    [Description("function")]
    FUNCTION,
    [Description("ticking")]
    TICKING,
    [Description("initializing")]
    INITIALIZING,
    [Description("for")]
    FOR,
    [Description("if")]
    IF,
    [Description("null")]
    NULL,
    [Description("or")]
    OR,
    [Description("return")]
    RETURN,
    [Description("true")]
    TRUE, 
    [Description("var")]
    VAR, 
    [Description("while")]
    WHILE,
    [Description("print")]
    PRINT,
    
    // File.
    [Description("end of file")]
    EOF,
}