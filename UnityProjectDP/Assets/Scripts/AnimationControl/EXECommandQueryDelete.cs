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

        protected override bool Execute(OALProgram OALProgram)
        {
            bool Result = false;
            EXEReferencingVariable Variable = SuperScope.FindReferencingVariableByName(this.VariableName);
            if (Variable != null)
            {
                bool DestructionSuccess = OALProgram.ExecutionSpace.DestroyInstance(Variable.ClassName, Variable.ReferencedInstanceId);
                if(DestructionSuccess)
                {
                    Result = SuperScope.UnsetReferencingVariables(Variable.ClassName, Variable.ReferencedInstanceId);
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
