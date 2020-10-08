grammar OAL;

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
	|	continueCommand
	|	breakCommand
	|	whileCommand
	|	ifCommnad
	|	foreachCommand
	|	parCommand
	;
	
parCommand
	:	'par'('thread' line+ 'end thread;')+ 'end par;'
	;

ifCommnad
	:	'if''('expr')' line* ('elif''('expr')' line+)* ('else' line+)? 'end if;'
	;

whileCommand
	:	'while''('expr')' line+ 'end while;'
	;
	
foreachCommand
	:	'for each' variableName ' in ' variableName line+ 'end for;'
	;

continueCommand
	:	'continue;'
	;

breakCommand
	:	'break;'
	;

exeCommandQueryCreate
	:	'create object instance 'instanceHandle' of 'keyLetter';'
	|	'create object instance of 'keyLetter';'
	;

exeCommandQueryRelate
	:	'relate 'instanceHandle' to 'instanceHandle' across 'relationshipSpecification';'
	;

exeCommandQuerySelect
	:	'select any 'instanceHandle' from instances of 'keyLetter (' where ' whereExpression)?';'
	|	'select many 'instanceHandle' from instances of 'keyLetter (' where ' whereExpression)?';'
	;

exeCommandQuerySelectRelatedBy
	:	'select any 'instanceHandle' related by 'start'->'className relationshipLink ('->'className relationshipLink)* (' where ' whereExpression)?';'
	|	'select many 'instanceHandle' related by 'start'->'className relationshipLink ('->'className relationshipLink)* (' where ' whereExpression)?';'
	;

exeCommandQueryDelete
	:	'delete object instance 'instanceHandle';'
	;

exeCommandQueryUnrelate
	:	'unrelate 'instanceHandle' from 'instanceHandle' across 'relationshipSpecification';'
	;

exeCommandAssignment
	:	variableName'='expr';'
	|	'assign 'variableName'='expr';'
	|	instanceHandle'.'atribute'='expr';'
	|	'assign 'instanceHandle'.'atribute'='expr';'
	;

exeCommandCall
	:	'call from 'keyLetter'::'methodName'() to 'keyLetter'::'methodName'()'(' across 'relationshipSpecification)?';'
	;

commands
	:	'create object instance 'instanceHandle' of 'keyLetter';'
	|	'create object instance of 'keyLetter';'
	|	'relate 'instanceHandle' to 'instanceHandle' across 'relationshipSpecification';'
	|	'unrelate 'instanceHandle' from 'instanceHandle' across 'relationshipSpecification';'
	|	'select any 'instanceHandle' from instances of 'keyLetter (' where ' whereExpression)?';'
	|	'select many 'instanceHandle' from instances of 'keyLetter (' where ' whereExpression)?';'
	|	'select any 'instanceHandle' related by 'start'->'className relationshipLink ('->'className relationshipLink)* (' where ' whereExpression)?';'
	|	'select many 'instanceHandle' related by 'start'->'className relationshipLink ('->'className relationshipLink)* (' where ' whereExpression)?';'
	|	'delete object instance 'instanceHandle';'
	|	variableName'='expr';'
	|	instanceHandle'.'atribute'='expr';'
	|	'assign 'variableName'='expr';'
	|	'assign 'instanceHandle'.'atribute'='expr';'
	|	'call from 'keyLetter'::'methodName'() to 'keyLetter'::'methodName'()'(' across 'relationshipSpecification)?';'
	;

relationshipLink
    :    '['RelationshipSpecification']'
    ;

instanceHandle
	:	VariableName
	;

keyLetter
	:	VariableName
	;

whereExpression
	:	expr
	;

start
	:	VariableName
	;

className
	:	VariableName
	;

variableName
	:	VariableName
	;

methodName
	:	VariableName
	;

anyOrMany
	:	AnyOrMany
	;

atribute
	:	VariableName
	;

expr
	:	 Digit | VariableName | Text
    |    VariableName'.'VariableName
	|	 'cardinality 'VariableName
	|	 expr ('*' | '/' | '%') expr
    |    expr ('+' | '-') expr
    |    expr ('<' | '>' | '<=' | '>=') expr
	|	 ('empty ' | 'not_empty ')VariableName
	|    ('NOT ' | 'not ') expr
    |    expr ('==' | '!=') expr
	|	 '(' expr ')'
    |    expr ('AND' | 'OR' | 'and' | 'or') expr
    ;

relationshipSpecification
    :    RelationshipSpecification
    ;

AnyOrMany
	:	('any'|'many')
	;

RelationshipSpecification
	:	'R'Digit
	;

VariableName
    :   Nondigit ( Nondigit | Digit )*
    ;

Text
	:	'"'( Nondigit | Digit | ' ')*'"'
	;

Digit
    :     [0-9]+('.'[0-9]+)?
    ;

Nondigit
    :     [a-zA-Z_#]
    ;

Whitespace
	:    [ \t]+ 
	-> skip
	;

NewLine
	:	( '\r'? '\n' | '\r')+
	-> skip
	;
