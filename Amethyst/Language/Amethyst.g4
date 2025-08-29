grammar Amethyst;

SPACE: [ \r\t\u000C\n]+ -> skip;
COMMENT: '#' ~[\r\n]* -> skip;
IDENTIFIER: [a-zA-Z] [a-zA-Z0-9_]*;

file
 : from* declaration* EOF
 ;
 
from
 : 'from' String_Literal String_Literal (',' String_Literal)* ';'
 ;

declaration
 : function_declaration
 | variable_declaration
 | record_declaration
 ;
 
function_declaration
 : attribute_list 'function' identifier '(' parameter_list? ')' (':' type)? block
 ;
 
variable_declaration
 : attribute_list 'var' identifier (':' type)? ('=' expression)? ';'
 ;
 
record_declaration
 : attribute_list 'record' identifier (':' type)? ('=' expression)? ';'
 ;
 
attribute_list
 : ('[' attribute (',' attribute)* ']')*
 ;
 
attribute
 : identifier
 ;
 
parameter_list
 : parameter (',' parameter)*
 ;
 
parameter
 : identifier ':' type
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
 : 'foreach' '(' identifier ':' expression ')' block
 ;

if_statement
 : 'if' '(' expression ')' block ('else' (block | if_statement))?
 ;
 
debug_statement
 : 'debug' expression ';'
 ;
 
comment_statement
 : 'comment' String_Literal ';'
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
 | primary_expression '.' identifier            # member_access
 | call                                         # call_expression
 | identifier                                   # identifier_expression
 | primary_expression '[' expression ']'        # indexed_access
 | primary_expression '++'                      # post_increment
 | primary_expression '--'                      # post_decrement
 ;
 
literal
 : boolean_literal
 | Integer_Literal
 | Decimal_Literal
 | String_Literal
 | Resource_Literal
 | object_creation
 | array_creation
 ;
 
boolean_literal
 : 'true'
 | 'false'
 ;
 
Integer_Literal
 : '0'..'9'+
 ;
 
Decimal_Literal
 : '0'..'9'* '.' '0'..'9'+
 ;
 
String_Literal
 : '"' .*? '"'
 ;
 
Resource_Literal
 : '`' .*? '`'
 ;

group
 : '(' expression ')'
 ;
 
call
 : identifier '(' argument_list? ')'
 ;
 
range_expression
 : expression '..' expression?
 | expression? '..' expression
 ; 

selector_query_list
 : selector_query (',' selector_query)*
 ;
 
selector_query
 : identifier '=' expression            # expression_selector
 | identifier '=' range_expression      # range_selector
 | identifier '=' record_selector_list  # records_selector
 ;
 
record_selector_list
 : '{}'
 | '{' (record_selector_element (',' record_selector_element)*)? '}'
 ;
 
record_selector_element
 : identifier ':' (expression | range_expression)
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
 
identifier
 : IDENTIFIER (':' IDENTIFIER ('/' IDENTIFIER)+ )   #absolute_identifier
 | IDENTIFIER ('/' IDENTIFIER)*                     #relative_identifier
 ;
 
type
 : ('int' | decimal | 'string' | 'bool' | 'array' | 'object') ('[]' | '{}')?
 ;
 
decimal
 : 'dec' ('(' Integer_Literal ')')?
 ;