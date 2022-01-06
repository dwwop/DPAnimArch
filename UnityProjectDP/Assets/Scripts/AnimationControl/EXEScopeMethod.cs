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
            /*
            this.SetSuperScope(Scope);
            this.OALProgram = OALProgram;

            Boolean Success = true;

            foreach (EXECommand Command in this.Commands)
            {
                Success = Command.SynchronizedExecute(OALProgram, this); 
                if (!Success)
                {
                    break;
                }
            }
            this.SetSuperScope(null);

            return Success;
            */
            return true;
        }
    }
}