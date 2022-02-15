using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    public class MethodCallRecord
    {
        public readonly string ClassName;
        public readonly string MethodName;

        public MethodCallRecord(string ClassName, string MethodName)
        {
            this.ClassName = ClassName;
            this.MethodName = MethodName;
        }

        public bool Matches(MethodCallRecord OtherMethodCallRecord)
        {
            return string.Equals(this.ClassName,    OtherMethodCallRecord.ClassName)
                && string.Equals(this.MethodName,   OtherMethodCallRecord.MethodName);
        }
    }
}
