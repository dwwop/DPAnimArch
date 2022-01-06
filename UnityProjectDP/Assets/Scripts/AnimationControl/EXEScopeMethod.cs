using System;
using System.Collections.Generic;

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
        public EXEScopeMethod CreateClone()
        {
            EXEScopeMethod Clone = new EXEScopeMethod
            {
                Commands = new List<EXECommand>(),
                OALCode = this.OALCode
            };

            Clone.Commands.AddRange(this.Commands);

            return Clone;
        }
    }
}