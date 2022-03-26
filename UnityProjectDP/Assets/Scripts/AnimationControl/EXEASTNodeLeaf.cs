using System;
using System.Collections.Generic;
using System.Linq;

namespace OALProgramControl
{
    public class EXEASTNodeLeaf : EXEASTNode
    {
        private String Value { get; }

        public EXEASTNodeLeaf(String Value)
        {
            this.Value = Value;
        }

        public String GetNodeValue()
        {
            return this.Value;
        }
        public String Evaluate(EXEScope Scope, CDClassPool ExecutionSpace)
        {
            String Result = null;

            String ValueType = EXETypes.DetermineVariableType("", this.Value);
            if (ValueType == null)
            {
                return Result;
            }

            // If we have simple value, e.g. 5, 3.14, "hi Madelyn", we are good
            if (!EXETypes.ReferenceTypeName.Equals(ValueType))
            {
                if (EXETypes.StringTypeName.Equals(ValueType))
                {
                    Result = EXETypes.EvaluateEscapeSequences(this.Value.Substring(1, this.Value.Length - 2));
                    Result = '\"' + Result + '\"';
                }
                else
                {
                    Result = this.Value;
                }
            }
            // We got here because we have a variable name, the variable is of primitive value, or object reference, or set reference
            else
            {
                EXEPrimitiveVariable ThisVariable = Scope.FindPrimitiveVariableByName(this.Value);
                if(ThisVariable != null)
                {
                    return ThisVariable.Value;
                }

                EXEReferencingVariable ThisRefVariable = Scope.FindReferencingVariableByName(this.Value);
                if (ThisRefVariable != null)
                {
                    return ThisRefVariable.ReferencedInstanceId.ToString();
                }

                EXEReferencingSetVariable ThisRefSetVariable = Scope.FindSetReferencingVariableByName(this.Value);
                if (ThisRefSetVariable != null)
                {
                    return String.Join(",", ThisRefSetVariable.GetReferencingVariables().Select(variable => variable.ReferencedInstanceId.ToString()).ToList());
                }
            }

            /*Console.WriteLine("Operand: " + this.Value);
            Console.WriteLine("Result: " + Result);*/

            return Result;
        }

        public bool VerifyReferences(EXEScope Scope, CDClassPool ExecutionSpace)
        {
            bool Result = false;
            if (!EXETypes.ReferenceTypeName.Equals(EXETypes.DetermineVariableType("", this.Value)))
            {
                Result = true;
            }
            else
            {
                Result = Scope.VariableNameExists(this.Value);
            }
            return Result;
        }

        //https://stackoverflow.com/questions/1649027/how-do-i-print-out-a-tree-structure
        public void PrintPretty(string indent, bool last)
        {
            Console.Write(indent);
            if (last)
            {
                Console.Write("\\-");
                indent += "  ";
            }
            else
            {
                Console.Write("|-");
                indent += "| ";
            }
            Console.WriteLine(this.Value);
        }

        public List<string> AccessChain()
        {
            return new List<string>(new string[] { this.Value });
        }

        public bool IsReference()
        {
            return EXETypes.ReferenceTypeName.Equals(EXETypes.DetermineVariableType("", this.Value));
        }

        public string ToCode()
        {
            return this.Value;
        }
    }
}
