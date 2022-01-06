using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    public class EXEExecutionStack
    {
        private LinkedList<EXECommand> CommandsToBeCalled;
        private Stack<EXECommand> CommandsThatHaveBeenCalled;

        public EXEExecutionStack()
        {
            this.CommandsToBeCalled = new LinkedList<EXECommand>();
            this.CommandsThatHaveBeenCalled = new Stack<EXECommand>();
        }

        public void Enqueue(EXECommand Command)
        {
            if (Command == null)
            {
                throw new Exception("Tried to add NULL to stack of commands to be called");
            }

            CommandsToBeCalled.AddFirst(Command);
        }

        public void Enqueue(IEnumerable<EXECommand> Commands)
        {
            if (Commands == null)
            {
                throw new Exception("Tried to add NULL to stack of commands to be called");
            }

            foreach (EXECommand Command in Commands.Reverse())
            {
                Enqueue(Command);
            }
        }

        public bool HasNext()
        {
            return CommandsToBeCalled.Any();
        }

        public EXECommand Next()
        {
            EXECommand Result = CommandsToBeCalled.First.Value;
            CommandsToBeCalled.RemoveFirst();

            CommandsThatHaveBeenCalled.Push(Result);

            return Result;
        }

        public bool HasPrevious()
        {
            return CommandsThatHaveBeenCalled.Any();
        }

        public EXECommand Previous()
        {
            EXECommand Result = CommandsThatHaveBeenCalled.Pop();

            CommandsToBeCalled.AddFirst(Result);

            return Result;
        }
    }
}
