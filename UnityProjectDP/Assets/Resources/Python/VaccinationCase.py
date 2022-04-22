class Client:
	instances = []

	def __init__(self):
		Client.instances.append(self)

	def StartCase(self):
		Vet = Veterinarian()
		Vet.RegisteredAnimals = []
		Husky = Dog()
		Husky.Veterinarian = Vet
		Husky.IsVaccinated = False
		Vet.Register(Husky, "1.4.2022")
		PersianCat = Cat()
		PersianCat.Veterinarian = Vet
		PersianCat.IsVaccinated = False
		Vet.Register(PersianCat, "2.4.2022")
		Vet.SetDate("1.4.2022")
		Vet.SetDate("2.4.2022")

class Observer:
	instances = []

	def __init__(self):
		Observer.instances.append(self)

	def ReceiveVaccine(self, Date):
		pass

class Subject:
	instances = []

	def __init__(self):
		Subject.instances.append(self)

	def Register(self, Obs, Date):
		pass

	def Unregister(self, Obs):
		pass

	def VaccinateAnimals(self):
		pass

class Veterinarian(Subject):
	instances = []

	def __init__(self):
		self.CurrentDate = None
		self.RegisteredAnimals = None
		Veterinarian.instances.append(self)

	def Register(self, Obs, Date):
		self.RegisteredAnimals.append(Obs)
		Obs.SetVaccinationDate(Date)

	def Unregister(self, Obs):
		self.RegisteredAnimals = [instance for instance in self.RegisteredAnimals if instance != Obs]
		Obs.SetVaccinationDate(None)

	def VaccinateAnimals(self):
		for Animal in self.RegisteredAnimals:
			Animal.ReceiveVaccine(self.CurrentDate)


	def SetDate(self, Date):
		self.CurrentDate = Date
		self.VaccinateAnimals()

class Dog(Observer):
	instances = []

	def __init__(self):
		self.Veterinarian = None
		self.IsVaccinated = None
		self.Name = None
		self.VaccinationDate = None
		Dog.instances.append(self)

	def ReceiveVaccine(self, Date):
		if (self.VaccinationDate == Date):
			self.IsVaccinated = True


	def SetVaccinationDate(self, Date):
		self.VaccinationDate = Date

class Cat(Observer):
	instances = []

	def __init__(self):
		self.Veterinarian = None
		self.IsVaccinated = None
		self.Name = None
		self.VaccinationDate = None
		Cat.instances.append(self)

	def ReceiveVaccine(self, Date):
		if (self.VaccinationDate == Date):
			self.IsVaccinated = True


	def SetVaccinationDate(self, Date):
		self.VaccinationDate = Date

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

# MAIN
client = Client()
client.StartCase()
