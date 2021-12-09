using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime;
using System.Linq;//

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            String exp = "create object instance observer1 of Observer; \n" +
                         "select any dog from instances of Dog; \n" +
                         "select many all_dogs from instances of Dog; \n" +
                         "select any young_dog from instances of Dog where selected.age < 5; \n" +
                         "call from Subject::register() to Subject::addObserver(); \n" +
                         "x = cardinality dog;";

            AntlrInputStream inputStream = new AntlrInputStream(exp);
            NewGrammarLexer lexer = new NewGrammarLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
            NewGrammarParser parser = new NewGrammarParser(commonTokenStream);
            //parser.BuildParseTree = true;//

            //OALToPythonParser.CodeContext result = parser.code();//
            //Console.WriteLine(result.ToStringTree());//
            //Console.WriteLine();//

            OALToPythonVisitor visitor = new OALToPythonVisitor(new List<string>());//
            var result = visitor.Visit(parser.lines());
            //Console.WriteLine(ioal.Visit(parser.code()));
            Console.WriteLine(result);

            //Priklad ockovania psikov
            /*Dictionary<string, Dictionary<String, String>> Classes = new Dictionary<string, Dictionary<String, String>>();
            Dictionary<string, List<String>> ClassesAttributes = new Dictionary<string, List<String>>();
            Dictionary<string, List<String>> MethodsParameters = new Dictionary<string, List<String>>();

            //trieda Veterinarian
            Dictionary<string, string> methodsVet = new Dictionary<string, string>();
            methodsVet.Add("registerDog", "if (registered_dogs == UNDEFINED) \n" +
                                                "create list registered_dogs of Dog; \n" +
                                          "else \n" +
                                                "add dog to registered_dogs;" +
                                          "end if;");
            methodsVet.Add("chipDogs", "for each dog in registered_dogs \n" +
                                            "dog.update(); \n " +
                                       "end for; \n");

            MethodsParameters.Add("registerDog", new List<string> { "dog" });
            MethodsParameters.Add("chipDogs", new List<string>());

            Classes.Add("Veterinarian", methodsVet);

            ClassesAttributes.Add("Veterinarian", new List<string> { "registered_dogs" });

            //trieda Dog
            Dictionary<string, string> methodsDog = new Dictionary<string, string>();
            methodsDog.Add("update", "chipped = TRUE;");

            MethodsParameters.Add("update", new List<string>());

            Classes.Add("Dog", methodsDog);

            ClassesAttributes.Add("Dog", new List<string> { "chipped" });

            // trieda Owner
            Dictionary<string, string> methodsOwner = new Dictionary<string, string>();
            methodsOwner.Add("buyPetDog", "create object instance petDog of Dog;");
            methodsOwner.Add("registerPetDog", "vet.registerDog(petDog);");

            MethodsParameters.Add("buyPetDog", new List<string>());
            MethodsParameters.Add("registerPetDog", new List<string> { "vet" });

            Classes.Add("Owner", methodsOwner);

            ClassesAttributes.Add("Owner", new List<string> { "petDog" });


            StringBuilder Code = new StringBuilder();

            foreach (KeyValuePair<string, Dictionary<String, String>> classItem in Classes)
            {
                Code.AppendLine("class " + classItem.Key + ":");
                Code.AppendLine("\t" + "instances = []");
                Code.AppendLine();

                Code.AppendLine("\t" + "def __init__(self):");

                //atributy
                List<String> attributesList = ClassesAttributes[classItem.Key];
                foreach (String attribute in attributesList)
                {
                    Code.AppendLine("\t\t" + "self." + attribute + " = None");
                }
                Code.AppendLine("\t\t" + classItem.Key + ".instances.append(self)");
                Code.AppendLine();

                //prechod cez metody
                foreach (KeyValuePair<string, String> methodItem in classItem.Value)
                {
                    Code.Append("\t" + "def " + methodItem.Key);
                    //parametre
                    List<String> Parameters = MethodsParameters[methodItem.Key];

                    if (Parameters.Any())
                    {
                        Code.AppendLine("(self, " + String.Join(", ", Parameters) + "):");
                    }
                    else
                    {
                        Code.AppendLine("(self):");
                    }

                    AntlrInputStream inputStream = new AntlrInputStream(methodItem.Value);
                    NewGrammarLexer lexer = new NewGrammarLexer(inputStream);
                    CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
                    NewGrammarParser parser = new NewGrammarParser(commonTokenStream);
                    //poslat atributy
                    OALToPythonVisitor visitor = new OALToPythonVisitor(attributesList);//
                    String result = visitor.Visit(parser.lines());
                    Code.AppendLine(result); //telo metody
                }
            }

            Code.AppendLine("def boolean(value):");
            Code.AppendLine("\t" + "if value == \"True\":");
            Code.AppendLine("\t\t" + "return True");
            Code.AppendLine("\t" + "elif value == \"False\":");
            Code.AppendLine("\t\t" + "return False");
            Code.AppendLine();

            Code.AppendLine("def cardinality(variable):");
            Code.AppendLine("\t" + "if isinstance(variable, list):");
            Code.AppendLine("\t\t" + "return len(variable)");
            Code.AppendLine("\t" + "elif hasattr(variable, '__dict__'):");
            Code.AppendLine("\t\t" + "return 1");
            Code.AppendLine("\t" + "else:");
            Code.AppendLine("\t\t" + "return 0");
            Code.AppendLine();

            Console.WriteLine(Code.ToString());*/



            //navrh vytvorenia python scriptu

            /*Anim selectedAnimation = AnimationData.Instance.selectedAnim;
            List<AnimClass> MethodsCodes = selectedAnimation.GetMethodsCodesList();
            //treba tento list Diagramclasses doplnit aj do Anim.cs aby bol serializable a treba donho dat mena class v stringu
            List<String> DiagramClasses = selectedAnimation.GetDiagramClasses();
            //chceme stringbuilder pouzit aj pri metodach?
            StringBuilder Code = new StringBuilder();

            //treba asi spravit cez vsetky triedy v tom diagrame a potom pozerat ci sa ta trieda nachadza v MethodCodes
            foreach (String className in DiagramClasses)   //Filip
            {
                
                Code.AppendLine("class " + className + ":");
                Code.AppendLine("\t" + "instances = []");
                Code.AppendLine();

                //tu asi dame atributy, je to spravene pre instance attributy
                Code.AppendLine("\t" + "def __init__(self):");

                CDClass Class = OALProgram.Instance.ExecutionSpace.getClassByName(className);
                List<String> attributesList = Class.Attributes.Select(item => item.Name).ToList());
                foreach (CDAttribute attribute in Class.Attributes)
                {
                     Code.AppendLine("\t\t" + "self." + attribute.Name + " = None"); //diskutovat ohladom uz danych dat
                }
                Code.AppendLine("\t\t" + className + ".instances.append(self)");

                Code.AppendLine();

                AnimClass classItem = MethodCodes.FirstOrDefault(c => c.Name.Equals(className));

                //foreach (AnimMethod methodItem in classItem.Methods)
                //{
                foreach (CDMethod methodItem in Class.Methods)
                {
                
                    Code.Append("\t" + "def " + methodItem.Name);
                    //List<CDParameter> Parameters = Class.getMethodByName(methodItem.Name).Parameters;
                    List<CDParameter> Parameters = methodItem.Parameters;

                    if (Parameters.Any())
                    {
                         Code.AppendLine("(self, " + String.Join(", ", Parameters.Select(item => item.Name).ToList()) + "):");
                    }
                    else
                    {
                         Code.AppendLine("(self):");
                    }
                    
                    //tu by mohli byt podmienky ze ak metoda nema telo tak dame pass a ak ma tak 
                    //sa zavola ten preklad vyssie
                    
                    if (classItem != null)
                    { 
                        AnimMethod Method = classItem.Methods.FirstOrDefault(m => m.Name.Equals(methodItem.Name));
                        if (Method != null)
                        {   
                            //Method.code
                            //OALToPythonVisitor2 visitor = new OALToPythonVisitor2(attributesList);
                            var result = visitor.Visit(parser.code());
                             Code.AppendLine(result); //telo metody
                        }
                        else
                        {
                             Code.AppendLine("\t\t" + "pass");
                             Code.AppendLine();
                        }
                    }
                    else
                    {
                        Code.AppendLine("\t\t" + "pass");
                        Code.AppendLine();
                    }
                }
            }

            Code.AppendLine("def boolean(value):");
            Code.AppendLine("\t" + "if value == \"True\":");
            Code.AppendLine("\t\t" + "return True");
            Code.AppendLine("\t" + "elif value == \"False\":");
            Code.AppendLine("\t\t" + "return False");
            Code.AppendLine();

            Code.AppendLine("def cardinality(variable):");
            Code.AppendLine("\t" + "if isinstance(variable, list):");
            Code.AppendLine("\t\t" + "return len(variable)");
            Code.AppendLine("\t" + "elif hasattr(variable, '__dict__'):");     //asi takto, mozno sa opytat
            Code.AppendLine("\t\t" + "return 1");
            Code.AppendLine("\t" + "else:");
            Code.AppendLine("\t\t" + "return 0");
            Code.AppendLine();

            return Code.ToString();


            //v menumanagerovi
            fileLoader.SavePythonCode(Code);

            //vo FileLoader, asi sa nieco prida aj do metody start() aby tam bol aj python
            IEnumerator SavePythonCode(string code)
            {
                FileBrowser.SetDefaultFilter(".py");
                yield return FileBrowser.WaitForSaveDialog(false, @"Assets\Resources\Animations\", "Save Animation", "Save"); //tu bude nieco ine
                if (FileBrowser.Success)
                {
                    string path = FileBrowser.Result;
                    string fileName = FileBrowserHelpers.GetFilename(FileBrowser.Result);
                    //newAnim.AnimationName = FileBrowserHelpers.GetFilename(FileBrowser.Result).Replace(".json", "");
                    File.WriteAllText(path, code);      //predtym: newAnim.SaveCode(path);
                    //FileBrowserHelpers.CreateFileInDirectory(@"Assets\Resources\Animations\",fileName);
                    //HandleTextFile.WriteString(path, newAnim.Code);
                    //AnimationData.Instance.AddAnim(newAnim);
                    //AnimationData.Instance.selectedAnim = newAnim;
                    //MenuManager.Instance.UpdateAnimations();
                    //MenuManager.Instance.SetSelectedAnimation(newAnim.AnimationName);
                }
            }*/

        }
    }
}
