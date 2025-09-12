grammar Amethyst;

SPACE: [ \r\t\u000C\n]+ -> skip;
COMMENT: '#' ~[\r\n]* -> skip;
IDENTIFIER: [a-zA-Z] [a-zA-Z0-9_]*;

file
 : from* declaration* EOF
 ;
 
from
 : 'from' RESOURCE_LITERAL IDENTIFIER (',' IDENTIFIER)* ';'
 ;

declaration
 : functionDeclaration
 | variableDeclaration
 | recordDeclaration
 ;
 
functionDeclaration
 : attributeList 'function' IDENTIFIER '(' parameterList? ')' (':' type)? block
 ;
 
variableDeclaration
 : attributeList 'var' IDENTIFIER (':' type)? ('=' expression)? ';'
 ;
 
recordDeclaration
 : attributeList 'record' IDENTIFIER (':' type)? ('=' expression)? ';'
 ;
 
attributeList
 : ('[' attribute (',' attribute)* ']')*
 ;
 
attribute
 : IDENTIFIER
 ;
 
parameterList
 : parameter (',' parameter)*
 ;
 
parameter
 : IDENTIFIER ':' type
 ;
 
block
 : '{' statement* '}'
 ;
 
statement
 : declaration
 | forStatement
 | whileStatement
 | foreachStatement
 | ifStatement
 | debugStatement
 | commentStatement
 | returnStatement
 | breakStatement
 | continueStatement
 | block
 | expressionStatement
 ;

forStatement
 : 'for' '(' (variableDeclaration | expressionStatement)? expression? ';' expression? ')' block
 ;
 
whileStatement
 : 'while' '(' expression ')' block
 ;
 
foreachStatement
 : 'foreach' '(' IDENTIFIER ':' expression ')' block
 ;

ifStatement
 : 'if' '(' expression ')' block ('else' (block | ifStatement))?
 ;
 
debugStatement
 : 'debug' expression ';'
 ;
 
commentStatement
 : 'comment' STRING_LITERAL ';'
 ;
 
returnStatement
 : 'return' expression? ';'
 ;
 
breakStatement
 : 'break' ';'
 ;

continueStatement
 : 'continue' ';'
 ;
 
expressionStatement
 : assignment ';'
 ;

assignment
 : expression ('=' | '+=' | '-=' | '*=' | '/=' | '%=') expression            # assignmentExpression
 | expression '++'                                                           # incrementExpression
 | expression '--'                                                           # decrementExpression
 | expression                                                                # baseExpression
 ;

expression
 : selectorType ('[' selectorQueryList ']')?            # selectorExpression
 | IDENTIFIER '(' argumentList? ')'                     # callExpression            // TODO:;
 | expression '.' IDENTIFIER                            # memberExpression
 | expression '[' expression ']'                        # indexExpression
 | expression ('*' | '/' | '%') expression              # factorExpression
 | expression ('+' | '-') expression                    # termExpression
 | expression ('<' | '<=' | '>' | '>=') expression      # comparisonExpression
 | expression ('==' | '!=') expression                  # equalityExpression
 | expression '&&' expression                           # conjunctionExpression
 | expression '||' expression                           # disjunctionExpression
 | '-' expression                                       # negationExpression        // TODO:
 | '!' expression                                       # inversionExpression       // TODO:
 | group                                                # groupedExpression
 | literal                                              # literalExpression
 | IDENTIFIER                                           # identifierExpression
 ;
 
literal
 : booleanLiteral
 | INTEGER_LITERAL
 | DECIMAL_LITERAL
 | STRING_LITERAL
 | RESOURCE_LITERAL
 | objectCreation
 | arrayCreation
 ;
 
booleanLiteral
 : 'true'
 | 'false'
 ;
 
INTEGER_LITERAL
 : '0'..'9'+
 ;
 
DECIMAL_LITERAL
 : '0'..'9'* '.' '0'..'9'+
 ;
 
STRING_LITERAL
 : '"' .*? '"'
 ;
 
RESOURCE_LITERAL
 : '`' .*? '`'
 ;

group
 : '(' expression ')'
 ;
 
rangeExpression
 : expression '..' expression?
 | expression? '..' expression
 ; 

selectorQueryList
 : selectorQuery (',' selectorQuery)*
 ;
 
selectorQuery
 : IDENTIFIER '=' expression                # expressionSelector
 | IDENTIFIER '=' rangeExpression           # rangeSelector
 | IDENTIFIER '=' recordSelectorList        # recordsSelector
 ;
 
recordSelectorList
 : '{}'
 | '{' (recordSelectorElement (',' recordSelectorElement)*)? '}'
 ;
 
recordSelectorElement
 : IDENTIFIER ':' (expression | rangeExpression)
 ;
 
selectorType
 : '@s'
 | '@r'
 | '@a'
 | '@e'
 | '@p'
 | '@n'
 ;
 
argumentList
 : expression (',' expression)*
 ;
  
objectCreation
 : '{}'
 | '{' (objectElement (',' objectElement)*)? '}'
 ;
 
objectElement
 : IDENTIFIER ':' expression
 ;

arrayCreation
 : '[]'
 | '[' (expression (',' expression)* )? ']'
 ;
 
type
 : ('int' | decimal | 'string' | 'bool' | 'array' | 'object') ('[]' | '{}')?
 ;
 
decimal
 : 'dec' ('(' INTEGER_LITERAL ')')?
 ;