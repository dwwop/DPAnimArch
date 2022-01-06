using OALProgramControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    class EXEProgramPointer
    {
        private readonly Stack<Integer> CommandIndexQueue;
        public EXECommand CurrentCommand
        {
            get
            {
                return CurrentScope._Commands[CommandIndexQueue.Peek().Value];
            }
        }
        public EXEScope CurrentScope { get; private set; }


        public EXEProgramPointer(EXEScope currentScope)
        {
            CommandIndexQueue = new Stack<Integer>();
            CommandIndexQueue.Push(new Integer(-1));

            CurrentScope = currentScope;
        }

        public EXECommand NextCommand()
        {
            EXECommand Result = null;

            CommandIndexQueue.Peek().Increment();

            while (CurrentScope != null && CommandIndexQueue.Peek().Value >= CurrentScope.Commands.Count)
            {
                CurrentScope = CurrentScope.GetSuperScope();
                CommandIndexQueue.Pop();
            }

            if (CurrentScope != null)
            {
                Result = CurrentScope._Commands[CommandIndexQueue.Peek().Value];  
            }

            if (Result != null)
            {
                Result.SetSuperScope(CurrentScope);
            }

            return Result;
        }
    }
}
