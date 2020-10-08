using System;

namespace OALProgramControl
{
    public class EXECommandQueryDelete : EXECommand
    {
        private String VariableName { get; }

        public EXECommandQueryDelete(String VariableName)
        {
            this.VariableName = VariableName;
        }

        public override bool Execute(OALProgram OALProgram, EXEScope Scope)
        {
            bool Result = false;
            EXEReferencingVariable Variable = Scope.FindReferencingVariableByName(this.VariableName);
            if (Variable != null)
            {
                bool DestructionSuccess = OALProgram.ExecutionSpace.DestroyInstance(Variable.ClassName, Variable.ReferencedInstanceId);
                if(DestructionSuccess)
                {
                    Result = Scope.UnsetReferencingVariables(Variable.ClassName, Variable.ReferencedInstanceId);
                }
            }
            return Result;
        }
        public override string ToCodeSimple()
        {
            return "delete object instance " + this.VariableName;
        }
    }
}
