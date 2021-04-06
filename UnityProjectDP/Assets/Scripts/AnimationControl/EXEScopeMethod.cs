using System;
using System.Collections.Generic;

namespace OALProgramControl     //Filip
{
    public class EXEScopeMethod : EXEScope
    {
        private List<EXEPrimitiveVariable> PrimitiveVariables;
        private List<EXEReferencingVariable> ReferencingVariables;
        private List<EXEReferencingSetVariable> SetReferencingVariables;
        private EXEScope SuperScope { get; set; }
        public List<EXECommand> Commands;

        public OALProgram OALProgram;

        public EXEScopeMethod(List<EXECommand> Commands)
        {
            this.PrimitiveVariables = new List<EXEPrimitiveVariable>();
            this.ReferencingVariables = new List<EXEReferencingVariable>();
            this.SetReferencingVariables = new List<EXEReferencingSetVariable>();
            this.SuperScope = null;
            this.Commands = Commands;

            this.OALProgram = null;
        }

        public override Boolean SynchronizedExecute(OALProgram OALProgram, EXEScope Scope)//asi netreba
        {
            Boolean Success = this.Execute(OALProgram, Scope);
            return Success;
        }

        public override Boolean Execute(OALProgram OALProgram, EXEScope Scope)
        {
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

            return Success;
        }
    }
}

