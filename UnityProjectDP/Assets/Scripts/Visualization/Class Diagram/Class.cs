using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class 
{
    public List<Attribute> attributes;
    public List<Method> methods;


    public string Name { get; set; }
    public string XmiId { get; set; }
    public string Visibility { get; set; }
    public string NameSpc { get;  set; }
    public string Geometry { get; set; }
    public float Left { get; set; }
    public float Right { get; set; }
    public float Top { get; set; }
    public float Bottom { get; set; }
    public string Type { get; set; }
    internal List<Attribute> Attributes { get; set; }
    internal List<Method> Methods { get; set; }

    public Class()
    {
    }
    public Class(string name, List<Attribute> attributes, List<Method> methods)
    {
        this.Name = name;
        this.Attributes = attributes;
        this.Methods = methods;
        Left = 0f;
        Top = 0f;
    }
    public Class(string name, List<Attribute> attributes, List<Method> methods, float left, float top)
    {
        this.Name = name;
        this.Attributes = attributes;
        this.Methods = methods;
        this.Left = left;
        this.Top = top;
    }
}
