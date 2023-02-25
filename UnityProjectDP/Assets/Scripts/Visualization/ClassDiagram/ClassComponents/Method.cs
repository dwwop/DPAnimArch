using System;
using System.Collections.Generic;

namespace Visualization.ClassDiagram.ClassComponents
{
    [Serializable]
    public class Method
    {
        public string Id;
        public string Name;
        public string ReturnValue;
        public List<string> arguments;
        public Method(string name, string id, string returnValue, List<string> arguments)
        {
            Name = name;
            Id = id;
            ReturnValue = returnValue;
            this.arguments = arguments;
        }

        public Method(string name, string id, string returnValue)
        {
            Name = name;
            Id = id;
            ReturnValue = returnValue;
        }
        public Method() { }

        protected bool Equals(Method other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Method)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name);
        }
    }
}
