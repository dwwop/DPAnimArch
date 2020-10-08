using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    class OALCodeLine
    {
        public int Order { get; }
        public String Code { get; }

        public OALCodeLine(int Order, String Code)
        {
            this.Order = Order;
            this.Code = Code;
        }
    }
}
