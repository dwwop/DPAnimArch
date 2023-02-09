using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace AnimArch.Visualization.Diagrams
{
    [Serializable]
    public class Class
    {
        public string Name;
        [FormerlySerializedAs("XmiId")] public string Id;
        public string Visibility;
        public string NameSpc;
        public string Geometry;
        public float Left;
        public float Right;
        public float Top;
        public float Bottom;
        public string Type;
        public List<Attribute> Attributes;
        public List<Method> Methods;

        public Class()
        {
        }

        public Class(string id)
        {
            Name = "NewClass_" + id;
            Id = id;
            Type = "uml:Class";
            Attributes = new List<Attribute>();
            Methods = new List<Method>();
        }
        public Class(string name, string id)
        {
            Name = name;
            Id = id;
            Type = "uml:Class";
            Attributes = new List<Attribute>();
            Methods = new List<Method>();
        }
    }
}
