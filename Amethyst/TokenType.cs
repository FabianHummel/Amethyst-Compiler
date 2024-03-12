using System.ComponentModel;

namespace Amethyst;

public enum TokenType
{
    [Description("keyword: 'function'")]
    KWD_FUNCTION, // ()

    [Description("keyword: 'ticking'")]
    KWD_TICKING, // ()

    [Description("keyword: 'initializing'")]
    KWD_INITIALIZING, // ()

    [Description("keyword: 'namespace'")]
    KWD_NAMESPACE, // ()

    [Description("keyword: 'variable'")]
    KWD_VARIABLE, // ()

    [Description("opening parenthesis '('")]
    PAREN_OPEN, // (

    [Description("closing parenthesis ')'")]
    PAREN_CLOSE, // )

    [Description("opening brace '{'")]
    BRACE_OPEN, // {

    [Description("closing brace '}'")]
    BRACE_CLOSE, // }

    [Description("opening bracket '['")]
    BRACKET_OPEN, // [

    [Description("closing bracket ']'")]
    BRACKET_CLOSE, // ]

    [Description("literal: 'string'")]
    LITERAL_STRING, // ""

    [Description("literal: 'number'")]
    LITERAL_NUMBER, // 123

    [Description("identifier")]
    IDENTIFIER, // name

    [Description("semicolon ';'")]
    SEMICOLON, // ;

    [Description("comma ','")]
    COMMA, // ,

    [Description("operator: 'addition' '+'")]
    OP_ADD, // +

    [Description("operator: 'subtraction' '-'")]
    OP_SUB, // -

    [Description("operator: 'multiplication' '*'")]
    OP_MUL, // *

    [Description("operator: 'division' '/'")]
    OP_DIV, // /

    [Description("operator: 'modulo' '%'")]
    OP_MOD, // %
    
    [Description("operator: 'assignment' '='")]
    OP_ASSIGN // =
}