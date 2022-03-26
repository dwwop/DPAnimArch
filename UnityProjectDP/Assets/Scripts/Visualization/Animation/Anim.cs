//Data structure for single animation

using OALProgramControl;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

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
            foreach (CDClass ClassItem in ClassPool)
            {
                List<string> Attributes = ClassItem.Attributes.Select(a => a.Name).ToList();
                List<AnimMethod> Methods = new List<AnimMethod>();

                foreach (CDMethod MethodItem in ClassItem.Methods)
                {
                    List<string> Parameters = MethodItem.Parameters.Select(p => p.Name).ToList();
                    Methods.Add(new AnimMethod(MethodItem.Name, Parameters, ""));
                }

                MethodsCodes.Add(new AnimClass(ClassItem.Name, "", Attributes, Methods));//TODO spravit superclass
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
}
