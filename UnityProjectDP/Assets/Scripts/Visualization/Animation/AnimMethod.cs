using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimMethod
{
    [SerializeField]
    public string Name;
    [SerializeField]
    public List<string> Parameters;
    [SerializeField]
    public string Code;

    public AnimMethod(string Name, List<string> Parameters, string Code)
    {
        this.Name = Name;
        this.Parameters = Parameters;
        this.Code = Code;
    }
}
