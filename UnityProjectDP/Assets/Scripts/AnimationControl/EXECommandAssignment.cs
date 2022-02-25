using System;
using System.Linq;

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
                /*//
                EXEReferencingVariable RefVariable = SuperScope.FindReferencingVariableByName(this.VariableName);
                EXEReferencingSetVariable SetVariable = SuperScope.FindSetReferencingVariableByName(this.VariableName);

                String AssignedValueType = EXETypes.DetermineVariableType("", AssignedValue);

                if (RefVariable != null)
                {
                    // treba nejak osetrit aby to nakoniec bolo ID a nie integer
                    if (!EXETypes.IntegerTypeName.Equals(AssignedValueType))
                    {
                        return Result;
                    }

                    CDClass VariableClass = OALProgram.ExecutionSpace.getClassByName(RefVariable.ClassName);
                    if (VariableClass == null)
                    {
                        return Result;
                    }

                    CDClassInstance ClassInstance = VariableClass.GetInstanceByID(long.Parse(AssignedValue));
                    if (ClassInstance == null)
                    {
                        return Result;
                    }

                    RefVariable.ReferencedInstanceId = long.Parse(AssignedValue);    
                }
                else if (SetVariable != null)
                {
                    CDClass VariableClass = OALProgram.ExecutionSpace.getClassByName(SetVariable.ClassName);
                    if (VariableClass == null)
                    {
                        return Result;
                    }

                    if (!EXETypes.IsValidReferenceValue(AssignedValue, SetVariable.ClassName + "[]"))
                    {
                        return Result;
                    }

                    int[] IDs = AssignedValue.Split(',').Select(id => int.Parse(id)).ToArray();

                    CDClassInstance ClassInstance;
                    foreach (int ID in IDs)
                    {
                        ClassInstance = VariableClass.GetInstanceByID(ID);
                        if (ClassInstance == null)
                        {
                            return Result;
                        }
                    }

                    //treba asi clearnut list referencing variables v SetVariable
                    foreach (int ID in IDs)
                    {
                        SetVariable.AddReferencingVariable(new EXEReferencingVariable("", VariableClass.Name, ID));
                    }
                }
                // We must create new Variable, it depends on the type of AssignedExpression
                else
                {
                    if (AssignedValueType == null)
                    {
                        return Result;
                    }

                    if (EXETypes.IsPrimitive(AssignedValueType))
                    {//tu si nie sme isty s integerom, moze to byt aj ID
                        Result = SuperScope.AddVariable(new EXEPrimitiveVariable(this.VariableName, AssignedValue));
                    }
                    else if (EXETypes.UnitializedName.Equals(AssignedValueType))
                    {
                        //neviem
                    }
                    // We have reference
                    else if (EXETypes.ReferenceTypeName.Equals(AssignedValueType))
                    {

                    }
                    else
                    {
                        return Result;
                    }
                //v poli bez mena a vytvara sa nove
                }
                //*/

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
