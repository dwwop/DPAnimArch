using System;
using System.Collections;
using System.Collections.Generic;

namespace OALProgramControl
{
    public class EXECommandRead : EXECommand
    {
        /*private String VariableName { get; }
        private String AttributeName { get; }
        private String ReadType { get; }
        private EXEASTNode Prompt { get; }  // Must be String type

        public EXECommandRead(String VariableName, String AttributeName, String ReadType, EXEASTNode Prompt)
        {
            this.VariableName = VariableName;
            this.AttributeName = AttributeName;
            this.ReadType = ReadType;
            this.Prompt = Prompt;
        }*/

        protected override Boolean Execute(OALProgram OALProgram)
        {/*
            String Result = "";

            if (this.Prompt != null)
            {
                Result = this.Prompt.Evaluate(SuperScope, OALProgram.ExecutionSpace);

                String ResultType = EXETypes.DetermineVariableType("", Result);
                //co psravit ak mame unitialized?
                // We need String or this fails
                if (!EXETypes.StringTypeName.Equals(ResultType))
                {
                    return false;
                }

                // Remove double quotes
                Result = Result.Substring(1, Result.Length - 2);
            }

            ConsolePanel.Instance.YieldOutput(Result);*/

            return true;
        }

        /*public Boolean AssignReadValue(String Value, OALProgram OALProgram)
        {
            Boolean Result = false;
            String ExpectedType;

            if (this.ReadType.Contains("int"))
            {
                ExpectedType = EXETypes.IntegerTypeName;
            }
            else if (this.ReadType.Contains("real"))
            {
                ExpectedType = EXETypes.RealTypeName;
            }
            else if (this.ReadType.Contains("bool"))
            {
                ExpectedType = EXETypes.BooleanTypeName;
            }
            // It must be String
            else
            {
                ExpectedType = EXETypes.StringTypeName;
            }

            String ValueType = EXETypes.DetermineVariableType("", Value);

            //netreba tu riesit aj unitializedtype ?
            if (!ExpectedType.Equals(ValueType))
            {
                return Result;
            }

            if (this.AttributeName == null)
            {
                EXEPrimitiveVariable PrimitiveVariable = this.SuperScope.FindPrimitiveVariableByName(this.VariableName);

                if (PrimitiveVariable != null)
                {
                    // We find the type of PrimitiveVariable
                    String PrimitiveVariableType = PrimitiveVariable.Type;
                    if (EXETypes.ReferenceTypeName.Equals(PrimitiveVariable.Type))
                    {
                        //PrimitiveVariableType = FindPrimitiveType(PrimitiveVariable.Value); //V tomto pripade by malo byt AssignedValue asi meno inej premennej
                    }

                    // If PrimitiveVariable exists and its type is UNDEFINED
                    if (EXETypes.UnitializedName.Equals(PrimitiveVariableType)) //moze sa stat ze aj AssignedType by bol unitialized?
                    {
                        return PrimitiveVariable.AssignValue(PrimitiveVariable.Name, Value);
                        //TODO: ak sa to podari asi treba aj pozmenit typ ci ? mozno reisit v AssignValue() metode alebo aj kontrolovat validValue
                    }

                    // We need to compare primitive types
                    if (!PrimitiveVariableType.Equals(ValueType))
                    {
                        return Result;
                    }

                    // If the types don't match, this fails and returns false
                    Value = EXETypes.AdjustAssignedValue(PrimitiveVariableType, Value);
                    Result = PrimitiveVariable.AssignValue("", Value);
                }
                // We must create new Variable, it depends on the type of ValueType/ExpectedType
                else
                {
                    //toto ani nemoze nastat pretoze nepridavame typ unitialized
                    // Its type is UNDEFINED
                    //if (EXETypes.UnitializedName.Equals(ValueType))
                    //{
                    //   //neviem ci to je dobre 
                    //    return SuperScope.AddVariable(new EXEPrimitiveVariable(this.VariableName, Value));
                    //}

                    // If the types don't match, this fails and returns false
                    Value = EXETypes.AdjustAssignedValue(ExpectedType, Value);
                    Result = SuperScope.AddVariable(new EXEPrimitiveVariable(this.VariableName, Value));
                }        
            }
            else
            {
                EXEReferenceEvaluator RefEvaluator = new EXEReferenceEvaluator();
                Result = RefEvaluator.SetAttributeValue(this.VariableName, this.AttributeName, SuperScope, OALProgram.ExecutionSpace, Value, ValueType);
            }

            return Result;
        }

        public override String ToCodeSimple()
        {
            return (this.AttributeName == null ? this.VariableName : (this.VariableName + "." + this.AttributeName))
                + " = " + this.ReadType + (this.Prompt != null ? this.Prompt.ToCode() : "") + (this.ReadType.Equals("read(") ? ")" : "))");
        }*/
    }
}
