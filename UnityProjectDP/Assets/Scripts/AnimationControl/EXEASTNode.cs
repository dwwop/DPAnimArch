using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    public interface EXEASTNode
    {
        String GetNodeValue();
        String Evaluate(EXEScope Scope, CDClassPool ExecutionSpace);
        bool VerifyReferences(EXEScope Scope, CDClassPool ExecutionSpace);
        void PrintPretty(string indent, bool last);
        List<String> AccessChain();
        bool IsReference();
        string ToCode();
    }
}
