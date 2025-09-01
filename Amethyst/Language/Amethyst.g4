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
 : function_declaration
 | variable_declaration
 | record_declaration
 ;
 
function_declaration
 : attribute_list 'function' IDENTIFIER '(' parameter_list? ')' (':' type)? block
 ;
 
variable_declaration
 : attribute_list 'var' IDENTIFIER (':' type)? ('=' expression)? ';'
 ;
 
record_declaration
 : attribute_list 'record' IDENTIFIER (':' type)? ('=' expression)? ';'
 ;
 
attribute_list
 : ('[' attribute (',' attribute)* ']')*
 ;
 
attribute
 : IDENTIFIER
 ;
 
parameter_list
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
 | for_statement
 | while_statement
 | foreach_statement
 | if_statement
 | debug_statement
 | comment_statement
 | return_statement
 | break_statement
 | continue_statement
 | block
 | expression_statement
 ;

for_statement
 : 'for' '(' (variable_declaration | expression_statement)? expression? ';' expression? ')' block
 ;
 
while_statement
 : 'while' '(' expression ')' block
 ;
 
foreach_statement
 : 'foreach' '(' IDENTIFIER ':' expression ')' block
 ;

if_statement
 : 'if' '(' expression ')' block ('else' (block | if_statement))?
 ;
 
debug_statement
 : 'debug' expression ';'
 ;
 
comment_statement
 : 'comment' STRING_LITERAL ';'
 ;
 
return_statement
 : 'return' expression? ';'
 ;
 
break_statement
 : 'break' ';'
 ;

continue_statement
 : 'continue' ';'
 ;
 
expression_statement
 : expression ';'
 ;

expression
 : conditional_expression
 | assignment_expression
 ;
 
assignment_expression
 : unary_expression ( '=' | '+=' | '-=' | '*=' | '/=' | '%=' ) expression
 ;
 
conditional_expression
 : or_expression
 ;
 
or_expression
 : and_expression ( '||' and_expression )*
 ;
 
and_expression
 : equality_expression ( '&&' equality_expression )*
 ;
 
equality_expression
 : comparison_expression ( ( '==' | '!=' ) comparison_expression )*
 ;

comparison_expression
 : term_expression ( ( '<' | '<=' | '>' | '>=' ) term_expression )*
 ;
 
term_expression
 : factor_expression ( ( '+' | '-' ) factor_expression )*
 ;
 
factor_expression
 : unary_expression ( ( '*' | '/' | '%' ) unary_expression )*
 ;
 
unary_expression
 : primary_expression
 | '++' unary_expression
 | '--' unary_expression
 | ( '+' | '-' | '!' ) unary_expression
 ;
 
primary_expression
 : literal                                      # literal_expression
 | group                                        # group_expression
 | selector_type ('[' selector_query_list ']')? # selector_expression
 | primary_expression '.' IDENTIFIER            # member_access
 | call                                         # call_expression
 | IDENTIFIER                                   # identifier_expression
 | primary_expression '[' expression ']'        # indexed_access
 | primary_expression '++'                      # post_increment
 | primary_expression '--'                      # post_decrement
 ;
 
literal
 : boolean_literal
 | INTEGER_LITERAL
 | DECIMAL_LITERAL
 | STRING_LITERAL
 | RESOURCE_LITERAL
 | object_creation
 | array_creation
 ;
 
boolean_literal
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
 
call
 : IDENTIFIER '(' argument_list? ')'
 ;
 
range_expression
 : expression '..' expression?
 | expression? '..' expression
 ; 

selector_query_list
 : selector_query (',' selector_query)*
 ;
 
selector_query
 : IDENTIFIER '=' expression            # expression_selector
 | IDENTIFIER '=' range_expression      # range_selector
 | IDENTIFIER '=' record_selector_list  # records_selector
 ;
 
record_selector_list
 : '{}'
 | '{' (record_selector_element (',' record_selector_element)*)? '}'
 ;
 
record_selector_element
 : IDENTIFIER ':' (expression | range_expression)
 ;
 
selector_type
 : '@s'
 | '@r'
 | '@a'
 | '@e'
 | '@p'
 | '@n'
 ;
 
argument_list
 : expression (',' expression)*
 ;
  
object_creation
 : '{}'
 | '{' (object_element (',' object_element)*)? '}'
 ;
 
object_element
 : IDENTIFIER ':' expression
 ;

array_creation
 : '[]'
 | '[' (expression (',' expression)* )? ']'
 ;
 
type
 : ('int' | decimal | 'string' | 'bool' | 'array' | 'object') ('[]' | '{}')?
 ;
 
decimal
 : 'dec' ('(' INTEGER_LITERAL ')')?
 ;