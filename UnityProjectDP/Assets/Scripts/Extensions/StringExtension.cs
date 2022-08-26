using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimArch.Extensions
{
    public static class StringExtension
    {
        public static String Last(this String Value)
        {
            return String.IsNullOrEmpty(Value) ? null : Value[Value.Length - 1].ToString();
        }
    }
}
