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
        private int IterationCounter;

        public EXEScopeLoopWhile(EXEASTNode Condition) : base()
        {
            this.Condition = Condition;
            this.IterationCounter = 0;
        }
        public EXEScopeLoopWhile(EXEScope SuperScope, EXECommand[] Commands, EXEASTNode Condition) : base(SuperScope, Commands)
        {
            this.Condition = Condition;
            this.IterationCounter = 0;
        }
        protected override Boolean Execute(OALProgram OALProgram)
        {
            Boolean Success = true;

            String ConditionResult = this.Condition.Evaluate(SuperScope, OALProgram.ExecutionSpace);

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

            bool ConditionTrue = EXETypes.BooleanTrue.Equals(ConditionResult);
            if (ConditionTrue)
            {
                if (IterationCounter >= EXEExecutionGlobals.LoopIterationCap)
                {
                    Success = false;
                }
                else
                {
                    IterationCounter++;
                    OALProgram.CommandStack.Enqueue(this);
                    AddCommandsToStack(OALProgram, this.Commands);
                    this.ClearVariables();
                }
            }

            return Success;
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

        protected override EXEScope CreateDuplicateScope()
        {
            return new EXEScopeLoopWhile(Condition);
        }
    }
}
