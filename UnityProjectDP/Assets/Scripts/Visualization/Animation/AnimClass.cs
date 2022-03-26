using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimClass  //Filip
{
    [SerializeField]
    public string Name;
    [SerializeField]
    public string SuperClass; //od ktorej dedi
    [SerializeField]
    public List<string> Attributes;
    [SerializeField]
    public List<AnimMethod> Methods;

    public AnimClass(string Name)
    {
        this.Name = Name;
        this.Methods = new List<AnimMethod>();
    }
}
