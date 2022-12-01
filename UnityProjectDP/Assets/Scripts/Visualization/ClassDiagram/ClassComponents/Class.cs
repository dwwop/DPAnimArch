using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimArch.Visualization.Diagrams
{
    public class Class
    {
        public string Name { get; set; }
        public string XmiId { get; set; }
        public string Visibility { get; set; }
        public string NameSpc { get; set; }
        public string Geometry { get; set; }
        public float Left { get; set; }
        public float Right { get; set; }
        public float Top { get; set; }
        public float Bottom { get; set; }
        public string Type { get; set; }
        internal List<Attribute> Attributes { get; set; }
        internal List<Method> Methods { get; set; }
    }
}