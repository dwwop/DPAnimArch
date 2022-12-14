using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimArch.Visualization.Diagrams
{
    [Serializable]
    public class Class
    {
        public string Name;
        public string XmiId;
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
    }
}