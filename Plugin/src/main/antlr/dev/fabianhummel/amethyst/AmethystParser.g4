parser grammar AmethystParser;

options { tokenVocab=AmethystLexer; }

file
 : preprocessor* EOF
 ;

preprocessor
 : preprocessorFromDeclaration
 | preprocessorStatement
 ;

preprocessorFromDeclaration
 : PREPROCESSOR_FROM RESOURCE_LITERAL PREPROCESSOR_IMPORT IDENTIFIER (COMMA IDENTIFIER)* SEMICOLON // replace RESOURCE_LITERAL with resourceLiteral in the future
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
 : PREPROCESSOR_VAR IDENTIFIER (COLON preprocessorType)? (EQUALS preprocessorExpression) SEMICOLON
 ;

preprocessorType
 : PREPROCESSOR_BOOL
 | PREPROCESSOR_INT
 | PREPROCESSOR_DEC
 | PREPROCESSOR_STRING
 | PREPROCESSOR_RESOURCE
 ;

preprocessorIfStatement
 : PREPROCESSOR_IF LPAREN preprocessorExpression RPAREN block (PREPROCESSOR_ELSE (block | preprocessorIfStatement))?
 ;

preprocessorForStatement
 : PREPROCESSOR_FOR LPAREN preprocessorForStatementInitializer? SEMICOLON preprocessorExpression? SEMICOLON preprocessorExpressionStatement? RPAREN block
 ;

preprocessorForStatementInitializer
 : preprocessorVariableDeclaration
 | preprocessorExpressionStatement
 ;

preprocessorReturnStatement
 : PREPROCESSOR_RETURN preprocessorExpression? SEMICOLON
 ;

preprocessorBreakStatement
 : PREPROCESSOR_BREAK SEMICOLON
 ;

preprocessorYieldStatement
 : PREPROCESSOR_YIELD (selectorElement | recordSelectorElement | objectElement | arrayElement) SEMICOLON
 ;

preprocessorDebugStatement
 : PREPROCESSOR_DEBUG preprocessorExpression SEMICOLON
 ;

preprocessorExpressionStatement
 : preprocessorAssignmentStatement SEMICOLON
 | preprocessorExpression SEMICOLON
 ;

preprocessorAssignmentStatement
 : preprocessorExpression op=(EQUALS | PLUSEQ | MINUSEQ | MULTEQ | DIVEQ | MODEQ) preprocessorExpression
 ;

preprocessorExpression
 : PLUSPLUS preprocessorExpression                                                          # preprocessorPreIncrementExpression
 | MINUSMINUS preprocessorExpression                                                        # preprocessorPreDecrementExpression
 | preprocessorExpression PLUSPLUS                                                          # preprocessorPostIncrementExpression
 | preprocessorExpression MINUSMINUS                                                        # preprocessorPostDecrementExpression
 | preprocessorExpression op=(MULTIPLY | DIVIDE | MODULO) preprocessorExpression            # preprocessorFactorExpression
 | preprocessorExpression op=(PLUS | MINUS) preprocessorExpression                          # preprocessorTermExpression
 | preprocessorExpression op=(LESS | GREATER | LESSEQ | GREATEREQ) preprocessorExpression   # preprocessorComparisonExpression
 | preprocessorExpression op=(EQEQ | NOTEQ) preprocessorExpression                          # preprocessorEqualityExpression
 | preprocessorExpression AND preprocessorExpression                                        # preprocessorConjunctionExpression
 | preprocessorExpression OR preprocessorExpression                                         # preprocessorDisjunctionExpression
 | MINUS preprocessorExpression                                                             # preprocessorNegationExpression
 | NOT preprocessorExpression                                                               # preprocessorInversionExpression
 | preprocessorGroup                                                                        # preprocessorGroupedExpression
 | preprocessorLiteral                                                                      # preprocessorLiteralExpression
 | IDENTIFIER                                                                               # preprocessorIdentifierExpression
 ;

preprocessorGroup
 : LPAREN preprocessorExpression RPAREN
 ;

preprocessorLiteral
 : booleanLiteral
 | INTEGER_LITERAL
 | DECIMAL_LITERAL
 | STRING_LITERAL // stringLiteral
 | RESOURCE_LITERAL // resourceLiteral
 ;
 
//stringLiteral
// : STRING_LITERAL stringLiteralPart* STRING_QUOTE
// ;
// 
//stringLiteralPart
// : STRING_CONTENT
// | STRING_ESCAPE_SEQUENCE
// | STRING_UNICODE_ESCAPE
// | STRING_INTERPOLATION_START expression INTERPOLATION_END
// ;
//
//resourceLiteral
// : RESOURCE_LITERAL resourceLiteralPart* RESOURCE_QUOTE
// ;
// 
//resourceLiteralPart
// : RESOURCE_CONTENT
// | RESOURCE_ESCAPE_SEQUENCE
// | RESOURCE_UNICODE_ESCAPE
// | RESOURCE_INTERPOLATION_START expression INTERPOLATION_END
// ;

declaration
 : functionDeclaration
 | variableDeclaration
 | recordDeclaration
 ;

functionDeclaration
 : attributeList FUNCTION IDENTIFIER LPAREN parameterList? RPAREN (ARROW type)? block
 ;

variableDeclaration
 : attributeList VAR IDENTIFIER (COLON type)? (EQUALS expression) SEMICOLON
 ;

recordDeclaration
 : attributeList RECORD IDENTIFIER (COLON type)? (EQUALS expression) SEMICOLON
 ;

type
 : (BOOL | INT | decimal | STRING | ARRAY | OBJECT | ENTITY) modifier=(LRBRACKET | LRBRACE)?
 | rawLocation
 ;

attributeList
 : (LBRACKET attribute (COMMA attribute)* RBRACKET)*
 ;

attribute
 : IDENTIFIER
 ;

parameterList
 : parameter (COMMA parameter)*
 ;

parameter
 : attributeList IDENTIFIER COLON type
 ;

block
 : LBRACE preprocessorStatement* RBRACE
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
 | commandStatement
 ;

forStatement
 : FOR LPAREN forStatementInitializer? SEMICOLON expression? SEMICOLON expressionStatement? RPAREN block
 ;

forStatementInitializer
 : variableDeclaration
 | expressionStatement
 ;

whileStatement
 : WHILE LPAREN expression RPAREN block
 ;

foreachStatement
 : FOREACH LPAREN IDENTIFIER COLON expression RPAREN block
 ;

ifStatement
 : IF LPAREN expression RPAREN block (ELSE (block | ifStatement))?
 ;

debugStatement
 : DEBUG expression SEMICOLON
 ;

commentStatement
 : COMMENT STRING_LITERAL SEMICOLON
 ;

returnStatement
 : RETURN expression? SEMICOLON
 ;

breakStatement
 : BREAK SEMICOLON
 ;

continueStatement
 : CONTINUE SEMICOLON
 ;

expressionStatement
 : assignment SEMICOLON
 | expression SEMICOLON
 ;

commandStatement
  : COMMAND
  ;

assignment
 : expression op=(EQUALS | PLUSEQ | MINUSEQ | MULTEQ | DIVEQ | MODEQ) expression
 ;

expression
 : IDENTIFIER LPAREN argumentList? RPAREN                           # callExpression
 | PLUSPLUS expression                                              # preIncrementExpression
 | MINUSMINUS expression                                            # preDecrementExpression
 | expression PLUSPLUS                                              # postIncrementExpression
 | expression MINUSMINUS                                            # postDecrementExpression
 | expression DOT IDENTIFIER                                        # memberExpression
 | expression LBRACKET expression RBRACKET                          # indexExpression
 | expression op=(MULTIPLY | DIVIDE | MODULO) expression            # factorExpression
 | expression op=(PLUS | MINUS) expression                          # termExpression
 | expression op=(LESS | GREATER | LESSEQ | GREATEREQ) expression   # comparisonExpression
 | expression op=(EQEQ | NOTEQ) expression                          # equalityExpression
 | expression AND expression                                        # conjunctionExpression
 | expression OR expression                                         # disjunctionExpression
 | MINUS expression                                                 # negationExpression        // TODO:
 | NOT expression                                                   # inversionExpression       // TODO:
 | group                                                            # groupedExpression
 | literal                                                          # literalExpression
 | IDENTIFIER                                                       # identifierExpression
 ;
 
argumentList
 : expression (COMMA expression)*
 ;

group
 : LPAREN expression RPAREN
 ;

literal
 : booleanLiteral
 | INTEGER_LITERAL
 | DECIMAL_LITERAL
 | STRING_LITERAL // stringLiteral
 | selectorCreation
 | objectCreation
 | arrayCreation
 | rawLocation
 ;

booleanLiteral
 : TRUE | FALSE
 ;

selectorCreation
 : selectorType (LBRACKET ( selectorElement COMMA? )* RBRACKET)?
 ;

selectorType
 : SELECTOR_SELF
 | SELECTOR_RANDOM_PLAYER
 | SELECTOR_ALL_PLAYERS
 | SELECTOR_ALL_ENTITIES
 | SELECTOR_NEAREST_PLAYER
 | SELECTOR_NEAREST_ENTITY
 ;

selectorElement
 : preprocessorYieldingStatement
 | selectorKvp
 ;

selectorKvp
 : IDENTIFIER EQUALS expression                # expressionSelector
 | IDENTIFIER EQUALS rangeExpression           # rangeSelector
 | IDENTIFIER EQUALS recordSelectorCreation    # recordSelector
 ;
 
rangeExpression
 : expression DOTDOT expression?
 | expression? DOTDOT expression
 ;

recordSelectorCreation
 : LRBRACE
 | LBRACE ( recordSelectorElement COMMA? )* RBRACE
 ;

recordSelectorElement
 : preprocessorYieldingStatement
 | recordSelectorKvp
 ;

recordSelectorKvp
 : IDENTIFIER COLON (expression | rangeExpression)
 ;

objectCreation
 : LRBRACE
 | LBRACE ( objectElement COMMA? )* RBRACE
 ;

objectElement
 : preprocessorYieldingStatement
 | objectKvp
 ;

objectKvp
 : IDENTIFIER COLON expression
 ;

arrayCreation
 : LRBRACKET
 | LBRACKET ( arrayElement COMMA? )* RBRACKET
 ;

arrayElement
 : preprocessorYieldingStatement
 | expression
 ;

decimal
 : DEC (LPAREN INTEGER_LITERAL RPAREN)?
 ;

rawLocation
 : STORAGE STORAGE_NAMESPACE STORAGE_MEMBER             # rawStorageLocation
 | SCOREBOARD SCOREBOARD_PLAYER SCOREBOARD_OBJECTIVE    # rawScoreboardLocation
 ;
