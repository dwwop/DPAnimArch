using System;

namespace OALProgramControl
{
    public abstract class EXECommand
    {
        public virtual Boolean SynchronizedExecute(OALProgram OALProgram, EXEScope Scope)
        {
            OALProgram.AccessInstanceDatabase();
            Console.WriteLine(this.ToCode());
            Boolean Success = this.Execute(OALProgram, Scope);
            Console.WriteLine("Done");
            OALProgram.LeaveInstanceDatabase();
            return Success;
        }
        public virtual Boolean IsComposite()
        {
            return false;
        }
        public abstract Boolean Execute(OALProgram OALProgram, EXEScope Scope);

        public virtual String ToCode(String Indent = "")
        {
            return Indent + ToCodeSimple() + ";\n";
        }
        public virtual String ToCodeSimple()
        {
            return "Command";
        }
        public virtual bool PreExecute(AnimationCommandStorage ACS, OALProgram OALProgram, EXEScope Scope)
        {
            OALProgram.AccessInstanceDatabase();
            Boolean Success = this.Execute(OALProgram, Scope);
            ACS.AddAnimationStep(new AnimationCommand(Scope, this));
            OALProgram.LeaveInstanceDatabase();

            return Success;
        }
    }
}
