using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    public class EXEScopeCondition : EXEScope
    {
        public EXEASTNode Condition { get; set; }
        private List<EXEScopeCondition> ElifScopes;
        public EXEScope ElseScope { get; set; }

        public EXEScopeCondition(EXEASTNode Condition) : base()
        {
            this.Condition = Condition;
            this.ElifScopes = null;
            this.ElseScope = null;
        }
        public EXEScopeCondition(EXEScope SuperScope, EXECommand[] Commands, EXEASTNode Condition) : base(SuperScope, Commands)
        {
            this.Condition = Condition;
            this.ElifScopes = null;
            this.ElseScope = null;
        }
        public EXEScopeCondition(EXEScope SuperScope, EXECommand[] Commands, EXEASTNode Condition, EXEScope ElseScope) : base(SuperScope, Commands)
        {
            this.Condition = Condition;
            this.ElifScopes = null;
            this.ElseScope = ElseScope;
        }
        public EXEScopeCondition(EXEScope SuperScope, EXECommand[] Commands, EXEASTNode Condition, EXEScopeCondition[] ElifScopes) : base(SuperScope, Commands)
        {
            this.Condition = Condition;
            this.ElifScopes = ElifScopes.ToList();
            this.ElseScope = null;
        }
        public EXEScopeCondition(EXEScope SuperScope, EXECommand[] Commands, EXEASTNode Condition, EXEScopeCondition[] ElifScopes, EXEScope ElseScope) : base(SuperScope, Commands)
        {
            this.Condition = Condition;
            this.ElifScopes = ElifScopes.ToList();
            this.ElseScope = ElseScope;
        }

        public override void SetSuperScope(EXEScope SuperScope)
        {
            base.SetSuperScope(SuperScope);

            if (this.ElifScopes != null)
            {
                foreach (EXEScope ElifScope in this.ElifScopes)
                {
                    ElifScope.SetSuperScope(this.GetSuperScope());
                }
            }

            if (this.ElseScope != null)
            {
                this.ElseScope.SetSuperScope(this.GetSuperScope());
            }
        }

        public void AddElifScope(EXEScopeCondition ElifScope)
        {
            if (this.ElifScopes == null)
            {
                this.ElifScopes = new List<EXEScopeCondition>();
            }

            this.ElifScopes.Add(ElifScope);
            ElifScope.SetSuperScope(this.GetSuperScope());
        }

        protected override Boolean Execute(OALProgram OALProgram)
        {
            Boolean Result = true;
            Boolean AScopeWasExecuted = false;

            if (this.Condition == null)
            {
                return false;
            }

            String ConditionResult = this.Condition.Evaluate(SuperScope, OALProgram.ExecutionSpace);

            if (ConditionResult == null)
            {
                return false;
            }
            if (!EXETypes.BooleanTypeName.Equals(EXETypes.DetermineVariableType("", ConditionResult)))
            {
                return false;
            }
            Boolean IfConditionResult = EXETypes.BooleanTrue.Equals(ConditionResult) ? true : false;

            if (IfConditionResult)
            {
                AddCommandsToStack(OALProgram, this.Commands);
                AScopeWasExecuted = true;
            }

            if (AScopeWasExecuted)
            {
                return Result;
            }

            if (this.ElifScopes != null)
            {
                foreach (EXEScopeCondition CurrentElif in this.ElifScopes)
                {
                    if (CurrentElif.Condition == null)
                    {
                        return false;
                    }

                    ConditionResult = CurrentElif.Condition.Evaluate(SuperScope, OALProgram.ExecutionSpace);


                    if (ConditionResult == null)
                    {
                        return false;
                    }
                    if (!EXETypes.BooleanTypeName.Equals(EXETypes.DetermineVariableType("", ConditionResult)))
                    {
                        return false;
                    }
                    IfConditionResult = EXETypes.BooleanTrue.Equals(ConditionResult) ? true : false;
                    
                    if (IfConditionResult)
                    {
                        Result = CurrentElif.PerformExecution(OALProgram);
                        AScopeWasExecuted = true;
                        break;
                    }
                }
            }

            if (AScopeWasExecuted)
            {
                return Result;
            }

            if (this.ElseScope != null)
            {
                Result = this.ElseScope.PerformExecution(OALProgram);
            }

            return Result;
        }
        public override String ToCode(String Indent = "")
        {
            String Result = Indent + "if (" + this.Condition.ToCode() + ")\n";
            foreach (EXECommand Command in this.Commands)
            {
                Result += Command.ToCode(Indent + "\t");
            }
            if (this.ElifScopes != null)
            {
                foreach (EXEScopeCondition Elif in this.ElifScopes)
                {
                    Result += Indent + "elif (" + Elif.Condition.ToCode() + ")\n";
                    foreach (EXECommand Command in Elif.Commands)
                    {
                        Result += Command.ToCode(Indent + "\t");
                    }
                }
            }
            if (this.ElseScope != null)
            {
                Result += Indent + "else\n";
                foreach (EXECommand Command in this.ElseScope.Commands)
                {
                    Result += Command.ToCode(Indent + "\t");
                }
            }
            Result += Indent + "end if;\n";
            return Result;
        }

        protected override EXEScope CreateDuplicateScope()
        {
            return new EXEScopeCondition(Condition)
            {
                ElifScopes = ElifScopes == null ? null : ElifScopes.Select(x => (EXEScopeCondition)x.CreateClone()).ToList(),
                ElseScope = ElseScope == null ? null : (EXEScope)ElseScope.CreateClone()
            };
        }
    }
}
