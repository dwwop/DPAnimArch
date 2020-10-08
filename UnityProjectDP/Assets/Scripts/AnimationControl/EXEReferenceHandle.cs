using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    public abstract class EXEReferenceHandle
    {
        public String Name { get; }
        public String ClassName { get; }
        public EXEReferenceHandle(String Name, String ClassName)
        {
            this.Name = Name;
            this.ClassName = ClassName;
        }
        public abstract List<long> GetReferencedIds();
    }
}
