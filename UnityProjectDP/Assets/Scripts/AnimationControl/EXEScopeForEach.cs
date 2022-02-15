using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    public class EXEScopeForEach : EXEScope
    {
        public String IteratorName { get; set; }
        public String IterableName { get; set; }
        private int IterableIndex;
        public EXEScopeForEach(String Iterator, String Iterable)  : base()
        {
            this.IteratorName = Iterator;
            this.IterableName = Iterable;
            int IterableIndex = 0;
        }
        public EXEScopeForEach(EXEScope SuperScope, EXECommand[] Commands, String Iterator, String Iterable) : base(SuperScope, Commands)
        {
            this.IteratorName = Iterator;
            this.IterableName = Iterable;
            int IterableIndex = 0;
        }
        protected override Boolean Execute(OALProgram OALProgram)
        {
            EXEReferencingVariable IteratorVariable = this.FindReferencingVariableByName(this.IteratorName);
            EXEReferencingSetVariable IterableVariable = this.FindSetReferencingVariableByName(this.IterableName);

            Boolean Success = true;

            // We cannot iterate over not existing reference set
            if (Success && IterableVariable == null)
            {
                Success = false;
            }

            // If iterator already exists and its class does not match the iterable class, we cannot do this
            if (Success && IteratorVariable != null && !IteratorVariable.ClassName.Equals(IterableVariable.ClassName))
            {
                Success = false;
            }

            // If iterator name is already taken for another variable, we quit again. Otherwise we create the iterator variable
            if (Success && IteratorVariable == null)
            {
                IteratorVariable = new EXEReferencingVariable(this.IteratorName, IterableVariable.ClassName, -1);
                Success = this.GetSuperScope().AddVariable(IteratorVariable);
            }

            Success = Success && IterableIndex < IterableVariable.GetReferencingVariables().Count;

            if (Success)
            {
                
                EXEReferencingVariable CurrentItem = IterableVariable.GetReferencingVariables()[IterableIndex];
                IteratorVariable.ReferencedInstanceId = CurrentItem.ReferencedInstanceId;

                IterableIndex++;
                OALProgram.CommandStack.Enqueue(this);
                AddCommandsToStack(OALProgram, this.Commands);
                this.ClearVariables();
                Success = this.GetSuperScope().AddVariable(IteratorVariable);
            }
            
            return Success;
        }

        public override String ToCode(String Indent = "")
        {
            String Result = Indent + "for each " + this.IteratorName + " in " + this.IterableName + "\n";
            foreach (EXECommand Command in this.Commands)
            {
                Result += Command.ToCode(Indent + "\t");
            }
            Result += Indent + "end for;\n";
            return Result;
        }
    }
}
