using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Method
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string ReturnValue { get; set; }
    public List<string> arguments;
    public Method(string name, string id, string returnValue, List<string> arguments)
    {
        this.Name = name;
        this.Id = id;
        this.ReturnValue = returnValue;
        this.arguments = arguments;
    }

    public Method(string name, string id, string returnValue)
    {
        this.Name = name;
        this.Id = id;
        this.ReturnValue = returnValue;
    }
    public Method() { }

}

