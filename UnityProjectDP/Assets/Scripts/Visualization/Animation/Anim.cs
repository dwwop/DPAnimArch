//Data structure for single animation

using OALProgramControl;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.AnimationControl.OAL;

[System.Serializable]
public struct Anim
{
    [SerializeField]
    public string Code; //{ set; get; }
    [SerializeField]
    public string AnimationName; //{ set; get; }
    [SerializeField]
    public string StartClass; //{ set; get; }
    [SerializeField]
    public string StartMethod; //{ set; get; }
    [SerializeField]
    private List<AnimClass> MethodsCodes;
    public Anim (string animation_name, string code)
    {
        Code = code;
        AnimationName = animation_name;
        StartClass = "";
        StartMethod = "";
        MethodsCodes = new List<AnimClass>();
    }
    public Anim(string animation_name)
    {
        AnimationName = animation_name;
        Code = "";
        StartClass = "";
        StartMethod = "";
        MethodsCodes = new List<AnimClass>();
    }

    public void Initialize()
    {
        List<CDClass> ClassPool = OALProgram.Instance.ExecutionSpace.ClassPool;

        if (ClassPool.Any())
        {
            List<Relation> Relations = ClassDiagram.Instance.GetRelationList().Where(r => ("Generalization".Equals(r.PropertiesEa_type) || "Realisation".Equals(r.PropertiesEa_type))).ToList();//

            string SuperClass;
            Relation Relation;
            List<string> Attributes;
            List<AnimMethod> Methods;
            List<string> Parameters;

            foreach (CDClass ClassItem in ClassPool)
            {
                SuperClass = "";
                Relation = Relations.FirstOrDefault(r => r.FromClass.Equals(ClassItem.Name));//
                if (Relation != null)//
                {
                    SuperClass = Relation.ToClass;
                }
                Attributes = ClassItem.Attributes.Select(a => a.Name).ToList();

                Methods = new List<AnimMethod>();
                foreach (CDMethod MethodItem in ClassItem.Methods)
                {
                    Parameters = MethodItem.Parameters.Select(p => p.Name).ToList();
                    Methods.Add(new AnimMethod(MethodItem.Name, Parameters, ""));
                }

                MethodsCodes.Add(new AnimClass(ClassItem.Name, SuperClass, Attributes, Methods));
            }
        }
    }

    public void SetMethodCode(string className, string methodName, string code)
    {
        int index = methodName.IndexOf("(");
        methodName = methodName.Substring(0, index); // remove "(...)" from method name

        AnimClass classItem = MethodsCodes.FirstOrDefault(c => c.Name.Equals(className));   //alebo SingleOrDefault
        if (classItem != null)
        {
            AnimMethod methodItem = classItem.Methods.FirstOrDefault(m => m.Name.Equals(methodName));  //alebo SingleOrDefault
            if (methodItem != null)
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    methodItem.Code = "";

                    CDMethod Method = OALProgram.Instance.ExecutionSpace.getClassByName(className).getMethodByName(methodName);
                    Method.ExecutableCode = null;
                }
                else
                {
                    methodItem.Code = code;
                }
            }
        }
    }

    public string GetMethodBody(string className, string methodName)
    {
        int index = methodName.IndexOf("(");
        methodName = methodName.Substring(0, index); // remove "(...)" from method name

        AnimClass classItem = MethodsCodes.FirstOrDefault(c => c.Name.Equals(className));   //alebo SingleOrDefault
        if (classItem != null)
        {
            AnimMethod methodItem = classItem.Methods.FirstOrDefault(m => m.Name.Equals(methodName));  //alebo SingleOrDefault
            if (methodItem != null)
            {
                return methodItem.Code;
            }
        }
        return "";  // className or methodName does not exist
    }

    public List<AnimClass> GetMethodsCodesList()
    {
        return MethodsCodes;
    }

    // Return Methods that have a code
    public List<AnimMethod> GetMethodsByClassName(string className)
    {
        List<AnimMethod> Methods = null;
        AnimClass classItem = MethodsCodes.FirstOrDefault(c => c.Name.Equals(className));   //alebo SingleOrDefault

        if (classItem != null)
        {
            Methods = new List<AnimMethod>();

            foreach (AnimMethod methodItem in classItem.Methods)
            {
                if (!string.IsNullOrEmpty(methodItem.Code))
                {
                    Methods.Add(methodItem);
                }
            }
        }
        return Methods;
    }

    public void SetStartClassName(string startClassName)
    {
        if (string.IsNullOrWhiteSpace(startClassName))
        {
            StartClass = "";
        }
        else
        {
            StartClass = startClassName;
        }
    }

    public void SetStartMethodName(string startMethodName)
    {
        if (string.IsNullOrWhiteSpace(startMethodName))
        {
            StartMethod = "";
        }
        else
        {
            StartMethod = startMethodName;
        }
    }

    public void SaveCode(string path)
    {
        string text = JsonUtility.ToJson(this);
        File.WriteAllText(path, text);
    }

    public void LoadCode(string path)
    {
        string text = File.ReadAllText(path);
        Anim anim = JsonUtility.FromJson<Anim>(text);
        MethodsCodes = anim.GetMethodsCodesList();
        StartClass = anim.StartClass;
        StartMethod = anim.StartMethod;
        Code = anim.Code;   //zatial davame aj code
    }

    public string GeneratePythonCode()
    {
        StringBuilder Code = new StringBuilder();

        foreach (AnimClass classItem in MethodsCodes)
        {
            if (string.Empty.Equals(classItem.SuperClass))
            {
                Code.AppendLine("class " + classItem.Name + ":");
            }
            else
            {
                Code.AppendLine("class " + classItem.Name + "(" + classItem.SuperClass + "):");
            }
            Code.AppendLine("\t" + "instances = []");
            Code.AppendLine();

            AnimMethod constructor = classItem.Methods.FirstOrDefault(m => m.Name.Equals(classItem.Name));  //alebo SingleOrDefault
            if (constructor == null)
            {
                Code.AppendLine("\t" + "def __init__(self):");

                foreach (string attributeName in classItem.Attributes)
                {
                    Code.AppendLine("\t\t" + "self." + attributeName + " = None");
                }
            }
            else
            {
                Code.Append("\t" + "def __init__(self");

                foreach (string parameterName in constructor.Parameters)
                {
                    Code.Append(", " + parameterName);
                }
                Code.AppendLine("):");

                foreach (string attributeName in classItem.Attributes)
                {
                    Code.AppendLine("\t\t" + "self." + attributeName + " = None");
                }

                if (!string.Empty.Equals(constructor.Code))
                {
                    string result = OALParserBridge.PythonParse(constructor.Code, classItem.Attributes);
                    Code.AppendLine(result);
                }

                classItem.Methods.Remove(constructor);
            }
            Code.AppendLine("\t\t" + classItem.Name + ".instances.append(self)");
            Code.AppendLine();

            foreach (AnimMethod methodItem in classItem.Methods)
            {
                Code.Append("\t" + "def " + methodItem.Name);

                if (methodItem.Parameters.Any())
                {
                    Code.AppendLine("(self, " + string.Join(", ", methodItem.Parameters) + "):");
                }
                else
                {
                    Code.AppendLine("(self):");
                }

                if (string.Empty.Equals(methodItem.Code))
                {
                    Code.AppendLine("\t\t" + "pass");
                    Code.AppendLine();
                }
                else
                {
                    string result = OALParserBridge.PythonParse(methodItem.Code, classItem.Attributes);
                    Code.AppendLine(result);
                }
            }
        }

        Code.AppendLine("def boolean(value):");
        Code.AppendLine("\t" + "if value == \"True\":");
        Code.AppendLine("\t\t" + "return True");
        Code.AppendLine("\t" + "elif value == \"False\":");
        Code.AppendLine("\t\t" + "return False");
        Code.AppendLine("\t" + "raise ValueError(\"could not convert string to boolean: '\" + value + \"'\")");
        Code.AppendLine();

        Code.AppendLine("def cardinality(variable):");
        Code.AppendLine("\t" + "if isinstance(variable, list):");
        Code.AppendLine("\t\t" + "return len(variable)");
        Code.AppendLine("\t" + "elif hasattr(variable, '__dict__'):");
        Code.AppendLine("\t\t" + "return 1");
        Code.AppendLine("\t" + "else:");
        Code.AppendLine("\t\t" + "return 0");
        Code.AppendLine();

        if (!string.Empty.Equals(StartClass) && !string.Empty.Equals(StartMethod))
        {
            Code.AppendLine("# MAIN");
            Code.AppendLine(StartClass.ToLower() + " = " + StartClass + "()");
            Code.AppendLine(StartClass.ToLower() + "." + StartMethod + "()");
        }

        return Code.ToString();
    }
}
