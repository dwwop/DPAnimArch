using System;
using System.Linq;

namespace OALProgramControl
{
    public class EXECommandAssignment : EXECommand
    {
        public String VariableName { get; }
        public String AttributeName { get; }
        public EXEASTNode AssignedExpression { get; }

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

                // Check if AssignedType is ReferenceTypeName, it means it is some bullshit
                if (EXETypes.ReferenceTypeName.Equals(AssignedType))
                {
                    return Result;
                }
            }
            // It must be primitive, not reference
            else
            {
                AssignedType = EXETypes.DetermineVariableType("", AssignedValue);
                if (AssignedType == null || EXETypes.ReferenceTypeName.Equals(AssignedType))
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
                    if (EXETypes.ReferenceTypeName.Equals(PrimitiveVariable.Type))
                    {
                        return false;
                    }

                    // If PrimitiveVariable exists and its type is UNDEFINED
                    if (EXETypes.UnitializedName.Equals(PrimitiveVariable.Type))
                    {
                        return false;
                    }

                    // We need to compare primitive types
                    if (!EXETypes.UnitializedName.Equals(AssignedType) && !Object.Equals(PrimitiveVariable.Type, AssignedType))
                    {
                        return false;
                    }

                    // If the types don't match, this fails and returns false
                    AssignedValue = EXETypes.AdjustAssignedValue(PrimitiveVariable.Type, AssignedValue);
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

                    long[] IDs = String.Empty.Equals(AssignedValue) ? new long[] { } : AssignedValue.Split(',').Select(id => long.Parse(id)).ToArray();

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
                        return false;
                    }
                    else if (EXETypes.IsPrimitive(AssignedType))
                    {
                        // If the types don't match, this fails and returns false
                        AssignedValue = EXETypes.AdjustAssignedValue(AssignedType, AssignedValue);
                        Result = SuperScope.AddVariable(new EXEPrimitiveVariable(this.VariableName, AssignedValue, AssignedType));
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

                        long[] IDs = String.Empty.Equals(AssignedValue) ? new long[] { } : AssignedValue.Split(',').Select(id => long.Parse(id)).ToArray();

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
            }
            // We are assigning to an attribute of a variable
            else
            {
                EXEReferenceEvaluator RefEvaluator = new EXEReferenceEvaluator();
                Result = RefEvaluator.SetAttributeValue(this.VariableName, this.AttributeName, SuperScope, OALProgram.ExecutionSpace, AssignedValue, AssignedType);
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

        public override EXECommand CreateClone()
        {
            return new EXECommandAssignment(VariableName, AttributeName, AssignedExpression);
        }
    }
}
