grammar Amethyst;

SPACE: [ \r\t\u000C\n]+ -> skip;
COMMENT: '#' ~[\r\n]* -> skip;
IDENTIFIER: [a-zA-Z] [a-zA-Z0-9_]*;

file
 : namespace_declaration? declaration* EOF
 ;
 
namespace_declaration
 : 'namespace' identifier ';'
 ;

declaration
 : function_declaration
 | statement
 ;
 
function_declaration
 : attribute_list 'function' identifier '(' parameter_list? ')' (':' type)? block
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
 : variable_declaration
 | record_declaration
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
 
variable_declaration
 : attribute_list 'var' identifier (':' type)? ('=' expression)? ';'
 ;
 
record_declaration
 : attribute_list 'record' identifier (':' type)? ('=' expression)? ';'
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
 | namespace_access                             # identifier_expression
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
 : namespace_access '(' argument_list? ')'
 ;
 
range_expression
 : expression '..' expression?
 | expression? '..' expression
 ;
 
namespace_access
 : identifier ('::' identifier)?
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
 : identifier ':' expression
 ;

array_creation
 : '[]'
 | '[' (expression (',' expression)*)? ']'
 ;
 
identifier
 : IDENTIFIER
 ;
 
type
 : ('int' | decimal | 'string' | 'bool' | 'array' | 'object') ('[]' | '{}')?
 ;
 
decimal
 : 'dec' ('(' Integer_Literal ')')?
 ;