class Component:
	instances = []

	def __init__(self):
		Component.instances.append(self)

	def OnClick(self, Position, Text):
		pass

class Font:
	instances = []

	def __init__(self):
		self.Name = None
		Font.instances.append(self)

class Mediator:
	instances = []

	def __init__(self):
		Mediator.instances.append(self)

	def ComponentClicked(self, Widget):
		pass

	def CreateComponents(self):
		pass

class User:
	instances = []

	def __init__(self):
		User.instances.append(self)

	def StartCase(self):
		FontDialogDirector1 = FontDialogDirector()
		FontDialogDirector1.ShowDialog()
		FontDialogDirector1.FontList.OnClick(2, None)
		FontDialogDirector1.FontName.OnClick(None, "Cambria")
		FontDialogDirector1.OkButton.OnClick(None, None)

class ListBox(Component):
	instances = []

	def __init__(self):
		self.Director = None
		self.ListItems = None
		self.ItemPosition = None
		ListBox.instances.append(self)

	def OnClick(self, Position, Text):
		self.ItemPosition = Position
		self.Director.ComponentClicked(self)

	def SetListItems(self, ListItems):
		self.ListItems = ListItems

class FontDialogDirector(Mediator):
	instances = []

	def __init__(self):
		self.FontList = None
		self.FontName = None
		self.OkButton = None
		self.FinalFont = None
		FontDialogDirector.instances.append(self)

	def ComponentClicked(self, Widget):
		if (Widget == self.FontList):
			Fonts = Font.instances
			FontListTemp = self.FontList
			Position = FontListTemp.ItemPosition
			i = 0
			for FontItem in Fonts:
				if (i == Position):
					self.FontName.SetText(FontItem.Name)
					break

				i = i + 1

		elif (Widget == self.FontName):
			Fonts = Font.instances
			Found = False
			i = 0
			FontNameTemp = self.FontName
			TypedFont = FontNameTemp.Text
			for FontItem in Fonts:
				if (FontItem.Name == TypedFont):
					TempList = self.FontList
					TempList.ItemPosition = i
					Found = True
					break

				i = i + 1

			if (not Found):
				NewFont = Font()
				NewFont.Name = TypedFont
				FontListTemp = self.FontList
				FontListTemp.ListItems.append(NewFont)

		elif (Widget == self.OkButton):
			self.CloseDialog()


	def CreateComponents(self):
		FontList = ListBox()
		self.FontList = FontList
		FontList.Director = self
		Font1 = Font()
		Font1.Name = "Times New Roman"
		Font2 = Font()
		Font2.Name = "Calibri"
		Font3 = Font()
		Font3.Name = "Arial"
		ListItems = [Font1, Font2, Font3]
		FontList.SetListItems(ListItems)
		FontName = EntryField()
		self.FontName = FontName
		FontName.Director = self
		FontName.SetText("")
		OkButton = Button()
		self.OkButton = OkButton
		OkButton.Director = self
		OkButton.SetText("OK")
		self.FinalFont = ""

	def ShowDialog(self):
		self.CreateComponents()

	def CloseDialog(self):
		FinalFontName = self.FontName
		self.FinalFont = FinalFontName.Text

class EntryField(Component):
	instances = []

	def __init__(self):
		self.Director = None
		self.Text = None
		EntryField.instances.append(self)

	def OnClick(self, Position, Text):
		self.SetText(Text)
		self.Director.ComponentClicked(self)

	def SetText(self, Text):
		self.Text = Text

class Button(Component):
	instances = []

	def __init__(self):
		self.Director = None
		self.Text = None
		Button.instances.append(self)

	def OnClick(self, Position, Text):
		self.Director.ComponentClicked(self)

	def SetText(self, Text):
		self.Text = Text

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
user = User()
user.StartCase()
