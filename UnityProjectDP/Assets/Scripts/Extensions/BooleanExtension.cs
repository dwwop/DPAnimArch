using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimArch.Extensions
{
    public static class BooleanExtension
    {
        public static Boolean Implies(this Boolean Precondition, Boolean Consequence)
        {
            return !Precondition || (Precondition && Consequence);
        }
    }
}
