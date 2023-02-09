using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    public class CDAttribute
    {
        public String Name { get; set; }
        public String Type { get; set; }
        public Boolean IsMockedByCompiler { get; }
        public CDAttribute(String Name, String Type)
        {
            this.Name = Name;
            this.Type = Type;
            this.IsMockedByCompiler = false;
        }
        public CDAttribute(String Name, String Type, Boolean IsMockedByCompiler)
        {
            this.Name = Name;
            this.Type = Type;
            this.IsMockedByCompiler = IsMockedByCompiler;
        }
    }
}
