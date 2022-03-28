grammar OAL;

// Parser Rules
//treba asi podoplnat commenty aj do jednotlivych pravidiel //asi netreba

lines
	:	line+ EOF
	;

line
	:	exeCommandQueryCreate
	|	exeCommandQueryRelate
	|	exeCommandQuerySelect
	|	exeCommandQuerySelectRelatedBy
	|	exeCommandQueryDelete
	|	exeCommandQueryUnrelate
	|	exeCommandAssignment
	|	exeCommandCall
	|	exeCommandCreateList
	|	exeCommandAddingToList
    |	exeCommandRemovingFromList
	|	exeCommandWrite
	|	exeCommandRead
    |	returnCommand
	|	continueCommand
	|	breakCommand
	|	whileCommand
	|	ifCommand
	|	foreachCommand
	|	parCommand
	|	commentCommand
	;

parCommand
    :   'par' 'thread' line+ 'end thread' ';' ('thread' line+ 'end thread' ';')+ 'end par' ';'
    ;

ifCommand
    :   'if' expr line* ('elif' '(' expr ')' line+)* ('else' line+)? 'end if' ';'  //zatvorky maju byt povinne alebo nemusia? moze byt v ifelse aj * pri line?
    ;

whileCommand
    :   'while' '(' expr ')' line+ 'end while' ';'
    ;

foreachCommand
    :   'for each ' variableName ' in ' instanceHandle line+ 'end for' ';'
    ;

continueCommand
    :   'continue' ';'
    ;

breakCommand
    :   'break' ';'
    ;

commentCommand
    :   COMMENT
    ;

exeCommandQueryCreate
    :   'create object instance ' instanceHandle ' of ' keyLetter ';'
    |   'create object instance of ' keyLetter ';'
    ;

exeCommandQueryRelate
    :   'relate ' instanceHandle ' to ' instanceHandle ' across ' relationshipSpecification ';'
    ;

exeCommandQuerySelect
    :   'select any ' instanceHandle ' from instances of ' keyLetter (' where ' whereExpression)? ';'
    |   'select many ' instanceHandle ' from instances of ' keyLetter (' where ' whereExpression)? ';'
    ;

exeCommandQuerySelectRelatedBy
    :   'select any ' instanceHandle ' related by ' instanceHandle '->' className relationshipLink ('->' className relationshipLink)* (' where ' whereExpression)? ';'
    |   'select many ' instanceHandle ' related by ' instanceHandle '->' className relationshipLink ('->' className relationshipLink)* (' where ' whereExpression)? ';'
    ;

exeCommandQueryDelete
    :   'delete object instance ' instanceHandle ';'
    ;

exeCommandQueryUnrelate
    :   'unrelate ' instanceHandle ' from ' instanceHandle ' across ' relationshipSpecification ';'
    ;

exeCommandAssignment
    :   ('assign ')? instanceHandle '=' expr ';'
    ;

exeCommandCall
    :   instanceHandle '.' methodName '(' (expr (',' expr)*)? ')' ';'
    // |   'call from ' keyLetter '::' methodName '(' ')' ' to ' keyLetter '::' methodName '(' ')' (' across ' relationshipSpecification)? ';'
    ;

exeCommandCreateList
    :   'create list ' instanceHandle ' of ' keyLetter ('{' instanceHandle (',' instanceHandle)* '}')? ';'
    ;

exeCommandAddingToList
    :   'add ' instanceHandle ' to ' instanceHandle ';'
    ;

exeCommandRemovingFromList
    :   'remove ' instanceHandle ' from ' instanceHandle ';'
    ;

exeCommandWrite
    :   'write' '(' (expr (',' expr)*)? ')' ';'
    ;

exeCommandRead
    :   ('assign ')? instanceHandle '=' (
                                        'read(' expr? ')'
                                        | 'int(read(' expr? ')' ')'
                                        | 'real(read(' expr? ')' ')'
                                        | 'bool(read(' expr? ')' ')'
                                        ) ';'
    ;

returnCommand
    :   'return' expr? ';'
    ;

expr
    :   NUM | NAME | BOOL | STRING
    |   NAME '.' NAME
    |   'cardinality ' instanceHandle
    |   ('empty ' | 'not_empty ') instanceHandle
    |   '(' expr ')'
    |   '-' expr 
    |   expr ('*' | '/' | '%') expr
    |   expr ('+' | '-')  expr
    |   expr ('<' | '>' | '<=' | '>=' | '==' | '!=') expr
    |   ('not ' | 'NOT ') expr
    |   expr (' and ' | ' AND ') expr
    |   expr (' or ' | ' OR ') expr
    ;

instanceHandle
    :   instanceName
    |   instanceName '.' attribute
    ;

instanceName
    :   NAME
    ;

keyLetter
    :   NAME
    ;

whereExpression
    :   expr
    ;

className
    :	NAME
    ;

variableName
    :	NAME
    ;

methodName
    :	NAME
    ;

attribute
    :	NAME
    ;

relationshipLink
    :   '['RELATIONSHIP_SPECIFICATION']'
    ;

relationshipSpecification
    :   RELATIONSHIP_SPECIFICATION
    ;

// Lexer Rules

RELATIONSHIP_SPECIFICATION
    :   'R'[0-9]+
    ;

BOOL
    :   'TRUE' | 'FALSE'
    ;

NAME
    :   [a-zA-Z_#][a-zA-Z0-9_#]*
    ;

STRING
    :   '"' (~'"' | '\\"')* '"'
    ;

NUM
    :   INT | DECIMAL
    ;

fragment INT
    :   '0' | [1-9][0-9]*
    ;
fragment DECIMAL
    :   [0-9]+'.'[0-9]+  //   '0''.'[0-9]+ | [1-9][0-9]*'.'[0-9]+
    ;

COMMENT
    :   '//' ~('\r' | '\n')*  // druhy priklad:  '//' .*? '/n'
    ;

WHITE_SPACE
    :   [ \t\r\n]+      // skip spaces, tabs, newlines
    -> skip
    ;