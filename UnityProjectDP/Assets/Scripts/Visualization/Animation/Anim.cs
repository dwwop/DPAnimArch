//Data structure for single animation

using System.Collections.Generic;   //Filip

public struct Anim 
{
    public string Code { set; get; }
    public string AnimationName { set; get; }

    private Dictionary<string, Dictionary<string, string>> MethodsCodes;//Filip
    public Anim (string animation_name, string code)
    {
        Code = code;
        AnimationName = animation_name;
        MethodsCodes = new Dictionary<string, Dictionary<string, string>>();//Filip
    }
    public Anim(string animation_name)
    {
        AnimationName = animation_name;
        Code = "";
        MethodsCodes = new Dictionary<string, Dictionary<string, string>>();//Filip
    }

    public void SetMethodCode(string className, string methodName, string code) //Filip
    {
        Dictionary<string, string> classMethods;
        if (!MethodsCodes.TryGetValue(className, out classMethods))  //className is not in MethodsCodes
        {
            Dictionary<string, string> method = new Dictionary<string, string>();
            method.Add(methodName, code);
            MethodsCodes.Add(className, method);
        }
        else
        {
            if (classMethods.ContainsKey(methodName))  //methodName is in classMethods
            {
                classMethods[methodName] = code;
            }
            else
            {
                classMethods.Add(methodName, code);
            }
        }
    }

    public string GetMethodBody(string className, string methodName) //Filip
    {
        Dictionary<string, string> classMethods;
        if (!MethodsCodes.TryGetValue(className, out classMethods))  //className is not in MethodsCodes
        {
            return "";
        }
        else 
        {
            string methodBody;
            if (!classMethods.TryGetValue(methodName, out methodBody))  //methodName is not in classMethods
            {
                return "";
            }
            else
            {
                return methodBody;
            }
        }
    }

    public Dictionary<string, Dictionary<string, string>> GetMethodsCodes() //Filip
    {
        return MethodsCodes;
    }
}
