using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    public class CDMethod
    {
        public CDClass OwningClass { get; set; }
        public string Name { get; set; }
        public string ReturnType { get; set; }
        public List<CDParameter> Parameters { get; set; }
        public string OALCode { get; set; }
        public int CallCountInOALProgram { get; set; }

        private EXEScopeMethod _ExecutableCode;
        public EXEScopeMethod ExecutableCode
        {
            get
            {
                return _ExecutableCode == null ?  new EXEScopeMethod() : (EXEScopeMethod)_ExecutableCode.CreateClone();
            }
            set
            {
                _ExecutableCode = value;
                if (_ExecutableCode != null)
                {
                    _ExecutableCode.MethodDefinition = new MethodCallRecord(this.OwningClass.Name, this.Name);
                }
            }
        }

        public CDMethod(CDClass OwningClass, String Name, String Type)
        {
            this.OwningClass = OwningClass;
            this.Name = Name;
            this.ReturnType = Type;
            this.CallCountInOALProgram = 0;
            this.OALCode = "";
            this.Parameters = new List<CDParameter>();
            this.ExecutableCode = null;
        }
        public void IncementCallCount()
        {
            this.CallCountInOALProgram++;
        }
    }
}
