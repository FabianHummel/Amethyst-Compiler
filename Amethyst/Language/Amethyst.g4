grammar Amethyst;

SPACE: [ \r\t\u000C\n]+ -> skip;
COMMENT: '#' ~[\r\n]* -> skip;
IDENTIFIER: [a-zA-Z] [a-zA-Z0-9_]*;

file
 : declaration* EOF
 ;
 
declaration
 : namespace_declaration
 | function_declaration
 | statement
 ;
 
namespace_declaration
 : 'namespace' namespace_identifier '{' declaration* '}'
 | 'namespace' namespace_identifier ';' declaration*
 ;
 
namespace_identifier
 : identifier ('::' identifier)*
 ;
 
function_declaration
 : attribute_list* 'function' identifier '(' parameter_list? ')' (':' type)? block
 ;
 
attribute_list
 : '[' attribute (',' attribute)* ']'
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
 : 'var' identifier (':' type)? ('=' expression)? ';'
 ;
 
record_declaration
 : 'record' identifier (':' type)? ('=' expression)? ';'
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
 : 'comment' expression ';'
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
 : conditional
 | assignment
 ;
 
assignment
 : unary ( '=' | '+=' | '-=' | '*=' | '/=' | '%=' ) expression
 ;
 
conditional
 : or
 ;
 
or
 : and ( '||' and )*
 ;
 
and
 : equality ( '&&' equality )*
 ;
 
equality
 : comparison ( ( '==' | '!=' ) comparison )*
 ;

comparison
 : term ( ( '<' | '<=' | '>' | '>=' ) term )*
 ;
 
term
 : factor ( ( '+' | '-' ) factor )*
 ;
 
factor
 : unary ( ( '*' | '/' | '%' ) unary )*
 ;
 
unary
 : primary
 | ( '+' | '-' | '!' ) unary
 | '++' unary
 | '--' unary
 ;
 
primary
 : literal                      # literal_expression
 | group                        # group_expression
 | selector                     # selector_expression
 | primary '.' identifier       # member_access
 | function_call                # function_call_expression
 | namespace_access             # identifier_expression
 | primary '[' expression ']'   # indexed_access
 | object_creation              # object_creation_expression
 | array_creation               # array_creation_expression
 | primary '++'                 # post_increment
 | primary '--'                 # post_decrement
 ;
 
literal
 : boolean_literal
 | Integer_Literal
 | Decimal_Literal
 | String_Literal
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

group
 : '(' expression ')'
 ;
 
function_call
 : namespace_access '(' argument_list? ')'
 ;
 
namespace_access
 : identifier ('::' identifier)*
 ;
 
selector
 : selector_type ('[' selector_query_list ']')?      # selector_specification
 | selector '.' identifier                       # selector_member_access
 ;
 
selector_query_list
 : identifier '=' expression (',' identifier '=' expression)*
 ;
 
selector_type
 : '@s'
 | '@r'
 | '@a'
 | '@e'
 | '@p'
 ;
 
argument_list
 : expression (',' expression)*
 ;
  
object_creation
 : '{}'
 | '{' identifier ':' expression '}'
 ;

array_creation
 : '[]'
 | '[' (expression (',' expression)*)? ']'
 ;
 
identifier
 : IDENTIFIER
 ;
 
type
 : ('int' | 'dec' | 'string' | 'bool' | 'array' | 'object') ('[]' | '{}')?
 ;