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

        public String Evaluate(String Operator, List<String> Operands, EXEScope Scope)
        {
            Console.WriteLine("EXEEvaluatorHandleOperators.Evaluate");
            String Result = null;

            if (Operator == null || Operands == null)
            {
                return Result;
            }

            Console.WriteLine("EXEEvaluatorHandleOperators.Evaluate - OPERATOR and OPERANDS not null");

            if (!IsHandleOperator(Operator))
            {
                return Result;
            }

            if (Operands.Count == 1)
            {
                switch (Operator) {
                    case "empty":
                        Console.WriteLine("Time to evaluate 'empty' operator");
                        Result = EvaluateEmpty(Operands.First(), Scope);
                        break;
                    case "not_empty":
                        Result = EvaluateNotEmpty(Operands.First(), Scope);
                        break;
                    case "cardinality":
                        Result = EvaluateCardinality(Operands.First(), Scope);
                        break;
                }
            }

            return Result;
        }

        public String EvaluateEmpty(String Operand, EXEScope Scope)
        {
            String Result = null;

            if (EXETypes.UnitializedName.Equals(Operand))
            {
                Result = EXETypes.BooleanTrue;
                return Result;
            }
           
            EXEReferencingVariable SingleInstanceVariable = Scope.FindReferencingVariableByName(Operand);
            if (SingleInstanceVariable != null)
            {
                if (SingleInstanceVariable.IsInitialized())
                {
                    Result = EXETypes.BooleanFalse;
                }
                else
                {
                    Result = EXETypes.BooleanTrue;
                }

                return Result;
            }

            EXEReferencingSetVariable MultiInstanceVariable = Scope.FindSetReferencingVariableByName(Operand);
            if (MultiInstanceVariable != null)
            {
                if (MultiInstanceVariable.GetReferencingVariables().Any())
                {
                    Result = EXETypes.BooleanFalse;
                }
                else
                {
                    Result = EXETypes.BooleanTrue;
                }

                return Result;
            }

            return Result;
        }

        public String EvaluateNotEmpty(String Operand, EXEScope Scope)
        {
            String Result = null;
            String TempResult = EvaluateEmpty(Operand, Scope);

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
        public String EvaluateCardinality(String Operand, EXEScope Scope)
        {
            String Result = null;

            if (EXETypes.UnitializedName.Equals(Operand))
            {
                Result = "0";
                return Result;
            }

            EXEReferencingVariable SingleInstanceVariable = Scope.FindReferencingVariableByName(Operand);
            if (SingleInstanceVariable != null)
            {
                if (SingleInstanceVariable.IsInitialized())
                {
                    Result = "1";
                }
                else
                {
                    Result = "0";
                }

                return Result;
            }

            EXEReferencingSetVariable MultiInstanceVariable = Scope.FindSetReferencingVariableByName(Operand);
            if (MultiInstanceVariable != null)
            {
                if (MultiInstanceVariable.GetReferencingVariables().Any())
                {
                    Result = MultiInstanceVariable.GetReferencingVariables().Count.ToString();
                }
                else
                {
                    Result = "0";
                }

                return Result;
            }

            return Result;
        }
    }
}
