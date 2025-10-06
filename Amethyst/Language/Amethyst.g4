grammar Amethyst;

SPACE: [ \r\t\u000C\n]+ -> skip;
COMMENT: '#' ~[\r\n]* -> skip;
IDENTIFIER: [a-zA-Z] [a-zA-Z0-9_]*;

file
 : preprocessor* EOF
 ;
 
preprocessor
 : preprocessorFromDeclaration
 | preprocessorStatement
 ;

preprocessorFromDeclaration
 : 'FROM' RESOURCE_LITERAL IDENTIFIER (',' IDENTIFIER)* ';'
 ;

preprocessorStatement
 : preprocessorYieldingStatement
 | statement
 ;
 
preprocessorYieldingStatement
 : preprocessorDeclaration
 | preprocessorIfStatement
 | preprocessorForStatement
 | preprocessorReturnStatement
 | preprocessorBreakStatement
 | preprocessorYieldStatement
 | preprocessorDebugStatement
 | preprocessorExpressionStatement
 ;
 
preprocessorDeclaration
 : preprocessorVariableDeclaration
 ;
 
preprocessorVariableDeclaration
 : 'VAR' IDENTIFIER (':' preprocessorType)? ('=' preprocessorExpression) ';'
 ;
 
preprocessorIfStatement
 : 'IF' '(' preprocessorExpression ')' block ('ELSE' (block | preprocessorIfStatement))?
 ;
 
preprocessorForStatement
 : 'FOR' '(' preprocessorForStatementInitializer? ';' preprocessorExpression? ';' preprocessorExpressionStatement? ')' block
 ;
 
preprocessorForStatementInitializer
 : preprocessorVariableDeclaration
 | preprocessorExpressionStatement
 ;
 
preprocessorReturnStatement
 : 'RETURN' preprocessorExpression? ';'
 ;

preprocessorBreakStatement
 : 'BREAK' ';'
 ;

preprocessorYieldStatement
 : 'YIELD' (selectorElement | recordSelectorElement | objectElement | arrayElement) ';'
 ;
 
preprocessorDebugStatement
 : 'DEBUG' preprocessorExpression ';'
 ;
 
preprocessorExpressionStatement
 : preprocessorAssignment ';'
 | preprocessorExpression ';'
 ;
 
preprocessorAssignment
 : preprocessorExpression ('=' | '+=' | '-=' | '*=' | '/=' | '%=') preprocessorExpression
 ;

preprocessorExpression
 : '++' preprocessorExpression                                                  # preprocessorPreIncrementExpression
 | '--' preprocessorExpression                                                  # preprocessorPreDecrementExpression
 | preprocessorExpression '++'                                                  # preprocessorPostIncrementExpression
 | preprocessorExpression '--'                                                  # preprocessorPostDecrementExpression
 | preprocessorExpression ('*' | '/' | '%') preprocessorExpression              # preprocessorFactorExpression
 | preprocessorExpression ('+' | '-') preprocessorExpression                    # preprocessorTermExpression
 | preprocessorExpression ('<' | '<=' | '>' | '>=') preprocessorExpression      # preprocessorComparisonExpression
 | preprocessorExpression ('==' | '!=') preprocessorExpression                  # preprocessorEqualityExpression
 | preprocessorExpression '&&' preprocessorExpression                           # preprocessorConjunctionExpression
 | preprocessorExpression '||' preprocessorExpression                           # preprocessorDisjunctionExpression
 | '-' preprocessorExpression                                                   # preprocessorNegationExpression
 | '!' preprocessorExpression                                                   # preprocessorInversionExpression
 | preprocessorGroup                                                            # preprocessorGroupedExpression
 | preprocessorLiteral                                                          # preprocessorLiteralExpression
 | IDENTIFIER                                                                   # preprocessorIdentifierExpression
 ;
 
preprocessorGroup
 : '(' preprocessorExpression ')'
 ;
 
preprocessorLiteral
 : booleanLiteral
 | INTEGER_LITERAL
 | DECIMAL_LITERAL
 | STRING_LITERAL
 | RESOURCE_LITERAL
 ;

preprocessorType
 : 'BOOL' | 'INT' | 'DEC' | 'STRING' | 'RESOURCE'
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
 : attributeList 'var' IDENTIFIER (':' type)? ('=' expression) ';'
 ;
 
recordDeclaration
 : attributeList 'record' IDENTIFIER (':' type)? ('=' expression) ';'
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
 : attributeList IDENTIFIER ':' type
 ;
 
block
 : '{' preprocessorStatement* '}'
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
 : 'for' '(' forStatementInitializer? ';' expression? ';' expressionStatement? ')' block
 ;
 
forStatementInitializer
 : variableDeclaration
 | expressionStatement
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
 | expression ';'
 ;

assignment
 : expression ('=' | '+=' | '-=' | '*=' | '/=' | '%=') expression
 ;

expression
 : IDENTIFIER '(' argumentList? ')'                     # callExpression
 | '++' expression                                      # preIncrementExpression
 | '--' expression                                      # preDecrementExpression
 | expression '++'                                      # postIncrementExpression
 | expression '--'                                      # postDecrementExpression
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
 
argumentList
 : expression (',' expression)*
 ;
 
group
 : '(' expression ')'
 ;
 
literal
 : booleanLiteral
 | INTEGER_LITERAL
 | DECIMAL_LITERAL
 | STRING_LITERAL
 | selectorCreation
 | objectCreation
 | arrayCreation
 ;
 
booleanLiteral
 : 'true' | 'false'
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
 
rangeExpression
 : expression '..' expression?
 | expression? '..' expression
 ;

selectorCreation
 : selectorType ('[' ( selectorElement ','? )* ']')?
 ;

selectorType
 : '@s' | '@r' | '@a' | '@e' | '@p' | '@n'
 ;

selectorElement
 : preprocessorYieldingStatement
 | selectorKvp
 ;
 
selectorKvp
 : IDENTIFIER '=' expression                # expressionSelector
 | IDENTIFIER '=' rangeExpression           # rangeSelector
 | IDENTIFIER '=' recordSelectorCreation    # recordSelector
 ;
 
recordSelectorCreation
 : '{}'
 | '{' ( recordSelectorElement ','? )* '}'
 ;

recordSelectorElement
 : preprocessorYieldingStatement
 | recordSelectorKvp
 ;
 
recordSelectorKvp
 : IDENTIFIER ':' (expression | rangeExpression)
 ;
  
objectCreation
 : '{}'
 | '{' ( objectElement ','? )* '}'
 ;

objectElement
 : preprocessorYieldingStatement
 | objectKvp
 ;

objectKvp
 : IDENTIFIER ':' expression
 ;

arrayCreation
 : '[]'
 | '[' ( arrayElement ','? )* ']'
 ;

arrayElement
 : preprocessorYieldingStatement
 | expression
 ;
 
type
 : ('bool' | 'int' | decimal | 'string' | 'array' | 'object') ('[]' | '{}')?
 ;
 
decimal
 : 'dec' ('(' INTEGER_LITERAL ')')?
 ;