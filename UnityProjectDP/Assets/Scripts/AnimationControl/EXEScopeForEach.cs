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
        public LoopControlStructure CurrentLoopControlCommand { get; set; }
        public EXEScopeForEach(String Iterator, String Iterable)  : base()
        {
            this.IteratorName = Iterator;
            this.IterableName = Iterable;
        }
        public EXEScopeForEach(EXEScope SuperScope, EXECommand[] Commands, String Iterator, String Iterable) : base(SuperScope, Commands)
        {
            this.IteratorName = Iterator;
            this.IterableName = Iterable;
            this.CurrentLoopControlCommand = LoopControlStructure.None;
        }
        public override Boolean SynchronizedExecute(OALProgram OALProgram, EXEScope Scope)
        {
            Boolean Success = this.Execute(OALProgram, this);
            return Success;
        }
        public override Boolean Execute(OALProgram OALProgram, EXEScope Scope)
        {
            this.OALProgram = OALProgram;

            OALProgram.AccessInstanceDatabase();
            EXEReferencingVariable IteratorVariable = this.FindReferencingVariableByName(this.IteratorName);
            EXEReferencingSetVariable IterableVariable = this.FindSetReferencingVariableByName(this.IterableName);
            OALProgram.LeaveInstanceDatabase();

            Boolean Success = true;

            // We cannot iterate over not existing reference set
            if (Success & IterableVariable == null)
            {
                Success = false;
            }

            // If iterator already exists and its class does not match the iterable class, we cannot do this
            if (Success & IteratorVariable != null && !IteratorVariable.ClassName.Equals(IterableVariable.ClassName))
            {
                Success = false;
            }

            // If iterator name is already taken for another variable, we quit again. Otherwise we create the iterator variable
            if (Success & IteratorVariable == null)
            {
                IteratorVariable = new EXEReferencingVariable(this.IteratorName, IterableVariable.ClassName, -1);
                Success = this.GetSuperScope().AddVariable(IteratorVariable);
            }

            if (Success)
            {
                foreach (EXEReferencingVariable CurrentItem in IterableVariable.GetReferencingVariables())
                {
                    //!!NON-RECURSIVE!!
                    this.ClearVariables();

                    IteratorVariable.ReferencedInstanceId = CurrentItem.ReferencedInstanceId;

                    Console.WriteLine("ForEach: " + CurrentItem.ReferencedInstanceId);

                    foreach (EXECommand Command in this.Commands)
                    {
                        if (this.CurrentLoopControlCommand != LoopControlStructure.None)
                        {
                            break;
                        }

                        Success = Command.SynchronizedExecute(OALProgram, this);
                        if (!Success)
                        {
                            break;
                        }
                    }

                    if (!Success)
                    {
                        break;
                    }

                    if (this.CurrentLoopControlCommand == LoopControlStructure.Break)
                    {
                        this.CurrentLoopControlCommand = LoopControlStructure.None;
                        break;
                    }
                    else if (this.CurrentLoopControlCommand == LoopControlStructure.Continue)
                    {
                        this.CurrentLoopControlCommand = LoopControlStructure.None;
                        continue;
                    }
                }
            }
            

            return Success;
        }
        public override Boolean PreExecute(AnimationCommandStorage ACS, OALProgram OALProgram, EXEScope Scope)
        {
            this.OALProgram = OALProgram;

            OALProgram.AccessInstanceDatabase();
            EXEReferencingVariable IteratorVariable = this.FindReferencingVariableByName(this.IteratorName);
            EXEReferencingSetVariable IterableVariable = this.FindSetReferencingVariableByName(this.IterableName);
            OALProgram.LeaveInstanceDatabase();

            Boolean Success = true;

            // We cannot iterate over not existing reference set
            if (Success & IterableVariable == null)
            {
                Success = false;
            }

            // If iterator already exists and its class does not match the iterable class, we cannot do this
            if (Success & IteratorVariable != null && !IteratorVariable.ClassName.Equals(IterableVariable.ClassName))
            {
                Success = false;
            }

            // If iterator name is already taken for another variable, we quit again. Otherwise we create the iterator variable
            if (Success & IteratorVariable == null)
            {
                IteratorVariable = new EXEReferencingVariable(this.IteratorName, IterableVariable.ClassName, -1);
                Success = this.GetSuperScope().AddVariable(IteratorVariable);
            }

            if (Success)
            {
                foreach (EXEReferencingVariable CurrentItem in IterableVariable.GetReferencingVariables())
                {
                    //!!NON-RECURSIVE!!
                    this.ClearVariables();

                    IteratorVariable.ReferencedInstanceId = CurrentItem.ReferencedInstanceId;

                    Console.WriteLine("ForEach: " + CurrentItem.ReferencedInstanceId);

                    foreach (EXECommand Command in this.Commands)
                    {
                        if (this.CurrentLoopControlCommand != LoopControlStructure.None)
                        {
                            break;
                        }

                        Success = Command.PreExecute(ACS, OALProgram, this);
                        if (!Success)
                        {
                            break;
                        }
                    }

                    if (!Success)
                    {
                        break;
                    }

                    if (this.CurrentLoopControlCommand == LoopControlStructure.Break)
                    {
                        this.CurrentLoopControlCommand = LoopControlStructure.None;
                        break;
                    }
                    else if (this.CurrentLoopControlCommand == LoopControlStructure.Continue)
                    {
                        this.CurrentLoopControlCommand = LoopControlStructure.None;
                        continue;
                    }
                }
            }


            return Success;
        }
        public override bool PropagateControlCommand(LoopControlStructure PropagatedCommand)
        {
            if (this.CurrentLoopControlCommand != LoopControlStructure.None)
            {
                return false;
            }

            this.CurrentLoopControlCommand = PropagatedCommand;

            return true;
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
