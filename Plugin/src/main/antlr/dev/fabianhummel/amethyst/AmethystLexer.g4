lexer grammar AmethystLexer;

DOT: '.' ;
DOTDOT: '..' ;
COMMA: ',' ;
COLON: ':' ;
SEMICOLON: ';' ;
LPAREN: '(' ;
RPAREN: ')' ;
LBRACE: '{' ;
RBRACE: '}' ;
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

PREPROCESSOR_FROM: 'FROM' ;
PREPROCESSOR_IMPORT: 'IMPORT' ;
PREPROCESSOR_VAR: 'VAR' ;

PREPROCESSOR_IF: 'IF' ;
PREPROCESSOR_ELSE: 'ELSE' ;
PREPROCESSOR_FOR: 'FOR' ;
PREPROCESSOR_RETURN: 'RETURN' ;
PREPROCESSOR_BREAK: 'BREAK' ;
PREPROCESSOR_YIELD: 'YIELD' ;
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
COMMENT: 'comment' ;
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

WS: [ \r\t\u000C\n]+ -> channel(HIDDEN) ;
SL_COMMENT: '#' ~[\r\n]* -> channel(HIDDEN) ;
COMMAND: BOL [ \r\t\u000C\n]* '/' ~[\r\n]+;
NEWLINE: '\r'? '\n';
STRING_LITERAL: '"' ~["\\\r\n]* '"' ; //STRING_LITERAL: '"' -> pushMode(IN_STRING) ;
RESOURCE_LITERAL: '`' ~[`\\\r\n]* '`' ; //RESOURCE_LITERAL: '`' -> pushMode(IN_RESOURCE) ;
INTEGER_LITERAL: '0'..'9'+ ;
DECIMAL_LITERAL: '0'..'9'* '.' '0'..'9'+ ;
IDENTIFIER: [a-zA-Z0-9_]+;
BOL : [\r\n\f]+ ;
//HEX: [0-9a-fA-F] ;
ERRCHAR: . -> channel(HIDDEN);

//mode IN_STRING;
//
//    STRING_CONTENT: ~["\\\r\n]+ ;
//    STRING_ESCAPE_SEQUENCE: '\\' ["\\/bfnrt] ;
//    STRING_UNICODE_ESCAPE: '\\' 'u' HEX HEX HEX HEX ;
//    STRING_QUOTE: '"' -> popMode ;
//    STRING_INTERPOLATION_START: '${' -> pushMode(IN_INTERPOLATION) ;
//
//mode IN_RESOURCE;
//
//    RESOURCE_CONTENT: ([a-zA-Z0-9_.-]+ ':')? [a-zA-Z0-9_./-]+ ;
//    RESOURCE_ESCAPE_SEQUENCE: '\\' [`\\/bfnrt] ;
//    RESOURCE_UNICODE_ESCAPE: '\\' 'u' HEX HEX HEX HEX ;
//    RESOURCE_QUOTE: '`' -> popMode ;
//    RESOURCE_INTERPOLATION_START: '${' -> pushMode(IN_INTERPOLATION) ;
//
//mode IN_INTERPOLATION;
//
//    INTERPOLATION_END: '}' -> popMode ;
    
mode IN_STORAGE;

    STORAGE_NAMESPACE: [a-zA-Z0-9_.-]+ ':' [a-zA-Z0-9_./-]+ -> popMode, pushMode(IN_STORAGE_2) ;
    WS_IN_STORAGE: [ \r\t\u000C\n]+ -> channel(HIDDEN) ;
    
mode IN_STORAGE_2;

    STORAGE_MEMBER: ([a-zA-Z0-9-+._]+ '.'?)+ -> popMode ;
    WS_IN_STORAGE_MEMBER: [ \r\t\u000C\n]+ -> channel(HIDDEN) ;
    
mode IN_SCOREBOARD;

    SCOREBOARD_PLAYER: ~[ \t\r\n]+ -> popMode, pushMode(IN_SCOREBOARD_2) ;
    WS_IN_SCOREBOARD: [ \r\t\u000C\n]+ -> channel(HIDDEN) ;
    
mode IN_SCOREBOARD_2;

    SCOREBOARD_OBJECTIVE: [a-zA-Z0-9-+._]+ -> popMode ;
    WS_IN_SCOREBOARD_OBJECTIVE: [ \r\t\u000C\n]+ -> channel(HIDDEN) ;
