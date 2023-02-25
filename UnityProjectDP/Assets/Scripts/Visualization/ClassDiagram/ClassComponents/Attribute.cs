using System;

namespace Visualization.ClassDiagram.ClassComponents
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

        protected bool Equals(Attribute other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Attribute)obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }
    }
}
