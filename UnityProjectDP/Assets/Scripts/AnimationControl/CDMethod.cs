using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    public class CDMethod
    {
        public string Name { get; set; }
        public string ReturnType { get; set; }
        public List<CDParameter> Parameters { get; set; }
        public string OALCode { get; set; }
        public int CallCountInOALProgram { get; set; }
        public CDMethod(String Name, String Type)
        {
            this.Name = Name;
            this.ReturnType = ReturnType;
            this.CallCountInOALProgram = 0;
            this.OALCode = "";
            this.Parameters = new List<CDParameter>();
        }
        public void IncementCallCount()
        {
            this.CallCountInOALProgram++;
        }
    }
}
