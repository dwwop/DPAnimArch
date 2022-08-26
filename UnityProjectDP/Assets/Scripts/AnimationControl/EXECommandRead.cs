using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace OALProgramControl
{
    public class EXECommandRead : EXECommand
    {
        private String VariableName { get; }
        private String AttributeName { get; }
        private String ReadType { get; }
        private EXEASTNode Prompt { get; }  // Must be String type

        public EXECommandRead(String VariableName, String AttributeName, String ReadType, EXEASTNode Prompt)
        {
            this.VariableName = VariableName;
            this.AttributeName = AttributeName;
            this.ReadType = ReadType;
            this.Prompt = Prompt;
        }

        protected override Boolean Execute(OALProgram OALProgram)
        {
            String Result = "";

            if (this.Prompt != null)
            {
                Result = this.Prompt.Evaluate(SuperScope, OALProgram.ExecutionSpace);
                if (Result == null)
                {
                    return false;
                }

                String ResultType = EXETypes.DetermineVariableType("", Result);

                // We need String otherwise this fails
                if (EXETypes.StringTypeName.Equals(ResultType))
                {
                    // Remove double quotes
                    Result = Result.Substring(1, Result.Length - 2);
                }
                else if (!EXETypes.UnitializedName.Equals(ResultType))
                {
                    return false;
                }
            }

            // ConsolePanel.Instance.YieldOutput(Result);

            return true;
        }

        public Boolean AssignReadValue(String Value, OALProgram OALProgram)
        {
            Boolean Result = false;
            String ValueType;

            if (this.ReadType.Contains("int"))
            {
                ValueType = EXETypes.IntegerTypeName;

                if (!int.TryParse(Value, out _))
                {
                    return false;
                }
            }
            else if (this.ReadType.Contains("real"))
            {
                ValueType = EXETypes.RealTypeName;

                try
                {
                    decimal.Parse(Value, CultureInfo.InvariantCulture);
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            else if (this.ReadType.Contains("bool"))
            {
                ValueType = EXETypes.BooleanTypeName;

                if (!EXETypes.BooleanTrue.Equals(Value) && !EXETypes.BooleanFalse.Equals(Value))
                {
                    return false;
                }
            }
            // It must be String
            else
            {
                ValueType = EXETypes.StringTypeName;
                Value = '\"' + Value + '\"';
            }

            if (this.AttributeName == null)
            {
                EXEPrimitiveVariable PrimitiveVariable = this.SuperScope.FindPrimitiveVariableByName(this.VariableName);

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
                    if (!Object.Equals(PrimitiveVariable.Type, ValueType))
                    {
                        return false;
                    }

                    // If the types don't match, this fails and returns false
                    Value = EXETypes.AdjustAssignedValue(PrimitiveVariable.Type, Value);
                    Result = PrimitiveVariable.AssignValue("", Value);
                }
                // We must create new Variable, it depends on the type of ValueType
                else
                {
                    // If the types don't match, this fails and returns false
                    Value = EXETypes.AdjustAssignedValue(ValueType, Value);
                    Result = SuperScope.AddVariable(new EXEPrimitiveVariable(this.VariableName, Value, ValueType));
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
        }

        public override EXECommand CreateClone()
        {
            return new EXECommandRead(VariableName, AttributeName, ReadType, Prompt);
        }
    }
}
