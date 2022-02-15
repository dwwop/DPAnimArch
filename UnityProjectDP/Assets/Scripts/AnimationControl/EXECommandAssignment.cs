using System;

namespace OALProgramControl
{
    public class EXECommandAssignment : EXECommand
    {
        private String VariableName { get; }
        private String AttributeName { get; }
        private EXEASTNode AssignedExpression { get; }

        public EXECommandAssignment(String VariableName, EXEASTNode AssignedExpression)
        {
            this.VariableName = VariableName;
            this.AttributeName = null;
            this.AssignedExpression = AssignedExpression;
        }
        public EXECommandAssignment(String VariableName, String AttributeName, EXEASTNode AssignedExpression)
        {
            this.VariableName = VariableName;
            this.AttributeName = AttributeName;
            this.AssignedExpression = AssignedExpression;
        }

        protected override Boolean Execute(OALProgram OALProgram)
        {
            UnityEngine.Debug.Log(this.ToCode());
            Boolean Result = false;

            String AssignedValue = this.AssignedExpression.Evaluate(SuperScope, OALProgram.ExecutionSpace);

            if (AssignedValue == null)
            {
                return Result;
            }

            // If we are assigning to a variable
            if (this.AttributeName == null)
            {

                EXEPrimitiveVariable Variable = SuperScope.FindPrimitiveVariableByName(this.VariableName);
                // If the variable doesnt exist, we simply create it
                if (Variable == null)
                {
                    Result = SuperScope.AddVariable(new EXEPrimitiveVariable(this.VariableName, AssignedValue));
                }
                //If variable exists and its type is UNDEFINED
                else if (EXETypes.UnitializedName.Equals(Variable.Type))
                {
                    Result = Variable.AssignValue(Variable.Name, AssignedValue);
                }
                // If the variable exists and is primitive
                else if (!EXETypes.ReferenceTypeName.Equals(Variable.Type))
                {
                    // If the types don't match, this fails and returns false
                    AssignedValue = EXETypes.AdjustAssignedValue(Variable.Type, AssignedValue);
                    Result = Variable.AssignValue("", AssignedValue);
                }

                // Variable exists and is not primitive. What to do, what to do?
                // We do nothing, we CANNOT ASSIGN TO HANDLES!!!
            }
            // We are assigning to an attribute of a variable
            else
            {
            
                EXEReferenceEvaluator RefEvaluator = new EXEReferenceEvaluator();
                Result = RefEvaluator.SetAttributeValue(this.VariableName, this.AttributeName, SuperScope, OALProgram.ExecutionSpace, AssignedValue);
            }

            return Result;
        }
        public override String ToCodeSimple()
        {
            String Result = this.VariableName;
            if (this.AttributeName != null)
            {
                Result += "." + this.AttributeName;
            }
            Result += " = " + this.AssignedExpression.ToCode();
            return Result;
        }
    }
}
