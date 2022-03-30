class AST:
	instances = []

	def __init__(self):
		self.code = None
		AST.instances.append(self)

	def execute(self):
		astleaf_1 = ASTLeaf()
		x = input("insert value: ")
		astleaf_1.value = x
		print(astleaf_1.value, sep='')
		astleaf_1.getType()

	def printPretty(self):
		pass

class ASTComposite(AST):
	instances = []

	def __init__(self):
		ASTComposite.instances.append(self)

class ASTLeaf(AST):
	instances = []

	def __init__(self):
		self.type = None
		self.value = None
		ASTLeaf.instances.append(self)

	def getType(self):
		operationevaluator_1 = OperationEvaluator()
		operationevaluator_1.evaluateOperation()

class OperationEvaluator:
	instances = []

	def __init__(self):
		OperationEvaluator.instances.append(self)

	def evaluateOperation(self):
		pass

	def tellType(self):
		pass

class Operator:
	instances = []

	def __init__(self):
		self.operator = None
		Operator.instances.append(self)

def boolean(value):
	if value == "True":
		return True
	elif value == "False":
		return False
	raise ValueError("could not convert string to boolean: '" + value + "'")

def cardinality(variable):
	if isinstance(variable, list):
		return len(variable)
	elif hasattr(variable, '__dict__'):
		return 1
	else:
		return 0

#main
ast = AST()
ast.execute()
