class AST:
	instances = []

	def __init__(self):
		self.code = None
		AST.instances.append(self)

	def execute(self, operationEvaluator, scope):
		astleaf_1 = ASTLeaf()
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
		pass

class OperationEvaluator:
	instances = []

	def __init__(self):
		OperationEvaluator.instances.append(self)

	def evaluateOperation(self, operator, operands):
		pass

	def tellType(self, value):
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

