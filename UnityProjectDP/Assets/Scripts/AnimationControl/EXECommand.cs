using System;

namespace OALProgramControl
{
    public abstract class EXECommand
    {
        protected EXEScope SuperScope { get; set; }

        public Boolean PerformExecution(OALProgram OALProgram)
        {
            Boolean Result = Execute(OALProgram);

            return Result;
        }
        protected abstract Boolean Execute(OALProgram OALProgram);
        public EXEScope GetSuperScope()
        {
            return this.SuperScope;
        }
        public virtual void SetSuperScope(EXEScope SuperScope)
        {
            this.SuperScope = SuperScope;
        }
        protected EXEScope GetTopLevelScope()
        {
            EXEScope CurrentScope = this.SuperScope;

            while (CurrentScope.SuperScope != null)
            {
                CurrentScope = CurrentScope.SuperScope;
            }

            return CurrentScope;
        }
        public virtual Boolean IsComposite()
        {
            return false;
        }
        public abstract EXECommand CreateClone();
        public virtual String ToCode(String Indent = "")
        {
            return Indent + ToCodeSimple() + ";\n";
        }
        public virtual String ToCodeSimple()
        {
            return "Command";
        }
    }
}
