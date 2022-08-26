using System;

namespace OALProgramControl
{
    public class EXECommandQueryDelete : EXECommand
    {
        private String VariableName { get; }
        private String AttributeName { get; }

        public EXECommandQueryDelete(String VariableName, String AttributeName)
        {
            this.VariableName = VariableName;
            this.AttributeName = AttributeName;
        }

        protected override bool Execute(OALProgram OALProgram)
        {
            bool Result = false;
            EXEReferencingVariable Variable = SuperScope.FindReferencingVariableByName(this.VariableName);
            if (Variable != null)
            {
                String VariableClassName = Variable.ClassName;
                long VariableInstanceId = Variable.ReferencedInstanceId;

                if (this.AttributeName != null)
                {
                    CDClass VariableClass = OALProgram.ExecutionSpace.getClassByName(Variable.ClassName);
                    if (VariableClass == null)
                    {
                        return false;
                    }

                    CDAttribute Attribute = VariableClass.GetAttributeByName(this.AttributeName);
                    if (Attribute == null)
                    {
                        return false;
                    }

                    if ("[]".Equals(Attribute.Type.Substring(Attribute.Type.Length - 2, 2)))
                    {
                        return false;
                    }

                    CDClassInstance ClassInstance = VariableClass.GetInstanceByID(Variable.ReferencedInstanceId);
                    if (ClassInstance == null)
                    {
                        return false;
                    }

                    if (!long.TryParse(ClassInstance.GetAttributeValue(this.AttributeName), out VariableInstanceId))
                    {
                        return false;
                    }

                    // We need to check if Attribute.Type is type of some class, not primitive type
                    CDClass AttributeClass = OALProgram.ExecutionSpace.getClassByName(Attribute.Type);
                    if (AttributeClass == null)
                    {
                        return false;
                    }

                    VariableClassName = Attribute.Type;
                }

                bool DestructionSuccess = OALProgram.ExecutionSpace.DestroyInstance(VariableClassName, VariableInstanceId);
                if(DestructionSuccess)
                {
                    //TODO: ako odstranit aj z atributov ?
                    Result = SuperScope.UnsetReferencingVariables(VariableClassName, VariableInstanceId);
                }
            }

            return Result;
        }
        public override string ToCodeSimple()
        {
            return "delete object instance " + (this.AttributeName == null ? this.VariableName : (this.VariableName + "." + this.AttributeName));
        }

        public override EXECommand CreateClone()
        {
            return new EXECommandQueryDelete(VariableName, AttributeName);
        }
    }
}
