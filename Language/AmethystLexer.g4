lexer grammar AmethystLexer;

@lexer::header {#pragma warning disable 3021}

DOT: '.' ;
DOTDOT: '..' ;
COMMA: ',' ;
COLON: ':' ;
SEMICOLON: ';' ;
LPAREN: '(' ;
RPAREN: ')' ;
LBRACE: '{' -> pushMode(DEFAULT_MODE) ;
RBRACE: '}' -> popMode ;
LBRACKET: '[' ;
RBRACKET: ']' ;
EQUALS: '=' ;
PLUS: '+' ;
MINUS: '-' ;
MULTIPLY: '*' ;
DIVIDE: '/' ;
MODULO: '%' ;
NOT: '!' ;
AND: '&&' ;
OR: '||' ;
LESS: '<' ;
GREATER: '>' ;
LESSEQ: '<=' ;
GREATEREQ: '>=' ;
EQEQ: '==' ;
NOTEQ: '!=' ;
PLUSPLUS: '++' ;
MINUSMINUS: '--' ;
PLUSEQ: '+=' ;
MINUSEQ: '-=' ;
MULTEQ: '*=' ;
DIVEQ: '/=' ;
MODEQ: '%=' ;
ARROW: '->' ;
LRBRACKET: '[]' ;
LRBRACE: '{}' ;
QUOTE: '"' -> pushMode(IN_STRING) ;
INTERPQUOTE: '$"' -> pushMode(IN_INTERP_STRING) ;
BACKTICK: '`' -> pushMode(IN_RESOURCE) ;
INTERPBACKTICK: '$`' -> pushMode(IN_INTERP_RESOURCE) ;

PREPROCESSOR_FROM: 'FROM' ;
PREPROCESSOR_IMPORT: 'IMPORT' ;
PREPROCESSOR_AS: 'AS' ;
PREPROCESSOR_VAR: 'VAR' ;

PREPROCESSOR_IF: 'IF' ;
PREPROCESSOR_ELSE: 'ELSE' ;
PREPROCESSOR_FOR: 'FOR' ;
PREPROCESSOR_RETURN: 'RETURN' ;
PREPROCESSOR_BREAK: 'BREAK' ;
PREPROCESSOR_YIELD: 'YIELD' ;
PREPROCESSOR_COMMENT: 'COMMENT' ;
PREPROCESSOR_DEBUG: 'DEBUG' ;

PREPROCESSOR_TRUE: 'TRUE' ;
PREPROCESSOR_FALSE: 'FALSE' ;

PREPROCESSOR_BOOL: 'BOOL' ;
PREPROCESSOR_INT: 'INT' ;
PREPROCESSOR_DEC: 'DEC' ;
PREPROCESSOR_STRING: 'STRING' ;
PREPROCESSOR_RESOURCE: 'RESOURCE' ;

FUNCTION: 'function' ;
VAR: 'var' ;
RECORD: 'record' ;

FOR: 'for' ;
WHILE: 'while' ;
FOREACH: 'foreach' ;
IF: 'if' ;
ELSE: 'else' ;
DEBUG: 'debug' ;
BREAK: 'break' ;
CONTINUE: 'continue' ;
RETURN: 'return' ;

TRUE: 'true' ;
FALSE: 'false' ;

BOOL: 'bool' ;
INT: 'int' ;
DEC: 'dec' ;
STRING: 'string' ;
ARRAY: 'array' ;
OBJECT: 'object' ;
ENTITY: 'entity' ;
STORAGE: 'storage' -> pushMode(IN_STORAGE) ;
SCOREBOARD: 'scoreboard' -> pushMode(IN_SCOREBOARD) ;

SELECTOR_SELF: '@s' ;
SELECTOR_RANDOM_PLAYER: '@r' ;
SELECTOR_ALL_PLAYERS: '@a' ;
SELECTOR_ALL_ENTITIES: '@e' ;
SELECTOR_NEAREST_PLAYER: '@p' ;
SELECTOR_NEAREST_ENTITY: '@n' ;

WS: [ \r\t\u000C\n]+ -> skip ;
SL_COMMENT: '#' ~[\r\n]* -> skip ;
COMMAND: BOL [ \r\t\u000C\n]* '/' ~[\r\n]+;
NEWLINE: '\r'? '\n';
INTEGER_LITERAL: '0'..'9'+ ;
DECIMAL_LITERAL: '0'..'9'* '.' '0'..'9'+ ;
IDENTIFIER: [a-zA-Z0-9_]+;
BOL: [\r\n\f]+ ;

mode IN_STRING;
    
    REGULAR_STRING_ESCAPED_BACKSLASH: '\\\\' -> type(REGULAR_STRING_CONTENT) ;
    REGULAR_STRING_ESCAPED_QUOTE: '\\"' -> type(REGULAR_STRING_CONTENT) ;
    REGULAR_STRING_END: '"' -> type(QUOTE), popMode ;
    REGULAR_STRING_CONTENT: ~["\\\r\n]+ ;

mode IN_INTERP_STRING;

    INTERP_STRING_ESCAPED_BACKSLASH: '\\\\' -> type(INTERP_STRING_CONTENT) ;
    INTERP_STRING_ESCAPED_LBRACE: '\\{' -> type(INTERP_STRING_CONTENT) ;
    INTERP_STRING_ESCAPED_QUOTE: '\\"' -> type(INTERP_STRING_CONTENT) ;
    INTERP_STRING_INTERPOLATION_START: '{' -> type(LBRACE), pushMode(DEFAULT_MODE) ;
    INTERP_STRING_END: '"' -> type(QUOTE), popMode ;
    INTERP_STRING_CONTENT: ~["{\\\r\n]+ ;

mode IN_RESOURCE;

    REGULAR_RESOURCE_ESCAPED_BACKSLASH: '\\\\' -> type(REGULAR_RESOURCE_CONTENT) ;
    REGULAR_RESOURCE_ESCAPED_BACKTICK: '\\`' -> type(REGULAR_RESOURCE_CONTENT) ;
    REGULAR_RESOURCE_END: '`' -> type(BACKTICK), popMode ;
    REGULAR_RESOURCE_CONTENT: ([a-zA-Z0-9_.-]+ ':')? [a-zA-Z0-9_./-]+ ;

mode IN_INTERP_RESOURCE;

    INTERP_RESOURCE_ESCAPED_BACKSLASH: '\\\\' -> type(INTERP_RESOURCE_CONTENT) ;
    INTERP_RESOURCE_ESCAPED_LBRACE: '\\{' -> type(INTERP_RESOURCE_CONTENT) ;
    INTERP_RESOURCE_ESCAPED_BACKTICK: '\\`' -> type(INTERP_RESOURCE_CONTENT) ;
    INTERP_RESOURCE_INTERPOLATION_START: '{' -> type(LBRACE), pushMode(DEFAULT_MODE) ;
    INTERP_RESOURCE_END: '`' -> type(BACKTICK), popMode ;
    INTERP_RESOURCE_CONTENT: ([a-zA-Z0-9_.-]+ ':')? [a-zA-Z0-9_./-]+ ;

mode IN_STORAGE;

    STORAGE_NAMESPACE: [a-zA-Z0-9_.-]+ ':' [a-zA-Z0-9_./-]+ -> popMode, pushMode(IN_STORAGE_2) ;
    WS_IN_STORAGE: [ \r\t\u000C\n]+ -> skip ;
    
mode IN_STORAGE_2;

    STORAGE_MEMBER: ([a-zA-Z0-9-+._]+ '.'?)+ -> popMode ;
    WS_IN_STORAGE_MEMBER: [ \r\t\u000C\n]+ -> skip ;
    
mode IN_SCOREBOARD;

    SCOREBOARD_PLAYER: ~[ \t\r\n]+ -> popMode, pushMode(IN_SCOREBOARD_2) ;
    WS_IN_SCOREBOARD: [ \r\t\u000C\n]+ -> skip ;
    
mode IN_SCOREBOARD_2;

    SCOREBOARD_OBJECTIVE: [a-zA-Z0-9-+._]+ -> popMode ;
    WS_IN_SCOREBOARD_OBJECTIVE: [ \r\t\u000C\n]+ -> skip ;
