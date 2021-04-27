//Data structure for single animation

using System.Collections.Generic;   //Filip
using System.IO;  //Filip
using UnityEngine;  //Filip

[System.Serializable]
public struct Anim
{
    public string Code; //{ set; get; }
    public string AnimationName; //{ set; get; }
    [SerializeField]
    private List<AnimClass> MethodsCodes;//Filip
    public Anim (string animation_name, string code)
    {
        Code = code;
        AnimationName = animation_name;
        MethodsCodes = new List<AnimClass>();//Filip
    }
    public Anim(string animation_name)
    {
        AnimationName = animation_name;
        Code = "";
        MethodsCodes = new List<AnimClass>();//Filip
    }
    public void SetMethodCode(string className, string methodName, string code) //Filip
    {
        int index = methodName.IndexOf("(");
        methodName = methodName.Substring(0, index); // remove "(...)" from method name
        bool classExist = false;

        foreach (AnimClass classItem in MethodsCodes)
        {
            if (classItem.Name.Equals(className))
            {
                classExist = true;
                bool methodExist = false;

                foreach (AnimMethod methodItem in classItem.Methods)
                {
                    if (methodItem.Name.Equals(methodName))
                    {
                        methodExist = true;
                        methodItem.Code = code;
                        break;
                    }
                }
                if (!methodExist)
                {
                    AnimMethod Method = new AnimMethod(methodName, code);
                    classItem.Methods.Add(Method);  
                }
                break;
            }
        }
        if (!classExist) 
        {
            AnimMethod Method = new AnimMethod(methodName, code);
            AnimClass Class = new AnimClass(className);
            Class.Methods.Add(Method);
            MethodsCodes.Add(Class);
        }
    }
    public string GetMethodBody(string className, string methodName) //Filip
    {
        int index = methodName.IndexOf("(");
        methodName = methodName.Substring(0, index); // remove "(...)" from method name

        foreach (AnimClass classItem in MethodsCodes)
        {
            if (classItem.Name.Equals(className))
            {
                foreach (AnimMethod methodItem in classItem.Methods)
                {
                    if (methodItem.Name.Equals(methodName))
                    {
                        return methodItem.Code;
                    }
                }
                return "";  //methodName is not in classItem.Methods
            }
        }
        return "";  //className is not in MethodsCodes
    }
    public List<AnimClass> GetMethodsCodesList() //Filip
    {
        return MethodsCodes;
    }
    public void SaveCode()
    {
        string text = JsonUtility.ToJson(this);
        File.WriteAllText(@"C:\Users\garah\Downloads\test.json", text);
    }
    public void LoadCode()
    {
        string text = File.ReadAllText(@"C:\Users\garah\Downloads\test.json");
        Anim anim = JsonUtility.FromJson<Anim>(text);
        MethodsCodes = anim.GetMethodsCodesList();
        Code = anim.Code;   //zatial davame aj code
        AnimationName = anim.AnimationName; //zatial davame
    }
}
