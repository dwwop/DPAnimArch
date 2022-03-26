using System;
using System.Collections.Generic;
using System.Linq;

namespace OALProgramControl
{
    public class EXEEvaluatorHandleOperators
    {
        private static readonly List<String> ValidOperators = new List<String>(new String[] { "empty", "not_empty", "cardinality"});

        public bool IsHandleOperator(String Operator)
        {
            return ValidOperators.Contains(Operator);
        }

        public String Evaluate(String Operator, String OperandValue)
        {
            Console.WriteLine("EXEEvaluatorHandleOperators.Evaluate");
            String Result = null;

            if (Operator == null || OperandValue == null)
            {
                return Result;
            }

            Console.WriteLine("EXEEvaluatorHandleOperators.Evaluate - OPERATOR and OPERANDS not null");

            if (!IsHandleOperator(Operator))
            {
                return Result;
            }

            long[] Values = String.Empty.Equals(OperandValue) ? new long[] {} : OperandValue.Split(',').Select(id => long.Parse(id)).ToArray(); 

            switch (Operator)
            {
                case "empty":
                    Console.WriteLine("Time to evaluate 'empty' operator");
                    Result = EvaluateEmpty(Values);
                    break;
                case "not_empty":
                    Result = EvaluateNotEmpty(Values);
                    break;
                case "cardinality":
                    Result = EvaluateCardinality(Values);
                    break;
            }
            
            return Result; 
        }

        public String EvaluateEmpty(long[] OperandValues)
        {
            if (OperandValues.Any())
            {
                if (OperandValues.Count() == 1)
                {
                    if (OperandValues[0] < 0)
                    {
                        return EXETypes.BooleanTrue;
                    }
                }

                return EXETypes.BooleanFalse;
            }

            return EXETypes.BooleanTrue; 
        }

        public String EvaluateNotEmpty(long[] OperandValues)
        {
            String Result = null;
            String TempResult = EvaluateEmpty(OperandValues);

            if (EXETypes.BooleanTrue.Equals(TempResult))
            {
                Result = EXETypes.BooleanFalse;
            }
            else if (EXETypes.BooleanFalse.Equals(TempResult))
            {
                Result = EXETypes.BooleanTrue;
            }

            return Result; 
        }

        public String EvaluateCardinality(long[] OperandValues)
        {
            if (OperandValues.Any())
            {
                if (OperandValues.Count() == 1)
                {
                    if (OperandValues[0] < 0)
                    {
                        return "0";
                    }
                }

                return OperandValues.Count().ToString();
            }

            return "0";
        }
    }
}
