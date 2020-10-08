using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    public class EXECommandCallTestDecorator : EXECommand
    {
        private EXECommand DecoratedCommand { get; }
        private StringBuffer CallTextBuffer { get; }
        public EXECommandCallTestDecorator(EXECommand DecoratedCommand, StringBuffer CallTextBuffer)
        {
            this.DecoratedCommand = DecoratedCommand;
            this.CallTextBuffer = CallTextBuffer;
        }

        public override bool Execute(OALProgram OALProgram, EXEScope Scope)
        {
            bool Result = DecoratedCommand.Execute(OALProgram, Scope);
            if (!Result)
            {
                return Result;
            }
            CallTextBuffer.Append(DecoratedCommand.ToCode());
            return Result;
        }
        public override bool SynchronizedExecute(OALProgram OALProgram, EXEScope Scope)
        {
            return this.Execute(OALProgram, Scope);
        }
        public override String ToCodeSimple()
        {
            return this.DecoratedCommand.ToCodeSimple();
        }
    }
}
