using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    public class EXEScopeLoopWhile : EXEScope
    {
        public EXEASTNode Condition;
        public LoopControlStructure CurrentLoopControlCommand { get; set; }

        public EXEScopeLoopWhile(EXEASTNode Condition) : base()
        {
            this.Condition = Condition;
            this.CurrentLoopControlCommand = LoopControlStructure.None;
        }
        public EXEScopeLoopWhile(EXEScope SuperScope, EXECommand[] Commands, EXEASTNode Condition) : base(SuperScope, Commands)
        {
            this.Condition = Condition;
            this.CurrentLoopControlCommand = LoopControlStructure.None;
        }

        public override Boolean SynchronizedExecute(OALProgram OALProgram, EXEScope Scope)
        {
            Boolean Success = this.Execute(OALProgram, Scope);
            return Success;
        }
        public override Boolean Execute(OALProgram OALProgram , EXEScope Scope)
        {
            Boolean Success = true;
            this.OALProgram = OALProgram;

            bool ConditionTrue = true;
            String ConditionResult;
            int IterationCounter = 0;
            while (ConditionTrue)
            {
                OALProgram.AccessInstanceDatabase();
                ConditionResult = this.Condition.Evaluate(Scope, OALProgram.ExecutionSpace);
                OALProgram.LeaveInstanceDatabase();

                //!!NON-RECURSIVE!!
                this.ClearVariables();

                if (ConditionResult == null)
                {
                    return false;
                }
                if (!EXETypes.BooleanTypeName.Equals(EXETypes.DetermineVariableType("", ConditionResult)))
                {
                    return false;
                }
                ConditionTrue = EXETypes.BooleanTrue.Equals(ConditionResult);
                if (!ConditionTrue)
                {
                    break;
                }

                if (IterationCounter >= EXEExecutionGlobals.LoopIterationCap)
                {
                    Success = false;
                    break;
                }

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

                IterationCounter++;

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
            return Success;
        }
        public override Boolean PreExecute(AnimationCommandStorage ACS, OALProgram OALProgram, EXEScope Scope)
        {
            Boolean Success = true;
            this.OALProgram = OALProgram;

            bool ConditionTrue = true;
            String ConditionResult;
            int IterationCounter = 0;
            while (ConditionTrue)
            {
                OALProgram.AccessInstanceDatabase();
                ConditionResult = this.Condition.Evaluate(Scope, OALProgram.ExecutionSpace);
                OALProgram.LeaveInstanceDatabase();

                //!!NON-RECURSIVE!!
                this.ClearVariables();

                if (ConditionResult == null)
                {
                    return false;
                }
                if (!EXETypes.BooleanTypeName.Equals(EXETypes.DetermineVariableType("", ConditionResult)))
                {
                    return false;
                }
                ConditionTrue = EXETypes.BooleanTrue.Equals(ConditionResult);
                if (!ConditionTrue)
                {
                    break;
                }

                if (IterationCounter >= EXEExecutionGlobals.LoopIterationCap)
                {
                    Success = false;
                    break;
                }

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

                IterationCounter++;

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
            String Result = Indent + "while (" + this.Condition.ToCode() + ")\n";
            foreach (EXECommand Command in this.Commands)
            {
                Result += Command.ToCode(Indent + "\t");
            }
            Result += Indent + "end while;\n";
            return Result;
        }
    }
}
