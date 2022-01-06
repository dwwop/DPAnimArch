using System;

namespace OALProgramControl
{
    public class EXEScopeMethod : EXEScope
    {
        public EXEScopeMethod() : base()
        {
        }
        public override Boolean Execute(OALProgram OALProgram)
        {
            AddCommandsToStack(OALProgram, this.Commands);
            return true;
        }
    }
}