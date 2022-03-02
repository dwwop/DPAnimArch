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

            // We find the type of AssignedExpression
            String AssignedType;
            if (this.AssignedExpression.IsReference())
            {
                AssignedType = SuperScope.DetermineVariableType(this.AssignedExpression.AccessChain(), OALProgram.ExecutionSpace);
                if (AssignedType == null)
                {
                    return Result;
                }

                // Check if AssignedType is ReferenceTypeName, it means it is primitive
                if (EXETypes.ReferenceTypeName.Equals(AssignedType))
                {
                    AssignedType = FindPrimitiveType(AssignedValue); //V tomto pripade by malo byt AssignedValue asi meno inej premennej
                }
            }
            // It must be primitive, not reference
            else
            {
                AssignedType = EXETypes.DetermineVariableType("", AssignedValue);
                if (AssignedType == null)
                {
                    return Result;
                }
            }


            // If we are assigning to a variable
            if (this.AttributeName == null)
            {
                EXEPrimitiveVariable PrimitiveVariable = SuperScope.FindPrimitiveVariableByName(this.VariableName);
                EXEReferencingVariable ReferencingVariable = SuperScope.FindReferencingVariableByName(this.VariableName);
                EXEReferencingSetVariable SetVariable = SuperScope.FindSetReferencingVariableByName(this.VariableName);

                if (PrimitiveVariable != null)
                {
                    // We find the type of PrimitiveVariable
                    String PrimitiveVariableType = PrimitiveVariable.Type;
                    if (EXETypes.ReferenceTypeName.Equals(PrimitiveVariable.Type))
                    {
                        PrimitiveVariableType = FindPrimitiveType(PrimitiveVariable.Value); //V tomto pripade by malo byt AssignedValue asi meno inej premennej
                    }

                    // If PrimitiveVariable exists and its type is UNDEFINED
                    if (EXETypes.UnitializedName.Equals(PrimitiveVariableType)) //moze sa stat ze aj AssignedType by bol unitialized?
                    {
                        return PrimitiveVariable.AssignValue(PrimitiveVariable.Name, AssignedValue);
                        //TODO: ak sa to podari asi treba aj pozmenit typ ci ? mozno reisit v AssignValue() metode alebo aj kontrolovat validValue
                    }

                    // We need to compare primitive types
                    if (!PrimitiveVariableType.Equals(AssignedType))
                    {
                        return Result;
                    }

                    // If the types don't match, this fails and returns false
                    AssignedValue = EXETypes.AdjustAssignedValue(PrimitiveVariableType, AssignedValue);
                    Result = PrimitiveVariable.AssignValue("", AssignedValue);   
                }
                else if (ReferencingVariable != null)
                {
                    CDClass Class = OALProgram.ExecutionSpace.getClassByName(ReferencingVariable.ClassName);
                    if (Class == null)
                    {
                        return Result;
                    }

                    if
                    (
                        !(
                            this.AssignedExpression.IsReference()
                            &&
                            Object.Equals(Class.Name, AssignedType)
                        )
                    )
                    {
                        return Result;
                    }

                    if (!EXETypes.IsValidReferenceValue(AssignedValue, Class.Name))
                    {
                        return Result;
                    }

                    long IDValue = long.Parse(AssignedValue);

                    CDClassInstance ClassInstance = Class.GetInstanceByID(IDValue);
                    if (ClassInstance == null)
                    {
                        return Result;
                    }

                    ReferencingVariable.ReferencedInstanceId = IDValue;
                    Result = true;
                }
                else if (SetVariable != null)
                {
                    CDClass Class = OALProgram.ExecutionSpace.getClassByName(SetVariable.ClassName);
                    if (Class == null)
                    {
                        return Result;
                    }

                    if
                    (
                        !(
                            this.AssignedExpression.IsReference()
                            &&
                            Object.Equals(SetVariable.Type, AssignedType)
                        )
                    )
                    {
                        return Result;
                    }

                    if (!EXETypes.IsValidReferenceValue(AssignedValue, SetVariable.Type))
                    {
                        return Result;
                    }

                    long[] IDs = AssignedValue.Split(',').Select(id => long.Parse(id)).ToArray();

                    CDClassInstance ClassInstance;
                    foreach (long ID in IDs)
                    {
                        ClassInstance = Class.GetInstanceByID(ID);
                        if (ClassInstance == null)
                        {
                            return Result;
                        }
                    }

                    SetVariable.ClearVariables();

                    foreach (long ID in IDs)
                    {
                        SetVariable.AddReferencingVariable(new EXEReferencingVariable("", Class.Name, ID));
                    }
                    Result = true;
                }
                // We must create new Variable, it depends on the type of AssignedExpression
                else
                {
                    // Its type is UNDEFINED
                    if (EXETypes.UnitializedName.Equals(AssignedType))
                    {
                        Result = SuperScope.AddVariable(new EXEPrimitiveVariable(this.VariableName, AssignedValue));
                    }
                    else if (EXETypes.IsPrimitive(AssignedType))
                    {
                        // If the types don't match, this fails and returns false
                        AssignedValue = EXETypes.AdjustAssignedValue(AssignedType, AssignedValue);
                        Result = SuperScope.AddVariable(new EXEPrimitiveVariable(this.VariableName, AssignedValue));
                    }
                    else if ("[]".Equals(AssignedType.Substring(AssignedType.Length - 2, 2)))
                    {
                        CDClass Class = OALProgram.ExecutionSpace.getClassByName(AssignedType.Substring(0, AssignedType.Length - 2));
                        if (Class == null)
                        {
                            return Result;
                        }

                        if (!EXETypes.IsValidReferenceValue(AssignedValue, AssignedType))
                        {
                            return Result;
                        }

                        long[] IDs = AssignedValue.Split(',').Select(id => long.Parse(id)).ToArray();

                        CDClassInstance ClassInstance;
                        foreach (long ID in IDs)
                        {
                            ClassInstance = Class.GetInstanceByID(ID);
                            if (ClassInstance == null)
                            {
                                return Result;
                            }
                        }

                        EXEReferencingSetVariable CreatedSetVariable = new EXEReferencingSetVariable(this.VariableName, Class.Name);

                        foreach (long ID in IDs)
                        {
                            CreatedSetVariable.AddReferencingVariable(new EXEReferencingVariable("", Class.Name, ID));
                        }

                        Result = SuperScope.AddVariable(CreatedSetVariable);
                    }
                    else if (!String.IsNullOrEmpty(AssignedType))
                    {
                        CDClass Class = OALProgram.ExecutionSpace.getClassByName(AssignedType);
                        if (Class == null)
                        {
                            return Result;
                        }

                        if (!EXETypes.IsValidReferenceValue(AssignedValue, AssignedType))
                        {
                            return Result;
                        }

                        long ID = long.Parse(AssignedValue);

                        CDClassInstance ClassInstance = Class.GetInstanceByID(ID);
                        if (ClassInstance == null)
                        {
                            return Result;
                        }

                        Result = SuperScope.AddVariable(new EXEReferencingVariable(this.VariableName, Class.Name, ID));
                    }
                }


                /*
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
                */
            }
            // We are assigning to an attribute of a variable
            else
            {
                EXEReferenceEvaluator RefEvaluator = new EXEReferenceEvaluator();
                Result = RefEvaluator.SetAttributeValue(this.VariableName, this.AttributeName, SuperScope, OALProgram.ExecutionSpace, AssignedValue, AssignedType);
            }

            return Result;
        }

        private String FindPrimitiveType(String ReferenceName)
        {
            EXEPrimitiveVariable Variable = SuperScope.FindPrimitiveVariableByName(ReferenceName);
            if (!EXETypes.ReferenceTypeName.Equals(Variable.Type))
            {
                return Variable.Type;
            }

            return FindPrimitiveType(Variable.Value);
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
