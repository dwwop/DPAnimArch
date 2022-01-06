using System;

namespace OALProgramControl
{
    public abstract class EXECommand
    {
        protected EXEScope SuperScope { get; set; }

        public abstract Boolean Execute(OALProgram OALProgram);
        public EXEScope GetSuperScope()
        {
            return this.SuperScope;
        }
        public virtual void SetSuperScope(EXEScope SuperScope)
        {
            this.SuperScope = SuperScope;
        }
        public virtual Boolean IsComposite()
        {
            return false;
        }
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
