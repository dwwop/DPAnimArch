using System;

namespace OALProgramControl
{
    public abstract class EXECommand
    {
        public bool IsActive { get; set; } = false;
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
        public EXEScope GetTopLevelScope()
        {
            EXEScope CurrentScope = this.SuperScope;

            if (CurrentScope == null)
            {
                if (this is EXEScope)
                {
                    return this as EXEScope;
                }
                else
                {
                    throw new Exception("Simple command with no superscope");
                }
            }

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
        public virtual string ToFormattedCode(String Indent = "")
        {
            return HighlightCodeIf(IsActive, ToCode(Indent));
        }
        protected string HighlightCodeIf(bool condition, string code)
        {
            return condition ? HighlightCode(code) : code;
        }
        private string HighlightCode(string code)
        {
            return "<b><color=green>" + code + "</color></b>";
        }

        public void ToggleActiveRecursiveBottomUp(bool active)
        {
            this.IsActive = active;

            if (this.SuperScope != null)
            {
                this.SuperScope.ToggleActiveRecursiveBottomUp(active);
            }
        }
    }
}
