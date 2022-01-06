using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    class Integer
    {
        public int Value { get; private set; }

        public Integer(int value = 0)
        {
            this.Value = value;
        }

        public void Increment()
        {
            this.Value++;
        }

        public void Decrement()
        {
            this.Value--;
        }
    }
}
