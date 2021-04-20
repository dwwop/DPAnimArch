using System;
using System.Collections.Generic;

namespace OALProgramControl
{
    public class EXEScopeMethod : EXEScope
    {
        public EXEScopeMethod(List<EXECommand> Commands) : base()
        {
            this.Commands = Commands;
        }

        public override Boolean SynchronizedExecute(OALProgram OALProgram, EXEScope Scope)
        {
            Boolean Success = this.Execute(OALProgram, Scope);
            return Success;
        }

        public override Boolean Execute(OALProgram OALProgram, EXEScope Scope)
        {
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
        }

        public override Boolean PreExecute(AnimationCommandStorage ACS, OALProgram OALProgram, EXEScope Scope)
        {
            this.SetSuperScope(Scope);
            this.OALProgram = OALProgram;

            Boolean Success = true;

            foreach (EXECommand Command in this.Commands)
            {
                Success = Command.PreExecute(ACS, OALProgram, this);
                if (!Success)
                {
                    break;
                }
            }
            this.SetSuperScope(null);

            return Success;
        }
    }
}

