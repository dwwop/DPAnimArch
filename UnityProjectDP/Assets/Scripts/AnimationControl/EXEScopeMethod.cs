using System;
using System.Collections.Generic;

namespace OALProgramControl
{
    public class EXEScopeMethod : EXEScope
    {
        public MethodCallRecord MethodDefinition;

        public EXEScopeMethod() : base()
        {
        }
        protected override Boolean Execute(OALProgram OALProgram)
        {
            EXECommandFinishMethodScope finishCommand = new EXECommandFinishMethodScope(this.MethodDefinition);
            finishCommand.SetSuperScope(this);

            OALProgram.CommandStack.Enqueue(finishCommand);
            AddCommandsToStack(OALProgram, this.Commands);
            return true;
        }

        protected override void AddCommandsToStack(OALProgram OALProgram, List<EXECommand> Commands)
        {
            base.AddCommandsToStack(OALProgram, Commands);
            OALProgram.CommandStack.CallStack.Add(this.MethodDefinition);
        }

        public override string ToFormattedCode(string Indent = "")
        {
            String Result = "";
            foreach (EXECommand Command in this.Commands)
            {
                Result += Command.ToFormattedCode(Indent);
            }
            return Result;
        }

        protected override EXEScope CreateDuplicateScope()
        {
            return new EXEScopeMethod() { MethodDefinition = MethodDefinition };
        }
    }
}