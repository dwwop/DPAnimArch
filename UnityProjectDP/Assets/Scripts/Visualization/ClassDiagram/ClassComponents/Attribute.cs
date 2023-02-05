using System;

namespace AnimArch.Visualization.Diagrams
{
    [Serializable]
    public class Attribute
    {
        public string Type;
        public string Name;
        public string Id;
        public Attribute(string id, string name, string type)
        {
            Id = id;
            Type = type;
            Name = name;
        }
        public Attribute() { }
        
    }
}