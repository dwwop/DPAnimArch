using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    public class EXEExecutionGlobals
    {
        public static readonly Boolean AllowPromotionOfIntegerToReal = true;
        public static readonly Boolean AllowLossyAssignmentOfRealToInteger = true;

        public static readonly int LoopIterationCap = 10000;
    }
}
