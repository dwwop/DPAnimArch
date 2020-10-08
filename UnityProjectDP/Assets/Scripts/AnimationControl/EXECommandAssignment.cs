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

        public override Boolean Execute(OALProgram OALProgram, EXEScope Scope)
        {
            //Console.WriteLine("Assignment is being executed");
            Boolean Result = false;

            String AssignedValue = this.AssignedExpression.Evaluate(Scope, OALProgram.ExecutionSpace);

            if (AssignedValue == null)
            {
                return Result;
            }

            // If we are assigning to a variable
            if (this.AttributeName == null)
            {
                //Console.WriteLine("Assigning value: " + AssignedValue + " to variable " + this.VariableName);

                EXEPrimitiveVariable Variable = Scope.FindPrimitiveVariableByName(this.VariableName);
                // If the variable doesnt exist, we simply create it
                if (Variable == null)
                {
                    //Console.WriteLine("Creating new var " + this.VariableName);
                    Result = Scope.AddVariable(new EXEPrimitiveVariable(this.VariableName, AssignedValue));
                }
                //If variable exists and its type is UNDEFINED
                else if (EXETypes.UnitializedName.Equals(Variable.Type))
                {
                    //Console.WriteLine("Assigning to unitialized var" + this.VariableName);
                    Result = Variable.AssignValue(Variable.Name, AssignedValue);
                }
                // If the variable exists and is primitive
                else if (!EXETypes.ReferenceTypeName.Equals(Variable.Type))
                {
                    //Console.WriteLine("Assigning to initialized var" + this.VariableName);
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
                Result = RefEvaluator.SetAttributeValue(this.VariableName, this.AttributeName, Scope, OALProgram.ExecutionSpace, AssignedValue);
                //Console.WriteLine("Tried to assign " + AssignedValue + " to " + this.VariableName + "." + this.AttributeName);
            }

            //Console.WriteLine("Assignment Result: " + Result);
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
