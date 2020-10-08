using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    public class StringBuffer
    {
        private List<String> Strings { get;  }
        private readonly object Syncer;
        public StringBuffer()
        {
            this.Strings = new List<String>();
            this.Syncer = new object();
        }
        public void Append(String String)
        {
            lock (this.Syncer)
            {
                this.Strings.Add(String);
            }
        }
        public String GenerateString()
        {
            String Result = "";
            lock (this.Syncer)
            {
                foreach (String String in this.Strings)
                {
                    Result += String;
                }
            }
            return Result;
        }
        public List<String> CloneStringList()
        {
            List<String> Result = new List<String>();
            lock (this.Syncer)
            {
                foreach (String String in this.Strings)
                {
                    Result.Add(String);
                }
            }
            return Result;
        }
    }
}
