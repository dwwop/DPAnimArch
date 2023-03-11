//Data structure for single animation

using System.Collections.Generic;
using System.IO;
using System.Linq;
using OALProgramControl;
using UnityEngine;

namespace Visualization.Animation
{
    [System.Serializable]
    public struct Anim
    {
        [SerializeField]
        public string Code; //{ set; get; }
        [SerializeField]
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

            if (string.IsNullOrWhiteSpace(code))
            {
                AnimClass classItem = MethodsCodes.FirstOrDefault(c => c.Name.Equals(className));   //alebo SingleOrDefault
                if (classItem != null)
                {
                    AnimMethod methodItem = classItem.Methods.FirstOrDefault(m => m.Name.Equals(methodName));  //alebo SingleOrDefault
                    if (methodItem != null)
                    {
                        CDMethod Method = OALProgram.Instance.ExecutionSpace.getClassByName(className).getMethodByName(methodName);
                        Method.ExecutableCode = null;

                        classItem.Methods.Remove(methodItem);
                        if (classItem.Methods.Count == 0) 
                        {
                            MethodsCodes.Remove(classItem);
                        }
                    }        
                }
            }
            else
            {
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
        public List<AnimMethod> GetMethodsByClassName(string className) //Filip
        {
            foreach (AnimClass classItem in MethodsCodes)
            {
                if (classItem.Name.Equals(className))
                {
                    return classItem.Methods;
                }
            }
            return null;
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
            Code = anim.Code;   //zatial davame aj code
        }
    }
}
